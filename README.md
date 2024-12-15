####################################################################################################################################
Este repositório foi criado para ajudar a mnater os recursos obtidos pela maipulação de dados dos projetos de lógicas das P=80/P-83

Os projetos de lógica dos PLCs principais das P80 e P83 são escritos para o COntrollogix 5000, atualmente na versão 35.
As lógicas são escritas em Function Blocks, Ladder, Structured Text ou Flowcharts.
Podem ser salvas em .acd, .l5x, l5k entre outros formatos.

    .ACD	Edição completa e comunicação com o controlador.	MyProject_v35.acd
    .L5X	Exportação/importação em XML para backup ou transferência.	ExportedProject_v35.l5x
    .L5K	Análise ou edição em texto ASCII.	ProjectText_v35.l5k
    .CSV	Manipulação em massa de tags e variáveis.	TagsExport_v35.csv
    .TXT	Relatórios e documentação específica.	DiagnosticsReport_v35.txt
    .XML	Integração e backups em formato legível.	ProjectExport_v35.xml

Usando o formato L5X se obtém o código completo das lógicas e é possível organizar trechos de dados para um estudo mais aprofundado.
O primeiro uso foi a obtenção de relação de implementação dos add-ons, padrões ou customizados.
Se obteve a lista de tags, o tipo do dado, e todos os parâmetros associados ao tipo.
Os dados obtidos nesta etapa foram salvos em csv para facilitar o manuesio em outras aproximações.
Uma das aproximações foi criar tabelas em banco de dados, uma tabela por tipo (documento/tipo).
####################################################################################################################################
