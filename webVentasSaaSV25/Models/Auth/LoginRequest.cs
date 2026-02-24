// Models/Auth/LoginRequest.cs — VentasSaaSDU.Web

namespace webVentasSaaSV25.Models.Auth;

/// <summary>
/// Payload que se envía al endpoint POST /api/v1/auth/login
/// </summary>
public sealed class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int IdEmpresa { get; set; } = 1;
    public string? DeviceId { get; set; } = null;
}
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Respuesta completa del login.
/// Mapea exactamente lo que devuelve sec.usp_sec_Login vía la API.
/// </summary>
public sealed class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpira { get; set; }
    public int IdUsuario { get; set; }
    public int IdEmpresa { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string CodigoRol { get; set; } = string.Empty;
    public string NombreRol { get; set; } = string.Empty;
    public bool RequiereCambioPass { get; set; }
    public string EstadoSuscripcion { get; set; } = string.Empty;

    /// <summary>
    /// Diccionario "modulo:accion" → true/false.
    /// Ej: { "ventas_tienda:ver": true, "ventas_tienda:crear": false }
    /// </summary>
    public Dictionary<string, bool> Permisos { get; set; } = new();
}

// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Payload para el endpoint POST /api/v1/auth/refresh
/// </summary>
public sealed class RefreshRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Respuesta estándar de la API: { success, data, error }
/// </summary>
public sealed class ApiResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
}