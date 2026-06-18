using System;

namespace BlazorPortfolio.Client.Services
{
    // État d'UI partagé (Scoped) entre composants qui ne sont pas dans la même
    // hiérarchie de rendu : Nav ouvre le menu mobile, MobileMenu y réagit.
    // Le pattern observable (OnChange) évite de chaîner des [Parameter] entre eux.
    public sealed class UIStateService : IUIStateService
    {
        public bool IsMobileMenuOpen { get; private set; }

        public event Action OnChange;

        public void OpenMobileMenu()
        {
            IsMobileMenuOpen = true;
            OnChange?.Invoke();
        }

        public void CloseMobileMenu()
        {
            IsMobileMenuOpen = false;
            OnChange?.Invoke();
        }
    }
}