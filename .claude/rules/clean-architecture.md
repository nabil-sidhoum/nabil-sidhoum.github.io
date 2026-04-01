# Architecture — BlazorPortfolio

## Flux de données

```
Pages → (inject) → Services → HttpClient → wwwroot/data/*.json
                                   ↑
                                Models (POCO)
Components ← [Parameter] ← Pages
```

---

## Responsabilités

| Couche | Fichiers | Règle clé |
|--------|----------|-----------|
| **Models** | `Models/*.cs` | POCO purs — aucune logique, aucune dépendance |
| **Services** | `Services/*.cs` | Retourne `IEnumerable<T>`, gère `HttpRequestException` → `[]` |
| **Pages** | `Pages/*.razor` | `@inject` + `OnInitializedAsync`, `@code {}` uniquement |
| **Components** | `Components/*.razor` | `[Parameter]` uniquement — pas d'injection de service |
| **Layout** | `Layout/` | `MainLayout.razor.cs` : seul code-behind autorisé (JS interop) |

---

## Pattern service (référence)

```csharp
public class ExperienceService
{
    private readonly HttpClient _client;

    public ExperienceService(HttpClient client) => _client = client;

    public async Task<IEnumerable<Experience>> GetExperiencesAsync()
    {
        try
        {
            IEnumerable<Experience> result = await _client.GetFromJsonAsync<List<Experience>>("data/experiences.json");
            return result?.OrderByDescending(e => e.DateDebut) ?? [];
        }
        catch (HttpRequestException)
        {
            return [];
        }
    }
}
```

---

## Interdit

- Logique dans les Models (méthodes, propriétés calculées, annotations EF Core)
- Injection de service dans les Components
- State global (Fluxor, singleton avec état partagé)
- Framework CSS externe (Bootstrap embarqué localement)
- Patterns à ne pas introduire sans discussion : CQRS, MediatR, Repository, interfaces de service, EF Core
