# Pour que l'IA vérifie mon code, il fallait d'abord que mes règles puissent échouer

*Publié le 26 août 2026 — Nabil Sidhoum, Senior .NET Tech Lead*

---

En juillet, Boris Cherny, le créateur de Claude Code, a publié une échelle d'adoption de l'IA en cinq paliers. Je l'ai lue comme on lit ce genre de grille : en cherchant où je me situais. Verdict inconfortable : sur mon propre template .NET, celui que je pose sur chaque nouveau projet et que je fais évoluer depuis des mois, j'étais au palier 1. Peut-être 2 les bons jours.

Ce qui m'a occupé, ensuite, n'est pas le classement. C'est la question qui suit : qu'est-ce qui me retient exactement ? La réponse n'était pas le modèle. Elle était dans mon dépôt, et elle tenait en un mot : mes règles ne pouvaient pas échouer.

---

## L'échelle, et ce qu'elle mesure vraiment

Les cinq paliers de Cherny se lisent au nombre d'agents par développeur, mais ce n'est pas la bonne façon de les regarder. Ce qui change d'un palier à l'autre, c'est **qui vérifie**.

| Palier | Agents par dev | Qui vérifie |
|---|---|---|
| 0 · Gated | 0 | L'accès à l'IA est restreint |
| 1 · Assisted | ~1 | Le développeur relit presque chaque changement |
| 2 · Parallel | ~10 | L'IA vérifie son propre travail : *tests, build, lint* |
| 3 · Supervised autonomy | ~100 | L'IA écrit tout ou presque, la supervision devient ponctuelle |
| 4 · AI-native | 1 000+ | La plupart des agents sont lancés par l'IA, pas par une personne |

Cherny le formule ainsi : ce qui sépare les paliers, c'est la confiance, pas le modèle. Le fossé entre la productivité d'une personne et l'adoption d'une équipe n'est pas une affaire de tokens, mais de goulots et de garde-fous propres à chaque étape.

La définition du palier 2 mérite qu'on s'y arrête, parce que c'est là que tout se joue : l'IA *« checks its own work — tests, build, lint »* avant qu'un développeur voie le diff.

Relisez cette phrase en pensant à votre propre dépôt. Elle suppose que `tests`, `build` et `lint` **puissent dire non**. Si ces trois-là répondent oui quoi qu'on leur soumette, l'agent ne vérifie rien du tout : il obtient une validation inconditionnelle et la présente comme une preuve.

C'était exactement mon cas.

![Les cinq paliers d'adoption de l'IA : à mesure que le nombre d'agents augmente, la vérification passe du développeur à la machine, ce qui suppose que la machine sache dire non](/posts/assets/paliers-adoption-ia.svg)

---

## L'audit : 101 déclarations de sévérité, aucune bloquante

Mon template distribue un `.editorconfig` de plusieurs centaines de lignes, neuf fichiers de règles écrites en RFC 2119, une quinzaine d'agents de revue spécialisés. Sur le papier, c'est un dispositif sérieux. J'ai décidé de le mesurer plutôt que de le croire.

Le résultat, daté du 25 août : sur les **101 déclarations de sévérité** du `.editorconfig` distribué, **aucune n'était en `warning` ni en `error`**. Toutes en `silent` ou en `suggestion`. Et `TreatWarningsAsErrors` était livré depuis des mois dans le `Directory.Build.props`, branché sur une prise vide.

Côté architecture, même tableau. Mon fichier `clean-architecture.md` énonce seize prescriptions : le Domain ne dépend de rien, les Handlers ne parlent qu'à leurs interfaces, seule l'Infrastructure touche EF Core. **Aucune n'était vérifiée par quoi que ce soit.**

La preuve tient en une classe. Je l'ai écrite exprès, avec trois violations de mes propres conventions : un `var` là où je prescris les types explicites, un `.Result` que j'interdis formellement parce qu'il produit des interblocages sous IIS, et un constructeur primaire que j'ai écarté par décision d'architecture documentée.

Elle compilait en **0 avertissement, 0 erreur**.

> Un garde-fou qui rassure sans garder est pire que pas de garde-fou. Sans lui, on sait qu'on relit. Avec lui, on croit que c'est fait.

Voilà le plafond. Il ne venait ni de Claude, ni du modèle, ni de la taille de la fenêtre de contexte. Il venait de moi : j'avais construit un dispositif qui disait toujours oui, puis je m'étonnais de devoir tout relire.

---

## Ce qu'on a le droit d'imposer, et ce qu'on n'a pas le droit d'imposer

Le réflexe, à ce stade, est de tout passer en `error`. C'est une erreur, et elle est coûteuse.

Une contrainte qui échoue sur du code correct est désactivée dès la première pose. Et elle n'est jamais désactivée seule : elle emporte avec elle la crédibilité de toutes les autres. Quiconque a déjà vu une équipe ajouter `<NoWarn>` avec un commentaire agacé sait comment cette histoire se termine.

J'ai donc posé un critère de tri, un seul, et je m'y suis tenu : **la violation produit-elle un bug à l'exécution ?**

| Règle | Ce que sa violation casse | Verdict |
|---|---|---|
| `.Result`, `.Wait()` | Interblocage sous IIS, en production, à la charge | **error** |
| `async void` | Les exceptions y sont perdues, le processus tombe | **error** |
| `CancellationToken` non propagé | L'annulation devient inopérante, sans aucun symptôme | **error** |
| Bloc sans accolades | Change de sens à la première ligne ajoutée, la revue ne le voit pas | **error** |
| Suffixe `Async` manquant | Un nom mal choisi ne casse rien à l'exécution | warning |
| `var` contre type explicite | Préférence de lecture, adossée à une décision d'architecture | error, assumé comme une exception au critère |

Sept règles sont passées bloquantes. **Dix-sept sont restées volontairement désactivées**, et ce n'est pas un oubli : ce sont des décisions documentées, pas de constructeurs primaires, pas d'expressions de collection, pas de method groups. Les réactiver ferait échouer le build sur exactement le code que le template prescrit.

C'est le passage important de ce chantier, et il ne se voit pas dans le diff : le template a cessé de proposer, mais il n'a pas commencé à tout interdire. Il impose ce qu'il peut défendre ligne à ligne.

---

## La violation qui ne se voit pas dans un fichier

Le durcissement du `.editorconfig` règle un problème de syntaxe. Il ne touche pas au plus coûteux.

Un développeur qui écrit un handler prenant un `DbContext` en paramètre ne viole aucune convention de style. Son fichier est irréprochable : types explicites, accolades, `CancellationToken` propagé. Ce qu'il casse ne se lit pas dans son fichier, mais dans le graphe : la couche Application vient de court-circuiter les interfaces qu'elle définit elle-même, et la logique métier n'est plus testable sans base de données.

Un relecteur attentif le voit. Un relecteur pressé, non. Et à mesure qu'on monte l'échelle de Cherny, il y a de moins en moins de relecteurs.

C'est ce que résout la seconde partie du chantier : quatre règles d'architecture devenues des tests exécutables, portées par un projet distribué avec le template et exécutées par le `dotnet test` que la CI lance déjà.

![Les quatre couches et leurs dépendances autorisées, avec les quatre dépendances interdites que chaque test attrape](/posts/assets/dependances-couches.svg)

Le tableau se lit dans les deux sens. Les flèches vertes sont légitimes, les rouges échouent :

| Test | Ce qu'il interdit | Ce que sa violation casse |
|---|---|---|
| T1 | Le Domain ne dépend d'aucune autre couche | La couche métier cesse d'être réutilisable et testable isolément |
| T2 | Application ne dépend pas d'Infrastructure | Le handler qui prend un `DbContext`, et la logique métier devient intestable sans base |
| T3 | Le Domain ne référence aucun framework de persistance | L'entité porte le mapping de sa table, le modèle métier suit le schéma |
| T8 | L'Api ne référence aucun framework de persistance | Le contrôleur interroge la base lui-même |

Le dernier a demandé un arbitrage qui illustre bien le critère. J'ai d'abord voulu écrire « l'Api ne dépend pas de l'Infrastructure », qui est la formulation naturelle. Elle est fausse : l'enregistrement des services dans `Startup` référence légitimement les implémentations, et cette règle aurait rougi sur du code parfaitement correct, dès la première pose. Je l'ai donc réduite à ce qu'elle devait viser : les frameworks de persistance, pas la couche. `Startup` reste vert, le contrôleur qui ouvre un `DbContext` rougit.

Et j'ai écrit noir sur blanc, dans la documentation distribuée, les cinq prescriptions que ces tests **ne verront jamais** : la logique métier dans un contrôleur, un fichier portant deux requêtes, les durées de vie de l'injection, l'état global partagé, les données sensibles dans les logs. Un test de dépendances lit un graphe, jamais un contenu. Prétendre l'inverse serait rendre le dispositif moins fiable, pas plus.

---

## Le vert qui ment

Vient alors le défaut le plus intéressant de tout le chantier, et celui qui donne son titre à cet article.

J'avais écrit mes quatre règles. Elles étaient justes. Sur une solution mal branchée, elles passaient toutes **au vert**.

La mécanique est d'une simplicité désarmante. Une règle d'architecture sélectionne des types, puis affirme quelque chose à leur sujet. Si la sélection ne ramène aucun type, l'affirmation est vraie par vacuité : *aucun type du Domain ne dépend de l'Infrastructure* est trivialement satisfait quand le Domain est vide. La bibliothèque ne lève aucune exception, ne prévient de rien, et rend un succès.

Il suffit d'un `ProjectReference` non complété, ou de couches nommées autrement que prévu, pour que quatre tests verts ne vérifient absolument rien.

![Une règle d'architecture qui n'inspecte aucun type est trivialement satisfaite et rend un succès ; le garde-fou de non-vacuité exige au moins un type inspecté avant d'évaluer la règle](/posts/assets/vert-sur-zero-type.svg)

Chaque règle commence donc désormais par exiger qu'elle inspecte au moins un type, et échoue sinon, avec un message qui nomme les deux causes possibles et dit quoi corriger.

C'est le geste le plus important du lot, et il ne produit aucune fonctionnalité. Sa valeur est ailleurs : **plus on monte l'échelle, moins il reste de monde pour s'apercevoir qu'un test ment.** Au palier 1, un test vert sur rien est une gêne ; quelqu'un finira par ouvrir le fichier. Au palier 3, c'est un mensonge qu'aucun relecteur n'est là pour détecter, et sur lequel une chaîne entière d'agents va bâtir en confiance.

Un dispositif de vérification ne se juge donc pas à ce qu'il valide. Il se juge à ce qu'il refuse, et à sa capacité de refuser quand il n'a rien à vérifier.

---

## Ce que la relecture ne pouvait pas voir

Reste une leçon que je n'avais pas anticipée, et qui a coûté le plus de temps. En construisant ce dispositif, j'ai introduit plusieurs défauts. **Aucun n'était visible à la lecture. Tous ont été trouvés en exécutant.**

**Le compilateur supprime les références qu'on n'utilise pas.** Ma première version chargeait les couches via `Assembly.GetReferencedAssemblies()`. C'est faux, et silencieusement : Roslyn élide des métadonnées toute référence de projet dont aucun type n'est employé. Or un fichier de tests d'architecture n'emploie précisément aucun type des couches qu'il inspecte. La méthode rendait une liste vide alors que les DLL étaient bien présentes à côté de l'exécutable. Livré tel quel, le dispositif aurait été rouge en permanence, chez tout le monde.

**La sortie de `dotnet test` est traduite.** Mon premier oracle cherchait les tests en échec dans la sortie console. Sur mon poste, en français, cette sortie annonce « Non réussi(s) : 4 » et ne nomme aucun test. Un dispositif de vérification dont le verdict dépend de la langue de la machine n'en est pas un. Il lit désormais le rapport XML, dont les statuts sont en anglais quelle que soit la culture.

**Et mon propre vérificateur mentait.** J'avais écrit un contrôle qui comptait les renvois mal formés dans ma documentation, en sommant une sortie `grep` avec `awk -F:`. Il annonçait fièrement zéro. En intégration continue, sur Linux, il en trouvait un — et il avait raison. La cause : mes chemins locaux commencent par `D:`, et `awk` découpait sur ce deux-points, lisant systématiquement la mauvaise colonne. **Vert sur ma machine, rouge en CI, sur le même commit.**

Ce dernier cas est le résumé de tout l'article. J'avais écrit un outil pour détecter un défaut ; l'outil et le défaut ont failli être livrés ensemble, tous deux verts. Ce qui les a séparés, ce n'est pas une relecture attentive. C'est une machine qui a exécuté la même chose ailleurs et a répondu non.

---

## Où j'en suis vraiment

Je n'ai pas changé de palier. Ce serait exactement le genre d'affirmation invérifiable que ce chantier a passé trois jours à traquer.

Ce que j'ai fait est plus modeste, et plus utile : j'ai levé une condition nécessaire. Le palier 2 suppose que l'IA vérifie son travail avec les tests, le build et le lint. Chez moi, ces trois-là disaient oui inconditionnellement. Ils peuvent désormais dire non, sur des motifs que je peux défendre un par un, et ils refusent de dire oui quand ils n'ont rien vérifié.

Ce qui manque pour la suite est d'une autre nature : une remontée d'alerte sur les échecs, une mesure de ce que les agents produisent réellement, et surtout le temps de vérifier que ces garde-fous tiennent sur des projets qui ne sont pas les miens.

La grille de Cherny a le mérite de déplacer la question. On demande souvent quel modèle utiliser, ou combien d'agents lancer en parallèle. La vraie question est ailleurs : **qu'est-ce qui, dans votre dépôt, est capable de dire non ?** Tant que la réponse est « une personne qui relit », le nombre d'agents n'a aucune importance. C'est cette personne qui est le goulot, et aucun modèle ne la remplacera tant que la vérification n'existera pas ailleurs qu'entre ses mains.

Mon template proposait des conventions. Il en impose désormais une partie, et refuse de se prononcer sur le reste. C'est moins ambitieux qu'un manifeste, et beaucoup plus solide.

---

*Nabil Sidhoum — Senior .NET Tech Lead, Paris. Spécialisé fintech/wealthtech, Clean Architecture, industrialisation d'agents IA en production.*

*GitHub : [nabil-sidhoum](https://github.com/nabil-sidhoum) — Portfolio : [nabil-sidhoum.github.io](https://nabil-sidhoum.github.io)*
