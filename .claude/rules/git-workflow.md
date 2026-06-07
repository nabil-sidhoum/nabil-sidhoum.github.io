# Workflow Git — branches, protection, contribution

> Source de vérité de la politique Git du dépôt : modèle de branches, protection, méthodes de merge et process de contribution. Le détail spécifique à la **refonte en cours** (branche par phase, tableau d'état) reste dans `CLAUDE.md`.

## Modèle de branches

| Branche | Rôle | Durée de vie |
|---------|------|--------------|
| `main` | Branche stable, déployée sur GitHub Pages | permanente |
| `feature/redesign-portfolio` | Branche d'**intégration** de la refonte (base et cible des PR de phase) | temporaire (jusqu'à fin Phase 10) |
| `feat/*`, `fix/*`, `docs/*`, `chore/*`, `ci/*`, `refactor/*` | Branches de travail — une par unité logique | supprimées au merge |

- Aucune modification directe sur `main` ni sur la branche d'intégration : tout passe par une **Pull Request**.
- Pendant la refonte, les PR de phase ciblent `feature/redesign-portfolio`. Hors refonte, les branches de travail ciblent `main`.

## Protection des branches (rulesets GitHub)

Deux **rulesets** (enforcement `active`, **aucun bypass — s'appliquent aussi à l'owner**).

### `main` — ruleset `Protect main` (id `17370109`)

Discipline complète. **Aucun push direct possible**, même en admin.

| Règle | Effet |
|-------|-------|
| Pull request obligatoire | Push direct rejeté ; modif via PR uniquement |
| 0 approbation requise | Self-merge autorisé (dépôt solo) |
| Résolution des conversations | Commentaires de review résolus avant merge |
| Merge `rebase` uniquement | Imposé par le ruleset (`allowed_merge_methods`) ; préserve les commits distincts |
| Historique linéaire | Aucun commit de merge sur `main` |
| Force-push interdit | `non_fast_forward` |
| Suppression interdite | `main` ne peut pas être supprimée |

### `feature/redesign-portfolio` — ruleset `Protect redesign integration` (id `17370581`)

Protection **minimale** : un filet contre les fausses manips irréversibles, **sans** imposer de process (le workflow PR par phase reste volontaire).

| Règle | Effet |
|-------|-------|
| Force-push interdit | `non_fast_forward` — protège le travail accumulé des phases |
| Suppression interdite | la branche d'intégration ne peut pas être supprimée par erreur |

## Réglages du repo

- **Méthodes de merge** : `merge_commit` ✅, `rebase` ✅, `squash` ❌. Les PR de phase vers l'intégration utilisent le **merge commit** ; `main` reste **Rebase-only** via son ruleset (indépendant du réglage repo).
- **Suppression auto des branches** (`delete_branch_on_merge` ✅) : la branche source d'une PR est supprimée **côté distant** dès le merge. Les branches **locales** ne sont pas touchées (`git fetch --prune` pour marquer les orphelines, suppression manuelle ensuite).

## Process de contribution

1. Brancher depuis la cible à jour (`main`, ou `feature/redesign-portfolio` en refonte).
2. Implémenter l'unité logique.
3. `dotnet build` → 0 erreur obligatoire.
4. `dotnet test` → si des tests existent.
5. Review (`/project:review` — agents spécialisés selon les fichiers touchés).
6. Commit(s) conventionnels.
7. `git push` + PR vers la cible. La branche source est supprimée automatiquement au merge.

## Conventions de commit (Conventional Commits, en français)

```
feat:     ajout d'une fonctionnalité
fix:      correction de bug
test:     ajout ou correction de tests
refactor: refactorisation sans changement de comportement
docs:     documentation
ci:       intégration continue / déploiement
chore:    maintenance, configuration
```

- Messages en **français**, sans abréviations.
- **Pas de co-auteur Claude** dans les commits ou les PR.
- Un commit par unité logique (les commits `feat` et `docs` restent distincts — ne pas les fusionner via squash).

## Opérations sensibles

- **Hotfix urgent sur `main`** : aucun bypass admin — passer le ruleset `Protect main` en `enforcement: disabled` temporairement, puis le réactiver.
- **Suppression volontaire de l'intégration (fin Phase 10)** : le ruleset `Protect redesign integration` bloque la suppression (y compris l'auto-delete au merge final). Le retirer d'abord, puis supprimer la branche :
  ```bash
  gh api -X DELETE repos/{owner}/{repo}/rulesets/17370581
  git push origin --delete feature/redesign-portfolio
  ```
- **Ajout du status check CI (Phase 10)** : ajouter le check requis (workflow `deploy.yml`) au ruleset `Protect main` une fois le pipeline vert une première fois.
