# Workflow Git — branches, protection, contribution

> Source de vérité de la politique Git du dépôt : modèle de branches, protection, méthodes de merge et process de contribution. Le détail du modèle **branche par phase** employé pendant la refonte (terminée) est archivé dans [`docs/refonte-2026.md`](../../docs/refonte-2026.md).

## Modèle de branches

| Branche | Rôle | Durée de vie |
|---------|------|--------------|
| `main` | Branche stable, déployée sur GitHub Pages | permanente |
| ~~`feature/redesign-portfolio`~~ | Branche d'**intégration** de la refonte (base des PR de phase) | **supprimée** après merge final sur `main` (PR #23) |
| `feat/*`, `fix/*`, `docs/*`, `chore/*`, `ci/*`, `refactor/*` | Branches de travail — une par unité logique | supprimées au merge |

- Aucune modification directe sur `main` : tout passe par une **Pull Request**.
- Les branches de travail ciblent `main`. *(Pendant la refonte — terminée — les PR de phase ciblaient la branche d'intégration `feature/redesign-portfolio`.)*

## Protection des branches (rulesets GitHub)

Un **ruleset actif** sur `main` (enforcement `active`, **aucun bypass — s'applique aussi à l'owner**). Le ruleset `Protect redesign integration` a été **supprimé en fin de refonte** (voir plus bas).

### `main` — ruleset `Protect main` (id `17370109`)

Discipline complète. **Aucun push direct possible**, même en admin.

| Règle | Effet |
|-------|-------|
| Pull request obligatoire | Push direct rejeté ; modif via PR uniquement |
| 0 approbation requise | Self-merge autorisé (dépôt solo) |
| Résolution des conversations | Commentaires de review résolus avant merge |
| Merge `rebase` uniquement | Imposé par le ruleset (`allowed_merge_methods`) ; préserve les commits distincts |
| Historique linéaire | Aucun commit de merge sur `main` |
| Status checks requis | `build-and-test` (CI) + `Conventions C# (dotnet format)` doivent passer avant merge |
| Force-push interdit | `non_fast_forward` |
| Suppression interdite | `main` ne peut pas être supprimée |

### ~~`feature/redesign-portfolio` — ruleset `Protect redesign integration` (id `17370581`)~~ — supprimé

Ruleset **supprimé en fin de refonte** (juin 2026), avant la suppression de la branche d'intégration. Conservé ici pour mémoire : protection minimale (force-push + suppression interdits) sans imposer de process, le workflow PR par phase restant volontaire.

## Réglages du repo

- **Méthodes de merge** : `merge_commit` ✅, `rebase` ✅, `squash` ❌. Les PR de phase vers l'intégration utilisent le **merge commit** ; `main` reste **Rebase-only** via son ruleset (indépendant du réglage repo).
- **Suppression auto des branches** (`delete_branch_on_merge` ✅) : la branche source d'une PR est supprimée **côté distant** dès le merge. Les branches **locales** ne sont pas touchées (`git fetch --prune` pour marquer les orphelines, suppression manuelle ensuite).

## Process de contribution

1. Brancher depuis `main` à jour.
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
- **Suppression de l'intégration (fin de refonte)** — ✅ *effectuée* : le ruleset `Protect redesign integration` (qui bloquait la suppression, y compris l'auto-delete au merge final) a été retiré avant suppression de la branche. Procédure utilisée :
  ```bash
  gh api -X DELETE repos/{owner}/{repo}/rulesets/17370581
  git push origin --delete feature/redesign-portfolio
  ```
- **Status checks requis sur `main`** — ✅ *configurés* : `build-and-test` (`ci-portfolio`) et `Conventions C# (dotnet format)` (`quality-portfolio`), tous deux déclenchés sur `pull_request`. ⚠️ **Ne pas** rendre requis le workflow `deploy-portfolio` : il se déclenche en `workflow_run` **après merge sur `main`** (post-merge), ne tourne jamais sur le commit d'une PR, et le rendre obligatoire **verrouillerait toutes les PR** (statut « Expected » jamais satisfait).
