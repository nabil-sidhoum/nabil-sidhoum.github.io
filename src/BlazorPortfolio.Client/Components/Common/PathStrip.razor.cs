using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazorPortfolio.Client.Components.Common
{
    public partial class PathStrip : IDisposable
    {
        private const string HomeSegment = "README.md";
        private const string BlogPrefix = "blog/";

        [Inject]
        private NavigationManager Navigation { get; set; }

        private string _currentSegment = HomeSegment;

        protected override void OnInitialized()
        {
            Navigation.LocationChanged += OnLocationChanged;
            UpdateSegment();
        }

        private void OnLocationChanged(object sender, LocationChangedEventArgs args)
        {
            UpdateSegment();
            StateHasChanged();
        }

        private void UpdateSegment()
        {
            string relativePath = Navigation.ToBaseRelativePath(Navigation.Uri);

            int queryIndex = relativePath.IndexOf('?');
            if (queryIndex >= 0)
            {
                relativePath = relativePath[..queryIndex];
            }

            relativePath = relativePath.Trim('/');

            if (string.IsNullOrEmpty(relativePath))
            {
                _currentSegment = HomeSegment;
            }
            else if (string.Equals(relativePath, "blog", StringComparison.OrdinalIgnoreCase))
            {
                _currentSegment = BlogPrefix;
            }
            else if (relativePath.StartsWith(BlogPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string slug = relativePath[BlogPrefix.Length..];
                _currentSegment = $"{BlogPrefix}{slug}.md";
            }
            else
            {
                _currentSegment = relativePath;
            }
        }

        public void Dispose()
        {
            Navigation.LocationChanged -= OnLocationChanged;
            GC.SuppressFinalize(this);
        }
    }
}