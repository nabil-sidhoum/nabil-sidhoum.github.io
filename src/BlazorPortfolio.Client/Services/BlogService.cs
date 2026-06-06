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

        private static readonly Regex _slugRegex = new Regex(SlugPattern);
        private static readonly MarkdownPipeline _markdownPipeline = new MarkdownPipelineBuilder().DisableHtml().Build();

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

            try
            {
                BlogIndex index = await _httpClient.GetFromJsonAsync<BlogIndex>("posts/index.json");
                _cachedArticles = index != null && index.Articles != null ? index.Articles : [];
                return _cachedArticles;
            }
            catch (HttpRequestException)
            {
                return [];
            }
        }

        public async Task<string> GetArticleContentAsync(string slug)
        {
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
                return null;
            }
        }

        private sealed class BlogIndex
        {
            public List<BlogArticle> Articles { get; set; }
        }
    }
}