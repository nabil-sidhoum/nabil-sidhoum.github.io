# Phase 03 — Layout principal + navigation + menu mobile

## Objectif

Créer le `MainLayout.razor` avec la navigation sticky (emprunt Direction B), le toggle dark/light, le bouton CV persistant, et le menu burger mobile avec overlay full-screen.

## Prérequis

- Phase 02 terminée (theme.css en place, fonts chargées)
- Fichier `Portfolio.html` de référence — sections NAV et mobile menu (lignes 815-896)

## Composants à créer

### 3.1 — `Layout/MainLayout.razor`

Structure :

```razor
@inherits LayoutComponentBase

<Nav />
<MobileMenu />

<main class="doc" id="top">
    @Body
</main>
```

Le `MainLayout.razor.css` contient :

```css
.doc {
    max-width: 900px;
    margin: 0 auto;
    padding: 64px 24px 120px;
}

@media (max-width: 1023px) {
    .doc { padding: 48px 22px 96px; }
}

@media (max-width: 767px) {
    .doc { padding: 32px 18px 80px; }
}
```

### 3.2 — `Components/Nav/Nav.razor`

Éléments :
- Brand : `~/nabil-sidhoum/README.md` en monospace (lien vers `#top`)
- Nav links : about · stack · projets · experience · blog · contact (ancres `#section`)
- Nav actions : ThemeToggle + bouton CV download (accent, `btn-primary`) + burger (mobile only)

Comportement :
- `position: sticky; top: 0; z-index: 50;`
- Background semi-transparent avec `backdrop-filter: blur(14px)`
- Les liens de nav sont masqués sous 767px (`display: none`)
- Le burger est masqué au-dessus de 767px

Le `Nav.razor.css` contient tous les styles de la nav (`.nav`, `.nav-inner`, `.brand`, `.nav-links`, `.nav-actions`, `.burger`). Reprendre fidèlement les styles du `Portfolio.html` lignes 81-177.

Attention : le lien "blog" doit pointer vers `/blog` (route Blazor) et non `#blog`, puisque le blog sera une page séparée (Phase 07). Tous les autres liens restent des ancres `#section`.

### 3.3 — `Components/Nav/MobileMenu.razor`

Éléments :
- Overlay full-screen (`position: fixed; inset: 0;`)
- Header avec brand + bouton close
- Liens numérotés (01 about, 02 stack, etc.)
- CTA "Télécharger le CV" en bas

Comportement :
- Masqué par défaut (`display: none`)
- Ouvert/fermé via un `bool IsOpen` géré par un service ou un `EventCallback`
- Fermeture sur clic d'un lien, clic du bouton close, ou touche Escape
- `document.body.style.overflow = 'hidden'` quand ouvert → nécessite `IJSRuntime`

Le `MobileMenu.razor.css` contient tous les styles du menu mobile. Reprendre les styles du `Portfolio.html` lignes 180-228.

### 3.4 — `Components/Nav/ThemeToggle.razor`

Éléments :
- Bouton 34x34px avec icône soleil (dark) / lune (light)
- Toggle entre `data-theme="dark"` et `data-theme="light"` sur `<html>`

Comportement :
- Lire le thème sauvegardé dans `localStorage` au démarrage (`OnAfterRenderAsync`)
- Sauvegarder le choix dans `localStorage` à chaque toggle
- Utiliser `IJSRuntime` pour accéder à `localStorage` et modifier l'attribut `data-theme`

Fichier JS interop nécessaire — créer `wwwroot/js/theme.js` :

```javascript
export function getTheme() {
    return localStorage.getItem('nabil-portfolio-theme') || 'dark';
}

export function setTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('nabil-portfolio-theme', theme);
}
```

Le `ThemeToggle.razor` charge ce module via `IJSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/theme.js")`.

Le `ThemeToggle.razor.css` contient les styles du bouton toggle. Reprendre les styles du `Portfolio.html` lignes 122-135.

### 3.5 — Communication entre Nav et MobileMenu

Deux approches possibles :

**Option A — Cascading parameter (simple)** :
Le `MainLayout` gère un `bool mobileMenuOpen` et le passe en `CascadingValue` aux enfants.

**Option B — Service injecté (plus propre pour la suite)** :
Créer un `Services/UIStateService.cs` :

```csharp
using System;

public sealed class UIStateService
{
    public bool IsMobileMenuOpen { get; private set; }

    public event Action? OnChange;

    public void OpenMobileMenu()
    {
        IsMobileMenuOpen = true;
        OnChange?.Invoke();
    }

    public void CloseMobileMenu()
    {
        IsMobileMenuOpen = false;
        OnChange?.Invoke();
    }
}
```

Enregistré en `Scoped` dans `Program.cs`. Les composants `Nav` et `MobileMenu` l'injectent et réagissent.

Recommandation : Option B — plus propre, et le service pourra gérer d'autres états UI plus tard si besoin.

### 3.6 — PathStrip (breadcrumb)

Créer `Components/Shared/PathStrip.razor` :

Affiche : `~/ nabil-sidhoum/ README.md` avec metadata `● disponible juillet 2026 · Paris / remote FR`

Le `PathStrip.razor.css` contient les styles du breadcrumb. Reprendre les styles du `Portfolio.html` lignes 240-262.

**Correction** : remplacer `dispo Q1 2026` par `disponible juillet 2026`.

### 3.7 — Footer

Créer `Components/Shared/Footer.razor` :

Affiche : `// EOF ───...` + ligne `© 2026 · Nabil Sidhoum` avec liens GitHub et LinkedIn.

Le `Footer.razor.css` reprend les styles lignes 692-721 du `Portfolio.html`.

## Validation

```bash
dotnet run
```

Vérifier :
- La nav est sticky et suit le scroll
- Le bouton CV est visible et cliquable (même si le PDF n'existe pas encore)
- Le toggle dark/light fonctionne et persiste après rechargement
- Le burger apparaît sous 767px
- Le menu mobile s'ouvre, se ferme, et les liens scrollent/naviguent correctement
- Le PathStrip affiche "disponible juillet 2026" (pas "Q1 2026")
- Le footer affiche les liens GitHub et LinkedIn

## Commit

```
feat: ajouter le layout principal avec navigation sticky et menu mobile

- MainLayout avec nav sticky, blur background, bouton CV persistant
- ThemeToggle dark/light avec persistance localStorage via JSInterop
- MobileMenu overlay full-screen avec fermeture Escape
- UIStateService pour la communication entre composants
- PathStrip breadcrumb et Footer
```

## Checklist avant de passer à la phase 04

- [ ] La nav est sticky avec backdrop-filter
- [ ] Le bouton CV est visible en permanence dans la nav
- [ ] Le toggle dark/light fonctionne et persiste
- [ ] Le burger apparaît sous 767px et ouvre le menu mobile
- [ ] Le menu mobile se ferme sur clic lien / bouton close / Escape
- [ ] Le PathStrip affiche "disponible juillet 2026"
- [ ] Le footer affiche GitHub + LinkedIn
- [ ] Le build compile sans erreur
