# Vídeo 2.2 – Azure Blob Storage para arquivos e mídias

## Configurando a aplicação para usar o banco de dados na nuvem

Agora que já temos o **Azure SQL Database** criado, vamos configurar nossa aplicação VollMed para utilizá-lo. Esse é o momento em que saímos do ambiente local e começamos a rodar de fato na **infraestrutura em nuvem**, algo essencial para empresas que querem escalar seus sistemas com segurança.

Vamos executar dois passos iniciais:

* Fazer uma cópia de `\VollMed.Web\appsettings.json` e renomear para `appsettings.Development.json`
* Fazer uma cópia de `\VollMed.WebAPI\appsettings.json` e renomear para `appsettings.Development.json`

> **Comentário:** Essa prática é comum em projetos .NET — manter arquivos de configuração específicos para cada ambiente (dev, staging, prod). Assim, evitamos usar as mesmas credenciais e parâmetros em todos os lugares, o que dá mais segurança e flexibilidade.

No portal do Azure, acesse o recurso do banco de dados **VollMedDB**, clique em **Visão geral** e, abaixo de **Cadeias de conexão**, copie a string de conexão (Connection String) do **ADO.NET (autenticação SQL)**.

> **Comentário:** Atenção aqui: essa connection string contém usuário e senha do banco. Em ambientes de produção, o ideal é **não expor essas credenciais** diretamente no código ou em arquivos. Mais à frente vamos usar **Azure Key Vault** e **Managed Identity** para resolver isso.

Abra e edite o arquivo `appsettings.Development.json` no projeto **VollMed.WebAPI** e adicione a connection string:

```json
"ConnectionStrings": {
  "VollMedDB": "Server=tcp:vollmed*******.database.windows.net,1433;Initial Catalog=VollMedDB;Persist Security Info=False;User ID=vollmed;Password=**********;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
}
```

> **Comentário:** Veja como o Azure já traz boas práticas por padrão, como criptografia (Encrypt=True) e certificados confiáveis (TrustServerCertificate=False). Isso garante que nossa aplicação já esteja mais segura desde o início.

Agora abra o terminal no Visual Studio para criar o esquema do banco de dados e popular as tabelas:

```console
cd VollMed.WebAPI
dotnet ef database update
```

> **Comentário:** Esse comando aplica as **migrations** do Entity Framework. É como transformar as classes C# em tabelas de verdade dentro do Azure SQL. Essa integração é um dos grandes pontos fortes do .NET.

Depois, volte ao **Portal do Azure**, vá para o **Editor de Consultas** e visualize os dados de médicos com a consulta:

```sql
SELECT Nome FROM [dbo].[medicos]
```

Se tudo deu certo, já veremos os médicos cadastrados dentro do **Azure SQL Database**, provando que nossa aplicação se conectou corretamente.

Por fim, rode a solução no Visual Studio com **F5** e teste a comunicação entre a aplicação cliente e o banco de dados na nuvem.

> **Comentário:** Esse é um marco importante: a VollMed agora já está rodando com **dados em nuvem**, preparados para atender usuários em qualquer lugar do mundo. A partir daqui, tudo o que construirmos vai estar pronto para escalar e crescer junto com o negócio.
