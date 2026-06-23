using webVentasSaaSV25.Services.Http;

namespace webVentasSaaSV25.Services.Pedidos;

public sealed class PedidoPagedResult<T>
{
    public List<T> Items { get; init; } = [];
    public int TotalRecords { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}

public sealed class PedidoClienteListItemResponse
{
    public long IdPedido { get; init; }
    public int IdEmpresa { get; init; }
    public int? IdSucursal { get; init; }
    public int? IdAlmacenSurtido { get; init; }
    public int? IdCliente { get; init; }
    public string? NombreCliente { get; init; }
    public int? IdRuta { get; init; }
    public string? NombreRuta { get; init; }
    public int? IdVendedor { get; init; }
    public byte TipoCanal { get; init; }
    public string Folio { get; init; } = string.Empty;
    public int Estatus { get; init; }
    public string DescEstatus { get; init; } = string.Empty;
    public DateTime FechaPedidoUtc { get; init; }
    public DateTime? FechaEntregaProg { get; init; }
    public DateTime? FechaEntregaRealUtc { get; init; }
    public long? IdVentaRef { get; init; }
    public string? Telefono { get; init; }
    public string? Observaciones { get; init; }
    public string? ClaveMovilPedido { get; init; }
    public int TotalProductos { get; init; }
    public decimal TotalCantidadPedida { get; init; }
    public decimal TotalCantidadSurtida { get; init; }
    public decimal TotalSubtotal { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? UpdatedUtc { get; init; }
}

public sealed class PedidoClienteCabeceraResponse
{
    public long IdPedido { get; init; }
    public int IdEmpresa { get; init; }
    public int? IdSucursal { get; init; }
    public int? IdAlmacenSurtido { get; init; }
    public int? IdCliente { get; init; }
    public string? NombreCliente { get; init; }
    public int? IdRuta { get; init; }
    public string? NombreRuta { get; init; }
    public int? IdVendedor { get; init; }
    public byte TipoCanal { get; init; }
    public string Folio { get; init; } = string.Empty;
    public int Estatus { get; init; }
    public string DescEstatus { get; init; } = string.Empty;
    public DateTime FechaPedidoUtc { get; init; }
    public DateTime? FechaEntregaProg { get; init; }
    public DateTime? FechaEntregaRealUtc { get; init; }
    public long? IdVentaRef { get; init; }
    public long? IdSolicitudAlmacen { get; init; }
    public string? Telefono { get; init; }
    public string? Correo { get; init; }
    public decimal? Latitud { get; init; }
    public decimal? Longitud { get; init; }
    public string? Observaciones { get; init; }
    public string? ClaveMovilPedido { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? UpdatedUtc { get; init; }
}

public sealed class PedidoClienteDetalleItemResponse
{
    public long IdPedido { get; init; }
    public int IdProducto { get; init; }
    public string? NombreProducto { get; init; }
    public decimal CantidadPedida { get; init; }
    public decimal CantidadSurtida { get; init; }
    public decimal PrecioUnitario { get; init; }
    public decimal DescuentoPct { get; init; }
    public decimal Subtotal { get; init; }
    public int? IdUnidadMedida { get; init; }
    public int EstatusLinea { get; init; }
    public string? Observaciones { get; init; }
}

public sealed class PedidoClienteDetalleResponse
{
    public PedidoClienteCabeceraResponse? Cabecera { get; init; }
    public List<PedidoClienteDetalleItemResponse> Detalle { get; init; } = [];
}

public sealed class PedidoClienteFiltro
{
    public int? IdRuta { get; set; }
    public int Estatus { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 100;
}

public sealed class FinalizarPreparacionPedidosRequest
{
    public int IdAlmacenOrigen { get; init; }
    public int IdAlmacenRuta { get; init; }
    public int IdRuta { get; init; }
    public List<long> IdsPedido { get; init; } = [];
    public string? Observaciones { get; init; }
}

public sealed class FinalizarPreparacionPedidosResponse
{
    public long IdSolicitudAlmacen { get; init; }
    public string FolioSolicitud { get; init; } = string.Empty;
    public int PedidosProcesados { get; init; }
    public int ProductosConsolidados { get; init; }
}

public sealed class PedidoClienteWebService(ApiClient api)
{
    public Task<(PedidoPagedResult<PedidoClienteListItemResponse>? D, string? E)> GetPedidosAsync(PedidoClienteFiltro filtro)
        => api.GetAsync<PedidoPagedResult<PedidoClienteListItemResponse>>($"api/v1/pedidos?{BuildQuery(new()
        {
            ["idRuta"] = filtro.IdRuta,
            ["estatus"] = filtro.Estatus,
            ["search"] = filtro.Search,
            ["page"] = filtro.Page,
            ["pageSize"] = filtro.PageSize
        })}");

    public Task<(PedidoClienteDetalleResponse? D, string? E)> GetPedidoAsync(long idPedido)
        => api.GetAsync<PedidoClienteDetalleResponse>($"api/v1/pedidos/{idPedido}");

    public Task<(FinalizarPreparacionPedidosResponse? D, string? E)> FinalizarPreparacionAsync(FinalizarPreparacionPedidosRequest request)
        => api.PostAsync<FinalizarPreparacionPedidosResponse>("api/v1/pedidos/preparacion/finalizar", request);

    private static string BuildQuery(Dictionary<string, object?> values)
    {
        var parts = new List<string>();

        foreach (var (key, value) in values)
        {
            if (value is null) continue;

            var text = value switch
            {
                string s when string.IsNullOrWhiteSpace(s) => null,
                string s => s,
                _ => Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture)
            };

            if (string.IsNullOrWhiteSpace(text)) continue;
            parts.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(text)}");
        }

        return string.Join("&", parts);
    }
}
