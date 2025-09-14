# Vídeo 2.1 – Azure SQL Database com EF Core

## Contexto

No projeto inicial, trabalhamos com um banco de dados **SQL Server local**. 

## Problema:

Gerenciar SQL Server local exige infraestrutura, licenciamento e dificulta escalabilidade. Além disso, empresas como a **VollMed** precisam de bancos que sejam escaláveis, disponíveis em qualquer lugar e não dependam de uma infraestrutura física própria.

## Solução


Agora, vamos migrar o banco de dados para a nuvem, utilizando o serviço **Azure SQL Database**. Essa mudança é importante porque, na prática, 

### Criando banco de dados no Azure SQL Database

Abrir portal do Azure: [https://portal.azure.com](https://portal.azure.com)

> **Comentário:** O portal é o ponto de partida para praticamente tudo no Azure. Uma dica é sempre usar a barra de busca para encontrar os recursos rapidamente, já que a quantidade de opções pode ser grande no início.

No Portal Azure, criar um novo recurso: **+ Create a resource > Databases > SQL Database**

Em seguida, preencher os dados do novo banco de dados:

* **Assinatura**: (sua assinatura - azure subscription)

  > É como a "conta de cobrança" onde os recursos ficam vinculados. Pense nisso como a carteira da empresa dentro do Azure.

* **Plano**: SQL Database

* **Grupo de recursos**: `vollmed-rg`

  > Grupos de recursos são como "pastas" onde organizamos os serviços. Isso facilita o gerenciamento, principalmente quando temos ambientes diferentes (dev, teste, produção).

* **Nome do banco de dados**: `VollMedDB`

* **Servidor**: (Clicar em Criar novo)

* **Nome do servidor (único no mundo todo)**: `vollmeddb20250815`

  > Dica: nomes de servidores precisam ser globais, então é comum adicionar números ou datas para garantir que não haja conflito.

* **Localização**: East US ou East US 2

  > Aqui entra uma decisão estratégica: escolher regiões próximas dos usuários finais pode reduzir a latência e melhorar a experiência.

* **Método de autenticação**: Usar autenticação SQL

* **Logon do administrador do servidor**: `vollmed`

* **Senha**: `!v0llmed`

  > Nunca use senhas reais em exemplos públicos. Aqui usamos apenas para fins didáticos. Em produção, é essencial aplicar boas práticas de segurança, como integração com o **Azure Active Directory**.

* **Deseja usar o pool elástico SQL?** Não

* **Ambiente de carga de trabalho**: Desenvolvimento

* **Computação + armazenamento**: Uso Geral - Sem servidor

  > O modo **serverless** é interessante em cenários de desenvolvimento e testes, porque você só paga quando realmente utiliza o banco.

