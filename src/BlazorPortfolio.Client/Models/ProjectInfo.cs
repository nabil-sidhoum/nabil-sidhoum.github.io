using System.Collections.Generic;

namespace BlazorPortfolio.Client.Models
{
    public sealed class ProjectInfo
    {
        public string Nom { get; set; }
        public string BadgeType { get; set; }
        public string Description { get; set; }
        public string GithubUrl { get; set; }
        public string ExtraUrl { get; set; }
        public string ExtraLabel { get; set; }
        public List<string> Competences { get; set; }
    }
}