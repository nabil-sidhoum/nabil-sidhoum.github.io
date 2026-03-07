namespace BlazorPortfolio.Client.Tests.Fixtures
{
    public static class JsonFixtures
    {
        public const string Experiences = /*lang=json,strict*/ """
            [
              {
                "Societe": "IRBIS",
                "Logo": "/images/experiences/irbis_finance_logo.jpg",
                "Poste": "Directeur technique",
                "Lieu": "Paris 08, Île-de-France",
                "DateDebut": "2025-01-01",
                "DateFin": null,
                "Descriptions": ["Conduite d'un refonte UX/UI."],
                "Competences": [".NET 8", "Clean Architecture"]
              },
              {
                "Societe": "SoftFluent",
                "Logo": "/images/experiences/softfluent_logo.jpg",
                "Poste": "Consultant C#/.Net",
                "Lieu": "Clichy, Île-de-France",
                "DateDebut": "2020-07-31",
                "DateFin": "2021-03-31",
                "Descriptions": ["Client : EIG."],
                "Competences": [".NET Core 3.1"]
              }
            ]
            """;

        public const string Projects = /*lang=json,strict*/ """
            [
              {
                "Nom": "Dynamics CRM Connector",
                "IconClass": "bi bi-plug-fill",
                "IconColor": "#0078D4",
                "Description": "Connecteur HTTP asynchrone.",
                "GithubUrl": "https://github.com/nabil-sidhoum/DynamicsCrmConnector",
                "ExtraUrl": "https://www.nuget.org/packages/Tools.DynamicsCRM",
                "ExtraLabel": "NuGet",
                "Fonctionnalites": ["CRUD complet"],
                "Competences": ["C#", ".NET 8"]
              }
            ]
            """;

        public const string Educations = /*lang=json,strict*/ """
            [
              {
                "School": "Sup Galilée – Université Sorbonne Paris Nord",
                "Logo": "images/educations/sup-galilee-logo.jpg",
                "Diploma": "Diplôme d'ingénieur, Télécommunication et Réseau",
                "Domain": "Réseaux et applications distribuées",
                "StartDate": "2012-09-01",
                "EndDate": "2015-06-30",
                "Descriptions": ["Formation d'ingénieur orientée logiciels et systèmes distribués."]
              }
            ]
            """;
    }
}