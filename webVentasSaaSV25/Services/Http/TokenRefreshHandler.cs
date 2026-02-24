// Services/Http/TokenRefreshHandler.cs — VentasSaaSDU.Web
// DelegatingHandler que:
//   1. Adjunta Bearer token en cada request saliente.
//   2. Si recibe 401, intenta refresh silencioso UNA sola vez.
//   3. Si el refresh falla, dispara evento para redirigir al login.

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using webVentasSaaSV25.Models.Auth;
using webVentasSaaSV25.Services.Auth;
using webVentasSaaSV25.State;

namespace webVentasSaaSV25.Services.Http;

public sealed class TokenRefreshHandler : DelegatingHandler
{
    private readonly AuthService _authService;

    // Evento para notificar al componente raíz que la sesión expiró
    public static event Func<Task>? OnSesionExpirada;

    public TokenRefreshHandler(AuthService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // 1. Adjuntar token actual
        var token = await _authService.ObtenerAccessTokenAsync();
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 2. Enviar request original
        var response = await base.SendAsync(request, cancellationToken);

        // 3. Si 401 → intentar refresh y reintentar una vez
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var nuevoToken = await _authService.RefreshAsync();

            if (!string.IsNullOrEmpty(nuevoToken))
            {
                // Clonar el request (no se puede reusar el original)
                var retryRequest = await CloneRequestAsync(request);
                retryRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", nuevoToken);
                response = await base.SendAsync(retryRequest, cancellationToken);
            }
            else
            {
                // Refresh falló → sesión expirada
                if (OnSesionExpirada is not null)
                    await OnSesionExpirada.Invoke();
            }
        }

        return response;
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage original)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri);

        if (original.Content is not null)
        {
            var content = await original.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(content);
            foreach (var header in original.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        foreach (var header in original.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        return clone;
    }
}