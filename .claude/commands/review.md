Examine les fichiers modifiés dans la branche courante et vérifie leur conformité avec les règles du projet.

## Étape 1 — Identifier les fichiers modifiés

Exécute `git diff --name-only HEAD` pour obtenir la liste des fichiers modifiés.

## Étape 2 — Analyse des fichiers modifiés

Pour chaque fichier `.cs` ou `.razor` modifié, vérifie :

### Architecture (.claude/rules/clean-architecture.md)
- Respect des responsabilités par couche
- Pas de logique métier dans les Controllers
- Pas d'accès direct aux données depuis Application
- Pas de dépendance circulaire entre couches
- Données sensibles absentes des logs

### Conventions C# (.claude/rules/csharp-conventions.md)
- Pas de `var` — types explicites partout
- Modificateurs d'accès explicites sur toutes les déclarations
- Accolades obligatoires sur tous les blocs
- Collections vides : `[]` uniquement
- Namespaces block-scoped uniquement
- Pas de primary constructors
- Pas de `.Result` / `.Wait()`
- Nullable reference types correctement déclarés
- Arguments publics validés

### Tests (.claude/rules/testing.md)
- Nommage `Methode_Scenario_ResultatAttendu`
- Pattern AAA avec commentaires `// Arrange`, `// Act`, `// Assert`
- Données JSON dans `JsonFixtures.cs`
- Couverture minimale 80% sur les nouveaux services

## Étape 3 — Mise à jour de `anomalies.local.md`

1. Lis le fichier `anomalies.local.md` existant
2. Pour chaque anomalie trouvée :
   - Vérifie qu'elle n'est pas déjà présente (évite les doublons)
   - Ajoute-la sous la section correspondante
   - Format : `- [ ] \`fichier:ligne\` — règle violée — correction attendue`
3. Pour les sections sans anomalie nouvelle, ne touche pas au contenu existant

## Étape 4 — Rapport dans le chat

Affiche pour chaque violation : `fichier:ligne — règle violée — correction attendue`

Si aucune violation : affiche `✅ Aucune anomalie détectée sur les fichiers modifiés.`
