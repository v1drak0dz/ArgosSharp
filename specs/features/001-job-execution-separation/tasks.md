# Tarefas — Separação entre Job e JobExecution

## Status

**Draft — tarefas para o desenvolvedor, não para execução automática pela IA.**

As tarefas de implementação só devem começar depois da revisão de `spec.md`,
`plan.md` e das decisões marcadas como bloqueantes.

## 0. Decisões e preparação

- [ ] **T-001** Revisar e aprovar o problema, objetivo e escopo de `spec.md`.
- [ ] **T-002** Definir tipos, criação e exposição pública de `Job.Id` e
  `JobExecution.Id`.
- [ ] **T-003** Decidir se `JobHash` desaparece ou recebe outra responsabilidade.
- [ ] **T-004** Definir aggregate boundaries de Job, JobExecution e News.
- [ ] **T-005** Confirmar o lifecycle inicial, incluindo ou excluindo `Cancelled`.
- [ ] **T-006** Definir semântica mínima de retry/falha terminal para esta feature.
- [ ] **T-007** Definir política de timestamps/UTC.
- [ ] **T-008** Decidir compatibilidade ou migração do JSON existente.
- [ ] **T-009** Confirmar o recorte de API incluído nesta entrega.
- [ ] **T-010** Decidir, com o mentor, quais escolhas requerem ADR.

## 1. Domain — implementação manual

- [ ] **T-101** Alinhar `Job` à definição permanente aprovada e suas invariantes.
- [ ] **T-102** Alinhar `JobFactory` ou o creation boundary escolhido à nova
  entidade.
- [ ] **T-103** Alinhar `JobExecution` à identidade, relationship e timestamps
  aprovados.
- [ ] **T-104** Modelar as transições de status aprovadas no domínio.
- [ ] **T-105** Impedir transições arbitrárias que violem o lifecycle.
- [ ] **T-106** Representar retry como nova tentativa da mesma execução.
- [ ] **T-107** Representar reexecução como uma nova execução.
- [ ] **T-108** Relacionar News à execução conforme aggregate decision.
- [ ] **T-109** Remover a necessidade conceitual do antigo `Data` não tipado.
- [ ] **T-110** Verificar que Domain continua sem referências de infraestrutura.

## 2. Domain — testes pelo desenvolvedor

- [ ] **T-201** Testar criação válida e invariantes de Job.
- [ ] **T-202** Testar criação válida e invariantes de JobExecution.
- [ ] **T-203** Testar todas as transições válidas aprovadas.
- [ ] **T-204** Testar rejeição de transições inválidas.
- [ ] **T-205** Testar que retry preserva execution ID e incrementa Attempt.
- [ ] **T-206** Testar que reexecução cria execution ID diferente.
- [ ] **T-207** Testar que uma execução não altera a configuração do Job.
- [ ] **T-208** Testar rastreabilidade de News para a execução produtora.

## 3. Application — implementação manual

- [ ] **T-301** Inventariar todos os contracts que tratam `Job` como execução.
- [ ] **T-302** Separar operações sobre definição e execução conforme o plano
  aprovado.
- [ ] **T-303** Atualizar o caso de uso de criação conforme o contrato de API
  escolhido.
- [ ] **T-304** Atualizar processor para alterar somente estado/resultado da
  execução.
- [ ] **T-305** Atualizar queue e worker para transportar a referência aprovada.
- [ ] **T-306** Alinhar contratos de repository, persistence e unit of work.
- [ ] **T-307** Preservar scheduler, dispatcher e mensageria fora do escopo desta
  feature, exceto por contracts explicitamente aprovados.

## 4. Infrastructure — implementação manual

- [ ] **T-401** Adaptar repository ao modelo e aos identifiers aprovados.
- [ ] **T-402** Adaptar persistência ao relacionamento aprovado.
- [ ] **T-403** Implementar a decisão de migração/compatibilidade do histórico.
- [ ] **T-404** Verificar comportamento de concorrência e lifetime separadamente
  das regras de domínio.
- [ ] **T-405** Confirmar que adapters dependem dos contracts internos, sem
  dependência inversa do Domain para Infrastructure.

## 5. API e composition root — implementação manual

- [ ] **T-501** Adaptar request/response ao recorte de API aprovado.
- [ ] **T-502** Remover expectativas de `Status`, `Data`, `Error` e `JobHash` no
  DTO de definição, conforme decisões.
- [ ] **T-503** Expor referências de job/execução necessárias ao cliente.
- [ ] **T-504** Atualizar validação sem duplicar ou contradizer invariantes
  aprovadas.
- [ ] **T-505** Atualizar registrations afetados pelos novos contracts.
- [ ] **T-506** Executar um smoke test do host/composition root.

## 6. Regressão e revisão

- [ ] **T-601** Atualizar testes antigos que constroem o modelo anterior,
  justificando mudanças de expectativa.
- [ ] **T-602** Compilar a solução completa.
- [ ] **T-603** Executar a suíte de testes unitários.
- [ ] **T-604** Executar testes de persistência/migração aprovados.
- [ ] **T-605** Solicitar review da IA por severidade, sem correções automáticas.
- [ ] **T-606** Corrigir manualmente os findings aceitos.
- [ ] **T-607** Solicitar review do mentor.
- [ ] **T-608** Atualizar arquitetura, domínio, README e ADRs confirmados com o
  estado implementado.

## Critério de conclusão

A feature só pode ser marcada como concluída quando:

- decisões bloqueantes estiverem registradas;
- critérios de `acceptance.md` aplicáveis estiverem verificados;
- a solução compilar;
- testes aprovados passarem;
- nenhuma dependência de infraestrutura tiver sido introduzida no Domain;
- documentação representar o código realmente entregue.
