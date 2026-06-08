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
            // JS interop seulement après le 1er rendu : le DOM (les sections observées
            // par le scroll-spy) doit exister avant d'initialiser l'IntersectionObserver.
            if (firstRender)
            {
                // DotNetObjectReference permet au module JS de rappeler OnSectionVisible (JS → C#).
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
            // Ordre important : on coupe l'observer JS et on libère le module AVANT le
            // DotNetObjectReference, pour qu'aucun callback ne vise un objet déjà disposé.
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