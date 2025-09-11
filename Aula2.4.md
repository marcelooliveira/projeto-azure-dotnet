# Vídeo 2.4 – Managed  Identity e connection strings seguras

**Contexto**: As aplicações da VollMed, como o portal do paciente e a Web API de agendamento, precisam se conectar com segurança a serviços do Azure, como o banco de dados e o armazenamento de arquivos médicos.

**Problema**: Guardar connection strings em arquivos de configuração ou variáveis de ambiente pode expor credenciais sensíveis, aumentando o risco de vazamento e acesso indevido a dados médicos.

**Solução**: Utilizar o Managed  Identity do Azure para que as aplicações da VollMed se autentiquem automaticamente nos serviços, sem a necessidade de armazenar senhas ou chaves de conexão. Em conjunto, o Azure Key Vault pode ser usado para gerenciar segredos e aplicar boas práticas de segurança.

**Teoria**: Explicação sobre identidade gerenciada (System-assigned e User-assigned), como funciona o RBAC (Role-Based Access Control) para definir permissões, e como o Key Vault centraliza e protege segredos.