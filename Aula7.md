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

## Vídeo 7.1 - Explicando publicação via Visual Studio
 
**Contexto**
Desenvolvemos a Function no Visual Studio: No Visual Studio, criamos e testamos a Function localmente.

**Problema**
Processo manual e dependente do ambiente local.
Precisamos enviar o código para o Azure:
O Visual Studio oferece publicação direta: Para colocá-la em produção, o desenvolvedor pode usar o assistente de publicação.

**Solução**
Usar o perfil de publicação (.pubxml) para gerar e enviar o pacote: Esse método gera um arquivo .pubxml, que contém as configurações do destino no Azure.

**Teoria**
O .pubxml cria um pacote ZIP e envia ao Azure App Service: O Visual Studio empacota o código em um arquivo .zip e faz o upload direto. É simples, mas depende do ambiente do desenvolvedor e não automatiza o processo.

## Vídeo 7.2 - Publicando Azure Function direto do Visual Studio para o Azure

1. Clique com botão direito do mouse sobre o projeto VollMed.FunctionApp
2. Clique no menu Publish
3. Target: Escolha Azure
4. Specific target: Escolha Azure Function App
5. Functions instance: Clique em **+ Create new**
6. No formulário "Create New":
    6.1. Name: VollMedFunctionApp2025************
    6.2. Resource group: vollmed-rg
    6.3. Plan type: App Service Plan
    6.4. Operation System: Linux
    6.5. Azure Storage: clique "+ Create new"
    6.6. Clique "Next"

7. Deployment type: Publish (generates pubxml file)
8. Clique "Finish"
9. Conclua o publish — isso cria a infraestrutura e configura o app no Azure.

Essa implantação inicial garante que a Function App exista no Azure e possa receber futuros deploys automatizados.

## Vídeo 7.3 — Explicando publicação via GitHub Actions

**Contexto**
Queremos automatizar o deploy da Azure Function
O código está versionado no GitHub. Em vez de publicar manualmente, configuramos um pipeline no GitHub Actions.

**Problema**
A publicação manual é suscetível a erros e não é reprodutível

**Solução**
Criar pipeline de CI/CD com GitHub Actions. CI/CD significa Continuous Integration / Continuous Deployment. Cada alteração no código é aplicada através de um commit. O commit vai subir para o GitHub através de um comando push. E sempre que há push na branch principal, o workflow executa automaticamente.

**Teoria**
O workflow compila o projeto, gera o pacote e faz o deploy no Azure automaticamente. Ele usa o mesmo processo de empacotamento e envio do .zip para o Azure.
Isso garante consistência, rastreabilidade e integração contínua.
O resultado final é o mesmo: a Function atualizada e rodando no Azure.

## Vídeo 7.4 - Criando um workflow do GitHub Actions para publicar Azure Function App

Para publicar Azure Function via GitHub Actions

1. Abra o portal do azure e navegue até a Function App criada anteriormente: VollMedFunctionApp2025************
2. Clique no menu Centro de Implantação (Deployment Center) e preencha as informações conforme abaixo:
    - Source: GitHub
    - Organização
    - Repositório
    - Branch
    - Opção de fluxo de trabalho: Adicionar workflow
    - Tipo de autenticação: identidade atribuída pelo usuário
    - Salvar
3. Abrir repositório GitHub no navegador
4. Abra os commits da branch Main
5. Note o arquivo de publicação do GitHub actions: VollMedFunctionApp202****************.yml
6. Clicar na aba Actions
7. Aguarde a execução da action.

## Vídeo 7.5 - Adicionando nova função HTTP na Azure Function App

1. Baixar o workflow criado pelo Portal do Azure na branch main no vídeo anterior:
```bash
git fetch
git pull --all
```
2. Clique em Publicar para verificar se o workflow foi gerado automaticamente pelo portal do Azure.

Agora crie e suba para o GitHub uma nova função HTTP chamada GerarProntuarioMedico.

3. No Visual Studio, clique com botão direito sobre projeto Azure Function App.
4. Adicione uma nova função HTTP chamada GerarProntuarioMedico.
5. Add New Azure Function:
    13.1. HTTP trigger
    13.2. Authorization level: Anonymous

6. Publique as alterações no GitHub:
```bash
git add .
git commit -m "Nova função GerarProntuarioMedico"
git push
```
7. Abrir repositório GitHub no navegador
8. Abra a aba Actions
9. Aguarde a execução da action.

## Vídeo 7.6 - Explicando Arquivo de Workflow do GitHub Actions

1. Abrir pasta \.github\workflows
2. Abrir arquivo de workflow

-----------

O que é este arquivo?
Este é um arquivo de workflow do GitHub Actions. Ele automatiza o processo de build e deploy de uma Azure Function App .NET sempre que há um push para o branch.
---

# Explicação do Workflow do GitHub Actions

## Nome e Gatilho
```yaml
name: Build and deploy dotnet core project to Azure Function App - VollMedFunctionApp20251025111641

on:
  push:
    branches:
      - main
  workflow_dispatch:
```
- **O que faz**: Define o nome do workflow e quando ele será executado
- **Quando roda**: Quando há um push na branch main ou manualmente através do GitHub

## Variáveis de Ambiente
```yaml
env:
  AZURE_FUNCTIONAPP_PACKAGE_PATH: '.'
  DOTNET_VERSION: '9.0.x'
```
- **O que faz**: Define variáveis de ambiente que serão usadas em todo o workflow
- **Configurações**:
  - Caminho do projeto: raiz do repositório
  - Versão do .NET: 9.0.x

## Configuração do Job
```yaml
jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    permissions:
      id-token: write
      contents: read
```
- **O que faz**: Define onde o workflow vai rodar e suas permissões
- **Onde roda**: Ubuntu mais recente
- **Permissões**: Permite escrever tokens e ler conteúdo

## Passos do Workflow

### 1. Checkout do Código
```yaml
- name: 'Checkout GitHub Action'
  uses: actions/checkout@v4
```
- **O que faz**: Baixa o código do repositório para o runner

### 2. Configuração do .NET
```yaml
- name: Setup DotNet ${{ env.DOTNET_VERSION }} Environment
  uses: actions/setup-dotnet@v1
```
- **O que faz**: Instala o .NET na versão especificada

### 3. Build do Projeto
```yaml
- name: 'Resolve Project Dependencies Using Dotnet'
  shell: bash
  run: |
    pushd './${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}'
    dotnet build --configuration Release --output ./output
    popd
```
- **O que faz**: Compila o projeto em modo Release

### 4. Login no Azure
```yaml
- name: Login to Azure
  uses: azure/login@v2
  with:
    client-id: ${{ secrets.AZUREAPPSERVICE_CLIENTID_FEBA8B95D65C4DBC9923EDCD20EFDE2B }}
    tenant-id: ${{ secrets.AZUREAPPSERVICE_TENANTID_31E18847E6E4497080BCE191803499BE }}
    subscription-id: ${{ secrets.AZUREAPPSERVICE_SUBSCRIPTIONID_698A82C58D634E2E96A1365B064A3248 }}
```
- **O que faz**: Faz login no Azure usando as credenciais do Service Principal

### 5. Deploy da Function
```yaml
- name: 'Run Azure Functions Action'
  uses: Azure/functions-action@v1
  id: fa
  with:
    app-name: 'VollMedFunctionApp20251025111641'
    slot-name: 'Production'
    package: '${{ env.AZURE_FUNCTIONAPP_PACKAGE_PATH }}/output'
```
- **O que faz**: Faz o deploy do código compilado para a Azure Function
- **Destino**: Slot de produção da Function App

## Resumo
Este workflow automatiza:
1. Compilação do código
2. Autenticação no Azure
3. Deploy na Azure Function
4. Tudo isso acontece automaticamente a cada push na main
