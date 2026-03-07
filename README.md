<div align="center">

# Portfolio — Nabil Sidhoum

**Tech Lead .NET** · [nabil-sidhoum.github.io](https://nabil-sidhoum.github.io)

[![Deploy](https://github.com/nabil-sidhoum/nabil-sidhoum.github.io/actions/workflows/deploy-portfolio.yml/badge.svg)](https://github.com/nabil-sidhoum/nabil-sidhoum.github.io/actions/workflows/deploy-portfolio.yml)
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-512BD4?logo=blazor)
![License](https://img.shields.io/badge/licence-MIT-green)

*Ce portfolio est lui-même un projet technique : une SPA Blazor WebAssembly déployée automatiquement sur GitHub Pages via GitHub Actions.*

</div>

---

## 🧱 Stack technique

| Couche | Technologie |
|---|---|
| Framework | .NET 8 / Blazor WebAssembly |
| Style | CSS scopé par composant |
| Tests | xUnit + RichardSzalay.MockHttp |
| CI/CD | GitHub Actions |
| Hébergement | GitHub Pages |

---

## 🗂️ Architecture

```
src/
├── BlazorPortfolio.Client/
│   ├── Components/        # Composants réutilisables (cards, nav)
│   ├── Layout/            # Layout principal et navigation
│   ├── Models/            # Modèles de données (Experience, Project, Education)
│   ├── Pages/             # Pages Razor (Home, Experiences, Projects, Educations)
│   ├── Services/          # Couche service — chargement des données via HttpClient
│   └── wwwroot/
│       └── data/          # Données JSON statiques
└── BlazorPortfolio.Client.Tests/
    └── Services/          # Tests unitaires des services
```

### Choix structurants

**📦 Données en JSON statique** — `experiences.json`, `projects.json` et `educations.json` sont servis statiquement depuis `wwwroot/data/` et consommés via `HttpClient`. Le code applicatif respecte le même contrat qu'une vraie API REST : il ne sait pas d'où viennent les données, seulement comment les consommer.

**🎨 CSS scopé par composant** — chaque composant Razor embarque son propre `.razor.css`. Blazor génère automatiquement des sélecteurs scoped, sans librairie externe.

**⚡ Zéro dépendance JavaScript externe** — un seul fichier `scroll.js` minimal, rien d'autre.

---

## 🧪 Tests

```bash
cd src
dotnet test BlazorPortfolio.Client.Tests
```

| Cas testé | Description |
|---|---|
| ✅ Nominal | Désérialisation JSON correcte |
| ✅ Erreur 404 | Retour d'une collection vide sans exception |
| ✅ Mapping | Cohérence modèle / JSON |
| ✅ Tri | Expériences triées par date décroissante |

---

## 🚀 CI/CD

Chaque push sur `main` déclenche automatiquement :

```
push → Tests → Build → Deploy
         ↓
    bloque si KO
```

1. `dotnet test` — le déploiement est **bloqué en cas d'échec**
2. `dotnet publish -c Release`
3. Publication sur `gh-pages` via `peaceiris/actions-gh-pages`

---

## 🔍 Qualité du code

Le projet embarque un `.editorconfig` appliqué par `dotnet format` et les analyseurs Roslyn :

- Indentation 4 espaces · fin de ligne CRLF
- Types explicites — pas de `var`
- Champs privés préfixés `_camelCase`
- Modificateurs d'accès obligatoires sur tous les membres
- Chaque diagnostic désactivé est **documenté et justifié**

---

## 📜 Licence

MIT — voir [LICENSE](LICENSE).
