| Aula| SQL | API / MVC | auth | Monitoring |    Synch       | Infra     | Caching     |
| ---| --- | --- | --- | --- |    ---       | ---     | ---     |
| 1 | local | local |     -    |      -     |    s�ncrono    | az portal |     -       |
| 2 | CLOUD | local |     -    |      -     |    s�ncrono    | az portal |     -       |
| 3 | CLOUD | CLOUD |     -    |      -     |    s�ncrono    | az portal |     -       |
| 4 | CLOUD | CLOUD | MS ENTRA |      -     |    s�ncrono    | az portal |     -       |
| 5 | CLOUD | CLOUD | MS ENTRA | AZ MONITOR |    s�ncrono    | az portal |     -       |
| 6 | CLOUD | CLOUD | MS ENTRA | AZ MONITOR | MSG/ASS�NCRONO | az portal |     -       |
| 7 | CLOUD | CLOUD | MS ENTRA | AZ MONITOR | MSG/ASS�NCRONO | IAAC      |     -       |
| 8 | CLOUD | CLOUD | MS ENTRA | AZ MONITOR | MSG/ASS�NCRONO | IAAC      | AZURE REDIS |


# Aula 5 - Habilitar o OpenTelemetry do Azure Monitor para aplicativos .NET

habilitar e configurar a coleta de dados baseada em OpenTelemetry no Azure Monitor Application Insights. Distribui��es do OpenTelemetry do Azure Monitor:

- Fornece uma distribui��o do OpenTelemetry que inclui suporte para recursos espec�ficos do Azure Monitor.
- Habilita telemetria autom�tica, incluindo bibliotecas de instrumenta��o OpenTelemetry para coletar rastreamentos, m�tricas, logs e exce��es.
- Permite coletar telemetria personalizada.
- Oferece suporte ao recurso Live Metrics para monitorar e coletar mais telemetria de aplicativos Web ao vivo e em produ��o.

## Habilitar o OpenTelemetry com o Application Insights

### Instalar a biblioteca de clientes

Instale o pacote Azure.Monitor.OpenTelemetry.AspNetCoreNuGet mais recente:

```powershell
dotnet add package Azure.Monitor.OpenTelemetry.AspNetCore
```

### Modificar o aplicativo

Importe o Azure.Monitor.OpenTelemetry.AspNetCore namespace, adicione o OpenTelemetry e configure-o para usar o Azure Monitor em sua program.cs classe:

```csharp
// Import the Azure.Monitor.OpenTelemetry.AspNetCore namespace.
using Azure.Monitor.OpenTelemetry.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add OpenTelemetry and configure it to use Azure Monitor.
builder.Services.AddOpenTelemetry().UseAzureMonitor();

var app = builder.Build();

app.Run();
```

### Copie a string de conex�o do seu recurso do Application Insights

A cadeia de conex�o � exclusiva e especifica para onde as Distribui��es do OpenTelemetry do Azure Monitor enviam a telemetria coletada.

Para copiar a cadeia de conex�o:

- Acesse o painel Vis�o geral do recurso Application Insights.
- Encontre sua cadeia de conex�o.
- Passe o mouse sobre a cadeia de conex�o e selecione o �cone Copiar para a �rea de transfer�ncia.

![Img 638912401486128309](img_638912401486128309.png)

### Colar a cadeia de conex�o em seu ambiente

APPLICATIONINSIGHTS_CONNECTION_STRING=<Your connection string>

### Confirmar se os dados est�o fluindo

Abra o seu aplicativo e, em seguida, abra o Application Insights no portal do Azure. Pode levar alguns minutos para os dados aparecerem.

![Img 638912402421624587](img_638912402421624587.png)


