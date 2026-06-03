# CLAUDE.md — Portfolio Nabil Sidhoum

## Vue d'ensemble

Portfolio personnel, **Blazor WebAssembly .NET 10**. SPA statique hébergée sur **GitHub Pages**, déployée automatiquement via GitHub Actions sur push `main`. Zéro backend, zéro API externe — données chargées dynamiquement depuis des fichiers JSON statiques via `HttpClient`.

**Refonte en cours** — design système README (`Portfolio.html` de référence dans `.claude/plans/`). Branche de travail : `feature/redesign-portfolio`.

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
| `Markdig` | 0.38.0 *(ajouté en Phase 07 — blog)* |
| GitHub Actions | deploy.yml *(Phase 10)* |

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

## Architecture cible (refonte)

```
src/BlazorPortfolio.Client/
├── Components/
│   ├── Nav/          # Nav.razor, MobileMenu.razor, ThemeToggle.razor
│   ├── Sections/     # HeroSection, AboutSection, StackSection, ProjectCard,
│   │                 # ProjectsSection, ExperienceSection, BlogSection, ContactSection
│   └── Shared/       # PathStrip.razor, Footer.razor
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

**Référence visuelle** : `.claude/plans/Portfolio.html` fait foi pour palette, typographie, layout et responsive.

---

## Données dynamiques

Les données sont chargées via `HttpClient` depuis `wwwroot/data/`. Ajouter du contenu = modifier le JSON, pas le code.

| Fichier | Modèle | Service | Sections affichées |
|---------|--------|---------|-------------------|
| `data/experiences.json` | `ExperienceInfo` | `ExperienceService` | ExperienceSection |
| `data/projects.json` | `ProjectInfo` | `ProjectService` | ProjectsSection |
| `data/educations.json` | *(non affiché pour l'instant)* | — | — |
| `posts/index.json` + `posts/*.md` | `BlogArticle` | `BlogService` | Blog.razor, BlogPost.razor |

Schema `projects.json` : champs `Nom`, `BadgeType` (`"Showcase"` ou `"OpenSource"`), `Description`, `GithubUrl`, `ExtraUrl`, `ExtraLabel`, `Competences`.

Schema `experiences.json` : champs `Societe`, `Poste`, `Lieu`, `DateDebut`, `DateFin` (null = poste actuel), `Accroche`, `Descriptions`, `Competences`.

---

## Workflow de refonte — une branche par phase

Chaque phase de la refonte suit ce process strict :

### Convention de branches

```
feature/redesign-portfolio        ← branche d'intégration (base et cible de toutes les PR de phase)
  └── feature/redesign-p01-preparation
  └── feature/redesign-p02-design-system
  └── feature/redesign-p03-navigation
  └── feature/redesign-p04-hero-about
  └── feature/redesign-p05-stack-projects
  └── feature/redesign-p06-experience-blog-contact
  └── feature/redesign-p07-blog-routing
  └── feature/redesign-p08-securite-csp
  └── feature/redesign-p09-polish-qa
  └── feature/redesign-p10-deploiement
```

`feature/redesign-portfolio` sera mergée sur `main` à la fin de la Phase 10.

### Process par phase (ordre strict)

```
1. git checkout feature/redesign-portfolio && git pull
2. git checkout -b feature/redesign-pXX-nom
3. Implémenter la phase
4. dotnet build        → 0 erreur obligatoire avant de continuer
5. dotnet test         → si des tests existent pour cette phase
6. ui-verifier         → screenshots dans .claude/screenshots/phase-XX-nom/ (phases UI uniquement)
7. /project:review     → dotnet-reviewer + architecture-reviewer sur les fichiers modifiés
8. git commit
9. git push + PR vers feature/redesign-portfolio
```

### Règles commits et PR

- Conventional Commits en français : `feat:`, `fix:`, `test:`, `refactor:`, `ci:`
- Pas de co-auteur Claude dans les commits ou les PR
- Un commit par phase (sauf si la phase est trop volumineuse)

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

## État de la refonte

| Phase | Branche | Statut |
|-------|---------|--------|
| 01 — Préparation repo | `feature/redesign-p01-preparation` | ✅ PR #8 ouverte |
| 02 — Design system (tokens CSS + fonts) | `feature/redesign-p02-design-system` | ⏳ À faire |
| 03 — Layout + navigation | `feature/redesign-p03-navigation` | ⏳ À faire |
| 04 — Hero + About | `feature/redesign-p04-hero-about` | ⏳ À faire |
| 05 — Stack + Projects | `feature/redesign-p05-stack-projects` | ⏳ À faire |
| 06 — Experience + Blog + Contact | `feature/redesign-p06-experience-blog-contact` | ⏳ À faire |
| 07 — Blog routing | `feature/redesign-p07-blog-routing` | ⏳ À faire |
| 08 — Sécurité CSP | `feature/redesign-p08-securite-csp` | ⏳ À faire |
| 09 — Polish + QA | `feature/redesign-p09-polish-qa` | ⏳ À faire |
| 10 — Déploiement | `feature/redesign-p10-deploiement` | ⏳ À faire |

---

## Conventions de commit (Conventional Commits, en français)

```
feat: ajout de la section stack
fix: correction du scroll sur mobile
test: ajout des tests pour ProjectService
refactor: extraction du composant ProjectCard
ci: configuration GitHub Actions déploiement
```

---

## Règles détaillées

- [Architecture](.claude/rules/clean-architecture.md)
- [Conventions C#](.claude/rules/csharp-conventions.md)
- [Tests](.claude/rules/testing.md)
