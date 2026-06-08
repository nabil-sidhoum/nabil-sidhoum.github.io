using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;
using Markdig;

namespace BlazorPortfolio.Client.Services
{
    public sealed class BlogService
    {
        private const string SlugPattern = "^[a-z0-9-]+$";

        // Slug limité à [a-z0-9-] : empêche un path-traversal (« ../ ») lors de la
        // construction du chemin "posts/{slug}.md" à partir d'un paramètre d'URL.
        private static readonly Regex _slugRegex = new Regex(SlugPattern);

        // DisableHtml : le Markdown des articles ne peut pas injecter de HTML brut
        // (protection anti-XSS, cohérente avec la CSP stricte du site).
        private static readonly MarkdownPipeline _markdownPipeline = new MarkdownPipelineBuilder().DisableHtml().Build();

        private readonly HttpClient _httpClient;
        private IReadOnlyList<BlogArticle> _cachedArticles;

        public BlogService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IReadOnlyList<BlogArticle>> GetArticlesAsync()
        {
            // L'index est un JSON statique : on le charge une seule fois et on le
            // mémorise pour éviter une requête HTTP à chaque affichage du blog.
            if (_cachedArticles != null)
            {
                return _cachedArticles;
            }

            try
            {
                BlogIndex index = await _httpClient.GetFromJsonAsync<BlogIndex>("posts/index.json");
                _cachedArticles = index != null && index.Articles != null ? index.Articles : [];
                return _cachedArticles;
            }
            catch (HttpRequestException)
            {
                // Résilience : un index manquant ou injoignable n'interrompt pas le site,
                // le blog s'affiche simplement vide.
                return [];
            }
        }

        public async Task<string> GetArticleContentAsync(string slug)
        {
            // On valide le slug AVANT de l'injecter dans un chemin de fichier :
            // un slug non conforme est rejeté sans tenter la requête.
            if (string.IsNullOrWhiteSpace(slug) || !_slugRegex.IsMatch(slug))
            {
                return null;
            }

            try
            {
                string markdown = await _httpClient.GetStringAsync($"posts/{slug}.md");
                return Markdown.ToHtml(markdown, _markdownPipeline);
            }
            catch (HttpRequestException)
            {
                // Article absent (404) → null : la page affiche son état « introuvable ».
                return null;
            }
        }

        private sealed class BlogIndex
        {
            public List<BlogArticle> Articles { get; set; }
        }
    }
}