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
    public class ProjectServiceTests
    {
        [Fact]
        public async Task GetProjectsAsync_RetourneProjects_QuandJsonEstValide()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/data/projects.json")
                    .Respond("application/json", JsonFixtures.Projects);
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            ProjectService service = new ProjectService(http);

            // Act
            IEnumerable<ProjectInfo> result = await service.GetProjectsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetProjectsAsync_RetourneListeVide_QuandReponseEst404()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/data/projects.json")
                    .Respond(HttpStatusCode.NotFound);
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            ProjectService service = new ProjectService(http);

            // Act
            IEnumerable<ProjectInfo> result = await service.GetProjectsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetProjectsAsync_MappeCorrectement_LeChampNom()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/data/projects.json")
                    .Respond("application/json", JsonFixtures.Projects);
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            ProjectService service = new ProjectService(http);

            // Act
            IEnumerable<ProjectInfo> result = await service.GetProjectsAsync();

            // Assert
            ProjectInfo premier = Assert.Single(result);
            Assert.Equal("ProjetTest", premier.Nom);
        }

        [Fact]
        public async Task GetProjectsAsync_MappeCorrectement_LesBadgeEtCompetences()
        {
            // Arrange
            MockHttpMessageHandler mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/data/projects.json")
                    .Respond("application/json", JsonFixtures.ProjectsOpenSource);
            HttpClient http = new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost/") };
            ProjectService service = new ProjectService(http);

            // Act
            IEnumerable<ProjectInfo> result = await service.GetProjectsAsync();

            // Assert
            ProjectInfo premier = Assert.Single(result);
            Assert.Equal("OpenSource", premier.BadgeType);
            Assert.Equal("https://github.com/test/projet", premier.GithubUrl);
            Assert.Equal(2, premier.Competences.Count);
            Assert.Equal("C#", premier.Competences.First());
        }
    }
}