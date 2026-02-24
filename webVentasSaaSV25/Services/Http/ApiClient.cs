

// ─────────────────────────────────────────────────────────────────────────────

// Services/Http/ApiClient.cs — VentasSaaSDU.Web
// Wrapper del HttpClient con métodos genéricos para consumir la API.
// Todos los métodos manejan el envelope { success, data, error }.

using System.Net.Http.Json;
using System.Text.Json;
using webVentasSaaSV25.Models.Auth;
using webVentasSaaSV25.Services.Auth;
using webVentasSaaSV25.State;

namespace webVentasSaaSV25.Services.Http;

public sealed class ApiClient
{
    private readonly HttpClient _http;
    private readonly AuthService _authService;
    private readonly AppState _appState;

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiClient(HttpClient http, AuthService authService, AppState appState)
    {
        _http = http;
        _authService = authService;
        _appState = appState;
    }

    // ─── GET ──────────────────────────────────────────────────────────────

    /// <summary>
    /// GET genérico. Retorna (data, errorMsg).
    /// </summary>
    public async Task<(T? Data, string? Error)> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _http.GetAsync(endpoint);
            return await ProcesarRespuestaAsync<T>(response);
        }
        catch (HttpRequestException)
        {
            return (default, "Error de conexión con el servidor.");
        }
    }

    // ─── POST ─────────────────────────────────────────────────────────────

    /// <summary>
    /// POST genérico con body JSON.
    /// </summary>
    public async Task<(T? Data, string? Error)> PostAsync<T>(string endpoint, object payload)
    {
        try
        {
            var response = await _http.PostAsJsonAsync(endpoint, payload);
            return await ProcesarRespuestaAsync<T>(response);
        }
        catch (HttpRequestException)
        {
            return (default, "Error de conexión con el servidor.");
        }
    }

    // ─── PUT ──────────────────────────────────────────────────────────────

    public async Task<(T? Data, string? Error)> PutAsync<T>(string endpoint, object payload)
    {
        try
        {
            var response = await _http.PutAsJsonAsync(endpoint, payload);
            return await ProcesarRespuestaAsync<T>(response);
        }
        catch (HttpRequestException)
        {
            return (default, "Error de conexión con el servidor.");
        }
    }

    // ─── DELETE ───────────────────────────────────────────────────────────

    public async Task<(bool Ok, string? Error)> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _http.DeleteAsync(endpoint);
            var (data, error) = await ProcesarRespuestaAsync<object>(response);
            return (error is null, error);
        }
        catch (HttpRequestException)
        {
            return (false, "Error de conexión con el servidor.");
        }
    }

    // ─── HELPER ───────────────────────────────────────────────────────────

    private static async Task<(T? Data, string? Error)> ProcesarRespuestaAsync<T>(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();

        try
        {
            var result = JsonSerializer.Deserialize<ApiResult<T>>(body, _jsonOpts);

            if (result is null)
                return (default, "Respuesta inválida del servidor.");

            return result.Success
                ? (result.Data, null)
                : (default, result.Error ?? "Error desconocido.");
        }
        catch (JsonException)
        {
            return (default, $"Error al procesar la respuesta: {body[..Math.Min(body.Length, 200)]}");
        }
    }
}