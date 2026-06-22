using webVentasSaaSV25.Services.Catalogos;
using webVentasSaaSV25.Services.Http;

namespace webVentasSaaSV25.Services.Hmr;

public sealed class EmpleadoResponse
{
    public int IdEmpleado { get; init; }
    public int IdEmpresa { get; init; }
    public int? IdDepartamento { get; init; }
    public string? NombreDepartamento { get; init; }
    public int? IdPuesto { get; init; }
    public string? NombrePuesto { get; init; }
    public int? IdTurno { get; init; }
    public int? IdSucursalBase { get; init; }
    public string? NombreSucursalBase { get; init; }
    public string? NumeroEmpleado { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string ApellidoPat { get; init; } = string.Empty;
    public string? ApellidoMat { get; init; }
    public string NombreCompleto { get; init; } = string.Empty;
    public DateOnly? FechaNacimiento { get; init; }
    public DateOnly FechaIngreso { get; init; }
    public string? Rfc { get; init; }
    public string? Curp { get; init; }
    public string? Nss { get; init; }
    public string? Telefono { get; init; }
    public string? Email { get; init; }
    public string? FotoUrl { get; init; }
    public bool IsActive { get; init; }
    public int? IdUsuario { get; init; }
    public string? Username { get; init; }
    public string? EmailUsuario { get; init; }
}

public sealed class EmpleadoLookupItem
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
}

public sealed class EmpleadoLookupsResponse
{
    public List<EmpleadoLookupItem> Departamentos { get; init; } = [];
    public List<EmpleadoLookupItem> Puestos { get; init; } = [];
    public List<EmpleadoLookupItem> Sucursales { get; init; } = [];
    public List<EmpleadoLookupItem> UsuariosDisponibles { get; init; } = [];
}

public sealed class FiltroEmpleadoRequest
{
    public string? Search { get; set; }
    public bool? IsActive { get; set; } = true;
    public int? IdSucursalBase { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class GuardarEmpleadoRequest
{
    public int? IdDepartamento { get; set; }
    public int? IdPuesto { get; set; }
    public int? IdTurno { get; set; }
    public int? IdSucursalBase { get; set; }
    public string? NumeroEmpleado { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string ApellidoPat { get; set; } = string.Empty;
    public string? ApellidoMat { get; set; }
    public DateOnly? FechaNacimiento { get; set; }
    public DateOnly? FechaIngreso { get; set; }
    public string? Rfc { get; set; }
    public string? Curp { get; set; }
    public string? Nss { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? FotoUrl { get; set; }
    public bool? IsActive { get; set; }
}

public sealed class ActualizarEmpleadoRequest : GuardarEmpleadoRequest
{
    public int IdEmpleado { get; set; }
}

public sealed class VincularEmpleadoUsuarioRequest
{
    public int IdEmpleado { get; set; }
    public int IdUsuario { get; set; }
    public bool Asignar { get; set; } = true;
}

public sealed class EmpleadoWebService(ApiClient api)
{
    public Task<(PagedResult<EmpleadoResponse>? D, string? E)> GetPagedAsync(FiltroEmpleadoRequest f)
    {
        var extra = "";
        if (f.IdSucursalBase.HasValue) extra += $"&idSucursalBase={f.IdSucursalBase.Value}";
        return api.GetAsync<PagedResult<EmpleadoResponse>>(
            $"api/v1/empleados?{Qs.Build(f.Search, f.IsActive, f.Page, f.PageSize, extra)}");
    }

    public Task<(EmpleadoResponse? D, string? E)> GetByIdAsync(int id)
        => api.GetAsync<EmpleadoResponse>($"api/v1/empleados/{id}");

    public Task<(EmpleadoLookupsResponse? D, string? E)> GetLookupsAsync()
        => api.GetAsync<EmpleadoLookupsResponse>("api/v1/empleados/lookups");

    public Task<(ApiWriteResult? D, string? E)> CrearAsync(GuardarEmpleadoRequest r)
        => api.PostAsync<ApiWriteResult>("api/v1/empleados", r);

    public Task<(ApiWriteResult? D, string? E)> ActualizarAsync(int id, ActualizarEmpleadoRequest r)
        => api.PutAsync<ApiWriteResult>($"api/v1/empleados/{id}", r);

    public Task<(bool Ok, string? E)> EliminarAsync(int id)
        => api.DeleteAsync($"api/v1/empleados/{id}");

    public Task<(ApiWriteResult? D, string? E)> VincularUsuarioAsync(int id, VincularEmpleadoUsuarioRequest r)
        => api.PostAsync<ApiWriteResult>($"api/v1/empleados/{id}/usuario", r);
}
