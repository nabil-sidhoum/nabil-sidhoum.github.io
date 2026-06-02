# Phase 05 — Sections Stack + Projects

## Objectif

Implémenter la section compétences (deux blocs .NET / AI) et la section projets (4 cards doc-block avec composant réutilisable `ProjectCard`).

## Prérequis

- Phase 04 terminée (Hero et About fonctionnels)
- Fichier `Portfolio.html` de référence — sections STACK (lignes 990-1030) et PROJECTS (lignes 1035-1132)

## Composants à créer

### 5.1 — `Components/Sections/StackSection.razor`

Éléments :
1. H2 : `## stack — deux domaines distincts`
2. Comment : `// Listés séparément par choix : la stack .NET est la base ; l'IA est l'extension récente.`
3. Bloc skills avec deux lignes :
   - `.net:` → pills : C# 14, .NET 8/10, Clean Architecture, CQRS, MediatR, EF Core, ASP.NET Core, Azure DevOps, Bloomberg BLPAPI
   - `ai:` → pills : Claude Code, MCP servers, Multi-agent, Groq, LLM integration

Structure du bloc skills :
- Container avec `background: var(--bg-elev)`, border, border-radius
- Chaque ligne est une grid `90px 1fr` : clé en accent + pills qui wrappent
- Séparateur `border-bottom` entre les deux lignes

Le `StackSection.razor.css` reprend les styles du `Portfolio.html` lignes 445-487. Points clés :
- Sur mobile (≤767px) : la grid passe en `1fr` (clé au-dessus des pills)
- Les pills wrappent naturellement via `flex-wrap: wrap`

### 5.2 — `Models/ProjectInfo.cs`

Record immuable pour les données de chaque projet :

```csharp
using System.Collections.Generic;

namespace Portfolio.Models;

public sealed record ProjectInfo
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required IReadOnlyList<string> Stack { get; init; }
    public required ProjectBadgeType BadgeType { get; init; }
    public string? GitHubUrl { get; init; }
    public string? NuGetUrl { get; init; }
}

public enum ProjectBadgeType
{
    Showcase,
    OpenSource
}
```

### 5.3 — `Components/Sections/ProjectCard.razor`

Composant réutilisable avec un `[Parameter]` de type `ProjectInfo`.

Éléments :
1. Header : nom du projet (avec `▸` en accent) + badge (SHOWCASE ou OPEN SOURCE) + liens optionnels (GitHub, NuGet)
2. Paragraphe description
3. Ligne de pills stack

```razor
@using Portfolio.Models

<article class="proj">
    <header class="proj-head">
        <h3>@Project.Name</h3>
        @if (Project.BadgeType == ProjectBadgeType.Showcase)
        {
            <span class="badge show">SHOWCASE</span>
        }
        else
        {
            <span class="badge open">OPEN SOURCE</span>
        }
        @if (Project.GitHubUrl is not null || Project.NuGetUrl is not null)
        {
            <div class="proj-links">
                @if (Project.GitHubUrl is not null)
                {
                    <a href="@Project.GitHubUrl" target="_blank" rel="noopener">
                        @* Icône GitHub SVG inline *@
                        github
                    </a>
                }
                @if (Project.NuGetUrl is not null)
                {
                    <a href="@Project.NuGetUrl" target="_blank" rel="noopener">
                        nuget
                    </a>
                }
            </div>
        }
    </header>
    <p>@((MarkupString)Project.Description)</p>
    <div class="stack">
        @foreach (string pill in Project.Stack)
        {
            <span class="pill">@pill</span>
        }
    </div>
</article>

@code {
    [Parameter]
    public required ProjectInfo Project { get; set; }
}
```

Le `ProjectCard.razor.css` reprend les styles du `Portfolio.html` lignes 492-578. Points clés :
- Le `h3::before { content: "▸ " }` est en accent
- Le header a un `border-bottom: 1px dashed`
- Les liens projets ont un hover avec background accent-soft
- Sur mobile : les liens passent en `width: 100%`

### 5.4 — `Components/Sections/ProjectsSection.razor`

Assemble les 4 `ProjectCard` avec les données en dur :

```razor
@using Portfolio.Models

<section id="projets">
    <h2 class="h-readme">projets <span class="sub">— ce que je construis</span></h2>
    <p class="comment">Classés par impact. SHOWCASE = repo privé, OPEN SOURCE = public sur GitHub.</p>

    <div class="projects">
        @foreach (ProjectInfo project in _projects)
        {
            <ProjectCard Project="project" />
        }
    </div>
</section>

@code {
    private static readonly IReadOnlyList<ProjectInfo> _projects = new[]
    {
        new ProjectInfo
        {
            Name = "VeilleTech",
            Description = "Système de veille tech automatisé. RSS aggregation → LLM scoring via Groq/Llama&nbsp;3.3 → publication Discord. <b>3 cron workflows indépendants</b> sur GitHub Actions.",
            Stack = new[] { ".NET 10", "C# 14", "EF Core", "xUnit", "Groq", "GitHub Actions" },
            BadgeType = ProjectBadgeType.Showcase
        },
        new ProjectInfo
        {
            Name = "dotnet-claude-template",
            Description = "Template Claude Code réutilisable pour projets .NET. Architecture multi-agent, conventions Clean Architecture, système ADR, CI configurée.",
            Stack = new[] { "Claude Code", "MCP", "Clean Arch", "ADR" },
            BadgeType = ProjectBadgeType.Showcase
        },
        new ProjectInfo
        {
            Name = "PageCorrelationId",
            Description = "Middleware ASP.NET Core de propagation d'un Correlation ID métier. Package NuGet. CI badge build.",
            Stack = new[] { "ASP.NET Core", "Middleware", "NuGet" },
            BadgeType = ProjectBadgeType.OpenSource,
            GitHubUrl = "https://github.com/nabil-sidhoum/PageCorrelationId"
        },
        new ProjectInfo
        {
            Name = "DynamicsCrmConnector",
            Description = "Connecteur HTTP asynchrone pour l'<b>API Web OData de Microsoft Dynamics CRM</b>. Authentification OAuth2, CRUD complet, mapping fortement typé, FetchXML et pagination automatique. Tests xUnit + Moq + MockHttp, CI gated publish.",
            Stack = new[] { ".NET 8", "System.Text.Json", "OData", "xUnit", "Moq", "MockHttp" },
            BadgeType = ProjectBadgeType.OpenSource,
            GitHubUrl = "https://github.com/nabil-sidhoum/DynamicsCrmConnector",
            NuGetUrl = "https://www.nuget.org/packages/DynamicsCrmConnector"
        }
    };
}
```

Le `ProjectsSection.razor.css` contient uniquement le layout de la section (gap entre cards). Les styles des cards elles-mêmes sont dans `ProjectCard.razor.css`.

### 5.5 — Mettre à jour `Pages/Index.razor`

Ajouter les nouvelles sections après AboutSection :

```razor
<StackSection />
<div class="divider" aria-hidden="true">//</div>

<ProjectsSection />
<div class="divider" aria-hidden="true">//</div>
```

## Validation

```bash
dotnet run
```

Vérifier :
- La section stack affiche deux lignes distinctes (.net et ai) avec pills
- Les pills wrappent correctement sur toutes les largeurs
- Sur mobile, la clé passe au-dessus des pills
- Les 4 project cards s'affichent dans l'ordre : VeilleTech, dotnet-claude-template, PageCorrelationId, DynamicsCrmConnector
- VeilleTech et dotnet-claude-template ont le badge SHOWCASE (dashed, pas de liens)
- PageCorrelationId a le badge OPEN SOURCE + lien GitHub
- DynamicsCrmConnector a le badge OPEN SOURCE + liens GitHub + NuGet
- Les liens s'ouvrent dans un nouvel onglet
- Le hover sur les cards change la border-color
- Sur mobile, les liens projets passent en full width

## Commit

```
feat: ajouter les sections Stack et Projects

- StackSection avec deux blocs .NET et AI tooling
- ProjectCard composant réutilisable paramétré par ProjectInfo
- ProjectsSection avec 4 projets (2 showcase + 2 open source)
- record immuable ProjectInfo avec enum ProjectBadgeType
```

## Checklist avant de passer à la phase 06

- [ ] Section stack avec 2 lignes et pills
- [ ] 4 project cards dans le bon ordre avec les bons badges
- [ ] Liens GitHub/NuGet fonctionnels sur les projets publics
- [ ] Pas de liens sur les projets SHOWCASE
- [ ] Responsive correct (pills wrap, grid stack, liens full width)
- [ ] Le build compile sans erreur
