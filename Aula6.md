| Aula| SQL | API / MVC | auth | Monitoring |    Synch       | Infra     | Caching     |
| ---| --- | --- | --- | --- |    ---       | ---     | ---     |
| 1 | local | local |     -    |      -     |    síncrono    | az portal |     -       |
| 2 | CLOUD | local |     -    |      -     |    síncrono    | az portal |     -       |
| 3 | CLOUD | CLOUD |     -    |      -     |    síncrono    | az portal |     -       |
| 4 | CLOUD | CLOUD | MS ENTRA |      -     |    síncrono    | az portal |     -       |
| 5 | CLOUD | CLOUD | MS ENTRA | AZ MONITOR |    síncrono    | az portal |     -       |
| 6 | CLOUD | CLOUD | MS ENTRA | AZ MONITOR | MSG/ASSÍNCRONO | az portal |     -       |
| 7 | CLOUD | CLOUD | MS ENTRA | AZ MONITOR | MSG/ASSÍNCRONO | IAAC      |     -       |
| 8 | CLOUD | CLOUD | MS ENTRA | AZ MONITOR | MSG/ASSÍNCRONO | IAAC      | AZURE REDIS |


# Aula 6 - Mensageria e Escalabilidade 

1. Create Azure Table Storage: "ResumoMensalConsultas"

Steps:
-	In the Azure Portal, create a Storage Account.
-	In the Storage Account, create a Table named ResumoMensalConsultas.

Schema:
-	PartitionKey (string): Use MedicoId as string.
-	RowKey (string): Use "Ano-Mes" (e.g., "2025-05").
-	MedicoId (long)
-	Ano (int)
-	Mes (int)
-	QtdeConsultas (int)
-	Honorarios (decimal)
---
2. Create Azure Service Bus Message Type: "NovaConsultaMessage"

Steps:
-	In Azure Portal, create a Service Bus namespace and a queue (e.g., nova-consulta-queue).
-	Define the message contract in C#:

```csharp
public class NovaConsultaMessage
{
    public long MedicoId { get; set; }
    public string Paciente { get; set; }
    public DateTime Data { get; set; }
}
```

---
3. Create Azure Function: "NovaConsultaAzFunction"
Steps:
-	In Visual Studio, create a new Azure Function project.
-	Add a function with a Service Bus trigger and Azure SQL input binding.

Function signature:

```csharp
public static async Task Run(
    [ServiceBusTrigger("nova-consulta-queue", Connection = "ServiceBusConnection")] NovaConsultaMessage consultaMsg,
    [Sql("SELECT * FROM Consultas WHERE MedicoId = @MedicoId AND YEAR(Data) = @Ano AND MONTH(Data) = @Mes", 
         CommandType = System.Data.CommandType.Text, 
         Parameters = "@MedicoId={consultaMsg.MedicoId},@Ano={consultaMsg.Data.Year},@Mes={consultaMsg.Data.Month}", 
         ConnectionStringSetting = "SqlConnectionString")] IEnumerable<Consulta> consultas,
    ILogger log)
{
    // Function logic here
}
```


---
4. Function Logic: Update ResumoMensalConsultas
Steps:
-	On receiving NovaConsultaMessage:
1.	Query all consultations for the given MedicoId, Ano, and Mes from SQL.
2.	Calculate QtdeConsultas (count) and Honorarios (sum, e.g., fixed value per consulta or from DB).
3.	Upsert the result into Azure Table Storage ResumoMensalConsultas.

Sample code:


```csharp
public static async Task Run(
    [ServiceBusTrigger("nova-consulta-queue", Connection = "ServiceBusConnection")] NovaConsultaMessage consultaMsg,
    [Sql("SELECT * FROM Consultas WHERE MedicoId = @MedicoId AND YEAR(Data) = @Ano AND MONTH(Data) = @Mes", 
         CommandType = System.Data.CommandType.Text, 
         Parameters = "@MedicoId={consultaMsg.MedicoId},@Ano={consultaMsg.Data.Year},@Mes={consultaMsg.Data.Month}", 
         ConnectionStringSetting = "SqlConnectionString")] IEnumerable<Consulta> consultas,
    ILogger log)
{
    var qtdeConsultas = consultas.Count();
    var honorarioPorConsulta = 100m; // Example value
    var honorarios = qtdeConsultas * honorarioPorConsulta;

    var tableClient = new TableClient(Environment.GetEnvironmentVariable("TableStorageConnectionString"), "ResumoMensalConsultas");
    var entity = new TableEntity(consultaMsg.MedicoId.ToString(), $"{consultaMsg.Data.Year}-{consultaMsg.Data.Month:D2}")
    {
        { "MedicoId", consultaMsg.MedicoId },
        { "Ano", consultaMsg.Data.Year },
        { "Mes", consultaMsg.Data.Month },
        { "QtdeConsultas", qtdeConsultas },
        { "Honorarios", honorarios }
    };

    await tableClient.UpsertEntityAsync(entity);
}
```


