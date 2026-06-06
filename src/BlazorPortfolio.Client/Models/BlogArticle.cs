using System.Collections.Generic;

namespace BlazorPortfolio.Client.Models
{
    public sealed class BlogArticle
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string PublishedAt { get; set; }
        public List<string> Tags { get; set; }
    }
}