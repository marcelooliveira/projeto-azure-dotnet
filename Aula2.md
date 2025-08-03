# Aula2 - Autentica��o e Autoriza��o 

-----

Assumindo que os App Services para `VollMed.Web`, `VollMed.WebAPI` e `VollMed.Identity` j� est�o provisionados no grupo de recursos `vollmed-rg`, os principais passos com Azure CLI seriam para registrar aplica��es no Azure AD e configurar a Managed Identity.

-----

## Comandos Azure CLI para Autentica��o e Autoriza��o com Azure AD

Os comandos a seguir focar�o nas etapas de configura��o do Azure Active Directory (Azure AD), que s�o o cerne da aula de Autentica��o e Autoriza��o.

### 1\. Registro da Aplica��o `VollMed.Web` (Cliente OIDC no Azure AD)

Mesmo que `VollMed.Web` se autentique primariamente com `VollMed.Identity`, para cen�rios mais avan�ados ou para simplificar a federa��o de identidades, podemos registrar `VollMed.Web` diretamente no Azure AD como uma aplica��o cliente. Isso � �til se voc� quiser que o **Azure AD seja o provedor de identidade direto para `VollMed.Web`**.

```bash
# Entrar na sua conta do Azure CLI (se ainda n�o estiver logado)
az login

# Definir a assinatura (substitua pelo ID da sua assinatura)
az account set --subscription "SUA_SUBSCRIPTION_ID"

# 1. Registrar a aplica��o cliente (VollMed.Web) no Azure AD
#    � a aplica��o que inicia o fluxo de login
az ad app create \
    --display-name "VollMed.Web" \
    --sign-in-audience "AzureADMyOrg" \
    --web-redirect-uris "https://localhost:5002/signin-oidc" "https://vollmed-web-app.azurewebsites.net/signin-oidc" \
    --identifier-uris "api://vollmed-web" \
    --query appId --output tsv

# Anote o appId retornado. Ele ser� o Client ID para VollMed.Web.
# Exemplo de sa�da: "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
```

### 2\. Registro da Aplica��o `VollMed.WebAPI` (API Protegida no Azure AD)

A `VollMed.WebAPI` precisar� ser registrada no Azure AD para que o Azure AD saiba que ela � um recurso protegido e possa emitir tokens para ela.

```bash
# 2. Registrar a aplica��o API (VollMed.WebAPI) no Azure AD
az ad app create \
    --display-name "VollMed.WebAPI" \
    --identifier-uris "api://vollmed-webapi" \
    --query appId --output tsv

# Anote o appId retornado. Ele ser� o Client ID da API.
# Exemplo de sa�da: "ffffffff-gggg-hhhh-iiii-jjjjjjjjjjjj"
```

#### Expor uma Permiss�o (Scope) para a `VollMed.WebAPI`

Agora, vamos definir as permiss�es que a `VollMed.WebAPI` exp�e, para que outras aplica��es (como o `VollMed.Web` ou o `VollMed.Identity` ao federar) possam solicit�-las. No seu `Program.cs` da WebAPI, voc� tem `policy.RequireClaim("scope", "VollMed.WebAPI")`. Vamos alinhar isso:

```bash
# Substitua pelo Client ID da VollMed.WebAPI que voc� anotou acima
VollMedWebAPI_AppId="ffffffff-gggg-hhhh-iiii-jjjjjjjjjjjj"

# Expor a permiss�o "VollMed.WebAPI"
az ad app permission add --id $VollMedWebAPI_AppId --api $VollMedWebAPI_AppId --api-permissions "user_impersonation=Scope"

# Definir o nome da permiss�o para "VollMed.WebAPI"
# (Este comando � um pouco mais complexo via CLI, geralmente � feito no portal para custom scopes.
# O user_impersonation � um scope padr�o que pode ser renomeado via portal ou manifesto).
# Para simplificar na CLI, usaremos 'user_impersonation' e focaremos em 'VollMed.WebAPI' no c�digo.
# Para configurar um custom scope exato "VollMed.WebAPI", seria necess�rio editar o manifesto da aplica��o:
# 1. az ad app show --id $VollMedWebAPI_AppId --query "web.oauth2Permissions" # Ver as permiss�es existentes
# 2. Copiar o conte�do, adicionar o novo scope "VollMed.WebAPI" e ent�o usar:
# az ad app update --id $VollMedWebAPI_AppId --manifest <json-string-do-manifesto-atualizado>
# Para fins did�ticos e simplificar a CLI aqui, focaremos em adicionar uma permiss�o e validar o scope no c�digo.
```

### 3\. Conceder Permiss�es (para `VollMed.Web` acessar `VollMed.WebAPI`)

Se o `VollMed.Web` for chamar diretamente a `VollMed.WebAPI` usando tokens do Azure AD (n�o via Identity Server), ele precisar� de permiss�o.

```bash
# Substitua pelo Client ID da VollMed.Web que voc� anotou
VollMedWeb_AppId="aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
# Substitua pelo Client ID da VollMed.WebAPI que voc� anotou
VollMedWebAPI_AppId="ffffffff-gggg-hhhh-iiii-jjjjjjjjjjjj"

# 3. Conceder permiss�es para VollMed.Web acessar VollMed.WebAPI
# Isso permite que VollMed.Web solicite o scope da VollMed.WebAPI
az ad app permission add --id $VollMedWeb_AppId --api $VollMedWebAPI_AppId --api-permissions "user_impersonation=Scope"

# Conceder consentimento de administrador para as permiss�es (requer permiss�es de admin no Azure AD)
az ad app permission grant --id $VollMedWeb_AppId --api $VollMedWebAPI_AppId

# Opcional: Se VollMed.Web precisar ler perfis de usu�rio do Azure AD
az ad app permission add --id $VollMedWeb_AppId --api "Microsoft Graph" --api-permissions "User.Read=Scope"
az ad app permission grant --id $VollMedWeb_AppId --api "Microsoft Graph"
```

### 4\. Configurar Application Settings nos App Services (Credenciais e URLs)

Para que suas aplica��es no Azure saibam como se conectar ao Azure AD e entre si, voc� precisar� configurar as vari�veis de ambiente (Application Settings) nos App Services.

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
# Caso contr�rio, essa configura��o fica no VollMed.Identity
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

### 5\. Gerenciar Roles e Policies (C�digo ASP.NET Core)

A cria��o de **roles e policies** � primariamente feita no **c�digo-fonte ASP.NET Core** e n�o diretamente via Azure CLI. Os comandos CLI se concentram na configura��o da infraestrutura de identidade no Azure AD.

  * **Roles:** Voc� as definir� e atribuir� a usu�rios (diretamente no `VollMed.Identity` se forem locais, ou via grupos do Azure AD que s�o mapeados para roles ou claims no c�digo).
  * **Policies:** No `Program.cs` e nos controladores do `VollMed.Web` e `VollMed.WebAPI`, voc� implementar� `[Authorize(Roles = "Admin")]` ou `[Authorize(Policy = "CanEditMedicalRecord")]`.

### Pr�ximos Passos:

Ap�s executar esses comandos, voc� precisar�:

1.  **Atualizar o c�digo** dos seus projetos `.NET` para refletir as novas configura��es de autentica��o (e.g., adicionar `AddOpenIdConnect` para Azure AD no `VollMed.Web` ou integrar o provedor Azure AD no `VollMed.Identity`).
2.  **Implantar as aplica��es atualizadas** nos seus App Services.
3.  **Testar** os fluxos de login e autoriza��o com diferentes usu�rios e roles.

Esses comandos fornecem a base para uma demonstra��o s�lida de Autentica��o e Autoriza��o com o Azure AD na solu��o VollMed.