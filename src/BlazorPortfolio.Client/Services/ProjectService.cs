using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;

namespace BlazorPortfolio.Client.Services
{
    public class ProjectService
    {
        private readonly HttpClient _client;

        public ProjectService(HttpClient client)
        {
            _client = client;
        }

        public Task<List<Project>> GetProjectsAsync()
        {
            return _client.GetFromJsonAsync<List<Project>>("data/projects.json")
                ?? Task.FromResult(new List<Project>());
        }
    }
}