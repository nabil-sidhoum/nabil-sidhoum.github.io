using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<Experience>> GetExperiencesAsync()
        {
            try
            {
                IEnumerable<Experience> result = await _client.GetFromJsonAsync<List<Experience>>("data/experiences.json");

                return result?.OrderByDescending(e => e.DateDebut) ?? Enumerable.Empty<Experience>();
            }
            catch (HttpRequestException)
            {
                return [];
            }
        }
    }
}