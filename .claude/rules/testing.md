# Tests — Conventions

## Frameworks

| Package | Version | Rôle |
|---------|---------|------|
| `xunit` | 2.9.x | Framework de test |
| `xunit.runner.visualstudio` | 2.8.x | Runner Visual Studio |
| `Microsoft.NET.Test.Sdk` | 17.x | Infrastructure |
| `RichardSzalay.MockHttp` | 7.x | Mock HTTP |
| `Moq` | 4.x | Mock général |

Pas de FluentAssertions — assertions `Assert.*` xUnit uniquement.

---

## Nommage : `Methode_Scenario_ResultatAttendu`

```
GetClientsAsync_RetourneClients_QuandJsonEstValide
GetClientsAsync_RetourneListeVide_QuandReponseEst404
SendAsync_LanceOpenAIClientException_QuandBulkheadSature
CalculateTotal_RetourneZero_QuandListeEstVide
```

En français, sans abréviations.

---

## Pattern AAA — commentaires explicites obligatoires

```csharp
[Fact]
public async Task GetClientsAsync_RetourneListeVide_QuandReponseEst404()
{
    // Arrange
    MockHttpMessageHandler mockHttp = new();
    mockHttp.When("http://localhost/api/clients").Respond(HttpStatusCode.NotFound);
    HttpClient http = new(mockHttp) { BaseAddress = new Uri("http://localhost/") };
    ClientService service = new(http);

    // Act
    IEnumerable<Client> result = await service.GetClientsAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result);
}
```

---

## Données de test

- Données JSON dans une classe `JsonFixtures.cs` avec `/*lang=json,strict*/` obligatoire
- Jamais de strings JSON hardcodées directement dans les méthodes de test
- Données sensibles (tokens, clés API) dans `appsettings.json` de test — jamais dans le code

```csharp
// JsonFixtures.cs
public static class JsonFixtures
{
    public static readonly string Clients = /*lang=json,strict*/ """
    [
        { "id": 1, "nom": "Dupont" }
    ]
    """;
}
```

---

## Mocks

- `MockHttp` pour tous les appels HTTP — jamais de vraies requêtes réseau en test unitaire
- `Moq` pour les interfaces Repository et services internes
- Pas de mock sur les classes concrètes — si tu dois mocker une classe concrète, extraire une interface
- Vérifier les appels avec `mockHttp.VerifyNoOutstandingExpectation()` en fin de test si pertinent

---

## Couverture minimale par service

| Cas | Description |
|-----|-------------|
| Nominal | Données valides → résultat attendu non vide |
| Erreur HTTP | 404 ou 500 → collection vide ou exception métier, sans crash |
| Mapping | Au moins une propriété vérifiée avec `Assert.Equal` |
| Tri / Ordre | Vérifier l'ordre si le service trie les résultats |
| Concurrence | Tester la saturation si BulkHead ou SemaphoreSlim présent |
| Validation | Arguments null → `ArgumentNullException` si applicable |

---

## Seuil de couverture

Couverture minimale cible : **80% par service**.
Vérifier avec : `dotnet test --collect:"XPlat Code Coverage"`

---

## Hors périmètre tests unitaires

- Controllers (tester via tests d'intégration avec `WebApplicationFactory`)
- Middlewares (tester via `WebApplicationFactory` ou pipeline ASP.NET Core)
- Entités Domain POCO sans logique

---

## Tests d'intégration (si présents)

- Utiliser `WebApplicationFactory<Program>` pour les tests de middleware et API
- Base de données : SQLite in-memory ou base de test dédiée — jamais la base de production
- Nommage : même convention `Methode_Scenario_ResultatAttendu`

---

## Exemple BulkHead (pattern présent dans les projets)

```csharp
[Fact]
public void SendAsync_LanceOpenAIClientException_QuandBulkheadSature()
{
    // Arrange
    BulkHeadDecorator client = new(new TestClient(new HttpClient()));
    int failedCount = 0;

    // Act
    Parallel.For(0, 15, i =>
    {
        try
        {
            Task<string> result = client.SendAsync<string>(new HttpRequestMessage(), default);
            _ = result.Result;
        }
        catch (Exception ex) when (ex.InnerException is OpenAIClientException)
        {
            failedCount++;
        }
    });

    // Assert
    Assert.NotEqual(0, failedCount);
}
```
