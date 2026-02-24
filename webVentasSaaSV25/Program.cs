// Program.cs — VentasSaaSDU.Web
// CORRECCIÓN v2:
//   - Microsoft.Extensions.Http requerido para AddHttpClient (agregar al .csproj)
//   - AuthService y PermisoClienteService como Scoped (IJSRuntime es Scoped en WASM)
//   - AppState como Singleton (no depende de IJSRuntime directamente)

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using webVentasSaaSV25;
using webVentasSaaSV25.Services.Auth;
using webVentasSaaSV25.Services.Http;
using webVentasSaaSV25.State;
using webVentasSaaSV25;
using webVentasSaaSV25.Services.Auth;
using webVentasSaaSV25.Services.Http;
using webVentasSaaSV25.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// URL de la API — viene de wwwroot/appsettings.json
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl no configurado en wwwroot/appsettings.json");

// ─── ESTADO GLOBAL ────────────────────────────────────────────────────────────
// Singleton: no depende de IJSRuntime, vive toda la sesión de la pestaña.
builder.Services.AddSingleton<AppState>();

// ─── SERVICIOS DE AUTENTICACIÓN ───────────────────────────────────────────────
// Scoped (NO Singleton) porque dependen de IJSRuntime que en WASM es Scoped.
// En Blazor WASM hay un solo Scope por sesión de pestaña, por lo que el
// comportamiento es equivalente a Singleton en la práctica.
builder.Services.AddScoped<PermisoClienteService>();
builder.Services.AddScoped<AuthService>();

// ─── HTTP CLIENTS ─────────────────────────────────────────────────────────────
// REQUIERE paquete NuGet: Microsoft.Extensions.Http
// Agregar al VentasSaaSDU.Web.csproj:
//   <PackageReference Include="Microsoft.Extensions.Http" Version="8.0.0" />

builder.Services.AddScoped<TokenRefreshHandler>();

// Cliente autenticado (con Bearer + refresh automático)
builder.Services.AddHttpClient("VentasSaaSAPI", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<TokenRefreshHandler>();

// Cliente público (solo para Login y Refresh — sin token todavía)
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

await builder.Build().RunAsync();