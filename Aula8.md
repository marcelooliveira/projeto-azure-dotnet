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

# Aula 8 - Tópicos Avançados 

## 5095-video8.2-Criação do recurso Azure Cache for Redis

Vamos usar o roteiro abaixo para criar um CPST no padrão da didática da Alura:

Contexto: 
Neste exercício, vamos criar um recurso do tipo Azure Cache for Redis no portal do Azure. O Redis será utilizado como cache distribuído para melhorar a performance da nossa aplicação Azure Function App, armazenando temporariamente os dados dos médicos.

Problema:
Atualmente, nossa Azure Function App consulta diretamente o banco de dados SQL para obter informações dos médicos, o que pode resultar em latência e sobrecarga no banco de dados. Ao utilizar o Redis como cache, podemos reduzir o tempo de resposta e melhorar a escalabilidade da aplicação.

A Solução com Azure Cache for Redis irá permitir armazenar em cache os dados dos médicos, reduzindo a necessidade de consultas frequentes ao banco de dados SQL.

1. No portal do Azure, crie um novo recurso do tipo **Azure Cache for Redis**.
2. Detalhes do projeto
    - Grupo de recursos: vollmed-rg
    - Nome: vollmed-cache
    - Região: East US 2
    - Camada de dados: Na memória
3. Acesso à rede
    - Ponto de Extremidade Público
4. Autenticação
    - Autenticação Microsoft Entra - checado
    - Autenticação das Chaves de Acesso - checado *
        * Habilitar apenas para desenvolvimento, nunca produção!.
5. Clicar em Examinar + Criar
6. Aguardar a implantação ser concluída.
7. Abrir o menu Configurações > Autenticação
8. Clicar **Data Access Configuration**
9. Clicar em "+" e **Novo Usuário do Redis**.
10. Clicar na aba **Usuários do Redis** 
11. Clicar **Atribuir acesso a** e **Identidade Gerenciada**.
12. Escolher em **Identidade Gerenciada**: **Aplicativo de Funções**
13. Escolher aplicativo **VollMedFunctionApp**

## 5095-video8.3-Configurando Redis no projeto Azure Function App

No seu projeto Function App (`.csproj`), abra o terminal e adicione os pacotes:

```bash
dotnet add package StackExchange.Redis --version 2.7.33
dotnet add package Microsoft.Data.SqlClient --version 6.0.0
```

Esses 2 pacotes acima servem para conectar ao Redis e ao SQL Server, respectivamente.

Agora, no final do arquivo da Function App `GerarProntuarioMedico.cs`, adicione as classes de apoio para o médico e especialidade:

```csharp
public class Prontuario
{
    public long MedicoId { get; set; }
    public string MedicoNome { get; set; } = string.Empty;
    public string MedicoCrm { get; set; } = string.Empty;
    public int MedicoEspecialidade { get; set; }
    public DateTime ConsultaData { get; set; }
    public string ConsultaPaciente { get; set; } = string.Empty;
}

public enum Especialidade
{
    [Display(Name = "Cardiologia")] Cardiologia = 1,
    [Display(Name = "Neurocirurgia")] Neurocirurgia = 2,
    [Display(Name = "Cirurgia Geral")] CirurgiaGeral = 3,
    [Display(Name = "Pediatria")] Pediatria = 4,
    [Display(Name = "Oncologia")] Oncologia = 5,
    [Display(Name = "Diagnóstico")] Diagnostico = 6
}
```

Agora volte ao portal do Azure e pegue a string de conexão do Redis:

1. No portal do Azure, vá até o recurso **vollmed-cache**.
2. No menu lateral, **Configurações > Autenticação**.
3. Clique na aba **Chaves de Acesso**.
4. Copie a string de conexão do Redis (Primary connection string)
5. De volta ao projeto no Visual Studio, abra o arquivo **local.settings.json**.
6. Adicione a chave "RedisConnectionString", com o valor copiado do portal do Azure.

Pronto, agora já podemos ajustar o código da Function App para utilizar o Redis como cache.

---

## 5095-video8.4-Ajuste no código da Function App

Abra o arquivo `GerarProntuarioMedico.cs` e importe os namespaces necessários no topo do arquivo:
```csharp
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
```

Modifique os campos e o construtor da classe abaixo, para incluir o logger, as strings de conexão e a conexão com o Redis:

```csharp
    private readonly ILogger<GerarProntuarioMedico> _logger;
    private readonly string _sqlConnectionString;
    private readonly string _redisConnectionString;
    private readonly ConnectionMultiplexer _redis;

    public GerarProntuarioMedico(ILogger<GerarProntuarioMedico> logger)
    {
        _logger = logger;
        _sqlConnectionString = Environment.GetEnvironmentVariable("SqlConnectionString")!;
        _redisConnectionString = Environment.GetEnvironmentVariable("RedisConnectionString")!;
        _redis = ConnectionMultiplexer.Connect(_redisConnectionString);
    }
```

Agora adicione uma constante para o template do prontuário médico, logo antes do método Run:


```csharp
    private const string TemplateProntuario = @"
# Prontuário Médico

Data do Atendimento: {0:dd/MM/yyyy HH:mm}

## 1. Identificação do médico
Nome médico: {1}
CRM: {2}
Especialidade: {3}

## 2. Identificação do Paciente
CPF: {4}
Nome Completo: 
Data de Nascimento: 

## 3. Anamnese
Queixa Principal: 
Medicação em uso:
";
```

Agora modifique a assinatura do método Run para ficar assim:
```csharp
    [Function("GerarProntuarioMedico")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "prontuario/{id:long}")] HttpRequest req,
        long id)
```

Note acima que adicionamos o parâmetro `int id`, que será utilizado para buscar a consulta pelo ID.

Agora vamos implementar o corpo do método Run para utilizar o Redis como cache. Substitua o conteúdo do método Run pelo seguinte código:

```csharp
        try
        {
            _logger.LogInformation($"Processando prontuário do médico ID {id}");

            var db = _redis.GetDatabase();

            _logger.LogInformation("_redis.GetDatabase OK");

            // 1️. Tenta obter do cache

            string? prontuarioJson = await db.StringGetAsync($"consulta:{id}");
            _logger.LogInformation("db.StringGetAsync executado");


            Prontuario? prontuario = null;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (!string.IsNullOrEmpty(prontuarioJson))
            {
                prontuario = JsonSerializer.Deserialize<Prontuario>(prontuarioJson);
                _logger.LogInformation("Dados obtidos do cache Redis.");
            }
            else
            {
                _logger.LogInformation("Dados não encontrados no cache. Consultando o banco SQL...");

                using (var conn = new SqlConnection(_sqlConnectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new SqlCommand("SELECT m.Id, m.Nome, m.Crm, m.Especialidade, c.Data, c.Paciente FROM [dbo].[medicos] as m INNER JOIN [dbo].[consultas] as c ON c.MedicoId = m.Id WHERE c.Id = @Id", conn);
                    cmd.Parameters.AddWithValue("@Id", id);

                    using var reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        prontuario = new Prontuario
                        {
                            MedicoId = reader.GetInt64(0),
                            MedicoNome = reader.GetString(1),
                            MedicoCrm = reader.GetString(2),
                            MedicoEspecialidade = reader.GetInt32(3),
                            ConsultaData = reader.GetDateTime(4),
                            ConsultaPaciente = reader.GetString(5)
                        };

                        // 2️⃣ Salva no cache Redis por 1 hora
                        await db.StringSetAsync(
                            $"consulta:{id}",
                            JsonSerializer.Serialize(prontuario),
                            TimeSpan.FromHours(1));

                        _logger.LogInformation("Dados do prontuário salvos no cache Redis.");
                    }
                }
            }

            stopwatch.Stop();

            _logger.LogInformation($"Tempo de execução (ms): {stopwatch.ElapsedMilliseconds}");

            if (prontuario == null)
            {
                return new NotFoundObjectResult($"Consulta com Id {id} não encontrada.");
            }

            // 2. Montar template do prontuário

            _logger.LogInformation("Prontuário médico gerado:");
            _logger.LogInformation(TemplateProntuario
                , prontuario.ConsultaData
                , prontuario.MedicoNome
                , prontuario.MedicoCrm
                , Enum.GetName(typeof(Especialidade)
                , prontuario.MedicoEspecialidade)
                , prontuario.ConsultaPaciente);

            return new OkObjectResult($"Prontuário do médico {prontuario.MedicoNome} gerado com sucesso (ver logs).");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            _logger.LogError(ex.ToString());
            _logger.LogError(ex, "Erro ao gerar prontuário médico...");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
```

## 5095-video8.5-Explicando código da Function App

Vamos explicar o que faz o método `Run`, que é o principal da Function App.

---

Em linhas gerais, este método faz o seguinte: 

* Escreve nos logs que o processamento do prontuário começou para o médico com o ID informado.
* Obtém uma instância do banco de dados Redis.
* Tenta buscar no cache Redis uma chave no formato `"consulta:{id}"`.
* Inicia um cronômetro para medir o tempo de execução.

---

* **Se encontrar dados no cache:**

  * Desserializa o JSON armazenado para um objeto `Prontuario`.
  * Registra no log que os dados vieram do cache.

---

* **Se não encontrar no cache:**

  * Registra que os dados serão buscados no banco SQL.
  * Abre uma conexão com o Azure SQL Database.
  * Executa um comando `SELECT` que junta as tabelas de médicos e consultas.
  * Lê os dados retornados e cria um objeto `Prontuario`.
  * Serializa o prontuário em JSON e salva no Redis por **1 hora**.
  * Escreve nos logs que o prontuário foi salvo no cache.

---

* Para o cronômetro e registra o tempo total de execução.
* Se nenhum prontuário for encontrado, retorna **404 (Not Found)**.
* Caso contrário, monta o **template do prontuário médico** e escreve os detalhes nos logs.
* Retorna **200 (OK)** informando que o prontuário foi gerado com sucesso.

---

* Se ocorrer algum erro em qualquer parte do processo:

  * Registra o erro completo nos logs.
  * Retorna **500 (Internal Server Error)**.



## 5095-video8.6-Testar a Aplicação com Cache

Agora execute sua Function App no Visual Studio, e execute o endpoint com esta requisição:

```
http://localhost:7031/api/prontuario/2
```

Nos logs, você deve ver:

```
Processando prontuário do médico ID 2
[2025-11-02T11:26:53.115Z] _redis.GetDatabase OK
[2025-11-02T11:26:53.248Z] db.StringGetAsync executado
[2025-11-02T11:26:53.250Z] Dados não encontrados no cache. Consultando o banco SQL...
[2025-11-02T11:26:53.527Z] Dados do prontuário salvos no cache Redis.
[2025-11-02T11:26:53.530Z] Tempo de execução (ms): 279
[2025-11-02T11:26:53.569Z] Prontuário médico gerado:
[2025-11-02T11:26:53.572Z]
[2025-11-02T11:26:53.573Z] # Prontuário Médico
[2025-11-02T11:26:53.575Z]
[2025-11-02T11:26:53.576Z] Data do Atendimento: 11/11/2024 12:00
[2025-11-02T11:26:53.579Z]
[2025-11-02T11:26:53.580Z] ## 1. Identificação do médico
[2025-11-02T11:26:53.582Z] Nome médico: Gregory House
[2025-11-02T11:26:53.585Z] CRM: 123456
[2025-11-02T11:26:53.587Z] Especialidade: Diagnostico
[2025-11-02T11:26:53.590Z]
[2025-11-02T11:26:53.591Z] ## 2. Identificação do Paciente
[2025-11-02T11:26:53.593Z] CPF: 23456789012
[2025-11-02T11:26:53.595Z] Nome Completo:
[2025-11-02T11:26:53.597Z] Data de Nascimento:
[2025-11-02T11:26:53.598Z]
[2025-11-02T11:26:53.605Z] ## 3. Anamnese
[2025-11-02T11:26:53.607Z] Queixa Principal:
[2025-11-02T11:26:53.608Z] Medicação em uso:
```

Note como os dados precisam ser buscados do banco de dados da primeira vez.

Note também como tempo de execução é 279 milissegundos.

Execute sua Function App novamente, desta vez para que a aplicação encontre o prontuário no cache:

```
http://localhost:7031/api/prontuario/2
```

Agora nos logs, você deve ver o tempo diminuir bastante:

```
[2025-11-02T11:27:02.775Z] Processando prontuário do médico ID 2
[2025-11-02T11:27:02.779Z] _redis.GetDatabase OK
[2025-11-02T11:27:02.919Z] db.StringGetAsync executado
[2025-11-02T11:27:02.922Z] Dados obtidos do cache Redis.
[2025-11-02T11:27:02.924Z] Tempo de execução (ms): 1
```

Note como tempo de execução com cache é 1 milissegundo, uma grande redução diante dos 279 milissegundos iniciais!