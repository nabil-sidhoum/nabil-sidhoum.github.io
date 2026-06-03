# Phase 09 — Polish : background pattern, contraste, responsive QA

## Objectif

Passer le portfolio en revue visuelle finale. Ajouter le background pattern subtil, corriger les derniers problèmes de contraste, et faire un QA responsive systématique sur les 3 breakpoints.

## Prérequis

- Phase 08 terminée (CSP en place, aucune erreur console)

## Étapes

### 9.1 — Background pattern subtil

Ajouter un pattern de grille de points très atténué sur le `<body>`. L'objectif est de donner de la texture au fond sans distraire.

Dans `theme.css`, ajouter au `body` :

```css
body {
    /* ... styles existants ... */
    background-image: radial-gradient(circle, var(--rule) 1px, transparent 1px);
    background-size: 24px 24px;
    background-attachment: fixed;
}
```

Points critiques :
- En dark mode, `--rule` (#1e1e22) sur `--bg` (#0d0d0f) donne un contraste très faible — c'est voulu, les points doivent être quasi-invisibles
- En light mode, `--rule` (#e5e5e5) sur `--bg` (#fafaf9) — même effet subtil
- `background-attachment: fixed` empêche le pattern de scroller avec le contenu (feel plus élégant)
- Si la performance est impactée sur mobile (rare), remplacer `fixed` par `scroll`

Alternative si les points ne plaisent pas — grille de lignes :

```css
body {
    background-image:
        linear-gradient(var(--rule) 1px, transparent 1px),
        linear-gradient(90deg, var(--rule) 1px, transparent 1px);
    background-size: 24px 24px;
}
```

Tester les deux et garder celui qui correspond le mieux à l'esthétique README.

**Important — fallback mobile** : `background-attachment: fixed` n'est pas accéléré GPU sur iOS Safari. Le scroll peut déclencher un lag visible (~50-200 ms par frame) sur iPhone. Ajouter systématiquement dans le même bloc :

```css
@media (max-width: 767px) {
  body { background-attachment: scroll; }
}
```

### 9.2 — Audit de contraste dark mode

Ouvrir le portfolio en dark mode et vérifier systématiquement chaque type de texte :

| Élément | Variable | Couleur | Fond | Ratio attendu | Action |
|---------|----------|---------|------|----------------|--------|
| Texte principal | `--fg` | #e5e5e5 | #0d0d0f | ~17:1 | OK — ne pas toucher |
| Titres | `--fg-strong` | #fafafa | #0d0d0f | ~19:1 | OK |
| Texte secondaire (commentaires, dates) | `--fg-muted` | #9ca3af | #0d0d0f | ~8.5:1 | OK — corrigé en Phase 02 |
| Éléments décoratifs (séparateurs, hashes) | `--fg-dim` | #6b7280 | #0d0d0f | ~4.5:1 | OK pour déco |
| Accent (liens, badges) | `--accent` | #3b82f6 | #0d0d0f | ~5.5:1 | OK |
| Pills texte | `--fg` | #e5e5e5 | #141418 (bg-elev) | ~15:1 | OK |
| Comment `//` prefix | `--fg-dim` | #6b7280 | #0d0d0f | ~4.5:1 | Acceptable (décoratif) |
| Nav links | `--fg-muted` | #9ca3af | nav blur bg | Vérifier | Tester visuellement |

Outil recommandé : extension Chrome "WCAG Color Contrast Checker" ou le contrast checker intégré dans Chrome DevTools (Inspect → Computed → color).

Les éléments à surveiller particulièrement :
- Le texte dans les pills sur fond `--bg-elev` (#141418)
- Les liens dans la nav sur le fond blur
- Le texte des ghost cards blog (muted sur bg)
- Les dates dans le git log

### 9.3 — QA responsive systématique

Tester sur 3 largeurs dans Chrome DevTools (Ctrl+Shift+M) :

**Desktop (1280px)**
- [ ] Contenu centré, max-width ~900px respecté
- [ ] Nav : tous les liens visibles + toggle + bouton CV
- [ ] Hero : CTAs côte à côte, curl visible
- [ ] Stack : grid 90px + pills sur une ligne
- [ ] Projects : cards en colonne, header grid lisible
- [ ] Experience : git log 3 colonnes alignées
- [ ] Blog : 3 ghost cards en row
- [ ] Contact : 4 boutons en row

**Tablet (768px)**
- [ ] Nav : liens visibles ou burger selon le breakpoint choisi
- [ ] Hero : CTAs possiblement serrés — vérifier le wrap
- [ ] Stack : vérifier que les pills wrappent proprement
- [ ] Projects : cards full width
- [ ] Experience : git log lisible, pas de troncature
- [ ] Blog : ghost cards possiblement en 2+1 — vérifier le grid

**Mobile (375px)**
- [ ] Nav : burger visible, liens masqués, toggle + CV toujours visibles
- [ ] Menu mobile : overlay full-screen, liens numérotés, bouton CV en bas
- [ ] Hero : CTAs empilés full width, curl masqué, H1 taille réduite
- [ ] About : photo au-dessus du texte, réduite à 96px
- [ ] Stack : clé au-dessus des pills (grid 1fr)
- [ ] Projects : cards full width, liens full width
- [ ] Experience : hash+date au-dessus, message en dessous
- [ ] Blog : ghost cards empilées
- [ ] Contact : boutons empilés full width
- [ ] Footer : texte wrappé correctement
- [ ] Scroll : smooth, pas de débordement horizontal (`overflow-x`)
- [ ] Touch targets : tous les boutons/liens font minimum 44x44px

### 9.4 — Vérifier l'absence de scroll horizontal

Bug fréquent sur mobile — un élément qui dépasse cause un scroll horizontal sur toute la page.

Test rapide :
```css
/* Temporaire — ajouter au body pour détecter les débordements */
* { outline: 1px solid red !important; }
```

Si un scroll horizontal existe, identifier l'élément qui dépasse avec cette outline, et corriger soit avec `overflow: hidden` sur le container, soit en ajustant les marges/paddings.

Le `divider::after` avec sa longue chaîne `────────` est un candidat typique. S'assurer que le `.divider` a `overflow: hidden` (déjà prévu en Phase 02).

### 9.5 — Performance : vérifier le First Contentful Paint

Ouvrir Chrome DevTools → Lighthouse → générer un rapport Performance.

Points à vérifier :
- Les fonts en `font-display: swap` ne bloquent pas le rendu
- Le chargement WASM est le bottleneck attendu — normal pour Blazor WASM
- Pas de CSS ou JS non utilisé qui ralentit le chargement
- Les images (photo placeholder, favicon) sont optimisées

Score Lighthouse attendu réaliste pour un Blazor WASM :
- Performance : 60-80 (le WASM initial est lourd, c'est structurel)
- Accessibility : 90+
- Best Practices : 90+
- SEO : 90+

### 9.6 — Dark/Light mode : vérifier la cohérence

Basculer entre dark et light plusieurs fois et vérifier :
- [ ] Toutes les sections sont lisibles dans les deux modes
- [ ] Le pattern de fond s'adapte
- [ ] Les pills, badges, boutons changent correctement
- [ ] Les SVG (icônes) utilisent `currentColor` et suivent le thème
- [ ] La transition est fluide (0.2s ease sur background et color)
- [ ] Le ghost cards blog restent lisibles en light mode

## Commit

```
fix: polish visuel — background pattern, contraste dark mode et responsive QA

- ajout du background pattern en grille de points subtile
- vérification et correction des ratios de contraste WCAG
- correction des débordements mobile
- QA responsive sur desktop, tablet et mobile
```

## Checklist avant de passer à la phase 10

- [ ] Le background pattern est visible et subtil dans les deux modes
- [ ] Aucun problème de contraste flagrant (texte lisible partout)
- [ ] Le responsive fonctionne sur les 3 breakpoints
- [ ] Pas de scroll horizontal sur mobile
- [ ] Le dark/light toggle est cohérent sur toutes les sections
- [ ] Lighthouse Accessibility ≥ 90
- [ ] Le build compile sans erreur
- [ ] **Comparaison finale** : Portfolio.html de référence vs site Blazor — écarts acceptables identifiés
