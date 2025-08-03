# Aula 4 - Armazenamento e Banco de Dados

4 - Hospedagem de Aplicações .NET 
	Azure App Service 
	Azure Functions 
	Azure Container Apps / AKS 
	Deploy com GitHub Actions ou Azure DevOps  
-----

-----

Nesta aula, nosso objetivo é migrar os bancos de dados locais da solução VollMed (que atualmente usam SQLite) para um **Azure SQL Database** robusto e escalável. Utilizaremos um único **servidor lógico** no Azure SQL Database para hospedar **duas bases de dados separadas**: uma para os dados da `VollMed.WebAPI` (médicos, consultas) e outra para os dados de identidade do `VollMed.Identity` (usuários, roles).

-----

## 1\. Criando o Servidor Lógico do Azure SQL Database

O primeiro passo é provisionar um servidor lógico do Azure SQL Database. Ele atuará como um "contêiner" para nossos bancos de dados e será o ponto de acesso principal.

```bash
# Definimos variáveis para o nosso grupo de recursos, localização e nomes
RESOURCE_GROUP="vollmed-rg"
LOCATION="eastus" # Escolha uma região do Azure próxima a você ou aos seus App Services
SQL_SERVER_NAME="vollmed-sql-server-01" # Este nome deve ser único globalmente no Azure
SQL_ADMIN_USER="vollmedadmin" # Nome de usuário administrador para o servidor SQL
SQL_ADMIN_PASSWORD="SuaSenhaForteAqui!123" # SENHA FORTE E SEGURA! Mude para uma senha real.

echo "Iniciando a criação do servidor SQL: $SQL_SERVER_NAME no grupo de recursos: $RESOURCE_GROUP..."

# Comando para criar o servidor SQL
az sql server create \
    --name $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --location $LOCATION \
    --admin-user $SQL_ADMIN_USER \
    --admin-password $SQL_ADMIN_PASSWORD \
    --query fullyQualifiedDomainName # Exibe o nome de domínio completo do servidor após a criação

echo "Servidor SQL criado com sucesso. Próximo passo: configurar o firewall."
```

-----

## 2\. Configurando as Regras de Firewall do Servidor SQL

Por padrão, o Azure SQL Database não permite acesso de nenhuma rede externa, por segurança. Precisamos configurar regras de firewall para permitir que nossos **serviços do Azure** (como os App Services onde a VollMed será hospedada) e, opcionalmente, o **seu IP local** (para desenvolvimento e testes) possam se conectar.

```bash
echo "Configurando regras de firewall para o servidor SQL..."

# Permite que todos os serviços e recursos do Azure acessem o servidor SQL.
# Essencial para que seus App Services consigam se conectar.
az sql server firewall-rule create \
    --name "AllowAzureServices" \
    --server $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --start-ip-address "0.0.0.0" \
    --end-ip-address "0.0.0.0"

# Opcional: Adiciona uma regra para o seu endereço IP atual.
# Isso é muito útil para que você possa se conectar ao banco de dados a partir da sua máquina local
# (por exemplo, para rodar as migrações do EF Core ou usar o SQL Server Management Studio).
# Lembre-se de REMOVER esta regra em um ambiente de produção para maior segurança.
CURRENT_IP=$(curl -s checkip.amazonaws.com)
echo "Seu IP atual detectado: $CURRENT_IP. Adicionando regra de firewall..."
az sql server firewall-rule create \
    --name "AllowMyIP" \
    --server $SQL_SERVER_NAME \
    --resource-group $RESOURCE_GROUP \
    --start-ip-address $CURRENT_IP \
    --end-ip-address $CURRENT_IP

echo "Regras de firewall configuradas. Continue para a criação dos bancos de dados."
```

-----

## 3\. Criando os Bancos de Dados Azure SQL

Dentro do servidor lógico que criamos, agora vamos criar as duas bases de dados separadas: uma para os dados de médicos e consultas (`vollmed-db`) e outra para os dados de usuários e roles do Identity Server (`vollmed-identity-db`).

```bash
# Definimos os nomes para nossos bancos de dados
SQL_DB_VOLLMED="vollmed-db"
SQL_DB_IDENTITY="vollmed-identity-db"

echo "Criando o banco de dados principal para a VollMed.WebAPI: $SQL_DB_VOLLMED..."

# Cria o banco de dados para os dados da VollMed.WebAPI
# O 'service-objective S0' é um tier básico de baixo custo, bom para demonstração.
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

## 4\. Obtendo as Strings de Conexão

Após a criação, você precisará das strings de conexão para que suas aplicações .NET saibam como se conectar a esses novos bancos de dados no Azure.

```bash
echo "Obtendo as strings de conexão para os bancos de dados..."

# String de conexão para o banco de dados da VollMed.WebAPI
echo "String de Conexão para vollmed-db (WebAPI):"
az sql db show-connection-string \
    --client ado.net \
    --name $SQL_DB_VOLLMED \
    --server $SQL_SERVER_NAME \
    --query connectionString

# String de conexão para o banco de dados do VollMed.Identity
echo "String de Conexão para vollmed-identity-db (Identity):"
az sql db show-connection-string \
    --client ado.net \
    --name $SQL_DB_IDENTITY \
    --server $SQL_SERVER_NAME \
    --query connectionString
```

**Importante:** As strings de conexão retornadas pelo Azure CLI conterão placeholders como `{your_admin_user}` e `{your_admin_password}`. **Substitua-os** pelo `SQL_ADMIN_USER` e `SQL_ADMIN_PASSWORD` que você definiu no início.

-----

## Próximos Passos Essenciais (Fora do Azure CLI)

Com os recursos do Azure provisionados, o trabalho continua nos seus projetos .NET:

1.  **Atualizar Dependências:** Nos projetos `VollMed.WebAPI` e `VollMed.Identity`, remova o pacote NuGet `Microsoft.EntityFrameworkCore.Sqlite` e adicione `Microsoft.EntityFrameworkCore.SqlServer`.
2.  **Ajustar `Program.cs`:**
      * Em `VollMed.WebAPI`, altere a configuração do `DbContext` para usar `UseSqlServer` e a string de conexão para apontar para o `vollmed-db` no Azure.
      * Em `VollMed.Identity`, faça o mesmo, apontando para o `vollmed-identity-db`.
3.  **Executar Migrações do EF Core:** A partir do seu ambiente de desenvolvimento, execute os comandos do Entity Framework Core para aplicar as migrações existentes aos seus novos bancos de dados Azure SQL. Isso criará as tabelas e o esquema necessário:
    ```bash
    dotnet ef database update --project VollMed.WebAPI
    dotnet ef database update --project VollMed.Identity
    ```
4.  **Configurar Application Settings Seguramente:** Finalmente, as strings de conexão que você obteve devem ser configuradas como **Application Settings** nos App Services correspondentes no Azure. Para aprimorar a segurança, a Aula 3 cobrirá o uso do **Azure Key Vault** e **Managed Identity** para gerenciar essas credenciais de forma segura, eliminando a necessidade de hardcodá-las ou expô-las.

Com essas etapas, a camada de dados da sua solução VollMed estará totalmente migrada e operacional no Azure SQL Database.