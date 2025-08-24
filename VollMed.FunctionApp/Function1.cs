using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace VollMed.FunctionApp;

public class Function1
{
    private readonly ILogger<Function1> _logger;
    private readonly IConfiguration _configuration;
    private readonly CosmosClient _cosmosClient;

    public Function1(ILogger<Function1> logger, IConfiguration configuration, CosmosClient client)
    {
        _logger = logger;
        _configuration = configuration;
        _cosmosClient = client;
    }

    [Function("ResumoMensalFunction")]
    public async Task Run(
        [ServiceBusTrigger("vollmedqueue", Connection = "ServiceBusConnection")]
        string message,
        ServiceBusMessageActions messageActions,
        [SqlInput(
        "SELECT m.Id as MedicoId, m.Nome as MedicoNome, COUNT(c.Id) as QtdeConsultas" +
        " FROM  Medicos m" +
        " LEFT JOIN Consultas c ON m.Id = c.MedicoId AND YEAR(c.Data) = @Ano AND MONTH(c.Data) = @Mes" +
        " WHERE m.Id = @MedicoId" +
        " GROUP BY m.Id , m.Nome",
        "SqlConnectionString", System.Data.CommandType.Text, "@MedicoId={MedicoId},@Ano={Ano},@Mes={Mes}")]
        IEnumerable<ConsultaPorMedico> consultas,
        FunctionContext context)
    {
        try
        {
            _logger.LogInformation("Message: {message}", message);

            var honorarioPorConsulta = 100m;

            Database database = _cosmosClient.GetDatabase(_configuration["AzureCosmosDB_DatabaseName"]);

            Microsoft.Azure.Cosmos.Container container = database.GetContainer(_configuration["AzureCosmosDB_ContainerName"]);

            container = await container.ReadContainerAsync();

            var consultaMsg = JsonSerializer.Deserialize<ConsultaQueueMessage>(message);
            _logger.LogInformation($"Processing consulta for MedicoId={consultaMsg.MedicoId}, Ano={consultaMsg.Ano}, Mes={consultaMsg.Mes}");

            var consulta = consultas.SingleOrDefault();

            var honorarios = (consulta.QtdeConsultas + 1) * honorarioPorConsulta;

            var resultadoMensal =
                    new ResultadoMensal
                    (
                        id: consulta.MedicoId.ToString("00000") + "-" + consultaMsg.Ano.ToString() + "-" + consultaMsg.Mes.ToString("00"),
                        medicoId: consulta.MedicoId,
                        medicoNome: consulta.MedicoNome,
                        ano: consultaMsg.Ano,
                        mes: consultaMsg.Mes,
                        qtdeConsultas: consulta.QtdeConsultas + 1,
                        honorarios: honorarios
                    );

            var result = await container.UpsertItemAsync<ResultadoMensal>(
                item: resultadoMensal,
                partitionKey: new Microsoft.Azure.Cosmos.PartitionKey(resultadoMensal.id));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process consulta");
        }
    }

}


public class ConsultaQueueMessage
{
    public int MedicoId { get; set; }
    public int Ano { get; set; }
    public int Mes { get; set; }
}

public class ConsultaPorMedico
{
    public long MedicoId { get; set; }
    public string MedicoNome { get; set; }
    public DateTime Data { get; set; }
    public int QtdeConsultas { get; set; }
    public decimal Honorarios { get; set; }
}

public record ResultadoMensal
(
    string id,
    long medicoId,
    string medicoNome,
    int ano,
    int mes,
    int qtdeConsultas,
    decimal honorarios
);