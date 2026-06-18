using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;
using BlazorPortfolio.Client.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorPortfolio.Client.Components.Sections
{
    public partial class ProjectsSection
    {
        [Inject]
        private IProjectService ProjectService { get; set; }

        private IEnumerable<ProjectInfo> _projects = [];

        protected override async Task OnInitializedAsync()
        {
            _projects = await ProjectService.GetProjectsAsync();
        }
    }
}