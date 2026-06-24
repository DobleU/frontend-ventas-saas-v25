using webVentasSaaSV25.Services.Http;
using webVentasSaaSV25.State;

namespace webVentasSaaSV25.Services.Sec;

public sealed class MenuClienteService(ApiClient api, AppState estado)
{
    private bool _cargando;

    public async Task<(IReadOnlyList<MenuNodo> Items, string? Error)> CargarAsync(
        bool force = false, CancellationToken ct = default)
    {
        if (!force && estado.MenuPrincipal.Count > 0)
            return (estado.MenuPrincipal, null);

        if (_cargando)
            return (estado.MenuPrincipal, null);

        _cargando = true;
        try
        {
            var (items, error) = await api.GetAsync<List<MenuNodo>>("api/v1/menu?canal=WEB");
            if (error is not null)
                return (estado.MenuPrincipal, error);

            estado.SetMenuPrincipal(items ?? new List<MenuNodo>());
            return (estado.MenuPrincipal, null);
        }
        finally
        {
            _cargando = false;
        }
    }

    public void Limpiar() => estado.SetMenuPrincipal(Array.Empty<MenuNodo>());
}
