using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;

namespace BlazorPortfolio.Client.Services
{
    public sealed class ExperienceService : IExperienceService
    {
        private readonly HttpClient _http;
        private List<ExperienceInfo> _cache;

        public ExperienceService(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        public async Task<IReadOnlyList<ExperienceInfo>> GetExperiencesAsync()
        {
            // Données statiques : un seul chargement HTTP, mémorisé pour les appels suivants.
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
                // Résilience : pas de crash si le JSON est absent, la section reste vide.
                return [];
            }
        }
    }
}