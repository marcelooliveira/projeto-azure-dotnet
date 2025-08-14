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


# Aula 4

070. Registrar apps no MS Entra ID:

### **Passo 1 � Registrar a Web API no Entra ID**

1. No **Portal do Azure** > **Microsoft Entra ID** > **Registros de aplicativos** > **Novo registro**.
2. Nome: `VollMed.WebAPI`.
3. Contas: �Somente contas neste diret�rio organizacional� (mais simples por enquanto).
4. Registrar.
5. **Expor uma API** > Defina o *URI de ID do aplicativo* (por exemplo, `api://<api-client-id>`).
6. Adicione um escopo:

   * Nome: `vollmed_api.all`
   * Quem pode dar consentimento: Administradores e usu�rios
   * Nome de exibi��o para consentimento do administrador: �Acesso completo VollMed.WebAPI�
   * Salvar.

### **Passo 2 � Registrar o aplicativo MVC**

1. **Novo registro** > Nome: `VollMed.Web`.
2. Contas: �Somente contas neste diret�rio organizacional�.
3. URI de redirecionamento:
   * Tipo: Web
   * URL: `https://vollmedweb2025XXXXXXXXXXXXXX.azurewebsites.net/signin-oidc`
4. Aba **Autentica��o**:

   * Abaixo de "Front-channel logout URL", adicionar URL de logout: `https://vollmedweb2025XXXXXXXXXXXXXX.azurewebsites.net/signout-callback-oidc`
   * Ativar �ID tokens� e �Access tokens�.
5. **Permiss�es de API**:

   * Adicionar > APIs my organization uses > `VollMed.WebAPI` > `vollmed_api.all` > Conceder consentimento do administrador.
   * Salvar
   * Clicar em "Grant admin consent for ..."

6. Criar um secret: No app registration "VollMed.Web"
    * ao lado de "Client credentials" clique "add secret"
    * + new client secret
    * Description: secret
    * Salvar
    * Copiar e guardar o valor 

### **Passo 3 � Autorizar app MVC no app WebAPI** 

1. Abrir o app registration "VollMed.WebAPI"
2. Ir ao menu Manage > Expose an API
3. Em "Authorized client applications" clicar em Add a client application
4. Adicionar o Application (client) ID do app registration "VollMed.Web"
5. Marcar como "authorized scope": api://xxxxxxxxxxxxx/vollmed_api.all

080. Integra��o com MS Entra ID (Azure AD) e Autoriza��o JWT

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

**Explica��o:**
Remova o coment�rio da anota��o `[Authorize]` nos controllers para exigir autentica��o nas rotas protegidas.

---

## 3. Configurar autentica��o e autoriza��o no Web (MVC)

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

**Explica��o:**
Habilite autentica��o OpenID Connect e integra��o com MS Entra ID, al�m de aquisi��o de tokens para chamadas � API protegida.

## 3. Ajustar BaseHttpService.cs em VollMed.Web:

A ideia � passar o access token no cabe�alho das requisi��es:

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



## 4. Ativar autentica��o e autoriza��o no pipeline

```csharp
// Antes:
//app.UseAuthentication();
//app.UseAuthorization();

// Depois:
app.UseAuthentication();
app.UseAuthorization();
```

**Explica��o:**
Garanta que o middleware de autentica��o e autoriza��o est� ativo na aplica��o.

---

## 5. Ativar `[Authorize]` nos Controllers da WebAPI

```csharp
// Antes:
//[Authorize]
// Depois:
[Authorize]
```

**Explica��o:**
Exija autentica��o para acessar rotas da API, protegendo endpoints sens�veis.

---

## 6. Configurar autentica��o JWT na WebAPI

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

**Explica��o:**
Configure autentica��o JWT usando MS Entra ID para proteger a WebAPI, vinculando as configura��es do Azure AD.



090. Republicar os apps

    * Publish Vollmed.WebAPI
    * Publish Vollmed.Web


