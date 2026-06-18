using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Models;

namespace BlazorPortfolio.Client.Services
{
    public interface IStackService
    {
        Task<IEnumerable<StackCategoryInfo>> GetStackAsync();
    }
}