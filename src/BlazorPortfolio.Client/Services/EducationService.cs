using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;

namespace BlazorPortfolio.Client.Services
{
    public class EducationService
    {
        private readonly HttpClient _client;

        public EducationService(HttpClient client)
        {
            _client = client;
        }

        public Task<List<Education>> GetEducationsAsync()
        {
            return _client.GetFromJsonAsync<List<Education>>("data/educations.json")
                ?? Task.FromResult(new List<Education>());
        }
    }
}