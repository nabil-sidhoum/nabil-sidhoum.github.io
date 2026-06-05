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
    public class ExperienceServiceTests
    {
        [Fact]
        public async Task GetExperiencesAsync_RetourneExperiences_QuandJsonEstValide()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/data/experiences.json")
                    .Respond("application/json", JsonFixtures.Experiences);
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            ExperienceService service = new ExperienceService(http);

            // Act
            List<ExperienceInfo> result = await service.GetExperiencesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("IRBIS Finance", result[0].Societe);
        }

        [Fact]
        public async Task GetExperiencesAsync_RetourneListeVide_QuandReponseEst404()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/data/experiences.json")
                    .Respond(HttpStatusCode.NotFound);
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            ExperienceService service = new ExperienceService(http);

            // Act
            List<ExperienceInfo> result = await service.GetExperiencesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetExperiencesAsync_RetourneMemeInstance_AuDeuxiemeAppel()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/data/experiences.json")
                    .Respond("application/json", JsonFixtures.Experiences);
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            ExperienceService service = new ExperienceService(http);

            // Act
            List<ExperienceInfo> premier = await service.GetExperiencesAsync();
            List<ExperienceInfo> second = await service.GetExperiencesAsync();

            // Assert
            Assert.Same(premier, second);
        }
    }
}