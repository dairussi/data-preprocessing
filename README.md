# Data Preprocessing

Este projeto utiliza Docker para facilitar o setup e execução do ambiente de desenvolvimento.

## Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e em execução
- Git instalado no sistema
- PowerShell (para Windows)

## Como executar o projeto

### 1. Clone o repositório

```bash
git clone https://github.com/dairussi/data-preprocessing.git
```

### 2. Certifique-se de que o Docker Desktop está aberto

Verifique se o Docker Desktop está executando no seu sistema antes de prosseguir.

### 3. Execute o script de inicialização

Na raiz do projeto, execute o seguinte comando:

```powershell
.\start-win.ps1
```

## Observações importantes

- **Docker Desktop**: É essencial que o Docker Desktop esteja aberto e funcionando antes de executar o script
- **Localização**: O comando deve ser executado obrigatoriamente na raiz do projeto
- **Sistema operacional**: O script `start-win.ps1` é específico para Windows

## Solução de problemas

Se encontrar problemas durante a execução:

1. Verifique se o Docker Desktop está funcionando corretamente
2. Confirme se você está na raiz do projeto
3. Execute o PowerShell como administrador se necessário

## API de Scripts - Guia Rápido

### 1. Salvar Script

**POST** `/preprocessing-script-store`

Enviar:

- `name`: nome do script
- `scriptContent`: código do script

Retorna: objeto criado com ID

### 2. Ver Scripts Salvos

**GET** `/preprocessing-script-store`

Retorna: lista de todos os scripts

### 3. Executar Script

**POST** `/process-data`

Enviar:

- `scriptName`: nome do script
- `input`: dados para processar

Retorna: retorna a primeira versão com status

### 4. Ver Resultado

**GET** `/process-data`

Enviar: ProcessId

Retorna:

- Todas as versões do objeto com seus referidos status

### Passo a Passo

1. Salve um script (endpoint 1)
2. Execute o script com seus dados (endpoint 3)
3. Use o ID retornado para ver o resultado (endpoint 4)

## Questionário

1. Como você faria para lidar com grandes volumes de dados enviados para pré-processamento? O design atual da API é suficiente?

   - Implementaria filas para lidar com volume de dados de forma mais organizada e robusta.

2. Que medidas você implementaria para se certificar que a aplicação não execute scripts maliciosos?

   - Está implementado um salva guarda de scripts maliciosos na service em ProcessDataAsync, porém ele pode evoluir.
     Dependendo da necessidade poderíamos criar um processo totalmente isolado com docker ao invés do isolamento do sandbox feito pelo V8, somente caso fosse necessário um isolamento mais extremo. Verificação de limite de memória e timeout dendro do processo V8 também seriam uma boa alternativa.

3. Como aprimorar a implementação para suportar um alto volume de execuções concorrentes de scripts?

   - Criando filas de execução e workers para processar os scripts de forma organizada e controlar a concorrência.
     Kubernets para lidar com a orquestração, embora eu nunca tenha trabalhado com Kubernets e afins, mas acredito que seria provavelmente uma boa solução.

4. Como você evoluiria a API para suportar o versionamento de scripts?

   - Criar uma tag no banco com isCurrent para acessar a versao atual do script e evoluir o codigo para fazer essa busca e devolução corretante, tambem o salvamento de versões.

5. Que tipo de política de backup de dados você aplicaria neste cenário?

   - Um backup principalmente nas tabelas de preprocessing-script-store e process-data de forma mais recorrente e automatizada.

6. Como tratar massas de dados com potenciais informações sensíveis na API e no banco de dados?

   - Aplicando criptografia em informações sensíveis, criar um serviço de Encrypt e Decrypt por exmplo salvando a hash e o salt para conseguir acessar os valores reais quando necessário.

7. Como você enxerga o paradigma funcional beneficiando a solução deste problema?

   - Embora eu ainda não tenha experiência prática extensa com programação funcional, vejo benefícios claros para esse problema. A imutabilidade garante que os dados originais nunca sejam alterados, facilitando rastreabilidade e auditoria. Além disso, o uso de tipos que representam explicitamente sucesso ou falha, como Result ou Option, permite tratar erros de forma segura sem exceções, tornando o processamento de grandes volumes de dados mais previsível e confiável.
