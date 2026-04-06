Effectue une analyse complète de la solution et ajoute les nouvelles anomalies dans `anomalies.local.md`.

## Étape 1 — Analyse du build

Exécute :
```
dotnet build --no-incremental
```
Capture toutes les lignes contenant `warning` ou `error` (ignore `0 Avertissement(s)` et `0 Erreur(s)`).

## Étape 2 — Analyse statique du code source

Parcours tous les fichiers `.cs` et `.razor` du projet et vérifie les règles suivantes :

### Architecture (.claude/rules/clean-architecture.md)
- Respect des responsabilités par couche (Domain / Application / Infrastructure / Api)
- Absence de logique métier dans les Controllers
- Absence d'accès direct aux données depuis Application
- Absence de dépendance circulaire entre couches
- Absence de données sensibles dans les logs

### Conventions C# (.claude/rules/csharp-conventions.md)
- Pas de `var` — types explicites partout
- Modificateurs d'accès explicites sur toutes les déclarations
- Accolades obligatoires sur tous les blocs
- Collections vides : `[]` uniquement — jamais `new List<T>()` ni `Enumerable.Empty<T>()`
- Namespaces block-scoped uniquement (`namespace Foo { }`, pas `namespace Foo;`)
- Pas de primary constructors
- Pas de `.Result` / `.Wait()`
- Nullable reference types : non-nullable initialisé avec `""` ou `[]`, nullable avec `?`
- Arguments publics validés (`ArgumentNullException`)

### Tests (.claude/rules/testing.md)
- Nommage `Methode_Scenario_ResultatAttendu`
- Pattern AAA avec commentaires `// Arrange`, `// Act`, `// Assert` explicites
- Données JSON dans `JsonFixtures.cs` avec `/*lang=json,strict*/`
- Couverture minimale : nominal, erreur HTTP, mapping, cas limites

## Étape 3 — Mise à jour de `anomalies.local.md`

1. Lis le fichier `anomalies.local.md` existant
2. Met à jour la date en haut : `# Anomalies détectées — YYYY-MM-DD`
3. Pour chaque anomalie trouvée :
   - Vérifie qu'elle n'est pas déjà présente (évite les doublons)
   - Ajoute-la sous la section correspondante (`clean-architecture.md`, `csharp-conventions.md`, `testing.md`)
   - Format : `- [ ] \`fichier:ligne\` — règle violée — correction attendue`
4. Pour les sections sans anomalie, indique `✅ RAS`
5. En bas du fichier, ajoute une section `## Build` avec les warnings/erreurs Roslyn

## Étape 4 — Résumé

Affiche :
- Nombre total d'anomalies ajoutées (nouvelles uniquement)
- Répartition par section
- Nombre de warnings/erreurs Roslyn
