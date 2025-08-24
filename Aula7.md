| Aula| SQL | API / MVC | auth | Monitoring |    Synch       | Infra     | Caching     |
| ---| --- | --- | --- | --- |    ---       | ---     | ---     |
| 1 | local | local |     -    |      -     |    s�ncrono    | az portal |     -       |
| 2 | CLOUD | local |     -    |      -     |    s�ncrono    | az portal |     -       |
| 3 | CLOUD | CLOUD |     -    |      -     |    s�ncrono    | az portal |     -       |
| 4 | CLOUD | CLOUD | MS ENTRA |      -     |    s�ncrono    | az portal |     -       |
| 5 | CLOUD | CLOUD | MS ENTRA | AZ MONITOR |    s�ncrono    | az portal |     -       |
| 6 | CLOUD | CLOUD | MS ENTRA | AZ MONITOR | MSG/ASS�NCRONO | az portal |     -       |
| 7 | CLOUD | CLOUD | MS ENTRA | AZ MONITOR | MSG/ASS�NCRONO | IAAC      |     -       |
| 8 | CLOUD | CLOUD | MS ENTRA | AZ MONITOR | MSG/ASS�NCRONO | IAAC      | AZURE REDIS |


# Aula 7 - DevOps e Infraestrutura como C�digo 

O que � este arquivo?
Este � um arquivo de workflow do GitHub Actions. Ele automatiza o processo de build e deploy de uma Azure Function App .NET sempre que h� um push para o branch Aula7.
---
Disparando o Workflow

```yml
on:
  push:
    branches:
    - Aula7
```

Explica��o:
O workflow � iniciado automaticamente toda vez que algu�m faz um push de c�digo para o branch Aula7 do reposit�rio.
---
Vari�veis de Ambiente

```yml
env:
  AZURE_FUNCTIONAPP_NAME: VollMedFunctionApp20250824122130
  AZURE_FUNCTIONAPP_PACKAGE_PATH: VollMed.FunctionApp/published
  CONFIGURATION: Release
  DOTNET_CORE_VERSION: 9.0.x
  WORKING_DIRECTORY: VollMed.FunctionApp
  DOTNET_CORE_VERSION_INPROC: 6.0.x
```

Explica��o:
Essas vari�veis definem configura��es importantes, como o nome da Function App no Azure, o caminho onde os arquivos publicados ser�o armazenados, a configura��o de build (Release) e as vers�es do .NET utilizadas.
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

Explica��o:
O job de build roda em um servidor Ubuntu fornecido pelo GitHub. O primeiro passo faz o checkout do seu c�digo para que o workflow possa acess�-lo.
---
Configurar o SDK do .NET

```yml
    - name: Setup .NET SDK
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_CORE_VERSION }}
```

Explica��o:
Este passo instala o SDK do .NET 9, necess�rio para compilar o projeto.
---
Restaurar Depend�ncias

```yml
    - name: Restore
      run: dotnet restore "${{ env.WORKING_DIRECTORY }}"

```

Explica��o:
Restaura todos os pacotes NuGet (bibliotecas externas) que o projeto precisa.
---
Compilar o Projeto

```yml
    - name: Build
      run: dotnet build "${{ env.WORKING_DIRECTORY }}" --configuration ${{ env.CONFIGURATION }} --no-restore
```

Explica��o:
Compila o c�digo na pasta especificada usando a configura��o Release.
---
Publicar o Projeto

```yml
    - name: Publish
      run: dotnet publish "${{ env.WORKING_DIRECTORY }}" --configuration ${{ env.CONFIGURATION }} --no-build --output "${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}"

```

Explica��o:
Prepara o c�digo compilado para o deploy, publicando os arquivos em uma pasta.
---
Salvar os Artefatos do Build

```yml
    - name: Publish Artifacts
      uses: actions/upload-artifact@v4
      with:
        name: functionapp
        path: ${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}
```

Explica��o:
Salva os arquivos publicados para serem usados no pr�ximo job (deploy).
---
2. Job de Deploy

```yml
  deploy:
    runs-on: ubuntu-latest
    needs: build
```

Explica��o:
O job de deploy tamb�m roda em Ubuntu e s� come�a ap�s o job de build terminar.
---
Baixar os Artefatos do Build

```yml
    - name: Download artifact from build job
      uses: actions/download-artifact@v4
      with:
        name: functionapp
        path: ${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}

```

Explica��o:
Recupera os arquivos publicados do job de build.
---
Login no Azure


```yml
    - name: Azure Login
      uses: azure/login@v2
      with:
        creds: ${{ secrets.VollMedFunctionApp20250824122130_SPN }}

```

Explica��o:
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

Explica��o:
Envia e publica os arquivos para a Function App especificada no Azure.
---
Resumo
�	Job de build: Faz checkout do c�digo, configura o .NET, restaura depend�ncias, compila, publica e salva os arquivos.
�	Job de deploy: Baixa os arquivos publicados, faz login no Azure e realiza o deploy da aplica��o.

Este workflow automatiza o processo de build e deploy da sua Azure Function App, evitando que voc� precise fazer tudo manualmente a cada atualiza��o de c�digo.
