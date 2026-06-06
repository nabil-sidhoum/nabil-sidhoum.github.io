# Phase 01 — Préparation du repo

## Objectif

Nettoyer le repo existant `nabil-sidhoum.github.io`, mettre à jour le SDK .NET, et poser la structure de fichiers cible avant de toucher au code.

## Prérequis

- Accès au repo GitHub `nabil-sidhoum.github.io`
- .NET 10 SDK installé localement

## Étapes

### 1.1 — État des lieux

Avant toute modification, lister ce qui existe :

```
Commande : dotnet --version
Commande : ls -la pour voir la structure actuelle
Commande : cat Portfolio.csproj (ou le nom du .csproj existant)
```

Identifier :
- Le TFM actuel (net8.0 ? net9.0 ?)
- Les packages NuGet installés
- La structure des pages/composants existants
- Le contenu de `wwwroot/` (CSS, images, index.html)

### 1.2 — Créer une branche de travail

```bash
git checkout -b feature/redesign-portfolio
```

Tout le travail se fait sur cette branche. Merge sur `main` uniquement quand la phase 09 (QA) est validée.

### 1.3 — Mettre à jour le TFM vers .NET 10

Dans le `.csproj` :

```xml
<TargetFramework>net10.0</TargetFramework>
```

Mettre à jour les packages Microsoft.AspNetCore.Components.WebAssembly vers la version .NET 10 correspondante.

```bash
dotnet restore
dotnet build
```

Si le build casse, corriger les breaking changes avant de continuer. Ne pas avancer avec un build cassé.

### 1.4 — Poser la structure de fichiers cible

Créer les dossiers vides (les fichiers seront créés dans les phases suivantes) :

```
Portfolio/
├── wwwroot/
│   ├── css/
│   │   └── theme.css              ← Phase 02
│   ├── fonts/                     ← Phase 02 (self-hosted)
│   ├── assets/                    ← images, cv.pdf
│   └── index.html                 ← existe déjà, sera modifié Phase 08
├── Layout/
│   ├── MainLayout.razor           ← Phase 03
│   └── MainLayout.razor.css       ← Phase 03
├── Components/
│   ├── Nav/                       ← Phase 03
│   ├── Sections/                  ← Phases 04-06
│   └── Common/                    ← composants réutilisables
├── Pages/
│   ├── Index.razor                ← Phase 04 (page principale)
│   ├── Blog.razor                 ← Phase 07
│   └── BlogPost.razor             ← Phase 07
├── Models/
│   └── ProjectInfo.cs             ← Phase 05
├── Program.cs
└── Portfolio.csproj
```

### 1.5 — Supprimer le contenu obsolète

Supprimer les anciens composants/pages qui ne seront pas réutilisés. Garder :
- `Program.cs` (sera adapté)
- `wwwroot/index.html` (sera modifié)
- `_Imports.razor` (sera mis à jour)
- Tout asset réutilisable (photo, favicon)

Ne PAS supprimer le `.github/workflows/` s'il existe — il sera mis à jour en phase 10.

### 1.6 — Valider le build vide

```bash
dotnet build
dotnet run
```

Le site doit démarrer sans erreur, même s'il affiche une page vide ou le template par défaut.

## Commit

```
feat: préparer la structure pour la refonte du portfolio

- mise à jour TFM vers net10.0
- création de la structure de dossiers cible
- suppression des composants obsolètes
```

## Checklist avant de passer à la phase 02

- [ ] Le build compile sans erreur
- [ ] La branche `feature/redesign-portfolio` est créée
- [ ] La structure de dossiers cible est en place
- [ ] Le TFM est net10.0
- [ ] Les anciens composants sont supprimés
- [ ] Le `Portfolio.html` de référence est accessible (copié dans le repo ou disponible localement)
