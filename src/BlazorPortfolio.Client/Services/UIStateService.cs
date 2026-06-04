using System;

namespace BlazorPortfolio.Client.Services
{
    public sealed class UIStateService
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