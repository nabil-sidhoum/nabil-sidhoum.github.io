# Ma CI a cassé sans que je touche une ligne de code : anatomie d'une dette invisible

*Publié le 23 juin 2026 — Nabil Sidhoum, Senior .NET Tech Lead*

---

Un mardi matin, le workflow `Veille IA` au rouge. Tombé en une vingtaine de secondes, bien avant d'exécuter la moindre ligne de mon code. Je remonte l'historique : la veille déjà, le run `Veille .NET` du lundi avait échoué à l'identique. Trois profils composent ce pipeline, un par jour de la semaine, mais ils partagent le même build. Ils étaient condamnés tous les trois. Et aucun commit applicatif depuis le dernier run vert.

Petite ironie au passage : `veille-tech` est l'outil que je me suis construit pour rester au fait de l'écosystème .NET et IA, et c'est une évolution discrète de ce même écosystème, une advisory publiée sur une dépendance, qui vient de le mettre à terre. On peut suivre l'actualité de près et se faire surprendre par sa plomberie.

Le diagnostic a été rapide. Le plus intéressant, c'est ce qu'il révèle : un système peut se dégrader tout seul, à l'arrêt, sans qu'on y touche. Une dette invisible avait mûri en silence, au plus profond de mon arbre de dépendances. En voici l'autopsie, et le parti pris que j'assume.

---

## Le coupable : une dépendance que je n'ai jamais installée

Le log était sans ambiguïté :

```
error NU1903: Warning As Error: Package 'SQLitePCLRaw.lib.e_sqlite3' 2.1.11
has a known high severity vulnerability
```

`SQLitePCLRaw.lib.e_sqlite3` ? Je ne l'ai jamais ajoutée. C'est une dépendance transitive : `Microsoft.EntityFrameworkCore.Sqlite` la tire trois niveaux plus bas, pour fournir le binaire natif de SQLite. Je consomme EF Core, EF Core consomme SQLite, et la faille était là, tapie au fond de l'arbre.

La vulnérabilité elle-même, [CVE-2025-6965](https://github.com/advisories/GHSA-2m69-gcr7-jv3q), publiée le 15 juillet 2025 et notée 7.2 sur l'échelle CVSS, touche SQLite avant la version 3.50.2 : un dépassement du nombre de termes agrégés pouvant corrompre la mémoire. Dans mon contexte de batch RSS, le risque réel est marginal. Mais là n'est pas la question. La question, c'est : pourquoi mon build s'est-il arrêté de compiler, du jour au lendemain, sans que rien ne change de mon côté ?

---

## Pourquoi maintenant, et pas il y a un an

L'advisory date de juillet 2025. Mon build, lui, ne tombe que des mois plus tard. Trois ingrédients devaient s'aligner, et ils l'ont fait sans bruit.

| Ingrédient | Depuis quand | Effet |
|---|---|---|
| **L'audit NuGet (NU1901 à NU1904)** | .NET 8 SDK, fin 2023 | À chaque `restore`, les paquets sont confrontés à la base d'advisories. NU1903 correspond à la sévérité haute. |
| **`NuGetAuditMode = all` par défaut** | Cible `net10.0` et au-delà | Avant net10, le défaut était `direct` : seules les dépendances directes étaient auditées. À partir de net10, l'audit descend aussi dans les transitives. |
| **`TreatWarningsAsErrors`** | Mon choix, depuis le début | Tout avertissement, qu'il soit C#, MSBuild ou NuGet, devient une erreur de build. |

La mécanique se lit alors d'elle-même. En ciblant .NET 10, mon projet a basculé `NuGetAuditMode` sur `all` : l'audit a commencé à inspecter les dépendances transitives, dont ce `lib.e_sqlite3`. Mon `TreatWarningsAsErrors` a fait le reste, en promouvant l'avertissement NU1903 en erreur fatale. La base d'advisories étant mise à jour côté serveur en continu, aucun de mes commits n'était nécessaire pour déclencher la bascule : il a suffi que les trois conditions coexistent un matin.

![Chronologie : .NET 8 introduit l'audit NuGet, l'advisory CVE-2025-6965 paraît en juillet 2025, le passage à .NET 10 active l'audit des transitives, et TreatWarningsAsErrors transforme le tout en alerte de build, résolue par un override de dépendance](/posts/assets/chronologie-nu1903.svg)

Ce n'est donc pas un bug. C'est un système d'alerte qui fonctionne exactement comme prévu, et qui me signalait, fort, quelque chose que j'avais choisi de pouvoir entendre.

---

## Le parti pris : `TreatWarningsAsErrors` n'est pas une rigidité, c'est un choix

La réaction réflexe, ici, consiste à faire taire l'alarme. NuGet l'autorise explicitement :

```xml
<WarningsNotAsErrors>NU1903</WarningsNotAsErrors>
```

Une ligne, et le build repart au vert. J'ai écarté cette option, et c'est le cœur de l'affaire.

> Faire taire NU1903, c'est troquer une panne de build visible contre une vulnérabilité silencieuse. Je préfère un système qui casse fort et tôt à un système qui tourne en paix au-dessus d'une faille connue.

`TreatWarningsAsErrors`, sur un projet personnel comme en production, n'est pas une coquetterie de discipline. C'est un dispositif de friction volontaire : il garantit qu'aucune dérive, un `await` oublié, une nullabilité douteuse, une dépendance vulnérable, ne se faufile dans `main` sans m'obliger à la regarder en face. Le jour où il bloque, il fait précisément son travail. Désactiver l'alarme parce qu'elle sonne, c'est confondre le symptôme et la cause.

La vraie correction n'était donc pas de masquer l'avertissement, mais de retirer la dépendance vulnérable de l'arbre. EF Core 10 ne propose pas encore de version sans cette transitive. J'ai donc épinglé moi-même le maillon natif, via un override :

```xml
<PackageReference Include="SQLitePCLRaw.bundle_e_sqlite3" Version="3.0.3" />
```

Le saut de SQLitePCLRaw en version 3 a un effet décisif : le bundle abandonne l'ancien `lib.e_sqlite3` au profit d'un nouveau paquet natif, `SourceGear.sqlite3` 3.50.4.5, soit SQLite 3.50.4, au-delà de la version corrigée. Vérification faite avec `dotnet list package --include-transitive`, la dépendance 2.1.11 a purement disparu de l'arbre. NU1903 n'a plus rien à signaler, et l'alarme reste armée pour la prochaine fois.

---

## Et le pruning de .NET 10, alors ?

.NET 10 a justement introduit le [NuGet package pruning](https://devblogs.microsoft.com/dotnet/nuget-package-pruning-in-dotnet-10/), activé par défaut sur les cibles `net10.0`. Le principe : retirer du graphe de dépendances, au moment du `restore`, les paquets transitifs déjà fournis par le runtime. On pourrait croire que c'est la solution durable à mon problème. Ce n'en est pas une, et comprendre pourquoi est instructif.

Le pruning ne touche qu'aux paquets du framework partagé, ceux qui sont déjà « in-box » dans le runtime, comme `System.Text.Json` ou `System.Security.Cryptography.Pkcs`. Son objectif est précisément de tuer les faux positifs d'audit : une CVE signalée sur un paquet que votre application n'utilise pas réellement, puisqu'elle s'appuie sur la version du runtime. Mais `SQLitePCLRaw.lib.e_sqlite3` est une vraie bibliothèque native tierce, absente du framework. Le pruning la laisse donc exactement où elle est.

Et c'est le point qui referme la boucle. Mon alerte a traversé le pruning sans être filtrée : ce n'était pas du bruit de framework, c'était un vrai composant vulnérable embarqué dans mon binaire. Loin de m'épargner l'alerte, .NET 10 a fait les deux choses qu'il fallait : élargir l'audit aux transitives pour la faire remonter, et élaguer les faux positifs pour que ce qui reste soit du signal. Ce qui restait, ici, méritait que je m'arrête.

---

## Industrialiser la détection

Mon correctif débloque la CI, mais il reste artisanal : l'override est à surveiller à la main, à retirer le jour où EF Core proposera une version saine. Voici comment je durcirais le dispositif si l'enjeu montait d'un cran.

**Séparer l'audit du build applicatif.** Plutôt que de laisser une advisory bloquer un correctif urgent au pire moment, NuGet permet un pipeline d'audit dédié : on conditionne le traitement des codes NU190x en erreurs à une variable, et seul ce pipeline échoue sur une nouvelle faille.

```xml
<PropertyGroup>
  <NuGetAuditCodes>NU1901;NU1902;NU1903;NU1904</NuGetAuditCodes>
  <WarningsAsErrors Condition="'$(AuditPipeline)' == 'true'">$(WarningsAsErrors);$(NuGetAuditCodes)</WarningsAsErrors>
  <WarningsNotAsErrors Condition="'$(AuditPipeline)' != 'true'">$(WarningsNotAsErrors);$(NuGetAuditCodes)</WarningsNotAsErrors>
</PropertyGroup>
```

Le build de tous les jours reste fluide, l'audit tourne à côté et alerte sans bloquer les livraisons.

**Centraliser les versions.** Avec Central Package Management (`Directory.Packages.props`) et le transitive pinning, un override de sécurité se déclare en un seul endroit, traçable, au lieu d'un `PackageReference` perdu dans un csproj qu'on oubliera de nettoyer.

**Automatiser les montées de version.** Dependabot ou Renovate ouvrent des pull requests dès qu'une dépendance, directe ou transitive, reçoit une advisory ou une mise à jour. La détection ne dépend plus du hasard d'un `restore`.

**Outiller l'investigation.** `dotnet nuget why` explique pourquoi une transitive est présente, `dotnet list package --vulnerable --include-transitive` liste les failles connues, et `dotnet package update --vulnerable` remonte automatiquement les paquets concernés.

---

## Ce que ça illustre

On parle beaucoup de la robustesse comme d'une propriété qu'on grave au moment de la conception : bonnes abstractions, responsabilités séparées, tests en isolation. C'est nécessaire, mais incomplet. Car un système ne se dégrade pas seulement quand on le modifie, il se dégrade aussi à l'arrêt. Le mien n'avait pourtant pas bougé d'une ligne : c'est l'écosystème, sous lui, qui a bougé. Une advisory tombée sur une dépendance que je n'avais jamais choisie, révélée par un changement de défaut du tooling.

La leçon que j'en retire n'est pas de désactiver les alarmes qui dérangent, mais l'inverse : investir dans les mécanismes qui rendent ces dérives bruyantes le plus tôt possible. L'audit NuGet et `TreatWarningsAsErrors` ont fait exactement ce pour quoi je les avais mis en place, m'imposer de regarder une faille en face plutôt que de tourner au-dessus. Et le package pruning de .NET 10, en filtrant le bruit des faux positifs, garantit que l'alarme qui subsiste mérite qu'on s'arrête.

C'est le même réflexe qui guide mes choix d'architecture : préférer un système qui échoue franchement et tôt à un système qui semble aller bien. Le silence, en ingénierie, est rarement une bonne nouvelle.

---

*Nabil Sidhoum — Senior .NET Tech Lead, Paris. Spécialisé fintech/wealthtech, Clean Architecture, industrialisation d'agents IA en production.*

*GitHub : [nabil-sidhoum](https://github.com/nabil-sidhoum) — Portfolio : [nabil-sidhoum.github.io](https://nabil-sidhoum.github.io)*
