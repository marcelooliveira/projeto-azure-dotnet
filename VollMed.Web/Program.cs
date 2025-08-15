using Microsoft.Extensions.DependencyInjection.Extensions;
using VollMed.Web.Filters;
using VollMed.Web.Interfaces;
using VollMed.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ExceptionHandlerFilter>();

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
        client.BaseAddress = new Uri(httpClientUrl);
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
