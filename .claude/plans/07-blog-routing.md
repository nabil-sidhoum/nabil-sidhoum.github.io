# Phase 07 — Pages Blog (routing multi-pages)

## Objectif

Mettre en place le routing Blazor pour le blog : page liste (`/blog`) et page article (`/blog/{slug}`). Le contenu sera en Markdown parsé côté client. La structure est prête même si aucun article n'est encore rédigé.

## Prérequis

- Phase 06 terminée (toutes les sections de la page principale fonctionnelles)

## Architecture du blog

```
/                    → Index.razor (single page scroll)
/blog                → Blog.razor (liste des articles)
/blog/{slug}         → BlogPost.razor (article individuel)
```

Les articles sont des fichiers `.md` dans `wwwroot/posts/`. Au runtime, Blazor WASM les fetch via `HttpClient`, les parse en HTML, et les affiche. Pas de logique serveur.

## Dépendance à ajouter

```bash
dotnet add package Markdig --version 0.38.0
```

Markdig est la référence .NET pour le parsing Markdown → HTML. Licence BSD, zéro dépendance externe, compatible Blazor WASM.

## Composants et fichiers à créer

### 7.1 — `Models/BlogArticle.cs`

```csharp
using System;

namespace Portfolio.Models;

public sealed record BlogArticle
{
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public required string Summary { get; init; }
    public required DateOnly PublishedAt { get; init; }
    public required string[] Tags { get; init; }
}
```

### 7.2 — `Models/BlogIndex.cs`

Fichier JSON qui référence tous les articles disponibles.

Créer `wwwroot/posts/index.json` :

```json
{
  "articles": []
}
```

Quand un article sera ajouté, le format sera :

```json
{
  "articles": [
    {
      "slug": "mon-premier-article",
      "title": "Mon premier article",
      "summary": "Description courte de l'article.",
      "publishedAt": "2026-08-15",
      "tags": [".NET", "Claude Code"]
    }
  ]
}
```

### 7.3 — `Services/BlogService.cs`

Service qui charge l'index et le contenu des articles via `HttpClient` :

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Portfolio.Models;

namespace Portfolio.Services;

public sealed class BlogService
{
    private readonly HttpClient _httpClient;
    private IReadOnlyList<BlogArticle>? _cachedArticles;

    public BlogService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<BlogArticle>> GetArticlesAsync()
    {
        if (_cachedArticles is not null)
        {
            return _cachedArticles;
        }

        BlogIndex? index = await _httpClient.GetFromJsonAsync<BlogIndex>("posts/index.json");
        _cachedArticles = index?.Articles ?? Array.Empty<BlogArticle>();
        return _cachedArticles;
    }

    public async Task<string?> GetArticleContentAsync(string slug)
    {
        try
        {
            string markdown = await _httpClient.GetStringAsync($"posts/{slug}.md");
            return Markdig.Markdown.ToHtml(markdown);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }
}

file sealed record BlogIndex
{
    public BlogArticle[] Articles { get; init; } = Array.Empty<BlogArticle>();
}
```

Enregistrer dans `Program.cs` :

```csharp
builder.Services.AddScoped<BlogService>();
```

### 7.4 — `Pages/Blog.razor`

Page liste — affiche les articles ou le placeholder si la liste est vide.

```razor
@page "/blog"
@using Portfolio.Models
@using Portfolio.Services
@inject BlogService BlogService

<section class="blog-page">
    <h1 class="h-readme">blog <span class="sub">— articles techniques</span></h1>

    @if (_articles is null)
    {
        <p class="comment">Chargement...</p>
    }
    else if (_articles.Count == 0)
    {
        <p class="comment">Drafts en cours. Les articles arriveront ici.</p>
        <div class="ghost-grid">
            <div class="ghost">
                <span class="ghost-tag">// TODO[1]</span>
                <span class="ghost-title">Article à venir</span>
            </div>
            <div class="ghost">
                <span class="ghost-tag">// TODO[2]</span>
                <span class="ghost-title">Article à venir</span>
            </div>
            <div class="ghost">
                <span class="ghost-tag">// TODO[3]</span>
                <span class="ghost-title">Article à venir</span>
            </div>
        </div>
    }
    else
    {
        <div class="article-list">
            @foreach (BlogArticle article in _articles)
            {
                <a href="/blog/@article.Slug" class="article-card">
                    <time>@article.PublishedAt.ToString("yyyy-MM-dd")</time>
                    <h3>@article.Title</h3>
                    <p>@article.Summary</p>
                    <div class="tags">
                        @foreach (string tag in article.Tags)
                        {
                            <span class="pill">@tag</span>
                        }
                    </div>
                </a>
            }
        </div>
    }

    <a href="/" class="back-link">← retour au portfolio</a>
</section>

@code {
    private IReadOnlyList<BlogArticle>? _articles;

    protected override async Task OnInitializedAsync()
    {
        _articles = await BlogService.GetArticlesAsync();
    }
}
```

Le `Blog.razor.css` reprend le style des ghost cards de la BlogSection + ajoute le style des article-cards (pour quand il y aura du contenu).

### 7.5 — `Pages/BlogPost.razor`

Page article individuel :

```razor
@page "/blog/{Slug}"
@using Portfolio.Services
@inject BlogService BlogService

<article class="blog-post">
    @if (_htmlContent is null && !_notFound)
    {
        <p class="comment">Chargement...</p>
    }
    else if (_notFound)
    {
        <h1 class="h-readme">404 <span class="sub">— article introuvable</span></h1>
        <p class="comment">Cet article n'existe pas ou a été déplacé.</p>
    }
    else
    {
        <div class="prose">
            @((MarkupString)_htmlContent!)
        </div>
    }

    <a href="/blog" class="back-link">← retour au blog</a>
</article>

@code {
    [Parameter]
    public string Slug { get; set; } = string.Empty;

    private string? _htmlContent;
    private bool _notFound;

    protected override async Task OnInitializedAsync()
    {
        _htmlContent = await BlogService.GetArticleContentAsync(Slug);
        _notFound = _htmlContent is null;
    }
}
```

Le `BlogPost.razor.css` contient les styles pour la classe `.prose` — typographie de l'article (headings, paragraphes, code blocks, listes, liens). S'inspirer du style README du portfolio pour la cohérence.

### 7.6 — Mettre à jour la nav

Le lien "blog" dans `Nav.razor` et `MobileMenu.razor` doit pointer vers `/blog` (route Blazor) et non `#blog` (ancre).

Sur la page `Index.razor`, la `BlogSection` reste un placeholder (elle pointe vers `/blog` avec un lien).

### 7.7 — Mettre à jour le `_Imports.razor`

Ajouter les usings nécessaires :

```razor
@using Portfolio.Models
@using Portfolio.Services
```

### 7.8 — Workflow pour ajouter un article

Quand Nabil écrira un article, le process est :

1. Créer `wwwroot/posts/mon-slug.md` (Markdown standard)
2. Ajouter l'entrée dans `wwwroot/posts/index.json`
3. Commit + push → GitHub Pages redéploie

Pas de build step supplémentaire. Le parsing Markdown se fait côté client.

## Validation

```bash
dotnet run
```

Vérifier :
- `/blog` affiche le placeholder (3 ghost cards) puisque `index.json` est vide
- La nav "blog" navigue vers `/blog` (pas un scroll)
- Le bouton "← retour au portfolio" ramène sur `/`
- Ajouter un article de test dans `index.json` + créer un `.md` → vérifier que `/blog` affiche la card et que `/blog/{slug}` rend le Markdown
- `/blog/slug-inexistant` affiche la 404
- Le style de l'article est cohérent avec le reste du portfolio
- Supprimer l'article de test après validation

## Commit

```
feat: ajouter le routing blog avec support Markdown

- Blog.razor (liste des articles ou placeholder si vide)
- BlogPost.razor (article individuel, Markdown → HTML via Markdig)
- BlogService avec cache et fetch HttpClient
- index.json comme registre des articles
- workflow documenté pour ajouter un article
```

## Checklist avant de passer à la phase 08

- [ ] `/blog` s'affiche correctement (placeholder ou liste)
- [ ] `/blog/{slug}` rend un article Markdown
- [ ] `/blog/{slug-inexistant}` affiche une 404 propre
- [ ] La nav "blog" navigue vers `/blog`
- [ ] Le retour portfolio fonctionne
- [ ] Le build compile sans erreur
