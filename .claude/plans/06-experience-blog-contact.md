# Phase 06 — Sections Experience + Blog + Contact + Footer

## Objectif

Implémenter les trois dernières sections de la page principale : l'expérience en format git log (chargement dynamique depuis JSON), le blog placeholder, et le bloc contact avec CTAs. Finaliser l'assemblage de `Index.razor`.

## Prérequis

- Phase 05 terminée (Stack et Projects fonctionnels)
- Fichier `Portfolio.html` de référence — sections EXPERIENCE (lignes 1139-1169), BLOG (lignes 1176-1197), CONTACT (lignes 1204-1232)
- `wwwroot/data/experiences.json` à jour avec le champ `Hash` (ajouté lors de la correction du plan)

## Composants à créer

### 6.1 — `Models/ExperienceInfo.cs`

Classe POCO correspondant au schéma de `wwwroot/data/experiences.json`. Pas de record — classe avec `{ get; set; }` :

```csharp
using System.Collections.Generic;

namespace BlazorPortfolio.Client.Models
{
    public class ExperienceInfo
    {
        public string Societe { get; set; }
        public string Poste { get; set; }
        public string Lieu { get; set; }
        public string DateDebut { get; set; }
        public string DateFin { get; set; }
        public string Accroche { get; set; }
        public List<string> Descriptions { get; set; }
        public List<string> Competences { get; set; }
    }
}
```

`DateFin` est `null` quand le poste est actuel. Pas de champ `Hash` — le hash affiché est généré automatiquement dans `ExperienceSection` (voir section 6.3).

### 6.2 — `Services/ExperienceService.cs`

Charge et met en cache la liste depuis `wwwroot/data/experiences.json` :

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;

namespace BlazorPortfolio.Client.Services
{
    public sealed class ExperienceService
    {
        private readonly HttpClient _httpClient;
        private List<ExperienceInfo> _cache;

        public ExperienceService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<ExperienceInfo>> GetExperiencesAsync()
        {
            if (_cache != null)
            {
                return _cache;
            }

            _cache = await _httpClient.GetFromJsonAsync<List<ExperienceInfo>>("data/experiences.json");
            return _cache ?? [];
        }
    }
}
```

Enregistrer dans `Program.cs` après la ligne `AddScoped<ProjectService>` :

```csharp
builder.Services.AddScoped<ExperienceService>();
```

### 6.3 — `Components/Sections/ExperienceSection.razor`

Format git log — chargement dynamique depuis `ExperienceService`.

Éléments :
1. H2 : `## experience — git log --oneline`
2. Comment : `// Trois postes. Onze ans. Tous .NET.`
3. Bloc git-log : container `bg-elev`, lignes de commit issues du JSON

Chaque commit affiche trois colonnes :
- **hash** (accent) : généré automatiquement depuis `Societe + DateDebut` via `GenerateHash()` — déterministe, 7 chars hex, style git
- **when** (muted) : `DateDebut[:4] → now` (premier commit) ou `DateDebut[:4] → DateFin[:4]`
- **commit-body** : ligne principale (`**Poste** @ Societe — Lieu`) + ligne `Accroche` en style `//`

Le premier commit (index 0) reçoit la classe CSS `head` — le CSS ajoute `(HEAD)` après le hash via `::after`. Le `now` est dans un `<span class="now">` pour le style vert `#22c55e`.

Ajouter une nouvelle expérience dans le JSON n'exige aucun champ `Hash` — le hash se calcule automatiquement.

```razor
@using BlazorPortfolio.Client.Models
@inject BlazorPortfolio.Client.Services.ExperienceService ExperienceService

<section id="experience">
    <h2 class="h-readme">experience <span class="sub">— git log --oneline</span></h2>
    <p class="comment" style="margin-bottom: 18px;">Trois postes. Onze ans. Tous .NET.</p>

    @if (_loading)
    {
        <p class="comment">// chargement...</p>
    }
    else
    {
        <div class="git-log">
            @for (int i = 0; i < _experiences.Count; i++)
            {
                ExperienceInfo exp = _experiences[i];
                bool isHead = i == 0;
                <div class="commit @(isHead ? "head" : "")">
                    <span class="hash">@GenerateHash(exp.Societe + exp.DateDebut)</span>
                    <span class="when">
                        @exp.DateDebut.Substring(0, 4) →
                        @if (string.IsNullOrEmpty(exp.DateFin))
                        {
                            <span class="now">now</span>
                        }
                        else
                        {
                            @exp.DateFin.Substring(0, 4)
                        }
                    </span>
                    <div class="commit-body">
                        <span class="msg">
                            <b>@exp.Poste</b><span class="at">@</span>@exp.Societe
                            <span class="loc">— @exp.Lieu</span>
                        </span>
                        @if (!string.IsNullOrEmpty(exp.Accroche))
                        {
                            <p class="accroche">@exp.Accroche</p>
                        }
                    </div>
                </div>
            }
        </div>
    }
</section>

@code {
    private List<ExperienceInfo> _experiences = [];
    private bool _loading = true;

    protected override async Task OnInitializedAsync()
    {
        _experiences = await ExperienceService.GetExperiencesAsync();
        _loading = false;
    }

    private static string GenerateHash(string seed)
    {
        int hash = 0;
        foreach (char c in seed)
        {
            hash = (hash * 31) + c;
        }
        return (hash & 0x0FFFFFFF).ToString("x7");
    }
}
```

Le `ExperienceSection.razor.css` reprend les styles du `Portfolio.html` lignes 583-624 et ajoute les styles pour `.commit-body` et `.accroche` :

```css
/* styles .git-log, .commit, .hash, .when, .msg, .now, .at, .loc repris de Portfolio.html */

.commit-body {
    display: flex;
    flex-direction: column;
    gap: 6px;
}

.commit-body .accroche {
    font-family: "JetBrains Mono", monospace;
    color: var(--fg-muted);
    font-size: 12px;
    line-height: 1.5;
    margin: 0;
}

.commit-body .accroche::before {
    content: "// ";
    color: var(--fg-dim);
}
```

Responsive mobile (≤767px) — adapté pour `commit-body` (remplace le `.msg` original dans les grid-areas) :

```css
@media (max-width: 767px) {
    .commit {
        grid-template-columns: auto auto 1fr;
        grid-template-areas:
            "hash when ."
            "body body body";
        gap: 6px 12px;
        padding: 14px 16px;
        font-size: 13px;
    }
    .commit .hash  { grid-area: hash; }
    .commit .when  { grid-area: when; }
    .commit-body   { grid-area: body; line-height: 1.5; }
    .commit .msg .loc { display: block; }
    .commit-body .accroche { font-size: 11px; }
}
```

### 6.4 — `Components/Sections/BlogSection.razor`

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

**Note** : cette section sera remplacée par du contenu dynamique en Phase 07 quand le routing blog sera en place. Pour l'instant, c'est un placeholder statique — pas de service, pas de chargement JSON.

### 6.5 — `Components/Sections/ContactSection.razor`

Éléments :
1. H2 : `## contact — pour parler poste, mission, ou code`
2. Bloc contact : container `bg-elev` avec :
   - Lead : "CV disponible immédiatement."
   - Lead comment : `// Postes Tech Lead .NET / Staff Engineer — Paris, remote France. Pas d'ESN.`
   - 4 boutons : Télécharger le CV (primary) + GitHub (outline) + LinkedIn (outline) + Email (outline)

URLs à utiliser :
- CV : `href="assets/cv.pdf"`
- GitHub : `href="https://github.com/nabil-sidhoum"`
- LinkedIn : `href="https://linkedin.com/in/nabilsidhoum"`
- Email : `href="mailto:nabil.sidhoum@gmail.com"` *(ou retirer le bouton si l'adresse ne doit pas être publique)*

Les icônes SVG (download, GitHub, LinkedIn, email) sont inline — mêmes icônes que dans le hero pour la cohérence.

Le `ContactSection.razor.css` reprend les styles du `Portfolio.html` lignes 664-687.

Responsive mobile (≤767px) :
- Les boutons passent en `flex-direction: column` avec `width: 100%`

### 6.6 — Finaliser `Pages/Index.razor`

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

### 6.7 — Scroll-spy de la nav

Les ancres `#about`, `#stack`, `#projets`, `#experience`, `#contact` doivent correspondre aux `id` des `<section>` dans chaque composant.

Ajouter `scroll-margin-top: 80px` sur les sections dans `theme.css` :

```css
section { scroll-margin-top: 80px; }
```

Le scroll-spy nécessite un `IntersectionObserver` en JS. Créer `wwwroot/js/scroll-spy.js` :

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
        if (el) { observer.observe(el); }
    }

    return observer;
}

export function destroy(observer) {
    if (observer) { observer.disconnect(); }
}
```

Le composant `Nav.razor` charge ce module via `IJSRuntime`, appelle `initScrollSpy` dans `OnAfterRenderAsync`, et met à jour la classe `active` sur les liens via `[JSInvokable] public void OnSectionVisible(string id)`.

**Important — liste des sections observées** : le lien "blog" dans la nav pointe vers `/blog` (route Blazor), pas `#blog` (ancre). Exclure `"blog"` de la liste passée à `initScrollSpy` — aucun lien nav ne correspond à `href="#blog"` donc l'observer ne servirait à rien pour cette section :

```csharp
// Dans Nav.razor — OnAfterRenderAsync
string[] sectionIds = ["about", "stack", "projets", "experience", "contact"];
_observer = await _scrollSpyModule.InvokeAsync<IJSObjectReference>(
    "initScrollSpy", sectionIds, _dotNetRef);
```

**Important — gestion mémoire** : `DotNetObjectReference.Create(this)` crée une référence que JavaScript conserve. `Nav.razor` doit implémenter `IAsyncDisposable` pour la disposer et déconnecter l'observer :

```csharp
public async ValueTask DisposeAsync()
{
    if (_scrollSpyModule != null)
    {
        if (_observer != null)
        {
            await _scrollSpyModule.InvokeVoidAsync("destroy", _observer);
            await _observer.DisposeAsync();
        }
        await _scrollSpyModule.DisposeAsync();
    }
    _dotNetRef?.Dispose();
}
```

### 6.8 — Tests — `ExperienceServiceTests.cs`

Couverture minimale requise (pattern MockHttp, commentaires AAA, nommage `Methode_Scenario_ResultatAttendu`) :

| Cas de test | Description |
|-------------|-------------|
| `GetExperiencesAsync_RetourneExperiences_QuandJsonEstValide` | JSON valide → 3 expériences chargées, `Societe` de la première = "IRBIS Finance" |
| `GetExperiencesAsync_RetourneListeVide_QuandReponseEst404` | HTTP 404 → collection vide, sans exception |
| `GetExperiencesAsync_RetourneMemeInstance_AuDeuxiemeAppel` | 2 appels → 1 seul HTTP (cache) |

Fixture JSON à ajouter dans `JsonFixtures.cs` :

```csharp
public static readonly string Experiences = /*lang=json,strict*/ """
[
    {
        "Societe": "IRBIS Finance",
        "Poste": "Tech Lead .NET",
        "Lieu": "Paris 08",
        "DateDebut": "2021-07-01",
        "DateFin": null,
        "Accroche": "Description test.",
        "Descriptions": [],
        "Competences": ["C#", ".NET 8"]
    }
]
""";
```

## Validation

```bash
dotnet run
```

Vérifier :
- La section experience affiche les commits chargés depuis JSON
- Le premier commit a `(HEAD)` via CSS et `now` en vert
- Le champ `Accroche` s'affiche en style `//` sous chaque commit
- Sur mobile, hash+date au-dessus, commit-body en dessous
- La section blog affiche 3 ghost cards avec `// TODO[1-3]`
- Sur mobile, les ghost cards s'empilent en colonne
- La section contact affiche 4 boutons (CV + GitHub + LinkedIn + Email)
- Sur mobile, les boutons s'empilent en full width
- Le footer affiche `// EOF` + copyright + liens
- Le scroll-spy met en surbrillance les liens `about`, `stack`, `projets`, `experience`, `contact` (pas `blog`)
- Toutes les ancres de la nav scrollent vers la bonne section
- Tests unitaires : 3 tests verts pour `ExperienceService`

## Commit

```
feat: ajouter les sections Experience, Blog placeholder et Contact

- ExperienceInfo POCO + ExperienceService (chargement dynamique depuis experiences.json)
- ExperienceSection en format git log avec Accroche en style commentaire
- BlogSection placeholder avec 3 ghost cards TODO
- ContactSection avec CTAs (CV, GitHub, LinkedIn, Email)
- scroll-spy sur la navigation via IntersectionObserver (sans "blog")
- assemblage complet de Index.razor
- tests unitaires ExperienceService (3 cas)
```

## Checklist avant de passer à la phase 07

- [ ] Les 7 sections s'affichent dans l'ordre correct avec dividers
- [ ] Le git log charge depuis JSON (`dotnet run` → Network tab → `experiences.json` fetché)
- [ ] L'`Accroche` s'affiche sous chaque commit en style `//`
- [ ] Le git log est lisible sur desktop et mobile
- [ ] Les ghost cards blog ont le bon style (dashed, muted)
- [ ] Le contact a 4 boutons fonctionnels avec les bonnes URLs
- [ ] Le scroll-spy fonctionne sur les 5 sections (about, stack, projets, experience, contact)
- [ ] Le lien "blog" de la nav navigue vers `/blog` et ne participe pas au scroll-spy
- [ ] Le footer est en place
- [ ] `DotNetObjectReference` disposé dans `Nav.razor` (`IAsyncDisposable`)
- [ ] Tests `ExperienceService` : 3 tests verts
- [ ] Le build compile sans erreur
- [ ] **Comparaison visuelle** : ouvrir le Portfolio.html de référence côte à côte avec le site Blazor et vérifier la correspondance
