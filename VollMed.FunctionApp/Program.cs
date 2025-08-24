using Azure.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using VollMed.FunctionApp;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton<CosmosClient>((serviceProvider) =>
{
    IOptions<Configuration> configurationOptions = serviceProvider.GetRequiredService<IOptions<Configuration>>();
    Configuration configuration = configurationOptions.Value;

    CosmosClient client = new(
        connectionString: Environment.GetEnvironmentVariable("AzureCosmosDB_ConnectionString")
    );
    return client;
});


builder.Build().Run();
