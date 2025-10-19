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

## Vídeo 6.1 - Apresentando Mensageria e Escalabilidade

Slide 1 — Cenário Inicial: Contexto

O front-end MVC envia os dados da nova consulta para a API, de forma síncrona.
A API grava imediatamente esses dados no Azure SQL Database.
Depois, a API busca no SQL as consultas do médico no mês atual.
Com isso, calcula o resumo mensal no próprio fluxo da requisição.

Slide 2 — Cenário Inicial: Problema

A API está fazendo gravação, leitura e cálculo no mesmo fluxo.
Isso torna cada requisição mais lenta, especialmente em alta demanda.
A dependência direta do SQL aumenta o risco de gargalos.
Esse modelo limita a escalabilidade e a resiliência da aplicação.

Slide 3 — Cenário Final: Solução

Após gravar a consulta no SQL, a API envia uma mensagem simples ao Azure Service Bus.
Quando a mensagem chega à fila, a Azure Function é disparada automaticamente.
A função lê a mensagem para identificar qual médico e período devem ser processados.
Em seguida, busca as informações no SQL para preparar o cálculo.

Slide 4 — Cenário Final: Teoria

A função calcula o resumo mensal de forma assíncrona e independente da API.
O resultado consolidado é salvo no Azure Cosmos DB, sem bloquear a requisição original.
A API deixa de executar tarefas pesadas, melhorando desempenho.
Com essa arquitetura baseada em mensageria, o sistema escala melhor e com mais resiliência.

## Vídeo 6.2 - Pré-requisito: Banco de Dados Cosmos DB

1. Na barra de busca do Portal, procure o recurso **Azure Cosmos DB**, e clique em **Create**.

2. Abra a conta que foi criada anteriormente neste curso: `vollmed-cosmosdb****`.

3. Entre no menu **Data Explorer**.

4. Confirme que você tem o banco de dados `vollmed` e a coleção `ResumoMensalConsultas`.

5. Se não tiver, volte à Aula 2 para criar essa conta do Cosmos DB com o banco de dados e essa coleção.

O **Azure Cosmos DB** é um banco de dados NoSQL globalmente distribuído, projetado para oferecer **alta disponibilidade**, **baixa latência** e **escalabilidade automática**, tornando-o ideal para cenários de processamento assíncrono como o da VollMed. No fluxo com mensageria, ele armazena o **resumo mensal das consultas médicas** gerado de forma independente pelo Azure Function, garantindo que a aplicação principal continue rápida e responsiva. Além disso, seu modelo de dados flexível permite gravar e consultar informações sem depender de esquemas rígidos, facilitando a evolução do sistema ao longo do tempo.

## Vídeo 6.3 - Pré-requisito: Azure Service Bus

1. No portal do Azure, procure por "Service Bus", ou "Barramento de Serviço".

3. Clique **+ Criar** para criar um novo Service Bus.

3. Preencha os valores:

Basics
    Namespace name
        vollmed20250822 
            (Note que este nome é único em todo o Azure, portanto escolha um nome diferente).
    Subscription
        Azure subscription 1
    Resource group
        vollmed-rg
    Location
        East US 2
    Pricing tier
        Básico

4. Clique em **Revisar** e depois **Criar**.

5. Ao final, clique em **Ir para o Recurso**.

6. No menu lateral **Filas**, crie uma nova fila, chamada "vollmedqueue".

O **Azure Service Bus**, também chamado de **Barramento de Serviço**, é um serviço de mensageria confiável que permite **desacoplar** diferentes partes de uma aplicação. Isso garante maior **resiliência**, **escalabilidade** e **desempenho**.

No fluxo da VollMed, ele recebe uma mensagem simples da WebAPI e aciona automaticamente a Azure Function para processar os dados do médico de forma assíncrona, sem bloquear a aplicação principal.

Isso permite que operações demoradas, como cálculos de resumo mensal, sejam executadas em segundo plano, mantendo a experiência do usuário rápida e fluida. 

Além disso, o Service Bus assegura a entrega confiável das mensagens, mesmo que o serviço consumidor esteja temporariamente indisponível.


## Vídeo 6.4 - Configurando Azure Service Bus

7. IAM

    - No menu lateral esquerdo, clique no menu **IAM > Atribuições de função**, ou **IAM > Role assignments** em inglês.

    - Na aba Função, procure a Função (Role): 
        "Proprietário de Dados de Barramento de Serviço do Azure" (Azure Service Bus Data Owner)

    - Na aba Membros, adicione o seu próprio usuário.

    - Clique em **Examinar + Atribuir**.

8. Connection String

    - No menu lateral esquerdo, clique no menu **Configurações > Políticas de Acesso Compartilhado**.
    
    - Clique na política RootManageSharedAccessKey

    - No painel direito, clique para exibir a "Cadeia de conexão primária".

    - Copie a "Cadeia de conexão primária" para um bloco de notas.

## Vídeo 6.5 - Apresentando Azure Functions

[slides]

## Vídeo 6.6 - Criando novo projeto Azure Function

1. Novo projeto VollMed.FunctionApp

    - Crie um novo projeto Function App no Visual Studio chamado "VollMed.FunctionApp".

    - Clique com botão direito do mouse sobre a solução e clique em Criar, "New Project".

    - Escolha a opção Azure Functions.

    - Project Name
	    VollMed.FunctionApp

    - Function worker
	    .NET 9.0 Isolated
    
    - Function
	    Service Bus Queue Trigger
    
    - Use Azurite [x]
    
    - Connection string setting name
	    ServiceBusConnection

    - Queue name
	    vollmedqueue

    - Clique em **Create**.

## Vídeo 6.7 - Adaptando a Função Azure

1. Adicione as configurações abaixo no arquivo locals.settings.json:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ServiceBusConnection": "",
    "SqlConnectionString": "",
    "AzureCosmosDB_DatabaseName": "vollmed",
    "AzureCosmosDB_ContainerName": "ResumoMensalConsultas",
    "AzureCosmosDB_ConnectionString": ""
  }
}
```

- Vá no projeto web api, copie a configuração da conexão com o banco Azure SQL Database e cole no valor de "SqlConnectionString".
- Copie o valor da conexão do Azure Service Bus que copiamos antes, e cole no valor de "ServiceBusConnection".
- Abra o recurso Azure Cosmos DB no Azure, clique no menu **Configurações > Chaves** e copie o valor de "Primary Connection String". Cole no valor da configuração AzureCosmosDB_ConnectionString.

2. No terminal do Visual Studio, instale os pacotes necessários:

```console
dotnet add package Microsoft.Azure.Cosmos --version 3.45.0
dotnet add package Microsoft.Azure.Functions.Worker.Extensions.CosmosDB --version 4.0.0
dotnet add package Microsoft.Azure.Functions.Worker.Extensions.Sql --version 3.1.512
dotnet add package Microsoft.Azure.WebJobs --version 3.0.42
```

3. Adicione as classes do modelo ao final do arquivo Function.cs:

    ```csharp
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
    ```

## Vídeo 6.8 - Adaptando a assinatura método da função Azure

13. Vamos modificar a assinatura do Método `Run` usando o código abaixo.
O objetivo do novo método Run vai ser processar mensagens recebidas de uma fila Service Bus, consultar dados no banco SQL com base nessas mensagens e registrar o resultado consolidado no Azure Cosmos DB.

```csharp
    [Function("ResumoMensalFunction")]
    [CosmosDBOutput(
        databaseName: "%AzureCosmosDB_DatabaseName%",
        containerName: "%AzureCosmosDB_ContainerName%",
        Connection = "AzureCosmosDB_ConnectionString")]
    public async Task<object?> Run(
        [ServiceBusTrigger("vollmedqueue", Connection = "ServiceBusConnection")]
        string message,
        ServiceBusMessageActions messageActions,
        [SqlInput(
        "SELECT m.Id as MedicoId, m.Nome as MedicoNome, COUNT(c.Id) as QtdeConsultas FROM Medicos m LEFT JOIN Consultas c ON m.Id = c.MedicoId AND YEAR(c.Data) = @Ano AND MONTH(c.Data) = @Mes WHERE m.Id = @MedicoId GROUP BY m.Id, m.Nome",
        "SqlConnectionString", System.Data.CommandType.Text, "@MedicoId={MedicoId},@Ano={Ano},@Mes={Mes}")]
        IEnumerable<ConsultaPorMedico> consultas,
        FunctionContext context)
    {
        // a lógica vai aqui
    }
```

Vamos explicar o que significa essa nova assinatura do método `Run`:

* **ServiceBusTrigger**: a execução da Function começa automaticamente quando uma nova mensagem chega na fila `vollmedqueue` do **Azure Service Bus**.
* **SqlInput binding**: a Function lê dados diretamente do **Azure SQL Database**, usando os parâmetros recebidos na mensagem.
* **CosmosDBOutput binding**: ao final, a Function envia automaticamente o resultado para o **Azure Cosmos DB**, sem precisar de código de conexão manual.
* **Function Context**: fornece contexto de execução e permite registrar logs e monitorar a Function.
* **Encadeamento de bindings**: a mensagem recebida dispara toda a cadeia de leitura (SQL) e gravação (Cosmos DB) de forma desacoplada e escalável.

## Vídeo 6.9 - Adicionando a lógica da função Azure

---
14. Adicione o corpo do método `Run` com a lógica completa da nossa Azure Function.

O corpo desse método será responsável por processar automaticamente mensagens enviadas para a fila de consultas médicas, buscar informações no banco de dados SQL, calcular o resumo mensal do médico e retornar esse resultado para ser gravado no Cosmos DB.

```csharp
    [Function("ResumoMensalFunction")]
    [CosmosDBOutput(
        databaseName: "%AzureCosmosDB_DatabaseName%",
        containerName: "%AzureCosmosDB_ContainerName%",
        Connection = "AzureCosmosDB_ConnectionString")]
    public async Task<object?> Run(
        [ServiceBusTrigger("vollmedqueue", Connection = "ServiceBusConnection")]
        string message,
        ServiceBusMessageActions messageActions,
        [SqlInput(
        "SELECT m.Id as MedicoId, m.Nome as MedicoNome, COUNT(c.Id) as QtdeConsultas FROM Medicos m LEFT JOIN Consultas c ON m.Id = c.MedicoId AND YEAR(c.Data) = @Ano AND MONTH(c.Data) = @Mes WHERE m.Id = @MedicoId GROUP BY m.Id, m.Nome",
        "SqlConnectionString", System.Data.CommandType.Text, "@MedicoId={MedicoId},@Ano={Ano},@Mes={Mes}")]
        IEnumerable<ConsultaPorMedico> consultas,
        FunctionContext context)
    {
        try
        {
            _logger.LogInformation("Message: {message}", message);

            var honorarioPorConsulta = 100m;
            var consultaMsg = JsonSerializer.Deserialize<ConsultaQueueMessage>(message);
            _logger.LogInformation($"Processing consulta for MedicoId={consultaMsg.MedicoId}, Ano={consultaMsg.Ano}, Mes={consultaMsg.Mes}");

            if (consultas == null || !consultas.Any())
                return null;

            var consulta = consultas.SingleOrDefault();
            var honorarios = (consulta.QtdeConsultas + 1) * honorarioPorConsulta;
            var resultadoMensal = new ResultadoMensal(
                id: consulta.MedicoId.ToString("00000") + "-" + consultaMsg.Ano.ToString() + "-" + consultaMsg.Mes.ToString("00"),
                medicoId: consulta.MedicoId,
                medicoNome: consulta.MedicoNome,
                ano: consultaMsg.Ano,
                mes: consultaMsg.Mes,
                qtdeConsultas: consulta.QtdeConsultas + 1,
                honorarios: honorarios
            );

            return resultadoMensal;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process consulta");
            return null;
        }
    }
```

Agora a nossa Azure Function é responsável por:

* Registrar nos logs que uma nova mensagem foi recebida.
* Converter o conteúdo da mensagem para um objeto fortemente tipado.
* Consultar o banco de dados SQL para obter informações do médico.
* Calcular a quantidade total de consultas e o valor de honorários.
* Criar um objeto com o resultado mensal consolidado.
* Retornar esse objeto para ser salvo automaticamente no Cosmos DB.
* Tratar e registrar possíveis erros que ocorrerem durante o processamento.




==============

## Vídeo 6.10 - Testando a Mensageria

- Clique com botão direito do mouse no projeto VollMed.FunctionApp
- Clique no menu "Set as startup project"
- Veja que a função está rodando. Agora ela está escutando e aguardando novas mensagens na fila do Azure Service Bus.
- No Portal do Azure, pesquise por “Azure SQL Database".
- Abra o Explorador de Dados
- Execute uma Query: `SELECT * FROM Consultas`
- No Portal do Azure, pesquise por “Service Bus” ou "Barramento de Serviço".
- Clique no namespace vollmed202xxxxxxx.
- No menu lateral esquerdo, clique em “Filas” (Queues).
- Selecione a fila vollmedqueue.
- No menu à esquerda, clique em “Gerenciador de Barramento de Serviço” ou "Service Bus Manager".
- No topo da página, clique em "Enviar mensagens".
- Escolha o Tipo de conteúdo: application/json
- No Corpo da mensagem, preencha:

```json
{
    "MedicoId": 1,
    "Ano": 2024,
    "Mes": 11
}
```

- Clique em Enviar

A mensagem será enfileirada imediatamente e vai diparar a Azure Function configurada como trigger da fila no Service Bus.