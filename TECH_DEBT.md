# Dette technique

> Fichier commité — visible par toute l'équipe.
> Alimenté lors des PR et revues de code.
> Traiter par priorité lors des sprints dédiés.

---

## Format

| Priorité | Fichier | Anomalie | Propriétaire | Statut |
|----------|---------|----------|--------------|--------|
| haute / moyenne / basse | `chemin/fichier:ligne` | Description courte | @github-handle | ouvert / en cours / fermé |

---

## Dette ouverte

| Priorité | Fichier | Anomalie | Propriétaire | Statut |
|----------|---------|----------|--------------|--------|
| moyenne | `src/BlazorPortfolio.Client/Layout/MainLayout.razor.cs` | Le JS interop (`scroll.js`) est invoqué directement dans `OnAfterRenderAsync` sans abstraction — difficile à tester et à faire évoluer | @nabil-sidhoum | ouvert |
| basse | `src/BlazorPortfolio.Client/Pages/*.razor` | Les pages gèrent le chargement des données directement dans `OnInitializedAsync` — logique à extraire dans un composant ou service dédié si le projet grandit | @nabil-sidhoum | ouvert |
| basse | `src/BlazorPortfolio.Client.Tests/` | Aucun test sur les Pages (Home, Experiences, Projects, Educations) — couverture limitée à la couche Service | @nabil-sidhoum | ouvert |
| basse | `.github/workflows/ci.yml` | Intégration Codecov configurée mais aucun seuil de couverture minimum défini (`fail_ci_if_error: false`) — la CI ne bloque pas sur un recul de couverture | @nabil-sidhoum | ouvert |

---

## Dette fermée

| Priorité | Fichier | Anomalie | Propriétaire | Statut |
|----------|---------|----------|--------------|--------|
| haute | `src/BlazorPortfolio.Client/` | Usage de `var` dans plusieurs fichiers — violation de la convention types explicites | @nabil-sidhoum | fermé |
| haute | `src/BlazorPortfolio.Client/` | Initialisations nullables (`= ""`) et `Enumerable.Empty<T>()` non conformes aux conventions | @nabil-sidhoum | fermé |
| moyenne | `src/BlazorPortfolio.Client/Components/ProjectCard.razor.css` | `border-left` présent sur ProjectCard alors que la convention visuelle réserve cet effet aux cards Experience et Education | @nabil-sidhoum | fermé |
