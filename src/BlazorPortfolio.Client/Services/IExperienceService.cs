using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;

namespace BlazorPortfolio.Client.Services
{
    public interface IExperienceService
    {
        Task<List<ExperienceInfo>> GetExperiencesAsync();
    }
}