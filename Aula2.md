# Aulas 2, 3 e 4

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

012. Modificar controllers VollMed.WebAPI, COMENTANDO atributo [Authorize] 

020. criar grupo de recursos vollmed-rg
030. criar banco de dados Azure Sql Database
    - Banco: VollMedDB
    - Servidor: vollmed20250808
    - SO: linux
    - mínimo de hardware
    - autenticação sql com usuário e senha
    - > Security > Networking
        - Firewall rules
            - Allow certain public internet IP addresses to access your resource
            - Clique "Add your client IPv4 address to access your resource"
            - Confirme o IP que aparece

````json
    {
        "kind": "v12.0",
        "properties": {
            "administratorLogin": "vollmed",
            "version": "12.0",
            "state": "Ready",
            "fullyQualifiedDomainName": "vollmed20250808.database.windows.net",
            "privateEndpointConnections": [],
            "minimalTlsVersion": "1.2",
            "publicNetworkAccess": "Enabled",
            "restrictOutboundNetworkAccess": "Disabled"
        },
        "location": "brazilsouth",
        "tags": {},
        "id": "/subscriptions/3acc8650-3ea0-42db-b1dd-694439b0aa06/resourceGroups/vollmed-rg/providers/Microsoft.Sql/servers/vollmed20250808",
        "name": "vollmed20250808",
        "type": "Microsoft.Sql/servers",
        "apiVersion": "2022-02-01-preview"
    }
```

032. ConnectionString nos arquivos appsettings.Development.json:
    - \VollMed.Web\appsettings.Development.json
        - Build action: Content
        - Copy to output directory: Copy if newer
    - \VollMed.WebAPI\appsettings.Development.json
        - Build action: Content
        - Copy to output directory: Copy if newer
    
040. Rodar WebAPI local, testar o swagger/index.html

050. Publish WebAPI
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

