// Services/Catalogos/CatalogoServices.cs — VentasSaaSDU.Web
// Servicios de consumo API para todos los catálogos simples.
// Cada servicio usa ApiClient (wrapper con envelope ApiResult<T>).

using System.Text.Json.Serialization;
using webVentasSaaSV25.Services.Http;

namespace webVentasSaaSV25.Services.Catalogos;

// ═══════════════════════════════════════════════════════════════════════════
// MODELOS CLIENTE (espejo de DTOs API — PropertyNameCaseInsensitive=true)
// ═══════════════════════════════════════════════════════════════════════════

// ── Paginado genérico ──────────────────────────────────────────────────────
public sealed class PagedResult<T>
{
    public List<T> Items        { get; init; } = [];
    public int     TotalRecords { get; init; }
    public int     Page         { get; init; }
    public int     PageSize     { get; init; }
    public int     TotalPages   { get; init; }
}


public sealed class ApiWriteResult
{
    public bool    Success    { get; init; }
    public int?    AffectedId { get; init; }
    public string? Error      { get; init; }
}

// ── Zona ──────────────────────────────────────────────────────────────────
public sealed class ZonaResponse
{
    public int       IdZona     { get; init; }
    public string    Nombre     { get; init; } = string.Empty;
    public bool      IsActive   { get; init; }
    public DateTime  CreatedUtc { get; init; }
    public DateTime? UpdatedUtc { get; init; }
}
public sealed class CrearZonaRequest    { public string Nombre { get; set; } = string.Empty; }
public sealed class ActualizarZonaRequest { public int IdZona { get; set; } public string? Nombre { get; set; } public bool? IsActive { get; set; } }
public sealed class FiltroZonaRequest  { public string? Search { get; set; } public bool? IsActive { get; set; } = true; public int Page { get; set; } = 1; public int PageSize { get; set; } = 50; }

// ── Ruta ──────────────────────────────────────────────────────────────────
public sealed class RutaResponse
{
    public int       IdRuta     { get; init; }
    public string    Nombre     { get; init; } = string.Empty;
    public bool      IsActive   { get; init; }
    public DateTime  CreatedUtc { get; init; }
    public DateTime? UpdatedUtc { get; init; }
}
public sealed class CrearRutaRequest    { public string Nombre { get; set; } = string.Empty; }
public sealed class ActualizarRutaRequest { public int IdRuta { get; set; } public string? Nombre { get; set; } public bool? IsActive { get; set; } }
public sealed class FiltroRutaRequest  { public string? Search { get; set; } public bool? IsActive { get; set; } = true; public int Page { get; set; } = 1; public int PageSize { get; set; } = 50; }

// ── Unidad ────────────────────────────────────────────────────────────────
public sealed class UnidadResponse
{
    public int       IdUnidad            { get; init; }
    public string    Clave               { get; init; } = string.Empty;
    public string    Nombre              { get; init; } = string.Empty;
    public bool      IsActive            { get; init; }
    public DateTime  CreatedUtc          { get; init; }
    public DateTime? UpdatedUtc          { get; init; }
}
public sealed class CrearUnidadRequest    { public string Clave { get; set; } = string.Empty; public string Nombre { get; set; } = string.Empty; }
public sealed class ActualizarUnidadRequest { public int IdUnidad { get; set; } public string? Clave { get; set; } public string? Nombre { get; set; } public bool? IsActive { get; set; } }
public sealed class FiltroUnidadRequest  { public string? Search { get; set; } public bool? IsActive { get; set; } = true; public int Page { get; set; } = 1; public int PageSize { get; set; } = 50; }

// ── Clasificacion ─────────────────────────────────────────────────────────
public sealed class ClasificacionResponse
{
    public int       IdClasificacion      { get; init; }
    public int?      IdClasificacionPadre { get; init; }
    public string?   NombrePadre          { get; init; }
    public string    Codigo               { get; init; } = string.Empty;
    public string    Nombre               { get; init; } = string.Empty;
    public int       Tipo                 { get; init; }
    public int       Orden                { get; init; }
    public bool      IsActive             { get; init; }
    public DateTime  CreatedUtc           { get; init; }
    public DateTime? UpdatedUtc           { get; init; }
}
public sealed class CrearClasificacionRequest { public string Codigo { get; set; } = string.Empty; public string Nombre { get; set; } = string.Empty; public int? IdClasificacionPadre { get; set; } public int Tipo { get; set; } = 1; public int Orden { get; set; } = 0; }
public sealed class ActualizarClasificacionRequest { public int IdClasificacion { get; set; } public string? Codigo { get; set; } public string? Nombre { get; set; } public int? IdClasificacionPadre { get; set; } public int? Tipo { get; set; } public int? Orden { get; set; } public bool? IsActive { get; set; } }
public sealed class FiltroClasificacionRequest { public string? Search { get; set; } public bool? IsActive { get; set; } = true; public int? Tipo { get; set; } public int Page { get; set; } = 1; public int PageSize { get; set; } = 50; }

// ── CatalogoTipo ──────────────────────────────────────────────────────────
public sealed class CatalogoTipoResponse
{
    public int       IdTipoCatalogo { get; init; }
    public string    Codigo         { get; init; } = string.Empty;
    public string    Nombre         { get; init; } = string.Empty;
    public string?   Descripcion    { get; init; }
    public bool      IsActive       { get; init; }
    public DateTime  CreatedUtc     { get; init; }
    public DateTime? UpdatedUtc     { get; init; }
}
public sealed class CrearCatalogoTipoRequest    { public string Codigo { get; set; } = string.Empty; public string Nombre { get; set; } = string.Empty; public string? Descripcion { get; set; } }
public sealed class ActualizarCatalogoTipoRequest { public int IdTipoCatalogo { get; set; } public string? Codigo { get; set; } public string? Nombre { get; set; } public string? Descripcion { get; set; } public bool? IsActive { get; set; } }
public sealed class FiltroCatalogoTipoRequest  { public string? Search { get; set; } public bool? IsActive { get; set; } = true; public int Page { get; set; } = 1; public int PageSize { get; set; } = 50; }

// ── CatalogoItem ──────────────────────────────────────────────────────────
public sealed class CatalogoItemResponse
{
    public int       IdCatalogoItem  { get; init; }
    public int       IdTipoCatalogo  { get; init; }
    public string?   NombreTipo      { get; init; }
    public string?   Clave           { get; init; }
    public string    Nombre          { get; init; } = string.Empty;
    public int?      Orden           { get; init; }
    public bool      IsActive        { get; init; }
    public DateTime  CreatedUtc      { get; init; }
    public DateTime? UpdatedUtc      { get; init; }
}
public sealed class CrearCatalogoItemRequest    { public int IdTipoCatalogo { get; set; } public string? Clave { get; set; } public string Nombre { get; set; } = string.Empty; public int? Orden { get; set; } }
public sealed class ActualizarCatalogoItemRequest { public int IdCatalogoItem { get; set; } public int IdTipoCatalogo { get; set; } public string? Clave { get; set; } public string? Nombre { get; set; } public int? Orden { get; set; } public bool? IsActive { get; set; } }
public sealed class FiltroCatalogoItemRequest  { public int? IdTipoCatalogo { get; set; } public string? Search { get; set; } public bool? IsActive { get; set; } = true; public int Page { get; set; } = 1; public int PageSize { get; set; } = 50; }

// ── Impuesto ──────────────────────────────────────────────────────────────
public sealed class ImpuestoResponse
{
    public int       IdImpuesto  { get; init; }
    public string    Nombre      { get; init; } = string.Empty;
    public decimal   Tasa        { get; init; }
    public bool      EsRetencion { get; init; }
    public bool      IsActive    { get; init; }
    public DateTime  CreatedUtc  { get; init; }
    public DateTime? UpdatedUtc  { get; init; }
}
public sealed class CrearImpuestoRequest    { public string Nombre { get; set; } = string.Empty; public decimal Tasa { get; set; } public bool EsRetencion { get; set; } = false; }
public sealed class ActualizarImpuestoRequest { public int IdImpuesto { get; set; } public string? Nombre { get; set; } public decimal? Tasa { get; set; } public bool? EsRetencion { get; set; } public bool? IsActive { get; set; } }
public sealed class FiltroImpuestoRequest  { public string? Search { get; set; } public bool? IsActive { get; set; } = true; public int Page { get; set; } = 1; public int PageSize { get; set; } = 50; }

// ── Proveedor ─────────────────────────────────────────────────────────────
public sealed class ProveedorResponse
{
    public int       IdProveedor    { get; init; }
    public string    Codigo         { get; init; } = string.Empty;
    public string    Nombre         { get; init; } = string.Empty;
    public string?   Rfc            { get; init; }
    public string?   Correo         { get; init; }
    public string?   Telefono       { get; init; }
    public int       DiasCredito    { get; init; }
    public bool      IsActive       { get; init; }
    public DateTime  CreatedUtc     { get; init; }
    public DateTime? UpdatedUtc     { get; init; }
}
public sealed class CrearProveedorRequest    { public string Codigo { get; set; } = string.Empty; public string Nombre { get; set; } = string.Empty; public string? Rfc { get; set; } public string? Correo { get; set; } public string? Telefono { get; set; } public string? Direccion1 { get; set; } public int DiasCredito { get; set; } = 0; }
public sealed class ActualizarProveedorRequest { public int IdProveedor { get; set; } public string? Codigo { get; set; } public string? Nombre { get; set; } public string? Rfc { get; set; } public string? Correo { get; set; } public string? Telefono { get; set; } public string? Direccion1 { get; set; } public int? DiasCredito { get; set; } public bool? IsActive { get; set; } }
public sealed class FiltroProveedorRequest  { public string? Search { get; set; } public bool? IsActive { get; set; } = true; public int Page { get; set; } = 1; public int PageSize { get; set; } = 50; }

// ── Sucursal ──────────────────────────────────────────────────────────────
public sealed class SucursalResponse
{
    public int       IdSucursal  { get; init; }
    public string    Codigo      { get; init; } = string.Empty;
    public string    Nombre      { get; init; } = string.Empty;
    public string?   NombreZona  { get; init; }
    public int?      IdZona      { get; init; }
    public bool      IsActive    { get; init; }
    public DateTime  CreatedUtc  { get; init; }
    public DateTime? UpdatedUtc  { get; init; }
}
public sealed class CrearSucursalRequest    { public string Codigo { get; set; } = string.Empty; public string Nombre { get; set; } = string.Empty; public int? IdZona { get; set; } public bool ManejaInventario { get; set; } = true; }
public sealed class ActualizarSucursalRequest { public int IdSucursal { get; set; } public string? Codigo { get; set; } public string? Nombre { get; set; } public int? IdZona { get; set; } public bool? ManejaInventario { get; set; } public bool? IsActive { get; set; } }
public sealed class FiltroSucursalRequest  { public string? Search { get; set; } public bool? IsActive { get; set; } = true; public int Page { get; set; } = 1; public int PageSize { get; set; } = 50; }

// ═══════════════════════════════════════════════════════════════════════════
// HELPER INTERNO DE QUERY STRING
// ═══════════════════════════════════════════════════════════════════════════
internal static class Qs
{
    internal static string Build(string? search, bool? isActive, int page, int pageSize, string extra = "")
    {
        var p = new List<string> { $"page={page}", $"pageSize={pageSize}" };
        if (!string.IsNullOrWhiteSpace(search)) p.Add($"search={Uri.EscapeDataString(search)}");
        if (isActive.HasValue) p.Add($"isActive={isActive.Value.ToString().ToLower()}");
        return string.Join("&", p) + extra;
    }
}

// ═══════════════════════════════════════════════════════════════════════════
// SERVICIOS
// ═══════════════════════════════════════════════════════════════════════════

public sealed class ZonaService(ApiClient api)
{
    public Task<(PagedResult<ZonaResponse>? D, string? E)> GetPagedAsync(FiltroZonaRequest f)
        => api.GetAsync<PagedResult<ZonaResponse>>($"api/v1/zonas?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize)}");
    public Task<(ZonaResponse? D, string? E)>                GetByIdAsync(int id)    => api.GetAsync<ZonaResponse>($"api/v1/zonas/{id}");
    public Task<(ApiWriteResult? D, string? E)>              CrearAsync(CrearZonaRequest r)         => api.PostAsync<ApiWriteResult>("api/v1/zonas", r);
    public Task<(ApiWriteResult? D, string? E)>              ActualizarAsync(int id, ActualizarZonaRequest r) => api.PutAsync<ApiWriteResult>($"api/v1/zonas/{id}", r);
    public Task<(bool Ok, string? E)>                        EliminarAsync(int id)   => api.DeleteAsync($"api/v1/zonas/{id}");
}

public sealed class RutaService(ApiClient api)
{
    public Task<(PagedResult<RutaResponse>? D, string? E)> GetPagedAsync(FiltroRutaRequest f)
        => api.GetAsync<PagedResult<RutaResponse>>($"api/v1/rutas?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize)}");
    public Task<(RutaResponse? D, string? E)>                GetByIdAsync(int id)    => api.GetAsync<RutaResponse>($"api/v1/rutas/{id}");
    public Task<(ApiWriteResult? D, string? E)>              CrearAsync(CrearRutaRequest r)          => api.PostAsync<ApiWriteResult>("api/v1/rutas", r);
    public Task<(ApiWriteResult? D, string? E)>              ActualizarAsync(int id, ActualizarRutaRequest r) => api.PutAsync<ApiWriteResult>($"api/v1/rutas/{id}", r);
    public Task<(bool Ok, string? E)>                        EliminarAsync(int id)   => api.DeleteAsync($"api/v1/rutas/{id}");
}

public sealed class UnidadService(ApiClient api)
{
    public Task<(PagedResult<UnidadResponse>? D, string? E)> GetPagedAsync(FiltroUnidadRequest f)
        => api.GetAsync<PagedResult<UnidadResponse>>($"api/v1/unidades?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize)}");
    public Task<(UnidadResponse? D, string? E)>               GetByIdAsync(int id)   => api.GetAsync<UnidadResponse>($"api/v1/unidades/{id}");
    public Task<(ApiWriteResult? D, string? E)>               CrearAsync(CrearUnidadRequest r)          => api.PostAsync<ApiWriteResult>("api/v1/unidades", r);
    public Task<(ApiWriteResult? D, string? E)>               ActualizarAsync(int id, ActualizarUnidadRequest r) => api.PutAsync<ApiWriteResult>($"api/v1/unidades/{id}", r);
    public Task<(bool Ok, string? E)>                         EliminarAsync(int id)  => api.DeleteAsync($"api/v1/unidades/{id}");
}

public sealed class ClasificacionService(ApiClient api)
{
    public Task<(PagedResult<ClasificacionResponse>? D, string? E)> GetPagedAsync(FiltroClasificacionRequest f)
        => api.GetAsync<PagedResult<ClasificacionResponse>>($"api/v1/clasificaciones?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize, f.Tipo.HasValue ? $"&tipo={f.Tipo}" : "")}");
    public Task<(PagedResult<ClasificacionResponse>? D, string? E)> GetAllActivosAsync()
        => api.GetAsync<PagedResult<ClasificacionResponse>>("api/v1/clasificaciones?page=1&pageSize=500&isActive=true");
    public Task<(ClasificacionResponse? D, string? E)>               GetByIdAsync(int id)   => api.GetAsync<ClasificacionResponse>($"api/v1/clasificaciones/{id}");
    public Task<(ApiWriteResult? D, string? E)>                      CrearAsync(CrearClasificacionRequest r)          => api.PostAsync<ApiWriteResult>("api/v1/clasificaciones", r);
    public Task<(ApiWriteResult? D, string? E)>                      ActualizarAsync(int id, ActualizarClasificacionRequest r) => api.PutAsync<ApiWriteResult>($"api/v1/clasificaciones/{id}", r);
    public Task<(bool Ok, string? E)>                                EliminarAsync(int id)  => api.DeleteAsync($"api/v1/clasificaciones/{id}");
}

public sealed class CatalogoTipoService(ApiClient api)
{
    public Task<(PagedResult<CatalogoTipoResponse>? D, string? E)> GetPagedAsync(FiltroCatalogoTipoRequest f)
        => api.GetAsync<PagedResult<CatalogoTipoResponse>>($"api/v1/catalogotipos?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize)}");
    public Task<(PagedResult<CatalogoTipoResponse>? D, string? E)> GetAllActivosAsync()
        => api.GetAsync<PagedResult<CatalogoTipoResponse>>("api/v1/catalogotipos?page=1&pageSize=500&isActive=true");
    public Task<(CatalogoTipoResponse? D, string? E)>               GetByIdAsync(int id)   => api.GetAsync<CatalogoTipoResponse>($"api/v1/catalogotipos/{id}");
    public Task<(ApiWriteResult? D, string? E)>                     CrearAsync(CrearCatalogoTipoRequest r)          => api.PostAsync<ApiWriteResult>("api/v1/catalogotipos", r);
    public Task<(ApiWriteResult? D, string? E)>                     ActualizarAsync(int id, ActualizarCatalogoTipoRequest r) => api.PutAsync<ApiWriteResult>($"api/v1/catalogotipos/{id}", r);
    public Task<(bool Ok, string? E)>                               EliminarAsync(int id)  => api.DeleteAsync($"api/v1/catalogotipos/{id}");
}

public sealed class CatalogoItemService(ApiClient api)
{
    public Task<(PagedResult<CatalogoItemResponse>? D, string? E)> GetPagedAsync(FiltroCatalogoItemRequest f)
        => api.GetAsync<PagedResult<CatalogoItemResponse>>($"api/v1/catalogoitems?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize, f.IdTipoCatalogo.HasValue ? $"&idTipoCatalogo={f.IdTipoCatalogo}" : "")}");
    public Task<(PagedResult<CatalogoItemResponse>? D, string? E)> GetByTipoAsync(int idTipo)
        => api.GetAsync<PagedResult<CatalogoItemResponse>>($"api/v1/catalogoitems?idTipoCatalogo={idTipo}&page=1&pageSize=500&isActive=true");
    public Task<(CatalogoItemResponse? D, string? E)>               GetByIdAsync(int id)   => api.GetAsync<CatalogoItemResponse>($"api/v1/catalogoitems/{id}");
    public Task<(ApiWriteResult? D, string? E)>                     CrearAsync(CrearCatalogoItemRequest r)          => api.PostAsync<ApiWriteResult>("api/v1/catalogoitems", r);
    public Task<(ApiWriteResult? D, string? E)>                     ActualizarAsync(int id, ActualizarCatalogoItemRequest r) => api.PutAsync<ApiWriteResult>($"api/v1/catalogoitems/{id}", r);
    public Task<(bool Ok, string? E)>                               EliminarAsync(int id)  => api.DeleteAsync($"api/v1/catalogoitems/{id}");
}

public sealed class ImpuestoService(ApiClient api)
{
    public Task<(PagedResult<ImpuestoResponse>? D, string? E)> GetPagedAsync(FiltroImpuestoRequest f)
        => api.GetAsync<PagedResult<ImpuestoResponse>>($"api/v1/impuestos?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize)}");
    public Task<(ImpuestoResponse? D, string? E)>               GetByIdAsync(int id)   => api.GetAsync<ImpuestoResponse>($"api/v1/impuestos/{id}");
    public Task<(ApiWriteResult? D, string? E)>                  CrearAsync(CrearImpuestoRequest r)          => api.PostAsync<ApiWriteResult>("api/v1/impuestos", r);
    public Task<(ApiWriteResult? D, string? E)>                  ActualizarAsync(int id, ActualizarImpuestoRequest r) => api.PutAsync<ApiWriteResult>($"api/v1/impuestos/{id}", r);
    public Task<(bool Ok, string? E)>                            EliminarAsync(int id)  => api.DeleteAsync($"api/v1/impuestos/{id}");
}

public sealed class ProveedorService(ApiClient api)
{
    public Task<(PagedResult<ProveedorResponse>? D, string? E)> GetPagedAsync(FiltroProveedorRequest f)
        => api.GetAsync<PagedResult<ProveedorResponse>>($"api/v1/proveedores?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize)}");
    public Task<(ProveedorResponse? D, string? E)>               GetByIdAsync(int id)   => api.GetAsync<ProveedorResponse>($"api/v1/proveedores/{id}");
    public Task<(ApiWriteResult? D, string? E)>                  CrearAsync(CrearProveedorRequest r)          => api.PostAsync<ApiWriteResult>("api/v1/proveedores", r);
    public Task<(ApiWriteResult? D, string? E)>                  ActualizarAsync(int id, ActualizarProveedorRequest r) => api.PutAsync<ApiWriteResult>($"api/v1/proveedores/{id}", r);
    public Task<(bool Ok, string? E)>                            EliminarAsync(int id)  => api.DeleteAsync($"api/v1/proveedores/{id}");
}

public sealed class SucursalService(ApiClient api)
{
    public Task<(PagedResult<SucursalResponse>? D, string? E)> GetPagedAsync(FiltroSucursalRequest f)
        => api.GetAsync<PagedResult<SucursalResponse>>($"api/v1/sucursales?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize)}");
    public Task<(PagedResult<SucursalResponse>? D, string? E)> GetAllActivasAsync()
        => api.GetAsync<PagedResult<SucursalResponse>>("api/v1/sucursales?page=1&pageSize=500&isActive=true");
    public Task<(SucursalResponse? D, string? E)>               GetByIdAsync(int id)   => api.GetAsync<SucursalResponse>($"api/v1/sucursales/{id}");
    public Task<(ApiWriteResult? D, string? E)>                 CrearAsync(CrearSucursalRequest r)          => api.PostAsync<ApiWriteResult>("api/v1/sucursales", r);
    public Task<(ApiWriteResult? D, string? E)>                 ActualizarAsync(int id, ActualizarSucursalRequest r) => api.PutAsync<ApiWriteResult>($"api/v1/sucursales/{id}", r);
    public Task<(bool Ok, string? E)>                           EliminarAsync(int id)  => api.DeleteAsync($"api/v1/sucursales/{id}");
}



// ── Cliente ───────────────────────────────────────────────────────────────
// Columnas reales verificadas: id_tipo_persona, nombre_completo,
// correo, telefono1, bloquear_credito, tiene_credito (del JOIN ClienteCredito)
public sealed class ClienteResponse
{
    public int IdCliente { get; init; }
    public int IdEmpresa { get; init; }
    public int IdTipoPersona { get; init; }   // 1=física, 2=moral
    public string NombreCompleto { get; init; } = string.Empty;
    public string? Nombre { get; init; }
    public string? ApePat { get; init; }
    public string? ApeMat { get; init; }
    public string? RazonSocial { get; init; }
    public string? Rfc { get; init; }
    public string? Correo { get; init; }
    public string? Telefono1 { get; init; }
    public string? Telefono2 { get; init; }
    public bool BloquearCredito { get; init; }
    public bool BloquearGeneral { get; init; }
    public int? IdListaPrecio { get; init; }
    public bool TieneCredito { get; init; }
    public decimal LimiteCredito { get; init; }
    public int DiasCredito { get; init; }
    public int? IdRuta { get; init; }
    public string? NombreRuta { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? UpdatedUtc { get; init; }
}
public sealed class CrearClienteRequest
{
    public int IdTipoPersona { get; set; } = 1;
    public string? Nombre { get; set; }
    public string? ApePat { get; set; }
    public string? ApeMat { get; set; }
    public string? RazonSocial { get; set; }
    public string? Rfc { get; set; }
    public string? Correo { get; set; }
    public string? Telefono1 { get; set; }
    public string? Telefono2 { get; set; }
    public decimal? CredLimite { get; set; }
    public int? CredDias { get; set; }
}
public sealed class ActualizarClienteRequest
{
    public int IdCliente { get; set; }
    public int? IdTipoPersona { get; set; }
    public string? Nombre { get; set; }
    public string? ApePat { get; set; }
    public string? ApeMat { get; set; }
    public string? RazonSocial { get; set; }
    public string? Rfc { get; set; }
    public string? Correo { get; set; }
    public string? Telefono1 { get; set; }
    public string? Telefono2 { get; set; }
    public bool? BloquearCredito { get; set; }
    public bool? BloquearGeneral { get; set; }
    public decimal? CredLimite { get; set; }
    public int? CredDias { get; set; }
    public bool? IsActive { get; set; }
}
public sealed class FiltroClienteRequest
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; } = true;
    public bool? BloquearCredito { get; set; }
    public int? IdRuta { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

// ── Producto ──────────────────────────────────────────────────────────────
// Columnas reales: sku, tipo_producto, id_clasif_categoria, id_clasif_marca,
// maneja_inventario, costo_actual, precio_actual, es_servicio (BIT real)
public sealed class ProductoResponse
{
    public int IdProducto { get; init; }
    public int IdEmpresa { get; init; }
    public string? Sku { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? NombreCorto { get; init; }
    public string? Descripcion { get; init; }
    public string CodigoBarra { get; init; } = string.Empty;
    public int TipoProducto { get; init; }   // 1=producto, 2=servicio, 3=kit
    public bool EsServicio { get; init; }   // BIT real en BD
    public bool EsKit { get; init; }   // BIT real en BD
    public bool EsProducido { get; init; }
    public int? IdClasifCategoria { get; init; }
    public string? NombreClasificacion { get; init; }
    public int? IdClasifMarca { get; init; }
    public string? NombreMarca { get; init; }
    public int? IdUnidad { get; init; }
    public string? ClaveUnidad { get; init; }
    public string? NombreUnidad { get; init; }
    public bool ManejaInventario { get; init; }
    public bool ManejaLote { get; init; }
    public bool ManejaSerie { get; init; }
    public bool? ManejaCaducidad { get; init; }
    public decimal CostoActual { get; init; }
    public decimal PrecioActual { get; init; }
    public decimal IvaTasa { get; init; }
    public bool VentaDirectaWeb { get; init; }
    public bool VentaDirectaApp { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? UpdatedUtc { get; init; }
}
public sealed class CrearProductoRequest
{
    public string? Sku { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? NombreCorto { get; set; }
    public string? Descripcion { get; set; }
    public string? CodigoBarra { get; set; }
    public int TipoProducto { get; set; } = 1;
    public int? IdClasifCategoria { get; set; }
    public int? IdClasifMarca { get; set; }
    public int? IdUnidad { get; set; }
    public bool ManejaInventario { get; set; } = true;
    public bool ManejaLote { get; set; } = false;
    public bool ManejaSerie { get; set; } = false;
    public decimal CostoActual { get; set; } = 0;
    public decimal PrecioActual { get; set; } = 0;
    public decimal IvaTasa { get; set; } = 0;
}
public sealed class ActualizarProductoRequest
{
    public int IdProducto { get; set; }
    public string? Sku { get; set; }
    public string? Nombre { get; set; }
    public string? NombreCorto { get; set; }
    public string? Descripcion { get; set; }
    public string? CodigoBarra { get; set; }
    public int? TipoProducto { get; set; }
    public int? IdClasifCategoria { get; set; }
    public int? IdClasifMarca { get; set; }
    public int? IdUnidad { get; set; }
    public bool? ManejaInventario { get; set; }
    public bool? ManejaLote { get; set; }
    public bool? ManejaSerie { get; set; }
    public decimal? CostoActual { get; set; }
    public decimal? PrecioActual { get; set; }
    public decimal? IvaTasa { get; set; }
    public bool? IsActive { get; set; }
}
public sealed class FiltroProductoRequest
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; } = true;
    public int? IdClasifCategoria { get; set; }
    public bool? EsServicio { get; set; }
    public bool? ManejaInventario { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

// ── Empresa (solo lectura del tenant activo) ──────────────────────────────
public sealed class EmpresaResponse
{
    public int IdEmpresa { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public string? Rfc { get; init; }
    public string? ZonaHoraria { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? UpdatedUtc { get; init; }
}

// ── Moneda ────────────────────────────────────────────────────────────────
public sealed class MonedaResponse
{
    public string Codigo { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? UpdatedUtc { get; init; }
}
public sealed class CrearMonedaRequest { public string Codigo { get; set; } = string.Empty; public string Nombre { get; set; } = string.Empty; }
public sealed class ActualizarMonedaRequest { public string Codigo { get; set; } = string.Empty; public string? Nombre { get; set; } public bool? IsActive { get; set; } }
public sealed class FiltroMonedaRequest { public string? Search { get; set; } public bool? IsActive { get; set; } = true; public int Page { get; set; } = 1; public int PageSize { get; set; } = 50; }

// ── SerieDocumento ────────────────────────────────────────────────────────
public sealed class SerieDocumentoResponse
{
    public int IdSerie { get; init; }
    public int IdEmpresa { get; init; }
    public string Modulo { get; init; } = string.Empty;
    public string TipoDoc { get; init; } = string.Empty;
    public string Serie { get; init; } = string.Empty;
    public long FolioActual { get; init; }
    public string FolioDisplay { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? UpdatedUtc { get; init; }
}
public sealed class CrearSerieDocumentoRequest { public string Modulo { get; set; } = string.Empty; public string TipoDoc { get; set; } = string.Empty; public string Serie { get; set; } = string.Empty; public long FolioInicial { get; set; } = 0; }
public sealed class ActualizarSerieDocumentoRequest { public int IdSerie { get; set; } public long? FolioActual { get; set; } public bool? IsActive { get; set; } }
public sealed class FiltroSerieDocumentoRequest { public string? Modulo { get; set; } public bool? IsActive { get; set; } = true; public int Page { get; set; } = 1; public int PageSize { get; set; } = 50; }

// ── ParametroEmpresa ──────────────────────────────────────────────────────
public sealed class ParametroEmpresaResponse
{
    public int IdEmpresa { get; init; }
    public string Clave { get; init; } = string.Empty;
    public string Valor { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public int Version { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? UpdatedUtc { get; init; }
}
public sealed class UpsertParametroEmpresaRequest { public string Clave { get; set; } = string.Empty; public string Valor { get; set; } = string.Empty; public string? Descripcion { get; set; } }
public sealed class FiltroParametroRequest { public string? Search { get; set; } public bool? IsActive { get; set; } = true; public int Page { get; set; } = 1; public int PageSize { get; set; } = 50; }

// ── ParametroSucursal ─────────────────────────────────────────────────────
public sealed class ParametroSucursalResponse
{
    public int IdSucursal { get; init; }
    public string NombreSucursal { get; init; } = string.Empty;
    public int IdEmpresa { get; init; }
    public string Clave { get; init; } = string.Empty;
    public string Valor { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public int Version { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? UpdatedUtc { get; init; }
}
public sealed class UpsertParametroSucursalRequest { public int IdSucursal { get; set; } public string Clave { get; set; } = string.Empty; public string Valor { get; set; } = string.Empty; public string? Descripcion { get; set; } }
public sealed class FiltroParametroSucursalRequest { public int? IdSucursal { get; set; } public string? Search { get; set; } public bool? IsActive { get; set; } = true; public int Page { get; set; } = 1; public int PageSize { get; set; } = 50; }

// ── Dashboard ─────────────────────────────────────────────────────────────
public sealed class DashboardResumenResponse
{
    public long ClientesActivos { get; init; }
    public long ClientesConCredito { get; init; }
    public long ProductosActivos { get; init; }
    public long ServiciosActivos { get; init; }
    public long ProveedoresActivos { get; init; }
    public long SucursalesActivas { get; init; }
    public long UsuariosActivos { get; init; }
    public long SeriesDocumentoActivas { get; init; }
    public bool AlertaSinSeries { get; init; }
    public bool AlertaSinMoneda { get; init; }
}

// ═══════════════════════════════════════════════════════════════════════════
// SERVICIOS NUEVOS
// ═══════════════════════════════════════════════════════════════════════════

public sealed class ClienteService(ApiClient api)
{
    public Task<(PagedResult<ClienteResponse>? D, string? E)> GetPagedAsync(FiltroClienteRequest f)
    {
        var extra = "";
        if (f.BloquearCredito.HasValue) extra += $"&bloquearCredito={f.BloquearCredito.Value.ToString().ToLower()}";
        if (f.IdRuta.HasValue) extra += $"&idRuta={f.IdRuta.Value}";
        return api.GetAsync<PagedResult<ClienteResponse>>(
            $"api/v1/clientes?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize, extra)}");
    }
    public Task<(ClienteResponse? D, string? E)> GetByIdAsync(int id) => api.GetAsync<ClienteResponse>($"api/v1/clientes/{id}");
    public Task<(ApiWriteResult? D, string? E)> CrearAsync(CrearClienteRequest r) => api.PostAsync<ApiWriteResult>("api/v1/clientes", r);
    public Task<(ApiWriteResult? D, string? E)> ActualizarAsync(int id, ActualizarClienteRequest r) => api.PutAsync<ApiWriteResult>($"api/v1/clientes/{id}", r);
    public Task<(bool Ok, string? E)> EliminarAsync(int id) => api.DeleteAsync($"api/v1/clientes/{id}");
}

public sealed class ProductoService(ApiClient api)
{
    public Task<(PagedResult<ProductoResponse>? D, string? E)> GetPagedAsync(FiltroProductoRequest f)
    {
        var extra = "";
        if (f.IdClasifCategoria.HasValue) extra += $"&idClasifCategoria={f.IdClasifCategoria.Value}";
        if (f.EsServicio.HasValue) extra += $"&esServicio={f.EsServicio.Value.ToString().ToLower()}";
        if (f.ManejaInventario.HasValue) extra += $"&manejaInventario={f.ManejaInventario.Value.ToString().ToLower()}";
        return api.GetAsync<PagedResult<ProductoResponse>>(
            $"api/v1/productos?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize, extra)}");
    }
    public Task<(ProductoResponse? D, string? E)> GetByIdAsync(int id) => api.GetAsync<ProductoResponse>($"api/v1/productos/{id}");
    public Task<(ApiWriteResult? D, string? E)> CrearAsync(CrearProductoRequest r) => api.PostAsync<ApiWriteResult>("api/v1/productos", r);
    public Task<(ApiWriteResult? D, string? E)> ActualizarAsync(int id, ActualizarProductoRequest r) => api.PutAsync<ApiWriteResult>($"api/v1/productos/{id}", r);
    public Task<(bool Ok, string? E)> EliminarAsync(int id) => api.DeleteAsync($"api/v1/productos/{id}");
}

public sealed class EmpresaService(ApiClient api)
{
    // Solo lectura del tenant activo — la lista completa es del panel SaaS admin
    public Task<(EmpresaResponse? D, string? E)> GetByIdAsync(int id)
        => api.GetAsync<EmpresaResponse>($"api/v1/empresas/{id}");
}

public sealed class MonedaService(ApiClient api)
{
    public Task<(PagedResult<MonedaResponse>? D, string? E)> GetPagedAsync(FiltroMonedaRequest f)
        => api.GetAsync<PagedResult<MonedaResponse>>($"api/v1/monedas?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize)}");
    public Task<(MonedaResponse? D, string? E)> GetByIdAsync(string codigo) => api.GetAsync<MonedaResponse>($"api/v1/monedas/{codigo}");
    public Task<(ApiWriteResult? D, string? E)> CrearAsync(CrearMonedaRequest r) => api.PostAsync<ApiWriteResult>("api/v1/monedas", r);
    public Task<(ApiWriteResult? D, string? E)> ActualizarAsync(string codigo, ActualizarMonedaRequest r) => api.PutAsync<ApiWriteResult>($"api/v1/monedas/{codigo}", r);
    public Task<(bool Ok, string? E)> EliminarAsync(string codigo) => api.DeleteAsync($"api/v1/monedas/{codigo}");
}

public sealed class SerieDocumentoService(ApiClient api)
{
    public Task<(PagedResult<SerieDocumentoResponse>? D, string? E)> GetPagedAsync(FiltroSerieDocumentoRequest f)
    {
        var extra = f.Modulo != null ? $"&modulo={f.Modulo}" : "";
        return api.GetAsync<PagedResult<SerieDocumentoResponse>>(
            $"api/v1/seriesdocumento?{Qs.Build(null, f.IsActive, f.Page, f.PageSize, extra)}");
    }
    public Task<(ApiWriteResult? D, string? E)> CrearAsync(CrearSerieDocumentoRequest r) => api.PostAsync<ApiWriteResult>("api/v1/seriesdocumento", r);
    public Task<(ApiWriteResult? D, string? E)> ActualizarAsync(int id, ActualizarSerieDocumentoRequest r) => api.PutAsync<ApiWriteResult>($"api/v1/seriesdocumento/{id}", r);
    public Task<(bool Ok, string? E)> EliminarAsync(int id) => api.DeleteAsync($"api/v1/seriesdocumento/{id}");
}

public sealed class ParametroEmpresaService(ApiClient api)
{
    public Task<(PagedResult<ParametroEmpresaResponse>? D, string? E)> GetPagedAsync(FiltroParametroRequest f)
        => api.GetAsync<PagedResult<ParametroEmpresaResponse>>($"api/v1/parametrosempresa?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize)}");
    public Task<(ApiWriteResult? D, string? E)> UpsertAsync(UpsertParametroEmpresaRequest r) => api.PostAsync<ApiWriteResult>("api/v1/parametrosempresa", r);
    public Task<(bool Ok, string? E)> EliminarAsync(string clave) => api.DeleteAsync($"api/v1/parametrosempresa/{clave}");
}

public sealed class ParametroSucursalService(ApiClient api)
{
    public Task<(PagedResult<ParametroSucursalResponse>? D, string? E)> GetPagedAsync(FiltroParametroSucursalRequest f)
    {
        var extra = f.IdSucursal.HasValue ? $"&idSucursal={f.IdSucursal.Value}" : "";
        return api.GetAsync<PagedResult<ParametroSucursalResponse>>(
            $"api/v1/parametrossucursal?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize, extra)}");
    }
    public Task<(ApiWriteResult? D, string? E)> UpsertAsync(UpsertParametroSucursalRequest r) => api.PostAsync<ApiWriteResult>("api/v1/parametrossucursal", r);
    public Task<(bool Ok, string? E)> EliminarAsync(int idSucursal, string clave) => api.DeleteAsync($"api/v1/parametrossucursal/{idSucursal}/{clave}");
}

public sealed class DashboardService(ApiClient api)
{
    public Task<(DashboardResumenResponse? D, string? E)> GetResumenAsync()
        => api.GetAsync<DashboardResumenResponse>("api/v1/dashboard/resumen");
}



// ── Usuarios ──────────────────────────────────────────────────────────────
public sealed class UsuarioResponse
{
    public int IdUsuario { get; init; }
    public int IdEmpresa { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Telefono { get; init; }
    public bool Bloqueado { get; init; }
    public bool RequiereCambio { get; init; }
    public int IntentosFallidos { get; init; }
    public DateTime? UltimoLoginUtc { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedUtc { get; init; }
    public DateTime? UpdatedUtc { get; init; }
    public int? IdRol { get; init; }
    public string? CodigoRol { get; init; }
    public string? NombreRol { get; init; }
    public int TotalSucursales { get; init; }
}
public sealed class UsuarioSucursalResponse
{
    public int IdSucursal { get; init; }
    public string NombreSucursal { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public DateTime CreatedUtc { get; init; }
}
public sealed class CrearUsuarioRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public int IdRol { get; set; }
}
public sealed class ActualizarUsuarioRequest
{
    public int IdUsuario { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public int? IdRol { get; set; }
    public bool? Bloqueado { get; set; }
    public bool? RequiereCambio { get; set; }
    public bool? IsActive { get; set; }
}
public sealed class FiltroUsuarioRequest
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; } = true;
    public bool? Bloqueado { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
public sealed class AsignarSucursalRequest
{
    public int IdUsuario { get; set; }
    public int IdSucursal { get; set; }
    public bool Asignar { get; set; } = true;
}

// ── Roles ─────────────────────────────────────────────────────────────────
public sealed class RolResponse
{
    public int IdRol { get; init; }
    public int IdEmpresa { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public bool EsSistema { get; init; }
    public int Orden { get; init; }
    public bool IsActive { get; init; }
    public int TotalUsuarios { get; init; }
    public int TotalPermisos { get; init; }
}
public sealed class PermisoMatrizItem
{
    public int IdModulo { get; init; }
    public string ModuloCodigo { get; init; } = string.Empty;
    public string ModuloNombre { get; init; } = string.Empty;
    public string? ModuloIcono { get; init; }
    public int ModuloOrden { get; init; }
    public int IdAccion { get; init; }
    public string AccionCodigo { get; init; } = string.Empty;
    public string AccionNombre { get; init; } = string.Empty;
    public bool EsSensible { get; init; }
    public bool Allow { get; init; }
}
public sealed class CrearRolRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Orden { get; set; } = 99;
}
public sealed class ActualizarRolRequest
{
    public int IdRol { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public int? Orden { get; set; }
    public bool? IsActive { get; set; }
}
public sealed class FiltroRolRequest
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
public sealed class UpsertPermisoRequest
{
    public int IdRol { get; set; }
    public int IdModulo { get; set; }
    public int IdAccion { get; set; }
    public bool Allow { get; set; }
}

// ── Módulos y Acciones ────────────────────────────────────────────────────
public sealed class ModuloResponse
{
    public int IdModulo { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public string? Icono { get; init; }
    public int Orden { get; init; }
    public bool EsSensible { get; init; }
    public bool IsActive { get; init; }
}
public sealed class AccionResponse
{
    public int IdAccion { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public bool EsSensible { get; init; }
    public bool IsActive { get; init; }
}

// ── Sesiones y LoginLog ───────────────────────────────────────────────────
public sealed class SesionActivaResponse
{
    public int IdSesion { get; init; }
    public int IdUsuario { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime FechaInicio { get; init; }
    public DateTime FechaExpira { get; init; }
    public bool Vigente { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgentShort { get; init; }
    public bool IsActive { get; init; }
}
public sealed class LoginLogResponse
{
    public int IdLoginLog { get; init; }
    public int? IdUsuario { get; init; }
    public string Username { get; init; } = string.Empty;
    public string? IpAddress { get; init; }
    public string? UserAgentShort { get; init; }
    public bool Exitoso { get; init; }
    public DateTime CreatedUtc { get; init; }
}
public sealed class FiltroSesionRequest
{
    public int? IdUsuario { get; set; }
    public bool SoloActivas { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
public sealed class FiltroLoginLogRequest
{
    public int? IdUsuario { get; set; }
    public bool? SoloFallidos { get; set; }
    public DateTime? FechaDesde { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

// ── Suscripción ───────────────────────────────────────────────────────────
public sealed class SuscripcionResponse
{
    public int IdSuscripcion { get; init; }
    public int IdPlan { get; init; }
    public string NombrePlan { get; init; } = string.Empty;
    public string? DescripcionPlan { get; init; }
    public decimal PrecioMensual { get; init; }
    public int MaxUsuarios { get; init; }
    public int MaxSucursales { get; init; }
    public int MaxAlmacenes { get; init; }
    public int MaxApiCallsDia { get; init; }
    public decimal MaxGbStorage { get; init; }
    public bool EsLite { get; init; }
    public string Estado { get; init; } = string.Empty;
    public DateTime FechaInicio { get; init; }
    public DateTime FechaFin { get; init; }
    public int GraceDays { get; init; }
    public int DiasRestantes { get; init; }
    public bool EnGracePeriod { get; init; }
    public string EstadoEmpresa { get; init; } = string.Empty;
    public IEnumerable<ModuloHabilitadoResponse> Modulos { get; init; } = [];
}
public sealed class ModuloHabilitadoResponse
{
    public int IdModulo { get; init; }
    public string Clave { get; init; } = string.Empty;
    public string Nombre { get; init; } = string.Empty;
    public bool Activo { get; init; }
}

// ══════════════════════════════════════════════════════════════════════════
// SERVICIOS
// ══════════════════════════════════════════════════════════════════════════

public sealed class UsuarioService(ApiClient api)
{
    public Task<(PagedResult<UsuarioResponse>? D, string? E)> GetPagedAsync(FiltroUsuarioRequest f)
    {
        var extra = "";
        if (f.Bloqueado.HasValue) extra += $"&bloqueado={f.Bloqueado.Value.ToString().ToLower()}";
        return api.GetAsync<PagedResult<UsuarioResponse>>(
            $"api/v1/usuarios?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize, extra)}");
    }
    public Task<(object? D, string? E)> GetByIdAsync(int id)
        => api.GetAsync<object>($"api/v1/usuarios/{id}");
    public Task<(ApiWriteResult? D, string? E)> CrearAsync(CrearUsuarioRequest r)
        => api.PostAsync<ApiWriteResult>("api/v1/usuarios", r);
    public Task<(ApiWriteResult? D, string? E)> ActualizarAsync(int id, ActualizarUsuarioRequest r)
        => api.PutAsync<ApiWriteResult>($"api/v1/usuarios/{id}", r);
    public Task<(bool Ok, string? E)> EliminarAsync(int id)
        => api.DeleteAsync($"api/v1/usuarios/{id}");
    public Task<(ApiWriteResult? D, string? E)> GestionarSucursalAsync(AsignarSucursalRequest r)
        => api.PostAsync<ApiWriteResult>($"api/v1/usuarios/{r.IdUsuario}/sucursales", r);
}

public sealed class RolService(ApiClient api)
{
    public Task<(PagedResult<RolResponse>? D, string? E)> GetPagedAsync(FiltroRolRequest f)
        => api.GetAsync<PagedResult<RolResponse>>(
            $"api/v1/roles?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize)}");
    public Task<(object? D, string? E)> GetByIdAsync(int id)
        => api.GetAsync<object>($"api/v1/roles/{id}");
    public Task<(ApiWriteResult? D, string? E)> CrearAsync(CrearRolRequest r)
        => api.PostAsync<ApiWriteResult>("api/v1/roles", r);
    public Task<(ApiWriteResult? D, string? E)> ActualizarAsync(int id, ActualizarRolRequest r)
        => api.PutAsync<ApiWriteResult>($"api/v1/roles/{id}", r);
    public Task<(bool Ok, string? E)> EliminarAsync(int id)
        => api.DeleteAsync($"api/v1/roles/{id}");
    public Task<(ApiWriteResult? D, string? E)> UpsertPermisoAsync(int idRol, UpsertPermisoRequest r)
        => api.PostAsync<ApiWriteResult>($"api/v1/roles/{idRol}/permisos", r);
    public Task<(IEnumerable<ModuloResponse>? D, string? E)> GetModulosAsync()
        => api.GetAsync<IEnumerable<ModuloResponse>>("api/v1/roles/modulos");
    public Task<(IEnumerable<AccionResponse>? D, string? E)> GetAccionesAsync()
        => api.GetAsync<IEnumerable<AccionResponse>>("api/v1/roles/acciones");
}

public sealed class SesionWebService(ApiClient api)
{
    public Task<(PagedResult<SesionActivaResponse>? D, string? E)> GetSesionesAsync(FiltroSesionRequest f)
    {
        var extra = f.IdUsuario.HasValue ? $"&idUsuario={f.IdUsuario.Value}" : "";
        extra += $"&soloActivas={f.SoloActivas.ToString().ToLower()}";
        return api.GetAsync<PagedResult<SesionActivaResponse>>(
            $"api/v1/sesiones?{Qs.Build(null, null, f.Page, f.PageSize, extra)}");
    }
    public Task<(PagedResult<LoginLogResponse>? D, string? E)> GetLoginLogAsync(FiltroLoginLogRequest f)
    {
        var extra = "";
        if (f.IdUsuario.HasValue) extra += $"&idUsuario={f.IdUsuario.Value}";
        if (f.SoloFallidos.HasValue) extra += $"&soloFallidos={f.SoloFallidos.Value.ToString().ToLower()}";
        if (f.FechaDesde.HasValue) extra += $"&fechaDesde={f.FechaDesde.Value:yyyy-MM-dd}";
        return api.GetAsync<PagedResult<LoginLogResponse>>(
            $"api/v1/sesiones/loginlog?{Qs.Build(null, null, f.Page, f.PageSize, extra)}");
    }
}

public sealed class SuscripcionWebService(ApiClient api)
{
    public Task<(SuscripcionResponse? D, string? E)> GetAsync()
        => api.GetAsync<SuscripcionResponse>("api/v1/suscripcion");
    public Task<(IEnumerable<object>? D, string? E)> GetPlanesAsync()
        => api.GetAsync<IEnumerable<object>>("api/v1/suscripcion/planes");
}