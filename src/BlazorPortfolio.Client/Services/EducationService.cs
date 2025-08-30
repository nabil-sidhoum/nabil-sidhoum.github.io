using BlazorPortfolio.Client.Data;
using BlazorPortfolio.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorPortfolio.Client.Services;

public class EducationService
{
    public async Task<List<Education>> GetEducationsAsync()
    {
        // Simule un appel asynchrone (comme une vraie API)
        await Task.Delay(500);

        return EducationData.GetEducations();
    }
}
