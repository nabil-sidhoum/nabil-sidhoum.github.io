using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;
using BlazorPortfolio.Client.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorPortfolio.Client.Components.Sections
{
    public partial class BlogSection
    {
        private const int MaxPreview = 3;

        [Inject]
        private BlogService BlogService { get; set; }

        private IReadOnlyList<BlogArticle> _articles = [];

        protected override async Task OnInitializedAsync()
        {
            IReadOnlyList<BlogArticle> articles = await BlogService.GetArticlesAsync();
            _articles = articles.Take(MaxPreview).ToList();
        }
    }
}