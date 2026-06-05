using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;
using BlazorPortfolio.Client.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorPortfolio.Client.Components.Sections
{
    public partial class ExperienceSection
    {
        private const int YearLength = 4;

        [Inject]
        private ExperienceService ExperienceService { get; set; }

        private List<ExperienceInfo> _experiences = [];
        private bool _loading = true;

        protected override async Task OnInitializedAsync()
        {
            _experiences = await ExperienceService.GetExperiencesAsync();
            _loading = false;
        }

        private static string GenerateHash(string seed)
        {
            int hash = 0;
            foreach (char c in seed)
            {
                hash = (hash * 31) + c;
            }

            return (hash & 0x0FFFFFFF).ToString("x7", CultureInfo.InvariantCulture);
        }

        private static string YearOf(string date)
        {
            if (string.IsNullOrEmpty(date) || date.Length < YearLength)
            {
                return date;
            }

            return date[..YearLength];
        }
    }
}