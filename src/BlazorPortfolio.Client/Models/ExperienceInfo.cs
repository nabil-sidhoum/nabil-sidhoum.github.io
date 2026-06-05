using System.Collections.Generic;

namespace BlazorPortfolio.Client.Models
{
    public sealed class ExperienceInfo
    {
        public string Societe { get; set; }
        public string Poste { get; set; }
        public string Lieu { get; set; }
        public string DateDebut { get; set; }
        public string DateFin { get; set; }
        public string Accroche { get; set; }
        public List<string> Descriptions { get; set; }
        public List<string> Competences { get; set; }
    }
}