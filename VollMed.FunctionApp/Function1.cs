using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Sql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

    //[Function(nameof(Function1))]
    //public async Task Run(
    //    [ServiceBusTrigger("vollmedqueue", Connection = "ServiceBusConnection")]
    //    ServiceBusReceivedMessage message,
    //    ServiceBusMessageActions messageActions)
    //{
    //    _logger.LogInformation("Message ID: {id}", message.MessageId);
    //    _logger.LogInformation("Message Body: {body}", message.Body);
    //    _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

    //    // Complete the message
    //    await messageActions.CompleteMessageAsync(message);
    //}

    [Function(nameof(Function1))]
    public async Task Run(
        [ServiceBusTrigger("vollmedqueue", Connection = "ServiceBusConnection")]
        string message,
        ServiceBusMessageActions messageActions,
        [SqlInput(
        "SELECT m.Id as MedicoId, m.Nome as MedicoNome, COUNT(c.Id) as QtdeConsultas" +
        " FROM  Medicos m" +
        " LEFT JOIN Consultas c ON m.Id = c.MedicoId AND YEAR(c.Data) = @Ano AND MONTH(c.Data) = @Mes" +
        " WHERE m.Id = 1" +
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

            //database = await database.ReadAsync();

            Microsoft.Azure.Cosmos.Container container = database.GetContainer(_configuration["AzureCosmosDB_ContainerName"]);

            container = await container.ReadContainerAsync();

            //var tableClient = new TableClient(_configuration["AzureCosmosDB:TableStorageConnectionString"], "ResumoMensalConsultas");

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

            // ❌ Don't complete the message, let it be retried or dead-lettered
            //await messageActions.AbandonMessageAsync(message);
        }


    }

    //[Function(nameof(Function1))]
    //public void Run(
    //[ServiceBusTrigger("vollmedqueue", Connection = "ServiceBusConnection")] string message,
    //[SqlInput("SELECT * FROM Consultas WHERE Id = @id", "SqlConnectionString", System.Data.CommandType.Text,
    //    "@Id={medicoId}")]
    //IEnumerable<Consulta> sqlInputData,
    //ILogger log)
    //{
    //    //log.LogInformation($"Received message from Service Bus: {myQueueItem}");
    //    //foreach (var item in sqlInputData)
    //    //{
    //    //    log.LogInformation($"SQL Input data item: {item.YourProperty}");
    //    //}
    //}

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