using System;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorPortfolio.Client.Components.Nav
{
    public partial class MobileMenu : IAsyncDisposable
    {
        [Inject]
        private UIStateService UIState { get; set; }

        [Inject]
        private IJSRuntime JS { get; set; }

        private IJSObjectReference _module;
        private bool _previousMenuOpen;

        protected override void OnInitialized()
        {
            UIState.OnChange += HandleStateChange;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/scroll.js");
            }

            bool currentOpen = UIState.IsMobileMenuOpen;
            if (currentOpen != _previousMenuOpen && _module != null)
            {
                _previousMenuOpen = currentOpen;
                if (currentOpen)
                {
                    await _module.InvokeVoidAsync("lockScroll");
                }
                else
                {
                    await _module.InvokeVoidAsync("unlockScroll");
                }
            }
        }

        private void HandleStateChange()
        {
            _ = InvokeAsync(StateHasChanged);
        }

        private void Close()
        {
            UIState.CloseMobileMenu();
        }

        public async ValueTask DisposeAsync()
        {
            UIState.OnChange -= HandleStateChange;
            if (_module != null)
            {
                await _module.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }
    }
}