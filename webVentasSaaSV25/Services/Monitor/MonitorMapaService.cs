using webVentasSaaSV25.Services.Http;
using webVentasSaaSV25.State;

namespace webVentasSaaSV25.Services.Monitor;

public sealed class MonitorMapaService(ApiClient api, AppState appState)
{
    public Task<(MapaConfiguracionResponse? D, string? E)> GetConfiguracionAsync()
        => api.GetAsync<MapaConfiguracionResponse>("api/v1/mapa/configuracion");

    public Task<(IEnumerable<MapaRutaOptionResponse>? D, string? E)> GetRutasAsync()
        => api.GetAsync<IEnumerable<MapaRutaOptionResponse>>("api/v1/mapa/rutas");

    public Task<(IEnumerable<MapaRecorridoOptionResponse>? D, string? E)> GetRecorridosAsync(int? idRuta, DateTime? fecha = null)
    {
        var queryParts = new List<string>();
        if (idRuta.HasValue && idRuta > 0) queryParts.Add($"idRuta={idRuta.Value}");
        if (fecha.HasValue) queryParts.Add($"fecha={fecha.Value:yyyy-MM-dd}");
        var query = queryParts.Count > 0 ? $"?{string.Join("&", queryParts)}" : "";
        return api.GetAsync<IEnumerable<MapaRecorridoOptionResponse>>($"api/v1/mapa/recorridos{query}");
    }

    public Task<(MapaRutaResponse? D, string? E)> GetRutaAsync(MapaRutaRequest request)
    {
        var query = $"modo={Uri.EscapeDataString(request.Modo ?? "ruta")}";
        if (request.IdRuta.HasValue) query += $"&idRuta={request.IdRuta.Value}";
        if (request.IdRecorrido.HasValue) query += $"&idRecorrido={request.IdRecorrido.Value}";
        if (request.FechaTrabajo.HasValue) query += $"&fechaTrabajo={request.FechaTrabajo.Value:yyyy-MM-dd}";
        if (!string.IsNullOrWhiteSpace(request.Search)) query += $"&search={Uri.EscapeDataString(request.Search)}";
        return api.GetAsync<MapaRutaResponse>($"api/v1/mapa/ruta?{query}");
    }

    public Task<(byte[]? D, string? ContentType, string? E)> GetClienteFotoAsync(int idCliente)
        => api.GetBytesAsync($"api/v1/mapa/clientes/{idCliente}/foto");

    public Task<(MonitorClienteResponse? D, string? E)> GetMonitorClienteAsync(
        int idCliente,
        int? idDireccion = null,
        int topProductos = 5,
        int mesesTendencia = 6)
    {
        var query = $"idCliente={idCliente}&topProductos={topProductos}&mesesTendencia={mesesTendencia}";
        if (idDireccion.HasValue && idDireccion > 0) query += $"&idDireccion={idDireccion.Value}";
        return api.GetAsync<MonitorClienteResponse>($"api/v1/mapa/clientes/detalle?{query}");
    }

    public Task<(ReporteRecorridosResponse? D, string? E)> GetReporteRecorridosAsync(
        int? idRuta,
        DateTime? fechaInicio,
        DateTime? fechaFin,
        int top = 20)
    {
        var query = $"top={top}";
        if (idRuta.HasValue && idRuta > 0) query += $"&idRuta={idRuta.Value}";
        if (fechaInicio.HasValue) query += $"&fechaInicio={fechaInicio.Value:yyyy-MM-dd}";
        if (fechaFin.HasValue) query += $"&fechaFin={fechaFin.Value:yyyy-MM-dd}";
        return api.GetAsync<ReporteRecorridosResponse>($"api/v1/mapa/reportes/recorridos?{query}");
    }

    public Task<(ReporteVisitasResponse? D, string? E)> GetReporteVisitasAsync(
        int idRuta,
        DateTime fecha,
        long? idRecorrido = null)
    {
        var query = $"idRuta={idRuta}&fecha={fecha:yyyy-MM-dd}";
        if (idRecorrido.HasValue && idRecorrido > 0) query += $"&idRecorrido={idRecorrido.Value}";
        return api.GetAsync<ReporteVisitasResponse>($"api/v1/mapa/reportes/visitas?{query}");
    }

    public Task<(ReporteEventosResponse? D, string? E)> GetReporteEventosAsync(
        DateTime? fechaInicio,
        DateTime? fechaFin,
        string? severidad,
        string? modulo,
        string? search,
        bool soloAbiertos = true,
        int top = 100)
    {
        var query = $"soloAbiertos={soloAbiertos.ToString().ToLowerInvariant()}&top={top}";
        if (fechaInicio.HasValue) query += $"&fechaInicio={fechaInicio.Value:yyyy-MM-dd}";
        if (fechaFin.HasValue) query += $"&fechaFin={fechaFin.Value:yyyy-MM-dd}";
        if (!string.IsNullOrWhiteSpace(severidad)) query += $"&severidad={Uri.EscapeDataString(severidad)}";
        if (!string.IsNullOrWhiteSpace(modulo)) query += $"&modulo={Uri.EscapeDataString(modulo)}";
        if (!string.IsNullOrWhiteSpace(search)) query += $"&search={Uri.EscapeDataString(search)}";
        return api.GetAsync<ReporteEventosResponse>($"api/v1/mapa/reportes/eventos?{query}");
    }

    public Task<(RecorridoTicketResponse? D, string? E)> GetRecorridoTicketAsync(long idRecorrido)
        => api.GetAsync<RecorridoTicketResponse>($"api/v1/app/recorrido/{idRecorrido}/ticket");

    public Task<(RecorridoCierreCompletoResponse? D, string? E)> GetRecorridoCierreCompletoAsync(long idRecorrido)
        => api.GetAsync<RecorridoCierreCompletoResponse>($"api/v1/app/recorrido/{idRecorrido}/cierre-completo?idEmpresa={appState.IdEmpresa}");

    public Task<(MonitorWriteResult? D, string? E)> RegistrarEventoAccionAsync(
        long idEvento,
        EventoAccionRequest request)
        => api.PostAsync<MonitorWriteResult>($"api/v1/mapa/reportes/eventos/{idEvento}/acciones", request);
}

public sealed class MapaRutaRequest
{
    public int? IdRuta { get; set; }
    public long? IdRecorrido { get; set; }
    public DateTime? FechaTrabajo { get; set; }
    public string? Search { get; set; }
    public string? Modo { get; set; } = "ruta";
}

public sealed class MapaConfiguracionResponse
{
    public string ProveedorMapa { get; init; } = "GOOGLE";
    public bool TieneApiKey { get; init; }
    public bool MostrarMapa { get; init; }
    public string? ApiKey { get; init; }
    public string MensajeConfiguracion { get; init; } = string.Empty;
}

public sealed class MapaRutaResponse
{
    public int? IdRuta { get; init; }
    public string NombreRuta { get; init; } = "Sin ruta";
    public string Modo { get; init; } = "ruta";
    public int TotalClientes { get; init; }
    public int Visitados { get; init; }
    public int Pendientes { get; init; }
    public int Alertas { get; init; }
    public IReadOnlyList<MapaClientePuntoResponse> Puntos { get; init; } = [];
}

public sealed class MapaClientePuntoResponse
{
    public int IdCliente { get; init; }
    public int? IdDireccion { get; init; }
    public int Orden { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? NombreRuta { get; init; }
    public string? Direccion { get; init; }
    public string? Telefono { get; init; }
    public string? Rfc { get; init; }
    public string Estado { get; init; } = "pendiente";
    public string Credito { get; init; } = "Sin credito";
    public string UltimaVisita { get; init; } = "Sin visita reciente";
    public decimal X { get; init; }
    public decimal Y { get; init; }
    public decimal? Latitud { get; init; }
    public decimal? Longitud { get; init; }
    public string GeoSource { get; init; } = "SIN_COORDENADA";
    public int GeoQuality { get; init; }
    public string? ProductoTop { get; init; }
    public int FrecuenciaSemanas { get; init; }
    public bool TieneImagen { get; init; }
    public string? UrlImagen { get; init; }
    public DateTime? UltimaVisitaUtc { get; init; }
    public int? DiasSinVisita { get; init; }
    public decimal MontoHistoricoTotal { get; init; }
    public bool TieneFacturacion { get; init; }
    public string? RazonSocialFacturacion { get; init; }
}

public sealed class MapaRutaOptionResponse
{
    public int IdRuta { get; init; }
    public string Nombre { get; init; } = string.Empty;
}

public sealed class MapaRecorridoOptionResponse
{
    public long IdRecorrido { get; init; }
    public int IdRuta { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public DateTime FechaRecorrido { get; init; }
    public int VisitasProgramadas { get; init; }
    public int VisitasRealizadas { get; init; }
    public decimal MontoVentas { get; init; }
}

public sealed class ReporteRecorridosResponse
{
    public ReporteRecorridosKpiResponse Kpi { get; init; } = new();
    public IReadOnlyList<RutaProgramacionDiaResponse> ProgramacionDias { get; init; } = [];
    public IReadOnlyList<ReporteRecorridoRowResponse> Recorridos { get; init; } = [];
}

public sealed class ReporteRecorridosKpiResponse
{
    public int TotalRecorridos { get; init; }
    public int RecorridosCompletos { get; init; }
    public decimal EfectividadPromedio { get; init; }
    public decimal MontoVentas { get; init; }
    public decimal MontoCancelaciones { get; init; }
}

public sealed class RutaProgramacionDiaResponse
{
    public byte DiaSemana { get; init; }
    public string DiaNombre { get; init; } = string.Empty;
    public int TotalProgramado { get; init; }
}

public sealed class ReporteRecorridoRowResponse
{
    public long IdRecorrido { get; init; }
    public int IdRuta { get; init; }
    public string Ruta { get; init; } = string.Empty;
    public string? Vendedor { get; init; }
    public DateTime FechaRecorrido { get; init; }
    public DateTime? FechaInicioUtc { get; init; }
    public DateTime? FechaFinUtc { get; init; }
    public string? Estatus { get; init; }
    public int VisitasProgramadas { get; init; }
    public int VisitasRealizadas { get; init; }
    public decimal Efectividad { get; init; }
    public decimal MontoVentas { get; init; }
    public decimal MontoEfectivo { get; init; }
    public decimal MontoCredito { get; init; }
    public decimal MontoCancelaciones { get; init; }
    public int MinutosRecorrido { get; init; }
    public int MinutosPausa { get; init; }
    public bool TieneRecargas { get; init; }
    public int TotalRecargas { get; init; }
    public string? ResumenRecargas { get; init; }
}

public sealed class ReporteVisitasResponse
{
    public ReporteVisitasHeaderResponse Header { get; init; } = new();
    public ReporteVisitasKpiResponse Kpi { get; init; } = new();
    public IReadOnlyList<ReporteVisitaRowResponse> Visitas { get; init; } = [];
    public IReadOnlyList<ReporteVisitaPendienteRowResponse> NoVisitados { get; init; } = [];
}

public sealed class ReporteVisitasHeaderResponse
{
    public long? IdRecorrido { get; init; }
    public int IdRuta { get; init; }
    public string Ruta { get; init; } = string.Empty;
    public DateTime Fecha { get; init; }
    public string? Vendedor { get; init; }
}

public sealed class ReporteVisitasKpiResponse
{
    public int VisitasProgramadas { get; init; }
    public int TotalVisitas { get; init; }
    public int VisitasConVenta { get; init; }
    public int VisitasSinVenta { get; init; }
    public int VisitasExtra { get; init; }
    public int NoVisitados { get; init; }
    public int LecturasQr { get; init; }
    public decimal PorcentajeQr { get; init; }
    public decimal MontoVentas { get; init; }
    public int TiempoMuertoMinutos { get; init; }
    public int VisitasGpsValido { get; init; }
    public decimal PorcentajeGpsValido { get; init; }
}

public sealed class ReporteVisitaRowResponse
{
    public long IdVisita { get; init; }
    public long IdRecorrido { get; init; }
    public int IdCliente { get; init; }
    public string Cliente { get; init; } = string.Empty;
    public string? Sucursal { get; init; }
    public string? Direccion { get; init; }
    public string? Estatus { get; init; }
    public string? MotivoNoVenta { get; init; }
    public DateTime? FechaInicioUtc { get; init; }
    public DateTime? FechaFinUtc { get; init; }
    public int SegundosVisita { get; init; }
    public int? MinutosTraslado { get; init; }
    public bool TieneQr { get; init; }
    public int Ventas { get; init; }
    public decimal MontoVentas { get; init; }
    public bool EsProgramada { get; init; }
    public bool EsExtra { get; init; }
    public decimal? DistanciaMetros { get; init; }
    public bool GpsValido50m { get; init; }
    public string? ProductosVendidos { get; init; }
    public string? Observaciones { get; init; }
}

public sealed class ReporteVisitaPendienteRowResponse
{
    public int IdCliente { get; init; }
    public int Orden { get; init; }
    public string Cliente { get; init; } = string.Empty;
    public string? Sucursal { get; init; }
    public string? Direccion { get; init; }
    public string TipoVisita { get; init; } = "PROGRAMADA";
    public string? Telefono { get; init; }
}

public sealed class ReporteEventosResponse
{
    public ReporteEventosKpiResponse Kpi { get; init; } = new();
    public IReadOnlyList<ReporteEventoRowResponse> Eventos { get; init; } = [];
}

public sealed class MonitorClienteResponse
{
    public MonitorClienteResumenResponse Resumen { get; init; } = new();
    public IReadOnlyList<MonitorClienteProductoTopResponse> TopProductos { get; init; } = [];
    public IReadOnlyList<MonitorClienteTendenciaResponse> Tendencia { get; init; } = [];
}

public sealed class MonitorClienteResumenResponse
{
    public int IdCliente { get; init; }
    public int? IdDireccion { get; init; }
    public string Cliente { get; init; } = string.Empty;
    public string? NombreSucursal { get; init; }
    public string? Ruta { get; init; }
    public string? Zona { get; init; }
    public string? Direccion { get; init; }
    public string? Telefono { get; init; }
    public string? Correo { get; init; }
    public string? Rfc { get; init; }
    public DateTime? ClienteDesdeUtc { get; init; }
    public string? ListaPrecio { get; init; }
    public decimal LimiteCredito { get; init; }
    public int DiasCredito { get; init; }
    public decimal MontoAcumulado { get; init; }
    public decimal MontoUltimos30Dias { get; init; }
    public int TotalVentas { get; init; }
    public decimal TicketPromedio { get; init; }
    public decimal DescuentoAcumulado { get; init; }
    public int VentasConDescuento { get; init; }
    public int FacturasEmitidas { get; init; }
    public bool TienePreciosEspecialesCliente { get; init; }
    public bool TienePreciosEspecialesZona { get; init; }
    public bool TieneDatosFacturacion { get; init; }
    public DateTime? UltimaCompraUtc { get; init; }
    public string? RazonSocialFacturacion { get; init; }
    public string? DireccionFiscal { get; init; }
}

public sealed class MonitorClienteProductoTopResponse
{
    public int RankProducto { get; init; }
    public int IdProducto { get; init; }
    public string? Sku { get; init; }
    public string Producto { get; init; } = string.Empty;
    public decimal CantidadVendida { get; init; }
    public decimal MontoTotal { get; init; }
    public DateTime? UltimaCompraUtc { get; init; }
}

public sealed class MonitorClienteTendenciaResponse
{
    public int Anio { get; init; }
    public int Mes { get; init; }
    public string Periodo { get; init; } = string.Empty;
    public decimal MontoTotal { get; init; }
    public int Ventas { get; init; }
}

public sealed class ReporteEventosKpiResponse
{
    public int TotalEventos { get; init; }
    public int Abiertos { get; init; }
    public int Criticos { get; init; }
    public int RequierenWs { get; init; }
    public int ProcesadosWs { get; init; }
}

public sealed class ReporteEventoRowResponse
{
    public long IdEvento { get; init; }
    public DateTime CreatedUtc { get; init; }
    public int? IdUsuario { get; init; }
    public string? Usuario { get; init; }
    public string? Severidad { get; init; }
    public string? TipoEvento { get; init; }
    public string? ModuloOrigen { get; init; }
    public string? AccionAplicada { get; init; }
    public string? SistemaOrigen { get; init; }
    public string? TablaOrigen { get; init; }
    public string? IdRegistroOrigen { get; init; }
    public string? Referencia { get; init; }
    public string? Descripcion { get; init; }
    public bool RequiereWs { get; init; }
    public bool ProcesadoWs { get; init; }
    public bool EstaCerrado { get; init; }
    public string? UltimaAccion { get; init; }
    public DateTime? FechaUltimaAccionUtc { get; init; }
}

public sealed class EventoAccionRequest
{
    public string Accion { get; set; } = "REVISADO";
    public string? Comentario { get; set; }
}

public sealed class MonitorWriteResult
{
    public int Id { get; init; }
    public int Conf { get; init; }
    public string? Msg { get; init; }
}

public sealed class RecorridoTicketResponse
{
    public long IdRecorrido { get; init; }
    public string? NombreVendedor { get; init; }
    public string? NombreRuta { get; init; }
    public DateOnly FechaRecorrido { get; init; }
    public DateTime? FechaInicioUtc { get; init; }
    public int TotalVisitas { get; init; }
    public int VisitasConVenta { get; init; }
    public int VisitasSinVenta { get; init; }
    public int VisitasOmitidas { get; init; }
    public int TotalVisitasRuta { get; init; }
    public int TotalVisitasExtra { get; init; }
    public int TotalVisitasCapturado { get; init; }
    public int TotalVisitasRepetido { get; init; }
    public decimal TotalVentas { get; init; }
    public decimal TotalEfectivo { get; init; }
    public decimal TotalCredito { get; init; }
    public decimal TotalCobros { get; init; }
    public decimal TotalCancelaciones { get; init; }
    public decimal TotalGastos { get; init; }
    public decimal TotalGasolina { get; init; }
    public decimal LiquidoRecibir { get; init; }
    public int? MinutosRecorrido { get; init; }
    public string? TxtInventarioInicio { get; init; }
    public string? TxtInventarioCierre { get; init; }
    public string? TxtVentasCierre { get; init; }
    public string? TxtResumenCierre { get; init; }
    public DateTime? TicketGeneradoUtc { get; init; }
}

public sealed class RecorridoCierreCompletoResponse
{
    public long IdRecorrido { get; init; }
    public string? NombreVendedor { get; init; }
    public string? NombreRuta { get; init; }
    public DateOnly FechaRecorrido { get; init; }
    public int? MinutosRecorrido { get; init; }
    public DateTime? FechaInicioUtc { get; init; }
    public int TotalVisitas { get; init; }
    public int VisitasConVenta { get; init; }
    public int VisitasSinVenta { get; init; }
    public int VisitasOmitidas { get; init; }
    public int TotalProspectos { get; init; }
    public int TotalVisitasRuta { get; init; }
    public int TotalVisitasExtra { get; init; }
    public int TotalVisitasCapturado { get; init; }
    public int TotalVisitasRepetido { get; init; }
    public decimal TotalVentas { get; init; }
    public decimal TotalEfectivo { get; init; }
    public decimal TotalCredito { get; init; }
    public decimal TotalCobros { get; init; }
    public decimal TotalCancelaciones { get; init; }
    public decimal TotalGastos { get; init; }
    public decimal TotalGasolina { get; init; }
    public decimal TotalDepositos { get; init; }
    public decimal TotalRetiros { get; init; }
    public decimal LiquidoARecibir { get; init; }
    public IReadOnlyList<ResumenDepositoItemResponse> DetalleMovimientos { get; init; } = [];
    public IReadOnlyList<CierreInventarioItemResponse> InventarioActual { get; init; } = [];
    public IReadOnlyList<CierreProductoVendidoResponse> ProductosVendidos { get; init; } = [];
}

public sealed class ResumenDepositoItemResponse
{
    public string TipoMovimiento { get; init; } = string.Empty;
    public string TipoDescripcion { get; init; } = string.Empty;
    public int Cantidad { get; init; }
    public decimal MontoTotal { get; init; }
}

public sealed class CierreInventarioItemResponse
{
    public int IdProducto { get; init; }
    public string? Sku { get; init; }
    public string? NombreProducto { get; init; }
    public long? IdLote { get; init; }
    public string? NumLote { get; init; }
    public DateOnly? FechaCaducidad { get; init; }
    public decimal CantidadPiezas { get; init; }
    public int? CantXCaja { get; init; }
    public decimal CantCajas { get; init; }
    public decimal CantPiezas { get; init; }
}

public sealed class CierreProductoVendidoResponse
{
    public int IdProducto { get; init; }
    public string? Sku { get; init; }
    public string? NombreProducto { get; init; }
    public decimal CantidadPiezas { get; init; }
    public int? CantXCaja { get; init; }
    public decimal CantCajas { get; init; }
    public decimal CantPiezas { get; init; }
    public decimal Total { get; init; }
}
