# Aula2 - Autenticação e Autorização 

-----

Assumindo que os App Services para `VollMed.Web`, `VollMed.WebAPI` e `VollMed.Identity` já estão provisionados no grupo de recursos `vollmed-rg`, os principais passos com Azure CLI seriam para registrar aplicações no Azure AD e configurar a Managed Identity.

-----

## Comandos Azure CLI para Autenticação e Autorização com Azure AD

Os comandos a seguir focarão nas etapas de configuração do Azure Active Directory (Azure AD), que são o cerne da aula de Autenticação e Autorização.

### 1\. Registro da Aplicação `VollMed.Web` (Cliente OIDC no Azure AD)

Mesmo que `VollMed.Web` se autentique primariamente com `VollMed.Identity`, para cenários mais avançados ou para simplificar a federação de identidades, podemos registrar `VollMed.Web` diretamente no Azure AD como uma aplicação cliente. Isso é útil se você quiser que o **Azure AD seja o provedor de identidade direto para `VollMed.Web`**.

```bash
# Entrar na sua conta do Azure CLI (se ainda não estiver logado)
az login

# Definir a assinatura (substitua pelo ID da sua assinatura)
az account set --subscription "SUA_SUBSCRIPTION_ID"

# 1. Registrar a aplicação cliente (VollMed.Web) no Azure AD
#    É a aplicação que inicia o fluxo de login
az ad app create \
    --display-name "VollMed.Web" \
    --sign-in-audience "AzureADMyOrg" \
    --web-redirect-uris "https://localhost:5002/signin-oidc" "https://vollmed-web-app.azurewebsites.net/signin-oidc" \
    --identifier-uris "api://vollmed-web" \
    --query appId --output tsv

# Anote o appId retornado. Ele será o Client ID para VollMed.Web.
# Exemplo de saída: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
```

### 2\. Registro da Aplicação `VollMed.WebAPI` (API Protegida no Azure AD)

A `VollMed.WebAPI` precisará ser registrada no Azure AD para que o Azure AD saiba que ela é um recurso protegido e possa emitir tokens para ela.

```bash
# 2. Registrar a aplicação API (VollMed.WebAPI) no Azure AD
az ad app create \
    --display-name "VollMed.WebAPI" \
    --identifier-uris "api://vollmed-webapi" \
    --query appId --output tsv

# Anote o appId retornado. Ele será o Client ID da API.
# Exemplo de saída: "ffffffff-gggg-hhhh-iiii-jjjjjjjjjjjj"
```

#### Expor uma Permissão (Scope) para a `VollMed.WebAPI`

Agora, vamos definir as permissões que a `VollMed.WebAPI` expõe, para que outras aplicações (como o `VollMed.Web` ou o `VollMed.Identity` ao federar) possam solicitá-las. No seu `Program.cs` da WebAPI, você tem `policy.RequireClaim("scope", "VollMed.WebAPI")`. Vamos alinhar isso:

```bash
# Substitua pelo Client ID da VollMed.WebAPI que você anotou acima
VollMedWebAPI_AppId="ffffffff-gggg-hhhh-iiii-jjjjjjjjjjjj"

# Expor a permissão "VollMed.WebAPI"
az ad app permission add --id $VollMedWebAPI_AppId --api $VollMedWebAPI_AppId --api-permissions "user_impersonation=Scope"

# Definir o nome da permissão para "VollMed.WebAPI"
# (Este comando é um pouco mais complexo via CLI, geralmente é feito no portal para custom scopes.
# O user_impersonation é um scope padrão que pode ser renomeado via portal ou manifesto).
# Para simplificar na CLI, usaremos 'user_impersonation' e focaremos em 'VollMed.WebAPI' no código.
# Para configurar um custom scope exato "VollMed.WebAPI", seria necessário editar o manifesto da aplicação:
# 1. az ad app show --id $VollMedWebAPI_AppId --query "web.oauth2Permissions" # Ver as permissões existentes
# 2. Copiar o conteúdo, adicionar o novo scope "VollMed.WebAPI" e então usar:
# az ad app update --id $VollMedWebAPI_AppId --manifest <json-string-do-manifesto-atualizado>
# Para fins didáticos e simplificar a CLI aqui, focaremos em adicionar uma permissão e validar o scope no código.
```

### 3\. Conceder Permissões (para `VollMed.Web` acessar `VollMed.WebAPI`)

Se o `VollMed.Web` for chamar diretamente a `VollMed.WebAPI` usando tokens do Azure AD (não via Identity Server), ele precisará de permissão.

```bash
# Substitua pelo Client ID da VollMed.Web que você anotou
VollMedWeb_AppId="aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
# Substitua pelo Client ID da VollMed.WebAPI que você anotou
VollMedWebAPI_AppId="ffffffff-gggg-hhhh-iiii-jjjjjjjjjjjj"

# 3. Conceder permissões para VollMed.Web acessar VollMed.WebAPI
# Isso permite que VollMed.Web solicite o scope da VollMed.WebAPI
az ad app permission add --id $VollMedWeb_AppId --api $VollMedWebAPI_AppId --api-permissions "user_impersonation=Scope"

# Conceder consentimento de administrador para as permissões (requer permissões de admin no Azure AD)
az ad app permission grant --id $VollMedWeb_AppId --api $VollMedWebAPI_AppId

# Opcional: Se VollMed.Web precisar ler perfis de usuário do Azure AD
az ad app permission add --id $VollMedWeb_AppId --api "Microsoft Graph" --api-permissions "User.Read=Scope"
az ad app permission grant --id $VollMedWeb_AppId --api "Microsoft Graph"
```

### 4\. Configurar Application Settings nos App Services (Credenciais e URLs)

Para que suas aplicações no Azure saibam como se conectar ao Azure AD e entre si, você precisará configurar as variáveis de ambiente (Application Settings) nos App Services.

```bash
# Substitua pelos nomes dos seus App Services e IDs de cliente/tenant
WEB_APP_NAME="vollmed-web-app"
WEBAPI_APP_NAME="vollmed-webapi-app"
IDENTITY_APP_NAME="vollmed-identity-app"
RESOURCE_GROUP="vollmed-rg"

VollMedWeb_AppId="aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
VollMedWebAPI_AppId="ffffffff-gggg-hhhh-iiii-jjjjjjjjjjjj"
YOUR_TENANT_ID=$(az account show --query tenantId --output tsv)

# Configurar VollMed.Web (se for autenticar diretamente com Azure AD)
# Caso contrário, essa configuração fica no VollMed.Identity
az webapp config appsettings set --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME --settings \
    "Authentication:AzureAd:ClientId=$VollMedWeb_AppId" \
    "Authentication:AzureAd:TenantId=$YOUR_TENANT_ID" \
    "Authentication:AzureAd:Instance=https://login.microsoftonline.com/" \
    "Authentication:AzureAd:CallbackPath=/signin-oidc" \
    "Authentication:AzureAd:Audience=$VollMedWebAPI_AppId" # Se for chamar a WebAPI com token do AAD

# Configurar VollMed.WebAPI (para validar tokens do Azure AD)
az webapp config appsettings set --resource-group $RESOURCE_GROUP --name $WEBAPI_APP_NAME --settings \
    "Authentication:JwtBearer:Authority=https://sts.windows.net/$YOUR_TENANT_ID/" \
    "Authentication:JwtBearer:Audience=api://vollmed-webapi" # ou o AppId da WebAPI

# Configurar VollMed.Identity (para usar Azure AD como provedor externo para Duende IdentityServer)
# Substitua "SUA_CHAVE_SECRETA_AZUREAD" pelo segredo do cliente gerado no Azure AD para VollMed.Identity (se for a abordagem)
az webapp config appsettings set --resource-group $RESOURCE_GROUP --name $IDENTITY_APP_NAME --settings \
    "Authentication:AzureAD:ClientId=CLIENT_ID_AZURE_AD_IDENTITY_APP" \
    "Authentication:AzureAD:ClientSecret=SUA_CHAVE_SECRETA_AZUREAD" \
    "Authentication:AzureAD:TenantId=$YOUR_TENANT_ID" \
    "Authentication:AzureAD:CallbackPath=/signin-azuread"
```

### 5\. Gerenciar Roles e Policies (Código ASP.NET Core)

A criação de **roles e policies** é primariamente feita no **código-fonte ASP.NET Core** e não diretamente via Azure CLI. Os comandos CLI se concentram na configuração da infraestrutura de identidade no Azure AD.

  * **Roles:** Você as definirá e atribuirá a usuários (diretamente no `VollMed.Identity` se forem locais, ou via grupos do Azure AD que são mapeados para roles ou claims no código).
  * **Policies:** No `Program.cs` e nos controladores do `VollMed.Web` e `VollMed.WebAPI`, você implementará `[Authorize(Roles = "Admin")]` ou `[Authorize(Policy = "CanEditMedicalRecord")]`.

### Próximos Passos:

Após executar esses comandos, você precisará:

1.  **Atualizar o código** dos seus projetos `.NET` para refletir as novas configurações de autenticação (e.g., adicionar `AddOpenIdConnect` para Azure AD no `VollMed.Web` ou integrar o provedor Azure AD no `VollMed.Identity`).
2.  **Implantar as aplicações atualizadas** nos seus App Services.
3.  **Testar** os fluxos de login e autorização com diferentes usuários e roles.

Esses comandos fornecem a base para uma demonstração sólida de Autenticação e Autorização com o Azure AD na solução VollMed.