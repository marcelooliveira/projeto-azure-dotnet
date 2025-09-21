# Vídeo 2.3 – Azure Table Storage e Cosmos DB para dados NoSQL

**Contexto**: A VollMed precisa lidar com informações que nem sempre seguem um formato rígido. Exemplos incluem registros de acessos dos pacientes no portal, histórico de consultas armazenado em diferentes formatos e dados de telemetria de dispositivos de saúde. Esses cenários exigem um armazenamento flexível e escalável.

**Problema**: Bancos relacionais tradicionais não são ideais para dados semi-estruturados, de grande volume ou que precisam de distribuição global com baixa latência.

**Solução**: Utilizar o Azure Table  Storage para armazenar informações chave-valor simples, como logs de acesso de pacientes, e o Cosmos DB para cenários mais complexos, como dados médicos globais e sincronização em tempo real entre diferentes unidades da clínica.

**Teoria**: Conceitos de NoSQL, estratégias de particionamento, consistência eventual e as diferentes APIs do Cosmos DB (como SQL API e Mongo API), que permitem escolher o modelo mais adequado para cada tipo de dado da VollMed.


## Criando Banco de dados Azure Cosmos DB

1 - Na barra de busca do Portal, procure o recurso **Azure Cosmos DB**, e clique em **Create**.

2 - Escolha a opção **Azure Cosmos DB for NoSQL**.

3 - **Workload Type**: `Learning`

4 - **Grupo de Recursos**: `vollmed-rg`

5 - Em **Account Name**, escreva `vollmed-cosmosdb`.

6 - Em **Availability Zone**, escolha `Disable`.

7 - Em **Localização** escolha `East US 2`.

8 - Em **Capacity Mode**, escolha **Serverless** e clique em **Examinar + Criar**.

9 - Aguarde a criação do Azure Cosmos DB Account

10 - Entre no menu **Data Explorer**.

11 - Clique em **+ Container** para criar um novo Container. Forneça o nome de database `vollmed`, preencha **Database id** como "vollmed" e  **Container id** como `ResumoMensalConsultas` e **Partition id** como `id`.

12 - Agora expanda o nó “items” do container “ResumoMensalConsultas” e clique em **New Item**. Preencha o documento com o seguinte conteúdo JSON:

```json
{
  "id": "1",
  "meses": [
    {
	"medicoNome": "Gregory House",
	"ano": 2025,
	"mes": 5,
	"qtdeConsultas": 3,
	"honorarios": 300
    }
  ]
}
```

13 - Clique em ***Save*** para salvar o item.