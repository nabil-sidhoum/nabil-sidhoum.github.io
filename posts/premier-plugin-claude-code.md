# Mon premier plugin pour Claude Code : une jauge de contexte et de quota, pas à pas

*Publié le 14 octobre 2026 — Nabil Sidhoum, Senior .NET Tech Lead*

---

Une longue tâche dans Claude Code peut s'arrêter au milieu, faute de contexte ou de quota. J'en connaissais deux formes. Un sous-agent qui, au-delà d'environ 110 000 tokens de contexte dans la session qui l'a lancé, cesse de rendre son rapport sans rien dire. Et une limite de session qui coupe un travail en cours. Dans les deux cas, je l'apprenais après coup : la passe de relecture n'avait rien rendu, ou il fallait reprendre la tâche à froid.

Les chiffres existent pourtant. Il suffit de taper `/context` ou d'ouvrir la page d'usage. Mais personne ne les a sous les yeux au moment où ils comptent.

J'ai donc écrit une jauge : une ligne permanente au-dessus du champ de saisie, qui affiche le contexte et les deux limites de quota, colorée par seuil. C'est mon premier plugin pour Claude Code, un « mod » dans le vocabulaire de l'outil. Cet article raconte comment je l'ai construit, étape par étape, avec les pièges rencontrés en chemin. Si vous voulez le refaire, vous avez tout ce qu'il faut ici. Et si vous préférez partir de mon plugin, je le partage volontiers.

---

## Ce que la jauge affiche

Voici la ligne relevée le jour de la livraison, après un rechargement des plugins :

```
Contexte 663 k / 1,0 M (66 %) · 5 h 71 %, réinit. dans 2 h 12 · 7 j 52 %, réinit. ven. 16/10 · Opus 5.5
```

| Segment | Ce qu'il dit |
|---|---|
| Contexte | Les tokens utilisés sur la taille de la fenêtre, et le pourcentage |
| 5 h | La limite de cinq heures, et le temps restant avant sa réinitialisation |
| 7 j | La limite de sept jours, et le jour de sa réinitialisation |
| Modèle | Le modèle actif, relu à chaque mesure puisque `/model` peut le changer en cours de session |

Chaque segment prend sa couleur selon son propre pourcentage : vert, orange à partir de 70 %, rouge à partir de 90 %. Sur la ligne ci-dessus, la limite de cinq heures était déjà orange. Les deux seuils se règlent dans `/config`. Une donnée absente s'affiche `n/d`.

Une décision de principe, enfin : **la jauge lit et affiche, rien d'autre.** Elle n'écrit aucun fichier, ne lance aucun processus, n'appelle ni le modèle ni le réseau.

![Trois sources alimentent l'état du plugin : la lecture à l'ouverture, la mesure après chaque tour et une minuterie chaque minute ; des fonctions pures en tirent le texte et la couleur, et le rendu dessine la ligne au-dessus du champ de saisie](/posts/assets/jauge-cycle-mise-a-jour.svg)

---

## Avant de commencer : ce qu'est un plugin ici

Un plugin de ce type est un dossier qui contient trois choses : un manifeste qui le décrit, une déclaration qui désigne son module, et le module lui-même, un fichier TypeScript. Ce module s'abonne à des événements de Claude Code (« une session démarre », « un tour vient de se terminer », « dessine la zone au-dessus du champ de saisie ») et y répond. Il tourne à l'intérieur de Claude Code, dans un environnement à part : ni Node, ni navigateur.

Un avertissement utile : cette API est en accès anticipé, et la documentation fournie avec Claude Code prévient qu'elle change d'une version à l'autre. Tout ce qui suit a été écrit et testé sur la version 2.1.299.

---

## Étape 1 : chercher dans les types de quoi répondre à chaque besoin

Claude Code embarque un skill dédié à l'écriture de plugins, `plugin-authoring`, qui se charge quand on lui demande d'en écrire un. À côté de lui, il dépose un fichier `claude-code.d.ts`. C'est la liste exacte, pour la version installée, de tout ce qu'un plugin peut écouter et appeler. Puisque l'API change d'une version à l'autre, c'est la seule référence fiable. Elle est aussi copieuse : 22 141 lignes.

On ne lit pas un tel fichier de bout en bout. On le fouille, besoin par besoin. Avant d'écrire la moindre ligne, j'ai dressé ce tableau :

| Ce dont la jauge a besoin | Ce que les types proposent |
|---|---|
| Dessiner une ligne au-dessus du champ de saisie | Un hook de rendu sur le composant `AbovePrompt` |
| Connaître le contexte | `context.percent`, `context.tokens`, `context.window` |
| Connaître les limites de 5 heures et de 7 jours | Une liste de fenêtres, chacune avec un pourcentage et une date de réinitialisation |
| Être prévenue quand les chiffres changent | L'événement `session.measure`, poussé après chaque tour |
| Lire les chiffres à l'ouverture | `$.session.usage()` |
| Connaître le modèle actif | `$.session.model()` |
| Laisser chacun régler ses seuils | Une configuration déclarée dans le manifeste |

Cette fouille a un second intérêt, moins visible : elle dit aussi ce qui peut manquer. Les types précisent par exemple que la liste des limites de quota est **vide** pour un compte API, ou avant la première réponse d'une session. Je le savais avant d'écrire le code, et la jauge affiche `n/d` dans ce cas au lieu de planter.

Ce que les types ne disaient pas, je l'ai noté comme une question à trancher par un essai. La principale : un coéquipier qui clone le projet aura-t-il la jauge sans rien installer ? C'est l'objet de l'étape 3.

---

## Étape 2 : poser les trois fichiers

Le plugin vit dans mon template dotnet-claude-template, celui que je pose sur chaque nouveau projet .NET, dans un dossier `.claude/plugins/` :

```
.claude/plugins/
├── .claude-plugin/marketplace.json   ← le catalogue local : liste les plugins du dossier
└── quota-guard/
    ├── .claude-plugin/plugin.json    ← le manifeste et les réglages
    ├── hooks/                        ← la déclaration, le module, la mise en forme, les tests
    ├── types/index.d.ts              ← ce que la jauge garde en mémoire pendant la session
    └── README.md
```

Le manifeste déclare les deux seuils réglables, avec leurs bornes et leurs valeurs par défaut. Claude Code les présente alors dans `/config`. En voici un extrait :

```jsonc
{
  "name": "quota-guard",
  "version": "0.1.0",
  "description": "Jauge de contexte et de quota au-dessus du champ de saisie",
  "types": "./types/index.d.ts",
  "userConfig": {
    "seuil_orange": {
      "type": "number",
      "title": "Seuil orange (%)",
      "default": 70,
      "min": 1,
      "max": 100
    }
    // seuil_rouge : même forme, 90 par défaut
  }
}
```

La déclaration des hooks tient en une ligne, `hooks/hooks.json` :

```json
{ "modules": ["./register.tsx"] }
```

Et le catalogue local, qui fait du dossier une source de plugins que Claude Code sait installer (description retirée pour la lecture) :

```json
{
  "name": "dotnet-claude-template",
  "owner": { "name": "dotnet-claude-template" },
  "plugins": [{ "name": "quota-guard", "source": "./quota-guard" }]
}
```

---

## Étape 3 : prouver que le plugin se charge, avant d'écrire la jauge

La première version du module ne faisait rien d'autre qu'afficher deux mots :

```tsx
import type { Register } from 'claude-code'

// Prouver le chargement, rien d'autre.
export const register: Register = on => {
  on('ui.render', { component: 'AbovePrompt' }, ($, e) => {
    const { Text } = $.ui.resolve(e)
    return <Text dimColor>quota-guard chargé</Text>
  })
}
```

Ce détour paraît inutile. Il ne l'est pas. Si l'activation ne fonctionne pas, mieux vaut le savoir avant d'avoir écrit deux cents lignes.

Pour un essai rapide sur son propre poste, `claude --plugin-dir <dossier>` charge un plugin le temps d'une session. Mais je voulais qu'un coéquipier ait la jauge en clonant le projet. Il faut pour cela l'installer à la portée du projet, depuis la racine du dépôt :

```bash
claude plugin marketplace add ./.claude/plugins --scope project
claude plugin install quota-guard@dotnet-claude-template --scope project
```

Ces deux commandes écrivent deux clés dans le `.claude/settings.json` versionné du projet : `extraKnownMarketplaces`, qui déclare le catalogue, et `enabledPlugins`, qui active le plugin. À l'ouverture de la session suivante, « quota-guard chargé » s'est affiché au-dessus du champ de saisie. L'essai était réussi. Le `git diff` du fichier, lui, montrait un défaut.

**La commande écrit un chemin absolu.** Le mien commençait par `D:\DEVS\`. Il existe sur mon disque, et sur aucun autre. Commité tel quel, le réglage aurait activé la jauge chez moi et chez personne d'autre, sans la moindre erreur visible. La documentation officielle admet un chemin relatif pour ce type de catalogue, qui *« resolves against your repository's main checkout »*. Je l'ai corrigé à la main :

```json
"extraKnownMarketplaces": {
  "dotnet-claude-template": {
    "source": {
      "source": "directory",
      "path": "./.claude/plugins"
    }
  }
},
"enabledPlugins": {
  "quota-guard@dotnet-claude-template": true
}
```

Restait à prouver qu'un coéquipier hérite bien du plugin. J'ai voulu le faire en ligne de commande, sur une configuration vierge. Impossible : hors d'une session interactive, la CLI ne lit pas les déclarations d'un projet. Elles ne sont lues qu'à l'ouverture d'une session, au moment où l'on accorde sa confiance au dossier. La vérification est donc manuelle : un clone neuf, ouvert en session et marqué de confiance, qui affiche la jauge sans aucune installation. Je l'ai faite avant de fusionner.

Dernier confort : le plugin est lu directement depuis son dossier, sans copie. Après une modification, `/reload-plugins` suffit pour la voir dans la session en cours.

---

## Étape 4 : écrire la mise en forme et ses tests d'abord

Toute la logique de la jauge tient dans de petites fonctions qui ne dépendent pas de Claude Code : choisir une couleur selon un pourcentage, écrire « 101 k » plutôt que « 101000 », calculer « dans 2 h 12 », retomber sur `n/d` quand une donnée manque. Je les ai isolées dans un fichier `format.ts`, et j'ai écrit leurs quatorze tests avant elles.

```ts
// Un réglage incohérent (orange au-dessus de rouge, hors de 0 à 100) retombe sur les défauts.
export function seuils(options: { orange?: unknown; rouge?: unknown }): Seuils {
  const orange = nombre(options.orange) ?? DEFAUT.orange
  const rouge = nombre(options.rouge) ?? DEFAUT.rouge
  const isCoherent = orange > 0 && orange < rouge && rouge <= 100
  return isCoherent ? { orange, rouge } : { ...DEFAUT }
}
```

```ts
test('niveau : vert, orange et rouge aux bornes, n/d sans donnée', () => {
  expect(niveau(69.9, DEFAUT)).toBe('ok')
  expect(niveau(70, DEFAUT)).toBe('orange')
  expect(niveau(90, DEFAUT)).toBe('rouge')
  expect(niveau(undefined, DEFAUT)).toBe('nd')
})
```

Un de ces tests a refusé de passer. Il vérifiait l'affichage d'une réinitialisation hebdomadaire au 17 octobre 2026, et attendait « ven. 17/10 ». Le code affichait « sam. 17/10 ». Le défaut n'était ni dans la conversion de date ni dans le tableau des jours : le 17 octobre 2026 est un samedi. Le test était faux, le code était juste.

L'anecdote est mince, mais elle rappelle une chose qu'on oublie volontiers quand on écrit les tests avant le code : un test écrit d'abord est une affirmation comme une autre, et il peut être faux. Quand il échoue, la première question n'est pas « qu'est-ce que le code a cassé ? », mais « qui a raison ? ».

Un détail pour qui reprendrait l'idée : les tests fixent l'heure à midi UTC, pour que la date affichée ne dépende pas du fuseau de la machine qui les exécute.

---

## Étape 5 : brancher le module

Le module n'a plus qu'à assembler. Il écoute trois choses : l'ouverture de la session, pour une première lecture ; l'événement `session.measure`, après chaque tour ; et une minuterie, pour que « dans 2 h 12 » continue de bouger entre deux tours.

```tsx
on('session.measure', async ($, e, next) => {
  const lue: Mesure = { context: e.context, rateLimits: [...e.rateLimits] }
  await update($, mesure, () => lue)
  await releverModele($)
  return next(e)
})
```

Les chiffres reçus ne sont pas gardés dans une variable du module, mais dans l'état que Claude Code tient pour le plugin. La documentation du skill en donne la raison : un rechargement à chaud perd les variables du module. Le dessin relit cet état, et Claude Code le redessine de lui-même quand il change :

```tsx
on('ui.render', { component: 'AbovePrompt' }, async ($, e, next) => {
  if (e.props.hasSurvey) {
    return next(e)
  }
  // … lecture de l'état, puis calcul des segments par format.ts
  const { Text } = $.ui.resolve(e)
  return (
    <Text>
      {liste.map((seg, i) => (
        <Text color={COULEUR[seg.niveau]}>
          {(i > 0 ? ' · ' : '') + seg.texte}
        </Text>
      ))}
    </Text>
  )
})
```

Deux précisions sur ce code. Quand Claude Code affiche un sondage au même endroit, la jauge lui laisse la place : c'est le rôle du test sur `hasSurvey`. Et les couleurs ne sont pas des codes en dur, mais des clés du thème (`success`, `warning`, `error`), qui suivent le mode clair ou sombre de chacun.

Un piège ici, signalé par le validateur de plugins : il refuse qu'une fonction reçoive `$`, l'objet par lequel le plugin parle à Claude Code, si elle n'est pas déclarée au niveau du fichier. La fonction qui relit le modèle actif est donc déclarée en tête du module, à côté de l'état.

---

## Étape 6 : tester le module sans moteur

Claude Code fournit un exécuteur de tests pour les plugins, `claude plugin test`. Le module y est monté comme une session le dessinerait, sur la surface de son choix. J'ai écrit chaque scénario une fois et l'ai fait tourner sur le terminal puis sur l'application de bureau.

Le premier lancement a échoué partout, avec le même message : `no implementation for session.measure`. La raison est simple, une fois comprise. En session, le plugin est posé sur le moteur de Claude Code, qui répond aux événements, donne l'heure et dessine le reste de l'écran. Sous l'exécuteur de tests, **il n'y a rien en dessous.** Chaque appel que le plugin transmet tombe dans le vide.

![En session, le moteur de Claude Code répond sous le plugin ; sous l'exécuteur de tests, rien ne répond et le test doit fournir son propre socle](/posts/assets/socle-test-plugin.svg)

Il faut donc écrire soi-même ce qui tient lieu de moteur. Dans mes tests, ce socle tient en trois lignes : un gestionnaire de `session.measure` qui renvoie ce qu'on lui donne, une horloge simulée arrêtée au 14 octobre 2026 à midi, et un rendu qui répond par une boîte vide.

```ts
// Ce qui tient lieu de moteur sous le plugin (typage retiré pour la lecture)
const socle = (on) => {
  on('session.measure', ($, e) => ({ changed: e.changed }))
  mock.clock(on, { now: MAINTENANT })
  on('ui.render', () => ({ type: 'Box', props: {}, children: [] }))
}
```

Un test complet se lit alors comme un scénario :

```ts
test(`${surface} : 5 h à 70 %, orange`, async ($, on) => {
  socle(on)
  await mesurer($, 70, 10)
  const ui = await monter($, surface)
  const s = await segment(ui, 1)
  expect(s?.text).toBe(' · 5 h 70 %')
  expect(s?.props.color).toBe('warning')
})
```

Un dernier piège : j'avais prévu de retrouver chaque segment par une `key`. Le moteur ne la conserve pas sur un `Text` imbriqué. Les tests retrouvent donc les segments par leur position, ce qui reste stable puisque l'ordre des segments fait partie de ce que la jauge promet.

---

## Étape 7 : valider, puis regarder

Deux commandes, à lancer souvent :

```bash
claude plugin validate .claude/plugins/quota-guard
claude plugin test .claude/plugins/quota-guard
```

La première lit le manifeste et le module comme Claude Code le fera, et liste ce que le module appelle. Pour la jauge, la liste se résume à cinq accès : l'horloge, le modèle actif, l'usage de la session, l'état du plugin et la résolution des composants d'affichage. C'est ma preuve que la jauge est en lecture seule : je n'ai pas à me croire sur parole. La seconde fait tourner les vingt-six tests, quatorze sur la mise en forme et douze sur le module.

Reste à regarder la jauge dans une vraie session. Une surprise, qui n'en est pas une : juste après un `/reload-plugins`, le segment de contexte affiche `n/d` jusqu'à la réponse suivante. Les types l'annoncent, le contexte n'est connu *« once a response of the live window reported one »*. La jauge affiche ce qu'elle sait, et à cet instant elle ne sait rien.

---

## Ce que je n'ai pas prouvé

La règle que je m'impose est de voir un test échouer avant de le voir passer. Un test qui n'a jamais été rouge peut être vert pour une mauvaise raison.

Pour les tests du module, je ne peux pas dire que je l'ai respectée. Ils ont échoué, mais faute de socle, pas parce que la jauge affichait une mauvaise couleur ou un mauvais texte. Puis le socle est arrivé, et ils sont passés. Je ne les ai donc jamais vus rouges pour la bonne raison. Les tests de mise en forme, eux, ont d'abord été rouges parce que le fichier testé n'existait pas encore : c'est le rouge le plus faible qui soit, celui qui prouve seulement que le test s'exécute.

Ces tests vérifient des valeurs précises, un texte exact et une clé de couleur, et je les ai relus un par un. Mais je préfère l'écrire que laisser croire qu'ils ont été éprouvés.

---

## Où j'en suis

Ce que j'ai livré est un premier lot, et sa limite principale tient en une phrase : **la jauge prévient, elle n'empêche rien.** Une session peut toujours atteindre 100 % de son quota sous mes yeux.

La suite est prévue, pas faite. Un garde-fou qui refusera les appels d'outils au-delà d'un seuil. Un relais, pour passer proprement la main quand une session approche de la coupure. Une reprise programmée, pour relancer le travail quand la limite se réinitialise. Le premier lot rend la coupure visible. Les suivants devront agir sur elle, ce qui pose des questions autrement plus délicates que l'affichage d'une ligne.

Restent aussi les limites de ce qui existe. Avec une clé d'API, les deux segments de quota restent à `n/d`, puisque ces limites n'existent que sur un abonnement. Une organisation peut interdire les mods qu'elle ne gère pas, et la jauge ne s'affiche alors pas. Je l'ai testée sur le terminal et l'application de bureau, pas sur l'extension VS Code ni sur mobile.

Si je devais résumer ce premier plugin en trois conseils pour qui veut écrire le sien :

**Fouiller les types avant d'écrire.** Ils disent où sont les chiffres, quand ils arrivent et quand ils manquent. Ce qu'ils ne disent pas devient une liste d'essais, faits avant le code plutôt que découverts après.

**Vérifier hors de son poste.** Une activation qui marche chez soi ne prouve rien pour les autres. Le chemin absolu ne se voyait que dans un `git diff`, et l'héritage que sur un clone neuf.

**Sortir la logique du module.** Tout ce qui se calcule sans Claude Code se teste sans lui, simplement. Le reste demande un socle, et l'écrire oblige à comprendre exactement ce que le moteur faisait.

Le plugin vit pour l'instant dans mon template, qui n'est pas public. Si vous voulez l'essayer ou en partir pour le vôtre, écrivez-moi : je le partage volontiers.

---

*Nabil Sidhoum — Senior .NET Tech Lead, Paris. Spécialisé fintech/wealthtech, Clean Architecture, industrialisation d'agents IA en production.*

*GitHub : [nabil-sidhoum](https://github.com/nabil-sidhoum) — Portfolio : [nabil-sidhoum.github.io](https://nabil-sidhoum.github.io)*