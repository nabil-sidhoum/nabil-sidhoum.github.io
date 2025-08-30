using System;
using System.Collections.Generic;

namespace BlazorPortfolio.Client.Models
{
    public class Education
    {
        public string School { get; set; } = "";
        public string Logo { get; set; } = "";
        public string Diploma { get; set; } = "";
        public string Domain { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }  
        public List<string> Descriptions { get; set; } = [];
    }
}
