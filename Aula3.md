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


# Aula 3

050. Criar App Service Plan único para MVC + Web API
    - Create App Service Plan
    - region: Brazil South
    - VollMedAppServicePlan
    - Pricing Plan: Free F1

051. Publish WebAPI
    - App Service Plan único: VollMedAppServicePlan
    - Criar perfil de publicação
    - Modificar .pubxml:
        - <SiteUrlToLaunchAfterPublish>https://vollmedwebapixxxxxxx.azurewebsites.net/Swagger/index.html</SiteUrlToLaunchAfterPublish>
    - Publicar

052. Abrir Portal Azure, localizar  Azure Sql Database "vollmed20250808/VollMedDB"
    - Menu Settings > Connection Strings
    - Copiar connection string abaixo de "ADO.NET (SQL authentication)"

054. Abrir Portal Azure, localizar no App Service o Web App "VollMedWebAPI2025xxxxxxxxx"
    - Menu Settings > Environment variables > Connection Strings
    - Adicionar connection string copiado acima 
        - Name: VollMedDB
        - Value: *************[valor copiado acima]
        - Type: SQL Azure
    - Reiniciar (restart) no Web App "VollMedWebAPI2025xxxxxxxxx"

060. Abrir WebAPI do azure no navegador
    - https://vollmedwebapixxxxxxx.azurewebsites.net/Swagger/index.html

