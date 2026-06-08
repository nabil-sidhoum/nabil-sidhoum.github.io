using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorPortfolio.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorPortfolio.Client
{
    public sealed class Program
    {
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            // HttpClient pointé sur la racine du site : les données (data/*.json, posts/*)
            // sont servies statiquement par GitHub Pages et consommées comme une API REST.
            // Aucun backend ni API externe — d'où l'absence d'IHttpClientFactory ici.
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // En WebAssembly une « session » = une instance d'app : Scoped se comporte
            // comme un singleton applicatif, ce qui rend le cache des services cohérent.
            builder.Services.AddScoped<UIStateService>();
            builder.Services.AddScoped<ProjectService>();
            builder.Services.AddScoped<ExperienceService>();
            builder.Services.AddScoped<BlogService>();
            builder.Services.AddScoped<StackService>();

            await builder.Build().RunAsync();
        }
    }
}