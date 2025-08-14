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


# Aula 4

070. Registrar apps no MS Entra ID:

### **Passo 1 – Registrar a Web API no Entra ID**

1. No **Portal do Azure** > **Microsoft Entra ID** > **Registros de aplicativos** > **Novo registro**.
2. Nome: `VollMed.WebAPI`.
3. Contas: “Somente contas neste diretório organizacional” (mais simples por enquanto).
4. Registrar.
5. **Expor uma API** > Defina o *URI de ID do aplicativo* (por exemplo, `api://<api-client-id>`).
6. Adicione um escopo:

   * Nome: `vollmed_api.all`
   * Quem pode dar consentimento: Administradores e usuários
   * Nome de exibição para consentimento do administrador: “Acesso completo VollMed.WebAPI”
   * Salvar.

### **Passo 2 – Registrar o aplicativo MVC**

1. **Novo registro** > Nome: `VollMed.Web`.
2. Contas: “Somente contas neste diretório organizacional”.
3. URI de redirecionamento:
   * Tipo: Web
   * URL: `https://vollmedweb2025XXXXXXXXXXXXXX.azurewebsites.net/signin-oidc`
4. Aba **Autenticação**:

   * Abaixo de "Front-channel logout URL", adicionar URL de logout: `https://vollmedweb2025XXXXXXXXXXXXXX.azurewebsites.net/signout-callback-oidc`
   * Ativar “ID tokens” e “Access tokens”.
5. **Permissões de API**:

   * Adicionar > APIs my organization uses > `VollMed.WebAPI` > `vollmed_api.all` > Conceder consentimento do administrador.
   * Salvar
   * Clicar em "Grant admin consent for ..."

6. Criar um secret: No app registration "VollMed.Web"
    * ao lado de "Client credentials" clique "add secret"
    * + new client secret
    * Description: secret
    * Salvar
    * Copiar e guardar o valor 

### **Passo 3 – Autorizar app MVC no app WebAPI** 

1. Abrir o app registration "VollMed.WebAPI"
2. Ir ao menu Manage > Expose an API
3. Em "Authorized client applications" clicar em Add a client application
4. Adicionar o Application (client) ID do app registration "VollMed.Web"
5. Marcar como "authorized scope": api://xxxxxxxxxxxxx/vollmed_api.all

080. Integração com MS Entra ID (Azure AD) e Autorização JWT

## 1. Configurar o MS Entra ID Nos projetos

### Projeto VollMed.WebApi

```json
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "xxxxx.onmicrosoft.com",
    "TenantId": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "ClientId": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "Audience": "api://xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  }
```

### Environment Variables - VollMedWebApi

```bash
AzureAd__Instance=https://login.microsoftonline.com/
AzureAd__Domain=xxxxx.onmicrosoft.com
AzureAd__TenantId=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
AzureAd__ClientId=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
AzureAd__Audience=api://xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
```

### VollMed.Web

```json
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "xxxxxxxx.onmicrosoft.com",
    "TenantId": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "ClientId": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "ClientSecret": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "CallbackPath": "/signin-oidc"
  },
  "VollMed_WebApi": {
    "Name": "VollMed.WebApi",
    "BaseAddress": "https://vollmedwebapixxxxxxxxxxxx.azurewebsites.net",
    "Scope": "api://xxxxxxxxxxxxx/vollmed_api.all"
  }
```

## Environment Variables - VollMed.Web

```bash
AzureAd__Instance=https://login.microsoftonline.com/
AzureAd__Domain=xxxxxxxxxxx.onmicrosoft.com
AzureAd__TenantId=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
AzureAd__ClientId=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
AzureAd__ClientSecret=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
VollMed_WebApi__Name=VollMed.WebApi
VollMed_WebApi__BaseAddress=https://vollmedwebapixxxxxxxxxxxx.azurewebsites.net
VollMed_WebApi__Scope=api://xxxxxxxxxxxxx/vollmed_api.all
```

## 2. Ativar `[Authorize]` nos Controllers

```csharp
// Antes:
//[Authorize]
// Depois:
[Authorize]
```

**Explicação:**
Remova o comentário da anotação `[Authorize]` nos controllers para exigir autenticação nas rotas protegidas.

---

## 3. Configurar autenticação e autorização no Web (MVC)

```csharp
// Antes:
//builder.Services
//    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
//    .EnableTokenAcquisitionToCallDownstreamApi()
//    .AddDownstreamApi("VollMed.WebApi", builder.Configuration.GetSection("VollMed.WebApi"))
//    .AddInMemoryTokenCaches();

// Depois:
builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddDownstreamApi("VollMed.WebApi", builder.Configuration.GetSection("VollMed.WebApi"))
    .AddInMemoryTokenCaches();
```

**Explicação:**
Habilite autenticação OpenID Connect e integração com MS Entra ID, além de aquisição de tokens para chamadas à API protegida.

## 3. Ajustar BaseHttpService.cs em VollMed.Web:

A ideia é passar o access token no cabeçalho das requisições:

```csharp
        private readonly ITokenAcquisition _tokenAcquisition;
        .
        .
        .
        public BaseHttpService(ITokenAcquisition tokenAcquisition,
        .
        .
        .
        private async Task<HttpClient> GetHttpClientAsync()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient(_configuration["VollMed_WebApi:Name"] ?? "");
            await SetTokenAsync(httpClient);
            return httpClient;
        }

        private async Task SetTokenAsync(HttpClient httpClient)
        {
            // Pega o escopo configurado para a API
            string[] scopes = new[] { _configuration["VollMed_WebApi:Scope"] };

            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
            httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", accessToken);
        }
```



## 4. Ativar autenticação e autorização no pipeline

```csharp
// Antes:
//app.UseAuthentication();
//app.UseAuthorization();

// Depois:
app.UseAuthentication();
app.UseAuthorization();
```

**Explicação:**
Garanta que o middleware de autenticação e autorização está ativo na aplicação.

---

## 5. Ativar `[Authorize]` nos Controllers da WebAPI

```csharp
// Antes:
//[Authorize]
// Depois:
[Authorize]
```

**Explicação:**
Exija autenticação para acessar rotas da API, protegendo endpoints sensíveis.

---

## 6. Configurar autenticação JWT na WebAPI

```csharp
// Antes:
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(options => {
//        builder.Configuration.Bind("AzureAd", options);
//    },
//    options => {
//        builder.Configuration.Bind("AzureAd", options);
//    });
//builder.Services.AddAuthorization();

// Depois:
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options => {
        builder.Configuration.Bind("AzureAd", options);
    },
    options => {
        builder.Configuration.Bind("AzureAd", options);
    });
builder.Services.AddAuthorization();
```

**Explicação:**
Configure autenticação JWT usando MS Entra ID para proteger a WebAPI, vinculando as configurações do Azure AD.



090. Republicar os apps

    * Publish Vollmed.WebAPI
    * Publish Vollmed.Web


