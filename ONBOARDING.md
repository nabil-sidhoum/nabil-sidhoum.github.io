# Guide d'onboarding — Portfolio Nabil Sidhoum

Portfolio personnel **Blazor WebAssembly .NET 8** — SPA statique hébergée sur GitHub Pages,
déployée automatiquement via GitHub Actions sur push `main`. Zéro backend, zéro API externe.

---

## Prérequis

- .NET 8 SDK
- VS Code + extension Claude Code (Anthropic)
- Git configuré avec ton identité

## Configuration Git (première fois)

```bash
git config --global user.name "Prénom Nom"
git config --global user.email "ton.email@exemple.com"

# Activer les hooks partagés (pre-commit + commit-msg)
git config core.hooksPath .githooks
chmod +x .githooks/pre-commit
chmod +x .githooks/commit-msg
```

---

## Commandes de démarrage

```bash
# Lancer en local (HTTP 5205 / HTTPS 7036)
dotnet run --project src/BlazorPortfolio.Client

# Lancer les tests
dotnet test src/BlazorPortfolio.Client.Tests/BlazorPortfolio.Client.Tests.csproj

# Vérifier les conventions sans modifier
dotnet format --verify-no-changes

# Build production
dotnet publish src/BlazorPortfolio.Client/BlazorPortfolio.Client.csproj -c Release -o build
```

---

## Workflow recommandé — dans cet ordre

### 1. Audit initial — `/project:log-anomalies`

À lancer en premier sur un repo fraîchement cloné.
Analyse l'ensemble de la solution et écrit le résultat dans `anomalies.local.md` (local, non commité).

```
/project:log-anomalies
```

### 2. Avant chaque commit — `/project:review`

Analyse uniquement les fichiers modifiés et signale les violations de convention.

```
/project:review
```

### 3. Corriger un bug — `/project:fix-issue`

Décris le bug. Claude Code consulte l'historique, propose un fix, attend ta validation.

```
/project:fix-issue [Description du bug]
```

### 4. Traiter la dette — `/project:cleanup`

Traite les anomalies ouvertes dans `anomalies.local.md` une par une.

```
/project:cleanup
```

### 5. Vérifier les tests — `/project:run-tests`

Lance la suite de tests et synthétise les résultats.

```
/project:run-tests
```

### 6. Créer une Pull Request — `/project:create-pr`

Prépare la branche, le commit et la PR GitHub. Génère les commandes à copier-coller.

```
/project:create-pr
```

Convention de nommage des branches :
- `feat/nom-du-sujet` — nouvelle fonctionnalité
- `fix/nom-du-sujet` — correction de bug

---

## Structure du projet

```
src/
├── BlazorPortfolio.Client/
│   ├── Components/   # ExperienceCard, EducationCard, ProjectCard, CustomNavLink
│   ├── Layout/       # MainLayout (razor + razor.cs JS interop), NavMenu
│   ├── Models/       # POCO — Experience, Project, Education
│   ├── Pages/        # Home, Experiences, Projects, Educations
│   ├── Services/     # ExperienceService, EducationService, ProjectService
│   └── wwwroot/
│       ├── data/     # experiences.json, projects.json, educations.json
│       ├── css/      # app.css + bootstrap/ (local)
│       ├── images/   # assets visuels
│       └── js/       # scroll.js uniquement
└── BlazorPortfolio.Client.Tests/
    └── Services/     # Tests unitaires des services
```

---

## Hooks Git

| Hook | Déclencheur | Vérifie |
|------|-------------|---------|
| `pre-commit` | Avant chaque commit | Anomalies ouvertes + `dotnet format` + `dotnet test` |
| `commit-msg` | À la saisie du message | Format Conventional Commits en français |

Le pre-commit **bloque** le commit si des anomalies sont ouvertes dans `anomalies.local.md` ou si les tests échouent.
Pour bypasser exceptionnellement : `git commit --no-verify` (à éviter).

---

## CI/CD

| Workflow | Déclencheur | Actions |
|----------|-------------|---------|
| `ci.yml` | Push/PR sur `main` | Build + tests + couverture Codecov |
| `quality.yml` | PR sur `main` | `dotnet format --verify-no-changes` |
| `deploy-portfolio.yml` | Push sur `main` | Déploiement GitHub Pages |

Le déploiement est **bloqué** si les tests échouent.

---

## Règles du projet

| Fichier | Contenu |
|---------|---------|
| `.claude/rules/clean-architecture.md` | Responsabilités par couche |
| `.claude/rules/csharp-conventions.md` | Conventions C# |
| `.claude/rules/testing.md` | Pattern AAA, nommage, couverture |

---

## Fichiers locaux — ne jamais commiter

| Fichier | Raison |
|---------|--------|
| `anomalies.local.md` | Backlog personnel de dette technique |
| `.claude/history/` | Journal interne des corrections |
| `.claude/settings.local.json` | Configuration locale Claude Code |

Ces fichiers sont dans `.gitignore`.

---

## Dette technique partagée

La dette identifiée en PR ou revue de code est tracée dans `TECH_DEBT.md` (commité).
Les anomalies personnelles restent dans `anomalies.local.md` (local).
