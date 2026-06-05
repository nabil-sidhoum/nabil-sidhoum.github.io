namespace BlazorPortfolio.Client.Tests
{
    public static class JsonFixtures
    {
        public static readonly string Projects = /*lang=json,strict*/ """
            [
                {
                    "Nom": "ProjetTest",
                    "BadgeType": "Showcase",
                    "Description": "Description du projet test.",
                    "GithubUrl": null,
                    "ExtraUrl": null,
                    "ExtraLabel": null,
                    "Competences": [".NET 10", "C# 14"]
                }
            ]
            """;

        public static readonly string Experiences = /*lang=json,strict*/ """
            [
                {
                    "Societe": "IRBIS Finance",
                    "Poste": "Tech Lead .NET",
                    "Lieu": "Paris 08",
                    "DateDebut": "2021-07-01",
                    "DateFin": null,
                    "Accroche": "Description test.",
                    "Descriptions": [],
                    "Competences": ["C#", ".NET 8"]
                },
                {
                    "Societe": "SoftFluent",
                    "Poste": "Consultant .NET",
                    "Lieu": "Paris",
                    "DateDebut": "2017-10-01",
                    "DateFin": "2021-06-30",
                    "Accroche": "Mission test.",
                    "Descriptions": [],
                    "Competences": [".NET Core"]
                }
            ]
            """;

        public static readonly string ProjectsOpenSource = /*lang=json,strict*/ """
            [
                {
                    "Nom": "ProjetOpen",
                    "BadgeType": "OpenSource",
                    "Description": "Projet open source de test.",
                    "GithubUrl": "https://github.com/test/projet",
                    "ExtraUrl": "https://nuget.org/packages/test",
                    "ExtraLabel": "NuGet",
                    "Competences": ["C#", ".NET 8"]
                }
            ]
            """;
    }
}