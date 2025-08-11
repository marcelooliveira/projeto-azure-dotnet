using VollMed.Web.Filters;
using VollMed.Web.Interfaces;
using VollMed.Web.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Logging;
//using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ExceptionHandlerFilter>();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ExceptionHandlerFilter>();
});


builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddRazorPages();

builder.Services.AddTransient<IVollMedApiService, VollMedApiService>();

var httpClientName = builder.Configuration["VollMed_WebApi:Name"];
var httpClientUrl = builder.Configuration["VollMed_WebApi:BaseAddress"];

builder.Services.AddHttpClient(
    httpClientName,
    client =>
    {
        // Configura o endereço-base do cliente nomeado.
        client.BaseAddress = new Uri(httpClientUrl);
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // Recria o handler a cada 5 minutos
    .AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = 10;
        options.Retry.OnRetry = args =>
        {
            var exception = args.Outcome.Exception!;
            Console.WriteLine($"Falha na chamada à API! Tentando novamente em 5 segundos. Msg: {exception.Message}");
            return default;
        };
        options.Retry.Delay = TimeSpan.FromSeconds(5); // Intervalo entre tentativas
    });

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

//builder.Services
//    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
//    .EnableTokenAcquisitionToCallDownstreamApi()
//    .AddDownstreamApi("VollMed.WebApi", builder.Configuration.GetSection("VollMed.WebApi"))
//    .AddInMemoryTokenCaches();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// MinimumLevel: Trace will show *everything*
builder.Logging.SetMinimumLevel(LogLevel.Trace);

builder.Logging.AddFilter("Microsoft", LogLevel.Trace);
builder.Logging.AddFilter("System.Net.Http", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.Identity", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.IdentityModel", LogLevel.Trace);
builder.Logging.AddFilter("Microsoft.Identity.Web", LogLevel.Trace);

IdentityModelEventSource.ShowPII = true;

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/erro/500");
    app.UseStatusCodePagesWithReExecute("/erro/{0}");
}

app.UseStaticFiles();


app.UseRouting();
//app.UseAuthentication();
//app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
