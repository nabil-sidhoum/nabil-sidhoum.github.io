# Architecture — Conventions

## Couches et responsabilités

| Couche | Dossier | Règle clé |
|--------|---------|-----------|
| **Domain** | `Domain/Entities/` | POCO purs — aucune logique, aucune dépendance externe, aucune annotation EF Core |
| **Application** | `Application/` | CQRS via MediatR — Queries/Commands + Handlers + interfaces Repository |
| **Infrastructure** | `Infrastructure/` | Implémentations concrètes des Repository, accès données, clients HTTP |
| **API / Présentation** | `Api/` ou `Web/` | Controllers minimalistes — délèguent à MediatR, aucune logique métier |
| **Tests** | `*.Tests/` | Un projet de test par projet testé |

---

## Règles absolues par couche

### Domain
- Aucune méthode métier complexe dans les entités
- Aucune dépendance vers Application, Infrastructure ou Api
- Pas d'annotations EF Core dans les entités Domain
- Valeur objects autorisés si immutables

### Application
- Toute la logique métier est dans les Handlers MediatR
- Les Handlers ne dépendent que des interfaces définies dans Application
- Pas d'accès direct à la base de données — passer par les Repository interfaces
- Un fichier = une Query ou Command + son Handler
- Nommage : `GetXxxQuery`, `CreateXxxCommand`, `XxxQueryHandler`, `XxxCommandHandler`

### Infrastructure
- Implémente les interfaces définies dans Application
- Seule couche autorisée à dépendre d'EF Core, Dapper, HttpClient
- Les clients HTTP externes (Bloomberg, SIX, OpenAI, CRM) sont dans Infrastructure
- Pattern Decorator autorisé pour les cross-cutting concerns (BulkHead, Retry, Cache)

### API / Présentation
- Controllers : injection de `IMediator` uniquement
- Aucune logique métier dans les Controllers
- Validation des entrées via FluentValidation dans Application, pas dans les Controllers
- Middlewares : responsabilité unique, documentée en en-tête de fichier

---

## Dependency Injection

- Lifetime Singleton : clients HTTP avec état partagé (token cache, BulkHead policy)
- Lifetime Scoped : Repository, services métier
- Lifetime Transient : Handlers MediatR (géré automatiquement)
- Toujours utiliser `IHttpClientFactory` — jamais `new HttpClient()`

---

## Patterns autorisés

- CQRS + MediatR avec Pipeline Behaviours pour cross-cutting (logging, validation)
- Decorator pattern pour enrichir un client sans modifier son interface (ex: BulkHeadDecorator)
- Repository pattern avec interface dans Application, implémentation dans Infrastructure

## Patterns interdits sans discussion

- State global partagé entre requêtes
- Appel direct à la base de données depuis Application ou Api
- Héritage profond (plus de 2 niveaux)
- Couplage circulaire entre couches

---

## Interdit

- Logique métier dans les Controllers
- Injection de services Infrastructure dans Application
- `Console.WriteLine` en production — utiliser `ILogger<T>`
- Données sensibles dans les logs (tokens, mots de passe, IBAN, données personnelles)
