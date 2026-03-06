using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Data;
using BlazorPortfolio.Client.Models;

namespace BlazorPortfolio.Client.Services
{
    public class ProjectService
    {
        public async Task<List<Project>> GetProjectsAsync()
        {
            await Task.Delay(500);
            return ProjectData.GetProjects();
        }
    }
}