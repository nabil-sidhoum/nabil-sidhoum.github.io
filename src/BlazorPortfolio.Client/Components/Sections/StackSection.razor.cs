using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;
using BlazorPortfolio.Client.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorPortfolio.Client.Components.Sections
{
    public partial class StackSection
    {
        [Inject]
        private StackService StackService { get; set; }

        private IEnumerable<StackCategoryInfo> _categories = [];

        protected override async Task OnInitializedAsync()
        {
            _categories = await StackService.GetStackAsync();
        }
    }
}