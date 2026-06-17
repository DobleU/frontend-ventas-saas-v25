// Models/Auth/LoginRequest.cs — VentasSaaSDU.Web
// Login WEB definitivo: el usuario entra con correo + contraseña.
// La empresa se resuelve en la API/BD desde sec.Usuario.email.

using System.ComponentModel.DataAnnotations;

namespace webVentasSaaSV25.Models.Auth;

/// <summary>
/// Payload que se envía al endpoint POST /api/v1/auth/login.
/// Nuevo contrato WEB: ya no se envía IdEmpresa ni DeviceId.
/// </summary>
public sealed class LoginRequest
{
    [Required(ErrorMessage = "Usuario es obligatorio.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get; set; } = string.Empty;
}

// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Respuesta completa del login.
/// Mapea lo que devuelve la API en LoginResponse.
/// </summary>
public sealed class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpira { get; set; }
    public int IdUsuario { get; set; }
    public int IdEmpresa { get; set; }
    public string CodigoRol { get; set; } = string.Empty;
    public string NombreRol { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public bool RequiereCambioPass { get; set; }
    public string EstadoSuscripcion { get; set; } = string.Empty;

    /// <summary>
    /// Compatibilidad con código Blazor existente que todavía use NombreUsuario.
    /// La API nueva devuelve NombreCompleto.
    /// </summary>
    public string NombreUsuario
    {
        get => NombreCompleto;
        set => NombreCompleto = value;
    }

    /// <summary>
    /// Diccionario "modulo:accion" → true/false.
    /// Ej: { "ventas_tienda:ver": true, "ventas_tienda:crear": false }
    /// </summary>
    public Dictionary<string, bool> Permisos { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Payload para el endpoint POST /api/v1/auth/refresh.
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
