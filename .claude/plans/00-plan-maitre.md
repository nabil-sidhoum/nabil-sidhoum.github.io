# Plan de refonte — Portfolio Blazor WASM

> **Document historique** — plan de la refonte terminée en juin 2026. Conservé comme archive de planification ; ne décrit pas l'état courant du projet. Synthèse de ce qui a réellement été livré : [`docs/refonte-2026.md`](../../docs/refonte-2026.md).

## Vue d'ensemble

Refonte complète du portfolio Blazor WebAssembly hébergé sur GitHub Pages (`nabil-sidhoum.github.io`).
Design source : fichier `Portfolio.html` généré via Claude Design (direction C — README + nav sticky de B).

## Phases (ordre d'exécution strict)

| # | Phase | Fichier | Prérequis | Estimation |
|---|-------|---------|-----------|------------|
| 01 | Nettoyage et préparation du repo | `01-preparation-repo.md` | Aucun | 30 min |
| 02 | Design system — tokens CSS + fonts self-hosted | `02-design-system.md` | Phase 01 | 45 min |
| 03 | Layout principal + navigation + mobile menu | `03-layout-navigation.md` | Phase 02 | 1h |
| 04 | Sections Hero + About | `04-hero-about.md` | Phase 03 | 45 min |
| 05 | Sections Stack + Projects | `05-stack-projects.md` | Phase 04 | 1h |
| 06 | Sections Experience + Blog + Contact + Footer | `06-experience-blog-contact.md` | Phase 05 | 1h |
| 07 | Pages Blog (routing multi-pages) | `07-blog-routing.md` | Phase 06 | 45 min |
| 08 | Sécurité — CSP + headers | `08-securite-csp.md` | Phase 06 | 30 min |
| 09 | Polish — background pattern, contraste, responsive QA | `09-polish-qa.md` | Phase 08 | 45 min |
| 10 | Déploiement — GitHub Actions + GitHub Pages | `10-deploiement.md` | Phase 09 | 30 min |

## Conventions à respecter dans toutes les phases

- **C#** : pas de `var`, pas de primary constructors, `Nullable disable`, `ImplicitUsings disable`, SRP
- **CSS** : scoped CSS par composant (`.razor.css`), tokens partagés dans `theme.css`
- **Commits** : Conventional Commits, descriptions en français
- **Contenu** : français pour le texte, anglais pour les termes techniques et noms de projets
- **Référence visuelle** : `Portfolio.html` (Claude Design) fait foi pour le rendu cible

## Fichier de référence

Le fichier `Portfolio.html` doit être accessible dans le repo ou fourni à Claude Code à chaque session.
Il sert de source de vérité pour : palette, typographie, layout, spacing, comportement responsive.

## Ce qui est hors scope (pour l'instant)

- Domaine custom (nabil-sidhoum.dev)
- Cloudflare (headers HTTP côté CDN)
- Prérendu SSR / migration Blazor Web App
- Contenu réel des articles de blog
- Photo headshot (placeholder conservé)
