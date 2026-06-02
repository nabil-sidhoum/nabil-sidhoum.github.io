# Phase 08 — Sécurité : CSP + meta headers

## Objectif

Sécuriser le portfolio avec une Content Security Policy stricte et les meta headers disponibles côté client. Préparer le terrain pour un futur passage Cloudflare (headers HTTP).

## Prérequis

- Phase 06 terminée minimum (toutes les sections de la page principale)
- Phase 07 optionnelle (le blog n'impacte pas la CSP)

## Contexte

Le portfolio est un Blazor WASM standalone déployé sur GitHub Pages. Il n'y a **pas de serveur ASP.NET Core** — donc pas de middleware, pas de NWebsec, pas de NetEscapades.AspNetCore.SecurityHeaders. La seule façon d'injecter des CSP est via des `<meta>` tags dans `index.html`.

Limitations des meta tags vs headers HTTP :
- `X-Frame-Options` → non supporté en meta tag, mais `frame-ancestors` dans CSP le remplace
- `X-Content-Type-Options` → **non supporté** en meta tag → disponible uniquement via Cloudflare (phase future)
- `Strict-Transport-Security` → **non supporté** en meta tag → GitHub Pages le gère nativement pour les sites HTTPS
- `Referrer-Policy` → supporté en meta tag ✓
- `Permissions-Policy` → **non supporté** en meta tag → Cloudflare uniquement

## Étapes

### 8.1 — Ajouter la CSP dans `wwwroot/index.html`

Dans le `<head>`, **avant** toute autre balise `<link>` ou `<script>` :

```html
<meta http-equiv="Content-Security-Policy" content="
    default-src 'self';
    script-src 'self' 'wasm-unsafe-eval';
    style-src 'self' 'unsafe-inline';
    font-src 'self';
    img-src 'self' data:;
    connect-src 'self';
    frame-src 'none';
    object-src 'none';
    base-uri 'self';
    form-action 'none';
    frame-ancestors 'none';
    upgrade-insecure-requests;
">
```

Explication rapide de chaque directive :

| Directive | Valeur | Pourquoi |
|-----------|--------|----------|
| `default-src` | `'self'` | Tout ce qui n'est pas explicitement autorisé est bloqué |
| `script-src` | `'self' 'wasm-unsafe-eval'` | Scripts locaux + instanciation WebAssembly (requis par Blazor WASM) |
| `style-src` | `'self' 'unsafe-inline'` | CSS locaux + styles inline (requis par Blazor scoped CSS) |
| `font-src` | `'self'` | Fonts self-hosted uniquement — aucune requête externe |
| `img-src` | `'self' data:` | Images locales + data URIs (placeholder photo) |
| `connect-src` | `'self'` | Fetch/XHR local uniquement (blog JSON, articles MD) |
| `frame-src` | `'none'` | Aucun iframe chargé par le site |
| `object-src` | `'none'` | Pas de Flash/plugins |
| `base-uri` | `'self'` | Empêche le détournement de `<base>` |
| `form-action` | `'none'` | Pas de formulaire (à changer si formulaire contact ajouté) |
| `frame-ancestors` | `'none'` | Empêche l'embarquement en iframe (remplace X-Frame-Options) |
| `upgrade-insecure-requests` | — | Force HTTPS sur toutes les requêtes |

### 8.2 — Ajouter le Referrer-Policy

Toujours dans le `<head>` de `index.html` :

```html
<meta name="referrer" content="strict-origin-when-cross-origin">
```

Cela signifie :
- Same-origin : le referrer complet est envoyé
- Cross-origin : seule l'origin (domaine) est envoyée, pas le path
- Downgrade HTTPS → HTTP : aucun referrer

### 8.3 — Nettoyer le `<head>` complet

Profiter de cette phase pour vérifier que le `<head>` de `index.html` est propre et complet :

```html
<!DOCTYPE html>
<html lang="fr" data-theme="dark">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <!-- Sécurité -->
    <meta http-equiv="Content-Security-Policy" content="default-src 'self'; script-src 'self' 'wasm-unsafe-eval'; style-src 'self' 'unsafe-inline'; font-src 'self'; img-src 'self' data:; connect-src 'self'; frame-src 'none'; object-src 'none'; base-uri 'self'; form-action 'none'; frame-ancestors 'none'; upgrade-insecure-requests;">
    <meta name="referrer" content="strict-origin-when-cross-origin">

    <!-- SEO / Social -->
    <title>Nabil Sidhoum — Tech Lead .NET | AI Agent Engineering</title>
    <meta name="description" content="Portfolio de Nabil Sidhoum — Tech Lead .NET, 11 ans d'expérience. Clean Architecture, CQRS, MediatR, agents IA. Paris, France.">
    <meta name="author" content="Nabil Sidhoum">

    <!-- Open Graph -->
    <meta property="og:title" content="Nabil Sidhoum — Tech Lead .NET">
    <meta property="og:description" content="Senior .NET Tech Lead — Clean Architecture, CQRS, AI Agent Engineering. Paris.">
    <meta property="og:type" content="website">
    <meta property="og:url" content="https://nabil-sidhoum.github.io">
    <meta property="og:locale" content="fr_FR">

    <!-- Twitter Card -->
    <meta name="twitter:card" content="summary">
    <meta name="twitter:title" content="Nabil Sidhoum — Tech Lead .NET">
    <meta name="twitter:description" content="Senior .NET Tech Lead — Clean Architecture, CQRS, AI Agent Engineering.">

    <!-- Favicon -->
    <link rel="icon" type="image/png" href="favicon.png" />

    <!-- CSS -->
    <link rel="stylesheet" href="css/theme.css" />
    <link href="Portfolio.styles.css" rel="stylesheet" />
</head>
<body>
    <div id="app">
        <!-- Loading indicator pendant le chargement WASM -->
        <div style="display:flex;justify-content:center;align-items:center;height:100vh;">
            <p style="font-family:monospace;color:#6b7280;">chargement...</p>
        </div>
    </div>

    <script src="_framework/blazor.webassembly.js"></script>
</body>
</html>
```

Points d'attention :
- `lang="fr"` sur le `<html>` — le contenu principal est en français
- `data-theme="dark"` par défaut — le ThemeToggle (Phase 03) le modifie ensuite
- `Portfolio.styles.css` est le fichier auto-généré par Blazor pour les scoped CSS
- Le script de décodage email Cloudflare (`email-decode.min.js`) est **supprimé**
- Aucune référence à Google Fonts
- Aucun script inline (tout passe par les fichiers JS dans `wwwroot/js/`)

### 8.4 — Vérifier que les modules JS respectent la CSP

Les fichiers JS créés dans les phases précédentes (`theme.js`, `scroll-spy.js`) sont chargés via `import()` dynamique depuis `IJSRuntime`. Ils sont dans `wwwroot/js/` donc servis depuis `'self'` — pas de problème avec `script-src 'self'`.

Vérifier qu'aucun JS inline n'est présent dans les fichiers `.razor`. Tout appel JS doit passer par `IJSRuntime.InvokeAsync` ou `IJSRuntime.InvokeVoidAsync`.

### 8.5 — Préparer le fichier `_headers` pour Cloudflare (futur)

Créer `wwwroot/_headers` — ce fichier n'aura aucun effet sur GitHub Pages mais sera prêt si Nabil migre vers Cloudflare Pages :

```
/*
  X-Content-Type-Options: nosniff
  X-Frame-Options: DENY
  Referrer-Policy: strict-origin-when-cross-origin
  Permissions-Policy: camera=(), microphone=(), geolocation=(), interest-cohort=()
  Strict-Transport-Security: max-age=31536000; includeSubDomains
  Content-Security-Policy: default-src 'self'; script-src 'self' 'wasm-unsafe-eval'; style-src 'self' 'unsafe-inline'; font-src 'self'; img-src 'self' data:; connect-src 'self'; frame-src 'none'; object-src 'none'; base-uri 'self'; form-action 'none'; frame-ancestors 'none'; upgrade-insecure-requests;
```

Note : si Cloudflare est activé, la CSP dans `_headers` prend le dessus sur la meta tag. Garder les deux synchronisés.

## Validation

```bash
dotnet run
```

Tests critiques dans le navigateur (F12 → Console) :

1. **Le site démarre** — pas d'erreur CSP bloquant le WASM
2. **Le toggle dark/light fonctionne** — `localStorage` via `IJSRuntime` n'est pas bloqué
3. **Les fonts se chargent** — pas de requête externe bloquée
4. **Les images s'affichent** — photo placeholder en `data:` ou local
5. **Le blog charge** — `fetch("posts/index.json")` autorisé par `connect-src 'self'`
6. **Aucune erreur CSP dans la console** — vérifier l'absence totale de violations

Test complémentaire : ouvrir https://securityheaders.com après déploiement pour auditer les headers. Sur GitHub Pages sans Cloudflare, le score sera C/D (à cause des headers manquants). Avec Cloudflare + `_headers`, le score passera à A/A+.

## Commit

```
feat: ajouter la Content Security Policy et les meta headers de sécurité

- CSP stricte dans index.html (wasm-unsafe-eval, fonts self-hosted, aucune ressource externe)
- Referrer-Policy strict-origin-when-cross-origin
- meta tags SEO et Open Graph
- nettoyage du <head> (suppression email-decode.js, Google Fonts)
- fichier _headers prêt pour migration Cloudflare
```

## Checklist avant de passer à la phase 09

- [ ] Le site démarre sans erreur CSP dans la console
- [ ] Le toggle dark/light fonctionne
- [ ] Les fonts se chargent localement
- [ ] Le blog charge ses données
- [ ] Aucune requête réseau externe (vérifier onglet Network)
- [ ] Le `<head>` est propre (lang, title, description, OG, favicon)
- [ ] Le fichier `_headers` est prêt pour Cloudflare
- [ ] Le build compile sans erreur
