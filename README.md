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
3. Use o processId retornado para ver o resultado (endpoint 4)

## Questionário

[Questionário aqui](README.QUESTIONS.md)
