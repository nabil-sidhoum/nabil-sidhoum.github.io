using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorPortfolio.Client.Components.Nav
{
    public partial class ThemeToggle : IAsyncDisposable
    {
        [Inject]
        private IJSRuntime JS { get; set; }

        private IJSObjectReference _module;
        private string _currentTheme = "dark";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/theme.js");
                _currentTheme = await _module.InvokeAsync<string>("getTheme");
                await _module.InvokeVoidAsync("setTheme", _currentTheme);
            }
        }

        private async Task Toggle()
        {
            _currentTheme = _currentTheme == "dark" ? "light" : "dark";
            await _module.InvokeVoidAsync("setTheme", _currentTheme);
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
            {
                await _module.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }
    }
}