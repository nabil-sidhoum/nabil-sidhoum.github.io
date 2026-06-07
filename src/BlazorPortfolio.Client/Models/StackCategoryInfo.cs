using System.Collections.Generic;

namespace BlazorPortfolio.Client.Models
{
    public sealed class StackCategoryInfo
    {
        public string Key { get; set; }
        public bool IsAccent { get; set; }
        public List<string> Items { get; set; }
    }
}