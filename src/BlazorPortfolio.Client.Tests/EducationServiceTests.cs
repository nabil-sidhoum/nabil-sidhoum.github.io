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

namespace BlazorPortfolio.Client.Tests
{
    public class EducationServiceTests
    {
        [Fact]
        public async Task GetEducationsAsync_RetourneFormations_QuandJsonEstValide()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new();
            mockHttp
                .When("http://localhost/data/educations.json")
                .Respond("application/json", JsonFixtures.Educations);

            HttpClient http = new(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            EducationService service = new(http);

            // Act
            IEnumerable<Education> result = await service.GetEducationsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetEducationsAsync_RetourneListeVide_QuandReponseEst404()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new();
            mockHttp
                .When("http://localhost/data/educations.json")
                .Respond(HttpStatusCode.NotFound);

            HttpClient http = new(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            EducationService service = new(http);

            // Act
            IEnumerable<Education> result = await service.GetEducationsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetEducationsAsync_MappeCorrectementLesProprietes()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new();
            mockHttp
                .When("http://localhost/data/educations.json")
                .Respond("application/json", JsonFixtures.Educations);

            HttpClient http = new(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            EducationService service = new(http);

            // Act
            IEnumerable<Education> result = await service.GetEducationsAsync();
            Education formation = result.FirstOrDefault();

            // Assert
            Assert.Equal("Sup Galilée – Université Sorbonne Paris Nord", formation.School);
            Assert.Equal("Diplôme d'ingénieur, Télécommunication et Réseau", formation.Diploma);
            Assert.Equal(new DateTime(2012, 9, 1), formation.StartDate);
            Assert.NotNull(formation.EndDate);
        }
    }
}