using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BlazorPortfolio.Client.Layout
{
    public partial class MainLayout : LayoutComponentBase
    {
        [Inject] private IJSRuntime JS { get; set; } = default!;

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
