# Conventions Blazor — Composants Razor

## Pattern code-behind obligatoire

Tout composant Blazor ayant du code C# (lifecycle, injection, état, méthodes) doit séparer markup et logique en trois fichiers :

| Fichier | Contenu |
|---------|---------|
| `ComponentName.razor` | Markup HTML/Razor uniquement — aucun `@code`, aucun `@inject`, aucun `@implements` |
| `ComponentName.razor.cs` | Classe `partial` avec toute la logique C# |
| `ComponentName.razor.css` | CSS scopé |

**Exception :** composants sans logique C# (markup pur) — pas de `.razor.cs` nécessaire.

---

## Fichier `.razor` — règles

- Pas de `@code { }` — déplacer dans `.razor.cs`
- Pas de `@inject` — remplacer par `[Inject]` dans `.razor.cs`
- Pas de `@implements` — déclarer sur la classe dans `.razor.cs`
- Les `@using` globaux vont dans `_Imports.razor`, pas dans les composants

---

## Fichier `.razor.cs` — règles

- Classe `partial`, même nom que le composant, même namespace
- Injections via `[Inject]` sur propriétés `private { get; set; }`
- Interfaces de cycle de vie déclarées sur la classe (`IAsyncDisposable`, etc.)
- Namespaces block-scoped, `using` explicites (ordre : System → Microsoft → projet)
- Méthodes `[JSInvokable]` déclarées `public` (requis par le runtime JS)
- `InvokeAsync` utilisé avec `_ =` dans les méthodes `void` pour fire-and-forget explicite

```csharp
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorPortfolio.Client.Components.Nav
{
    public partial class Nav : IAsyncDisposable
    {
        [Inject]
        private IJSRuntime JS { get; set; }

        private IJSObjectReference _module;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/mon-module.js");
            }
        }

        [JSInvokable]
        public void OnCallback(string value)
        {
            _ = InvokeAsync(StateHasChanged);
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
            {
                await _module.DisposeAsync();
            }
        }
    }
}
```

---

## Règles JS Interop

- Modules JS chargés via `import` ES module — jamais `IJSRuntime.InvokeVoidAsync` sur des globaux
- `DotNetObjectReference` disposé dans `DisposeAsync` après le module JS
- `IJSObjectReference` vérifié non null avant `DisposeAsync`

---

## Règles de lifecycle

- `OnInitialized` / `OnInitializedAsync` : abonnement aux events uniquement
- `OnAfterRenderAsync(bool firstRender)` : initialisation JS interop (guard `if (firstRender)`)
- `IAsyncDisposable` seul — pas de double `IDisposable` + `IAsyncDisposable` ; désabonner les events dans `DisposeAsync`

---

## Services injectés

- `UIStateService` : service Scoped, état partagé entre composants — injecter, ne pas instancier
- Pas d'accès direct à `HttpClient` dans les composants — passer par un service dédié
