using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;
using BlazorPortfolio.Client.Services;
using RichardSzalay.MockHttp;
using Xunit;

namespace BlazorPortfolio.Client.Tests
{
    public class BlogServiceTests
    {
        [Fact]
        public async Task GetArticlesAsync_RetourneArticles_QuandIndexEstValide()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/posts/index.json")
                    .Respond("application/json", JsonFixtures.BlogIndex);
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            BlogService service = new BlogService(http);

            // Act
            IReadOnlyList<BlogArticle> result = await service.GetArticlesAsync();

            // Assert
            Assert.NotNull(result);
            BlogArticle premier = Assert.Single(result);
            Assert.Equal("mon-premier-article", premier.Slug);
            Assert.Equal("Mon premier article", premier.Title);
            Assert.Equal(2, premier.Tags.Count);
        }

        [Fact]
        public async Task GetArticlesAsync_RetourneListeVide_QuandIndexEstVide()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/posts/index.json")
                    .Respond("application/json", JsonFixtures.BlogIndexVide);
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            BlogService service = new BlogService(http);

            // Act
            IReadOnlyList<BlogArticle> result = await service.GetArticlesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetArticlesAsync_RetourneListeVide_QuandReponseEst404()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/posts/index.json")
                    .Respond(HttpStatusCode.NotFound);
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            BlogService service = new BlogService(http);

            // Act
            IReadOnlyList<BlogArticle> result = await service.GetArticlesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetArticlesAsync_RetourneMemeInstance_AuDeuxiemeAppel()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/posts/index.json")
                    .Respond("application/json", JsonFixtures.BlogIndex);
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            BlogService service = new BlogService(http);

            // Act
            IReadOnlyList<BlogArticle> premier = await service.GetArticlesAsync();
            IReadOnlyList<BlogArticle> second = await service.GetArticlesAsync();

            // Assert
            Assert.Same(premier, second);
        }

        [Fact]
        public async Task GetArticleContentAsync_RetourneHtml_QuandMarkdownExiste()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/posts/mon-article.md")
                    .Respond("text/markdown", "# Titre\n\nUn paragraphe.");
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            BlogService service = new BlogService(http);

            // Act
            string result = await service.GetArticleContentAsync("mon-article");

            // Assert
            Assert.NotNull(result);
            Assert.Contains("<h1", result);
            Assert.Contains("Un paragraphe.", result);
        }

        [Fact]
        public async Task GetArticleContentAsync_RetourneNull_QuandArticleIntrouvable()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/posts/inexistant.md")
                    .Respond(HttpStatusCode.NotFound);
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            BlogService service = new BlogService(http);

            // Act
            string result = await service.GetArticleContentAsync("inexistant");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetArticleContentAsync_RetourneNull_QuandSlugEstVide()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            BlogService service = new BlogService(http);

            // Act
            string result = await service.GetArticleContentAsync("");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetArticleContentAsync_RetourneNull_QuandSlugContientCaracteresInvalides()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            BlogService service = new BlogService(http);

            // Act
            string result = await service.GetArticleContentAsync("../secret");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetArticleContentAsync_EchappeHtmlInline_QuandMarkdownContientDuHtml()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/posts/avec-html.md")
                    .Respond("text/markdown", "<script>alert('xss')</script>\n\n# Titre");
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            BlogService service = new BlogService(http);

            // Act
            string result = await service.GetArticleContentAsync("avec-html");

            // Assert
            Assert.NotNull(result);
            Assert.DoesNotContain("<script>", result);
            Assert.Contains("<h1", result);
        }
    }
}