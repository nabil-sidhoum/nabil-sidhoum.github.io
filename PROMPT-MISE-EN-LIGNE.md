# Prompt de mise en ligne — article « Pour que l'IA vérifie mon code… »

> Fichier de travail temporaire. **Le supprimer avant de merger** : il n'a pas vocation à rester
> dans le dépôt. Il existe pour qu'une session ouverte ici, avec les règles et les agents locaux
> chargés, puisse finir le travail sans reconstituer le contexte.

## Le prompt à coller

```
Je veux finaliser et publier l'article de blog déjà rédigé sur la branche
feat/article-regles-qui-peuvent-echouer, actuellement poussée.

Ce qui est déjà fait, ne le refais pas :
- wwwroot/posts/regles-qui-peuvent-echouer.md (155 lignes)
- wwwroot/posts/assets/paliers-adoption-ia.svg, dependances-couches.svg, vert-sur-zero-type.svg
- l'entrée en tête de wwwroot/posts/index.json

Déjà vérifié mécaniquement : slug conforme au pattern de BlazorPortfolio, H1 identique
au Title de l'index, date au format attendu, aucun HTML brut hors blocs de code, images
en chemin absolu /posts/assets/, SVG bien formés avec role/aria-label/desc et aucune
ressource externe, rendu contrôlé sur les deux fonds (#0d0d0f et #fafaf9).

Ce que je te demande :

1. Lis .claude/rules/blog-conventions.md, puis relis l'article et les trois SVG pour
   vérifier ce qu'une vérification mécanique ne peut pas juger : la justesse du ton par
   rapport aux articles existants, et la cohérence des SVG avec ceux de wwwroot/posts/assets/.

2. dotnet build et les tests unitaires.

3. Lance le site en local et invoque ui-verifier sur /blog et sur
   /blog/regles-qui-peuvent-echouer, EN THÈME SOMBRE ET EN THÈME CLAIR. Vérifie :
   - l'article apparaît en tête de /blog, avec son résumé et ses tags ;
   - le rendu Markdown : titre, sections, tableaux GFM, citation en bloc, code ;
   - les trois SVG se chargent depuis /posts/assets/ et restent lisibles dans les deux
     thèmes, aucun élément confondu avec le fond ;
   - console : 0 erreur, 0 violation CSP.

4. Si tout passe : supprime PROMPT-MISE-EN-LIGNE.md, commite, et ouvre la PR vers main.
   Rappel du ruleset : merge REBASE uniquement, historique linéaire, et les deux status
   checks (build-and-test, Conventions C#) doivent être verts avant merge.

Si un point échoue, arrête-toi et dis-moi lequel avant de corriger.
```

## Contexte utile, si la session pose des questions

**Ce que l'article raconte.** La conversion d'un template .NET qui prescrivait des conventions sans
en imposer aucune. Deux chantiers : sept règles de style passées bloquantes, et quatre règles
d'architecture devenues des tests exécutables. Le fil rouge est l'échelle d'adoption de l'IA de
Boris Cherny (juillet 2026), dont le palier 2 suppose que l'IA vérifie son travail par les tests,
le build et le lint, ce qui n'a de sens que si ces trois-là savent dire non.

**Les chiffres cités sont mesurés**, pas estimés : 101 déclarations de sévérité dont aucune
bloquante, seize prescriptions d'architecture dont aucune vérifiée, une classe à trois violations
qui compilait en 0 avertissement et 0 erreur.

**L'article ne prétend pas avoir changé de palier**, seulement avoir levé une condition nécessaire.
C'est délibéré : la nuance est ce qui le rend crédible auprès d'un lecteur technique.

## Ce qui reste ouvert, et que tu peux vouloir trancher

- Les tags retenus sont `.NET`, `IA`, `Architecture`, `Agents IA`. Un tag `Claude Code` existerait
  ailleurs qu'ici si tu veux le créer.
- Le résumé de l'index fait quatre phrases, la convention en demande une à trois. À raccourcir si
  la carte le rend mal.
