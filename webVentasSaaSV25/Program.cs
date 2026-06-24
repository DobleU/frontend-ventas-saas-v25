

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using webVentasSaaSV25;
using webVentasSaaSV25.Services.Auth;
using webVentasSaaSV25.Services.Catalogos;
using webVentasSaaSV25.Services.Http;
using webVentasSaaSV25.State;
using webVentasSaaSV25.Services.Almacen;
using webVentasSaaSV25.Services.Monitor;
using webVentasSaaSV25.Services.Crm;
using webVentasSaaSV25.Services.Hmr;
using webVentasSaaSV25.Services.Pedidos;
using webVentasSaaSV25.Services.Sec;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ─── CONFIGURACIÓN ────────────────────────────────────────────────────────────
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl no configurado en wwwroot/appsettings.json");

// ─── ESTADO GLOBAL ────────────────────────────────────────────────────────────
// Singleton: vive toda la sesión de la pestaña, no depende de IJSRuntime.
builder.Services.AddSingleton<AppState>();

// ─── AUTENTICACIÓN ────────────────────────────────────────────────────────────
// Scoped porque dependen de IJSRuntime (Scoped en WASM).
// En Blazor WASM hay un solo Scope por pestaña — equivale a Singleton en práctica.
builder.Services.AddScoped<PermisoClienteService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MenuClienteService>();

// ─── HTTP CLIENTS ─────────────────────────────────────────────────────────────
// Requiere NuGet: Microsoft.Extensions.Http 8.0.0
// Cliente autenticado: Bearer token + refresh automático vía TokenRefreshHandler
builder.Services.AddScoped<TokenRefreshHandler>();

builder.Services.AddHttpClient("VentasSaaSAPI", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<TokenRefreshHandler>();

// Cliente público: solo para Login y Refresh (sin token todavía)
builder.Services.AddHttpClient("VentasSaaSPublic", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(15);
});

// ApiClient: wrapper genérico que usa el cliente autenticado
builder.Services.AddScoped<ApiClient>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var authService = sp.GetRequiredService<AuthService>();
    var appState = sp.GetRequiredService<AppState>();
    var http = factory.CreateClient("VentasSaaSAPI");
    return new ApiClient(http, authService, appState);
});

// ─── CATÁLOGOS GENERALES ──────────────────────────────────────────────────────
builder.Services.AddScoped<ZonaService>();
builder.Services.AddScoped<RutaService>();
builder.Services.AddScoped<UnidadService>();
builder.Services.AddScoped<ClasificacionService>();
builder.Services.AddScoped<CatalogoTipoService>();
builder.Services.AddScoped<CatalogoItemService>();
builder.Services.AddScoped<ImpuestoService>();
builder.Services.AddScoped<ProveedorService>();

// ─── CATÁLOGOS FASE 1 ─────────────────────────────────────────────────────────
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<ProductoService>();

// ─── CORE / ESTRUCTURA EMPRESA ────────────────────────────────────────────────
builder.Services.AddScoped<SucursalService>();
builder.Services.AddScoped<EmpresaService>();
builder.Services.AddScoped<MonedaService>();
builder.Services.AddScoped<SerieDocumentoService>();
builder.Services.AddScoped<ParametroEmpresaService>();
builder.Services.AddScoped<ParametroSucursalService>();

// ─── REPORTES / DASHBOARD ─────────────────────────────────────────────────────
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<MonitorMapaService>();

// ─── INVENTARIO / ALMACENES ───────────────────────────────────────────────────
builder.Services.AddScoped<InventarioAlmacenWebService>();
builder.Services.AddScoped<PedidoClienteWebService>();

// ─── CRM ──────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<CrmWebService>();


// ── Seguridad /  ────────────────────────────────────────────────────
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<RolService>();
builder.Services.AddScoped<SesionWebService>();
builder.Services.AddScoped<SuscripcionWebService>();
builder.Services.AddScoped<EmpleadoWebService>();


await builder.Build().RunAsync();
