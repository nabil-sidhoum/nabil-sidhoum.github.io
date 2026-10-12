# Mon blog répondait 404 aux robots, et je ne l'avais jamais vu

*Publié le 12 octobre 2026 — Nabil Sidhoum, Senior .NET Tech Lead*

---

Pendant des mois, chaque lien direct vers un article de ce blog a répondu « page introuvable ». Pas à vous : dans votre navigateur, l'article s'affichait normalement. Mais à LinkedIn quand je partageais un lien, aux moteurs de recherche, à tout ce qui lit une page sans l'exécuter. Le site fonctionnait pour les visiteurs et mentait aux robots, et rien dans mon usage quotidien ne pouvait me le montrer.

Je l'ai découvert par hasard, en réécrivant le README de mon profil GitHub. Pour vérifier les liens que j'y mettais, une commande toute bête :

```
curl -s -o /dev/null -w "%{http_code}" https://nabil-sidhoum.github.io/blog/regles-qui-peuvent-echouer
404
```

Voici comment un site peut afficher une page et la déclarer introuvable en même temps, comment je l'ai corrigé sans quitter GitHub Pages, et ce que ça m'a rappelé sur la façon de vérifier un système.

---

## Une SPA sur un hébergement statique

Ce portfolio est une application Blazor WebAssembly, déployée sur GitHub Pages. Pas de serveur : GitHub sert des fichiers, point. Toute la navigation est gérée côté navigateur par le routeur Blazor, qui lit l'URL et affiche le bon composant.

Le problème est connu de toutes les SPA hébergées en statique. Quand on arrive directement sur `/blog/regles-qui-peuvent-echouer`, GitHub Pages cherche un fichier à ce chemin. Il n'en trouve aucun, puisque cette page n'existe que dans le routeur de l'application. La parade classique, celle que mon workflow appliquait, consiste à copier `index.html` en `404.html` :

```yaml
# 5. Fallback 404 pour le routing SPA Blazor
- name: Create 404.html for SPA fallback
  run: cp build/wwwroot/index.html build/wwwroot/404.html
```

GitHub Pages sert alors `404.html` pour toute adresse inconnue. Ce fichier étant l'application elle-même, Blazor démarre, lit l'URL et affiche l'article. Le visiteur ne voit aucune différence.

Sauf que GitHub sert ce fichier avec le code qu'il porte dans son nom : **HTTP 404**.

![Avant le correctif, la même requête est lue de deux façons : le navigateur exécute Blazor et affiche l'article, le robot d'aperçu s'arrête au code 404 et aux balises génériques du site](/posts/assets/404-deux-lecteurs.svg)

---

## Deux lecteurs, deux vérités

Un navigateur et un robot ne lisent pas la même chose.

Le navigateur reçoit la 404, l'ignore en pratique, exécute le JavaScript et le WebAssembly, et finit par afficher l'article. La seule trace du problème est une ligne dans la console, que personne n'ouvre :

```
Failed to load resource: the server responded with a status of 404
```

Un robot d'aperçu, celui qui fabrique la petite carte d'un lien partagé sur LinkedIn ou Slack, s'arrête à la réponse HTTP et au HTML tel qu'il arrive. Il n'exécute pas l'application. Il lit un code 404 et des balises `og:title` et `og:description` qui sont celles de la page d'accueil, puisque `404.html` est une copie d'`index.html`. Un moteur de recherche, de son côté, prend le code 404 au mot : cette page n'existe pas.

Le défaut était donc double. Le code HTTP disait « introuvable », et même un robot indulgent n'aurait trouvé que le titre et le résumé du portfolio, jamais ceux de l'article. Chaque lien direct vers un article portait cette double erreur.

Ce qui rend ce bug instructif, c'est qu'il était **invisible depuis la position de l'auteur**. Je consulte mon site dans un navigateur. Je clique sur mes articles depuis la liste du blog, une navigation interne qui ne recharge rien et ne passe jamais par le serveur. Aucun de mes gestes habituels ne pouvait révéler le problème.

---

## Mesurer GitHub Pages plutôt que le supposer

La solution de principe est simple : donner à GitHub Pages un vrai fichier pour chaque route. Mais quel nom lui donner ? Un fichier `blog/regles-qui-peuvent-echouer/index.html`, ou `blog/regles-qui-peuvent-echouer.html` ? La réponse dépend de la façon dont GitHub Pages résout une URL, et je ne voulais pas la deviner. Le site en production contenait déjà de quoi la mesurer :

| Requête | Ce qui existe | Réponse mesurée |
|---|---|---|
| `/posts`, `/css` | un dossier | **301** vers `/posts/`, `/css/` |
| `/404`, `/index` | un fichier `404.html`, `index.html` | **200** direct, sans redirection |
| `/blog/regles-qui-peuvent-echouer` | rien | **404**, avec `404.html` |

![GitHub Pages sert un fichier x.html en 200 direct, redirige un dossier en 301 et répond 404 sinon](/posts/assets/github-pages-resolution.svg)

Le choix s'impose de lui-même. Un dossier par article provoquerait une redirection systématique vers l'adresse avec barre finale. Un fichier `blog/<slug>.html` est servi en 200 à l'adresse exacte, celle que je partage et que les robots ont déjà en cache.

Il restait un cas ambigu : la liste `/blog`. Les articles imposent un dossier `blog/`, et je voulais aussi un fichier `blog.html`. Je n'avais aucun moyen de savoir à l'avance lequel des deux GitHub servirait. J'ai donc généré les deux : `blog.html` et `blog/index.html`. Si le fichier l'emporte, `/blog` répond 200. Si le dossier l'emporte, `/blog` redirige vers `/blog/`, qui répond 200, et j'ai vérifié en production que le routeur Blazor accepte la barre finale. Les deux issues sont bonnes, la mesure dirait laquelle se produit.

---

## Une page par route, générée au déploiement

Le correctif tient dans un script Python lancé par le workflow de déploiement, juste après la création de `404.html`. Il lit l'index des articles, `posts/index.json`, et écrit une copie d'`index.html` par route en ne changeant que huit balises : le titre, la description, `og:title`, `og:description`, `og:url`, `og:type` et les deux balises `twitter:`. L'application reste exactement la même. Seules les métadonnées diffèrent d'une copie à l'autre.

```python
page = render(
    template,
    article["Title"] + " · " + AUTHOR,
    article["Summary"],
    SITE_URL + "/blog/" + slug,
    "article")
write(root / "blog" / (slug + ".html"), page)
```

![La chaîne de déploiement : publication, forçage du base href, copie en 404.html, puis génération d'une page par route avec ses garde-fous, pour un résultat en 200 sur les articles et la liste, et 404 sur les routes inconnues](/posts/assets/routes-statiques-deploiement.svg)

Deux choix de conception me tenaient à cœur.

**Échouer plutôt que publier faux.** Chaque remplacement exige que la balise visée apparaisse exactement une fois dans `index.html`. Si un jour je renomme une balise ou en supprime une, le script s'arrête et le déploiement avec lui. Je préfère un site qui ne se met pas à jour à un site qui publie, en silence, des pages sans leurs métadonnées. C'est la même logique que pour mes règles d'architecture : un contrôle qui ne peut pas échouer ne contrôle rien.

**Ne rien déclarer à la main.** Le script relit l'index à chaque déploiement. Publier un article reste la même opération qu'avant : un fichier Markdown et une entrée dans `index.json`. Aucune liste de routes à maintenir, rien à oublier.

---

## Ce que la revue de sécurité a trouvé

Le slug d'un article devient ici un nom de fichier. Il fallait donc le valider, avec la même règle que le service qui charge les articles côté Blazor : `^[a-z0-9-]+$`, pour qu'aucun `../` ne permette d'écrire hors du dossier publié.

La revue de sécurité de la pull request a relevé un détail que j'aurais laissé passer. En Python, `re.match` avec un motif terminé par `$` accepte une fin de ligne finale : le slug `"abc\n"` passait la validation.

```python
>>> re.compile(r'^[a-z0-9-]+$').match('abc\n')
<re.Match object; span=(0, 3), match='abc'>
>>> re.compile(r'^[a-z0-9-]+$').fullmatch('abc\n')
>>>
```

Le risque réel était à peu près nul, puisque l'index est versionné dans mon propre dépôt et qu'aucune barre oblique ne pouvait passer. Mais une validation qui accepte plus que ce qu'elle annonce est une validation fausse, et la corriger coûtait un mot : `fullmatch`.

---

## Vérifier avec le bon lecteur

Le bug était passé inaperçu parce que je vérifiais avec le mauvais lecteur. Pour le correctif, j'ai voulu vérifier avec celui qui compte.

Avant de livrer, j'ai simulé GitHub Pages en local avec un serveur de quelques lignes qui reproduit les trois règles mesurées : fichier `.html` en 200, dossier en 301, sinon `404.html` en 404. Sur la vraie sortie d'un `dotnet publish`, le lien direct d'un article répondait 200, Blazor démarrait depuis ce chemin imbriqué, et la console restait vide. Une route inexistante, elle, répondait toujours 404, ce qu'elle doit faire.

En production, le premier test après le déploiement a pourtant renvoyé... 404, partout. Les fichiers étaient bien publiés sur la branche `gh-pages`. Simplement, GitHub Pages n'avait pas fini sa propre construction, et le CDN servait encore l'ancienne réponse depuis son cache (`X-Cache: HIT`, `Cache-Control: max-age=600`). Une minute plus tard :

| URL | Avant | Après |
|---|---|---|
| `/blog/regles-qui-peuvent-echouer` | 404 | **200**, avec le titre et le résumé de l'article |
| `/blog` | 404 | **200** direct : `blog.html` est servi avant le dossier |
| `/blog/inexistant` | 404 | 404 |

La question laissée ouverte par la mesure avait trouvé sa réponse : GitHub sert `blog.html` en priorité sur le dossier `blog/`. J'avais prévu les deux cas, je n'aurai jamais à me demander lequel était le bon.

Au passage, un défaut voisin est apparu. Une fois l'application chargée, l'onglet du navigateur affichait « Blog · Nabil Sidhoum » pour tous les articles, car le titre était écrit en dur dans le composant Blazor. Le titre statique, désormais correct, était aussitôt écrasé. Le composant lit maintenant le titre de l'article dans l'index, déjà mis en cache par le service, et affiche le même titre que la page statique. L'onglet ne bascule plus au chargement.

---

## Ce que ça illustre

Un système a plusieurs lecteurs, et chacun ne voit qu'une partie de la vérité. Mon navigateur voyait un site qui marche. Un robot voyait un site cassé. Les deux avaient raison sur ce qu'ils lisaient. Le problème, c'est que je ne vérifiais qu'avec l'un d'eux, celui qui exécute tout et pardonne beaucoup.

C'est un piège classique : on teste avec l'outil qu'on a sous la main, souvent le plus tolérant. Le navigateur rattrape une 404, comme une interface rattrape une API qui renvoie 200 avec un message d'erreur dans le corps. Tant que le lecteur final est indulgent, le défaut reste invisible. Il apparaît le jour où arrive un lecteur qui prend les codes au mot.

Les deux réflexes que j'en retiens sont simples. **Vérifier au niveau du protocole**, pas seulement au niveau de l'écran : un `curl` d'une ligne aurait trouvé ce bug le premier jour. Et **mesurer plutôt que supposer** le comportement de la plateforme : la façon dont GitHub Pages résout une URL ne se devine pas, elle se lit dans trois réponses HTTP.

---

*Nabil Sidhoum — Senior .NET Tech Lead, Paris. Spécialisé fintech/wealthtech, Clean Architecture, industrialisation d'agents IA en production.*

*GitHub : [nabil-sidhoum](https://github.com/nabil-sidhoum) — Portfolio : [nabil-sidhoum.github.io](https://nabil-sidhoum.github.io)*