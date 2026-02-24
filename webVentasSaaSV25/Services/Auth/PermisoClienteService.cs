// Services/Auth/PermisoClienteService.cs — VentasSaaSDU.Web
// Servicio singleton que expone los permisos del usuario autenticado
// para controlar visibilidad en UI. La validación REAL está siempre en el backend.

namespace webVentasSaaSV25.Services.Auth;

/// <summary>
/// Gestiona los permisos efectivos del usuario en el cliente.
/// Se carga una sola vez al hacer login desde el JWT/loginResponse.
/// Propósito: UX (mostrar/ocultar elementos). NO reemplaza validación del backend.
/// </summary>
public sealed class PermisoClienteService
{
    private Dictionary<string, bool> _permisos = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Carga los permisos desde el diccionario del LoginResponse.
    /// Llamar inmediatamente después de un login o refresh exitoso.
    /// </summary>
    public void Cargar(Dictionary<string, bool> permisos)
    {
        _permisos = new Dictionary<string, bool>(permisos, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Limpia los permisos al hacer logout.
    /// </summary>
    public void Limpiar() => _permisos.Clear();

    /// <summary>
    /// Verifica si el usuario tiene un permiso específico.
    /// </summary>
    /// <param name="modulo">Código del módulo. Ej: "ventas_tienda"</param>
    /// <param name="accion">Código de la acción. Ej: "crear", "ver", "ver_costo"</param>
    /// <returns>true si el permiso está presente y habilitado</returns>
    public bool Tiene(string modulo, string accion)
    {
        var key = $"{modulo}:{accion}";
        return _permisos.TryGetValue(key, out var valor) && valor;
    }

    /// <summary>
    /// Verifica si el usuario tiene al menos un permiso en el módulo dado.
    /// Útil para mostrar/ocultar entradas del menú.
    /// </summary>
    /// <param name="modulo">Código del módulo. Ej: "inventario"</param>
    public bool TieneAlgunoEn(string modulo)
    {
        var prefijo = $"{modulo}:";
        return _permisos.Any(p =>
            p.Key.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase) && p.Value);
    }

    /// <summary>
    /// Obtiene todos los permisos cargados. Solo para depuración.
    /// </summary>
    public IReadOnlyDictionary<string, bool> TodosLosPermisos() =>
        _permisos.AsReadOnly();
}