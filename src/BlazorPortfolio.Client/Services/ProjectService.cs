using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;

namespace BlazorPortfolio.Client.Services
{
    public sealed class ProjectService
    {
        private readonly HttpClient _http;

        public ProjectService(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        public async Task<IEnumerable<ProjectInfo>> GetProjectsAsync()
        {
            try
            {
                IEnumerable<ProjectInfo> projects = await _http.GetFromJsonAsync<IEnumerable<ProjectInfo>>("data/projects.json");
                return projects ?? [];
            }
            catch (HttpRequestException)
            {
                // Résilience : un JSON injoignable renvoie une liste vide plutôt qu'une erreur.
                return [];
            }
        }
    }
}