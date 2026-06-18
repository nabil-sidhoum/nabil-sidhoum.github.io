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
        private IUIStateService UIState { get; set; }

        [Inject]
        private IJSRuntime JS { get; set; }

        private IJSObjectReference _module;
        private bool _previousMenuOpen;

        protected override void OnInitialized()
        {
            // S'abonne à l'état partagé : quand Nav ouvre/ferme le menu, ce composant re-rend.
            UIState.OnChange += HandleStateChange;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/scroll.js");
            }

            // On ne touche au scroll du body que sur un VRAI changement d'état
            // (garde _previousMenuOpen) : sinon chaque re-rendu rejouerait lock/unlock.
            bool currentOpen = UIState.IsMobileMenuOpen;
            if (currentOpen != _previousMenuOpen && _module != null)
            {
                _previousMenuOpen = currentOpen;
                if (currentOpen)
                {
                    // Menu plein écran ouvert : on fige le scroll de la page derrière.
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
            // Désabonnement indispensable : sans lui, le service Scoped garderait une
            // référence vers ce composant détruit (fuite mémoire).
            UIState.OnChange -= HandleStateChange;
            if (_module != null)
            {
                await _module.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }
    }
}