// Services/Auth/AuthService.cs — VentasSaaSDU.Web
// Gestiona el ciclo completo de autenticación:
//   Login → almacena tokens → Refresh silencioso → Logout

using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json;
using webVentasSaaSV25.Models.Auth;
using webVentasSaaSV25.State;
using webVentasSaaSV25.Services.Auth;

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

    // Opciones de JSON que coinciden con la API (.NET camelCase o PascalCase)
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

    // ─── LOGIN ───────────────────────────────────────────────────────────

    /// <summary>
    /// Autentica al usuario contra POST /api/v1/auth/login.
    /// Si tiene éxito: persiste tokens, carga estado global y permisos.
    /// </summary>
    /// <returns>(exito, mensajeError)</returns>
    public async Task<(bool Ok, string? Error)> LoginAsync(LoginRequest request)
    {
        try
        {
            var http = _httpFactory.CreateClient("VentasSaaSPublic");

            var response = await http.PostAsJsonAsync("api/v1/auth/login", request);

            if (!response.IsSuccessStatusCode)
            {
                // 401, 403: credenciales inválidas o empresa suspendida
                var errBody = await response.Content.ReadAsStringAsync();
                var errResult = JsonSerializer.Deserialize<ApiResult<object>>(errBody, _jsonOpts);
                return (false, errResult?.Error ?? "Credenciales inválidas.");
            }

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResult<LoginResponse>>(body, _jsonOpts);

            if (result is null || !result.Success || result.Data is null)
                return (false, result?.Error ?? "Error desconocido en el servidor.");

            // Persistir tokens y contexto
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
    /// <returns>Nuevo access token o null si el refresh falló</returns>
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
            var result = JsonSerializer.Deserialize<ApiResult<LoginResponse>>(body, _jsonOpts);

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
                // POST logout — ignora fallo de red (limpieza local es suficiente)
                await http.PostAsync("api/v1/auth/logout", null);
            }
        }
        catch { /* Ignorar errores de red en logout */ }
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

            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(userCtxJson, _jsonOpts);
            if (loginResponse is null) return;

            // Verificar si el access token aún es válido
            if (loginResponse.AccessTokenExpira > DateTime.UtcNow.AddSeconds(30))
            {
                // Token aún válido: restaurar estado sin llamar a la API
                _appState.SetUsuario(loginResponse);
                _permisos.Cargar(loginResponse.Permisos);
            }
            else
            {
                // Token expirado: intentar refresh
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

    public async Task<string?> ObtenerAccessTokenAsync() =>
        await _js.InvokeAsync<string?>("AppInterop.localStorageGet", KeyAccess);

    private async Task PersistirSesionAsync(LoginResponse data)
    {
        // Guardar tokens
        await _js.InvokeVoidAsync("AppInterop.localStorageSet", KeyAccess, data.AccessToken);
        await _js.InvokeVoidAsync("AppInterop.localStorageSet", KeyRefresh, data.RefreshToken);

        // Guardar contexto completo (para restaurar al recargar sin hacer login)
        var json = JsonSerializer.Serialize(data);
        await _js.InvokeVoidAsync("AppInterop.localStorageSet", KeyUserCtx, json);

        // Actualizar estado global y permisos
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
}