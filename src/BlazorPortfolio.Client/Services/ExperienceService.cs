using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;

namespace BlazorPortfolio.Client.Services
{
    public class ExperienceService
    {
        private readonly HttpClient _client;

        public ExperienceService(HttpClient client)
        {
            _client = client;
        }

        public Task<List<Experience>> GetExperiencesAsync()
        {
            return _client.GetFromJsonAsync<List<Experience>>("data/experiences.json")
                ?? Task.FromResult(new List<Experience>());
        }
    }
}