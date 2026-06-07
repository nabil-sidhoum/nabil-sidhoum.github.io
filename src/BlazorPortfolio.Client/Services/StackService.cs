using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;

namespace BlazorPortfolio.Client.Services
{
    public sealed class StackService
    {
        private readonly HttpClient _http;

        public StackService(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
        }

        public async Task<IEnumerable<StackCategoryInfo>> GetStackAsync()
        {
            try
            {
                IEnumerable<StackCategoryInfo> stack = await _http.GetFromJsonAsync<IEnumerable<StackCategoryInfo>>("data/stack.json");
                return stack ?? [];
            }
            catch (HttpRequestException)
            {
                return [];
            }
        }
    }
}