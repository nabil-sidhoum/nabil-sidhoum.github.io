# Sécurité — Content Security Policy & headers

> Source de vérité unique de la posture CSP du portfolio. `index.html`, `wwwroot/_headers`, le `security-reviewer` (vérif statique) et le `ui-verifier` (vérif runtime) DOIVENT s'aligner sur ce document.

## Contexte

Blazor WebAssembly **standalone**, déployé sur **GitHub Pages**. Pas de serveur ASP.NET Core → pas de middleware, pas de NWebsec. La CSP active en production passe **uniquement** par une balise `<meta http-equiv="Content-Security-Policy">` dans `index.html`. Le fichier `wwwroot/_headers` est **inerte sur GitHub Pages** ; il ne s'appliquera qu'après une éventuelle migration vers Cloudflare Pages.

---

## CSP appliquée (balise `<meta>` dans `index.html`)

```
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
upgrade-insecure-requests;
```

| Directive | Valeur | Justification |
|-----------|--------|---------------|
| `default-src` | `'self'` | Tout ce qui n'est pas explicitement autorisé est bloqué |
| `script-src` | `'self' 'wasm-unsafe-eval'` | Scripts locaux + instanciation WebAssembly — **`'wasm-unsafe-eval'` requis par Blazor WASM** |
| `style-src` | `'self' 'unsafe-inline'` | CSS locaux + styles inline injectés par le runtime Blazor *(durcissement à étudier — voir plus bas)* |
| `font-src` | `'self'` | Fonts self-hosted uniquement (`wwwroot/fonts/`) — aucune requête externe |
| `img-src` | `'self' data:` | Images locales + data URIs (placeholder) |
| `connect-src` | `'self'` | `fetch`/XHR local uniquement (JSON data, articles MD du blog) |
| `frame-src` | `'none'` | Aucun iframe chargé par le site |
| `object-src` | `'none'` | Pas de plugins |
| `base-uri` | `'self'` | Empêche le détournement de `<base>` |
| `form-action` | `'none'` | Aucun formulaire — à revoir si un formulaire de contact est ajouté |
| `upgrade-insecure-requests` | — | Force HTTPS sur toutes les requêtes |

---

## Directives NON supportées en `<meta>` (header HTTP uniquement)

Ces directives DOIVENT être absentes de la balise `<meta>` (le navigateur les **ignore et émet une erreur console**) et présentes uniquement dans `_headers` :

| Directive / Header | Statut en `<meta>` | Couverture |
|--------------------|--------------------|------------|
| `frame-ancestors` | ❌ ignoré (erreur console) | `_headers` (`frame-ancestors` + `X-Frame-Options: DENY`) |
| `X-Frame-Options` | ❌ non supporté | `_headers` |
| `X-Content-Type-Options` | ❌ non supporté | `_headers` |
| `Strict-Transport-Security` | ❌ non supporté | GitHub Pages (natif HTTPS) + `_headers` |
| `Permissions-Policy` | ❌ non supporté | `_headers` |
| `Referrer-Policy` | ✅ supporté (`<meta name="referrer">`) | `index.html` + `_headers` |

> **Conséquence à connaître** : tant que le site est servi par GitHub Pages, la protection anti-clickjacking (`frame-ancestors` / `X-Frame-Options`) **n'est pas active**. Elle ne le sera qu'après migration Cloudflare.

---

## Règles de cohérence

- La CSP de `index.html` (`<meta>`) et celle de `_headers` DOIVENT rester **synchronisées** sur les directives communes. La seule divergence autorisée : `_headers` ajoute `frame-ancestors` (non valide en `<meta>`).
- `_headers` DOIT porter en tête un commentaire rappelant qu'il est **inerte sur GitHub Pages**.
- `index.html` DOIT conserver `<base href="/" />` (requis par le routing Blazor) — `base-uri 'self'` n'interdit pas la balise légitime, seulement son détournement.

---

## Interdits absolus

- ❌ **Aucune ressource externe** : pas de Google Fonts, pas de CDN, pas de script tiers. Tout est self-hosted (`fonts/`, `css/`, `js/`).
- ❌ **Aucun script inline** dans `index.html` ou les `.razor`. Tout appel JS passe par `IJSRuntime` + modules ES (`wwwroot/js/`).
- ❌ **Aucun style inline** dans `index.html` ni les `.razor` (y compris le splash de chargement → classe `.app-loading` dans `theme.css`).
- ❌ **Aucun secret / donnée personnelle sensible** dans `wwwroot/` (data JSON inclus).

---

## Durcissement différé (backlog QA — Phase 09)

- `style-src 'unsafe-inline'` : à retirer **uniquement** si l'app ne génère plus aucun style inline au runtime. Blazor PEUT en injecter — tout retrait DOIT être validé via `ui-verifier` (0 erreur console + rendu intact) avant d'être acté.

---

## Vérification — statique ET runtime (obligatoirement les deux)

La CSP ne se valide pas qu'en lisant les fichiers. Une CSP « correcte » sur le papier peut **casser le runtime** (WASM, fonts, JSInterop) — invisible en statique.

**Statique** (`security-reviewer`) — cohérence et sévérité :
- `default-src 'self'` présent · `script-src` contient `'wasm-unsafe-eval'` · pas de ressource externe · pas de secret · cohérence `index.html` ↔ `_headers`.

**Runtime** (`ui-verifier`) — la CSP ne casse rien :

| Point | Attendu |
|-------|---------|
| Boot Blazor sous CSP | `chargement...` disparaît, app rendue |
| Erreurs console | **0** violation CSP (`Refused to…`, `frame-ancestors ignored`, etc.) |
| Requêtes réseau | **100 % `'self'`** — aucune URL externe, aucune requête bloquée |
| Toggle dark/light (JSInterop + `localStorage`) | fonctionne (clé `nabil-portfolio-theme`) |
| Fonts self-hosted | chargées (`font-src 'self'`), aucune requête `googleapis`/`gstatic` |
| Données dynamiques | `data/*.json` et `posts/*` chargés (`connect-src 'self'`) |

**Déclencheur** : toute modification de la CSP, de `index.html`, ou d'un fichier de `wwwroot/` susceptible d'affecter le chargement de ressources DOIT déclencher **les deux** vérifications.
