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
        private IExperienceService ExperienceService { get; set; }

        private IReadOnlyList<ExperienceInfo> _experiences = [];
        private bool _loading = true;

        protected override async Task OnInitializedAsync()
        {
            _experiences = await ExperienceService.GetExperiencesAsync();
            _loading = false;
        }

        // Génère un pseudo-hash court (style « commit SHA ») affiché en décor à côté de
        // chaque expérience. Implémentation maison volontaire : string.GetHashCode() n'est
        // PAS déterministe entre exécutions/plateformes, ce hash l'est (rendu stable).
        // Le masque 0x0FFFFFFF force un positif tenant sur 7 chiffres hexadécimaux.
        private static string GenerateHash(string seed)
        {
            int hash = 0;
            foreach (char c in seed)
            {
                hash = (hash * 31) + c;
            }

            return (hash & 0x0FFFFFFF).ToString("x7", CultureInfo.InvariantCulture);
        }

        private static bool HasDetails(ExperienceInfo experience)
        {
            return experience.Descriptions?.Count > 0 || experience.Competences?.Count > 0;
        }

        // Libellé du bloc repliable, façon statistique de commit : « 8 lignes », « 1 ligne ».
        private static string LinesLabel(ExperienceInfo experience)
        {
            int count = experience.Descriptions?.Count ?? 0;
            return count == 1 ? "1 ligne" : $"{count} lignes";
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