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

Classe POCO — pas de record, namespace block-scoped, Nullable disable :

```csharp
using System.Collections.Generic;

namespace BlazorPortfolio.Client.Models
{
    public class BlogArticle
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string PublishedAt { get; set; }
        public List<string> Tags { get; set; }
    }
}
```

`PublishedAt` est une chaîne au format `"2026-08-15"` — formatée directement à l'affichage, pas de conversion `DateOnly`.

### 7.2 — Créer `wwwroot/posts/index.json`

Fichier JSON qui référence tous les articles disponibles :

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

Service qui charge l'index et le contenu des articles via `HttpClient`.

Conventions à respecter : namespace block-scoped `BlazorPortfolio.Client.Services`, pas de Nullable (`?` interdit sur types référence), `!= null` pour les null checks, `[]` pour les collections vides :

```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;

namespace BlazorPortfolio.Client.Services
{
    public sealed class BlogService
    {
        private readonly HttpClient _httpClient;
        private IReadOnlyList<BlogArticle> _cachedArticles;

        public BlogService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IReadOnlyList<BlogArticle>> GetArticlesAsync()
        {
            if (_cachedArticles != null)
            {
                return _cachedArticles;
            }

            BlogIndex index = await _httpClient.GetFromJsonAsync<BlogIndex>("posts/index.json");
            _cachedArticles = index?.Articles ?? [];
            return _cachedArticles;
        }

        public async Task<string> GetArticleContentAsync(string slug)
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

        private class BlogIndex
        {
            public List<BlogArticle> Articles { get; set; }
        }
    }
}
```

`BlogIndex` est une classe privée interne à `BlogService` — pas de record, pas de file-scoped namespace.

Enregistrer dans `Program.cs` :

```csharp
builder.Services.AddScoped<BlogService>();
```

### 7.4 — Mettre à jour `_Imports.razor`

Ajouter les usings manquants avec les namespaces corrects (ne pas utiliser `Portfolio.*`) :

```razor
@using BlazorPortfolio.Client.Models
@using BlazorPortfolio.Client.Services
```

### 7.5 — `Pages/Blog.razor`

Page liste — affiche les articles ou le placeholder si la liste est vide.

`_articles` est `null` pendant le chargement initial (avant `OnInitializedAsync`), puis une liste (éventuellement vide). Utiliser `== null` pour les null checks :

```razor
@page "/blog"
@using BlazorPortfolio.Client.Models
@using BlazorPortfolio.Client.Services
@inject BlogService BlogService

<PageTitle>Blog — Nabil Sidhoum</PageTitle>

<section class="blog-page">
    <h1 class="h-readme">blog <span class="sub">— articles techniques</span></h1>

    @if (_articles == null)
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
                    <time>@article.PublishedAt</time>
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
    private IReadOnlyList<BlogArticle> _articles;

    protected override async Task OnInitializedAsync()
    {
        _articles = await BlogService.GetArticlesAsync();
    }
}
```

Le `Blog.razor.css` reprend le style des ghost cards de la BlogSection + ajoute le style des article-cards (pour quand il y aura du contenu).

### 7.6 — `Pages/BlogPost.razor`

Page article individuel. `_htmlContent` est `null` avant chargement et quand l'article est introuvable — gérer les deux cas avec `== null` :

```razor
@page "/blog/{Slug}"
@using BlazorPortfolio.Client.Services
@inject BlogService BlogService

<PageTitle>Blog · Nabil Sidhoum</PageTitle>

<article class="blog-post">
    @if (_htmlContent == null && !_notFound)
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
            @((MarkupString)_htmlContent)
        </div>
    }

    <a href="/blog" class="back-link">← retour au blog</a>
</article>

@code {
    [Parameter]
    public string Slug { get; set; }

    private string _htmlContent;
    private bool _notFound;

    protected override async Task OnInitializedAsync()
    {
        _htmlContent = await BlogService.GetArticleContentAsync(Slug);
        _notFound = _htmlContent == null;
    }
}
```

Le `BlogPost.razor.css` contient les styles pour la classe `.prose` — typographie de l'article (headings, paragraphes, code blocks, listes, liens). S'inspirer du style README du portfolio pour la cohérence.

### 7.7 — Mettre à jour la nav

Le lien "blog" dans `Nav.razor` et `MobileMenu.razor` doit pointer vers `/blog` (route Blazor) et non `#blog` (ancre).

Sur la page `Index.razor`, la `BlogSection` reste un placeholder et conserve son `id="blog"` pour la cohérence du DOM, mais ce lien ne participe pas au scroll-spy (voir Phase 06, section 6.7).

### 7.8 — Workflow pour ajouter un article

Quand un article sera rédigé, le process est :

1. Créer `wwwroot/posts/mon-slug.md` (Markdown standard)
2. Ajouter l'entrée dans `wwwroot/posts/index.json`
3. Commit + push → GitHub Pages redéploie

Pas de build step supplémentaire. Le parsing Markdown se fait côté client via Markdig.

## Validation

```bash
dotnet run
```

Vérifier :
- `/blog` affiche le placeholder (3 ghost cards) puisque `index.json` est vide
- La nav "blog" navigue vers `/blog` (pas un scroll d'ancre)
- Le bouton "← retour au portfolio" ramène sur `/`
- Ajouter un article de test dans `index.json` + créer un `.md` → vérifier que `/blog` affiche la card et que `/blog/{slug}` rend le Markdown
- `/blog/slug-inexistant` affiche la 404 propre
- Le style de l'article est cohérent avec le reste du portfolio
- Supprimer l'article de test après validation

## Commit

```
feat: ajouter le routing blog avec support Markdown

- BlogArticle classe POCO + BlogService (chargement depuis posts/index.json via HttpClient)
- Blog.razor (liste des articles ou placeholder si vide) avec PageTitle
- BlogPost.razor (article individuel, Markdown → HTML via Markdig) avec PageTitle
- index.json comme registre des articles
- workflow documenté pour ajouter un article
```

## Checklist avant de passer à la phase 08

- [ ] `/blog` s'affiche correctement (placeholder ou liste)
- [ ] `/blog/{slug}` rend un article Markdown
- [ ] `/blog/{slug-inexistant}` affiche une 404 propre
- [ ] La nav "blog" navigue vers `/blog`
- [ ] Le retour portfolio fonctionne
- [ ] Les namespaces sont tous `BlazorPortfolio.Client.*` (pas `Portfolio.*`)
- [ ] Aucun `?` sur types référence, aucun `record`, aucun `required`/`init`
- [ ] Le build compile sans erreur
