using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;
using BlazorPortfolio.Client.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorPortfolio.Client.Pages
{
    public partial class BlogPost
    {
        private const string TitleSuffix = " · Nabil Sidhoum";
        private const string DefaultTitle = "Blog" + TitleSuffix;

        [Inject]
        private IBlogService BlogService { get; set; }

        [Parameter]
        public string Slug { get; set; }

        private string _htmlContent;
        private bool _notFound;
        private string _pageTitle = DefaultTitle;

        protected override async Task OnInitializedAsync()
        {
            // null couvre les deux cas refusés par le service : slug invalide (rejeté
            // par la garde anti-path-traversal) ou article inexistant (404).
            _htmlContent = await BlogService.GetArticleContentAsync(Slug);
            _notFound = _htmlContent == null;

            if (!_notFound)
            {
                // Même titre que la page statique générée au déploiement
                // (.github/scripts/generate_static_routes.py) : l'onglet ne change pas
                // au démarrage de Blazor. L'index est mis en cache par le service.
                IReadOnlyList<BlogArticle> articles = await BlogService.GetArticlesAsync();
                BlogArticle article = articles.FirstOrDefault(a => a.Slug == Slug);
                if (article != null)
                {
                    _pageTitle = article.Title + TitleSuffix;
                }
            }
        }
    }
}