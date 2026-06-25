// Services/Auth/AuthService.cs — VentasSaaSDU.Web
// Gestiona el ciclo completo de autenticación WEB:
//   Login → almacena tokens → Refresh silencioso → Logout
// Contrato WEB definitivo: POST /api/v1/auth/login usa Email + Password.

using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json;
using webVentasSaaSV25.Models.Auth;
using webVentasSaaSV25.State;

namespace webVentasSaaSV25.Services.Auth;

public sealed class AuthService
{
    // Claves en localStorage (prefijo para evitar colisiones)
    private const string KeyAccess = "ventassaas_access_token";
    private const string KeyRefresh = "ventassaas_refresh_token";
    private const string KeyUserCtx = "ventassaas_user_context";

    private readonly IHttpClientFactory _httpFactory;
    private readonly AppState _appState;
    private readonly PermisoClienteService _permisos;
    private readonly IJSRuntime _js;

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AuthService(
        IHttpClientFactory httpFactory,
        AppState appState,
        PermisoClienteService permisos,
        IJSRuntime js)
    {
        _httpFactory = httpFactory;
        _appState = appState;
        _permisos = permisos;
        _js = js;
    }

    // ─── LOGIN WEB ───────────────────────────────────────────────────────

    /// <summary>
    /// Autentica al usuario WEB contra POST /api/v1/auth/login.
    /// La API ya no recibe IdEmpresa ni DeviceId; solo Email + Password.
    /// Si tiene éxito: persiste tokens, carga estado global y permisos.
    /// </summary>
    public async Task<(bool Ok, string? Error)> LoginAsync(LoginRequest request)
    {
        try
        {
            var http = _httpFactory.CreateClient("VentasSaaSPublic");

            request.Email = request.Email.Trim().ToLowerInvariant();

            var response = await http.PostAsJsonAsync("api/v1/auth/login", request);

            if (!response.IsSuccessStatusCode)
            {
                var errBody = await response.Content.ReadAsStringAsync();
                var errResult = SafeDeserialize<ApiResult<object>>(errBody);
                var error = errResult?.Error ?? "Credenciales invalidas.";

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized &&
                    error.Contains("Usuario o contrase", StringComparison.OrdinalIgnoreCase))
                {
                    error = "Usuario/correo o contrasena incorrectos. Verifica que estes usando el acceso correcto.";
                }

                return (false, error);
            }

            var body = await response.Content.ReadAsStringAsync();
            var result = SafeDeserialize<ApiResult<LoginResponse>>(body);

            if (result is null || !result.Success || result.Data is null)
                return (false, result?.Error ?? "Error desconocido en el servidor.");

            await PersistirSesionAsync(result.Data);
            return (true, null);
        }
        catch (HttpRequestException)
        {
            return (false, "No se puede conectar con el servidor. Verifique su conexión.");
        }
        catch (Exception ex)
        {
            return (false, $"Error inesperado: {ex.Message}");
        }
    }

    // ─── REFRESH ─────────────────────────────────────────────────────────

    /// <summary>
    /// Renueva el access token usando el refresh token almacenado.
    /// Llamado automáticamente por TokenRefreshHandler cuando recibe 401.
    /// </summary>
    public async Task<string?> RefreshAsync()
    {
        try
        {
            var refreshToken = await _js.InvokeAsync<string?>("AppInterop.localStorageGet", KeyRefresh);
            if (string.IsNullOrEmpty(refreshToken))
                return null;

            var http = _httpFactory.CreateClient("VentasSaaSPublic");
            var payload = new RefreshRequest { RefreshToken = refreshToken };

            var response = await http.PostAsJsonAsync("api/v1/auth/refresh", payload);
            if (!response.IsSuccessStatusCode)
            {
                await LimpiarSesionLocalAsync();
                return null;
            }

            var body = await response.Content.ReadAsStringAsync();
            var result = SafeDeserialize<ApiResult<LoginResponse>>(body);

            if (result is null || !result.Success || result.Data is null)
            {
                await LimpiarSesionLocalAsync();
                return null;
            }

            await PersistirSesionAsync(result.Data);
            return result.Data.AccessToken;
        }
        catch
        {
            await LimpiarSesionLocalAsync();
            return null;
        }
    }

    // ─── LOGOUT ──────────────────────────────────────────────────────────

    /// <summary>
    /// Cierra la sesión: revoca token en servidor y limpia localStorage.
    /// </summary>
    public async Task LogoutAsync()
    {
        try
        {
            var accessToken = await ObtenerAccessTokenAsync();
            if (!string.IsNullOrEmpty(accessToken))
            {
                var http = _httpFactory.CreateClient("VentasSaaSPublic");
                http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                await http.PostAsync("api/v1/auth/logout", null);
            }
        }
        catch
        {
            // Ignorar errores de red en logout; la limpieza local siempre debe ejecutarse.
        }
        finally
        {
            await LimpiarSesionLocalAsync();
        }
    }

    // ─── RESTAURAR SESIÓN AL RECARGAR ────────────────────────────────────

    /// <summary>
    /// Intenta restaurar la sesión desde localStorage al iniciar la app.
    /// Si el access token no ha expirado, carga el estado sin llamar a la API.
    /// </summary>
    public async Task IntentarRestaurarSesionAsync()
    {
        try
        {
            var userCtxJson = await _js.InvokeAsync<string?>("AppInterop.localStorageGet", KeyUserCtx);
            if (string.IsNullOrEmpty(userCtxJson)) return;

            var loginResponse = SafeDeserialize<LoginResponse>(userCtxJson);
            if (loginResponse is null) return;

            if (loginResponse.AccessTokenExpira > DateTime.UtcNow.AddSeconds(30))
            {
                _appState.SetUsuario(loginResponse);
                _permisos.Cargar(loginResponse.Permisos);
            }
            else
            {
                var nuevoToken = await RefreshAsync();
                if (nuevoToken is null)
                    await LimpiarSesionLocalAsync();
            }
        }
        catch
        {
            await LimpiarSesionLocalAsync();
        }
    }

    // ─── HELPERS ─────────────────────────────────────────────────────────

    public async Task<string?> ObtenerAccessTokenAsync()
    {
        var token = await _js.InvokeAsync<string?>("AppInterop.localStorageGet", KeyAccess);
        if (string.IsNullOrWhiteSpace(token))
            return null;

        if (EsAccessTokenValido(token))
            return token;

        await LimpiarSesionLocalAsync();
        return null;
    }

    private async Task PersistirSesionAsync(LoginResponse data)
    {
        if (!EsAccessTokenValido(data.AccessToken))
            throw new InvalidOperationException("Token de acceso invalido o demasiado grande.");

        await _js.InvokeVoidAsync("AppInterop.localStorageSet", KeyAccess, data.AccessToken);
        await _js.InvokeVoidAsync("AppInterop.localStorageSet", KeyRefresh, data.RefreshToken);

        var json = JsonSerializer.Serialize(data, _jsonOpts);
        await _js.InvokeVoidAsync("AppInterop.localStorageSet", KeyUserCtx, json);

        _appState.SetUsuario(data);
        _permisos.Cargar(data.Permisos);
    }

    private async Task LimpiarSesionLocalAsync()
    {
        await _js.InvokeVoidAsync("AppInterop.localStorageRemove", KeyAccess);
        await _js.InvokeVoidAsync("AppInterop.localStorageRemove", KeyRefresh);
        await _js.InvokeVoidAsync("AppInterop.localStorageRemove", KeyUserCtx);
        _appState.LimpiarSesion();
        _permisos.Limpiar();
    }

    private static T? SafeDeserialize<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(json, _jsonOpts);
        }
        catch
        {
            return default;
        }
    }

    private static bool EsAccessTokenValido(string token)
    {
        if (string.IsNullOrWhiteSpace(token) || token.Length > 6000)
            return false;

        return token.Count(c => c == '.') == 2;
    }
}
