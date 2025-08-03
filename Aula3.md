# Aula 3 - Armazenamento e Banco de Dados

3 - Armazenamento e Banco de Dados 
	Azure SQL Database com EF Core
	Azure Blob Storage 
	Azure Table Storage e Cosmos DB 
	Managed Identity e connection strings seguras 

-----

Migrar os bancos de dados SQLite da solução VollMed para um único **Azure SQL Database** é um passo crucial para ter uma solução robusta e escalável na nuvem. Você pode consolidar ambos os bancos (para dados da VollMed.WebAPI e AspIdUsers.db do VollMed.Identity) no mesmo servidor Azure SQL Database, mas em bases de dados separadas ou até mesmo dentro da mesma base de dados com esquemas diferentes (embora bases de dados separadas sejam geralmente mais claras para fins didáticos e de gerenciamento).

Para esta demonstração, vamos criar um único **servidor lógico** do Azure SQL Database e, em seguida, **dois bancos de dados separados** dentro dele, um para os dados da `VollMed.WebAPI` (por exemplo, `vollmed-db`) e outro para os dados do `VollMed.Identity` (por exemplo, `vollmed-identity-db`).

## Comandos Azure CLI para Migração de Banco de Dados SQLite para Azure SQL Database

Assumindo que você já tem o grupo de recursos `vollmed-rg`, os passos com o Azure CLI seriam os seguintes:

-----

### 1\. Criar um Servidor Lógico do Azure SQL Database

Primeiro, você precisará de um servidor lógico do Azure SQL Database na sua região preferida. Este servidor hospedará seus bancos de dados.

```bash
# Definir variáveis para organização
RESOURCE_GROUP="vollmed-rg"
LOCATION="eastus" # Escolha uma região do Azure próxima a você ou aos seus App Services
SQL_SERVER_NAME="vollmed-sql-server-01" # Nome único para o servidor SQL no Azure
SQL_ADMIN_USER="vollmedadmin" # Nome de usuário administrador
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

Você precisa permitir que serviços Azure e, se necessário, seu endereço IP local acessem o servidor SQL.

```bash
# Permitir que serviços e recursos do Azure acessem o servidor SQL
az sql server firewall-rule create \
    --name "AllowAzureServices" \
    --server $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --start-ip-address "0.0.0.0" \
    --end-ip-address "0.0.0.0"

# Opcional: Adicionar uma regra para o seu endereço IP atual (se for acessar localmente)
# Isso é útil para testes ou para executar migrações EF Core do seu ambiente de desenvolvimento.
CURRENT_IP=$(curl -s checkip.amazonaws.com)
az sql server firewall-rule create \
    --name "AllowMyIP" \
    --server $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --start-ip-address $CURRENT_IP \
    --end-ip-address $CURRENT_IP

echo "Regras de firewall configuradas. Lembre-se de remover a regra 'AllowMyIP' em produção."
```

-----

### 3\. Criar os Bancos de Dados Azure SQL

Agora, crie os dois bancos de dados separados dentro do servidor lógico que acabamos de criar.

```bash
# Banco de dados para VollMed.WebAPI
SQL_DB_VOLLMED="vollmed-db"
az sql db create \
    --resource-group $RESOURCE_GROUP \
    --server $SQL_SERVER_NAME \
    --name $SQL_DB_VOLLMED \
    --service-objective S0 # S0 é um tier de menor custo para demonstração. Escolha o adequado para produção.

# Banco de dados para VollMed.Identity
SQL_DB_IDENTITY="vollmed-identity-db"
az sql db create \
    --resource-group $RESOURCE_GROUP \
    --server $SQL_SERVER_NAME \
    --name $SQL_DB_IDENTITY \
    --service-objective S0 # S0 é um tier de menor custo para demonstração.

echo "Bancos de dados '$SQL_DB_VOLLMED' e '$SQL_DB_IDENTITY' criados com sucesso."
```

-----

### 4\. Obter as Strings de Conexão (Para Configuração na Aplicação)

Você precisará das strings de conexão para configurar suas aplicações ASP.NET Core. Lembre-se que, para segurança, estas devem ser armazenadas no **Azure Key Vault** e referenciadas via **Managed Identity** pelos App Services (como discutido na aula de Armazenamento e Banco de Dados).

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

As strings de conexão retornadas terão placeholders como `{your_admin_user}` e `{your_admin_password}`. Substitua-os pelo `SQL_ADMIN_USER` e `SQL_ADMIN_PASSWORD` que você definiu.

-----

### Próximos Passos (Fora do Azure CLI):

Após a criação dos recursos no Azure, você precisará fazer as seguintes modificações em seus projetos `.NET`:

1.  **Atualizar Pacotes NuGet:** Nos projetos `VollMed.WebAPI` e `VollMed.Identity`, adicione o pacote NuGet `Microsoft.EntityFrameworkCore.SqlServer` e remova o `Microsoft.EntityFrameworkCore.Sqlite`.
2.  **Alterar `Program.cs`:**
      * No `VollMed.WebAPI`, altere `x.UseSqlite(connectionString)` para `x.UseSqlServer(connectionString)` e atualize a string de conexão para apontar para `vollmed-db`.
      * No `VollMed.Identity`, faça o mesmo para `vollmed-identity-db`.
3.  **Executar Migrações do EF Core:** No seu ambiente de desenvolvimento, execute os comandos do Entity Framework Core para aplicar as migrações existentes ao novo banco de dados SQL Server no Azure:
    ```bash
    dotnet ef database update --project VollMed.WebAPI
    dotnet ef database update --project VollMed.Identity
    ```
    Isso criará as tabelas e o esquema.
4.  **Configurar Application Settings:** As strings de conexão finais (preferencialmente usando Managed Identity) serão configuradas nos **Application Settings** dos seus App Services, conforme detalhado na Aula 3.

Com esses comandos e os ajustes nos projetos, seus bancos de dados da solução VollMed estarão migrados para o Azure SQL Database, prontos para serem usados pelas suas aplicações na nuvem.

