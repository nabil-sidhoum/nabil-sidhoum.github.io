# Conventions C# — BlazorPortfolio

Source de vérité : `src/.editorconfig`.

---

## Nommage

| Élément | Convention | Exemple |
|---------|-----------|---------|
| Classes / Types / Composants | PascalCase | `ExperienceService`, `ExperienceCard` |
| Propriétés publiques | PascalCase | `Societe`, `DateDebut`, `GithubUrl` |
| Champs privés | `_camelCase` | `_client` |
| Paramètres / Variables locales | camelCase | `client`, `result` |
| Méthodes async | suffixe `Async` | `GetExperiencesAsync()` |

---

## Règles absolues

- **Pas de `var`** — types explicites toujours (`IDE0007 = none`)
- **Modificateurs d'accès** — toujours explicites (public, private, etc.)
- **Accolades** — obligatoires sur tous les blocs sans exception
- **Namespaces** — block-scoped uniquement (`namespace Foo { }`, pas `namespace Foo;`)
- **Primary constructors** — non utilisés (`IDE0290 = none`)
- **Records** — non utilisés, tous les modèles sont des `class { get; set; }`
- **Pas de `.Result` / `.Wait()`** — risque de deadlock en WASM

---

## Nullable reference types (.NET 8)

```csharp
public string Societe { get; set; } = "";             // non-nullable : initialisé avec ""
public List<string> Descriptions { get; set; } = [];  // non-nullable : initialisé avec []
public DateTime? DateFin { get; set; }                 // nullable : suffixe ?
public string? ExtraUrl { get; set; }                  // nullable : suffixe ?
```

---

## Collections vides (C# 12)

Toujours `[]` — jamais `new List<T>()` ni `Enumerable.Empty<T>()`.

---

## Formatage

- 4 espaces · CRLF · UTF-8
- Expression-bodied : acceptable pour propriétés simples, bloc préféré pour méthodes
