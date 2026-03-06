using System.Collections.Generic;

namespace BlazorPortfolio.Client.Models
{
    public class Project
    {
        public string Nom { get; set; } = "";
        public string IconClass { get; set; } = "bi bi-code-slash";
        public string IconColor { get; set; } = "#1F4E79";
        public string Description { get; set; } = "";
        public string GithubUrl { get; set; } = "";
        public string ExtraUrl { get; set; }
        public string ExtraLabel { get; set; }
        public List<string> Fonctionnalites { get; set; } = [];
        public List<string> Competences { get; set; } = [];
    }
}