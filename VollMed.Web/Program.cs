using VollMed.Web.Filters;
using VollMed.Web.Interfaces;
using VollMed.Web.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
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

var httpClientName = builder.Configuration["VollMed.WebApi.Name"];
var httpClientUrl = builder.Configuration["VollMed.WebApi.Url"];

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

//builder.Services.AddRateLimiter(options =>
//{
//    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
//        RateLimitPartition.GetFixedWindowLimiter(
//            partitionKey: context.User?.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
//            factory: partition => new FixedWindowRateLimiterOptions
//            {
//                PermitLimit = 5, // até 5 requisições
//                Window = TimeSpan.FromSeconds(10), // por intervalo de 10 segundos
//                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
//                QueueLimit = 0 // sem fila de espera
//            }));

//    options.OnRejected = async (context, cancellationToken) =>
//    {
//        await context.HttpContext.Response.WriteAsync(
//            "Limite de taxa excedido. Por favor tente novamente mais tarde.",
//            cancellationToken);
//    };
//});

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

//builder.Services
//    .AddAuthentication(options =>
//    {
//        options.DefaultScheme = "Cookies";
//        options.DefaultChallengeScheme = "oidc";
//    })
//    .AddCookie("Cookies", options =>
//    {
//        options.Cookie.Name = "VollMedAuthCookie";
//        options.LoginPath = "/Account/Login";
//        options.AccessDeniedPath = "/Account/AccessDenied";
//        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
//        options.SlidingExpiration = true;
//        options.Cookie.HttpOnly = true;
//        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//        options.Cookie.SameSite = SameSiteMode.Strict;
//    })
//    .AddOpenIdConnect("oidc", options =>
//    {
//        options.Authority = builder.Configuration["VollMed.Identity.Url"];
//        options.ClientId = "VollMed.Web";
//        options.ClientSecret = "secret";
//        options.ResponseType = "code";

//        options.Scope.Clear();
//        options.Scope.Add("openid");
//        options.Scope.Add("profile");
//        options.Scope.Add("VollMed.WebAPI");

//        options.GetClaimsFromUserInfoEndpoint = true;
//        options.MapInboundClaims = false;
//        options.SaveTokens = true;
//    });


var app = builder.Build();

//app.UseRateLimiter();

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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
