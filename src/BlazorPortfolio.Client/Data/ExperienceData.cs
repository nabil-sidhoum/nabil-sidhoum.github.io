using BlazorPortfolio.Client.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BlazorPortfolio.Client.Data
{
    public static class ExperienceData
    {
        public static List<Experience> GetExperiences()
        {
            List<Experience> model = [];

            model.AddRange(GetIRBISExperiences());
            model.AddRange(GetSoftFluentExperiences());
            model.AddRange(GetCapgeminiExperiences());

            return model.OrderByDescending(exp => exp.DateDebut).ToList();
        }

        public static List<Experience> GetIRBISExperiences()
        {
            string irbisLogo = "/images/experiences/irbis_finance_logo.jpg";

            List<Experience> model =
            [
                new Experience
                {
                    Societe = "IRBIS",
                    Logo = irbisLogo,
                    Poste = "Directeur technique",
                    Lieu = "Paris 08, Île-de-France",
                    DateDebut = new DateTime(2025, 1, 1),
                    Descriptions =
                    [
                        "Conduite d'un refonte UX/UI.",
                        "Mise en place d’une démarche DevSecOps dans Azure DevOps afin d’intégrer la sécurité et l’analyse de code (via IntelliSense) directement dans les pipelines CI/CD.",
                        "Refonte et unification des pipelines de déploiement pour fiabiliser et automatiser la livraison des projets.",
                        "Création d’un repository NuGet interne permettant de mutualiser et de standardiser les outils et librairies maison.",
                        "Introduction du versioning SemVer afin de garantir la traçabilité des versions, la compatibilité ascendante et une gestion plus professionnelle des livrables."
                    ],
                    Competences =
                    [
                        ".NET 8",
                        "Clean Architecture",
                        "ASP.NET Core MVC",
                        "Entity Framework Core",
                        "ASP.NET Core Identity",
                        "CRM Dynamics",
                        "Hangfire",
                        "Swagger",
                        "PuppeteerSharp",
                        "SQL Server",
                        "Revue de code",
                        "Gestion d’équipe",
                        "Gestion de projet",
                        "Leadership technique",
                    ]
                },
                new Experience
                {
                    Societe = "IRBIS",
                    Logo = irbisLogo,
                    Poste = "Directeur technique",
                    Lieu = "Paris 08, Île-de-France",
                    DateDebut = new DateTime(2024, 1, 1),
                    DateFin = new DateTime(2024, 12, 31),
                    Descriptions =
                    [
                        "Pilotage de l’évolution de l’architecture logicielle vers un modèle scalable, maintenable et sécurisé.",
                        "Migration de .NET 6 vers .NET 8 et adoption des nouvelles fonctionnalités du framework.",
                        "Optimisation des performances et de la scalabilité applicative (requêtes EF Core, architecture).",
                        "Mise en place de solutions d’observabilité (logs structurés) pour améliorer la supervision.",
                        "Renforcement de la sécurité applicative et des API (authentification, autorisation, conformité).",
                        "Encadrement technique et transfert de compétences auprès des développeurs."
                    ],
                    Competences =
                    [
                        ".NET 8",
                        "Clean Architecture",
                        "ASP.NET Core MVC",
                        "Entity Framework Core",
                        "ASP.NET Core Identity",
                        "CRM Dynamics",
                        "Hangfire",
                        "Swagger",
                        "PuppeteerSharp",
                        "SQL Server",
                        "Revue de code",
                        "Gestion d’équipe",
                        "Gestion de projet",
                        "Leadership technique",
                    ]
                },
                new Experience
                {
                    Societe = "IRBIS",
                    Logo = irbisLogo,
                    Poste = "Technical Leader",
                    Lieu = "Paris 08, Île-de-France",
                    DateDebut = new DateTime(2023, 1, 1),
                    DateFin = new DateTime(2023, 12, 31),
                    Descriptions =
                    [
                        "Migration de .NET 5 vers .NET 6 et adaptation de l’architecture aux nouvelles capacités du framework.",
                        "Mise en place de standards techniques (revues de code, pratiques de développement).",
                        "Formation et accompagnement d’un développeur junior.",
                        "Supervision de l’automatisation des processus métiers et amélioration continue."
                    ],
                    Competences =
                    [
                        ".NET 6",
                        "Clean Architecture",
                        "ASP.NET Core MVC",
                        "Entity Framework Core",
                        "ASP.NET Core Identity",
                        "CRM Dynamics",
                        "Hangfire",
                        "Swagger",
                        "SelectPDF",
                        "SQL Server",
                        "Revue de code",
                        "Gestion d’équipe",
                        "Gestion de projet"
                    ]
                },
                new Experience
                {
                    Societe = "IRBIS",
                    Logo = irbisLogo,
                    Poste = "Technical Leader",
                    Lieu = "Paris 08, Île-de-France",
                    DateDebut = new DateTime(2022, 1, 1),
                    DateFin = new DateTime(2022, 12, 31),
                    Descriptions =
                    [
                        "Conception et mise en œuvre de l’architecture logicielle définie en 2021.",
                        "Migration de .NET Framework 4.6.1 vers .NET 5, garantissant performance et compatibilité.",
                        "Mise en place d’une architecture modulaire et évolutive.",
                        "Encadrement technique et recrutement de développeurs pour soutenir la montée en charge.",
                        "Rédaction et maintien de la documentation d’architecture (diagrammes, décisions techniques)."
                    ],
                    Competences =
                    [
                        ".NET 5",
                        "Clean Architecture",
                        "ASP.NET Core MVC",
                        "Entity Framework Core",
                        "ASP.NET Core Identity",
                        "CRM Dynamics",
                        "Hangfire",
                        "Swagger",  
                        "SelectPDF",
                        "SQL Server",
                        "Revue de code",
                        "Gestion d’équipe"
                    ]
                },
                new Experience
                {
                    Societe = "IRBIS",
                    Logo = irbisLogo,
                    Poste = "Technical Leader",
                    Lieu = "Paris 08, Île-de-France",
                    DateDebut = new DateTime(2021, 4, 1),
                    DateFin = new DateTime(2021, 12, 31),
                    Descriptions =
                    [
                        "Audit technique complet de la plateforme existante.",
                        "Rédaction des plans d’architecture logicielle : définition des choix technologiques (Clean Architecture, modularité).",
                        "Conduite d’une refonte UX/UI et cadrage technique pour moderniser l’existant.",
                        "Élaboration d’une feuille de route technique."
                    ],
                    Competences =
                    [
                        ".NET Framework 4.6.1",
                        "Audit technique",
                        "ASP.NET MVC",
                        "CRM Dynamics",
                        "Entity Framework",
                        "ASP.NET Identity",
                        "Hangfire",
                        "SelectPDF",
                        "SQL Server"
                    ]
                }
            ];

            return model;
        }

        public static List<Experience> GetSoftFluentExperiences()
        {
            string softfluentLogo = "/images/experiences/softfluent_logo.jpg";

            List<Experience> model =
            [
                new Experience
                {
                    Societe = "SoftFluent",
                    Logo = softfluentLogo,
                    Poste = "Consultant C#/.Net",
                    Lieu = "Clichy, Île-de-France, France",
                    DateDebut = new DateTime(2020, 7, 31),
                    DateFin = new DateTime(2021, 3, 31),
                    Descriptions =
                    [
                        "Client : EIG.",
                        "Étude et conception d’une nouvelle architecture logicielle basée sur les microservices en .NET Core pour remplacer une architecture vieillissante développée en Delphi.",
                        "Définition des bonnes pratiques de développement et mise en place des standards de qualité du code.",
                        "Accompagnement et formation des équipes internes sur .NET Core et l’approche microservices.",
                        "Développement de composants techniques clés pour valider et illustrer l’architecture cible."
                    ],
                    Competences =
                    [
                        ".NET Core 3.1",
                        "Architecture microservice",
                        "Entity Framework",
                        "SQL Server",
                    ]
                },
                new Experience
                {
                    Societe = "SoftFluent",
                    Logo = softfluentLogo,
                    Poste = "Consultant C#/.Net",
                    Lieu = "Paris 08, Île-de-France, France",
                    DateDebut = new DateTime(2020, 1, 1),
                    DateFin = new DateTime(2020, 04, 30),
                    Descriptions =
                    [
                        "Client : Irbis Finance.",
                        "Identification des besoins client et analyse fonctionnelle.",
                        "Découpage et estimation des tâches techniques.",
                        "Encadrement technique et transfert de compétences auprès des équipes internes.",
                    ],
                    Competences =
                    [
                        ".NET Framework 4.6.1",
                        "ASP.NET MVC",
                        "CRM Dynamics",
                        "Entity Framework",
                        "ASP.NET Identity",
                        "SQL Server",
                        "Hangfire", 
                        "SelectPDF"
                    ]
                },
                new Experience
                {
                    Societe = "SoftFluent",
                    Logo = softfluentLogo,
                    Poste = "Consultant C#/.Net",
                    Lieu = "Paris 10, Île-de-France, France",
                    DateDebut = new DateTime(2019, 6, 1),
                    DateFin = new DateTime(2019, 12, 31),
                    Descriptions =
                    [
                        "Client : Groupe IGS.",
                        "Traitement des anomalies et correction des bugs sur les applications internes.",
                        "Optimisation des performances et des ressources des logiciels existants.",
                        "Migration des applications de .NET Framework 4.5.2 vers .NET Framework 4.7.2.",
                        "Mise en place d'un repository Git et gestion des versions avec Azure DevOps.",
                    ],
                    Competences =
                    [
                        ".NET Framework 4.5.1",
                        "ASP.NET Winforms",
                        "SQL Server"
                    ]
                },
                new Experience
                {
                    Societe = "SoftFluent",
                    Logo = softfluentLogo,
                    Poste = "Consultant C#/.Net Junior",
                    Lieu = "Paris 08, Île-de-France, France",
                    DateDebut = new DateTime(2018, 4, 1),
                    DateFin = new DateTime(2019, 4, 30),
                    Descriptions =
                    [
                        "Client : Irbis Finance.",
                        "Identification des besoins client et analyse fonctionnelle.",
                        "Étude de faisabilité technique par rapport aux besoins fonctionnels.",
                        "Découpage et estimation des tâches techniques.",
                        "Conception du MVP (Minimum Viable Product) pour valider rapidement les fonctionnalités essentielles.",
                        "Mise en place de l’architecture applicative et développement technique des fonctionnalités en ASP.NET MVC."
                    ],
                    Competences =
                    [
                        ".NET Framework 4.6.1",
                        "ASP.NET MVC",
                        "CRM Dynamics",
                        "Entity Framework",
                        "ASP.NET Identity",
                        "SQL Server",
                        "Hangfire",
                        "SelectPDF"
                    ]
                },
                new Experience
                {
                    Societe = "SoftFluent",
                    Logo = softfluentLogo,
                    Poste = "Consultant C#/.Net Junior",
                    Lieu = "Paris 08, Île-de-France, France",
                    DateDebut = new DateTime(2017, 10, 1),
                    DateFin = new DateTime(2018, 3, 31),
                    Descriptions =
                    [
                        "Client : Linxea.",
                        "Identification des besoins client et rédaction des spécifications techniques.",
                        "Découpage et estimation des tâches techniques.",
                        "Mise en place de l’architecture applicative du site partenaire.",
                        "Développement des fonctionnalités en ASP.NET MVC."
                    ],
                    Competences =
                    [
                        ".NET Framework 4.6.1",
                        "ASP.NET MVC",
                        "CRM Dynamics",
                        "Entity Framework",
                        "ASP.NET Identity",
                        "SQL Server"
                    ]
                }
            ];

            return model;
        }

        public static List<Experience> GetCapgeminiExperiences()
        {
            string capgeminiLogo = "/images/experiences/capgemini_logo.jpg";

            List<Experience> model =
            [
                new Experience
                {
                    Societe = "Capgemini",
                    Logo = capgeminiLogo,
                    Poste = "Consultant C#/.Net",
                    Lieu = "Paris 09, Île-de-France, France",
                    DateDebut = new DateTime(2017, 7, 1),
                    DateFin = new DateTime(2017, 9, 30),
                    Descriptions =
                    [
                        "Client : Itelios/Domus VI.",
                        "Maintenance applicative du site ASP.NET dédié aux résidents des maisons de retraite Domus VI.",
                        "Support et évolutions correctives sur le web service WCF d’accès aux données pour les applications mobiles.",
                        "Amélioration de la fiabilité et de la stabilité des services existants."
                    ],
                    Competences =
                    [
                        ".NET Framework 4.6.1",
                        "SQL Server",
                    ]
                },
                new Experience
                {
                    Societe = "Capgemini",
                    Logo = capgeminiLogo,
                    Poste = "Consultant Dynamics CRM",
                    Lieu = "Paris 08, Île-de-France, France",
                    DateDebut = new DateTime(2017, 3, 1),
                    DateFin = new DateTime(2017, 6, 30),
                    Descriptions =
                    [
                        "Client : Unibail-Rodamco.",
                        "Mise en place d’une infrastructure DevOps pour accompagner le développement du CRM personnalisé.",
                        "Découpage et estimation des tâches techniques.",
                        "Développement et intégration des solutions techniques en .NET."
                    ],
                    Competences =
                    [
                        ".NET Framework 4.5.1",
                        "CRM Dynamics",
                        "HTML",
                        "Javascripts",
                        "CSS"
                    ]
                },
                new Experience
                {
                    Societe = "Capgemini",
                    Logo = capgeminiLogo,
                    Poste = "Consultant Dynamics CRM",
                    Lieu = "Suresnes, Île-de-France, France",
                    DateDebut = new DateTime(2015, 10, 1),
                    DateFin = new DateTime(2017, 1, 31),
                    Descriptions =
                    [
                        "Client : Orange Caraïbes & Réunion-Mayotte.",
                        "Découpage et estimation des tâches techniques.",
                        "Mise en place de l’architecture applicative pour le CRM personnalisé.",
                        "Conception et développement de plusieurs web services WCF.",
                        "Encadrement et coordination d’une équipe de 10 développeurs."
                    ],
                    Competences =
                    [
                        ".NET Framework 4.5.1",
                        "CRM Dynamics",
                        "HTML",
                        "Javascripts",
                        "CSS"
                    ]
                },
                new Experience
                {
                    Societe = "Capgemini",
                    Logo = capgeminiLogo,
                    Poste = "Consultant stagiaire Dynamics CRM",
                    Lieu = "Suresnes, Île-de-France, France",
                    DateDebut = new DateTime(2015, 3, 1),
                    DateFin = new DateTime(2017, 8, 31),
                    Descriptions =
                    [
                        "Client : Prosodie.",
                        "Analyse de faisabilité technique en fonction des besoins fonctionnels pour un projet de couplage téléphonie-informatique (CTI).",
                        "Découpage et estimation des tâches techniques.",
                        "Mise en place de l’architecture applicative.",
                        "Développement et intégration des solutions techniques en .NET."
                    ],
                    Competences =
                    [
                        ".NET Framework 4.5.1",
                        "CRM Dynamics",
                        "Application WPF"
                    ]
                },


            ];

            return model;
        }
    }
}
