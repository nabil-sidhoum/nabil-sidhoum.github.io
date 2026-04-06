Prépare une pull request GitHub pour les modifications en cours. Ne push rien — génère les commandes à exécuter.

## Processus

1. **Analyser les modifications** — lancer `git status` et `git diff` pour comprendre ce qui a changé
2. **Déterminer le type** :
   - Bug corrigé → préfixe `fix/`
   - Nouvelle fonctionnalité → préfixe `feat/`
3. **Proposer un nom de branche** — format kebab-case, concis, descriptif :
   - `feat/add-user-authentication`
   - `fix/null-reference-handler`
4. **Rédiger le titre de PR** — format Conventional Commits en français :
   - `feat: ajout de l'authentification utilisateur`
   - `fix: correction de la référence nulle dans le handler`
5. **Rédiger la description de PR** avec cette structure :

```markdown
## Contexte
[Pourquoi ce changement est nécessaire]

## Modifications
- [Fichier ou composant] : [ce qui a changé]
- ...

## Tests
- [ ] Tests unitaires ajoutés / mis à jour
- [ ] Build OK (`dotnet build`)
- [ ] Tests OK (`dotnet test`)

## Notes pour le reviewer
[Points d'attention particuliers, choix techniques, compromis]
```

6. **Générer le bloc de commandes à copier-coller** :

```bash
# 1. Créer et basculer sur la branche
git checkout -b [NOM_BRANCHE]

# 2. Stager les modifications
git add [FICHIERS]

# 3. Commiter
git commit -m "[TYPE]: [DESCRIPTION EN FRANÇAIS]"

# 4. Pusher
git push origin [NOM_BRANCHE]

# 5. Ouvrir la PR avec GitHub CLI (si gh installé)
gh pr create \
  --title "[TITRE DE LA PR]" \
  --body "[DESCRIPTION]" \
  --base main
```

## Règles

- Ne jamais commiter directement sur `main` ou `develop`
- Un commit = une unité logique cohérente
- Si `anomalies.local.md` contient des cases non cochées, le signaler avant de générer les commandes
- Si des tests manquent, le signaler explicitement dans la description de PR
- Vérifier que `/project:review` et `/project:run-tests` ont été lancés avant de générer
