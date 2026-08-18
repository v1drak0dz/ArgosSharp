# ADR-XXX — Vincular Sources ao JobType

**Status:** Accepted
**Date:** 2026-08-17

## Context

Um `Job` possui um tipo de processamento (`JobType`) e uma lista de `Sources` utilizadas durante sua execução.

Inicialmente, foi considerada a possibilidade de criar enums ou tipos específicos para representar os sources de cada domínio, como `JobSource` e `NewsSource`.

Essa abordagem adicionaria uma relação de tipagem entre cada source e seu domínio, mas também aumentaria o acoplamento entre os sources e a estrutura do domínio.

Além disso, o sistema possui diferentes tipos de jobs que podem utilizar conjuntos de sources diferentes.

Por exemplo:

* `JobPosting` pode utilizar `LinkedIn`, `Indeed` e `Glassdoor`.
* `NewsArticle` pode utilizar `G1`, `CNN` e `Reuters`.

Portanto, um source isoladamente não deve determinar qual estratégia de processamento será utilizada.

## Decision

O `JobType` será responsável por determinar:

1. Qual domínio/processador deve tratar o Job.
2. Quais `Sources` são válidos para aquele tipo de Job.

`Sources` continuará sendo representado como uma coleção de identificadores, sem a criação de enums globais como `JobSource` ou `NewsSource`.

A relação conceitual será:

```text
JobType
   │
   ├── define o tipo de processamento
   │
   └── define os Sources permitidos
```

Exemplo:

```text
JobPosting
├── LinkedIn
├── Indeed
└── Glassdoor

NewsArticle
├── G1
├── CNN
└── Reuters
```

Assim, uma combinação como:

```text
JobType = JobPosting
Source = LinkedIn
```

é válida, enquanto:

```text
JobType = JobPosting
Source = G1
```

é inválida.

## Consequences

### Positivas

* `JobType` passa a ser a única informação necessária para determinar o domínio do processamento.
* `Sources` não ficam acoplados a enums específicos de cada tipo de Job.
* Novos tipos de Job podem definir seus próprios sources sem alterar um enum global.
* O frontend pode montar dinamicamente o multiselect de sources a partir do `JobType`.
* A mesma definição de `JobType` pode ser utilizada para validação no backend e construção da UI.
* A arquitetura permite adicionar novos sources sem necessariamente modificar o modelo de domínio.

### Negativas

* A validade de um `Source` não pode ser determinada olhando apenas para o próprio source.
* O backend precisa validar a combinação `JobType + Source`.
* A representação textual dos sources exige uma definição clara de identificadores estáveis.
* Pode ser necessário criar um catálogo/registry de definições de `JobType`.

## Validation

A validação deve ocorrer no backend, independentemente da validação realizada pelo frontend.

O frontend deve impedir combinações inválidas na UI, mas isso não constitui uma garantia de domínio.

Exemplo:

```text
POST /jobs

JobType: JobPosting
Sources:
    - LinkedIn
    - Indeed
```

Deve ser aceito.

Enquanto:

```text
POST /jobs

JobType: JobPosting
Sources:
    - LinkedIn
    - G1
```

deve ser rejeitado.

A regra deve possuir testes automatizados cobrindo, no mínimo:

```text
JobPosting + LinkedIn  → válido
JobPosting + Indeed    → válido
JobPosting + G1        → inválido

NewsArticle + G1       → válido
NewsArticle + Reuters  → válido
NewsArticle + LinkedIn → inválido
```

## Frontend

O frontend deverá obter os sources disponíveis de acordo com o `JobType` selecionado.

Preferencialmente, a API deverá disponibilizar uma definição semelhante a:

```json
[
  {
    "type": "JobPosting",
    "sources": [
      "linkedin",
      "indeed",
      "glassdoor"
    ]
  },
  {
    "type": "NewsArticle",
    "sources": [
      "g1",
      "cnn",
      "reuters"
    ]
  }
]
```

O formulário de criação de Job poderá então:

1. Permitir a seleção do `JobType`.
2. Carregar os sources disponíveis para aquele tipo.
3. Exibir os sources em um multiselect.
4. Enviar somente sources pertencentes ao `JobType` selecionado.

## Processing

Durante o processamento, o `JobType` será utilizado para determinar o processor/strategy responsável pela execução.

Conceitualmente:

```text
JobExecution
      │
      ▼
   JobType
      │
      ├── JobPosting
      │       │
      │       ▼
      │   JobPostingProcessor
      │       │
      │       ├── LinkedIn
      │       └── Indeed
      │
      └── NewsArticle
              │
              ▼
          NewsArticleProcessor
              │
              ├── G1
              └── Reuters
```

O `Source` representa **onde** o processamento será realizado, enquanto o `JobType` representa **o que está sendo processado**.

## Future Considerations

Pode ser introduzida uma abstração como:

```csharp
public interface IJobTypeDefinition
{
    JobType Type { get; }

    IReadOnlyCollection<string> AvailableSources { get; }
}
```

Cada tipo poderia então fornecer sua própria definição:

```csharp
public sealed class JobPostingDefinition : IJobTypeDefinition
{
    public JobType Type => JobType.JobPosting;

    public IReadOnlyCollection<string> AvailableSources =>
    [
        "linkedin",
        "indeed",
        "glassdoor"
    ];
}
```

Essa abordagem deve ser adotada apenas quando a quantidade de `JobTypes` justificar a existência de um catálogo/registry dedicado.

## Alternatives Considered

### Typed Sources por JobType

Criar enums separados:

```text
JobSource
NewsSource
ScientificArticleSource
```

**Rejeitado.**

Isso aumentaria o acoplamento entre `Source` e `JobType` e faria com que a estrutura de sources precisasse acompanhar a estrutura dos tipos de Job.

### Source determinando a Strategy

Utilizar o próprio source para determinar qual strategy executar.

Exemplo:

```text
LinkedIn → JobPostingStrategy
G1 → NewsArticleStrategy
```

**Rejeitado.**

O source não representa o domínio do trabalho. Além disso, diferentes sources podem pertencer ao mesmo `JobType`, e a estratégia deve ser determinada pelo tipo de Job.

## Result

A arquitetura adotada será:

```text
                  ┌───────────────┐
                  │    JobType    │
                  └───────┬───────┘
                          │
             ┌────────────┴────────────┐
             │                         │
             ▼                         ▼
      Processing Strategy       Valid Sources
             │                         │
             ▼                         ▼
       JobProcessor             Source Registry
```

`JobType` é a autoridade sobre o domínio do Job.

`Sources` são identificadores de recursos pertencentes àquele domínio.

A combinação entre ambos deve ser válida antes que o `Job` seja criado ou executado.
