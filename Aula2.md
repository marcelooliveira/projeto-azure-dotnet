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

010. Configurar os arquivos appsettings.json:
    - \VollMed.Web\appsettings.json
        - Build action: Content
        - Copy to output directory: Do not copy
    - \VollMed.Web\appsettings.Development.json
        - Build action: Content
        - Copy to output directory: Copy if newer
    - \VollMed.WebAPI\appsettings.json
        - Build action: Content
        - Copy to output directory: Do not copy
    - \VollMed.WebAPI\appsettings.Development.json
        - Build action: Content
        - Copy to output directory: Copy if newer

020. criar grupo de recursos
030. criar banco de dados Azure Sql Database
    - Banco: VollMedDB
    - Servidor: vollmed20250808
    - SO: linux
    - mínimo de hardware
    - autenticação sql com usuário e senha:
        - vollmed
        - !v0llmed
    - Connectivity method:
        - Public Endpoint
    - Networking
        - Firewall rules
            - Allow certain public internet IP addresses to access your resource: YES
            - Add current client IP address: YES

032. ConnectionString nos arquivos appsettings.Development.json:
    - \VollMed.Web\appsettings.Development.json
        - Build action: Content
        - Copy to output directory: Copy if newer
    - \VollMed.WebAPI\appsettings.Development.json
        - Build action: Content
        - Copy to output directory: Copy if newer
    
040. Rodar WebAPI local, testar o swagger/index.html

045. Rodar MVC + WebAPI, testar a aplicação completa

