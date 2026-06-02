# Phase 02 — Design system : tokens CSS + fonts self-hosted

## Objectif

Créer le design system portable du portfolio : variables CSS (dark/light), fonts self-hosted, classes utilitaires partagées. Ce fichier est la fondation — toutes les phases suivantes en dépendent.

## Prérequis

- Phase 01 terminée (build compile, structure en place)
- Fichier `Portfolio.html` de référence accessible

## Étapes

### 2.1 — Télécharger et self-host les fonts

Les deux familles de polices utilisées :
- **JetBrains Mono** (headings, code, monospace) — licence OFL, téléchargeable sur https://www.jetbrains.com/lp/mono/
- **IBM Plex Sans** (body) — licence OFL, téléchargeable sur https://github.com/IBM/plex

Télécharger uniquement les fichiers `.woff2` (format le plus performant, supporté par tous les navigateurs modernes).

Variantes nécessaires :
- JetBrains Mono : Regular (400), Medium (500), SemiBold (600), Bold (700)
- IBM Plex Sans : Regular (400), Medium (500), SemiBold (600), Bold (700)

Placer dans `wwwroot/fonts/` :

```
wwwroot/fonts/
├── jetbrains-mono-400.woff2
├── jetbrains-mono-500.woff2
├── jetbrains-mono-600.woff2
├── jetbrains-mono-700.woff2
├── ibm-plex-sans-400.woff2
├── ibm-plex-sans-500.woff2
├── ibm-plex-sans-600.woff2
└── ibm-plex-sans-700.woff2
```

### 2.2 — Créer `wwwroot/css/theme.css`

Ce fichier contient UNIQUEMENT :
- Les `@font-face`
- Les variables CSS (tokens dark + light)
- Le reset/base (body, liens, selection)
- Les classes utilitaires réutilisées par plusieurs composants

Structure du fichier :

```css
/* ============================================================
   FONT FACES — self-hosted, aucune dépendance externe
   ============================================================ */

@font-face {
  font-family: "JetBrains Mono";
  src: url("/fonts/jetbrains-mono-400.woff2") format("woff2");
  font-weight: 400;
  font-style: normal;
  font-display: swap;
}
/* ... répéter pour chaque variante ... */

@font-face {
  font-family: "IBM Plex Sans";
  src: url("/fonts/ibm-plex-sans-400.woff2") format("woff2");
  font-weight: 400;
  font-style: normal;
  font-display: swap;
}
/* ... répéter pour chaque variante ... */


/* ============================================================
   TOKENS DARK (défaut)
   ============================================================ */

:root[data-theme="dark"] {
  --bg:          #0d0d0f;
  --bg-elev:     #141418;
  --fg:          #e5e5e5;
  --fg-strong:   #fafafa;
  --fg-muted:    #9ca3af;    /* CORRIGÉ — était #6b7280, insuffisant en contraste */
  --fg-dim:      #6b7280;    /* réservé aux éléments décoratifs uniquement */
  --rule:        #1e1e22;
  --rule-strong: #2a2a30;
  --accent:      #3b82f6;
  --accent-soft: rgba(59, 130, 246, 0.12);
  --accent-line: rgba(59, 130, 246, 0.32);
  --selection:   rgba(59, 130, 246, 0.35);

  --shadow-nav:  0 1px 0 var(--rule), 0 8px 24px -16px rgba(0, 0, 0, 0.6);
}


/* ============================================================
   TOKENS LIGHT
   ============================================================ */

:root[data-theme="light"] {
  --bg:          #fafaf9;
  --bg-elev:     #f2f1ef;
  --fg:          #1a1a1a;
  --fg-strong:   #050505;
  --fg-muted:    #6b7280;
  --fg-dim:      #9ca3af;
  --rule:        #e5e5e5;
  --rule-strong: #d4d4d4;
  --accent:      #2563eb;
  --accent-soft: rgba(59, 130, 246, 0.10);
  --accent-line: rgba(37, 99, 235, 0.32);
  --selection:   rgba(59, 130, 246, 0.25);

  --shadow-nav:  0 1px 0 var(--rule), 0 8px 24px -16px rgba(0, 0, 0, 0.10);
}


/* ============================================================
   RESET + BASE
   ============================================================ */

*, *::before, *::after { box-sizing: border-box; }
html, body { margin: 0; padding: 0; }
html {
  -webkit-text-size-adjust: 100%;
  scroll-behavior: smooth;
}

body {
  background: var(--bg);
  color: var(--fg);
  font-family: "IBM Plex Sans", system-ui, -apple-system, "Segoe UI", sans-serif;
  font-size: 16px;
  line-height: 1.6;
  -webkit-font-smoothing: antialiased;
  text-rendering: optimizeLegibility;
  transition: background-color 0.2s ease, color 0.2s ease;
}

::selection {
  background: var(--selection);
  color: var(--fg-strong);
}

a { color: var(--accent); text-decoration: none; }
a:hover { text-decoration: underline; text-underline-offset: 3px; }


/* ============================================================
   CLASSES UTILITAIRES PARTAGÉES
   ============================================================ */

/* Monospace shortcut — utilisé dans plusieurs composants */
.mono {
  font-family: "JetBrains Mono", ui-monospace, "SF Mono", Menlo, Consolas, monospace;
}

/* Commentaire style code — utilisé dans hero, about, stack, contact */
.comment {
  font-family: "JetBrains Mono", ui-monospace, monospace;
  color: var(--fg-muted);
  font-size: 13.5px;
  line-height: 1.65;
}
.comment::before { content: "// "; color: var(--fg-dim); }

/* Séparateur entre sections */
.divider {
  color: var(--fg-dim);
  font-family: "JetBrains Mono", ui-monospace, monospace;
  font-size: 13px;
  margin: 72px 0 48px;
  overflow: hidden;
  white-space: nowrap;
  user-select: none;
}
.divider::after {
  content: " ────────────────────────────────────────────────────────────────────";
  color: var(--fg-dim);
}

/* Pill / tag — utilisé dans stack et projects */
.pill {
  display: inline-flex;
  align-items: center;
  padding: 2px 9px;
  border-radius: 4px;
  border: 1px solid var(--rule-strong);
  background: var(--bg);
  color: var(--fg);
  font-family: "JetBrains Mono", ui-monospace, monospace;
  font-size: 11.5px;
  line-height: 1.6;
  white-space: nowrap;
}
.pill.accent {
  border-color: var(--accent-line);
  color: var(--accent);
  background: var(--accent-soft);
}

/* Boutons — utilisés dans hero, nav, contact */
.btn {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  font-family: "JetBrains Mono", ui-monospace, monospace;
  font-size: 13px;
  font-weight: 500;
  padding: 8px 14px;
  border-radius: 8px;
  border: 1px solid transparent;
  cursor: pointer;
  text-decoration: none;
  transition: background-color 0.15s ease, border-color 0.15s ease, color 0.15s ease;
  white-space: nowrap;
}
.btn:hover { text-decoration: none; }
.btn:active { transform: translateY(1px); }
.btn svg { width: 14px; height: 14px; flex-shrink: 0; }

.btn-primary {
  background: var(--accent);
  color: #fff;
  border-color: var(--accent);
}
.btn-primary:hover {
  background: color-mix(in oklab, var(--accent) 88%, white);
}

.btn-outline {
  background: transparent;
  color: var(--fg);
  border-color: var(--rule-strong);
}
.btn-outline:hover {
  border-color: var(--fg-muted);
  background: var(--bg-elev);
}

/* Heading style README — ## avant le titre */
.h-readme {
  font-family: "JetBrains Mono", ui-monospace, monospace;
  font-weight: 600;
  letter-spacing: -0.4px;
  margin: 0 0 20px;
  color: var(--fg-strong);
  display: flex;
  align-items: baseline;
  gap: 12px;
}
.h-readme::before {
  content: "##";
  color: var(--accent);
  font-weight: 400;
  flex-shrink: 0;
}
.h-readme .sub {
  font-weight: 400;
  font-size: 13px;
  color: var(--fg-muted);
  letter-spacing: 0;
}

/* Badge — utilisé dans projects */
.badge {
  font-family: "JetBrains Mono", ui-monospace, monospace;
  font-size: 10px;
  font-weight: 500;
  letter-spacing: 1px;
  padding: 2px 7px;
  border-radius: 4px;
  border: 1px solid var(--rule-strong);
  color: var(--fg-muted);
  text-transform: uppercase;
}
.badge.open {
  border-color: var(--accent-line);
  color: var(--accent);
  background: var(--accent-soft);
}
.badge.show {
  border-style: dashed;
  color: var(--fg-muted);
}


/* ============================================================
   RESPONSIVE — breakpoints globaux
   ============================================================ */

@media (max-width: 767px) {
  .divider { margin: 56px 0 36px; }
  .h-readme { flex-wrap: wrap; }
}

/* Animation — le seul mouvement autorisé */
@keyframes pulse {
  0%, 100% { box-shadow: 0 0 0 0 rgba(34, 197, 94, 0.45); }
  50% { box-shadow: 0 0 0 5px rgba(34, 197, 94, 0); }
}
.pulse { animation: pulse 2.4s ease-in-out infinite; }

@media (prefers-reduced-motion: reduce) {
  .pulse { animation: none; }
  html { scroll-behavior: auto; }
}

/* Print */
@media print {
  body { background: #fff; color: #000; }
}
```

### 2.3 — Référencer theme.css dans index.html

Dans `wwwroot/index.html`, remplacer les anciens liens CSS par :

```html
<link rel="stylesheet" href="css/theme.css" />
```

Ne PAS référencer Google Fonts — tout est self-hosted maintenant.

### 2.4 — Vérifier le chargement des fonts

```bash
dotnet run
```

Ouvrir dans le navigateur, inspecter l'onglet Network :
- Les fichiers `.woff2` doivent se charger depuis `/fonts/`
- Aucune requête vers `fonts.googleapis.com` ou `fonts.gstatic.com`
- Vérifier visuellement que JetBrains Mono et IBM Plex Sans s'affichent (pas de fallback system-ui)

### 2.5 — Vérifier le contraste dark mode

Ouvrir la console du navigateur, vérifier les ratios de contraste :
- `--fg` (#e5e5e5) sur `--bg` (#0d0d0f) → ratio ~17:1 ✓
- `--fg-muted` (#9ca3af) sur `--bg` (#0d0d0f) → ratio ~8.5:1 ✓ (était ~4.5:1 avec #6b7280)
- `--fg-dim` (#6b7280) sur `--bg` (#0d0d0f) → ratio ~4.5:1 — OK pour éléments décoratifs uniquement
- `--accent` (#3b82f6) sur `--bg` (#0d0d0f) → ratio ~5.5:1 ✓

## Commit

```
feat: mettre en place le design system (tokens CSS + fonts self-hosted)

- ajout des fonts JetBrains Mono et IBM Plex Sans en woff2 local
- création de theme.css avec tokens dark/light corrigés en contraste
- classes utilitaires partagées (comment, divider, pill, btn, badge, h-readme)
- suppression de toute dépendance à Google Fonts
```

## Checklist avant de passer à la phase 03

- [ ] Les fonts se chargent localement (aucune requête externe)
- [ ] Le toggle dark/light applique bien les deux palettes (même si le toggle n'existe pas encore, tester en changeant `data-theme` dans le HTML)
- [ ] Le contraste `--fg-muted` est lisible sur fond dark
- [ ] Les classes utilitaires `.comment`, `.divider`, `.pill`, `.btn`, `.badge`, `.h-readme` existent et correspondent visuellement au `Portfolio.html` de référence
- [ ] Le build compile sans erreur
