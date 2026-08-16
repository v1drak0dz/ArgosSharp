# Plano técnico — Separação entre Job e JobExecution

## Status

**Draft — não aprovado para implementação.**

Este plano deve ser discutido com o desenvolvedor. Ele identifica impactos e
sequência; não autoriza a IA a alterar código de produção.

## Objetivo técnico

Alinhar Domain, Application, Infrastructure, API e testes ao modelo confirmado em
[`spec.md`](spec.md), preservando a direção da Clean Architecture e removendo a
dependência do fluxo atual em um `Job` que também funciona como execução.

## Pré-condições para aprovação

- [ ] Definir tipos e exposição pública de `Job.Id` e `JobExecution.Id`.
- [ ] Decidir o destino de `JobHash`.
- [ ] Escolher aggregate boundaries entre job, execução e notícia.
- [ ] Confirmar lifecycle inicial e a inclusão ou não de `Cancelled`.
- [ ] Definir retry máximo/falha terminal ou declarar explicitamente que ficará
  para a feature de scheduler.
- [ ] Decidir política de timestamps.
- [ ] Decidir estratégia de compatibilidade/migração do JSON.
- [ ] Confirmar o recorte de API necessário nesta feature.

Enquanto esses pontos estiverem abertos, as fases abaixo são uma proposta de
discussão, não tarefas autorizadas para um agente executar.

## Delta entre código atual e modelo alvo

| Área | Estado atual | Estado alvo confirmado |
|---|---|---|
| `Job` | Nova shape de definição, mas factory/consumidores ainda usam o modelo antigo. | Somente identidade, configuração, prioridade, habilitação e timestamps. |
| `JobExecution` | Classe isolada e não utilizada. | Entidade de uma execução completa, relacionada ao job. |
| Status | Enum e serviços usam lifecycle antigo no próprio job. | Lifecycle pertence à execução e é controlado pelo domínio. |
| Resultado | Serviços esperam `Job.Data`; classe atual não possui a propriedade. | `News` relacionada à execução; coleção de `object` eliminada. |
| Retry/reexecução | Não representados de forma explícita. | Retry mantém execução/eleva attempt; reexecução cria outra. |
| Application | Use case, queue, worker e processor trafegam `Job`. | Contratos devem distinguir definição de comando/execução conforme decisões. |
| Infrastructure | Repository/JSON indexam e persistem o modelo antigo. | Persistência coerente para definição, execução e resultado, após decisão. |
| API | `POST /Jobs` cria e retorna o objeto usado como execução. | Recorte exato ainda deve ser confirmado; alvo geral cria execução e retorna imediatamente. |

## Componentes potencialmente afetados

### Domain

- `Entity/Job.cs`;
- `Entity/JobExecution.cs`;
- `ValueObjects/JobParameters.cs`;
- `ValueObjects/News.cs`;
- enums de status e prioridade;
- factory de job e possíveis creation boundaries;
- exceptions/invariantes relacionadas.

### Application

- interfaces de repository, persistence e unit of work;
- `CreateJobUseCase` e seu contrato;
- `JobQueue` e `JobWorker`;
- `JobProcessorService`;
- contratos do scraper processor;
- futuros contracts de execução, sem implementar scheduler/dispatcher nesta
  feature.

### Infrastructure

- repository e indexação;
- serialização JSON e possível migração;
- unit of work/coordenador de persistência;
- mapper de resultados, dependendo da escolha de relationship.

### API

- request/response DTOs;
- controller e status HTTP;
- validation boundaries;
- composition root afetado pelos contratos alterados.

### Testes

- testes de Domain para entidades, invariantes e state machine;
- testes de Application para criação/reexecução/retry sem infraestrutura;
- testes existentes que constroem o modelo antigo;
- testes de repository/persistence somente após decisão de storage/migração;
- smoke test de DI é recomendado, mas pode ser uma tarefa separada se ficar fora
  do recorte aprovado.

## Escolhas técnicas que não devem ser presumidas

### Aggregate boundary

Alternativas para discussão:

1. `Job` e `JobExecution` como aggregates separados, relacionados por ID.
2. `Job` como aggregate root que gerencia uma coleção de execuções.

O volume histórico e a necessidade futura de carregar uma execução pelo
`executionId` favorecem investigar aggregates separados, mas isso é
**INFERENCE**, não decisão.

### Relação com News

Alternativas para discussão:

1. `News` dentro do aggregate de execução.
2. `News` persistida separadamente e ligada por `ExecutionId`.

Quantidade de notícias, carregamento e comparação entre execuções devem orientar
a escolha. Não há decisão atual.

### Lifecycle

O lifecycle inicial confirmado é `Pending -> Queued -> Running -> Completed` ou
`Failed`. A implementação da state machine, a possibilidade de cancelar e as
transições de retry precisam ser aprovadas antes de escrever o domínio.

### Compatibilidade

Alternativas para discussão:

1. Migração explícita do JSON anterior.
2. Leitura temporariamente compatível e escrita somente no novo formato.
3. Descarte do histórico anterior nesta etapa da mentoria.

Nenhuma alternativa está aprovada.

## Fases propostas

### Fase 0 — Fechar decisões

1. Revisar `spec.md` e `acceptance.md`.
2. Responder às pré-condições deste plano.
3. Decidir quais escolhas justificam ADR.
4. Atualizar os documentos antes da implementação.

### Fase 1 — Modelo de domínio

1. Alinhar identidades e criação de `Job`.
2. Modelar `JobExecution` e seu lifecycle.
3. Representar a relação com `News` conforme aggregate decision.
4. Expressar retry versus reexecução no nível aprovado.
5. Criar testes de domínio antes ou junto da implementação manual.

### Fase 2 — Contratos de Application

1. Separar operações sobre definição e execução.
2. Atualizar contratos que ainda transportam `Job` como work item.
3. Definir o que queue/worker recebem nesta fase, sem introduzir RabbitMQ.
4. Manter infraestrutura fora das regras de domínio.

### Fase 3 — Persistência e adapters atuais

1. Aplicar o modelo de storage aprovado.
2. Tratar migração/compatibilidade conforme decisão.
3. Validar concorrência e lifetime sem misturar essa correção com regras de
   domínio.

### Fase 4 — API e composition root

1. Aplicar o recorte de API aprovado.
2. Ajustar DTOs e mapping.
3. Corrigir registrations impactados pelos novos contracts.
4. Validar que o host inicia.

### Fase 5 — Verificação

1. Compilar todos os projetos.
2. Executar testes unitários relevantes.
3. Executar smoke/integration tests aprovados.
4. Solicitar AI review por severidade.
5. Atualizar arquitetura, domínio e README com comportamento implementado.

## Estratégia de testes proposta

- Unit tests do Domain cobrem criação válida/inválida e state transitions.
- Tests de Application confirmam que retry mantém execution ID e reexecução cria
  outro ID.
- Tests confirmam que executar não altera configuração do job.
- Tests de relacionamento confirmam rastreabilidade de news por execution.
- Regression tests cobrem consumers atualmente dependentes de `Status`, `Data`,
  `Error` e `JobHash`.
- Build/architecture check confirma que Domain não referencia infraestrutura.

Coverage threshold e mutation score continuam `TO BE DEFINED` na constituição.

## Riscos

- Corrigir apenas o construtor pode restaurar acidentalmente o modelo antigo.
- Alterar IDs sem decisão pode quebrar API e persistência silenciosamente.
- Carregar todo histórico dentro de `Job` pode criar aggregate excessivamente
  grande; separar demais pode perder invariantes. A escolha exige discussão.
- Misturar esta feature com scheduler/RabbitMQ amplia o escopo e esconde erros do
  domínio.
- Migração JSON tardia pode tornar dados atuais ilegíveis.
- Atualizar testes apenas para compilar pode preservar expectativas obsoletas.

## Possíveis ADRs

- Separação entre definição e execução, após concluir identifiers/aggregates.
- Aggregate/storage boundary de `JobExecution` e `News`.
- Compatibilidade/migração da persistência atual, caso o JSON precise ser
  preservado.

Dispatcher/RabbitMQ e scheduler devem ser discutidos em specs futuras, não neste
plano.

## Saída esperada da revisão deste plano

O desenvolvedor deve:

1. aprovar ou corrigir o recorte;
2. responder às pré-condições;
3. escolher as alternativas necessárias;
4. confirmar quais fases pertencem à mesma entrega;
5. autorizar somente então o início da implementação manual.
