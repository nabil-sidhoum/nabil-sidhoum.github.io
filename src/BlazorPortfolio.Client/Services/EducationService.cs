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

        public async Task<IEnumerable<Education>> GetEducationsAsync()
        {
            try
            {
                IEnumerable<Education> result = await _client.GetFromJsonAsync<List<Education>>("data/educations.json");
                return result ?? [];
            }
            catch (HttpRequestException)
            {
                return [];
            }
        }
    }
}