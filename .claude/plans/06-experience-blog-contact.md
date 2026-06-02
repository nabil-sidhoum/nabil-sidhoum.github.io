# Phase 06 — Sections Experience + Blog + Contact + Footer

## Objectif

Implémenter les trois dernières sections de la page principale : l'expérience en format git log, le blog placeholder, et le bloc contact avec CTAs. Finaliser l'assemblage de `Index.razor`.

## Prérequis

- Phase 05 terminée (Stack et Projects fonctionnels)
- Fichier `Portfolio.html` de référence — sections EXPERIENCE (lignes 1139-1169), BLOG (lignes 1176-1197), CONTACT (lignes 1204-1232)

## Composants à créer

### 6.1 — `Components/Sections/ExperienceSection.razor`

Format git log — trois commits, trois postes.

Éléments :
1. H2 : `## experience — git log --oneline`
2. Comment : `// Trois postes. Onze ans. Tous .NET.`
3. Bloc git-log : container avec `bg-elev`, 3 lignes de commit

Chaque commit est une grid 3 colonnes :
- Hash (accent) : `a8f3c12` / `d7b2901` / `3f1e44a`
- When (muted) : `2021 → now` / `2017 → 2021` / `2015 → 2017`
- Message : **rôle** `@` entreprise — description courte

Le premier commit a le suffixe `(HEAD)` après le hash en muted.
Le `now` dans la date du premier commit est en vert `#22c55e`.

Données :

```
a8f3c12 (HEAD)  2021 → now   Tech Lead .NET @ IRBIS Finance — Paris 08, fintech / wealthtech
d7b2901         2017 → 2021  Lead Développeur .NET @ SoftFluent — éditeur logiciel, .NET tooling
3f1e44a         2015 → 2017  Consultant .NET @ Capgemini — grands comptes, missions techniques
```

Le `ExperienceSection.razor.css` reprend les styles du `Portfolio.html` lignes 583-624.

Responsive mobile (≤767px) :
- La grid passe en `grid-template-areas` : hash + date sur la première ligne, message sur la deuxième
- Font-size réduit à 13px
- `.loc` passe en `display: block` (retour à la ligne)

### 6.2 — `Components/Sections/BlogSection.razor`

Section placeholder — structure prête, contenu vide.

Éléments :
1. H2 : `## blog — articles à venir`
2. Comment : `// Drafts en cours. La grille est prête, les articles arriveront ici.`
3. Grid 3 colonnes avec 3 ghost cards

Chaque ghost card :
- `// TODO[n]` en accent (11px)
- "Article à venir" en muted (14px)
- `// 0000-00-00` en dim (11px)
- Border dashed, `min-height: 180px`

Le `BlogSection.razor.css` reprend les styles du `Portfolio.html` lignes 629-658.

Responsive mobile (≤767px) :
- Grid passe en `1fr` (cards empilées)
- `min-height` réduit à 120px

**Note** : cette section sera remplacée par du contenu dynamique en Phase 07 quand le routing blog sera en place. Pour l'instant, c'est un placeholder statique.

### 6.3 — `Components/Sections/ContactSection.razor`

Éléments :
1. H2 : `## contact — pour parler poste, mission, ou code`
2. Bloc contact : container `bg-elev` avec :
   - Lead : "CV disponible immédiatement."
   - Lead comment : `// Postes Tech Lead .NET / Staff Engineer — Paris, remote France. Pas d'ESN.`
   - 4 boutons : Télécharger le CV (primary) + GitHub (outline) + LinkedIn (outline) + Email (outline)

**Corrections par rapport au Portfolio.html** :
- Ajouter LinkedIn (absent dans l'original)
- Remplacer le `href` email Cloudflare (`/cdn-cgi/l/email-protection#...`) par un vrai `mailto:` — Nabil doit fournir l'adresse email ou choisir de ne pas la mettre

Les icônes SVG (download, GitHub, LinkedIn, email) sont inline — mêmes icônes que dans le hero pour la cohérence.

Le `ContactSection.razor.css` reprend les styles du `Portfolio.html` lignes 664-687.

Responsive mobile (≤767px) :
- Les boutons passent en `flex-direction: column` avec `width: 100%`

### 6.4 — Finaliser `Pages/Index.razor`

Assemblage complet de toutes les sections :

```razor
@page "/"

<PathStrip />

<HeroSection />
<div class="divider" aria-hidden="true">//</div>

<AboutSection />
<div class="divider" aria-hidden="true">//</div>

<StackSection />
<div class="divider" aria-hidden="true">//</div>

<ProjectsSection />
<div class="divider" aria-hidden="true">//</div>

<ExperienceSection />
<div class="divider" aria-hidden="true">//</div>

<BlogSection />
<div class="divider" aria-hidden="true">//</div>

<ContactSection />

<Footer />
```

### 6.5 — Vérifier le scroll-spy de la nav

Les ancres `#about`, `#stack`, `#projets`, `#experience`, `#blog`, `#contact` doivent correspondre aux `id` des `<section>` dans chaque composant.

Ajouter `scroll-margin-top: 80px` sur les sections (dans `theme.css` ou en global) pour compenser la nav sticky :

```css
section { scroll-margin-top: 80px; }
```

Le scroll-spy (highlight du lien actif dans la nav pendant le scroll) nécessite un `IntersectionObserver` en JS. Créer `wwwroot/js/scroll-spy.js` :

```javascript
export function initScrollSpy(sectionIds, dotnetHelper) {
    const observer = new IntersectionObserver((entries) => {
        for (const entry of entries) {
            if (entry.isIntersecting) {
                dotnetHelper.invokeMethodAsync('OnSectionVisible', entry.target.id);
            }
        }
    }, { rootMargin: '-40% 0px -55% 0px', threshold: 0 });

    for (const id of sectionIds) {
        const el = document.getElementById(id);
        if (el) observer.observe(el);
    }

    return observer;
}
```

Le composant `Nav.razor` charge ce module et met à jour la classe `active` sur les liens.

## Validation

```bash
dotnet run
```

Vérifier :
- La section experience affiche 3 commits en format git log
- Le premier commit a `(HEAD)` et `now` en vert
- Sur mobile, hash+date au-dessus, message en dessous
- La section blog affiche 3 ghost cards avec `// TODO[1-3]`
- Sur mobile, les ghost cards s'empilent en colonne
- La section contact affiche 4 boutons (CV + GitHub + LinkedIn + Email)
- Sur mobile, les boutons s'empilent en full width
- Le footer affiche `// EOF` + copyright + liens
- Le scroll-spy met en surbrillance le lien de nav correspondant à la section visible
- Toutes les ancres de la nav scrollent vers la bonne section
- La page complète est scrollable sans problème de layout

## Commit

```
feat: ajouter les sections Experience, Blog placeholder et Contact

- ExperienceSection en format git log (3 commits)
- BlogSection placeholder avec 3 ghost cards TODO
- ContactSection avec CTAs (CV, GitHub, LinkedIn, Email)
- scroll-spy sur la navigation via IntersectionObserver
- assemblage complet de Index.razor
```

## Checklist avant de passer à la phase 07

- [ ] Les 7 sections s'affichent dans l'ordre correct avec dividers
- [ ] Le git log est lisible sur desktop et mobile
- [ ] Les ghost cards blog ont le bon style (dashed, muted)
- [ ] Le contact a 4 boutons fonctionnels
- [ ] Le scroll-spy fonctionne dans la nav
- [ ] Le footer est en place
- [ ] Le build compile sans erreur
- [ ] **Comparaison visuelle** : ouvrir le Portfolio.html de référence côte à côte avec le site Blazor et vérifier la correspondance
