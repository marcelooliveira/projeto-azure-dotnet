# Vídeo 2.2 – Azure Blob Storage para arquivos e mídias

**Contexto**: Aplicações modernas, como a VollMed, precisam armazenar fotos dos médicos associados ao serviço, vídeos de orientação de saúde e até prontuários digitalizados de pacientes. Esse tipo de conteúdo exige um armazenamento escalável e seguro.

**Problema**: Guardar esses arquivos diretamente no banco de dados ou em servidores locais gera custos altos, pouca flexibilidade e dificuldades de desempenho.

**Solução**: Utilizar o Azure Blob  Storage, que oferece armazenamento de objetos binários de forma escalável, com alta disponibilidade e custo ajustado ao uso.

**Teoria**: Estrutura baseada em containers e blobs; diferentes tiers de armazenamento (hot, cool, archive) que permitem equilibrar custo e performance; acesso facilitado via SDK do .NET para integração direta com a aplicação da VollMed.