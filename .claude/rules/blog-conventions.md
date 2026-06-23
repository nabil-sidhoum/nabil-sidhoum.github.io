# Conventions Blog — articles & diagrammes

> Source de vérité pour la rédaction d'un article de blog et de ses illustrations. Le blog est 100 % statique : articles Markdown + index JSON dans `wwwroot/posts/`, rendus par `BlogService` (Markdig) et affichés par `Blog.razor` / `BlogPost.razor` / `BlogSection.razor`.

## Emplacement des fichiers

| Élément | Chemin |
|---------|--------|
| Index des articles | `wwwroot/posts/index.json` |
| Contenu d'un article | `wwwroot/posts/{slug}.md` |
| Diagrammes / images | `wwwroot/posts/assets/{nom}.svg` |

---

## Créer un article — procédure

1. Choisir un **slug** conforme au pattern `^[a-z0-9-]+$` (lettres minuscules, chiffres, tirets). Toute autre forme est **rejetée** par `BlogService` (le slug est injecté dans un chemin de fichier → protection anti path-traversal).
2. Créer `wwwroot/posts/{slug}.md` (voir structure ci-dessous).
3. Ajouter l'entrée correspondante **en tête** du tableau `articles` de `index.json` (voir ordre ci-dessous).
4. Placer les éventuels diagrammes dans `wwwroot/posts/assets/` et les référencer en chemin **absolu** (`/posts/assets/...`).
5. Vérifier le rendu (build + `ui-verifier` en thème sombre **et** clair).

---

## `index.json` — schéma et ordre

```json
{
  "Slug": "mon-article",
  "Title": "Titre complet de l'article",
  "Summary": "Résumé affiché sur la carte du blog et la section accueil.",
  "PublishedAt": "2026-06-23",
  "Tags": [ ".NET", "IA" ]
}
```

| Champ | Format |
|-------|--------|
| `Slug` | `^[a-z0-9-]+$`, identique au nom du fichier `.md` |
| `Title` | Titre complet, identique au `# H1` du Markdown |
| `Summary` | 1 à 3 phrases, affichées sur la carte (liste + accueil) |
| `PublishedAt` | `YYYY-MM-DD` |
| `Tags` | Liste courte de libellés |

**Ordre = chronologie inverse manuelle.** Ni `Blog.razor` ni `BlogSection.razor` ne trient par date : ils consomment les articles **dans l'ordre du JSON** (l'accueil n'affiche que les 3 premiers via `Take(3)`). Le **nouvel article se place donc toujours en tête** du tableau.

---

## Structure du fichier `.md`

```markdown
# Titre complet de l'article

*Publié le 23 juin 2026 — Nabil Sidhoum, Senior .NET Tech Lead*

---

Chapô introductif…

## Première section

…

---

*Nabil Sidhoum — Senior .NET Tech Lead, Paris. …*

*GitHub : [nabil-sidhoum](https://github.com/nabil-sidhoum) — Portfolio : [nabil-sidhoum.github.io](https://nabil-sidhoum.github.io)*
```

- `# H1` unique en première ligne, identique au `Title` de l'index.
- Ligne de publication en italique : `*Publié le {date longue en français} — Nabil Sidhoum, Senior .NET Tech Lead*`.
- Séparateurs de section : `---` (rendus en `<hr>`).
- Pied de page : bio + ligne GitHub/Portfolio, séparateurs **tiret cadratin `—`** (cohérence entre articles — ne pas mélanger avec `·`).

### Limites du rendu Markdown (Markdig)

`BlogService` configure le pipeline avec `DisableHtml()` + `UsePipeTables()` :

- **Aucun HTML brut** dans le `.md` : il est neutralisé (sécurité / cohérence CSP). Tout passe par la syntaxe Markdown.
- **Tableaux GFM** (`| … | … |`) supportés.
- Images : syntaxe Markdown standard `![alt](/posts/assets/...)` — jamais de balise `<img>`.

---

## Diagrammes SVG — theming sombre & clair

Un SVG inséré via Markdown est chargé en `<img>` : c'est un **document isolé**. Il ne peut **pas** lire les variables CSS du site, ni `currentColor`, et le thème du site bascule par **toggle manuel** (pas via `prefers-color-scheme`). Conséquence : **un SVG ne peut pas suivre le thème**. On vise donc un SVG **neutre, lisible sur les deux fonds** (`--bg` sombre `#0d0d0f` et clair `#fafaf9`).

### Règles

- **Fond transparent.** Pas de `<rect>` de fond opaque couvrant le canvas — le fond de la page doit transparaître (cohérent entre tous les SVG du blog).
- **Le contenu coloré porte son propre fond.** Boîtes, bandeaux et nœuds ont un `fill` → le texte clair qu'ils contiennent reste lisible sur n'importe quel fond de page.
- **Texte / traits posés sur le fond transparent** : couleurs neutres lisibles en sombre **et** en clair :
  - titres → accent `#3b82f6` ;
  - texte secondaire et connecteurs (flèches, lignes) → gris `#6b7280` (`--fg-dim`).
  - ❌ à proscrire sur fond transparent : quasi-blanc (`#f1f5f9`, invisible en clair), quasi-noir (invisible en sombre), gris trop pâles (`#94a3b8`, illisibles en clair).
- **Polices** : `'IBM Plex Sans'` (texte) et `'JetBrains Mono'` (code/identifiants) — les fonts self-hosted du site. Jamais `Segoe UI` ni police système en premier choix.
- **Accent** : `#3b82f6` (token `--accent`). Éviter les variantes hors palette (`#38bdf8`, etc.).
- **Accessibilité** : `role="img"` + `aria-label`, et un `<desc>` décrivant le diagramme.
- **CSP** : aucune ressource externe (police Google, image distante…). Tout est self-hosted.

---

## Vérification

Toute modification d'un article ou d'un SVG DOIT être contrôlée via `ui-verifier`, **dans les deux thèmes** :

| Point | Attendu |
|-------|---------|
| Article en tête de `/blog` | présent, résumé + tags affichés |
| Rendu Markdown | titre, sections, tableaux GFM, code, citations OK |
| SVG chargé | self-hosted (`/posts/assets/...`), aucune requête externe |
| SVG lisible en **sombre** | titre, sous-titre, flèches, cartes lisibles |
| SVG lisible en **clair** | idem après toggle — aucun élément confondu avec le fond |
| Console | 0 erreur, 0 violation CSP |
