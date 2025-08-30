using BlazorPortfolio.Client.Models;
using System;
using System.Collections.Generic;

namespace BlazorPortfolio.Client.Data
{
    public static class EducationData
    {
        public static List<Education> GetEducations()
        {
            List<Education> model =
            [
                new Education
                {
                    School = "Sup Galilée – Université Sorbonne Paris Nord",
                    Logo = "images/educations/sup-galilee-logo.jpg",
                    Diploma = "Diplôme d'ingénieur, Télécommunication et Réseau",
                    Domain = "Réseaux et applications distribuées",
                    StartDate = new DateTime(2012, 9, 1),
                    EndDate = new DateTime(2015, 6, 30),
                    Descriptions =
                    [
                        "Formation d’ingénieur orientée logiciels et systèmes distribués, avec une forte base en conception et développement d’applications réseau.",
                        "Projets pratiques incluant le développement de logiciels pour robots et systèmes embarqués (Java), favorisant l’esprit d’ingénierie logicielle et la gestion de projet technique.",
                        "Acquisition de compétences en architecture réseau, protocoles de communication, télécommunications et optimisation de systèmes distribués.",
                        "Renforcement des bonnes pratiques de programmation, test, debug et intégration de systèmes complexes pour des applications variées (mobile, GPS, communication satellite)."
                    ]
                },
                new Education
                {
                    School = "Institut Universitaire Technologique de Paris XIII",
                    Logo = "images/educations/iut-paris13-logo.png",
                    Diploma = "DUT, Mesures Physiques",
                    Domain = "Techniques de mesure et traitement du signal",
                    StartDate = new DateTime(2010, 9, 1),
                    EndDate = new DateTime(2012, 6, 30),
                    Descriptions =
                    [
                        "Formation technique approfondie en mesures physiques, traitement du signal et analyse de données.",
                        "Acquisition de compétences en instrumentation, métrologie et caractérisation des matériaux.",
                        "Initiation aux outils logiciels pour le contrôle qualité et le traitement de données expérimentales.",
                        "Développement de rigueur scientifique et capacité à concevoir des protocoles de mesure précis pour des systèmes complexes."
                    ]
                },
                new Education
                {
                    School = "Lycée André Boulloche",
                    Logo = "images/educations/lycee-boulloche-logo.png",
                    Diploma = "Baccalauréat général",
                    Domain= "Scientifique",
                    StartDate = new DateTime(2005, 9, 1),
                    EndDate = new DateTime(2009, 7, 1),
                    Descriptions =
                    [
                        "Formation générale axée sur les sciences fondamentales, incluant les mathématiques, la physique, la chimie et la biologie.",
                        "Développement de compétences analytiques, rigueur scientifique et capacité à résoudre des problèmes complexes.",
                        "Préparation solide pour des études supérieures en sciences, ingénierie ou technologies."
                    ]
                },
            ];

            return model;
        }
    }
}
