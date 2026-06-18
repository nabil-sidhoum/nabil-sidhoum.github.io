# J'ai automatisé ma veille technique .NET — et ce que ça m'a appris sur l'usage pragmatique des LLM

*Publié le 18 juin 2026 — Nabil Sidhoum, Senior .NET Tech Lead*

---

Rester à jour en .NET, en IA appliquée au développement et en architecture logicielle, c'est un travail à plein temps. Les flux RSS débordent, l'essentiel est noyé dans le bruit, et le tri manuel du week-end finit toujours par passer à la trappe.

Plutôt que de m'imposer cette discipline, je l'ai déléguée à un pipeline. `veille-tech` collecte mes sources, écarte ce que j'ai déjà lu, **note chaque article** selon mon profil de Tech Lead, et me livre une sélection thématique trois fois par semaine. Zéro serveur, zéro euro, zéro intervention.

Ce projet est volontairement simple — et c'est précisément ce qui en fait un bon terrain pour parler d'une chose que je trouve sous-estimée : **savoir où placer un LLM dans un pipeline, et où ne surtout pas le mettre.**

---

## Ce que fait le pipeline

Une console app .NET 10, déclenchée par cron sur GitHub Actions. Trois profils tournent indépendamment, chacun avec ses propres sources (19 flux RSS/Atom au total) et son canal de diffusion : **.NET** le lundi, **IA** le mardi, **Architecture** le mercredi.

L'application est **stateless**. Le seul état persistant est une petite base SQLite, versionnée avec le code, qui mémorise les articles déjà traités. Conséquence directe : le pipeline est **idempotent** — le relancer ne republie jamais deux fois le même contenu.

![Architecture du pipeline veille-tech](/posts/assets/veille-tech-pipeline.svg)

Chaque étape est une interface dédiée (`IFeedCollector`, `IDeduplicator`, `IArticleScorer`…) injectée dans un orchestrateur. Le pipeline se lit de haut en bas, et chaque maillon est testable en isolation.

---

## Le LLM pour le jugement, pas pour la mise en forme

C'est la décision de design dont je suis le plus satisfait, et elle va à rebours de l'usage par défaut.

Le réflexe, quand on branche un LLM sur de la veille, c'est de lui faire **générer le rapport** : résumer, hiérarchiser, mettre en forme. C'est coûteux en tokens, lent, et non reproductible — le modèle reformule, déforme, invente parfois. Je l'utilise autrement : il fait du **jugement structuré**. Chaque article retenu reçoit, en un seul appel JSON contraint, un score de 1 à 5, une catégorie et un résumé court, calibrés sur mon profil.

Mais l'**assemblage du rapport** (tri, mise en forme Markdown, découpage des messages Discord) reste 100 % local et déterministe. Le LLM décide *quoi* retenir et le condense ; il ne touche jamais à la *fabrication* du livrable. Résultat : un rapport reproductible, que je peux tester sans appeler d'API, et pas un token gaspillé sur de la tuyauterie.

> Le scoring est volontairement **séquentiel et throttlé** (~10 s entre articles) pour rester dans le quota gratuit du provider. Un run dure 2 à 5 minutes selon le volume, et coûte 0 €.

---

## Les autres décisions qui comptent

**Collecte résiliente.** Les flux RSS tombent, changent d'URL, ou ne se déclarent pas proprement. Le collecteur applique une cascade : URL principale → URLs de repli → auto-découverte du flux depuis la page HTML. Les timeouts et sources injoignables sont dégradés proprement (loggués, jamais bloquants) plutôt que de faire échouer tout le run.

**Configuration déclarative.** Ajouter une thématique de veille = écrire un fichier YAML : sources, pondérations, seuil de score, mots-clés de boost. Le code ne connaît aucune source en dur.

**Discipline de bout en bout.** Conventions C# strictes et documentées, tests unitaires (xUnit + Shouldly) avec HTTP mocké et SQLite en mémoire, logging structuré (Serilog) avec un résumé de run normalisé, et des ADRs pour tracer chaque choix structurant et son alternative. Sur un projet personnel comme en production, je m'astreins au même niveau.

---

## La stack

| Domaine | Choix |
|---|---|
| Runtime / langage | .NET 10 LTS · C# 14 (records immuables, pattern matching) |
| Persistance | SQLite + EF Core, versionnée avec le code |
| Configuration | YAML, un profil par thématique |
| Scoring | LLM via API (provider gratuit) — scoring par article |
| Observabilité | Serilog (lisible en local, structuré en CI) |
| Tests | xUnit · Shouldly · HTTP mocké |
| Orchestration | GitHub Actions — workflows cron indépendants |

---

## Ce que ce projet illustre

Au-delà de l'outil, `veille-tech` condense ma façon de travailler : partir d'un besoin concret, le découper en composants à responsabilité unique, **automatiser intégralement** l'exécution, et investir dans la testabilité et la traçabilité dès le départ.

C'est surtout un parti pris sur l'intégration des LLM dans un système déterministe : les utiliser là où ils apportent un vrai gain (le jugement) et garder le reste sous contrôle classique. Un pipeline où le LLM décide de *tout* est impressionnant en démo et ingérable en production. Un pipeline où il décide d'*une seule chose*, bien choisie, est fiable, testable et reproductible.

C'est le même réflexe que j'applique au choix des outils : l'adéquation architecturale prime sur le feature set.

---

*Nabil Sidhoum — Senior .NET Tech Lead, Paris. Spécialisé fintech/wealthtech, Clean Architecture, industrialisation d'agents IA en production.*

*GitHub : [nabil-sidhoum](https://github.com/nabil-sidhoum) — Portfolio : [nabil-sidhoum.github.io](https://nabil-sidhoum.github.io)*
