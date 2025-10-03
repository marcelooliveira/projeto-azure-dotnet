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

## Registrar apps no MS Entra ID:

### **Passo 1 – Registrar a Web API no Microsoft Entra ID**

1. No **Portal do Azure** > **Microsoft Entra ID**
2. Clicar em **+Adicionar**, escolher **Registros de aplicativos**
3. Nome: `VollMed.WebAPI`.
4. Contas: “Somente contas neste diretório organizacional” (mais simples por enquanto).
5. Registrar.
6. No menu lateral esquerdo, clique em **Gerenciar > Expor uma API**
7. Na parte superior, em **URI da ID do aplicativo**, clique em **Adicionar**.
8. Vai ser gerado uma URI nova, como por exemplo "api://de76203e-4aa2-46e3-bee4-35cac7c6b1cc".
9. Clique para salvar a URI do aplicativo.
10. Clique em **+Adicionar um Escopo**
   * Nome do escopo: `vollmed_api.all`
   * Quem pode dar consentimento: Administradores e usuários
   * Nome de exibição para consentimento do administrador: "Acesso completo VollMed.WebAPI"
   * Descrição de consentimento do administrador: "Acesso completo VollMed.WebAPI"
   * Clique em **Adicionar escopo**

### **Passo 2 – Registrar o aplicativo MVC no Microsoft Entra ID**

1. No **Portal do Azure** > **Microsoft Entra ID**
2. Clicar em **+Adicionar**, escolher **Registros de aplicativos**
3. Nome: `VollMed.Web`.
2. Contas: “Somente contas neste diretório organizacional”.
3. URI de redirecionamento (cloud):
   * Tipo: Web
   * URL: `https://localhost:5001/signin-oidc`
4. Registrar.
5. Em **Visão Geral**, clicar em **URIs de Redirecionamento: 1 Web, 0 SPA, 0 cliente público**
7. Salvar.
   * Abaixo de "Front-channel logout URL" (URL de logoff do canal frontal), adicionar URL de logout: `https://localhost:5001/signout-callback-oidc`
8. Na seção **Concessão implícita e fluxos híbridos**, marcar “ID tokens” e “Access tokens”.
9. Clicar em Salvar.
10. No menu lateral, clique em **Gerenciar > Permissões de API**:
11. Na seção **Permissões configuradas**, clique em **+Adicionar uma permissão**.
12. Clicar na aba **APIs que a minha organização usa**
13. Buscar por "VollMed.WebAPI"
14. Selecionar essa API
15. Na seção Selecionar permissões, selecione "vollmed_api.all"
16. Clique em "Adicionar Permissões".
17. Na seção **Permissões configuradas**, clique em "Conceder consentimento do administrador".
18. Criar um secret: No app registration "VollMed.Web"
    * Ao lado de "Credenciais de cliente" clique "Adicionar um certificado ou segredo"
    * Clicar em **+Novo segredo do cliente**
    * - Descrição: secret
    * Clicar em **Adicionar**
    * Copiar e guardar o valor

### **Passo 3 – Autorizar app MVC no app VollMed.WebAPI** 

1. No **Portal do Azure** > **Microsoft Entra ID**
2. No menu lateral esquerdo, clique em **Gerenciar > Registros de aplicativo**
3. Clicar na aba **Todos os aplicativos**
4. Filtrar por "VollMed"
5. Procurar o registro de aplicativo do Front MVC (Vollmed.Web)
6. Copiar o ID do aplicativo (cliente) do Registro de Aplicativo "VollMed.Web" e guardar em outro lugar
7. Procurar o registro de aplicativo do Backend WebAPI (VollMed.WebAPI).
8. Selecionar o registro de aplicativo "VollMed.WebAPI"
6. No menu lateral esquerdo, clique em **Gerenciar > Expor uma API**
7. Em "Aplicativos cliente autorizados" clicar em **+Adicionar um aplicativo cliente**
8. Adicionar o Application (client) ID do Registro de Aplicativo "VollMed.Web"
9. Marcar como "escopo autorizado": api://xxxxxxxxxxxxx/vollmed_api.all
10. Clicar em **Adicionar aplicativo**

### **Passo 4 – Configurar o MS Entra ID No Projeto VollMed.WebApi

#### 1. Adicione configurações do MS Entra ID (AzureAD)

Adicionar o seguinte bloco de configurações no arquivo **appsettings.Development.json**:

```json
  "AzureAd": {
	"Instance": "https://login.microsoftonline.com/",
	"Domain": "xxxxxx.onmicrosoft.com",
	"TenantId": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
	"ClientId": "[CLIENT-ID-DO-APP-REGISTRATION-DO-API]",
	"Audience": "api://[CLIENT-ID-DO-APP-REGISTRATION-DO-API]",
    "Authority": "https://login.microsoftonline.com/[TENANT-ID]/v2.0"
  }
```

#### 2. Ativar `[Authorize]` nos Controllers da VollMed.WebAPI

* \VollMed.WebAPI\Controllers\ConsultaController.cs
* \VollMed.WebAPI\Controllers\MedicoController.cs

```csharp
[Authorize]
```

**Explicação:**
Exija autenticação para acessar rotas da API, protegendo endpoints sensíveis.

---

#### 3. Configurar autenticação JWT na VollMed.WebAPI

Abrir o terminal na pasta-raiz da solução.

Executar os comandos para instalar os pacotes de autenticação e identidade:

```powershell
cd VollMed.WebAPI

dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.Identity.Web
```

No arquivo `Program.cs` adicione os middlewares abaixo:

```csharp
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

#### 4. Ativar autenticação e autorização no pipeline do VollMed.WebAPI

No arquivo `Program.cs`, adicione as linhas **antes** de `app.UseHttpsRedirection();`

```csharp
app.UseAuthentication();
app.UseAuthorization();
```


### **Passo 5 –  

#### 1. Adicione configurações do MS Entra ID (AzureAD)

Adicionar o seguinte bloco de configurações no arquivo **appsettings.Development.json**:

```json
  "AzureAd": {
	"Instance": "https://login.microsoftonline.com/",
	"Domain": "xxxxxx.onmicrosoft.com",
	"TenantId": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
	"ClientId": "[CLIENT-ID-DO-APP-REGISTRATION-DO-MVC]",
	"ClientSecret": "[SECRET-DO-APP-REGISTRATION-DO-MVC]",
	"CallbackPath": "/signin-oidc"
  },
  "VollMed_WebApi": {
	"Name": "VollMed.WebApi",
	"BaseAddress": "https://localhost:6001",
	"Scope": "api://xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx/vollmed_api.all"
  }
```

#### 2. Habilite o SignOut no projeto VollMed.WebAPI

* Abra o arquivo `HomeController.cs`
* Descomente as linhas do método `Logout`

```csharp
        [HttpPost]
        public IActionResult Logout()
        {
            return SignOut(
                new AuthenticationProperties { RedirectUri = "/" },
                OpenIdConnectDefaults.AuthenticationScheme,  // "OpenIdConnect"
                CookieAuthenticationDefaults.AuthenticationScheme // "Cookies"
            );
        }
```

Esse método realiza o logout completo do usuário em uma aplicação ASP.NET Core. Ele encerra tanto a sessão local, removendo o cookie de autenticação, quanto a sessão no provedor de identidade configurado via OpenID Connect (como o Microsoft Entra ID). Após finalizar o logout, o usuário é automaticamente redirecionado para a página inicial da aplicação (/).

#### 3. Ativar `[Authorize]` nos Controllers

`VollMed.Web/Controllers/ConsultaController.cs`

```csharp
[Authorize]
```
`VollMed.Web/Controllers/MedicoController.cs`

```csharp
[Authorize]
```

**Explicação:**
Remova o comentário da anotação `[Authorize]` nos controllers para exigir autenticação nas rotas protegidas.

---

#### 4. Configurar autenticação e autorização no Web (MVC)

Abra o terminal no Visual Studio (CTRL + apóstrofo)

Instale os pacotes para autenticação e autorização:

```powershell
cd VollMed.Web
dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect
dotnet add package Microsoft.Identity.Web
dotnet add package Microsoft.Identity.Web.DownstreamApi
```

Em `Program.cs`, adicione esta linha antes de `var app = builder.Build();`:

```csharp
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
```

Em seguida, adicione esses middlewares de autenticação e autorização:

```csharp
builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddDownstreamApi("VollMed.WebApi", builder.Configuration.GetSection("VollMed.WebApi"))
    .AddInMemoryTokenCaches();
```

**Explicação:**
Habilite autenticação OpenID Connect e integração com MS Entra ID, além de aquisição de tokens para chamadas à API protegida.

#### 5. Ativar autenticação e autorização no pipeline em VollMed.Web

No arquivo `Program.cs`, adicione as linhas abaixo depois de `app.UseRouting();`

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

#### 6. Ajustar BaseHttpService.cs em VollMed.Web:

A ideia é passar o access token no cabeçalho das requisições.

Adicione e inicialize o campo somente-leitura `_tokenAcquisition`:

```csharp
        private readonly ITokenAcquisition _tokenAcquisition;
        .
        .
        .
        public BaseHttpService(ITokenAcquisition tokenAcquisition,
        ...
        _tokenAcquisition = tokenAcquisition;
        .
        .
        .
```

Adicione o método `SetTokenAsync` no final da classe `BaseHttpService`:

```csharp
        private async Task SetTokenAsync(HttpClient httpClient)
        {
            string[] scopes = [_configuration["VollMed_WebApi:Scope"]];

            try
            {
                // Tenta pegar o token silenciosamente (AcquireTokenSilent)
                var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
            }
            catch (MsalUiRequiredException)
            {
                // Se não conseguir de forma silenciosa, redireciona para login
                // (em API pode lançar para o middleware de autenticação tratar)
                throw;
            }
        }
```

Adicione a chamada a `SetTokenAsync` no método `GetHttpClientAsync`:

```csharp
        private async Task<HttpClient> GetHttpClientAsync()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient(_configuration["VollMed_WebApi:Name"] ?? "");
            await SetTokenAsync(httpClient);
            return httpClient;
        }
```

Agora abra o arquivo `\VollMed.Web\Services\MedVollApiService.cs` e adicione o parâmetro `tokenAcquisition` no construtor:

```csharp
        public VollMedApiService(
            ITokenAcquisition tokenAcquisition
            , IConfiguration configuration
            , IHttpClientFactory httpClientFactory
            , ILogger<BaseHttpService> logger)
            : base(tokenAcquisition, configuration, httpClientFactory, logger)
```




**Explicação:**
Garanta que o middleware de autenticação e autorização está ativo na aplicação.

#### 7. Testar aplicação local com autorização e autenticação

* Rode os 2 projetos da solução com a tecla F5.
* Entre no menu **Médicos**
* Obs.: nas primeiras vezes, a operação pode falhar com time-out.

===========================

# FAÇA COMO EU FIZ

`https://vollmedweb2025XXXXXXXXXXXXXX.azurewebsites.net/signin-oidc`

`https://vollmedweb2025XXXXXXXXXXXXXX.azurewebsites.net/signout-callback-oidc`

### Environment Variables - VollMed.WebApi

Abrir portal do Azure
Adicionar as variáveis de ambiente no App Service VollMed.WebApi:

```bash
AzureAd__Instance=https://login.microsoftonline.com/
AzureAd__Domain=xxxxx.onmicrosoft.com
AzureAd__TenantId=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
AzureAd__ClientId=[CLIENT-ID-DO-APP-REGISTRATION-DO-API]
AzureAd__Audience=api://xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
```


## Environment Variables - VollMed.Web

Abrir portal do Azure
Adicionar as variáveis de ambiente no App Service VollMed.WebApi:

```bash
AzureAd__Instance=https://login.microsoftonline.com/
AzureAd__Domain=xxxxxxxxxxx.onmicrosoft.com
AzureAd__TenantId=xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
AzureAd__ClientId=[CLIENT-ID-DO-APP-REGISTRATION-DO-MVC]
AzureAd__ClientSecret=[SECRET-DO-APP-REGISTRATION-DO-MVC]
VollMed_WebApi__Scope=api://xxxxxxxxxxxxx/vollmed_api.all
```


