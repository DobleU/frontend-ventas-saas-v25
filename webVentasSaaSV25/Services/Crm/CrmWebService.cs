using webVentasSaaSV25.Services.Catalogos;
using webVentasSaaSV25.Services.Http;

namespace webVentasSaaSV25.Services.Crm;

public sealed class CrmWebService(ApiClient api)
{
    public Task<(List<CrmEscenarioListItemResponse>? D, string? E)> GetEscenariosAsync(bool soloActivos = true, bool soloAprobados = false)
        => api.GetAsync<List<CrmEscenarioListItemResponse>>($"api/crm/escenarios?SoloActivos={soloActivos}&SoloAprobados={soloAprobados}");

    public Task<(CrmEscenarioDetalleResponse? D, string? E)> GetEscenarioAsync(long id)
        => api.GetAsync<CrmEscenarioDetalleResponse>($"api/crm/escenarios/{id}");

    public Task<(ApiWriteResult? D, string? E)> CrearEscenarioAsync(CrmEscenarioGuardarRequest request)
        => api.PostAsync<ApiWriteResult>("api/crm/escenarios", request);

    public Task<(ApiWriteResult? D, string? E)> ActualizarEscenarioAsync(long id, CrmEscenarioGuardarRequest request)
        => api.PutAsync<ApiWriteResult>($"api/crm/escenarios/{id}", request);

    public Task<(ApiWriteResult? D, string? E)> AprobarEscenarioAsync(long id, bool aprobado = true)
        => api.PostAsync<ApiWriteResult>($"api/crm/escenarios/{id}/aprobar", new { aprobado });

    public Task<(ApiWriteResult? D, string? E)> ActivarEscenarioAsync(long id, bool isActive)
        => api.PostAsync<ApiWriteResult>($"api/crm/escenarios/{id}/activar", new { isActive });

    public Task<(List<CrmSeguimientoResponse>? D, string? E)> GetSeguimientosAsync(DateTime? fechaIniUtc = null, DateTime? fechaFinUtc = null, bool soloPendientes = false)
    {
        var qs = $"SoloPendientes={soloPendientes}";
        if (fechaIniUtc is not null) qs += $"&FechaIniUtc={Uri.EscapeDataString(fechaIniUtc.Value.ToString("O"))}";
        if (fechaFinUtc is not null) qs += $"&FechaFinUtc={Uri.EscapeDataString(fechaFinUtc.Value.ToString("O"))}";
        return api.GetAsync<List<CrmSeguimientoResponse>>($"api/crm/seguimientos?{qs}");
    }

    public Task<(ApiWriteResult? D, string? E)> CrearSeguimientoAsync(CrmSeguimientoGuardarRequest request)
        => api.PostAsync<ApiWriteResult>("api/crm/seguimientos", request);

    public Task<(ApiWriteResult? D, string? E)> ActualizarSeguimientoAsync(long id, CrmSeguimientoGuardarRequest request)
        => api.PutAsync<ApiWriteResult>($"api/crm/seguimientos/{id}", request);

    public Task<(ApiWriteResult? D, string? E)> RealizarSeguimientoAsync(long id)
        => api.PostAsync<ApiWriteResult>($"api/crm/seguimientos/{id}/realizar", new { fechaRealizadoUtc = DateTime.UtcNow });

    public Task<(ApiWriteResult? D, string? E)> CancelarSeguimientoAsync(long id)
        => api.PostAsync<ApiWriteResult>($"api/crm/seguimientos/{id}/cancelar", new { fechaCanceladoUtc = DateTime.UtcNow });

    public Task<(CrmSyncProspectosResultadosResponse? D, string? E)> GetProspectosResultadosAsync(int top = 500)
        => api.GetAsync<CrmSyncProspectosResultadosResponse>($"api/app/sync/crm/prospectos-resultados?Top={top}");

    public async Task<(long? D, string? E)> CrearProspectoAsync(CrmProspectoReplicarRequest request)
    {
        var (d, e) = await api.PostAsync<long>("api/app/sync/crm/prospectos", request);
        return (d, e);
    }
}

public sealed record CrmEscenarioListItemResponse
{
    public long id_escenario { get; init; }
    public string? codigo { get; init; }
    public string nombre { get; init; } = string.Empty;
    public string? descripcion { get; init; }
    public string? situacion_cliente { get; init; }
    public string? objetivo { get; init; }
    public string? etapa { get; init; }
    public string? tono { get; init; }
    public int prioridad { get; init; }
    public bool aplica_global { get; init; }
    public int version { get; init; }
    public bool aprobado { get; init; }
    public bool is_active { get; init; }
    public DateTime created_utc { get; init; }
    public DateTime? updated_utc { get; init; }
}

public sealed record CrmEscenarioDetalleResponse
{
    public CrmEscenarioListItemResponse? Escenario { get; init; }
    public List<CrmEscenarioTipoNegocioResponse> TiposNegocio { get; init; } = [];
    public List<CrmEscenarioTagResponse> Tags { get; init; } = [];
    public List<CrmEscenarioRespuestaResponse> Respuestas { get; init; } = [];
}

public sealed record CrmEscenarioTipoNegocioResponse
{
    public int id_tipo_negocio_item { get; init; }
    public string? tipo_negocio { get; init; }
    public int peso { get; init; }
}

public sealed record CrmEscenarioTagResponse
{
    public int id_tag_item { get; init; }
    public string? tag { get; init; }
    public int peso { get; init; }
    public bool es_tag_requerido { get; init; }
}

public sealed record CrmEscenarioRespuestaResponse
{
    public long id_respuesta { get; init; }
    public string titulo { get; init; } = string.Empty;
    public string texto_respuesta { get; init; } = string.Empty;
    public string? tono { get; init; }
    public int orden { get; init; }
    public bool es_principal { get; init; }
    public bool permite_reproducir { get; init; }
    public bool aprobado { get; init; }
    public bool is_active { get; init; }
}

public sealed class CrmEscenarioGuardarRequest
{
    public long? IdEscenario { get; set; }
    public string? Codigo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? SituacionCliente { get; set; }
    public string Objetivo { get; set; } = string.Empty;
    public int Prioridad { get; set; } = 100;
    public bool AplicaGlobal { get; set; } = true;
    public bool RequiereTipoNegocio { get; set; }
    public bool Aprobado { get; set; }
    public bool IsActive { get; set; } = true;
    public List<CrmEscenarioRespuestaRequest> Respuestas { get; set; } = [];
}

public sealed class CrmEscenarioRespuestaRequest
{
    public long? id_respuesta { get; set; }
    public string titulo { get; set; } = string.Empty;
    public string texto_respuesta { get; set; } = string.Empty;
    public int orden { get; set; } = 1;
    public bool es_principal { get; set; } = true;
    public bool permite_reproducir { get; set; } = true;
    public bool aprobado { get; set; }
}

public sealed record CrmSeguimientoResponse
{
    public long id_seguimiento { get; init; }
    public long? id_prospecto { get; init; }
    public string? nombre_negocio { get; init; }
    public string? telefono_prospecto { get; init; }
    public string? whatsapp_prospecto { get; init; }
    public int? id_cliente { get; init; }
    public int? id_vendedor_asignado { get; init; }
    public int? id_ruta_asignada { get; init; }
    public string? estatus { get; init; }
    public string? clave_estatus { get; init; }
    public string? tipo_accion { get; init; }
    public string? escenario_sugerido { get; init; }
    public DateTime? fecha_programada_utc { get; init; }
    public DateTime? fecha_realizado_utc { get; init; }
    public DateTime? fecha_cancelado_utc { get; init; }
    public string? titulo { get; init; }
    public string? descripcion_accion { get; init; }
    public string? ultimo_resumen { get; init; }
    public string? telefono_contacto { get; init; }
    public byte prioridad { get; init; }
    public bool is_active { get; init; }
}

public sealed class CrmSeguimientoGuardarRequest
{
    public long? IdSeguimiento { get; set; }
    public long? IdProspecto { get; set; }
    public int? IdCliente { get; set; }
    public string? ClaveEstatus { get; set; } = "PENDIENTE";
    public string? ClaveTipoAccion { get; set; } = "VISITA";
    public DateTime? FechaProgramadaUtc { get; set; } = DateTime.UtcNow.AddDays(1);
    public string Titulo { get; set; } = string.Empty;
    public string? DescripcionAccion { get; set; }
    public string? TelefonoContacto { get; set; }
    public byte Prioridad { get; set; } = 2;
    public bool IsActive { get; set; } = true;
}

public sealed record CrmSyncProspectosResultadosResponse
{
    public List<CrmProspectoSyncDto> Prospectos { get; init; } = [];
    public List<CrmProspectoVisitaSyncDto> Visitas { get; init; } = [];
    public List<CrmProspectoVisitaTagSyncDto> TagsVisita { get; init; } = [];
    public List<CrmDialogoSyncDto> Dialogos { get; init; } = [];
}

public sealed record CrmProspectoSyncDto
{
    public long id_prospecto { get; init; }
    public string? clave_movil { get; init; }
    public string nombre_negocio { get; init; } = string.Empty;
    public string? nombre_dueno { get; init; }
    public string? nombre_encargado { get; init; }
    public string? telefono { get; init; }
    public string? whatsapp { get; init; }
    public string? direccion_texto { get; init; }
    public string? colonia { get; init; }
    public string? municipio { get; init; }
    public string? estado { get; init; }
    public byte? nivel_interes { get; init; }
    public byte? nivel_potencial { get; init; }
    public string? productos_interes { get; init; }
    public string? comentario_final { get; init; }
    public string? observaciones { get; init; }
    public bool is_active { get; init; }
    public DateTime created_utc { get; init; }
}

public sealed class CrmProspectoReplicarRequest
{
    public string ClaveMovil { get; set; } = $"WEB-PROS-{Guid.NewGuid():N}";
    public int? IdTipoNegocioItem { get; set; }
    public int? IdEstadoProspectoItem { get; set; }
    public int? IdRutaOrigen { get; set; }
    public int? IdVendedorOrigen { get; set; }
    public long? IdRecorridoOrigen { get; set; }
    public long? IdVisitaOrigen { get; set; }
    public string NombreNegocio { get; set; } = string.Empty;
    public string? RazonSocial { get; set; }
    public string? Rfc { get; set; }
    public string? NombreDueno { get; set; }
    public string? NombreEncargado { get; set; }
    public string? Telefono { get; set; }
    public string? Whatsapp { get; set; }
    public string? Correo { get; set; }
    public string? DireccionTexto { get; set; }
    public string? Calle { get; set; }
    public string? NumeroExterior { get; set; }
    public string? Colonia { get; set; }
    public string? CodigoPostal { get; set; }
    public string? Municipio { get; set; }
    public string? Estado { get; set; }
    public string? Referencia { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public decimal? PrecisionMetros { get; set; }
    public byte? NivelInteres { get; set; } = 3;
    public byte? NivelPotencial { get; set; } = 3;
    public int? Score { get; set; }
    public string? ProveedorActual { get; set; }
    public string? ProductosInteres { get; set; }
    public decimal? PotencialMensual { get; set; }
    public string? FrecuenciaCompra { get; set; }
    public bool RequiereCredito { get; set; }
    public bool RequiereFactura { get; set; }
    public string? HorarioRecomendado { get; set; }
    public string? DiaRecomendado { get; set; }
    public string? ComentarioFinal { get; set; }
    public string? Observaciones { get; set; }
}

public sealed record CrmProspectoVisitaSyncDto
{
    public long id_prospecto_visita { get; init; }
    public long id_prospecto { get; init; }
    public int? id_resultado_visita_item { get; init; }
    public DateTime fecha_inicio_utc { get; init; }
    public DateTime? fecha_fin_utc { get; init; }
    public string? persona_contactada { get; init; }
    public string? telefono_capturado { get; init; }
    public string? comentario { get; init; }
    public bool requiere_seguimiento { get; init; }
    public DateTime? fecha_proximo_seguimiento_utc { get; init; }
    public string? texto_siguiente_accion { get; init; }
}

public sealed record CrmProspectoVisitaTagSyncDto
{
    public long id_prospecto_visita { get; init; }
    public long id_prospecto { get; init; }
    public int id_tag_item { get; init; }
    public string? nombre { get; init; }
    public string? comentario { get; init; }
}

public sealed record CrmDialogoSyncDto
{
    public long id_dialogo { get; init; }
    public long id_prospecto_visita { get; init; }
    public long id_prospecto { get; init; }
    public string? resumen_dialogo { get; init; }
    public string? comentario_final { get; init; }
}
