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


# Aula 7 - DevOps e Infraestrutura como Código 

O que é este arquivo?
Este é um arquivo de workflow do GitHub Actions. Ele automatiza o processo de build e deploy de uma Azure Function App .NET sempre que há um push para o branch Aula7.
---
Disparando o Workflow

```yml
on:
  push:
    branches:
    - Aula7
```

Explicação:
O workflow é iniciado automaticamente toda vez que alguém faz um push de código para o branch Aula7 do repositório.
---
Variáveis de Ambiente

```yml
env:
  AZURE_FUNCTIONAPP_NAME: VollMedFunctionApp20250824122130
  AZURE_FUNCTIONAPP_PACKAGE_PATH: VollMed.FunctionApp/published
  CONFIGURATION: Release
  DOTNET_CORE_VERSION: 9.0.x
  WORKING_DIRECTORY: VollMed.FunctionApp
  DOTNET_CORE_VERSION_INPROC: 6.0.x
```

Explicação:
Essas variáveis definem configurações importantes, como o nome da Function App no Azure, o caminho onde os arquivos publicados serão armazenados, a configuração de build (Release) e as versões do .NET utilizadas.
---
Jobs: Build e Deploy
O workflow possui dois jobs principais: build e deploy.
---
1. Job de Build

```yml
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
```

Explicação:
O job de build roda em um servidor Ubuntu fornecido pelo GitHub. O primeiro passo faz o checkout do seu código para que o workflow possa acessá-lo.
---
Configurar o SDK do .NET

```yml
    - name: Setup .NET SDK
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_CORE_VERSION }}
```

Explicação:
Este passo instala o SDK do .NET 9, necessário para compilar o projeto.
---
Restaurar Dependências

```yml
    - name: Restore
      run: dotnet restore "${{ env.WORKING_DIRECTORY }}"

```

Explicação:
Restaura todos os pacotes NuGet (bibliotecas externas) que o projeto precisa.
---
Compilar o Projeto

```yml
    - name: Build
      run: dotnet build "${{ env.WORKING_DIRECTORY }}" --configuration ${{ env.CONFIGURATION }} --no-restore
```

Explicação:
Compila o código na pasta especificada usando a configuração Release.
---
Publicar o Projeto

```yml
    - name: Publish
      run: dotnet publish "${{ env.WORKING_DIRECTORY }}" --configuration ${{ env.CONFIGURATION }} --no-build --output "${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}"

```

Explicação:
Prepara o código compilado para o deploy, publicando os arquivos em uma pasta.
---
Salvar os Artefatos do Build

```yml
    - name: Publish Artifacts
      uses: actions/upload-artifact@v4
      with:
        name: functionapp
        path: ${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}
```

Explicação:
Salva os arquivos publicados para serem usados no próximo job (deploy).
---
2. Job de Deploy

```yml
  deploy:
    runs-on: ubuntu-latest
    needs: build
```

Explicação:
O job de deploy também roda em Ubuntu e só começa após o job de build terminar.
---
Baixar os Artefatos do Build

```yml
    - name: Download artifact from build job
      uses: actions/download-artifact@v4
      with:
        name: functionapp
        path: ${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}

```

Explicação:
Recupera os arquivos publicados do job de build.
---
Login no Azure


```yml
    - name: Azure Login
      uses: azure/login@v2
      with:
        creds: ${{ secrets.VollMedFunctionApp20250824122130_SPN }}

```

Explicação:
Realiza o login no Azure usando uma credencial segura armazenada nos secrets do GitHub.
---
Deploy na Azure Function App

```yml
    - name: Deploy to Azure Function App
      uses: Azure/functions-action@v1
      with:
        app-name: ${{ env.AZURE_FUNCTIONAPP_NAME }}
        package: ${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}

```

Explicação:
Envia e publica os arquivos para a Function App especificada no Azure.
---
Resumo
•	Job de build: Faz checkout do código, configura o .NET, restaura dependências, compila, publica e salva os arquivos.
•	Job de deploy: Baixa os arquivos publicados, faz login no Azure e realiza o deploy da aplicação.

Este workflow automatiza o processo de build e deploy da sua Azure Function App, evitando que você precise fazer tudo manualmente a cada atualização de código.
