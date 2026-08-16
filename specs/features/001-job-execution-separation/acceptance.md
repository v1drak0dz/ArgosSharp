# Aceitação — Separação entre Job e JobExecution

## Status

**Draft — cenários para revisão.**

Os cenários abaixo descrevem comportamento esperado, não implementação. Cenários
dependentes de decisões ainda abertas estão identificados no final.

## Cenários do Job

### AC-001 — Job representa uma definição

**Given** uma configuração válida de scraper

**When** um Job é criado

**Then** ele contém identidade, nome, termo, parâmetros, prioridade, habilitação e
timestamps

**And** ele não contém status, erro, worker, attempt ou resultados de execução.

### AC-002 — Execução não altera a definição

**Given** um Job existente

**And** uma JobExecution relacionada a ele

**When** a execução muda de status ou recebe resultados

**Then** o estado de execução muda somente na JobExecution

**And** a configuração do Job permanece inalterada.

### AC-003 — Job sem execução

**Given** uma definição válida de Job

**When** nenhuma execução foi solicitada

**Then** o Job pode existir sem JobExecutions.

## Cenários de JobExecution

### AC-004 — Nova execução possui identidade própria

**Given** um Job existente e habilitado

**When** uma execução é criada

**Then** uma nova JobExecution com identidade própria referencia o Job

**And** inicia no estado inicial aprovado

**And** possui `Attempt` inicial coerente com a regra aprovada.

### AC-005 — Várias execuções do mesmo Job

**Given** um Job que já possui uma execução concluída

**When** uma reexecução manual ou agendada é solicitada

**Then** uma nova JobExecution é criada

**And** sua identidade é diferente da execução anterior

**And** ambas continuam relacionadas ao mesmo Job.

### AC-006 — Retry automático preserva a execução

**Given** uma JobExecution que pode receber nova tentativa

**When** o scheduler decide realizar um retry automático

**Then** a identidade da JobExecution permanece a mesma

**And** `Attempt` é incrementado

**And** nenhuma nova JobExecution é criada.

## Cenários de lifecycle

### AC-007 — Fluxo concluído

**Given** uma JobExecution válida

**When** ela percorre `Pending`, `Queued` e `Running` e termina com sucesso

**Then** seu estado final é `Completed`

**And** timestamps aplicáveis são coerentes com a sequência da execução.

### AC-008 — Fluxo com falha

**Given** uma JobExecution em `Running`

**When** ocorre uma falha terminal segundo a regra aprovada

**Then** seu estado é `Failed`

**And** o erro pertence à JobExecution

**And** o Job relacionado não recebe estado de falha.

### AC-009 — Transição inválida

**Given** uma JobExecution em um estado conhecido

**When** é solicitada uma transição não permitida pelo lifecycle aprovado

**Then** o domínio rejeita a transição

**And** o estado anterior é preservado.

## Cenários de News

### AC-010 — Resultado atribuído à execução

**Given** uma JobExecution em processamento

**When** notícias são coletadas

**Then** cada News pode ser atribuída à JobExecution produtora

**And** não é necessário usar `IEnumerable<object> Data` no Job.

### AC-011 — Comparação entre execuções

**Given** duas JobExecutions do mesmo Job com resultados diferentes

**When** os resultados são consultados

**Then** é possível distinguir quais notícias foram produzidas por cada execução.

### AC-012 — Execução sem resultado

**Given** uma JobExecution válida

**When** nenhuma notícia é encontrada

**Then** a execução pode representar zero resultados sem adicionar dados ao Job.

## Cenários arquiteturais

### AC-013 — Domínio independente

**Given** os projetos da solução após a refatoração

**When** as dependências do Domain são inspecionadas

**Then** não existe referência a RabbitMQ, SignalR, banco de dados, ASP.NET Core ou
adapter de infraestrutura.

### AC-014 — Infraestrutura substituível

**Given** o modelo de Job e JobExecution

**When** uma tecnologia futura de dispatch ou persistência for escolhida

**Then** as entidades e regras confirmadas não precisam conhecer essa tecnologia.

## Cenários de regressão

### AC-015 — Consumidores não usam Job como execução

**Given** os consumidores atuais adaptados ao novo modelo

**When** controller, use cases, queue, worker, processor, repository e persistence
são revisados

**Then** nenhum deles depende de `Job.Status`, `Job.Error`, `Job.Data` ou de outra
propriedade de execução no Job.

### AC-016 — Solução verificável

**Given** a implementação manual concluída

**When** a solução é compilada e a suíte aprovada é executada

**Then** não há erro causado pela coexistência dos modelos antigo e novo

**And** os testes de domínio e regressão passam.

## Cenários aguardando decisão

Não considerar estes cenários aprovados até fechar as respectivas questões:

- exposição pública dos identificadores e destino de `JobHash`;
- aggregate/storage boundary de JobExecution e News;
- comportamento de `Enabled = false`;
- cancelamento e transições para `Cancelled`;
- retry máximo, backoff e falha terminal;
- política UTC/timestamps;
- leitura ou migração do JSON anterior;
- status HTTP, payload e consulta da API assíncrona.

Após cada decisão, os cenários correspondentes devem ser adicionados antes da
implementação afetada.
