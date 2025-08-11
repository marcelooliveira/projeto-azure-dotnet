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

062. /subscriptions/3acc8650-3ea0-42db-b1dd-694439b0aa06/resourceGroups/vollmed-rg/providers/Microsoft.Web/sites/VollMedWebAPI20250810195450

```json
    {
        "id": "/subscriptions/3acc8650-3ea0-42db-b1dd-694439b0aa06/resourceGroups/vollmed-rg/providers/Microsoft.Web/sites/VollMedWebAPI20250810195450",
        "name": "VollMedWebAPI20250810195450",
        "type": "Microsoft.Web/sites",
        "kind": "app,linux",
        "location": "Brazil South",
        "tags": {
            "hidden-related:/subscriptions/3acc8650-3ea0-42db-b1dd-694439b0aa06/resourceGroups/vollmed-rg/providers/Microsoft.Web/serverFarms/VollMedWebAPI20250810195450Plan": "empty"
        },
        "properties": {
            "name": "VollMedWebAPI20250810195450",
            "state": "Running",
            "hostNames": [
                "vollmedwebapi20250810195450.azurewebsites.net"
            ],
            "webSpace": "vollmed-rg-BrazilSouthwebspace-Linux",
            "selfLink": "https://waws-prod-cq1-021.api.azurewebsites.windows.net:454/subscriptions/3acc8650-3ea0-42db-b1dd-694439b0aa06/webspaces/vollmed-rg-BrazilSouthwebspace-Linux/sites/VollMedWebAPI20250810195450",
            "repositorySiteName": "VollMedWebAPI20250810195450",
            "owner": null,
            "usageState": "Normal",
            "enabled": true,
            "adminEnabled": true,
            "siteScopedCertificatesEnabled": false,
            "afdEnabled": false,
            "enabledHostNames": [
                "vollmedwebapi20250810195450.azurewebsites.net",
                "vollmedwebapi20250810195450.scm.azurewebsites.net"
            ],
            "siteProperties": {
                "metadata": null,
                "properties": [
                    {
                        "name": "LinuxFxVersion",
                        "value": "DOTNETCORE|9.0"
                    },
                    {
                        "name": "WindowsFxVersion",
                        "value": null
                    }
                ],
                "appSettings": null
            },
            "availabilityState": "Normal",
            "sslCertificates": null,
            "csrs": [],
            "cers": null,
            "siteMode": null,
            "hostNameSslStates": [
                {
                    "name": "vollmedwebapi20250810195450.azurewebsites.net",
                    "sslState": "Disabled",
                    "ipBasedSslResult": null,
                    "virtualIP": null,
                    "virtualIPv6": null,
                    "thumbprint": null,
                    "certificateResourceId": null,
                    "toUpdate": null,
                    "toUpdateIpBasedSsl": null,
                    "ipBasedSslState": "NotConfigured",
                    "hostType": "Standard"
                },
                {
                    "name": "vollmedwebapi20250810195450.scm.azurewebsites.net",
                    "sslState": "Disabled",
                    "ipBasedSslResult": null,
                    "virtualIP": null,
                    "virtualIPv6": null,
                    "thumbprint": null,
                    "certificateResourceId": null,
                    "toUpdate": null,
                    "toUpdateIpBasedSsl": null,
                    "ipBasedSslState": "NotConfigured",
                    "hostType": "Repository"
                }
            ],
            "computeMode": null,
            "serverFarm": null,
            "serverFarmId": "/subscriptions/3acc8650-3ea0-42db-b1dd-694439b0aa06/resourceGroups/vollmed-rg/providers/Microsoft.Web/serverfarms/VollMedWebAPI20250810195450Plan",
            "reserved": true,
            "isXenon": false,
            "hyperV": false,
            "sandboxType": null,
            "lastModifiedTimeUtc": "2025-08-11T01:08:43.1633333",
            "storageRecoveryDefaultState": "Running",
            "contentAvailabilityState": "Normal",
            "runtimeAvailabilityState": "Normal",
            "dnsConfiguration": {},
            "vnetRouteAllEnabled": false,
            "containerAllocationSubnet": null,
            "useContainerLocalhostBindings": null,
            "vnetImagePullEnabled": false,
            "vnetContentShareEnabled": false,
            "siteConfig": {
                "numberOfWorkers": 1,
                "defaultDocuments": null,
                "netFrameworkVersion": null,
                "phpVersion": null,
                "pythonVersion": null,
                "nodeVersion": null,
                "powerShellVersion": null,
                "linuxFxVersion": "DOTNETCORE|9.0",
                "windowsFxVersion": null,
                "sandboxType": null,
                "windowsConfiguredStacks": null,
                "requestTracingEnabled": null,
                "remoteDebuggingEnabled": null,
                "remoteDebuggingVersion": null,
                "httpLoggingEnabled": null,
                "azureMonitorLogCategories": null,
                "acrUseManagedIdentityCreds": false,
                "acrUserManagedIdentityID": null,
                "logsDirectorySizeLimit": null,
                "detailedErrorLoggingEnabled": null,
                "publishingUsername": null,
                "publishingPassword": null,
                "appSettings": null,
                "metadata": null,
                "connectionStrings": null,
                "machineKey": null,
                "handlerMappings": null,
                "documentRoot": null,
                "scmType": null,
                "use32BitWorkerProcess": null,
                "webSocketsEnabled": null,
                "alwaysOn": false,
                "javaVersion": null,
                "javaContainer": null,
                "javaContainerVersion": null,
                "appCommandLine": null,
                "managedPipelineMode": null,
                "virtualApplications": null,
                "winAuthAdminState": null,
                "winAuthTenantState": null,
                "customAppPoolIdentityAdminState": null,
                "customAppPoolIdentityTenantState": null,
                "runtimeADUser": null,
                "runtimeADUserPassword": null,
                "loadBalancing": null,
                "routingRules": null,
                "experiments": null,
                "limits": null,
                "autoHealEnabled": null,
                "autoHealRules": null,
                "tracingOptions": null,
                "vnetName": null,
                "vnetRouteAllEnabled": null,
                "vnetPrivatePortsCount": null,
                "publicNetworkAccess": null,
                "cors": null,
                "push": null,
                "apiDefinition": null,
                "apiManagementConfig": null,
                "autoSwapSlotName": null,
                "localMySqlEnabled": null,
                "managedServiceIdentityId": null,
                "xManagedServiceIdentityId": null,
                "keyVaultReferenceIdentity": null,
                "ipSecurityRestrictions": null,
                "ipSecurityRestrictionsDefaultAction": null,
                "scmIpSecurityRestrictions": null,
                "scmIpSecurityRestrictionsDefaultAction": null,
                "scmIpSecurityRestrictionsUseMain": null,
                "http20Enabled": false,
                "minTlsVersion": null,
                "minTlsCipherSuite": null,
                "scmMinTlsCipherSuite": null,
                "supportedTlsCipherSuites": null,
                "scmSupportedTlsCipherSuites": null,
                "scmMinTlsVersion": null,
                "ftpsState": null,
                "preWarmedInstanceCount": null,
                "functionAppScaleLimit": 0,
                "elasticWebAppScaleLimit": null,
                "healthCheckPath": null,
                "fileChangeAuditEnabled": null,
                "functionsRuntimeScaleMonitoringEnabled": null,
                "websiteTimeZone": null,
                "minimumElasticInstanceCount": 1,
                "azureStorageAccounts": null,
                "http20ProxyFlag": null,
                "sitePort": null,
                "antivirusScanEnabled": null,
                "storageType": null,
                "sitePrivateLinkHostEnabled": null,
                "clusteringEnabled": false
            },
            "functionAppConfig": null,
            "daprConfig": null,
            "deploymentId": "VollMedWebAPI20250810195450",
            "slotName": null,
            "trafficManagerHostNames": null,
            "sku": "Standard",
            "scmSiteAlsoStopped": false,
            "targetSwapSlot": null,
            "hostingEnvironment": null,
            "hostingEnvironmentProfile": null,
            "clientAffinityEnabled": true,
            "clientAffinityProxyEnabled": false,
            "useQueryStringAffinity": false,
            "blockPathTraversal": false,
            "clientCertEnabled": false,
            "clientCertMode": "Required",
            "clientCertExclusionPaths": null,
            "clientCertExclusionEndPoints": null,
            "hostNamesDisabled": false,
            "ipMode": "IPv4",
            "vnetBackupRestoreEnabled": false,
            "domainVerificationIdentifiers": null,
            "customDomainVerificationId": "F1A45B7C0331C6478918F5A5DAE0079425362171F681E3679E9208367887E6AF",
            "kind": "app,linux",
            "managedEnvironmentId": null,
            "workloadProfileName": null,
            "resourceConfig": null,
            "inboundIpAddress": "191.233.203.33",
            "possibleInboundIpAddresses": "191.233.203.33",
            "inboundIpv6Address": "2603:1050:6:3::5",
            "possibleInboundIpv6Addresses": "2603:1050:6:3::5",
            "ftpUsername": "VollMedWebAPI20250810195450\\$VollMedWebAPI20250810195450",
            "ftpsHostName": "ftps://waws-prod-cq1-021.ftp.azurewebsites.windows.net/site/wwwroot",
            "outboundIpAddresses": "191.232.237.67,191.232.232.81,191.232.193.240,191.232.238.181,191.233.203.33",
            "possibleOutboundIpAddresses": "191.232.237.67,191.232.232.81,191.232.193.240,191.232.238.181,191.232.195.211,191.232.199.56,191.234.189.236,191.232.195.246,191.232.198.180,191.232.67.101,191.232.66.95,191.232.68.93,191.232.69.43,191.232.69.105,191.232.70.57,20.226.185.17,20.226.185.167,20.226.185.169,20.226.185.181,191.232.64.19,20.226.185.189,191.233.203.33",
            "outboundIpv6Addresses": "2603:1050:1:8::48,2603:1050:1:b::51,2603:1050:1:d::3b,2603:1050:1:a::277,2603:1050:6:3::5,2603:10e1:100:2::bfe9:cb21",
            "possibleOutboundIpv6Addresses": "2603:1050:1:8::48,2603:1050:1:b::51,2603:1050:1:d::3b,2603:1050:1:a::277,2603:1050:1:8::4b,2603:1050:1:d::3c,2603:1050:1:d::3d,2603:1050:1:a::27a,2603:1050:1:d::3e,2603:1050:1:d::3f,2603:1050:1:8::4c,2603:1050:1:a::27c,2603:1050:1:a::27d,2603:1050:1:b::56,2603:1050:1:b::57,2603:1050:1:8::4d,2603:1050:1:a::27e,2603:1050:1:a::27f,2603:1050:1:a::280,2603:1050:1:b::58,2603:1050:1:a::281,2603:1050:6:3::5,2603:10e1:100:2::bfe9:cb21",
            "containerSize": 0,
            "dailyMemoryTimeQuota": 0,
            "suspendedTill": null,
            "siteDisabledReason": 0,
            "functionExecutionUnitsCache": null,
            "maxNumberOfWorkers": null,
            "homeStamp": "waws-prod-cq1-021",
            "cloningInfo": null,
            "hostingEnvironmentId": null,
            "tags": {
                "hidden-related:/subscriptions/3acc8650-3ea0-42db-b1dd-694439b0aa06/resourceGroups/vollmed-rg/providers/Microsoft.Web/serverFarms/VollMedWebAPI20250810195450Plan": "empty"
            },
            "resourceGroup": "vollmed-rg",
            "defaultHostName": "vollmedwebapi20250810195450.azurewebsites.net",
            "slotSwapStatus": null,
            "httpsOnly": true,
            "endToEndEncryptionEnabled": false,
            "functionsRuntimeAdminIsolationEnabled": false,
            "redundancyMode": "None",
            "inProgressOperationId": null,
            "geoDistributions": null,
            "privateEndpointConnections": [],
            "publicNetworkAccess": null,
            "buildVersion": null,
            "targetBuildVersion": null,
            "migrationState": null,
            "eligibleLogCategories": "AppServiceAppLogs,AppServiceAuditLogs,AppServiceConsoleLogs,AppServiceHTTPLogs,AppServiceIPSecAuditLogs,AppServicePlatformLogs,ScanLogs,AppServiceAuthenticationLogs",
            "inFlightFeatures": [
                "SiteContainers"
            ],
            "storageAccountRequired": false,
            "virtualNetworkSubnetId": null,
            "keyVaultReferenceIdentity": "SystemAssigned",
            "autoGeneratedDomainNameLabelScope": null,
            "privateLinkIdentifiers": null,
            "sshEnabled": null
        },
        "identity": {
            "type": "SystemAssigned",
            "tenantId": "0ec7eb2f-a6b5-4cd1-9695-3f3ec151ed0c",
            "principalId": "eaa24e67-9720-4148-a2a0-87266e4be68e"
        },
        "apiVersion": "2022-03-01"
    }
```