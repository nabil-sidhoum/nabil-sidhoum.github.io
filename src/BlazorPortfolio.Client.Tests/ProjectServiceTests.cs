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
    public class ProjectServiceTests
    {
        [Fact]
        public async Task GetProjectsAsync_RetourneProjets_QuandJsonEstValide()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new();
            mockHttp
                .When("http://localhost/data/projects.json")
                .Respond("application/json", JsonFixtures.Projects);

            HttpClient http = new(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            ProjectService service = new(http);

            // Act
            IEnumerable<Project> result = await service.GetProjectsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetProjectsAsync_RetourneListeVide_QuandReponseEst404()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new();
            mockHttp
                .When("http://localhost/data/projects.json")
                .Respond(HttpStatusCode.NotFound);

            HttpClient http = new(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            ProjectService service = new(http);

            // Act
            IEnumerable<Project> result = await service.GetProjectsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProjectsAsync_MappeCorrectementLesProprietes()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new();
            mockHttp
                .When("http://localhost/data/projects.json")
                .Respond("application/json", JsonFixtures.Projects);

            HttpClient http = new(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            ProjectService service = new(http);

            // Act
            IEnumerable<Project> result = await service.GetProjectsAsync();
            Project projet = result.FirstOrDefault();

            // Assert
            Assert.Equal("Dynamics CRM Connector", projet.Nom);
            Assert.Equal("#0078D4", projet.IconColor);
            Assert.Equal("NuGet", projet.ExtraLabel);
            Assert.Contains("C#", projet.Competences);
        }
    }
}