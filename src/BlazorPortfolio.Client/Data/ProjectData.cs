using BlazorPortfolio.Client.Models;
using System.Collections.Generic;

namespace BlazorPortfolio.Client.Data
{
    public static class ProjectData
    {
        public static List<Project> GetProjects()
        {
            return
            [
                new Project
                {
                    Nom = "DynamicsCrmConnector",
                    IconClass = "bi bi-plug-fill",
                    IconColor = "#0078D4",
                    Description = "Connecteur HTTP asynchrone pour interagir avec l'API Web OData de Microsoft Dynamics CRM depuis une application .NET Core. Authentification sécurisée via ClientId / SecretId, injection de dépendance intégrée et gestion complète des opérations CRUD.",
                    GithubUrl = "https://github.com/nabil-sidhoum/DynamicsCrmConnector",
                    ExtraUrl = "https://www.nuget.org/packages/Tools.DynamicsCRM",
                    ExtraLabel = "NuGet",
                    Fonctionnalites =
                    [
                        "CRUD complet : Create, Read, Update, Delete",
                        "Authentification OAuth2 applicative (ClientId / SecretId)",
                        "Extension typée avec mapping d'entités fortement typées",
                        "Support FetchXML et pagination automatique",
                        "Publié sur NuGet : Tools.DynamicsCRM & Tools.DynamicsCRM.Extensions"
                    ],
                    Competences = [ "C#", ".NET 8","Dynamics CRM", "HttpClient", "OAuth2", "OData",  "NuGet" ]
                },
                new Project
                {
                    Nom = "OpenstackSwiftConnector",
                    IconClass = "bi bi-cloud-arrow-up-fill",
                    IconColor = "#E95420",
                    Description = "Connecteur HTTP asynchrone pour l'API REST OpenStack Swift, conçu pour interagir avec un espace de stockage objet hébergé chez OVH. Configuration via appsettings.json, injection de dépendance native et renouvellement automatique des tokens.",
                    GithubUrl = "https://github.com/nabil-sidhoum/OpenstackSwiftConnector",
                    ExtraUrl = "https://www.nuget.org/packages/Tools.Swift.Connector",
                    ExtraLabel = "NuGet",
                    Fonctionnalites =
                    [
                        "Lister les conteneurs et fichiers disponibles",
                        "Upload de fichiers (Create / Replace) sur un conteneur",
                        "Authentification OVH OpenStack avec renouvellement automatique du token",
                        "Interface ISwiftClient injectable via DI",
                        "Publié sur NuGet : Tools.Swift.Connector"
                    ],
                    Competences = [ "C#", ".NET 8", "HttpClient", "OpenStack Swift", "OVH", "REST API", "NuGet" ]
                },
                new Project
                {
                    Nom = "Portfolio Blazor WebAssembly",
                    IconClass = "bi bi-person-vcard-fill",
                    IconColor = "#512BD4",
                    Description = "Ce portfolio est lui-même un projet technique : une Single Page Application développée en Blazor WebAssembly, déployée automatiquement sur GitHub Pages via un pipeline CI/CD GitHub Actions. Architecture en composants réutilisables, responsive mobile.",
                    GithubUrl = "https://github.com/nabil-sidhoum/nabil-sidhoum.github.io",
                    Fonctionnalites =
                    [
                        "Single Page Application Blazor WebAssembly",
                        "Déploiement automatique via GitHub Actions (CI/CD)",
                        "Architecture en composants Razor réutilisables",
                        "Responsive mobile avec menu burger",
                        "Zéro dépendance JavaScript externe"
                    ],
                    Competences = [ "C#", "Blazor WebAssembly", ".NET 8", "GitHub Actions", "CI/CD", "HTML/CSS" ]
                }
            ];
        }
    }
}
