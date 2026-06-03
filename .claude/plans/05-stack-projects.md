# Phase 05 — Sections Stack + Projects

## Objectif

Implémenter la section compétences (deux blocs .NET / AI) et la section projets (cards doc-block avec composant réutilisable `ProjectCard`). Les projets sont chargés dynamiquement depuis `wwwroot/data/projects.json` via `ProjectService`.

## Prérequis

- Phase 04 terminée (Hero et About fonctionnels)
- Fichier `Portfolio.html` de référence — sections STACK (lignes 990-1030) et PROJECTS (lignes 1035-1132)
- `wwwroot/data/projects.json` à jour (5 projets : VeilleTech, dotnet-claude-template, PageCorrelationId, DynamicsCrmConnector, OpenstackSwiftConnector)

## Composants à créer

### 5.1 — `Models/ProjectInfo.cs`

Classe POCO correspondant exactement au schéma JSON. Pas de record — classe avec `{ get; set; }` :

```csharp
using System.Collections.Generic;

namespace BlazorPortfolio.Client.Models
{
    public class ProjectInfo
    {
        public string Nom { get; set; }
        public string BadgeType { get; set; }
        public string Description { get; set; }
        public string GithubUrl { get; set; }
        public string ExtraUrl { get; set; }
        public string ExtraLabel { get; set; }
        public List<string> Competences { get; set; }
    }
}
```

`BadgeType` reste une `string` (`"Showcase"` ou `"OpenSource"`) — comparaison directe dans le composant.
`GithubUrl`, `ExtraUrl`, `ExtraLabel` sont `null` pour les projets SHOWCASE (nullable désactivé mais la valeur JSON peut être `null` sans annotation).

### 5.2 — `Services/ProjectService.cs`

Charge et met en cache la liste depuis `wwwroot/data/projects.json` :

```csharp
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;

namespace BlazorPortfolio.Client.Services
{
    public sealed class ProjectService
    {
        private readonly HttpClient _httpClient;
        private List<ProjectInfo> _cache;

        public ProjectService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProjectInfo>> GetProjectsAsync()
        {
            if (_cache != null)
            {
                return _cache;
            }

            _cache = await _httpClient.GetFromJsonAsync<List<ProjectInfo>>("data/projects.json");
            return _cache ?? [];
        }
    }
}
```

### 5.3 — Mettre à jour `Program.cs`

Enregistrer `ProjectService` en Scoped après la ligne `AddScoped(HttpClient)` :

```csharp
builder.Services.AddScoped<ProjectService>();
```

### 5.4 — Mettre à jour `_Imports.razor`

Ajouter les deux using manquants (à faire uniquement quand les namespaces ont au moins un fichier) :

```razor
@using BlazorPortfolio.Client.Models
@using BlazorPortfolio.Client.Services
```

### 5.5 — `Components/Sections/StackSection.razor`

Données en dur dans le composant — les compétences ne viennent pas d'un JSON, c'est voulu.

Éléments :
1. H2 : `## stack — deux domaines distincts`
2. Comment : `// Listés séparément par choix : la stack .NET est la base ; l'IA est l'extension récente.`
3. Bloc skills avec deux lignes :
   - `.net:` → pills : C# 14, .NET 10, Clean Architecture, CQRS, MediatR, EF Core, ASP.NET Core, Azure DevOps, Bloomberg BLPAPI
   - `ai:` → pills : Claude Code, MCP servers, Multi-agent, Groq, LLM integration

Structure du bloc skills :
- Container avec `background: var(--bg-elev)`, border, border-radius
- Chaque ligne est une grid `90px 1fr` : clé en accent + pills qui wrappent
- Séparateur `border-bottom` entre les deux lignes

Le `StackSection.razor.css` reprend les styles du `Portfolio.html` lignes 445-487. Points clés :
- Sur mobile (≤767px) : la grid passe en `1fr` (clé au-dessus des pills)
- Les pills wrappent naturellement via `flex-wrap: wrap`

### 5.6 — `Components/Sections/ProjectCard.razor`

Composant réutilisable avec un `[Parameter]` de type `ProjectInfo`.

Éléments :
1. Header : nom du projet (avec `▸` en accent) + badge (SHOWCASE ou OPEN SOURCE) + liens optionnels
2. Paragraphe description (HTML safe via `MarkupString`)
3. Ligne de pills compétences

```razor
@using BlazorPortfolio.Client.Models

<article class="proj">
    <header class="proj-head">
        <h3>@Project.Nom</h3>
        @if (Project.BadgeType == "Showcase")
        {
            <span class="badge show">SHOWCASE</span>
        }
        else
        {
            <span class="badge open">OPEN SOURCE</span>
        }
        @if (Project.GithubUrl != null || Project.ExtraUrl != null)
        {
            <div class="proj-links">
                @if (Project.GithubUrl != null)
                {
                    <a href="@Project.GithubUrl" target="_blank" rel="noopener">
                        github
                    </a>
                }
                @if (Project.ExtraUrl != null)
                {
                    <a href="@Project.ExtraUrl" target="_blank" rel="noopener">
                        @Project.ExtraLabel
                    </a>
                }
            </div>
        }
    </header>
    <p>@((MarkupString)Project.Description)</p>
    <div class="stack">
        @foreach (string pill in Project.Competences)
        {
            <span class="pill">@pill</span>
        }
    </div>
</article>

@code {
    [Parameter]
    public ProjectInfo Project { get; set; }
}
```

Le `ProjectCard.razor.css` reprend les styles du `Portfolio.html` lignes 492-578. Points clés :
- Le `h3::before { content: "▸ " }` est en accent
- Le header a un `border-bottom: 1px dashed`
- Les liens projets ont un hover avec background accent-soft
- Sur mobile : les liens passent en `width: 100%`

### 5.7 — `Components/Sections/ProjectsSection.razor`

Charge les projets via `ProjectService`, affiche un état de chargement pendant la requête :

```razor
@inject BlazorPortfolio.Client.Services.ProjectService ProjectService

<section id="projets">
    <h2 class="h-readme">projets <span class="sub">— ce que je construis</span></h2>
    <p class="comment">// Classés par impact. SHOWCASE = repo privé, OPEN SOURCE = public sur GitHub.</p>

    @if (_loading)
    {
        <p class="comment">// chargement...</p>
    }
    else
    {
        <div class="projects">
            @foreach (ProjectInfo project in _projects)
            {
                <ProjectCard Project="project" />
            }
        </div>
    }
</section>

@code {
    private List<ProjectInfo> _projects = [];
    private bool _loading = true;

    protected override async Task OnInitializedAsync()
    {
        _projects = await ProjectService.GetProjectsAsync();
        _loading = false;
    }
}
```

Le `ProjectsSection.razor.css` contient uniquement le layout de la section (gap entre cards). Les styles des cards sont dans `ProjectCard.razor.css`.

### 5.8 — Tests — `ProjectServiceTests.cs`

Couverture minimale requise (pattern MockHttp, commentaires AAA, nommage `Methode_Scenario_ResultatAttendu`) :

| Cas de test | Description |
|-------------|-------------|
| `GetProjectsAsync_RetourneProjects_QuandJsonEstValide` | JSON valide → 5 projets chargés, Nom du premier = "VeilleTech" |
| `GetProjectsAsync_RetourneListeVide_QuandReponseEst404` | HTTP 404 → collection vide, sans exception |
| `GetProjectsAsync_RetourneMemeInstance_AuDeuxiemeAppel` | Vérifier que le cache fonctionne (2 appels → 1 seul HTTP) |

Fixture JSON à créer dans `JsonFixtures.cs` :

```csharp
public static readonly string Projects = /*lang=json,strict*/ """
[
    {
        "Nom": "VeilleTech",
        "BadgeType": "Showcase",
        "Description": "Description test.",
        "GithubUrl": null,
        "ExtraUrl": null,
        "ExtraLabel": null,
        "Competences": [".NET 10", "Groq"]
    }
]
""";
```

### 5.9 — Mettre à jour `Pages/Index.razor`

Ajouter les nouvelles sections après `AboutSection` :

```razor
<StackSection />
<div class="divider" aria-hidden="true">//</div>

<ProjectsSection />
<div class="divider" aria-hidden="true">//</div>
```

## Validation

```bash
dotnet build src/BlazorPortfolio.Client/BlazorPortfolio.Client.csproj
dotnet test src/BlazorPortfolio.Client.Tests/BlazorPortfolio.Client.Tests.csproj
dotnet run --project src/BlazorPortfolio.Client
```

Vérifier :
- La section stack affiche deux lignes distinctes (.net et ai) avec pills
- Les pills wrappent correctement sur toutes les largeurs
- Sur mobile, la clé passe au-dessus des pills
- Les 5 project cards s'affichent dans l'ordre du JSON
- VeilleTech et dotnet-claude-template ont le badge SHOWCASE (dashed, pas de liens)
- PageCorrelationId, DynamicsCrmConnector, OpenstackSwiftConnector ont le badge OPEN SOURCE + liens
- Les liens s'ouvrent dans un nouvel onglet (`rel="noopener"`)
- L'état "chargement..." s'affiche brièvement puis disparaît
- Tests unitaires : 3 tests verts pour ProjectService

## Commit

```
feat: ajouter les sections Stack et Projects avec chargement dynamique

- StackSection avec deux blocs .NET et AI tooling (données en dur dans le composant)
- ProjectInfo classe POCO + ProjectService chargement depuis projects.json via HttpClient
- ProjectCard composant réutilisable paramétré par ProjectInfo
- ProjectsSection avec injection de ProjectService et OnInitializedAsync
- Tests unitaires ProjectService (nominal, 404, cache)
```

## Checklist avant de passer à la phase 06

- [ ] Section stack avec 2 lignes et pills
- [ ] 5 project cards dans l'ordre du JSON avec les bons badges
- [ ] Liens GitHub/extra fonctionnels sur les projets OpenSource
- [ ] Aucun lien sur les projets SHOWCASE
- [ ] État de chargement visible pendant la requête JSON
- [ ] Cache : le second appel ne refait pas de requête HTTP
- [ ] Responsive correct (pills wrap, grid stack, liens full width mobile)
- [ ] `dotnet build` sans erreur ni warning
- [ ] Tests unitaires ProjectService : 3 tests verts
- [ ] `/project:lead-dev` passé sans blocage CRITIQUE
