# Aula 3 - Armazenamento e Banco de Dados

3 - Armazenamento e Banco de Dados 
	Azure SQL Database com EF Core
	Azure Blob Storage 
	Azure Table Storage e Cosmos DB 
	Managed Identity e connection strings seguras 

-----

Migrar os bancos de dados SQLite da solu��o VollMed para um �nico **Azure SQL Database** � um passo crucial para ter uma solu��o robusta e escal�vel na nuvem. Voc� pode consolidar ambos os bancos (para dados da VollMed.WebAPI e AspIdUsers.db do VollMed.Identity) no mesmo servidor Azure SQL Database, mas em bases de dados separadas ou at� mesmo dentro da mesma base de dados com esquemas diferentes (embora bases de dados separadas sejam geralmente mais claras para fins did�ticos e de gerenciamento).

Para esta demonstra��o, vamos criar um �nico **servidor l�gico** do Azure SQL Database e, em seguida, **dois bancos de dados separados** dentro dele, um para os dados da `VollMed.WebAPI` (por exemplo, `vollmed-db`) e outro para os dados do `VollMed.Identity` (por exemplo, `vollmed-identity-db`).

## Comandos Azure CLI para Migra��o de Banco de Dados SQLite para Azure SQL Database

Assumindo que voc� j� tem o grupo de recursos `vollmed-rg`, os passos com o Azure CLI seriam os seguintes:

-----

### 1\. Criar um Servidor L�gico do Azure SQL Database

Primeiro, voc� precisar� de um servidor l�gico do Azure SQL Database na sua regi�o preferida. Este servidor hospedar� seus bancos de dados.

```bash
# Definir vari�veis para organiza��o
RESOURCE_GROUP="vollmed-rg"
LOCATION="eastus" # Escolha uma regi�o do Azure pr�xima a voc� ou aos seus App Services
SQL_SERVER_NAME="vollmed-sql-server-01" # Nome �nico para o servidor SQL no Azure
SQL_ADMIN_USER="vollmedadmin" # Nome de usu�rio administrador
SQL_ADMIN_PASSWORD="SuaSenhaForteAqui!123" # Senha forte para o administrador. Mude para algo seguro!

echo "Criando o servidor SQL: $SQL_SERVER_NAME no grupo de recursos: $RESOURCE_GROUP..."
az sql server create \
    --name $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --admin-user $SQL_ADMIN_USER \
    --admin-password $SQL_ADMIN_PASSWORD \
    --query fullyQualifiedDomainName # Exibe o FQDN do servidor SQL

echo "Servidor SQL criado. Configurando o firewall..."
```

-----

### 2\. Configurar Regras de Firewall para o Servidor SQL

Voc� precisa permitir que servi�os Azure e, se necess�rio, seu endere�o IP local acessem o servidor SQL.

```bash
# Permitir que servi�os e recursos do Azure acessem o servidor SQL
az sql server firewall-rule create \
    --name "AllowAzureServices" \
    --server $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --start-ip-address "0.0.0.0" \
    --end-ip-address "0.0.0.0"

# Opcional: Adicionar uma regra para o seu endere�o IP atual (se for acessar localmente)
# Isso � �til para testes ou para executar migra��es EF Core do seu ambiente de desenvolvimento.
CURRENT_IP=$(curl -s checkip.amazonaws.com)
az sql server firewall-rule create \
    --name "AllowMyIP" \
    --server $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --start-ip-address $CURRENT_IP \
    --end-ip-address $CURRENT_IP

echo "Regras de firewall configuradas. Lembre-se de remover a regra 'AllowMyIP' em produ��o."
```

-----

### 3\. Criar os Bancos de Dados Azure SQL

Agora, crie os dois bancos de dados separados dentro do servidor l�gico que acabamos de criar.

```bash
# Banco de dados para VollMed.WebAPI
SQL_DB_VOLLMED="vollmed-db"
az sql db create \
    --resource-group $RESOURCE_GROUP \
    --server $SQL_SERVER_NAME \
    --name $SQL_DB_VOLLMED \
    --service-objective S0 # S0 � um tier de menor custo para demonstra��o. Escolha o adequado para produ��o.

# Banco de dados para VollMed.Identity
SQL_DB_IDENTITY="vollmed-identity-db"
az sql db create \
    --resource-group $RESOURCE_GROUP \
    --server $SQL_SERVER_NAME \
    --name $SQL_DB_IDENTITY \
    --service-objective S0 # S0 � um tier de menor custo para demonstra��o.

echo "Bancos de dados '$SQL_DB_VOLLMED' e '$SQL_DB_IDENTITY' criados com sucesso."
```

-----

### 4\. Obter as Strings de Conex�o (Para Configura��o na Aplica��o)

Voc� precisar� das strings de conex�o para configurar suas aplica��es ASP.NET Core. Lembre-se que, para seguran�a, estas devem ser armazenadas no **Azure Key Vault** e referenciadas via **Managed Identity** pelos App Services (como discutido na aula de Armazenamento e Banco de Dados).

```bash
# Para vollmed-db (VollMed.WebAPI)
az sql db show-connection-string \
    --client ado.net \
    --name $SQL_DB_VOLLMED \
    --server $SQL_SERVER_NAME \
    --query connectionString

# Para vollmed-identity-db (VollMed.Identity)
az sql db show-connection-string \
    --client ado.net \
    --name $SQL_DB_IDENTITY \
    --server $SQL_SERVER_NAME \
    --query connectionString
```

As strings de conex�o retornadas ter�o placeholders como `{your_admin_user}` e `{your_admin_password}`. Substitua-os pelo `SQL_ADMIN_USER` e `SQL_ADMIN_PASSWORD` que voc� definiu.

-----

### Pr�ximos Passos (Fora do Azure CLI):

Ap�s a cria��o dos recursos no Azure, voc� precisar� fazer as seguintes modifica��es em seus projetos `.NET`:

1.  **Atualizar Pacotes NuGet:** Nos projetos `VollMed.WebAPI` e `VollMed.Identity`, adicione o pacote NuGet `Microsoft.EntityFrameworkCore.SqlServer` e remova o `Microsoft.EntityFrameworkCore.Sqlite`.
2.  **Alterar `Program.cs`:**
      * No `VollMed.WebAPI`, altere `x.UseSqlite(connectionString)` para `x.UseSqlServer(connectionString)` e atualize a string de conex�o para apontar para `vollmed-db`.
      * No `VollMed.Identity`, fa�a o mesmo para `vollmed-identity-db`.
3.  **Executar Migra��es do EF Core:** No seu ambiente de desenvolvimento, execute os comandos do Entity Framework Core para aplicar as migra��es existentes ao novo banco de dados SQL Server no Azure:
    ```bash
    dotnet ef database update --project VollMed.WebAPI
    dotnet ef database update --project VollMed.Identity
    ```
    Isso criar� as tabelas e o esquema.
4.  **Configurar Application Settings:** As strings de conex�o finais (preferencialmente usando Managed Identity) ser�o configuradas nos **Application Settings** dos seus App Services, conforme detalhado na Aula 3.

Com esses comandos e os ajustes nos projetos, seus bancos de dados da solu��o VollMed estar�o migrados para o Azure SQL Database, prontos para serem usados pelas suas aplica��es na nuvem.

