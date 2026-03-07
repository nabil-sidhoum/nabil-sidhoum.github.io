using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;
using BlazorPortfolio.Client.Services;
using BlazorPortfolio.Client.Tests.Fixtures;
using RichardSzalay.MockHttp;
using Xunit;

namespace BlazorPortfolio.Client.Tests.Services
{
    public class ExperienceServiceTests
    {
        [Fact]
        public async Task GetExperiencesAsync_ReturnsExperiences_WhenJsonIsValid()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new();
            mockHttp
                .When("http://localhost/data/experiences.json")
                .Respond("application/json", JsonFixtures.Experiences);

            HttpClient http = new(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            ExperienceService service = new(http);

            // Act
            IEnumerable<Experience> result = await service.GetExperiencesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetExperiencesAsync_RetourneListeVide_QuandReponseEst404()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new();
            mockHttp
                .When("http://localhost/data/experiences.json")
                .Respond(HttpStatusCode.NotFound);

            HttpClient http = new(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            ExperienceService service = new(http);

            // Act
            IEnumerable<Experience> result = await service.GetExperiencesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetExperiencesAsync_ExperiencesTrieesParDateDecroissante()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new();
            mockHttp
                .When("http://localhost/data/experiences.json")
                .Respond("application/json", JsonFixtures.Experiences);

            HttpClient http = new(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            ExperienceService service = new(http);
            // Act
            IEnumerable<Experience> result = await service.GetExperiencesAsync();

            // Assert
            Assert.True(result.FirstOrDefault().DateDebut >= result.LastOrDefault().DateDebut);
        }

        [Fact]
        public async Task GetExperiencesAsync_MappeCorrectementLesProprietes()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new();
            mockHttp
                .When("http://localhost/data/experiences.json")
                .Respond("application/json", JsonFixtures.Experiences);

            HttpClient http = new(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            ExperienceService service = new(http);

            // Act
            IEnumerable<Experience> result = await service.GetExperiencesAsync();
            Experience premiere = result.FirstOrDefault();

            // Assert
            Assert.Equal("IRBIS", premiere.Societe);
            Assert.Equal("Directeur technique", premiere.Poste);
            Assert.Null(premiere.DateFin);
            Assert.Contains(".NET 8", premiere.Competences);
        }
    }
}