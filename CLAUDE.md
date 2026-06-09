# CLAUDE.md — Portfolio Nabil Sidhoum

## Vue d'ensemble

Portfolio personnel, **Blazor WebAssembly .NET 10**. SPA statique hébergée sur **GitHub Pages**, déployée automatiquement via GitHub Actions sur push `main`. Zéro backend, zéro API externe — données chargées dynamiquement depuis des fichiers JSON statiques via `HttpClient`.

> Le projet est issu d'une refonte complète (migration .NET 10 + design system maison) menée par phases puis clôturée en juin 2026. Récit complet : [`docs/refonte-2026.md`](docs/refonte-2026.md).

---

## Projets

| Projet | SDK | Rôle |
|--------|-----|------|
| `src/BlazorPortfolio.Client` | `Microsoft.NET.Sdk.BlazorWebAssembly` | Application SPA |
| `src/BlazorPortfolio.Client.Tests` | `Microsoft.NET.Sdk` | Tests unitaires |

---

## Stack technique

| Package | Version |
|---------|---------|
| `Microsoft.AspNetCore.Components.WebAssembly` | 10.0.8 |
| `xunit` | 2.9.3 |
| `RichardSzalay.MockHttp` | 7.0.0 |
| `Markdig` | 0.38.0 *(rendu Markdown du blog)* |
| GitHub Actions | `ci-portfolio` · `quality-portfolio` · `deploy-portfolio` |

Design system : CSS custom avec tokens dark/light — pas de framework CSS externe.

---

## Commandes essentielles

```bash
# Dev local
dotnet run --project src/BlazorPortfolio.Client

# Tests unitaires
dotnet test src/BlazorPortfolio.Client.Tests/BlazorPortfolio.Client.Tests.csproj

# Build production
dotnet publish src/BlazorPortfolio.Client/BlazorPortfolio.Client.csproj -c Release -o build

# Vérification des conventions (sans modification)
dotnet format --verify-no-changes
```

---

## Architecture

```
src/BlazorPortfolio.Client/
├── Components/
│   ├── Nav/          # Nav.razor, MobileMenu.razor, ThemeToggle.razor
│   ├── Sections/     # HeroSection, AboutSection, StackSection, ProjectCard,
│   │                 # ProjectsSection, ExperienceSection, BlogSection, ContactSection
│   └── Common/       # PathStrip.razor, Footer.razor
├── Layout/           # MainLayout.razor + .razor.css
├── Models/           # ProjectInfo.cs, ExperienceInfo.cs, BlogArticle.cs
├── Pages/            # Index.razor, Blog.razor, BlogPost.razor
├── Services/         # ProjectService, ExperienceService, BlogService, UIStateService
└── wwwroot/
    ├── css/          # theme.css (tokens dark/light, reset, classes utilitaires)
    ├── fonts/        # JetBrains Mono + IBM Plex Sans (.woff2, self-hosted)
    ├── assets/       # cv.pdf, favicon.png
    ├── js/           # theme.js, scroll-spy.js
    ├── posts/        # index.json + articles .md (blog)
    └── data/         # experiences.json, projects.json, educations.json
```

---

## Données dynamiques

Les données sont chargées via `HttpClient` depuis `wwwroot/data/`. Ajouter du contenu = modifier le JSON, pas le code.

| Fichier | Modèle | Service | Sections affichées |
|---------|--------|---------|-------------------|
| `data/stack.json` | `StackCategoryInfo` | `StackService` | StackSection |
| `data/experiences.json` | `ExperienceInfo` | `ExperienceService` | ExperienceSection |
| `data/projects.json` | `ProjectInfo` | `ProjectService` | ProjectsSection |
| `data/educations.json` | *(non affiché pour l'instant)* | — | — |
| `posts/index.json` + `posts/*.md` | `BlogArticle` | `BlogService` | Blog.razor, BlogPost.razor |

Schema `stack.json` : champs `Key` (libellé de la catégorie, ex. `".net"`), `IsAccent` (`true` = pills en style accent), `Items` (liste de technos).

Schema `projects.json` : champs `Nom`, `BadgeType` (`"Showcase"` ou `"OpenSource"`), `Description`, `GithubUrl`, `ExtraUrl`, `ExtraLabel`, `Competences`.

Schema `experiences.json` : champs `Societe`, `Poste`, `Lieu`, `DateDebut`, `DateFin` (null = poste actuel), `Accroche`, `Descriptions`, `Competences`.

---

## Workflow Git

Modèle **une branche par unité de travail** (`feat/*`, `fix/*`, `docs/*`, `chore/*`, `ci/*`, `refactor/*`), jamais de commit direct sur `main` — tout passe par une Pull Request mergée en *rebase* (historique linéaire). Politique complète : [`git-workflow.md`](.claude/rules/git-workflow.md).

---

## Agents disponibles (locaux — non commités)

Les agents sont dans `.claude/agents/` (gitignored — proviennent d'un repo privé).

| Agent | Modèle | Rôle | Déclencheur |
|-------|--------|------|-------------|
| `dotnet-reviewer` | Haiku | Conventions C# style et patterns | Après modification `.cs` |
| `architecture-reviewer` | Sonnet | Séparation des responsabilités Blazor | Après modification structurelle |
| `test-reviewer` | Haiku | Qualité tests xUnit | Si `*Tests.cs` dans le diff |
| `security-reviewer` | Sonnet | Sécurité Blazor WASM, CSP, secrets | Si `index.html`, `wwwroot/` ou Services touchés |
| `lead-dev` | Sonnet | Orchestration review PR complète | `/project:review-pr` |
| `ui-verifier` | Sonnet | Vérification visuelle Blazor — screenshots par phase dans `.claude/screenshots/phase-XX/` | En fin de chaque phase UI |

---

## Commandes disponibles

| Commande | Usage |
|----------|-------|
| `/project:review` | Review des fichiers non commités via agents spécialisés |
| `/project:review-pr` | Review complète branche via lead-dev avant merge |
| `/project:run-tests` | Tests et synthèse |
| `/project:create-pr` | PR avec vérifications préalables |
| `/project:fix-issue [desc]` | Correction bug avec historique |
| `/project:log-anomalies` | Audit complet |
| `/project:cleanup` | Traitement violations du dernier rapport |

---

## Règles détaillées

- [Architecture](.claude/rules/clean-architecture.md)
- [Conventions C#](.claude/rules/csharp-conventions.md)
- [Conventions Blazor](.claude/rules/blazor-conventions.md)
- [Tests](.claude/rules/testing.md)
- [Sécurité CSP & headers](.claude/rules/csp-security.md)
- [Workflow Git — branches, protection, contribution](.claude/rules/git-workflow.md)

## Historique

- [Refonte du portfolio (2026)](docs/refonte-2026.md) — récit de la migration .NET 10 + design system, phase par phase *(versionné)*
- `.claude/issues/fixes.md` — journal local des corrections (alimenté par `/project:fix-issue`, non versionné)
