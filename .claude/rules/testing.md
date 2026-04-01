# Tests — BlazorPortfolio

## Frameworks

| Package | Version | Rôle |
|---------|---------|------|
| `xunit` | 2.9.3 | Framework |
| `xunit.runner.visualstudio` | 2.8.2 | Runner |
| `Microsoft.NET.Test.Sdk` | 17.12.0 | Infrastructure |
| `RichardSzalay.MockHttp` | 7.0.0 | Mock HTTP |

Pas de FluentAssertions — assertions `Assert.*` uniquement.

---

## Structure

```
src/BlazorPortfolio.Client.Tests/
├── JsonFixtures.cs           # Constantes JSON (/*lang=json,strict*/ requis)
├── ExperienceServiceTests.cs # 4 tests
├── EducationServiceTests.cs  # 3 tests
└── ProjectServiceTests.cs    # 3 tests
```

---

## Nommage : `Methode_Scenario_ResultatAttendu` — en français

```
GetExperiencesAsync_RetourneExperiences_QuandJsonEstValide
GetExperiencesAsync_RetourneListeVide_QuandReponseEst404
GetExperiencesAsync_ExperiencesTrieesParDateDecroissante
```

---

## Pattern AAA — commentaires explicites obligatoires

```csharp
[Fact]
public async Task GetExperiencesAsync_RetourneListeVide_QuandReponseEst404()
{
    // Arrange
    MockHttpMessageHandler mockHttp = new();
    mockHttp.When("http://localhost/data/experiences.json").Respond(HttpStatusCode.NotFound);
    HttpClient http = new(mockHttp) { BaseAddress = new Uri("http://localhost/") };
    ExperienceService service = new(http);

    // Act
    IEnumerable<Experience> result = await service.GetExperiencesAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result);
}
```

---

## Couverture minimale par service

| Cas | Description |
|-----|-------------|
| Nominal | JSON valide → collection non vide |
| Erreur HTTP | 404 → collection vide, sans exception |
| Mapping | Au moins une propriété vérifiée avec `Assert.Equal` |
| Tri (ExperienceService) | Ordre décroissant par `DateDebut` |

---

## Hors périmètre

Composants Razor, Pages, Models (POCO sans logique), JS interop. Pour composants : **bunit** (à ajouter si besoin).
