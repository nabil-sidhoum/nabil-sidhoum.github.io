Analyse le bug décrit et applique un fix conforme à l'architecture et aux conventions.

## Processus

1. **Identifier la couche** — quel fichier, quelle couche (Model / Service / Page / Component / Layout) ?
2. **Lire le code concerné** avant de proposer quoi que ce soit
3. **Consulter `.claude/history/fixes.md`** — un bug similaire a-t-il déjà été corrigé ?
4. **Proposer le fix** — conforme à `.claude/rules/`
5. **Attendre validation** avant d'appliquer
6. **Vérifier la couverture** — le fix est couvert par un test existant ? Sinon, créer le test selon `.claude/rules/testing.md`
7. **Mettre à jour `.claude/history/fixes.md`** après application :

```markdown
## YYYY-MM-DD — Titre court du problème
**Symptôme** : ce qui était observable
**Cause** : origine technique identifiée
**Fix** : ce qui a été fait concrètement
**Leçon** : règle ou pattern à retenir pour éviter la récurrence
```
