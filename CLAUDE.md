# CLAUDE.md — Portfolio Nabil Sidhoum

## Vue d'ensemble

Portfolio personnel, **Blazor WebAssembly .NET 8**. SPA statique hébergée sur **GitHub Pages** (`gh-pages`), déployée automatiquement via GitHub Actions sur push `main`. Zéro backend, zéro API externe — données JSON statiques.

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
| `Microsoft.AspNetCore.Components.WebAssembly` | 8.0.18 |
| `xunit` | 2.9.3 |
| `RichardSzalay.MockHttp` | 7.0.0 |
| Bootstrap (local) | 5.x |
| Bootstrap Icons (CDN jsDelivr) | 1.11.3 |
| GitHub Actions `peaceiris/actions-gh-pages` | v3 |

---

## Commandes essentielles

```bash
# Dev local (HTTP 5205 / HTTPS 7036)
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
├── Components/   # ExperienceCard, EducationCard, ProjectCard, CustomNavLink
├── Layout/       # MainLayout (.razor + .razor.cs JS interop), NavMenu
├── Models/       # POCO — Experience, Project, Education
├── Pages/        # Home, Experiences, Projects, Educations
├── Services/     # ExperienceService, EducationService, ProjectService
└── wwwroot/
    ├── data/     # experiences.json, projects.json, educations.json
    ├── css/      # app.css + bootstrap/ (local)
    ├── images/   # assets visuels
    └── js/       # scroll.js uniquement (28 lignes)
```

---

## Conventions de commit (Conventional Commits, en français)

```
feat: ajout de la section compétences
fix: correction du scroll sur mobile
test: ajout des tests pour ProjectService
refactor: extraction du composant ExperienceCard
```

---

## Règles détaillées

- [Architecture](.claude/rules/clean-architecture.md)
- [Conventions C#](.claude/rules/csharp-conventions.md)
- [Tests](.claude/rules/testing.md)
