using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;
using BlazorPortfolio.Client.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorPortfolio.Client.Pages
{
    public partial class Blog
    {
        [Inject]
        private BlogService BlogService { get; set; }

        private IReadOnlyList<BlogArticle> _articles;

        protected override async Task OnInitializedAsync()
        {
            _articles = await BlogService.GetArticlesAsync();
        }
    }
}