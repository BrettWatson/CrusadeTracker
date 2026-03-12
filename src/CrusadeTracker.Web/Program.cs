using Blazored.LocalStorage;
using CrusadeTracker.Web;
using CrusadeTracker.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure API base address
var apiBaseAddress = builder.Configuration["ApiBaseAddress"] ?? "http://localhost:5051/";

// Add MudBlazor
builder.Services.AddMudServices();

// Add Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Add token service
builder.Services.AddScoped<TokenService>();

// Add auth delegating handler
builder.Services.AddScoped<AuthenticationDelegatingHandler>();

// Add HTTP client with auth handler
builder.Services.AddHttpClient("CrusadeTrackerApi", client =>
{
    client.BaseAddress = new Uri(apiBaseAddress);
}).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

// Add default HttpClient that uses the named client
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("CrusadeTrackerApi"));

// Add authentication state provider
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<ApiAuthenticationStateProvider>());

// Add auth service
builder.Services.AddScoped<IAuthService, AuthService>();

// Add domain services
builder.Services.AddScoped<ForceService>();
builder.Services.AddScoped<BattleService>();

// Add authorization
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
