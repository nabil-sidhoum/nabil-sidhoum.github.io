using BlazorPortfolio.Client.Services;
using Xunit;

namespace BlazorPortfolio.Client.Tests
{
    public class UIStateServiceTests
    {
        [Fact]
        public void OpenMobileMenu_OuvreLeMenu_EtDeclencheOnChange()
        {
            // Arrange
            UIStateService service = new UIStateService();
            bool eventDeclenche = false;
            service.OnChange += () => eventDeclenche = true;

            // Act
            service.OpenMobileMenu();

            // Assert
            Assert.True(service.IsMobileMenuOpen);
            Assert.True(eventDeclenche);
        }

        [Fact]
        public void CloseMobileMenu_FermeLeMenu_EtDeclencheOnChange()
        {
            // Arrange
            UIStateService service = new UIStateService();
            service.OpenMobileMenu();
            bool eventDeclenche = false;
            service.OnChange += () => eventDeclenche = true;

            // Act
            service.CloseMobileMenu();

            // Assert
            Assert.False(service.IsMobileMenuOpen);
            Assert.True(eventDeclenche);
        }

        [Fact]
        public void IsMobileMenuOpen_EstFalse_ParDefaut()
        {
            // Arrange
            UIStateService service = new UIStateService();

            // Act & Assert
            Assert.False(service.IsMobileMenuOpen);
        }
    }
}