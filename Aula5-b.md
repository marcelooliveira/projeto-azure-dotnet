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


---

## 🎞️ **Apresentação: Application Insights no Azure**

---

### 🟦 **Problema: Falta de visibilidade**

**Bullet points:**

* Erros podem passar despercebidos
* Tempo de resposta é desconhecido
* Dificuldade em entender gargalos
* Falta de dados impede decisões técnicas

**Anotações do apresentador:**

* **Erros podem passar despercebidos:** Sem logs centralizados, falhas só aparecem quando o cliente reclama.
* **Tempo de resposta é desconhecido:** Sem métricas, não há como medir desempenho real.
* **Dificuldade em entender gargalos:** Sem telemetria, não sabemos se o problema é no app, banco ou rede.
* **Falta de dados impede decisões técnicas:** Não se pode otimizar o que não se mede.

---

### 🟦 **Solução: Observando sua aplicação**

**Bullet points:**

* Aplicações precisam ser monitoradas
* Telemetria revela o comportamento real
* Application Insights facilita essa visão
* É o primeiro passo em Observabilidade

**Anotações do apresentador:**

* **Aplicações precisam ser monitoradas:** Em produção, precisamos entender como os usuários interagem e como o sistema responde.
* **Telemetria revela o comportamento real:** Métricas e logs ajudam a entender desempenho, falhas e uso.
* **Application Insights facilita essa visão:** O Azure fornece esse serviço pronto, sem configuração complexa.
* **É o primeiro passo em Observabilidade:** Telemetria é a base para diagnóstico e melhoria contínua.













---

### 🟦 **Solução: Habilitando o Application Insights**

**Bullet points:**

* Gerenciar App Service no portal Azure
* Habilitar Application Insights
* Configure as opções para .NET Core

**Anotações do apresentador:**

* **Acesse o App Service no portal Azure:** Esse é o ponto de gerenciamento da aplicação publicada.
* **Clique em Habilitar Application Insights:** Isso ativa a coleta de métricas e logs automaticamente.
* **Configure as opções para .NET Core:** O Azure ajusta o SDK de telemetria conforme o tipo de app.

---

### 🟦 **Solução: Visualizando dados**

**Bullet points:**

* Interoperabilidade com o SDK
* Rastreamento de comandos SQL
* Exibir dados do Application Insights

**Anotações do apresentador:**

* **Ative a interoperabilidade com o SDK:** Isso garante que a Web API envie dados completos de execução.
* **Ative o rastreamento de comandos SQL:** Permite identificar consultas lentas ou com erro.
* **Acesse Application Insights novamente:** O menu mostra as métricas disponíveis.

---

### 🟦 **Teoria: Interpretando os painéis**

**Bullet points:**

* Solicitações com falha indicam erros
* Tempo de resposta mostra desempenho
* Solicitações do servidor indicam carga
* Disponibilidade / uptime

**Anotações do apresentador:**

* **Solicitações com falha indicam erros:** Mostra falhas HTTP e exceções da API.
* **Tempo de resposta mostra desempenho:** Ajuda a identificar gargalos no servidor.
* **Solicitações do servidor indicam carga:** Mostra o volume de tráfego processado.
* **Disponibilidade / uptime:** Indica se a aplicação está respondendo corretamente.

---

### 🟦 **Slide 6 – Teoria: Valor da Observabilidade**

**Bullet points:**

* Diagnóstico rápido de falhas
* Melhoria contínua baseada em dados
* Integração com Azure Monitor e Log Analytics
* Base para tracing distribuído com OpenTelemetry

**Anotações do apresentador:**

* **Diagnóstico rápido de falhas:** O Application Insights facilita encontrar a causa raiz de erros.
* **Melhoria contínua baseada em dados:** Métricas reais orientam otimizações e ajustes.
* **Integração com Azure Monitor e Log Analytics:** É parte de um ecossistema completo de observabilidade.
* **Base para tracing distribuído:** Será o próximo passo ao adicionar OpenTelemetry à solução VollMed.

---


---

### **1. Application Insights**

Azure Application Insights é a forma mais simples e visual de introduzir o conceito de telemetria — sem necessidade de código complexo. E por isso será nosso primeiro contato com observabilidade.

Nesse roteiro, vamos mostrar como coletar telemetria básica (requisições, dependências, exceções e tempo de resposta) da Web API VollMed.

## Habilitando Application Insights

1. Abrir o portal do Azure: https://portal.azure.com.
2. Abrir o serviço **App Service (Serviço de Aplicativos)**.
3. Localize o aplicativo do backend API da VollMed: **VollMedWebAPI2025xxxxxxxxxx**.
4. Abrir o menu **Monitoramento > Application Insights**.
5. Na página **Application Insights**, clique no botão **Habilitar o Application Insights**.
6. Na seção **Instrumentar o aplicativo**, clique na aba **.NET Core**.
7. Na seção **Interoperabilidade com o SDK do Application Insights**, mudar o status para **Ligado**.
8. Na seção **Comandos SQL**, mudar o status para **Ligado**.
9. No final da página, clique no botão **Aplicar**.
10. Aguarde o Azure aplicar as alterações.

## Visualizando dados do Application Insights

1. Abrir o serviço **App Service (Serviço de Aplicativos)**.
2. Iniciar o aplicativo **Vollmed.WebAPI**.
3. Iniciar o aplicativo **Vollmed.Web**.
4. No aplicativo **Vollmed.Web**, clique no link abaixo de **Domínio padrão**.
5. Navegue pela aplicação **Vollmed.Web**, abra as páginas de médicos e de consultas. Se houver erros, continue tentando abrir essas páginas.
6. Volte a abrir o serviço **App Service (Serviço de Aplicativos)**.
7. Localize o aplicativo do backend API da VollMed: **VollMedWebAPI2025xxxxxxxxxx**.
8. Abrir o menu **Monitoramento > Application Insights**.
9. Clique no link **Exibir dados do Application Insights**.
10. Note que temos 4 painéis do **Application Insights**:
	- Solicitações com falha
	- Tempo de resposta do servidor
	- Solicitações do servidor
	- Disponibilidade

## Solicitações com falha

1. Abra o painel **Solicitações com falha**
2. Na aba **Operações**, note os filtros **Servidor/Navegador**, **Hora Local** e **Funções**.
3. Note também os painéis: Falhas na contagem de solicitação, códigos de resposta, tipos de exceção, e dependências com falha.


---

### 🟩 **2. Azure Monitor e Log Analytics (observabilidade centralizada)**

O Azure Monitor é o serviço central de observabilidade do Azure — ele coleta, analisa e correlaciona métricas e logs de todos os recursos, incluindo VMs, bancos de dados e aplicações web.

É através dele que o Application Insights e o Log Analytics se integram, oferecendo uma visão completa do desempenho e da integridade dos serviços.

Nesse roteiro, vamos aprender a criar um novo Log Analytics Workspace, onde vão ser gravados os logs das aplicações da vollmed.

---

## **1. Criar um novo Log Analytics Workspace**

1. Acesse o [Portal do Azure](https://portal.azure.com).
2. Na barra de busca, pesquise por "Application Insights".
3. Marque e exclua cada um dos Application Insights que existirem.
4. Na barra de busca, pesquise por "workspaces" e clique no serviço "Workspaces do Log Analytics**.
3. Clique em **+ Criar**.
4. Escolha:
   * Grupo de recursos: vollmed-rg
   * Nome do workspace: vollmed-log-workspace
   * Região: igual à da sua aplicação (East US 2).
5. Clique em **Revisar + criar** e depois em **Criar**.

## **2. Associar o Application Insights com o novo Workspace do Log Analytics**

1. Acesse o recurso **Serviços de Aplicativos** e clique no app de backend **VollMed.WebAPI**.
2. No menu lateral esquerdo, clique **Monitoramento > Application Insights**.
3. Na seção **Vincular a um recurso do Application Insights**, clicar em **Criar novo recurso**.
4. Escolher o **Workspace do Log Analytics**: vollmed-log-workspace.
5. Clique em **Aplicar**.

*Dessa forma, todos os logs do Application Insights irão automaticamente para o Workspace do Log Analytics*: "vollmed-log-workspace".

6. No menu lateral esquerdo, clique **Monitoramento > Adicionar configurações de diagnóstico** e prencha o formulário.
	- Nome da configuração de diagnóstico: diagnostico vollmed
	- Logs > Grupos de categorias: allLogs
	- Métricas: AllMetrics
	- Detalhes do destino: Enviar para o workspace do Log Analytics
	- Workspace do Log Analytics: vollmed-log-workspace
	- clicar no botão **Salvar**.
7. Aguarde enquanto a implantação está em andamento.
*Este workspace será o "repositório central" dos logs que você irá coletar.*

##  **3. Configurar Log Analytics no App Service**

1. No portal do Azure, abra o Serviço de Aplicativos.
2. Localize a aplicação de backend **VollMed.WebAPI**.
3. Abra o menu **Configurações > Variáveis de Ambiente**.
4. Abra a configuração `APPINSIGHTS_INSTRUMENTATIONKEY`.
5. Copie o valor dessa configuração, cole num bloco de notas.
6. Adicione esta variável de ambiente:

| Nome da variável                          | Valor         |
| ----------------------------------------- | ------------- |
| `ApplicationInsights__ConnectionString`   | `InstrumentationKey=[APPINSIGHTS_INSTRUMENTATIONKEY]` |

Depois disso, clique em "Aplicar" e "Salvar".

##  **4. Configurar Log Analytics no Visual Studio**

1. No portal do Azure, abra o Serviço de Aplicativos.
2. Localize a aplicação de backend **VollMed.WebAPI**.
3. Abra o menu **Configurações > Variáveis de Ambiente**.
4. Adicione mais duas variáveis de ambiente:

| Nome da variável                          | Valor         |
| ----------------------------------------- | ------------- |
| `Logging__LogLevel__Default`              | `Information` |
| `Logging__LogLevel__Microsoft.AspNetCore` | `Information` |

Depois disso, clique em "Aplicar" e "Salvar".


5. Abra o projeto **VollMed.WebAPI** no Visual Studio.
6. No arquivo `appsettings.Development.json`, cole esta configuração:

```json
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=[VALOR DO APPINSIGHTS_INSTRUMENTATIONKEY]"
  }
```

7. No terminal PowerShell, vá para a pasta do projeto VollMed.WebAPI e rode estes comandos:

```console
dotnet add package Azure.Monitor.OpenTelemetry.AspNetCore
dotnet add package Microsoft.ApplicationInsights.AspNetCore
dotnet add package Microsoft.Extensions.Logging.ApplicationInsights
```

##  **4. Habilitar Telemetria e Republicar o App**

1. No arquivo `Program.cs`, adicione logo antes da declaração `var app = builder.Build();`:

```csharp
// Habilita o Application Insights (telemetria automática)
builder.Services.AddApplicationInsightsTelemetry();

// Configura telemetria
builder.Services.Configure<TelemetryConfiguration>((config) =>
{
    config.TelemetryChannel.DeveloperMode = true; // flush imediato
});

// Habilita o provedor de logging do Application Insights
builder.Logging.AddApplicationInsights();

builder.Services.AddOpenTelemetry()
    .UseAzureMonitor(o =>
    {
        o.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    });
```

2. Agora, modifique o `ConsultaController` para enviar telemetria sempre que uma consulta for agendada.

Crie um campo privado `_logger` e o inicialize no construtor por injeção de dependência:

```csharp
    public class ConsultaController : ControllerBase
    {
        private readonly IConsultaService _consultaservice;
        private readonly IMedicoService _medicoService;
        private readonly ILogger<ConsultaController> _logger;     

        public ConsultaController(IConsultaService consultaService, IMedicoService medicoService, ILogger<ConsultaController> logger)
        {
            _consultaservice = consultaService;
            _medicoService = medicoService;
            _logger = logger;
        }
```

3. No método `SalvarAsync`, adicione o log de informação antes do retorno do método:

```csharp
				await _consultaservice.CadastrarAsync(dados);
                _logger.LogInformation("Consulta criada para o Paciente {0} com o médico {1} na data/hora {2} {3}", dados.Paciente, dados.IdMedico, dados.Data.ToShortDateString(), dados.Data.ToShortTimeString());
				return Ok(dados);
```

4. Compile a aplicação.

5. Clique com botão direito sobre o nome do projeto WebAPI, e clique no botão Publish.

6. Em seguida, clique no botão Publish da página para publicar a aplicação no Azure.


---

## **5. Gerar logs reais na aplicação**

1. Inicie a aplicação **VollMed.WebAPI**.
2. Inicie a aplicação **VollMed.Web**.
3. Entre no menu **Consultas** e crie 3 novas consultas médicas.

👉 *Essas interações serão enviadas para o Workspace do Log Analytics através de telemetria.*

---

## **6. Consultar logs no Log Analytics**

1. No portal do Azure, abra o **Workspace do Log Analytics**.
2. Abra o "vollmed-log-workspace"
3. Na janela de consultas, execute:

```kusto
AppTraces
```

4. Agora execute a consulta com filtro:

```kusto
AppTraces
| where Message startswith "Consulta criada"
```

5. Clque em **Salvar como consulta**.
6. Dê o nome "Consultas médicas criadas"
7. Crie em uma nova aba e execute a mesma consulta:

```kusto
AppTraces
| where Message startswith "Consulta criada"
```
8. Desta vez, clique em **Salvar > Fixar em Painel do Azure**.
9. Salve no painel "My Dashboard".
10. Abra o "Dashboard Hub" no portal do Azure
11. lique em "My Dashboard".
12. Visualize as consultas criadas.
