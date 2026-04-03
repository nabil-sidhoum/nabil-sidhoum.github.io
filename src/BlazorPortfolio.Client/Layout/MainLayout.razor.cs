using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorPortfolio.Client.Layout
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject] private IJSRuntime JS { get; set; } = default;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Initialise le scroll helper JS
                await JS.InvokeVoidAsync("scrollHelper.init");
            }
        }
    }
}