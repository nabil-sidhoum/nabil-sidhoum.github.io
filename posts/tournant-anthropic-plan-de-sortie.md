# Une IA « éthique » qui veut tout savoir de moi : repenser ma dépendance à Claude

*Publié le 24 juin 2026 — Nabil Sidhoum, Senior .NET Tech Lead*

---

Mon outillage de développement repose presque entièrement sur un seul fournisseur : Claude, et Claude Code. Ce n'est pas un accident, c'est un choix que j'assumais pleinement. Pourtant, en quelques semaines de l'été 2026, ce choix a cessé d'être évident. Deux décisions d'Anthropic, l'une subie, l'autre revendiquée, m'ont obligé à reposer une question que je repoussais : que se passe-t-il le jour où le fournisseur au cœur de ma chaîne d'outils change de nature ?

Cet article n'est pas un billet de rupture. C'est l'analyse, en Tech Lead, de la façon dont j'évalue une dépendance critique quand le terrain bouge sous elle : pourquoi Claude était mon premier choix, pourquoi ça ne va plus de soi, et ce que je construis en réponse. La conclusion est volontairement technique et juridique, parce que c'est là que se joue la décision réelle.

---

## Pourquoi Claude était mon premier choix

Deux raisons, et elles comptaient toutes les deux.

**La qualité technique, d'abord.** Sur le code et l'agentique, Claude est la référence. Sur une boucle autonome longue de *tool-use*, là où la justesse d'un changement multi-fichiers difficile prime sur tout le reste, aucun modèle accessible ne tient aussi bien la distance : [Opus 4.8 marque 88,6 % sur SWE-bench Verified](https://www.anthropic.com/news/claude-opus-4-8). C'est exactement ce que mon `dotnet-claude-template` exploite : une orchestration multi-agents, des skills et des conventions documentées, un système d'ADR pour tracer les décisions. Tout cela suppose un modèle qui suit des instructions fines sans dériver. Claude le fait.

**La confiance, ensuite.** On l'oublie quand on ne regarde que les benchmarks, mais choisir un fournisseur, c'est aussi parier sur son comportement futur. Or Anthropic ne s'est jamais présentée comme un simple éditeur de modèles. Elle s'est posée en acteur responsable, jusqu'à [refuser au Pentagone l'usage de ses modèles pour la surveillance de masse](https://www.anthropic.com/news/usage-policy-update) et à [limiter ce que le FBI, le Secret Service et l'ICE peuvent en faire](https://www.yahoo.com/news/articles/anthropic-irks-white-house-limits-085748525.html), un bras de fer qui lui a [valu la colère de la Maison-Blanche](https://en.wikipedia.org/wiki/Anthropic%E2%80%93United_States_Department_of_Defense_dispute). Ce positionnement avait une valeur opérationnelle pour moi. J'accordais plus facilement une dépendance profonde à un acteur qui semblait préférer la retenue à la captation.

Premier choix, donc, sur la qualité comme sur la confiance. C'est précisément ce double appui qui vient de se fissurer.

---

## Pourquoi ce n'est plus le cas

En quelques semaines, deux décisions sont venues contredire le pari. Prises isolément, chacune s'explique. Mises bout à bout, elles racontent un changement de cap.

**Le 13 juin 2026, [une directive d'export control américaine a frappé les modèles de pointe](https://time.com/article/2026/06/13/anthropic-fable-mythos-ban-US-security/).** Le texte interdisait l'accès aux ressortissants étrangers, y compris ceux présents sur le sol américain et jusqu'aux employés non-citoyens d'Anthropic. Mais filtrer un accès par nationalité est techniquement impossible à garantir, et l'entreprise a tranché par le bas : [Fable 5 et Mythos 5 ont été désactivés pour tout le monde](https://www.anthropic.com/news/fable-mythos-access) afin d'assurer la conformité. Y compris les utilisateurs américains, que la directive ne visait pourtant pas. Une décision réglementaire, sans la moindre panne, a suffi à faire disparaître les meilleurs modèles du catalogue pour l'ensemble de la base. Mon vrai plafond accessible aujourd'hui n'est plus Mythos 5, c'est Opus 4.8.

**À compter du 8 juillet, [Anthropic se dote du cadre pour exiger une vérification d'identité renforcée](https://support.claude.com/en/articles/14328960-identity-verification-on-claude).** Pièce d'identité officielle, selfie, et [gabarit de géométrie faciale confié au prestataire KYC Persona](https://thenextweb.com/news/anthropic-claude-id-verification-privacy-policy-persona-biometric) : les comptes Free, Pro et Max sont concernés ; les plans Team, Enterprise et API en sont exemptés. L'individu doit donc prouver biométriquement qui il est, l'entreprise cliente non. La donnée la plus intime qui soit, le visage, devient une condition d'accès pour le particulier.

Le rapprochement des deux est ce qui me frappe. Aujourd'hui, l'entreprise ne sait pas filtrer ses utilisateurs par nationalité, et elle retire un produit pour tous faute de mieux. Demain, avec l'identité vérifiée de chacun, elle disposera exactement de la capacité qui lui manque : identifier précisément qui vous êtes, et donc, potentiellement, vous différencier selon votre provenance. Identification d'un côté, découpage géographique de l'autre. Ce sont les deux faces d'une logique de contrôle, et c'est l'inverse de la sobriété qui m'avait séduit.

![Chronologie du glissement : d'un positionnement éthique affiché (IA constitutionnelle, refus de la surveillance des migrants) vers le retrait de modèles pour tous faute de pouvoir filtrer par nationalité, puis la vérification d'identité biométrique des comptes particuliers, aboutissant à une entreprise qui se dote des moyens d'identifier et différencier ses utilisateurs](/posts/assets/glissement-fournisseur-ia.svg)

Je ne prête pas de mauvaises intentions, et une partie de ces décisions est imposée par un régulateur. Mais du point de vue de l'utilisateur que je suis, le résultat est le même : l'acteur que j'avais choisi pour sa retenue se met à construire les moyens d'un contrôle qu'il dénonçait ailleurs. Quand le discours et la pratique divergent à ce point, ce n'est plus un incident isolé, c'est un signal sur la nature de l'entreprise. Et un signal sur la nature d'un fournisseur, pour un architecte, ça se traite comme un risque.

---

## La dépendance, mesurée froidement

Mon `dotnet-claude-template` est dépendant à près de 100 % de Claude et de Claude Code. C'est un actif que j'ai mûri sur des mois, donc un point de défaillance unique d'autant plus coûteux. La bonne nouvelle, quand on regarde la mécanique de près, est que sortir ne serait pas une exportation de données mais un portage : on garde la matière, on réécrit l'enveloppe.

L'essentiel de l'actif tient dans des fichiers texte neutres : ils se déplacent d'un outil à l'autre sans réécriture. Ce qui dépend d'un outil précis n'est que des points d'entrée et de la syntaxe, réécrits en quelques jours.

L'effort véritable est ailleurs, et il ne se voit pas dans un diff. Mes conventions sont calibrées sur le comportement de Claude ; un autre modèle obéira différemment aux mêmes consignes. Le portage des fichiers prend des jours ; le re-réglage pour qu'un autre modèle les respecte avec la même rigueur est l'inconnue réelle, et le vrai coût d'une sortie.

---

## Le repli, sous l'angle juridique autant que technique

C'est ici que l'analyse devient concrète, parce que le critère de choix a changé. Tant que je raisonnais qualité pure, Opus 4.8 gagnait sans débat. En ajoutant la souveraineté et la confiance, la grille se redessine. Mythos 5 et Fable 5 mènent les classements mais sont précisément les modèles suspendus ; il faut donc comparer ce qui reste réellement accessible.

| | [Opus 4.8](https://www.anthropic.com/news/claude-opus-4-8) | [Mistral Medium 3.5](https://mistral.ai/news/vibe-remote-agents-mistral-medium-3-5/) | [Gemini 3.1 Pro](https://llm-stats.com/models/gemini-3.1-pro-preview) |
|---|---|---|---|
| SWE-bench Verified | 88,6 % | ~77,6 % | 80,6 % |
| Contexte | 1M | 256K | 1M (jusqu'à 2M) |
| Prix in/out (par MTok) | $5 / $25 | ~$1,50 / $7,50 | $2 / $12 |
| Licence | Propriétaire | Modified MIT (open weights) | Propriétaire |
| Juridiction | 🇺🇸 US | 🇫🇷 FR/EU | 🇺🇸 US |

Trois lectures, trois questions tranchées.

**Sur la qualité brute, Opus 4.8 reste devant.** L'écart sur le suivi d'instructions fines est réel, et c'est ce qui en fait, aujourd'hui encore, mon outil de production. La justesse prime quand la tâche est difficile.

**Sur l'axe juridique, Mistral est la seule vraie réponse.** Entreprise française, datacenter européen, hors de portée d'une directive d'export US. [Modèle dense 128B *open weights* sous licence Modified MIT, 77,6 % sur SWE-bench Verified](https://www.marktechpost.com/2026/05/02/mistral-ai-launches-remote-agents-in-vibe-and-mistral-medium-3-5-with-77-6-swe-bench-verified-score/), environ 3,3 fois moins cher sur une charge type, et auto-hébergeable jusque sur une RTX 3060. C'est la seule option qui échappe à la fois au découpage par nationalité et à l'exigence biométrique, parce que je peux à la limite faire tourner le modèle chez moi. L'outillage suit : CLI Vibe, extension VS Code, mode web Vibe Work.

**Sur l'axe du risque, Gemini n'apporte rien.** Ses qualités sont réelles, en particulier sur le contexte long pour analyser un monorepo entier. Mais il relève de la même juridiction américaine qu'Anthropic. Remplacer Claude par Gemini, c'est changer de fournisseur sans changer d'exposition. Sur le seul axe qui motive cette réflexion, le déplacement est nul.

Le verdict juridique est donc net : seul Mistral neutralise à la fois le scénario du retrait réglementaire et celui de l'identification imposée. La qualité, elle, reste à Opus. Tout l'enjeu est de ne pas avoir à choisir l'un contre l'autre dans l'urgence.

---

## Construire la sortie à froid

Conclure « il faut passer à Mistral » serait reproduire l'erreur de départ : remplacer une dépendance unique par une autre, et redevenir prisonnier le jour où le contexte de Mistral changera. La vraie question de gouvernance est de ne plus jamais dépendre d'un seul acteur. J'en tire quatre principes.

**Construire la route de sortie sans urgence.** Monter dès maintenant un `dotnet-vibe-template` parallèle et le tester une fois de bout en bout, à froid, hors de toute crise. La valeur n'est pas dans son usage immédiat, elle est dans son existence. Un plan jamais exécuté reste une hypothèse ; un plan exécuté une fois est une assurance.

**Garder Claude Code en production tant que c'est rationnel.** Tant qu'Opus et Sonnet restent accessibles et supérieurs sur le suivi d'instructions fines, je continue de les utiliser. On ne sabote pas son meilleur outil par principe. On cesse simplement d'en dépendre aveuglément.

**Abstraire l'actif réel.** Ce qui a de la valeur, ce ne sont pas les intégrations propres à un fournisseur, ce sont mes conventions, mon architecture d'agents, mes skills documentés. En format neutre, ils deviennent un *write once, deploy both*. Le `SKILL.md` multi-outils va déjà dans ce sens.

**Appliquer au workflow ce que j'applique déjà au code.** Sur `veille-tech`, ma collecte est résiliente par conception : source principale, sources de repli, auto-découverte, pour qu'un flux qui tombe ne fasse jamais tomber le run. C'est la même philosophie transposée un cran plus haut : non plus la résilience d'une source, mais celle de toute ma chaîne d'outillage.

---

## Ce que ça illustre

Ce qui m'a fait bouger n'est pas une baisse de qualité d'Opus, qui reste excellent. C'est un changement de nature de l'entreprise qui le produit. Une société qui faisait de la retenue son identité voit ses meilleurs modèles retirés pour tous sur une décision réglementaire, et réclame désormais le visage de ses utilisateurs particuliers. Une partie de ce mouvement lui est imposée, l'autre non. Mais du point de vue de la dépendance, peu importe la cause : un fournisseur dont les actes s'éloignent à ce point de son discours n'offre plus les mêmes garanties qu'au moment où j'ai parié sur lui.

La leçon n'est pas « Anthropic est devenue malveillante » ni « Mistral est meilleur ». C'est qu'une dépendance fournisseur critique inclut une variable qu'on oublie trop souvent de modéliser : la confiance, et sa trajectoire. On la traite comme n'importe quel risque. On l'identifie, on en mesure le coût de sortie technique, on évalue l'exposition juridique, on construit l'alternative avant d'en avoir besoin, et on garde le meilleur outil tant qu'il reste accessible et acceptable.

C'est le même réflexe qui guide mes choix d'architecture : ne pas concevoir un système autour de l'hypothèse que rien ne changera jamais, mais faire en sorte que le changement, quand il arrive, soit une bascule préparée plutôt qu'une crise. Transformer un signal inconfortable en cadre de décision réutilisable, c'est précisément le travail d'un Tech Lead.

Reste l'honnêteté de l'incertitude : je ne sais pas encore si ces décisions dessinent un vrai tournant ou une simple mauvaise passe pour Anthropic. Peut-être l'entreprise corrigera-t-elle le tir, peut-être la directive sera-t-elle levée et la vérification d'identité reléguée à la marge. On verra si le changement se confirme. D'ici là, je ne romps rien et je ne m'emballe pas : je garde le meilleur outil tant qu'il reste accessible, et je tiens ma porte de sortie repérée au cas où. *Wait and see*, donc, mais la main déjà posée sur la poignée.

---

*Nabil Sidhoum — Senior .NET Tech Lead, Paris. Spécialisé fintech/wealthtech, Clean Architecture, industrialisation d'agents IA en production.*

*GitHub : [nabil-sidhoum](https://github.com/nabil-sidhoum) — Portfolio : [nabil-sidhoum.github.io](https://nabil-sidhoum.github.io)*
