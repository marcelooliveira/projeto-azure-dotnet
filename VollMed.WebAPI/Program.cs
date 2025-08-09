using VollMed.Web.Data;
using VollMed.Web.Interfaces;
using VollMed.Web.Repositories;
using VollMed.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>((options) => {
    options
            .UseSqlServer(builder.Configuration["ConnectionStrings:VollMedDB"],
                b => b.MigrationsAssembly("VollMed.WebAPI"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IMedicoRepository, MedicoRepository>();
builder.Services.AddTransient<IConsultaRepository, ConsultaRepository>();
builder.Services.AddTransient<IMedicoService, MedicoService>();
builder.Services.AddTransient<IConsultaService, ConsultaService>();

//builder.Services.AddAuthentication()
//    .AddJwtBearer(options =>
//    {
//        options.Authority = "https://login.microsoftonline.com/0ec7eb2f-a6b5-4cd1-9695-3f3ec151ed0c/v2.0";
//        options.Audience = "b44fa4a2-48ce-459e-9646-345c84ac2978";
//    });

//builder.Services.AddAuthorizationBuilder()
//    .AddPolicy("ApiScope", policy =>
//    {
//        policy.RequireAuthenticatedUser();
//        policy.RequireClaim("scope", "VollMed.WebAPI");
//    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAd", options);
    },
    options =>
    {
        builder.Configuration.Bind("AzureAd", options);
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
