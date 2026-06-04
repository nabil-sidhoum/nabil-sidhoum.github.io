using System;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorPortfolio.Client.Components.Nav
{
    public partial class Nav : IAsyncDisposable
    {
        [Inject]
        private UIStateService UIState { get; set; }

        [Inject]
        private IJSRuntime JS { get; set; }

        private IJSObjectReference _module;
        private DotNetObjectReference<Nav> _dotnetRef;
        private string _activeSection = string.Empty;

        private string MenuExpanded => UIState.IsMobileMenuOpen ? "true" : "false";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _dotnetRef = DotNetObjectReference.Create(this);
                _module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/scroll-spy.js");
                await _module.InvokeVoidAsync("init", _dotnetRef);
            }
        }

        [JSInvokable]
        public void OnSectionVisible(string id)
        {
            _activeSection = id;
            _ = InvokeAsync(StateHasChanged);
        }

        private string ActiveClass(string id)
        {
            return _activeSection == id ? "active" : string.Empty;
        }

        private void OpenMenu()
        {
            UIState.OpenMobileMenu();
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
            {
                await _module.InvokeVoidAsync("dispose");
                await _module.DisposeAsync();
            }

            _dotnetRef?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}