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

## Pré-requisito de Banco de Dados


1 - Na barra de busca do Portal, procure o recurso **Azure Cosmos DB**, e clique em **Create**.

2 - Escolha a opção **Azure Cosmos DB for NoSQL**.

3 - Em **Account Name**, escreva `vollmed-cosmosdb`.

4 - Em **Capacity Mode**, escolha **Serverless** e clique em **Create**.

5 - Aguarde a criação do Azure Cosmos DB Account

6 - Entre no menu **Data Explorer**.

7 - Crie um novo Container no database `vollmed`, preencha **Database id** como "vollmed" e  **Container id** como `ResumoMensalConsultas` e **Partition id** como `id`.

8 - Agora expanda o nó “items” do container “ResumoMensalConsultas” e clique em **New Item**. Preencha o documento com o seguinte conteúdo JSON:

```json
{
  "id": "1",
  "meses": [
    {
	"medicoNome": "Gregory House",
	"ano": 2025,
	"mes": 5,
	"qtdeConsultas": 3,
	"honorarios": 300
    }
  ]
}
```

9 - Clique em ***Save*** para salvar o item.

## Azure Service Bus

Service Bus
 
Basics
    Namespace name
        vollmed20250822
    Subscription
        Azure subscription 1
    Resource group
        vollmed-rg
    Location
        East US
    Pricing tier
        Basic
Networking
    Connectivity method
        Public access
Security
    Minimum TLS version
        1.2
    Local Authentication
        Enabled

### IAM

Role
    Azure Service Bus Data Owner
Scope
    /subscriptions/XXXXX/resourceGroups/vollmed-rg/providers/Microsoft.ServiceBus/namespaces/vollmed20250822
Members
    Name
        [YOUR USER]
    Type
        User

### Connection String

Portal:
    vollmed20250822 | Shared access policies
SAS Policy:
    RootManageSharedAccessKey
Primary connection string:
    Endpoint=sb://vollmed20250822.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=xxxxxxxxxxxxx


2. Create Azure Service Bus Message Type: "ConsultaMessage"

Steps:
-	In Azure Portal, create a Service Bus namespace and a queue (e.g., vollmedqueue).
-	Define the message contract in C#:

```csharp
public class ConsultaMessage
{
    public long MedicoId { get; set; }
    public string Paciente { get; set; }
    public DateTime Data { get; set; }
}
```

Testar o envio de mensagem para vollmedqueue:

```json
{
    "MedicoId": 1,
    "Ano": 2025,
    "Mes": 5
}
```

---
3. Create Azure Function App Project: "VollMed.FunctionApp"
Steps:
-	In Visual Studio, create a new Azure Function project.

    Project Name
	    VollMed.FunctionApp
    Function worker
	    .NET 9.0 Isolated
    Function
	    Service Bus Queue Trigger
    Use Azurite [x]
    Connection string setting name
	    ServiceBusConnection
    Queue name
	    vollmedqueue


-	Add a function with a Service Bus trigger and Azure SQL input binding.

Function signature:

```csharp
public static async Task Run(
    [ServiceBusTrigger("vollmedqueue", Connection = "ServiceBusConnection")] ConsultaMessage consultaMsg,
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
    [ServiceBusTrigger("vollmedqueue", Connection = "ServiceBusConnection")] NovaConsultaMessage consultaMsg,
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
        { "MedicoNome", consultaMsg.MedicoNome },
        { "Ano", consultaMsg.Data.Year },
        { "Mes", consultaMsg.Data.Month },
        { "QtdeConsultas", qtdeConsultas },
        { "Honorarios", honorarios }
    };

    await tableClient.UpsertEntityAsync(entity);
}
```


