# Mon alarme de dépendances a sonné quatre jours avant la panne

*Publié le 19 octobre 2026 — Nabil Sidhoum, Senior .NET Tech Lead*

---

En juin, j'ai raconté [comment ma CI avait cassé sans que je touche une ligne de code](/blog/ci-cassee-dette-invisible) : une advisory tombée sur une dépendance transitive de SQLite, révélée par l'audit NuGet de .NET 10 et transformée en erreur par `TreatWarningsAsErrors`. J'avais refusé de faire taire l'alarme, et je concluais que le silence, en ingénierie, est rarement une bonne nouvelle.

Jeudi dernier, l'alarme a de nouveau sonné. Même projet, `veille-tech`, même mécanisme, autre dépendance transitive. Et toujours aucun commit de ma part. Une différence pourtant, et c'est toute l'histoire de cet article : cette fois, ce n'est pas la production qui me l'a appris. C'est un job que j'avais ajouté exprès pour ça, et il a prévenu quatre jours avant que la panne ne touche quoi que ce soit.

---

## Ce que juin m'avait vraiment appris

En relisant l'incident de juin, le défaut le plus gênant n'était pas la faille elle-même. C'était le détecteur. Le premier à avoir vu le problème, c'était le run `Veille .NET` du lundi, puis celui du mardi : mes runs de production. Un projet qui ne bouge pas n'est compilé que lorsqu'il tourne, et l'audit NuGet ne s'exécute qu'à la restauration. Autrement dit, l'alarme existait, mais elle n'était branchée que sur la production.

Fin juillet, rebelote : une autre advisory, cette fois sur `System.Security.Cryptography.Xml` 10.0.8, bloquait la restauration et empêchait toute branche de correction de compiler. Je l'ai corrigée par un override en 10.0.10. Et le lendemain, j'ai ajouté au dépôt un workflow de CI avec une ligne qui compte plus que les autres :

```yaml
on:
  push:
    branches: [main, 'integration/**']
  pull_request:
  schedule:
    # Quotidien : révèle les CVE sur les dépendances même sans activité de commit.
    - cron: '0 6 * * *'
```

Dans l'article de juin, j'avais listé plusieurs pistes pour industrialiser la détection : un pipeline d'audit séparé, Central Package Management, Dependabot ou Renovate. J'ai retenu la plus simple. Pas de nouvel outil, pas de nouvelle configuration NuGet : le même build, la même restauration, le même `TreatWarningsAsErrors`, simplement déclenchés tous les jours, même quand personne ne pousse rien. Un run vert prend moins d'une minute.

---

## Ce qui s'est passé cette semaine

Mardi 13 octobre au soir, Microsoft publie une série d'avis de sécurité .NET. Trois concernent `System.Security.Cryptography.Xml` de 10.0.0 à 10.0.12, deux autres sa dépendance `System.Security.Cryptography.Pkcs`. Ce sont des dénis de service, quatre notés élevés et un modéré.

Le mercredi, le run quotidien passe encore au vert, sans doute parce que les avis ne sont pas encore remontés jusqu'à l'audit NuGet. Le jeudi 15, il tombe en 29 secondes, à l'étape `Restore`, sur un commit identique à celui de la veille :

```
error NU1903: Warning As Error: Package 'System.Security.Cryptography.Xml' 10.0.10
has a known high severity vulnerability, https://github.com/advisories/GHSA-7hqp-7c5m-4vq6
error NU1902: Warning As Error: Package 'System.Security.Cryptography.Pkcs' 10.0.10
has a known moderate severity vulnerability, https://github.com/advisories/GHSA-2fmv-rv2c-59x7
```

GitHub m'envoie la notification d'échec. Le vendredi, le samedi et le dimanche, le run échoue de nouveau : un rappel par jour. Pendant ce temps, aucun profil de veille ne tourne, le prochain étant celui du lundi.

![Frise du 13 au 19 octobre 2026 : la CI quotidienne échoue du 15 au 18 pendant qu'aucun run de veille n'est prévu, puis le correctif du lundi matin remet la CI et la veille .NET au vert](/posts/assets/alerte-avant-la-panne.svg)

Lundi 19 au matin, le correctif est mergé. La veille .NET de l'après-midi passe au vert, comme si de rien n'était. De son point de vue, il ne s'est rien passé.

---

## Le correctif de juillet était devenu la faille d'octobre

Le détail qui m'a fait sourire : le paquet en cause, je l'avais déjà touché. La version 10.0.10 était précisément celle que j'avais épinglée en juillet pour corriger la 10.0.8. Je ne l'ai jamais choisie pour elle-même, d'ailleurs. `dotnet nuget why` montre d'où elle vient :

```
Microsoft.EntityFrameworkCore.Design (v10.0.0)
├── Microsoft.Build.Tasks.Core (v17.14.28)
│   └── System.Security.Cryptography.Xml
└── Microsoft.CodeAnalysis.Workspaces.MSBuild (v4.14.0)
    └── System.Security.Cryptography.Xml
```

C'est l'outillage de design d'EF Core, celui qui sert aux migrations, qui tire MSBuild, qui tire cette bibliothèque XML. Une dépendance de dépendance d'outillage.

Un override, c'est une version figée. Il corrige la faille du jour, puis il vieillit en silence pendant que le reste de l'écosystème avance. En juin, j'écrivais que mon override était « à surveiller à la main ». Soyons honnêtes : personne ne surveille un override à la main. Ce qui l'a surveillé, c'est le cron.

La correction a tenu en une ligne : passer l'override de 10.0.10 à 10.0.13, qui tire `Pkcs` 10.0.13 avec lui, sans second override. J'ai complété le commentaire du csproj avec les nouveaux identifiants d'avis, pour que la raison de cette version reste lisible au prochain passage. Vérification faite, `dotnet list package --vulnerable --include-transitive` ne remonte plus aucun paquet vulnérable.

Et le risque réel ? Faible, pour être franc. Un déni de service dans une bibliothèque XML embarquée par l'outillage de migration, sur un batch qui lit des flux RSS, c'est loin d'un scénario d'attaque crédible. Mais je reste sur le parti pris de juin : je ne trie pas les alarmes à l'intuition. Retirer la version vulnérable coûtait une ligne, la faire taire en aurait coûté une aussi, et seule la première laisse l'alarme armée.

---

## Prévenir plutôt que guérir, en chiffres

Même mécanisme, même projet, deux détecteurs différents :

| | Juin (SQLite) | Octobre (Cryptography.Xml) |
|---|---|---|
| **Qui a vu la panne en premier** | Le run de veille du lundi | La CI quotidienne du jeudi |
| **Runs de production en échec** | Deux (lundi et mardi) | Aucun |
| **Marge entre l'alerte et l'impact** | Aucune : l'alerte était l'impact | Quatre jours |
| **Conditions du correctif** | Production arrêtée | Lundi matin, avant le run |
| **Coût du dispositif** | Aucun dispositif dédié | Moins d'une minute de CI par jour |

Le dispositif n'a rien inventé. L'audit NuGet et `TreatWarningsAsErrors` faisaient déjà le travail en juin. Le cron n'ajoute qu'une chose : il déplace le moment où l'alarme sonne, de la production vers la CI. C'est peu, et c'est toute la différence entre réparer et prévenir.

---

## Ce que le cron ne garantit pas

La marge de cette semaine doit beaucoup au calendrier. Les avis sont tombés un mardi soir, et le prochain profil de veille ne tournait que le lundi suivant. Si la même advisory était arrivée un dimanche soir, le cron du lundi et la veille .NET du lundi auraient échoué à quelques heures d'écart, et la planification de GitHub Actions n'est pas ponctuelle : ce run « de 6 h UTC » est parti à 10 h 47. Le cron réduit la fenêtre, il ne la supprime pas.

C'est suffisant pour un outil personnel. Sur un projet où chaque run de production compte, j'irais plus loin : faire dépendre le run de production d'une restauration réussie, ou passer aux pull requests automatiques de Dependabot pour que le correctif soit prêt avant même que je lise la notification. Mais je n'ai pas besoin de ce niveau ici, et je préfère un dispositif simple qui tourne à un dispositif complet que je n'aurais jamais mis en place.

---

## Ce que ça confirme

En juin, j'avais tiré une leçon d'un incident. En octobre, je l'ai vue fonctionner. C'est un plaisir assez rare en ingénierie : constater qu'une décision prise à froid, une ligne de YAML ajoutée un lendemain de panne, a fait exactement ce pour quoi elle était là, sans que j'y repense une seule fois en deux mois et demi.

Les dépendances transitives continueront de vieillir sous mes projets, et d'autres avis tomberont. Je ne peux pas empêcher ça. Je peux en revanche choisir qui l'apprend en premier : moi, un jeudi, par une notification de CI, ou ma veille elle-même, un lundi, par un run qui ne livre rien. Une alarme ne sert que si elle sonne à un moment où l'on peut encore agir.

---

*Nabil Sidhoum — Senior .NET Tech Lead, Paris. Spécialisé fintech/wealthtech, Clean Architecture, industrialisation d'agents IA en production.*

*GitHub : [nabil-sidhoum](https://github.com/nabil-sidhoum) — Portfolio : [nabil-sidhoum.github.io](https://nabil-sidhoum.github.io)*