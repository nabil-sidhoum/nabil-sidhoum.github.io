# Pourquoi j'ai choisi de ne pas intégrer Hermes Agent dans mon pipeline de veille technique .NET

*Publié le 16 juin 2026 — Nabil Sidhoum, Senior .NET Tech Lead*

---

Il y a quelques semaines, Hermes Agent a explosé sur GitHub : ~140 000 étoiles en moins de trois mois, et la première place du classement OpenRouter en volume de tokens. Quand un outil fait ces chiffres dans l'écosystème AI, je l'analyse systématiquement : est-ce que ça change quelque chose à ce que je construis ? Est-ce que je rate quelque chose ?

Dans mon cas : non. Et voilà pourquoi — parce que la réponse en dit plus sur le choix d'outils que sur Hermes lui-même.

---

## Mon pipeline de veille technique

Depuis plusieurs mois, je fais tourner `veille-tech`, un pipeline automatisé sur GitHub Actions. L'objectif : surveiller une vingtaine de flux RSS répartis sur trois profils (.NET, IA, architecture logicielle), scorer les articles par pertinence, et publier les retenus sur Discord trois fois par semaine, à raison d'un profil par jour.

L'architecture est intentionnellement simple :

![Schéma du pipeline veille-tech : 3 crons GitHub Actions, collecte RSS, déduplication SQLite, scoring LLM Groq, assemblage Markdown local, publication Discord](/posts/assets/veille-pipeline.svg)

Stack : .NET 10, C#, GitHub Actions, Groq comme provider LLM. Coût mensuel : **0 €** (repo public, minutes Actions gratuites). Aucune infrastructure à maintenir : le pipeline s'exécute, produit son rapport, et s'arrête. Le LLM ne sert qu'au scoring article par article ; l'assemblage du rapport reste 100 % local et déterministe. Stateless par design.

---

## Ce qu'est Hermes Agent — honnêtement

Hermes Agent est un projet open-source (MIT) de Nous Research, sorti en février 2026. Ce n'est pas un wrapper autour d'une API : c'est un **agent autonome persistant** conçu pour tourner en continu sur une infrastructure que vous contrôlez. Ses quatre arguments :

- **Mémoire cross-session** — il se souvient de ce qu'il a appris d'une conversation à l'autre, sans re-contextualisation.
- **Self-improving skills** — quand il résout un problème complexe, il génère un *skill* réutilisable et l'affine à l'usage.
- **Multi-plateforme** — Telegram, Discord, Slack, WhatsApp et d'autres, via un gateway centralisé.
- **Model-agnostic** — OpenAI, OpenRouter, Nous Portal ou tout endpoint compatible.

Sur le papier, c'est impressionnant. En pratique, il faut comprendre ce que cette architecture implique.

---

## L'incompatibilité architecturale

| Dimension | Mon pipeline `veille-tech` | Hermes Agent |
|---|---|---|
| **Exécution** | Batch déclenché (cron) | Agent persistant (24/7) |
| **État** | Stateless — SQLite pour dédup uniquement | Stateful — mémoire cross-session |
| **Infrastructure** | GitHub Actions runners (gratuit) | VPS ou serverless |
| **Coût** | 0 €/mois | Variable selon usage |
| **Durée d'un run** | ~2 à 5 min (scoring séquentiel throttlé) | Continu |
| **Interaction** | Aucune pendant le run | Conversationnelle, asynchrone |
| **Amélioration dans le temps** | Non — déterministe par design | Oui — skills auto-générés |

La différence n'est pas une question de maturité ou de qualité, mais de **paradigme**. Mon pipeline répond à *« quels articles pertinents ont été publiés cette semaine ? »* : il s'exécute, répond, s'arrête. Hermes répond à *« comment avoir un assistant IA qui me connaît, travaille pendant que je dors, et s'améliore au fil du temps ? »*. Deux questions, deux réponses.

Concrètement, trois points bloquent dans mon contexte :

1. **Budget et infrastructure zéro.** Hermes suppose une instance qui tourne en permanence — même un VPS à quelques euros par mois reste une infra à provisionner, monitorer, maintenir. Pour une veille personnelle, le ratio valeur/complexité ne justifie pas le saut.
2. **La mémoire persistante est surqualifiée ici.** Elle a de la valeur quand l'agent accumule du contexte sur des projets et des préférences. Pour un batch qui collecte des RSS et les score, la déduplication SQLite couvre 100 % de mon besoin de « mémoire ».
3. **Le modèle conversationnel est orthogonal au batch.** Hermes brille quand on interagit avec lui pendant qu'il travaille. Mon pipeline n'a pas d'utilisateur en session : personne à qui envoyer des messages intermédiaires.

---

## Si le contexte était différent

Je veux être honnête : avec un autre use case, la réponse changerait. Si j'avais besoin d'un assistant qui retient mes préférences de filtrage d'une semaine à l'autre, mémorise les sources que j'ai jugées pertinentes et m'alerte sur Telegram quand un article matche un pattern précis — Hermes serait un candidat sérieux.

Son concept de self-improving skills est d'ailleurs proche de mon système de `SKILL.md` dans `dotnet-claude-template`. La différence : mes skills sont rédigés manuellement, en français, calibrés pour mon stack .NET ; Hermes les génère depuis ses exécutions. Les deux approches ont leurs mérites.

---

## Ce que ça m'a appris sur le choix d'outils AI

Analyser Hermes m'a confirmé un principe que j'applique partout : **l'adéquation architecturale prime sur le feature set**. Un outil peut être techniquement impressionnant, viral, porté par une communauté active — et ne pas être le bon choix pour un contexte donné. Ce n'est pas un jugement sur l'outil, c'est un jugement sur le *fit*.

En fintech/wealthtech, où je travaille habituellement, la question revient sans cesse : adopte-t-on une techno parce qu'elle répond à un besoin réel, ou parce qu'elle est en tête des trending repos cette semaine ?

Hermes Agent mérite d'être suivi. Pour un pipeline batch à budget zéro, ce n'est pas le bon outil. Reconnaître ça aussi clairement que de reconnaître quand un outil *est* le bon choix — c'est une compétence à part entière.

---

*Nabil Sidhoum — Senior .NET Tech Lead, Paris. Spécialisé fintech/wealthtech, Clean Architecture, industrialisation d'agents IA en production.*

*GitHub : [nabil-sidhoum](https://github.com/nabil-sidhoum) — Portfolio : [nabil-sidhoum.github.io](https://nabil-sidhoum.github.io)*
