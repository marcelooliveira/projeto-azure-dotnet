# Aula 4 - Armazenamento e Banco de Dados

4 - Hospedagem de Aplica��es .NET 
	Azure App Service 
	Azure Functions 
	Azure Container Apps / AKS 
	Deploy com GitHub Actions ou Azure DevOps  
-----

-----

Nesta aula, nosso objetivo � migrar os bancos de dados locais da solu��o VollMed (que atualmente usam SQLite) para um **Azure SQL Database** robusto e escal�vel. Utilizaremos um �nico **servidor l�gico** no Azure SQL Database para hospedar **duas bases de dados separadas**: uma para os dados da `VollMed.WebAPI` (m�dicos, consultas) e outra para os dados de identidade do `VollMed.Identity` (usu�rios, roles).

-----

## 1\. Criando o Servidor L�gico do Azure SQL Database

O primeiro passo � provisionar um servidor l�gico do Azure SQL Database. Ele atuar� como um "cont�iner" para nossos bancos de dados e ser� o ponto de acesso principal.

```bash
# Definimos vari�veis para o nosso grupo de recursos, localiza��o e nomes
RESOURCE_GROUP="vollmed-rg"
LOCATION="eastus" # Escolha uma regi�o do Azure pr�xima a voc� ou aos seus App Services
SQL_SERVER_NAME="vollmed-sql-server-01" # Este nome deve ser �nico globalmente no Azure
SQL_ADMIN_USER="vollmedadmin" # Nome de usu�rio administrador para o servidor SQL
SQL_ADMIN_PASSWORD="SuaSenhaForteAqui!123" # SENHA FORTE E SEGURA! Mude para uma senha real.

echo "Iniciando a cria��o do servidor SQL: $SQL_SERVER_NAME no grupo de recursos: $RESOURCE_GROUP..."

# Comando para criar o servidor SQL
az sql server create \
    --name $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --admin-user $SQL_ADMIN_USER \
    --admin-password $SQL_ADMIN_PASSWORD \
    --query fullyQualifiedDomainName # Exibe o nome de dom�nio completo do servidor ap�s a cria��o

echo "Servidor SQL criado com sucesso. Pr�ximo passo: configurar o firewall."
```

-----

## 2\. Configurando as Regras de Firewall do Servidor SQL

Por padr�o, o Azure SQL Database n�o permite acesso de nenhuma rede externa, por seguran�a. Precisamos configurar regras de firewall para permitir que nossos **servi�os do Azure** (como os App Services onde a VollMed ser� hospedada) e, opcionalmente, o **seu IP local** (para desenvolvimento e testes) possam se conectar.

```bash
echo "Configurando regras de firewall para o servidor SQL..."

# Permite que todos os servi�os e recursos do Azure acessem o servidor SQL.
# Essencial para que seus App Services consigam se conectar.
az sql server firewall-rule create \
    --name "AllowAzureServices" \
    --server $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --start-ip-address "0.0.0.0" \
    --end-ip-address "0.0.0.0"

# Opcional: Adiciona uma regra para o seu endere�o IP atual.
# Isso � muito �til para que voc� possa se conectar ao banco de dados a partir da sua m�quina local
# (por exemplo, para rodar as migra��es do EF Core ou usar o SQL Server Management Studio).
# Lembre-se de REMOVER esta regra em um ambiente de produ��o para maior seguran�a.
CURRENT_IP=$(curl -s checkip.amazonaws.com)
echo "Seu IP atual detectado: $CURRENT_IP. Adicionando regra de firewall..."
az sql server firewall-rule create \
    --name "AllowMyIP" \
    --server $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --start-ip-address $CURRENT_IP \
    --end-ip-address $CURRENT_IP

echo "Regras de firewall configuradas. Continue para a cria��o dos bancos de dados."
```

-----

## 3\. Criando os Bancos de Dados Azure SQL

Dentro do servidor l�gico que criamos, agora vamos criar as duas bases de dados separadas: uma para os dados de m�dicos e consultas (`vollmed-db`) e outra para os dados de usu�rios e roles do Identity Server (`vollmed-identity-db`).

```bash
# Definimos os nomes para nossos bancos de dados
SQL_DB_VOLLMED="vollmed-db"
SQL_DB_IDENTITY="vollmed-identity-db"

echo "Criando o banco de dados principal para a VollMed.WebAPI: $SQL_DB_VOLLMED..."

# Cria o banco de dados para os dados da VollMed.WebAPI
# O 'service-objective S0' � um tier b�sico de baixo custo, bom para demonstra��o.
az sql db create \
    --resource-group $RESOURCE_GROUP \
    --server $SQL_SERVER_NAME \
    --name $SQL_DB_VOLLMED \
    --service-objective S0

echo "Criando o banco de dados para os dados de identidade do VollMed.Identity: $SQL_DB_IDENTITY..."

# Cria o banco de dados para os dados de identidade do VollMed.Identity
az sql db create \
    --resource-group $RESOURCE_GROUP \
    --server $SQL_SERVER_NAME \
    --name $SQL_DB_IDENTITY \
    --service-objective S0

echo "Bancos de dados '$SQL_DB_VOLLMED' e '$SQL_DB_IDENTITY' criados com sucesso."
```

-----

## 4\. Obtendo as Strings de Conex�o

Ap�s a cria��o, voc� precisar� das strings de conex�o para que suas aplica��es .NET saibam como se conectar a esses novos bancos de dados no Azure.

```bash
echo "Obtendo as strings de conex�o para os bancos de dados..."

# String de conex�o para o banco de dados da VollMed.WebAPI
echo "String de Conex�o para vollmed-db (WebAPI):"
az sql db show-connection-string \
    --client ado.net \
    --name $SQL_DB_VOLLMED \
    --server $SQL_SERVER_NAME \
    --query connectionString

# String de conex�o para o banco de dados do VollMed.Identity
echo "String de Conex�o para vollmed-identity-db (Identity):"
az sql db show-connection-string \
    --client ado.net \
    --name $SQL_DB_IDENTITY \
    --server $SQL_SERVER_NAME \
    --query connectionString
```

**Importante:** As strings de conex�o retornadas pelo Azure CLI conter�o placeholders como `{your_admin_user}` e `{your_admin_password}`. **Substitua-os** pelo `SQL_ADMIN_USER` e `SQL_ADMIN_PASSWORD` que voc� definiu no in�cio.

-----

## Pr�ximos Passos Essenciais (Fora do Azure CLI)

Com os recursos do Azure provisionados, o trabalho continua nos seus projetos .NET:

1.  **Atualizar Depend�ncias:** Nos projetos `VollMed.WebAPI` e `VollMed.Identity`, remova o pacote NuGet `Microsoft.EntityFrameworkCore.Sqlite` e adicione `Microsoft.EntityFrameworkCore.SqlServer`.
2.  **Ajustar `Program.cs`:**
      * Em `VollMed.WebAPI`, altere a configura��o do `DbContext` para usar `UseSqlServer` e a string de conex�o para apontar para o `vollmed-db` no Azure.
      * Em `VollMed.Identity`, fa�a o mesmo, apontando para o `vollmed-identity-db`.
3.  **Executar Migra��es do EF Core:** A partir do seu ambiente de desenvolvimento, execute os comandos do Entity Framework Core para aplicar as migra��es existentes aos seus novos bancos de dados Azure SQL. Isso criar� as tabelas e o esquema necess�rio:
    ```bash
    dotnet ef database update --project VollMed.WebAPI
    dotnet ef database update --project VollMed.Identity
    ```
4.  **Configurar Application Settings Seguramente:** Finalmente, as strings de conex�o que voc� obteve devem ser configuradas como **Application Settings** nos App Services correspondentes no Azure. Para aprimorar a seguran�a, a Aula 3 cobrir� o uso do **Azure Key Vault** e **Managed Identity** para gerenciar essas credenciais de forma segura, eliminando a necessidade de hardcod�-las ou exp�-las.

Com essas etapas, a camada de dados da sua solu��o VollMed estar� totalmente migrada e operacional no Azure SQL Database.