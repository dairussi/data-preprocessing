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
