using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;

namespace BlazorPortfolio.Client.Services
{
    public sealed class ExperienceService
    {
        private readonly HttpClient _http;
        private List<ExperienceInfo> _cache;

        public ExperienceService(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        public async Task<List<ExperienceInfo>> GetExperiencesAsync()
        {
            if (_cache != null)
            {
                return _cache;
            }

            try
            {
                _cache = await _http.GetFromJsonAsync<List<ExperienceInfo>>("data/experiences.json");
                return _cache ?? [];
            }
            catch (HttpRequestException)
            {
                return [];
            }
        }
    }
}