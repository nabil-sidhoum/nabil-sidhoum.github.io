using System;

namespace BlazorPortfolio.Client.Services
{
    public interface IUIStateService
    {
        bool IsMobileMenuOpen { get; }

        event Action OnChange;

        void OpenMobileMenu();

        void CloseMobileMenu();
    }
}