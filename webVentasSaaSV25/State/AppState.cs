// State/AppState.cs — VentasSaaSDU.Web
// Estado global singleton: contiene el usuario autenticado, permisos y tenant.
// Registrado como Singleton en Program.cs.
// Los componentes suscriben al evento OnChange para reaccionar a cambios.

using webVentasSaaSV25.Models.Auth;
//using webVentasSaaSV25.Models.Responses.Auth;

namespace webVentasSaaSV25.State;

/// <summary>
/// Estado global de la sesión activa.
/// Accesible por inyección de dependencias en cualquier componente o servicio.
/// </summary>
public sealed class AppState
{
    // ─── Usuario autenticado ───────────────────────────────────────────────
    public LoginResponse? Usuario { get; private set; }

    // ─── Indicadores derivados ────────────────────────────────────────────
    public bool EstaAutenticado => Usuario is not null;

    public string NombreUsuario => Usuario?.NombreUsuario ?? string.Empty;
    public string NombreRol => Usuario?.NombreRol ?? string.Empty;
    public string CodigoRol => Usuario?.CodigoRol ?? string.Empty;
    public int IdEmpresa => Usuario?.IdEmpresa ?? 0;
    public string EstadoSusc => Usuario?.EstadoSuscripcion ?? string.Empty;

    /// <summary>
    /// Indica si la suscripción permite operación completa.
    /// Grace = puede operar pero con aviso de vencimiento próximo.
    /// </summary>
    public bool SuscripcionActiva =>
        EstadoSusc is "Active" or "Trial" or "Grace";

    // ─── Evento de cambio de estado ───────────────────────────────────────
    /// <summary>
    /// Se dispara cuando cambia el estado de autenticación.
    /// Los componentes (TopBar, NavMenu) se suscriben a este evento
    /// para re-renderizarse sin necesidad de polling.
    /// </summary>
    public event Action? OnChange;

    // ─── Métodos de mutación ──────────────────────────────────────────────

    /// <summary>
    /// Establece el usuario autenticado tras un login o refresh exitoso.
    /// </summary>
    public void SetUsuario(LoginResponse loginResponse)
    {
        Usuario = loginResponse;
        NotificarCambio();
    }

    /// <summary>
    /// Limpia el estado al hacer logout o expirar la sesión.
    /// </summary>
    public void LimpiarSesion()
    {
        Usuario = null;
        NotificarCambio();
    }

    private void NotificarCambio() => OnChange?.Invoke();
}