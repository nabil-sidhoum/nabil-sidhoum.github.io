Examine les fichiers modifiés dans la branche courante et vérifie leur conformité avec les règles du projet.

## Architecture (.claude/rules/clean-architecture.md)

- Les Models ne contiennent pas de logique (méthodes, propriétés calculées)
- Les Components reçoivent leurs données uniquement via `[Parameter]`, pas d'injection de service
- Les Services retournent `IEnumerable<T>`, pas `List<T>`
- Les Pages chargent les données dans `OnInitializedAsync`, logique dans `@code {}` uniquement
- Pas de code-behind `.razor.cs` sauf `MainLayout`

## Conventions C# (.claude/rules/csharp-conventions.md)

- Pas de `var` — types explicites partout
- Modificateurs d'accès explicites sur toutes les déclarations
- Accolades sur tous les blocs
- Collections vides avec `[]`, jamais `Enumerable.Empty<T>()`
- Namespaces block-scoped

## Tests (.claude/rules/testing.md)

- Tout nouveau service : au moins 3 tests (nominal, erreur 404, mapping)
- Nommage `Methode_Scenario_ResultatAttendu` en français
- Pattern AAA avec commentaires `// Arrange`, `// Act`, `// Assert`
- Données JSON dans `JsonFixtures.cs` avec `/*lang=json,strict*/`

## Format de rapport

Pour chaque violation : `fichier:ligne — règle violée — correction attendue`
