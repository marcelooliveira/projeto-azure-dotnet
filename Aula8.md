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

## 5095-video8.1-Criação do recurso “Azure Managed Redis”

Vamos usar o roteiro abaixo para criar um CPST no padrão da didática da Alura:

Contexto: 
Neste exercício, vamos criar um recurso do tipo Azure Cache for Redis no portal do Azure. O Redis será utilizado como cache distribuído para melhorar a performance da nossa aplicação Azure Function App, armazenando temporariamente os dados dos médicos.

Problema:
Atualmente, nossa Azure Function App consulta diretamente o banco de dados SQL para obter informações dos médicos, o que pode resultar em latência e sobrecarga no banco de dados. Ao utilizar o Redis como cache, podemos reduzir o tempo de resposta e melhorar a escalabilidade da aplicação.

A Solução com Azure Managed Redis irá permitir armazenar em cache os dados dos médicos, reduzindo a necessidade de consultas frequentes ao banco de dados SQL.

1. No portal do Azure, crie um novo recurso do tipo **Azure Cache for Redis** (versão gerenciada).
2. Detalhes do projeto
    - Grupo de recursos: vollmed-rg
    - Nome: vollmed-cache
    - Região: East US 2
    - Camada de dados: Na memória
3. Acesso à rede
    - Permitir acesso público de todas as redes
4. Clicar em Examinar + Criar
5. Aguardar a implantação ser concluída.

## 5095-video8.2-Configurando Redis no projeto Azure Function App

No roteiro abaixo, vamos configurar a Azure Function App para utilizar o cache Redis para armazenar os dados dos médicos.

1. Abra **Configurações > Autenticação** do cache Redis criado.
2. Na aba **Chaves de acesso**, habilite a autenticação.
3. Copie o valor da chave de acesso primária.
4. No projeto VollMed.FunctionApp, adicione no arquivo `local.settings.json` a nova variável RedisConnectionString.
5. A RedisConnectionString deve ser montada com a chave primária assim: 

```json
"RedisConnectionString": "<HOST>:<PORT>,password=<PRIMARY_KEY>,ssl=True,abortConnect=False"
```

6. Agora copie a seguinte linha para o arquivo local.settings.json e substitua o valor da senha pelo access key do Redis Cache:

```json
"RedisConnectionString": "vollmed-cache.redis.cache.windows.net:6380,password=***********,ssl=True,abortConnect=False"
```

No seu projeto Function App (`.csproj`), abra o terminal e adicione o pacote StackExchange.Redis:

O comando dotnet CLI equivalente para adicionar o pacote é:
```bash
dotnet add package StackExchange.Redis --version 2.7.33
dotnet add package Microsoft.Data.SqlClient --version 6.0.0
```

Agora, no final do arquivo da Function App `GerarProntuarioMedico.cs`, adicione as classes de apoio para o médico e especialidade:

```csharp
        public class Prontuario
        {
            public int MedicoId { get; set; }
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

Pronto, agora já podemos ajustar o código da Function App para utilizar o Redis como cache.

---

## 5095-video8.3-Ajuste no código da Function App

Abra o arquivo `GerarProntuarioMedico.cs` e importe os namespaces necessários no topo do arquivo:
```csharp
using System;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
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

Data do Atendimento: {0}

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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "prontuario/{id:int}")] HttpRequest req,
            int id)
```

Note acima que adicionamos o parâmetro `int id`, que será utilizado para buscar a consulta pelo ID.

Agora vamos implementar o corpo do método Run para utilizar o Redis como cache. Substitua o conteúdo do método Run pelo seguinte código:

```csharp
        _logger.LogInformation($"Processando prontuário do médico ID {id}");

        var db = _redis.GetDatabase();

        // 1️. Tenta obter do cache
        string? prontuarioJson = await db.StringGetAsync($"consulta:{id}");

        Prontuario? prontuario = null;

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
                var cmd = new SqlCommand("SELECT m.Id, m.Nome, m.Crm, m.Especialidade FROM [dbo].[medicos] as m INNER JOIN [dbo].[consultas] as c ON c.MedicoId = m.Id WHERE c.Id = @Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    prontuario = new Prontuario
                    {
                        MedicoId = reader.GetInt32(0),
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
```

---

## 5095-video8.4-Teste da Function App Local

1. Faça commit das alterações no Git:
```bash
git add .
git commit -m "Prontuário com Redis cache"
git push
```

2. Aguarde o deploy automático no Azure DevOps.

3. No portal do Azure, adicione a string de conexão do Redis na Function App:

Nome:
RedisConnectionString
Valor:
vollmed-cache.redis.cache.windows.net:6380,password=xxxxxxx=,ssl=True,abortConnect=False"

4. No portal do Azure, acesse a Function App, e procure pela função `GerarProntuarioMedico`.

5. Clique em **Obter URL da função**

6. Copie a URL e adicione o ID da consulta no final, por exemplo, a consulta com id 3:

```bash
https://vollmedfunctionapp20251026080859.azurewebsites.net/api/prontuario/3
```

7. Cole no navegador e acesse a URL.

8. Abra a aba **Logs** da Function App no portal do Azure para ver o resultado.

```
Processando prontuário do médico ID 2
Dados não encontrados no cache. Consultando o banco SQL...
Dados do médico salvos no cache Redis.
Prontuário médico gerado:
# Prontuário Médico
Data do Atendimento: 26/10/2025 10:45
...
```

Na segunda execução, ele trará:

```
Dados obtidos do cache Redis.
Prontuário médico gerado:
# Prontuário Médico
...
```

---

## ✅ Resultado Final

Você agora tem:

* **Azure Managed Redis** como cache distribuído para dados de médicos.
* **Azure SQL Database** como fonte de verdade.
* **Azure Function App** consumindo Redis + SQL, gerando o prontuário médico e logando o resultado.
