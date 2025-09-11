# Vídeo 2.3 – Azure Table Storage e Cosmos DB para dados NoSQL

**Contexto**: A VollMed precisa lidar com informações que nem sempre seguem um formato rígido. Exemplos incluem registros de acessos dos pacientes no portal, histórico de consultas armazenado em diferentes formatos e dados de telemetria de dispositivos de saúde. Esses cenários exigem um armazenamento flexível e escalável.

**Problema**: Bancos relacionais tradicionais não são ideais para dados semi-estruturados, de grande volume ou que precisam de distribuição global com baixa latência.

**Solução**: Utilizar o Azure Table  Storage para armazenar informações chave-valor simples, como logs de acesso de pacientes, e o Cosmos DB para cenários mais complexos, como dados médicos globais e sincronização em tempo real entre diferentes unidades da clínica.

**Teoria**: Conceitos de NoSQL, estratégias de particionamento, consistência eventual e as diferentes APIs do Cosmos DB (como SQL API e Mongo API), que permitem escolher o modelo mais adequado para cada tipo de dado da VollMed.