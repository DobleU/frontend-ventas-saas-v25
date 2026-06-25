using webVentasSaaSV25.Services.Http;
using webVentasSaaSV25.State;

namespace webVentasSaaSV25.Services.Sec;

public sealed class MenuClienteService(ApiClient api, AppState estado)
{
    private bool _cargando;

    private static readonly Dictionary<string, string> RutasPorCodigo = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CRM.CLIENTES"] = "/catalogos/clientes",
        ["CRM.PROSPECTOS"] = "/crm/prospectos",
        ["CRM.SEGUIMIENTO_COMERCIAL"] = "/crm/seguimiento-comercial",
        ["CRM.ESCENARIOS_COMERCIALES"] = "/crm/escenarios-comerciales",

        ["POS.VENTAS"] = "/pos/ventas",
        ["POS.CAJA"] = "/pos/caja",
        ["POS.TICKETS"] = "/pos/tickets",

        ["ROUTE.RECORRIDOS"] = "/ventas/ruta/recorridos",
        ["ROUTE.RECORRIDOS.RECORRIDOS_ACTIVOS"] = "/ventas/ruta/recorridos?estatus=51",
        ["ROUTE.RECORRIDOS.RECORRIDOS_CERRADOS"] = "/ventas/ruta/recorridos?estatus=52",
        ["ROUTE.RECORRIDOS.RECORRIDOS_CONCLUIDOS"] = "/ventas/ruta/recorridos?estatus=52",
        ["ROUTE.VISITAS"] = "/ventas/ruta/visitas",
        ["ROUTE.VISITAS.VISITAS_REALIZADAS"] = "/ventas/ruta/visitas?modo=realizadas",
        ["ROUTE.VISITAS.VISITAS_NO_REALIZADAS"] = "/ventas/ruta/visitas?modo=no-realizadas",
        ["ROUTE.VISITAS.VISITAS_EXTRAS"] = "/ventas/ruta/visitas?modo=extras",
        ["ROUTE.VISITAS.VISITAS_REPETIDAS"] = "/ventas/ruta/visitas?modo=repetidas",
        ["ROUTE.VISITAS.EVIDENCIA_DE_VISITA"] = "/ventas/ruta/visitas?modo=evidencias",
        ["ROUTE.EVENTOS"] = "/ventas/ruta/eventos",
        ["ROUTE.CLIENTES_RUTA"] = "/route/clientes-ruta",
        ["ROUTE.CLIENTES_POR_RUTA"] = "/route/clientes-ruta",
        ["ROUTE.PLANEACION_VISITAS"] = "/route/planeacion-visitas",
        ["ROUTE.VISITAS.PLANEACION_DE_VISITAS"] = "/route/planeacion-visitas",
        ["ROUTE.LIQUIDACION_RUTA"] = "/route/liquidacion-ruta",
        ["ROUTE.LIQUIDACION_DE_RUTA"] = "/route/liquidacion-ruta",
        ["ROUTE.ESCENARIOS_COMERCIALES"] = "/route/escenarios-comerciales",
        ["ROUTE.ESCENARIOS_EN_APP"] = "/route/escenarios-comerciales",
        ["ROUTE.SINCRONIZACION_APP"] = "/route/sincronizacion-app",

        ["ERP.COMPRAS"] = "/erp/compras",
        ["ERP.PROMOCIONES_CAMPANAS"] = "/erp/promociones-campanas",
        ["ERP.RECURSOS_HUMANOS"] = "/erp/rh",
        ["ERP.RH"] = "/erp/rh",
        ["ERP.PROVEEDORES"] = "/erp/proveedores",
        ["ERP.ORDENES_COMPRA"] = "/erp/ordenes-compra",
        ["ERP.RECEPCION_MERCANCIA"] = "/erp/recepcion-mercancia",
        ["ERP.ALMACENES"] = "/almacen/almacenes",
        ["ERP.ALMACENES.CATALOGO_DE_ALMACENES"] = "/almacen/almacenes",
        ["ERP.ALMACENES.SUCURSALES"] = "/core/sucursales",
        ["ERP.ALMACENES.VEHICULOS"] = "/almacen/vehiculos",
        ["ERP.ALMACENES.MOVIMIENTOS_DE_ALMACEN"] = "/reportes/inventario/movimientos",

        ["ALMACEN.ALMACENES"] = "/almacen/almacenes",
        ["ALMACEN.CATALOGO_DE_ALMACENES"] = "/almacen/almacenes",
        ["ALMACEN.SUCURSALES"] = "/core/sucursales",
        ["ALMACEN.VEHICULOS"] = "/almacen/vehiculos",
        ["ALMACEN.MOVIMIENTOS"] = "/reportes/inventario/movimientos",
        ["ALMACEN.MOVIMIENTOS_DE_ALMACEN"] = "/reportes/inventario/movimientos",

        ["CORE.EMPRESAS"] = "/core/empresas",
        ["CORE.SUCURSALES"] = "/core/sucursales",
        ["CORE.ZONAS"] = "/core/zonas",
        ["CORE.RUTAS"] = "/core/rutas",
        ["CORE.PRODUCTOS"] = "/catalogos/productos",
        ["CORE.PRECIOS"] = "/core/precios",
        ["CORE.MONEDAS"] = "/core/monedas",
        ["CORE.SERIES_DOCUMENTO"] = "/core/series-documento",
        ["CORE.SERIES_DE_DOCUMENTO"] = "/core/series-documento",
        ["CORE.PARAMETROS"] = "/core/parametros",
        ["CORE.CATALOGOS_TIPO"] = "/catalogos/tipos",
        ["CORE.CATALOGOS_ITEM"] = "/catalogos/items",
        ["CORE.CLASIFICACIONES"] = "/catalogos/clasificaciones",
        ["CORE.UNIDADES"] = "/catalogos/unidades",
        ["CORE.IMPUESTOS"] = "/catalogos/impuestos",

        ["MONITOR.MAPA_GENERAL"] = "/monitor/mapa",
        ["MONITOR.MONITOR_RECORRIDO"] = "/monitor/rutas",
        ["MONITOR.MONITOR_CLIENTES"] = "/monitor/clientes",
        ["MONITOR.MONITOR_POS"] = "/monitor/pos",

        ["BI_REP.VENTAS"] = "/reportes/ventas",
        ["BI_REP.CLIENTES"] = "/reportes/clientes",
        ["BI_REP.RUTA"] = "/reportes/ruta",
        ["BI_REP.INVENTARIO"] = "/reportes/inventario/movimientos",
        ["BI_REP.UTILIDAD_MARGEN"] = "/reportes/utilidad-margen",
        ["REPORTES.VENTAS"] = "/reportes/ventas",
        ["REPORTES.CLIENTES"] = "/reportes/clientes",
        ["REPORTES.RUTA"] = "/reportes/ruta",
        ["REPORTES.INVENTARIO"] = "/reportes/inventario/movimientos",
        ["REPORTES.UTILIDAD_MARGEN"] = "/reportes/utilidad-margen",

        ["FINANZAS.CXC"] = "/finanzas/cxc",
        ["FINANZAS.CXP"] = "/finanzas/cxp",
        ["FINANZAS.BANCOS"] = "/finanzas/bancos",
        ["FINANZAS.CONCILIACION"] = "/finanzas/conciliacion",
        ["FINANZAS.REPORTES_FINANCIEROS"] = "/finanzas/reportes-financieros",

        ["FACTURACION.VENTAS"] = "/facturacion/ventas",
        ["FACTURACION.GLOBAL"] = "/facturacion/global",
        ["FACTURACION.CANCELACIONES"] = "/facturacion/cancelaciones",

        ["CONF.DATOS_EMPRESA"] = "/config/datos-empresa",
        ["CONF.MI_EMPRESA"] = "/config/datos-empresa",
        ["CONF.PARAMETROS_GENERALES"] = "/config/parametros-generales",
        ["CONF.PARAMETROS_VENTA"] = "/config/parametros-venta",
        ["CONF.PARAMETROS_DE_VENTA"] = "/config/parametros-venta",
        ["CONF.PARAMETROS_RUTA"] = "/config/parametros-ruta",
        ["CONF.PARAMETROS_DE_RUTA"] = "/config/parametros-ruta",
        ["CONF.PARAMETROS_INVENTARIO"] = "/config/parametros-inventario",
        ["CONF.PARAMETROS_DE_INVENTARIO"] = "/config/parametros-inventario",
        ["CONF.PARAMETROS_CREDITO"] = "/config/parametros-credito",
        ["CONF.PARAMETROS_DE_CREDITO"] = "/config/parametros-credito",
        ["CONF.PARAMETROS_FACTURACION"] = "/config/parametros-facturacion",
        ["CONF.PARAMETROS_DE_FACTURACION"] = "/config/parametros-facturacion",
        ["CONF.REGLAS_COMERCIALES"] = "/config/reglas-comerciales",
        ["CONF.ALERTAS"] = "/config/alertas-sistema",
        ["CONF.ALERTAS_SISTEMA"] = "/config/alertas-sistema",
        ["CONF.CONFIGURACION_APP_MOVIL"] = "/config/app-movil",
        ["CONF.CONFIGURACION_DE_AUDIO"] = "/config/audio",
        ["CONF.CONFIGURACION_DE_IA"] = "/config/ia",
        ["CONF.CATALOGOS_INTERNOS"] = "/config/catalogos-generales",

        ["ADM.EMPLEADOS"] = "/seguridad/empleados",
        ["ADM.USUARIOS"] = "/seguridad/usuarios",
        ["ADM.ROLES"] = "/seguridad/roles",
        ["ADM.SESIONES"] = "/seguridad/sesiones",
        ["ADM.DISPOSITIVOS_MOVILES"] = "/admin/dispositivos-moviles",
        ["ADM.AUDITORIA"] = "/admin/auditoria",
        ["ADM.BITACORA_ERRORES"] = "/admin/bitacora-errores",
        ["ADM.SUSCRIPCION"] = "/configuracion/suscripcion"
    };

    private static readonly (string Prefijo, string Ruta)[] RutasPorPrefijo =
    [
        ("CRM.CLIENTES.", "/catalogos/clientes"),
        ("CRM.PROSPECTOS.", "/crm/prospectos"),
        ("CRM.SEGUIMIENTO_COMERCIAL.", "/crm/seguimiento-comercial"),
        ("CRM.ESCENARIOS_COMERCIALES.", "/crm/escenarios-comerciales"),
        ("POS.VENTAS.", "/pos/ventas"),
        ("POS.CAJA.", "/pos/caja"),
        ("POS.TICKETS.", "/pos/tickets"),
        ("ROUTE.RECORRIDOS.", "/ventas/ruta/recorridos"),
        ("ROUTE.VISITAS.", "/ventas/ruta/visitas"),
        ("ROUTE.EVENTOS.", "/ventas/ruta/eventos"),
        ("ROUTE.CLIENTES_RUTA.", "/route/clientes-ruta"),
        ("ROUTE.CLIENTES_POR_RUTA.", "/route/clientes-ruta"),
        ("ROUTE.PLANEACION_VISITAS.", "/route/planeacion-visitas"),
        ("ROUTE.LIQUIDACION_RUTA.", "/route/liquidacion-ruta"),
        ("ROUTE.LIQUIDACION_DE_RUTA.", "/route/liquidacion-ruta"),
        ("ROUTE.ESCENARIOS_COMERCIALES.", "/route/escenarios-comerciales"),
        ("ROUTE.ESCENARIOS_EN_APP.", "/route/escenarios-comerciales"),
        ("ROUTE.SINCRONIZACION_APP.", "/route/sincronizacion-app"),
        ("ERP.COMPRAS.", "/erp/compras"),
        ("ERP.PROMOCIONES_CAMPANAS.", "/erp/promociones-campanas"),
        ("ERP.RECURSOS_HUMANOS.", "/erp/rh"),
        ("ERP.RH.", "/erp/rh"),
        ("ERP.PROVEEDORES.", "/erp/proveedores"),
        ("ERP.ORDENES_COMPRA.", "/erp/ordenes-compra"),
        ("ERP.RECEPCION_MERCANCIA.", "/erp/recepcion-mercancia"),
        ("ERP.ALMACENES.CATALOGO_DE_ALMACENES.", "/almacen/almacenes"),
        ("ERP.ALMACENES.SUCURSALES.", "/core/sucursales"),
        ("ERP.ALMACENES.VEHICULOS.", "/almacen/vehiculos"),
        ("ERP.ALMACENES.MOVIMIENTOS_DE_ALMACEN.", "/reportes/inventario/movimientos"),
        ("ALMACEN.ALMACENES.", "/almacen/almacenes"),
        ("ALMACEN.SUCURSALES.", "/core/sucursales"),
        ("ALMACEN.VEHICULOS.", "/almacen/vehiculos"),
        ("ALMACEN.MOVIMIENTOS_DE_ALMACEN.", "/reportes/inventario/movimientos"),
        ("CORE.EMPRESAS.", "/core/empresas"),
        ("CORE.SUCURSALES.", "/core/sucursales"),
        ("CORE.ZONAS.", "/core/zonas"),
        ("CORE.RUTAS.", "/core/rutas"),
        ("CORE.PRODUCTOS.", "/catalogos/productos"),
        ("CORE.PRECIOS.", "/core/precios"),
        ("CORE.MONEDAS.", "/core/monedas"),
        ("CORE.SERIES_DOCUMENTO.", "/core/series-documento"),
        ("CORE.SERIES_DE_DOCUMENTO.", "/core/series-documento"),
        ("CORE.PARAMETROS.", "/core/parametros"),
        ("CORE.CATALOGOS_TIPO.", "/catalogos/tipos"),
        ("CORE.CATALOGOS_ITEM.", "/catalogos/items"),
        ("CORE.CLASIFICACIONES.", "/catalogos/clasificaciones"),
        ("CORE.UNIDADES.", "/catalogos/unidades"),
        ("CORE.IMPUESTOS.", "/catalogos/impuestos"),
        ("MONITOR.MAPA_GENERAL.", "/monitor/mapa"),
        ("MONITOR.MONITOR_RECORRIDO.", "/monitor/rutas"),
        ("MONITOR.MONITOR_CLIENTES.", "/monitor/clientes"),
        ("MONITOR.MONITOR_POS.", "/monitor/pos"),
        ("BI_REP.VENTAS.", "/reportes/ventas"),
        ("BI_REP.CLIENTES.", "/reportes/clientes"),
        ("BI_REP.RUTA.", "/reportes/ruta"),
        ("BI_REP.INVENTARIO.", "/reportes/inventario/movimientos"),
        ("BI_REP.UTILIDAD_MARGEN.", "/reportes/utilidad-margen"),
        ("REPORTES.VENTAS.", "/reportes/ventas"),
        ("REPORTES.CLIENTES.", "/reportes/clientes"),
        ("REPORTES.RUTA.", "/reportes/ruta"),
        ("REPORTES.INVENTARIO.", "/reportes/inventario/movimientos"),
        ("REPORTES.UTILIDAD_MARGEN.", "/reportes/utilidad-margen"),
        ("FINANZAS.CXC.", "/finanzas/cxc"),
        ("FINANZAS.CXP.", "/finanzas/cxp"),
        ("FINANZAS.BANCOS.", "/finanzas/bancos"),
        ("FINANZAS.CONCILIACION.", "/finanzas/conciliacion"),
        ("FINANZAS.REPORTES_FINANCIEROS.", "/finanzas/reportes-financieros"),
        ("FACTURACION.VENTAS.", "/facturacion/ventas"),
        ("FACTURACION.GLOBAL.", "/facturacion/global"),
        ("FACTURACION.CANCELACIONES.", "/facturacion/cancelaciones"),
        ("CONF.", "/config/parametros-generales"),
        ("ADM.EMPLEADOS.", "/seguridad/empleados"),
        ("ADM.USUARIOS.", "/seguridad/usuarios"),
        ("ADM.ROLES.", "/seguridad/roles"),
        ("ADM.SESIONES.", "/seguridad/sesiones"),
        ("ADM.DISPOSITIVOS_MOVILES.", "/admin/dispositivos-moviles"),
        ("ADM.AUDITORIA.", "/admin/auditoria"),
        ("ADM.BITACORA_ERRORES.", "/admin/bitacora-errores"),
        ("ADM.SUSCRIPCION.", "/configuracion/suscripcion")
    ];

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

            var menu = items ?? new List<MenuNodo>();
            NormalizarRutas(menu);
            estado.SetMenuPrincipal(menu);
            return (estado.MenuPrincipal, null);
        }
        finally
        {
            _cargando = false;
        }
    }

    public void Limpiar() => estado.SetMenuPrincipal(Array.Empty<MenuNodo>());

    private static void NormalizarRutas(IEnumerable<MenuNodo> nodos)
    {
        foreach (var nodo in nodos)
        {
            nodo.RutaWeb = ResolverRuta(nodo);
            NormalizarRutas(nodo.Hijos);
        }
    }

    private static string? ResolverRuta(MenuNodo nodo)
    {
        if (string.IsNullOrWhiteSpace(nodo.Codigo))
            return NormalizarRuta(nodo.RutaWeb);

        if (RutasPorCodigo.TryGetValue(nodo.Codigo, out var rutaExacta))
            return rutaExacta;

        foreach (var (prefijo, ruta) in RutasPorPrefijo)
        {
            if (nodo.Codigo.StartsWith(prefijo, StringComparison.OrdinalIgnoreCase))
                return ruta;
        }

        return NormalizarRuta(nodo.RutaWeb);
    }

    private static string? NormalizarRuta(string? ruta)
    {
        if (string.IsNullOrWhiteSpace(ruta))
            return null;

        ruta = ruta.Trim();
        return ruta.StartsWith('/') ? ruta : $"/{ruta}";
    }
}
