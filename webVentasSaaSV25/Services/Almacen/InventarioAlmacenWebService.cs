// Services/Almacen/InventarioAlmacenWebService.cs — VentasSaaSDU.Web
// Consumo WEB del módulo de almacenes/subalmacenes/vehículos.

using webVentasSaaSV25.Services.Http;

namespace webVentasSaaSV25.Services.Almacen;

// ═══════════════════════════════════════════════════════════════════════════
// RESPONSES PARA CARDS
// ═══════════════════════════════════════════════════════════════════════════

public sealed class AlmacenEmpresaCardResponse
{
    public int IdAlmacen { get; init; }
    public int IdEmpresa { get; init; }
    public int? IdSucursal { get; init; }
    public string? NombreSucursal { get; init; }
    public int? IdRuta { get; init; }
    public string? NombreRuta { get; init; }
    public int? IdEncargado { get; init; }
    public string? NombreEncargado { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public int TipoAlmacen { get; init; }
    public int IdCatalogoTipoAlmacen { get; init; }
    public string? NombreTipoAlmacen { get; init; }
    public string Icono { get; init; } = "warehouse";
    public string TipoCard { get; init; } = "Almacén";
    public string EtiquetaValor { get; init; } = "Inventario actual";
    public string IconoValor { get; init; } = "payments";
    public string? Subtitulo { get; init; }
    public decimal MontoInventario { get; init; }
    public decimal MontoInventarioCosto { get; init; }
    public int ProductosActivos { get; init; }
    public decimal ExistenciaTotal { get; init; }
    public DateTime? InventarioActualizadoUtc { get; init; }
    public int TotalSubalmacenes { get; init; }
    public bool PermiteNegativo { get; init; }
    public string? Observaciones { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? UpdatedUtc { get; init; }
}

public sealed class SubalmacenVehiculoCardResponse
{
    public long IdAsignacion { get; init; }
    public int IdEmpresa { get; init; }
    public int IdAlmacenPadre { get; init; }
    public string NombreAlmacenPadre { get; init; } = string.Empty;
    public int IdAlmacen { get; init; }
    public int IdSubalmacen { get; init; }
    public string NombreSubalmacen { get; init; } = string.Empty;
    public int? IdSucursal { get; init; }
    public string? NombreSucursal { get; init; }
    public int IdCatalogoTipoAlmacen { get; init; }
    public string? NombreTipoAlmacen { get; init; }
    public int IdRuta { get; init; }
    public string NombreRuta { get; init; } = string.Empty;
    public int IdActivoVehiculo { get; init; }
    public string NombreVehiculo { get; init; } = string.Empty;
    public string? ClaveInterna { get; init; }
    public string? Placa { get; init; }
    public int? AnioModelo { get; init; }
    public decimal? KmActual { get; init; }
    public int? IdEncargado { get; init; }
    public string? NombreEncargado { get; init; }
    public string Icono { get; init; } = "local_shipping";
    public string TipoCard { get; init; } = "Vehículo";
    public string EtiquetaValor { get; init; } = "Inventario actual";
    public string IconoValor { get; init; } = "payments";
    public string Nombre { get; init; } = string.Empty;
    public string? Subtitulo { get; init; }
    public decimal MontoInventario { get; init; }
    public decimal MontoInventarioCosto { get; init; }
    public int ProductosActivos { get; init; }
    public decimal ExistenciaTotal { get; init; }
    public DateTime? InventarioActualizadoUtc { get; init; }
    public DateTime FechaInicioUtc { get; init; }
    public DateTime? FechaFinUtc { get; init; }
    public string? Observaciones { get; init; }
    public bool IsActive { get; init; }
}

public sealed class AlmacenPadreDisponibleResponse
{
    public int IdAlmacen { get; init; }
    public int IdEmpresa { get; init; }
    public int? IdSucursal { get; init; }
    public string? NombreSucursal { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public int IdCatalogoTipoAlmacen { get; init; }
    public string? NombreTipoAlmacen { get; init; }
}

public sealed class RutaDisponibleSubalmacenResponse
{
    public int IdRuta { get; init; }
    public int IdEmpresa { get; init; }
    public int? IdSucursal { get; init; }
    public string? NombreSucursal { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public bool Disponible { get; init; }
    public long? IdAsignacionActual { get; init; }
}

public sealed class VehiculoDisponibleSubalmacenResponse
{
    public int IdActivoVehiculo { get; init; }
    public int IdEmpresa { get; init; }
    public int? IdSucursalBase { get; init; }
    public string? NombreSucursalBase { get; init; }
    public string? ClaveInterna { get; init; }
    public string NombreVehiculo { get; init; } = string.Empty;
    public string? Placa { get; init; }
    public int? AnioModelo { get; init; }
    public decimal? KmActual { get; init; }
    public bool Disponible { get; init; }
    public long? IdAsignacionActual { get; init; }
}

public sealed class ChoferDisponibleResponse
{
    public int IdUsuario { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string NombreCompleto { get; init; } = string.Empty;
    public long? IdAsignacionActual { get; init; }
    public string? AlmacenActual { get; init; }
    public string? RutaActual { get; init; }
}

// ═══════════════════════════════════════════════════════════════════════════
// REQUESTS ABC
// ═══════════════════════════════════════════════════════════════════════════

public sealed class InventarioAlmacenPagedResult<T>
{
    public List<T> Items { get; init; } = [];
    public int TotalRecords { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}

public sealed class InventarioAlmacenItemResponse
{
    public int IdAlmacen { get; init; }
    public string NombreAlmacen { get; init; } = string.Empty;
    public int IdProducto { get; init; }
    public string? Sku { get; init; }
    public string? CodigoBarra { get; init; }
    public string NombreProducto { get; init; } = string.Empty;
    public string? NombreCorto { get; init; }
    public long IdLote { get; init; }
    public string NumLote { get; init; } = string.Empty;
    public DateTime? FechaCaducidad { get; init; }
    public decimal CantidadPiezas { get; init; }
    public int? CantXcaja { get; init; }
    public decimal? CantidadCajas { get; init; }
    public decimal? CostoPromedio { get; init; }
    public decimal? CostoTotal { get; init; }
    public decimal PrecioActual { get; init; }
    public decimal ValorVenta { get; init; }
    public string? NombreCategoria { get; init; }
    public string? NombreMarca { get; init; }
    public string? NombreUnidad { get; init; }
    public bool SeFactura { get; init; }
    public long? IdCompra { get; init; }
    public DateTime UltimaActualizacion { get; init; }
}

public sealed class IngresoProduccionImportRequest
{
    public DateTime? FechaProduccion { get; set; }
    public string? ReferenciaProduccion { get; set; }
    public string? Observaciones { get; set; }
    public List<IngresoProduccionImportRowRequest> Filas { get; set; } = [];
}

public sealed class IngresoProduccionListItemResponse
{
    public long IdIngresoProduccion { get; init; }
    public string Folio { get; init; } = string.Empty;
    public DateTime FechaProduccion { get; init; }
    public string? Referencia { get; init; }
    public string? Observaciones { get; init; }
    public long? IdMovimiento { get; init; }
    public int TotalLineas { get; init; }
    public decimal TotalPiezas { get; init; }
    public decimal TotalCosto { get; init; }
    public DateTime CreatedUtc { get; init; }
    public string? UsuarioRegistro { get; init; }
}

// ═══ Proceso de inventario (conteo físico) ═══

public sealed class ConteoFisicoCabeceraResponse
{
    public long IdConteo { get; init; }
    public string Folio { get; init; } = string.Empty;
    public int IdAlmacen { get; init; }
    public string NombreAlmacen { get; init; } = string.Empty;
    public byte TipoConteo { get; init; }
    public byte Estatus { get; init; }
    public bool ConLotes { get; init; }
    public int IdEncargado { get; init; }
    public DateTime FechaInicioUtc { get; init; }
    public DateTime? FechaCierreUtc { get; init; }
    public decimal? MontoFaltante { get; init; }
    public decimal? MontoSobrante { get; init; }
    public long? IdMovimientoAjuste { get; init; }
    public string? Observaciones { get; init; }
}

public sealed class ConteoFisicoExistenciaResponse
{
    public int IdProducto { get; init; }
    public string? Sku { get; init; }
    public string NombreProducto { get; init; } = string.Empty;
    public long IdLote { get; init; }
    public string? NumLote { get; init; }
    public DateTime? FechaCaducidad { get; init; }
    public decimal ExistenciaDb { get; init; }
    public decimal PrecioBase { get; init; }
    public int? CantXcaja { get; init; }
    public decimal? ExistenciaFisica { get; init; }
    public string? NombreUnidad { get; init; }
}

public sealed class ConteoFisicoLineaResponse
{
    public int IdProducto { get; init; }
    public string? Sku { get; init; }
    public string NombreProducto { get; init; } = string.Empty;
    public long IdLote { get; init; }
    public string? NumLote { get; init; }
    public DateTime? FechaCaducidad { get; init; }
    public decimal ExistenciaDb { get; init; }
    public decimal? ExistenciaFisica { get; init; }
    public decimal Diferencia { get; init; }
    public decimal? PrecioUnitario { get; init; }
    public decimal MontoDiferencia { get; init; }
    public decimal? TasaCumplimiento { get; init; }
    public int? IdResultado { get; init; }
    public string? DescResultado { get; init; }
    public int? IdMotivoDiferencia { get; init; }
    public string? Observaciones { get; init; }
    public string TipoDiferencia { get; init; } = string.Empty;
}

public sealed class ConteoFisicoResultadoResponse
{
    public ConteoFisicoCabeceraResponse? Cabecera { get; init; }
    public List<ConteoFisicoLineaResponse> Lineas { get; init; } = [];
}

public sealed class ConteoFisicoListItemResponse
{
    public long IdConteo { get; init; }
    public string Folio { get; init; } = string.Empty;
    public int IdAlmacen { get; init; }
    public byte TipoConteo { get; init; }
    public byte Estatus { get; init; }
    public string DescEstatus { get; init; } = string.Empty;
    public bool ConLotes { get; init; }
    public int IdEncargado { get; init; }
    public string? Encargado { get; init; }
    public DateTime FechaInicioUtc { get; init; }
    public DateTime? FechaCierreUtc { get; init; }
    public decimal? MontoFaltante { get; init; }
    public decimal? MontoSobrante { get; init; }
    public long? IdMovimientoAjuste { get; init; }
}

public sealed class ConteoFisicoCierreResponse
{
    public long? IdMovimiento { get; init; }
    public long? IdMovimientoSalida { get; init; }
    public long? IdMovimientoEntrada { get; init; }
    public decimal MontoCobrarVendedor { get; init; }
}

public sealed class ConteoFisicoReporteResponse
{
    public ConteoFisicoCabeceraResponse? Cabecera { get; init; }
    public List<ConteoFisicoReporteLineaResponse> Faltantes { get; init; } = [];
}

public sealed class ConteoFisicoReporteLineaResponse
{
    public string? Sku { get; init; }
    public string NombreProducto { get; init; } = string.Empty;
    public string? NumLote { get; init; }
    public decimal ExistenciaDb { get; init; }
    public decimal? ExistenciaFisica { get; init; }
    public decimal Diferencia { get; init; }
    public decimal PrecioUnitario { get; init; }
    public decimal Monto { get; init; }
    public string? ClaveResultado { get; init; }
    public string? DescResultado { get; init; }
    public decimal MontoCobrar { get; init; }
}

public sealed class CrearConteoFisicoRequest
{
    public int IdAlmacen { get; set; }
    public byte TipoConteo { get; set; } = 1;
    public bool ConLotes { get; set; } = true;
    public int? IdRuta { get; set; }
    public string? Observaciones { get; set; }
}

public sealed class GuardarCapturaConteoRequest
{
    public List<CapturaConteoLineaRequest> Lineas { get; set; } = [];
}

public sealed class CapturaConteoLineaRequest
{
    public int IdProducto { get; set; }
    public long? IdLote { get; set; }
    public decimal ExistenciaFisica { get; set; }
    public short? CantCajas { get; set; }
    public int? CantPiezas { get; set; }
    public string? Observaciones { get; set; }
}

public sealed class ConcluirConteoRequest
{
    public decimal UmbralReconteo { get; set; }
    public bool Forzar { get; set; }
}

public sealed class ResolverLineaConteoRequest
{
    public int IdProducto { get; set; }
    public long IdLote { get; set; }
    public string ClaveResultado { get; set; } = string.Empty;
    public int? IdMotivo { get; set; }
    public string? Observaciones { get; set; }
}

public sealed class IngresoProduccionImportRowRequest
{
    public int RowNum { get; set; }
    public int IdProducto { get; set; }
    public string NumLote { get; set; } = string.Empty;
    public DateTime? FechaCaducidad { get; set; }
    public decimal CantidadCajas { get; set; }
    public long? IdCompra { get; set; }
    public string? IdCompraRef { get; set; }
    public bool SeFactura { get; set; } = true;
    public string? NumSerie { get; set; }
    public string? Observaciones { get; set; }
    public decimal? CostoUnitario { get; set; }
}

public sealed class SolicitudRutaResponse
{
    public long IdSolicitud { get; init; }
    public string Folio { get; init; } = string.Empty;
    public int IdAlmacenOrigen { get; init; }
    public string NombreAlmacenOrigen { get; init; } = string.Empty;
    public int IdAlmacenDestino { get; init; }
    public string NombreAlmacenDestino { get; init; } = string.Empty;
    public byte Estatus { get; init; }
    public string DescEstatus { get; init; } = string.Empty;
    public DateTime FechaSolicitudUtc { get; init; }
    public DateTime? FechaEntregaUtc { get; init; }
    public DateTime? FechaCierreUtc { get; init; }
    public int IdUsuarioSolicita { get; init; }
    public string? UsuarioSolicita { get; init; }
    public int? IdUsuarioEntrega { get; init; }
    public string? UsuarioEntrega { get; init; }
    public string? Observaciones { get; init; }
    public int TotalProductos { get; init; }
    public decimal TotalPiezasSolicitadas { get; init; }
    public decimal TotalPiezasSurtidas { get; init; }
    public decimal TotalCajasSolicitadas { get; init; }
    public decimal TotalPiezasSueltasSolicitadas { get; init; }
    public decimal TotalCajasSurtidas { get; init; }
    public decimal TotalPiezasSueltasSurtidas { get; init; }
}

public sealed class SolicitudRutaCabeceraResponse
{
    public long IdSolicitud { get; init; }
    public string Folio { get; init; } = string.Empty;
    public byte Estatus { get; init; }
    public string DescEstatus { get; init; } = string.Empty;
    public DateTime FechaSolicitudUtc { get; init; }
    public DateTime? FechaEntregaUtc { get; init; }
    public DateTime? FechaCierreUtc { get; init; }
    public int IdAlmacenOrigen { get; init; }
    public string NombreAlmacenOrigen { get; init; } = string.Empty;
    public int IdAlmacenDestino { get; init; }
    public string NombreAlmacenDestino { get; init; } = string.Empty;
    public int IdUsuarioSolicita { get; init; }
    public string? UsuarioSolicita { get; init; }
    public int? IdUsuarioEntrega { get; init; }
    public string? UsuarioEntrega { get; init; }
    public string? Observaciones { get; init; }
}

public sealed class SolicitudRutaDetalleItemResponse
{
    public int IdProducto { get; init; }
    public string? Sku { get; init; }
    public string? CodigoBarra { get; init; }
    public string NombreProducto { get; init; } = string.Empty;
    public decimal CantidadSolicitada { get; init; }
    public decimal? CantidadSurtida { get; init; }
    public decimal CantidadPendiente { get; init; }
    public short? CantTarimasSol { get; init; }
    public short? CantCajasSol { get; init; }
    public int? CantPiezasSol { get; init; }
    public short? CantCajasSurtidas { get; init; }
    public int? CantPiezasSurtidas { get; init; }
    public int? CantXcaja { get; init; }
    public byte EstatusLinea { get; init; }
    public string DescEstatusLinea { get; init; } = string.Empty;
    public string? Observaciones { get; init; }
}

public sealed class SolicitudRutaEntregaItemResponse
{
    public long IdEntrega { get; init; }
    public int IdProducto { get; init; }
    public string NombreProducto { get; init; } = string.Empty;
    public string? CodigoBarra { get; init; }
    public long IdLote { get; init; }
    public string NumLote { get; init; } = string.Empty;
    public DateTime? FechaCaducidad { get; init; }
    public decimal Cantidad { get; init; }
    public short? CantTarimas { get; init; }
    public short? CantCajas { get; init; }
    public int? CantPiezas { get; init; }
    public int? CantXcaja { get; init; }
    public DateTime FechaEntregaUtc { get; init; }
}

public sealed class SolicitudRutaDetalleResponse
{
    public SolicitudRutaCabeceraResponse? Cabecera { get; init; }
    public List<SolicitudRutaDetalleItemResponse> Detalle { get; init; } = [];
    public List<SolicitudRutaEntregaItemResponse> Entregas { get; init; } = [];
}

public sealed class RegistrarEntregaRutaRequest
{
    public int IdProducto { get; set; }
    public long? IdLote { get; set; }
    public string? NumLote { get; set; }
    public decimal Cantidad { get; set; }
    public short? CantTarimas { get; set; }
    public short? CantCajas { get; set; }
    public int? CantPiezas { get; set; }
}

public sealed class CrearTransferenciaAlmacenRequest
{
    public int IdAlmacenDestino { get; set; }
    public string? Observaciones { get; set; }
    public List<CrearTransferenciaAlmacenDetalleRequest> Detalle { get; set; } = [];
}

public sealed class CrearTransferenciaAlmacenDetalleRequest
{
    public int IdProducto { get; set; }
    public long IdLote { get; set; }
    public decimal Cantidad { get; set; }
    public short? CantTarimas { get; set; }
    public short? CantCajas { get; set; }
    public int? CantPiezas { get; set; }
}

public sealed class CrearAlmacenEmpresaRequest
{
    public int? IdSucursal { get; set; }
    public int? IdEncargado { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool PermiteNegativo { get; set; }
    public string? Observaciones { get; set; }
}

public sealed class ActualizarAlmacenEmpresaRequest
{
    public int IdAlmacen { get; set; }
    public int? IdSucursal { get; set; }
    public int? IdEncargado { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool PermiteNegativo { get; set; }
    public string? Observaciones { get; set; }
}

public sealed class CrearSubalmacenVehiculoRutaRequest
{
    public int IdRuta { get; set; }
    public int IdActivoVehiculo { get; set; }
    public int? IdEncargado { get; set; }
    public string? NombreSubalmacen { get; set; }
    public bool PermiteNegativo { get; set; }
    public string? Observaciones { get; set; }
}

public sealed class ActualizarSubalmacenVehiculoRutaRequest
{
    public long IdAsignacion { get; set; }
    public int IdAlmacenPadre { get; set; }
    public int IdRuta { get; set; }
    public int IdActivoVehiculo { get; set; }
    public int? IdEncargado { get; set; }
    public string? NombreSubalmacen { get; set; }
    public bool PermiteNegativo { get; set; }
    public string? Observaciones { get; set; }
}

public sealed class CambiarChoferVehiculoRutaRequest
{
    public int IdEncargado { get; set; }
}

public sealed class CambiarRutaVehiculoRequest
{
    public int IdRuta { get; set; }
}

// ═══════════════════════════════════════════════════════════════════════════
// SERVICIO
// ═══════════════════════════════════════════════════════════════════════════

public sealed class InventarioAlmacenWebService(ApiClient api)
{
    public Task<(List<AlmacenEmpresaCardResponse>? D, string? E)> GetAlmacenesCardsAsync(
        string? search = null,
        bool? isActive = true,
        int? idSucursal = null)
        => api.GetAsync<List<AlmacenEmpresaCardResponse>>($"api/v1/inv/almacenes/cards?{BuildQuery(new()
        {
            ["search"] = search,
            ["isActive"] = isActive,
            ["idSucursal"] = idSucursal
        })}");

    public Task<(List<SubalmacenVehiculoCardResponse>? D, string? E)> GetSubalmacenesVehiculosCardsAsync(
        int idAlmacen,
        string? search = null,
        bool? isActive = true)
        => api.GetAsync<List<SubalmacenVehiculoCardResponse>>($"api/v1/inv/almacenes/{idAlmacen}/subalmacenes/cards?{BuildQuery(new()
        {
            ["search"] = search,
            ["isActive"] = isActive
        })}");

    public Task<(SubalmacenVehiculoCardResponse? D, string? E)> GetSubalmacenVehiculoByIdAsync(long idAsignacion)
        => api.GetAsync<SubalmacenVehiculoCardResponse>($"api/v1/inv/almacenes/subalmacenes/{idAsignacion}");

    public Task<(List<AlmacenPadreDisponibleResponse>? D, string? E)> GetAlmacenesPadreDisponiblesAsync(
        string? search = null,
        int? idSucursal = null)
        => api.GetAsync<List<AlmacenPadreDisponibleResponse>>($"api/v1/inv/almacenes/padres/disponibles?{BuildQuery(new()
        {
            ["search"] = search,
            ["idSucursal"] = idSucursal
        })}");

    public Task<(List<RutaDisponibleSubalmacenResponse>? D, string? E)> GetRutasDisponiblesAsync(
        int? idSucursal = null,
        long? idAsignacionActual = null)
        => api.GetAsync<List<RutaDisponibleSubalmacenResponse>>($"api/v1/inv/almacenes/rutas/disponibles?{BuildQuery(new()
        {
            ["idSucursal"] = idSucursal,
            ["idAsignacionActual"] = idAsignacionActual
        })}");

    public Task<(List<VehiculoDisponibleSubalmacenResponse>? D, string? E)> GetVehiculosDisponiblesAsync(
        string? search = null,
        int? idSucursalBase = null,
        long? idAsignacionActual = null)
        => api.GetAsync<List<VehiculoDisponibleSubalmacenResponse>>($"api/v1/inv/almacenes/vehiculos/disponibles?{BuildQuery(new()
        {
            ["search"] = search,
            ["idSucursalBase"] = idSucursalBase,
            ["idAsignacionActual"] = idAsignacionActual
        })}");

    public Task<(List<ChoferDisponibleResponse>? D, string? E)> GetChoferesDisponiblesAsync(string? search = null)
        => api.GetAsync<List<ChoferDisponibleResponse>>($"api/v1/inv/almacenes/choferes/disponibles?{BuildQuery(new()
        {
            ["search"] = search
        })}");

    public Task<(InventarioAlmacenPagedResult<InventarioAlmacenItemResponse>? D, string? E)> GetInventarioAsync(
        int idAlmacen,
        string? search = null,
        int page = 1,
        int pageSize = 20)
        => api.GetAsync<InventarioAlmacenPagedResult<InventarioAlmacenItemResponse>>($"api/v1/inv/almacenes/{idAlmacen}/inventario?{BuildQuery(new()
        {
            ["search"] = search,
            ["page"] = page,
            ["pageSize"] = pageSize
        })}");

    public Task<(int? D, string? E)> ImportarIngresoProduccionAsync(
        int idAlmacen,
        IngresoProduccionImportRequest request)
        => api.PostAsync<int?>($"api/v1/inv/almacenes/{idAlmacen}/ingreso-produccion/importar", request);

    public Task<(InventarioAlmacenPagedResult<IngresoProduccionListItemResponse>? D, string? E)> GetIngresoProduccionListAsync(
        int idAlmacen,
        string? search = null,
        int page = 1,
        int pageSize = 20)
        => api.GetAsync<InventarioAlmacenPagedResult<IngresoProduccionListItemResponse>>($"api/v1/inv/almacenes/{idAlmacen}/ingreso-produccion?{BuildQuery(new()
        {
            ["search"] = search,
            ["page"] = page,
            ["pageSize"] = pageSize
        })}");

    // ═══ Proceso de inventario (conteo físico) ═══

    public Task<(int? D, string? E)> CrearConteoAsync(CrearConteoFisicoRequest request)
        => api.PostAsync<int?>("api/v1/inv/almacenes/conteos", request);

    public Task<(List<ConteoFisicoExistenciaResponse>? D, string? E)> CargarExistenciasConteoAsync(long idConteo, string? search = null)
        => api.GetAsync<List<ConteoFisicoExistenciaResponse>>($"api/v1/inv/almacenes/conteos/{idConteo}/existencias?{BuildQuery(new() { ["search"] = search })}");

    public Task<(int? D, string? E)> GuardarCapturaConteoAsync(long idConteo, GuardarCapturaConteoRequest request)
        => api.PostAsync<int?>($"api/v1/inv/almacenes/conteos/{idConteo}/captura", request);

    public Task<(int? D, string? E)> ConcluirConteoAsync(long idConteo, ConcluirConteoRequest request)
        => api.PostAsync<int?>($"api/v1/inv/almacenes/conteos/{idConteo}/concluir", request);

    public Task<(ConteoFisicoResultadoResponse? D, string? E)> GetConteoResultadoAsync(long idConteo)
        => api.GetAsync<ConteoFisicoResultadoResponse>($"api/v1/inv/almacenes/conteos/{idConteo}/resultado");

    public Task<(int? D, string? E)> ResolverLineaConteoAsync(long idConteo, ResolverLineaConteoRequest request)
        => api.PostAsync<int?>($"api/v1/inv/almacenes/conteos/{idConteo}/resolver", request);

    public Task<(ConteoFisicoCierreResponse? D, string? E)> AplicarCerrarConteoAsync(long idConteo)
        => api.PostAsync<ConteoFisicoCierreResponse>($"api/v1/inv/almacenes/conteos/{idConteo}/aplicar", new { });

    public Task<(InventarioAlmacenPagedResult<ConteoFisicoListItemResponse>? D, string? E)> GetConteoListAsync(
        int idAlmacen, byte estatus = 0, string? search = null, int page = 1, int pageSize = 20)
        => api.GetAsync<InventarioAlmacenPagedResult<ConteoFisicoListItemResponse>>($"api/v1/inv/almacenes/{idAlmacen}/conteos?{BuildQuery(new()
        {
            ["estatus"] = estatus,
            ["search"] = search,
            ["page"] = page,
            ["pageSize"] = pageSize
        })}");

    public Task<(ConteoFisicoReporteResponse? D, string? E)> GetConteoReporteFaltantesAsync(long idConteo)
        => api.GetAsync<ConteoFisicoReporteResponse>($"api/v1/inv/almacenes/conteos/{idConteo}/reporte-faltantes");

    public Task<(InventarioAlmacenPagedResult<SolicitudRutaResponse>? D, string? E)> GetSolicitudesRutaAsync(
        int idAlmacen,
        byte estatus = 0,
        string? search = null,
        int page = 1,
        int pageSize = 20)
        => api.GetAsync<InventarioAlmacenPagedResult<SolicitudRutaResponse>>($"api/v1/inv/almacenes/{idAlmacen}/solicitudes-ruta?{BuildQuery(new()
        {
            ["estatus"] = estatus,
            ["search"] = search,
            ["page"] = page,
            ["pageSize"] = pageSize
        })}");

    public Task<(SolicitudRutaDetalleResponse? D, string? E)> GetSolicitudRutaDetalleAsync(long idSolicitud)
        => api.GetAsync<SolicitudRutaDetalleResponse>($"api/v1/inv/almacenes/solicitudes-ruta/{idSolicitud}");

    public Task<(int? D, string? E)> RegistrarEntregaRutaAsync(long idSolicitud, RegistrarEntregaRutaRequest request)
        => api.PostAsync<int?>($"api/v1/inv/almacenes/solicitudes-ruta/{idSolicitud}/entregas", request);

    public Task<(bool Ok, string? E)> EliminarEntregaRutaAsync(long idSolicitud, long idEntrega)
        => api.DeleteAsync($"api/v1/inv/almacenes/solicitudes-ruta/{idSolicitud}/entregas/{idEntrega}");

    public Task<(int? D, string? E)> CerrarEntregaRutaAsync(long idSolicitud)
        => api.PostAsync<int?>($"api/v1/inv/almacenes/solicitudes-ruta/{idSolicitud}/entregado", new { });

    public Task<(int? D, string? E)> CrearTransferenciaAlmacenAsync(
        int idAlmacenOrigen,
        CrearTransferenciaAlmacenRequest request)
        => api.PostAsync<int?>($"api/v1/inv/almacenes/{idAlmacenOrigen}/transferencias", request);

    public Task<(int? D, string? E)> CrearAlmacenAsync(CrearAlmacenEmpresaRequest request)
        => api.PostAsync<int?>("api/v1/inv/almacenes", request);

    public Task<(int? D, string? E)> ActualizarAlmacenAsync(int idAlmacen, ActualizarAlmacenEmpresaRequest request)
        => api.PutAsync<int?>($"api/v1/inv/almacenes/{idAlmacen}", request);

    public Task<(bool Ok, string? E)> DesactivarAlmacenAsync(int idAlmacen)
        => api.DeleteAsync($"api/v1/inv/almacenes/{idAlmacen}");

    public Task<(int? D, string? E)> ReactivarAlmacenAsync(int idAlmacen)
        => api.PostAsync<int?>($"api/v1/inv/almacenes/{idAlmacen}/reactivar", new { });

    public Task<(int? D, string? E)> CrearSubalmacenVehiculoRutaAsync(int idAlmacenPadre, CrearSubalmacenVehiculoRutaRequest request)
        => api.PostAsync<int?>($"api/v1/inv/almacenes/{idAlmacenPadre}/subalmacenes", request);

    public Task<(int? D, string? E)> ActualizarSubalmacenVehiculoRutaAsync(long idAsignacion, ActualizarSubalmacenVehiculoRutaRequest request)
        => api.PutAsync<int?>($"api/v1/inv/almacenes/subalmacenes/{idAsignacion}", request);

    public Task<(bool Ok, string? E)> DesactivarSubalmacenVehiculoRutaAsync(long idAsignacion, bool permitirBajaConExistencia = false)
        => api.DeleteAsync($"api/v1/inv/almacenes/subalmacenes/{idAsignacion}?permitirBajaConExistencia={permitirBajaConExistencia.ToString().ToLowerInvariant()}");

    public Task<(int? D, string? E)> ReactivarSubalmacenVehiculoRutaAsync(long idAsignacion)
        => api.PostAsync<int?>($"api/v1/inv/almacenes/subalmacenes/{idAsignacion}/reactivar", new { });

    public Task<(int? D, string? E)> CambiarChoferVehiculoRutaAsync(long idAsignacion, CambiarChoferVehiculoRutaRequest request)
        => api.PostAsync<int?>($"api/v1/inv/almacenes/subalmacenes/{idAsignacion}/cambiar-chofer", request);

    public Task<(int? D, string? E)> CambiarRutaVehiculoAsync(long idAsignacion, CambiarRutaVehiculoRequest request)
        => api.PostAsync<int?>($"api/v1/inv/almacenes/subalmacenes/{idAsignacion}/cambiar-ruta", request);

    private static string BuildQuery(Dictionary<string, object?> values)
    {
        var parts = new List<string>();

        foreach (var (key, value) in values)
        {
            if (value is null) continue;

            string? text = value switch
            {
                string s when string.IsNullOrWhiteSpace(s) => null,
                string s => s,
                bool b => b.ToString().ToLowerInvariant(),
                _ => Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture)
            };

            if (string.IsNullOrWhiteSpace(text)) continue;
            parts.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(text)}");
        }

        return string.Join("&", parts);
    }
}
