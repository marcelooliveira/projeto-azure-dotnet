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


# Aula 2

No projeto inicial, trabalhamos com um banco de dados SQL Server local. Agora, vamos migrar o banco de dados para a nuvem, utilizando o serviço Azure SQL Database.

## Criando banco de dados no Azure SQL Database

Abrir portal do Azure: https://portal.azure.com

No Portal Azure, criar um novo recurso: + Create a resource > Databases > SQL Database

Em seguida, preencher os dados do novo banco de dados:

- Assinatura: (sua assinatura - azure subscription)
- Plano: SQL Database
- Clicar em Criar
- Grupo de recursos: vollmed-rg
- Nome do banco de dados: VollMedDB
- Servidor: (Clicar em Criar novo)
- Nome do servidor (deve ser único no mundo todo): vollmeddb20250815
- Localização: US East US (ou US East 2)
- Método de autenticação: Usar autenticação SQL
- Logon do administrador do servidor: vollmed
- Senha: !v0llmed
- Confirmar senha: !v0llmed
- Deseja usar o pool elástico SQL? Não
- Ambiente de carga de trabalho: Desenvolvimento
- Computação + armazenamento: Uso Geral - Sem servidor
- Redundância do armazenamento de backup: Armazenamento de backup com redundância local
- Clicar em Avançar + Redes
- Método de conectividade: Ponto de extremidade público
- Regras de Firewall
    - Permitir que serviços e recursos do Azure acessem este servidor: SIM
    - Adicionar o endereço IP do cliente atual: SIM
- Política de conexão: Padrão
- Avançar: Segurança
    - Manter TODAS as opções padrão
- Avançar: Configurações adicionais
    - Manter TODAS as opções padrão
- Avançar: Rótulos
    - Não adicionar nada
- Avançar: Revisar + criar
- Clicar em Criar

Agora vamos aguardar a implantação do banco de dados.

- Após implantação, clicar no Editor de Consultas no painel esquerdo.
- Clicar em OK abaixo de Autenticação do servidor SQL

Veja que ainda não existe nenhuma tabela no banco de dados. Vamos criá-las depois.

Agora, para testar a conexão, execute uma query simples no Query Editor para obter a média entre números de uma lista separada por vírgulas:

```sql
SELECT AVG(CONVERT(INT, value)) AS Media
FROM STRING_SPLIT('10,20,30,40,50', ',');
```


## Configurando a aplicação para usar o banco de dados na nuvem

Vamos executar os dois passos abaixo para configurar a aplicação para usar o banco de dados na nuvem:

- Fazer uma cópia de \VollMed.Web\appsettings.json, renomear para appsettings.Development.json
- Fazer uma cópia de \VollMed.WebAPI\appsettings.json, renomear para appsettings.Development.json

No portal do Azure, acessar o recurso do banco de dados VollMedDB, clicar em Visão geral e abaixo de "Cadeias de conexão" copiar a string de conexão (Connection String) do ADO.NET (autenticação SQL).

Abrir e editar arquivo appsettings.Development.json no projeto VollMed.WebAPI:

```json
  "ConnectionStrings": {
    "VollMedDB": "Server=tcp:vollmed*******.database.windows.net,1433;Initial Catalog=VollMedDB;Persist Security Info=False;User ID=vollmed;Password=**********;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
```

Agora abra o terminal no Visual Studio para criar esquema banco de dados + popular tabelas

```console
cd VollMed.WebAPI
dotnet ef database update
```
    
Agora volte ao Portal do Azure, vá para o Editor de Consultas e visualize os dados de médicos com a consulta:

```sql
SELECT Nome FROM [dbo].[medicos]
```

Agora rode a solução no Visual Studio com F5 para testar a comunicação entre a aplicação cliente e o banco de dados na nuvem.