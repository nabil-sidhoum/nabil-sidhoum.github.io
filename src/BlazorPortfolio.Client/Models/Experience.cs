using System;
using System.Collections.Generic;

namespace BlazorPortfolio.Client.Models
{
    public class Experience
    {

        public string Societe { get; set; } = "";
        public string Logo { get; set; } = "";
        public string Poste { get; set; } = "";
        public string Lieu { get; set; } = "";
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }  
        public List<string> Descriptions { get; set; } = [];
        public List<string> Competences { get; set; } = [];
    }
}
