using System.Threading.Tasks;
using BlazorPortfolio.Client.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorPortfolio.Client.Pages
{
    public partial class BlogPost
    {
        [Inject]
        private BlogService BlogService { get; set; }

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
}