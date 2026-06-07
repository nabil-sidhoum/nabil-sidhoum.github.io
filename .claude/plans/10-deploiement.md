# Phase 10 — Déploiement : GitHub Actions + GitHub Pages

## Objectif

Configurer le pipeline CI/CD pour builder le Blazor WASM et déployer automatiquement sur GitHub Pages à chaque push sur `main`.

## Prérequis

- Phase 09 terminée (QA validée, site prêt visuellement)
- Le repo `nabil-sidhoum.github.io` est configuré pour GitHub Pages

## Contexte

GitHub Pages pour un repo `<username>.github.io` publie la branche `main` (ou `gh-pages`) à l'URL `https://<username>.github.io`. Pour Blazor WASM, il faut builder le projet, copier le contenu de `wwwroot/` dans le dossier de publication, et gérer le routing SPA (fallback `404.html`).

## Étapes

### 10.1 — Créer le workflow GitHub Actions

Créer `.github/workflows/deploy.yml` :

```yaml
name: Déployer le portfolio

on:
  push:
    branches: [main]
  workflow_dispatch:

permissions:
  contents: read
  pages: write
  id-token: write

concurrency:
  group: pages
  cancel-in-progress: true

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    environment:
      name: github-pages
      url: ${{ steps.deployment.outputs.page_url }}
    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Setup .NET 10
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: "10.0.x"

      - name: Restore
        run: dotnet restore

      - name: Publish
        run: dotnet publish -c Release -o output

      - name: Vérifier le base href
        run: |
          # Pour nabil-sidhoum.github.io le base href "/" est correct — aucune modification.
          # Si le repo avait un autre nom (ex: "portfolio"), utiliser :
          # sed -i 's|<base href="/" />|<base href="/portfolio/" />|g' output/wwwroot/index.html
          grep -q '<base href="/" />' output/wwwroot/index.html && echo "base href OK" || (echo "ERREUR : base href inattendu" && exit 1)

      - name: Créer le fallback 404.html pour le SPA routing
        run: cp output/wwwroot/index.html output/wwwroot/404.html

      - name: Créer .nojekyll
        run: touch output/wwwroot/.nojekyll

      - name: Upload artifact
        uses: actions/upload-pages-artifact@v3
        with:
          path: output/wwwroot

      - name: Deploy to GitHub Pages
        id: deployment
        uses: actions/deploy-pages@v4
```

### 10.2 — Le fichier `404.html`

GitHub Pages renvoie `404.html` pour toute URL qui ne correspond pas à un fichier statique. En copiant `index.html` vers `404.html`, Blazor WASM intercepte la route côté client et affiche la bonne page (`/blog`, `/blog/{slug}`).

C'est le mécanisme standard pour le SPA routing sur GitHub Pages. Sans ce fichier, naviguer directement vers `https://nabil-sidhoum.github.io/blog` afficherait une erreur 404 de GitHub.

### 10.3 — Le fichier `.nojekyll`

GitHub Pages utilise Jekyll par défaut pour processer les fichiers. Les fichiers commençant par `_` (comme `_framework/` de Blazor WASM) sont ignorés par Jekyll. Le fichier `.nojekyll` à la racine désactive Jekyll et sert tous les fichiers tels quels.

Sans ce fichier, le runtime Blazor WASM ne se chargera pas.

### 10.4 — Configurer GitHub Pages pour les Actions

Dans le repo GitHub :

1. Settings → Pages
2. Source : **GitHub Actions** (pas "Deploy from a branch")
3. Sauvegarder

C'est nécessaire pour que le workflow `deploy-pages@v4` fonctionne.

### 10.5 — Vérifier le `<base href>`

Dans `wwwroot/index.html`, vérifier que le base href est correct :

```html
<base href="/" />
```

Pour un repo `<username>.github.io`, le base path est `/`. Si le repo avait un autre nom (par exemple `portfolio`), le base href serait `/<repo-name>/`.

### 10.6 — Merge et déploiement

```bash
# Sur la branche feature/redesign-portfolio
git add .
git commit -m "feat: configurer le pipeline de déploiement GitHub Pages"

# Merge sur main
git checkout main
git merge feature/redesign-portfolio

# Push — le workflow se déclenche automatiquement
git push origin main
```

### 10.7 — Vérification post-déploiement

Après le push, aller dans Actions → vérifier que le workflow "Déployer le portfolio" passe au vert.

Puis vérifier sur `https://nabil-sidhoum.github.io` :

- [ ] La page principale se charge
- [ ] Le toggle dark/light fonctionne
- [ ] La navigation scrolle vers les sections
- [ ] `/blog` fonctionne (routing SPA)
- [ ] Le CV se télécharge
- [ ] Les liens GitHub/NuGet fonctionnent
- [ ] Le responsive est correct sur mobile (tester sur téléphone réel)
- [ ] La console n'affiche aucune erreur CSP
- [ ] Les fonts se chargent (pas de fallback system-ui)

### 10.8 — Audit de sécurité post-déploiement

Aller sur https://securityheaders.com et entrer `https://nabil-sidhoum.github.io`.

Score attendu sans Cloudflare : **C ou D** — les meta tags CSP et Referrer-Policy sont détectés, mais les headers HTTP manquants (X-Content-Type-Options, Permissions-Policy, HSTS) font baisser le score. C'est normal et attendu pour un hébergement GitHub Pages pur.

Pour passer à A+ : migrer vers Cloudflare Pages et activer le fichier `_headers` (préparé en Phase 08).

### 10.9 — Supprimer la branche de travail

```bash
git branch -d feature/redesign-portfolio
git push origin --delete feature/redesign-portfolio
```

## Commit

```
ci: configurer le déploiement GitHub Pages via GitHub Actions

- workflow deploy.yml avec .NET 10, publish, fallback 404.html
- fichier .nojekyll pour désactiver Jekyll
- documentation du process de déploiement
```

## Checklist finale

- [ ] Le workflow GitHub Actions passe au vert
- [ ] Le site est accessible sur `https://nabil-sidhoum.github.io`
- [ ] Toutes les sections s'affichent correctement
- [ ] Le routing SPA fonctionne (`/blog`, `/blog/{slug}`)
- [ ] Le CV se télécharge
- [ ] Le toggle dark/light fonctionne et persiste
- [ ] Le responsive est validé sur mobile réel
- [ ] La console est propre (aucune erreur)
- [ ] Les meta tags SEO et Open Graph sont en place (vérifier avec https://metatags.io)
- [ ] La branche de travail est supprimée
- [ ] Le `Portfolio.html` de référence peut être archivé ou supprimé du repo

---

## Annexe — Cloudflare Pages : viser un score A+ (vitrine, optionnel)

**Cadrage honnête.** Pour ce portfolio **statique, sans backend ni authentification ni données sensibles**, les headers HTTP ci-dessous ne corrigent **aucune faille exploitable** : les attaques qu'ils bloquent (clickjacking, MIME-sniffing, downgrade HTTPS…) n'ont pas de surface d'attaque ici. L'intérêt n'est donc **pas** la sécurité « risque », mais la **vitrine** : un score **A+** sur securityheaders.com est un signal de professionnalisme pertinent pour un portfolio de Tech Lead. C'est l'argument qui justifie cette annexe — pas une menace à corriger.

Le fichier `wwwroot/_headers` préparé en **Phase 08** porte les headers que la balise `<meta>` ne peut pas (`X-Content-Type-Options`, `X-Frame-Options`/`frame-ancestors`, `Permissions-Policy`, `HSTS`) et fait passer le score **C/D → A+**.

### Point bloquant : l'URL `*.github.io`

Tant que l'URL est `nabil-sidhoum.github.io`, **Cloudflare en proxy est impossible** : le proxy exige de gérer le DNS du domaine chez Cloudflare (changement de nameservers), or `github.io` appartient à GitHub. Pour activer Cloudflare, il faut **changer d'hébergeur (Cloudflare Pages)** ou un **domaine personnalisé**.

> ⚠️ Le fichier `_headers` n'est lu **que** par **Cloudflare Pages** (l'hébergeur). Derrière un simple proxy Cloudflare devant GitHub Pages, il est ignoré — il faudrait recréer les headers via les *Transform Rules* / Workers de Cloudflare.

### Options

| Option | Coût | `_headers` actif ? | URL |
|--------|------|--------------------|-----|
| **A. Cloudflare Pages + sous-domaine `*.pages.dev`** | **0 €** | ✅ nativement | `nabil-sidhoum.pages.dev` |
| **B. Domaine perso + Cloudflare Pages** | ~10–15 €/an | ✅ nativement | `tondomaine.dev` |
| **C. Domaine perso + GitHub Pages derrière proxy Cloudflare** | ~10–15 €/an | ⚠️ partiel (Transform Rules / Workers) | `tondomaine.dev` |
| **D. Rester GitHub Pages seul** (état après Phase 10) | 0 € | ❌ | `nabil-sidhoum.github.io` |

L'option **A** atteint le A+ **gratuitement**. On peut garder les deux hébergements en parallèle (GitHub Pages `.github.io` **et** Cloudflare Pages `.pages.dev`) sur le même repo. L'option **B** ajoute en plus un domaine perso (vitrine renforcée).

### Subtilité build — Blazor WASM .NET 10 sur Cloudflare Pages

L'environnement de build de Cloudflare Pages n'inclut pas .NET 10 en preset. Montage robuste réutilisant le workflow de la Phase 10 :

1. **GitHub Actions** exécute le `dotnet publish -c Release` (déjà en place).
2. Une étape pousse `output/wwwroot` vers Cloudflare Pages en **Direct Upload** via `wrangler` (`cloudflare/wrangler-action`), avec `CLOUDFLARE_API_TOKEN` en secret.

Ce montage évite de dépendre du build natif de Cloudflare Pages et garde une seule source de build (.NET). À implémenter uniquement si l'option A ou B est retenue.

Référence des règles CSP/headers : `.claude/rules/csp-security.md`.
