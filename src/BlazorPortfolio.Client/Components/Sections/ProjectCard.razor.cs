using BlazorPortfolio.Client.Models;
using Microsoft.AspNetCore.Components;

namespace BlazorPortfolio.Client.Components.Sections
{
    public partial class ProjectCard
    {
        [Parameter]
        public ProjectInfo Project { get; set; }

        private string BadgeClass => Project.BadgeType == "OpenSource" ? "open" : "show";
        private string BadgeLabel => Project.BadgeType == "OpenSource" ? "Open source" : "Showcase";
        private bool HasLinks => !string.IsNullOrEmpty(Project.GithubUrl) || !string.IsNullOrEmpty(Project.ExtraUrl);
    }
}