using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;

namespace BlazorPortfolio.Client.Services
{
    public interface IBlogService
    {
        Task<IReadOnlyList<BlogArticle>> GetArticlesAsync();

        Task<string> GetArticleContentAsync(string slug);
    }
}