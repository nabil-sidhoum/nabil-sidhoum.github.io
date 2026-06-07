using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;
using BlazorPortfolio.Client.Services;
using RichardSzalay.MockHttp;
using Xunit;

namespace BlazorPortfolio.Client.Tests
{
    public class StackServiceTests
    {
        [Fact]
        public async Task GetStackAsync_RetourneCategories_QuandJsonEstValide()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/data/stack.json")
                    .Respond("application/json", JsonFixtures.Stack);
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            StackService service = new StackService(http);

            // Act
            IEnumerable<StackCategoryInfo> result = await service.GetStackAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetStackAsync_RetourneListeVide_QuandReponseEst404()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/data/stack.json")
                    .Respond(HttpStatusCode.NotFound);
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            StackService service = new StackService(http);

            // Act
            IEnumerable<StackCategoryInfo> result = await service.GetStackAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetStackAsync_MappeCorrectement_LesChampsCategorie()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/data/stack.json")
                    .Respond("application/json", JsonFixtures.Stack);
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            StackService service = new StackService(http);

            // Act
            IEnumerable<StackCategoryInfo> result = await service.GetStackAsync();

            // Assert
            StackCategoryInfo accentCategory = result.Single(category => category.IsAccent);
            Assert.Equal("ai", accentCategory.Key);
            Assert.Equal("Claude Code", accentCategory.Items.Single());
        }
    }
}