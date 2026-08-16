# Feature 001 — Separação entre Job e JobExecution

## Status

**Draft — aguardando revisão do desenvolvedor.**

Esta especificação registra as decisões fornecidas pelo desenvolvedor em
2026-08-14. Pontos não confirmados permanecem como `UNKNOWN` e não podem ser
decididos durante a implementação.

## Problema

O modelo anterior usa `Job` simultaneamente como definição do scraper e registro
de uma execução. Isso mistura configuração de longa duração com estado volátil,
resultado, erro e dados de worker.

Essa mistura impede representar adequadamente várias execuções do mesmo job,
histórico, retries, agendamento e comparação de resultados.

## Objetivo

Separar a definição permanente do trabalho (`Job`) de cada execução concreta
(`JobExecution`) e relacionar as notícias produzidas à execução responsável.

Essa separação deve preparar o domínio para scheduler, algoritmos de
escalonamento, múltiplos workers, histórico e adapters substituíveis sem acoplar o
domínio a RabbitMQ, SignalR ou persistência específica.

## Atores

- **Cliente da API:** cria ou solicita a execução de jobs e recebe uma resposta
  imediata.
- **Desenvolvedor/mentor:** define e avalia invariantes, architecture boundaries e
  trade-offs da refatoração.
- **Scheduler futuro:** seleciona qual definição executar e cria/agenda uma
  execução.
- **Worker futuro:** processa uma execução usando a definição correspondente.

Scheduler e worker aparecem como atores para definir os limites do modelo; sua
implementação não pertence ao escopo desta feature.

## Vocabulário

### Job

Uma entidade permanente, editável e reutilizável que descreve o que deve ser
executado. Pode existir por meses e ser executada inúmeras vezes.

### JobExecution

Uma entidade com identidade própria que representa um ciclo completo de execução
de um job.

### Retry

Nova tentativa automática dentro da mesma `JobExecution`, incrementando
`Attempt`.

### Reexecução

Execução manual ou agendada posterior que cria uma nova `JobExecution`.

### News

Resultado produzido por uma execução e relacionado à execução que o encontrou.

## Requisitos funcionais

### Job

- **FR-001:** O sistema deve representar `Job` como uma entidade de definição de
  scraper, não como uma execução.
- **FR-002:** `Job` deve possuir identidade, nome, termo de busca, parâmetros,
  prioridade, estado de habilitado e timestamps de criação/atualização.
- **FR-003:** `Job` não deve possuir status de execução, erro, worker, tentativa,
  tempo de execução ou dados coletados.
- **FR-004:** Um job deve poder existir sem execuções e acumular várias execuções
  ao longo do tempo.
- **FR-005:** Uma execução não deve alterar o estado de execução no próprio job.

### JobExecution

- **FR-006:** O sistema deve representar cada execução concreta como uma entidade
  `JobExecution` com identidade própria.
- **FR-007:** Toda execução deve referenciar exatamente um job.
- **FR-008:** `JobExecution` deve registrar status, attempt, worker opcional,
  timestamps de fila/início/fim e erro opcional.
- **FR-009:** Um job deve poder possuir zero ou muitas execuções.
- **FR-010:** As transições de status devem ser controladas por comportamento do
  domínio, não por alteração arbitrária de propriedade.
- **FR-011:** A primeira versão deve considerar `Pending`, `Queued`, `Running`,
  `Completed` e `Failed` como estados do fluxo.

### Tentativas e reexecução

- **FR-012:** Um retry automático deve manter a identidade da execução e
  incrementar `Attempt`.
- **FR-013:** Uma reexecução manual ou agendada deve criar uma nova execução com
  identidade própria.
- **FR-014:** A decisão de quando realizar retry pertence ao scheduler futuro e
  não a adapters de mensageria.

### Resultados

- **FR-015:** Cada notícia produzida deve ser associada à `JobExecution` que a
  encontrou.
- **FR-016:** Uma execução deve poder produzir zero ou muitas notícias.
- **FR-017:** O modelo não deve utilizar o antigo `IEnumerable<object> Data` para
  representar resultados.
- **FR-018:** Deve ser possível distinguir resultados de execuções diferentes do
  mesmo job.

### Value objects

- **FR-019:** `JobParameters` deve permanecer um value object do domínio.
- **FR-020:** Transformar `RetryPolicy`, `WorkerIdentifier` ou `SearchTerm` em
  value objects não faz parte desta feature sem decisão posterior.

## Requisitos não funcionais

- **NFR-001 — Independência:** o domínio não deve referenciar RabbitMQ, SignalR,
  banco de dados, framework web ou implementação de mensageria.
- **NFR-002 — Clean Architecture:** dependências devem apontar para dentro e
  tecnologias externas devem permanecer substituíveis por adapters.
- **NFR-003 — Testabilidade:** invariantes e transições do domínio devem ser
  verificáveis sem infraestrutura externa.
- **NFR-004 — Tipagem:** resultados não devem ser armazenados em coleções de
  `object` sem contrato de domínio.
- **NFR-005 — Rastreabilidade:** decisões ainda abertas não devem ser resolvidas
  silenciosamente durante a implementação.

## Regras de negócio confirmadas

1. `Job` descreve o que executar; `JobExecution` descreve como uma execução
   ocorreu.
2. A relação planejada é `Job 1:N JobExecution 1:N News`.
3. Configuração, parâmetros, nome, prioridade e habilitação pertencem ao job.
4. Status, timestamps, worker, attempt, erro e resultados pertencem à execução.
5. Retry automático não cria uma nova execução.
6. Reexecução manual ou agendada cria uma nova execução.
7. O domínio controla transições de estado.
8. RabbitMQ e SignalR não participam das regras do domínio.

## Escopo

- definir os contratos de domínio de `Job`, `JobExecution` e sua relação com
  `News`;
- remover do conceito de job as responsabilidades de execução;
- definir a semântica de retry versus reexecução;
- definir e testar o lifecycle inicial de execução após confirmação dos pontos
  ainda abertos;
- adaptar contratos consumidores somente até o necessário para representar o
  novo modelo de forma coerente, conforme plano aprovado.

## Fora de escopo

- implementação do scheduler;
- algoritmos de priority queue, heap, aging ou concorrência por domínio;
- implementação de retry/backoff/jitter;
- `IJobDispatcher` e dispatchers concretos;
- integração com RabbitMQ, Kafka ou SignalR;
- dashboard;
- escolha de banco de dados;
- definição completa do contrato público da API;
- autenticação e autorização;
- migração do JSON antigo sem decisão explícita.

## Critérios de aceite

1. O modelo diferencia inequivocamente definição e execução.
2. Um job pode ser relacionado a múltiplas execuções.
3. Uma execução possui identidade e lifecycle próprios.
4. Status, erro, worker, tentativa e timestamps não pertencem ao job.
5. Notícias são atribuíveis à execução produtora.
6. Retry mantém a execução; reexecução cria outra.
7. Transições inválidas são impedidas pelo domínio conforme lifecycle aprovado.
8. O Domain continua sem dependência de infraestrutura.
9. O antigo `Data` deixa de ser necessário para representar resultados.
10. Testes relevantes passam depois da implementação manual.

Os cenários verificáveis estão em [`acceptance.md`](acceptance.md).

## Decisões ainda necessárias

| Questão | Estado | Impacto |
|---|---|---|
| Tipo e exposição pública de `Job.Id` e `JobExecution.Id` | `UNKNOWN` | Contratos, persistência e API. |
| Futuro de `JobHash` | `UNKNOWN` | Compatibilidade e identidade pública. |
| Aggregate boundary entre job, execução e notícia | `UNKNOWN` | Navegação, invariantes e repositories. |
| Origem e invariantes de `Name` | `UNKNOWN` | Criação/edição de job. |
| Semântica detalhada de `Enabled` | `UNKNOWN` | Scheduler e execuções pendentes. |
| Ordenação e desempate de `Priority` | `UNKNOWN` | Scheduler futuro. |
| Inclusão e transições de `Cancelled` | `UNKNOWN` | Máquina de estados e API. |
| Retry máximo e falha terminal | `UNKNOWN` | Lifecycle e scheduler. |
| Política UTC/timestamps | `UNKNOWN` | Domínio, persistência e comparações. |
| Migração do JSON anterior | `UNKNOWN` | Compatibilidade e rollout. |

## Dependências com features futuras

Esta feature fornece a base conceitual para, em ordem planejada:

1. `JobScheduler`;
2. `InMemoryDispatcher` através de uma port de dispatch;
3. priority queue e heap binário;
4. aging e concorrência por domínio;
5. retry/backoff/jitter;
6. adapter RabbitMQ;
7. adapter SignalR e dashboard.

Essa lista registra sequência e contexto; não inclui essas funcionalidades no
escopo atual.
