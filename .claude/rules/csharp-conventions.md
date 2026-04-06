# Conventions C# — Source de vérité : `src/.editorconfig`

---

## Nommage

| Élément | Convention | Exemple |
|---------|-----------|---------|
| Namespaces | PascalCase block-scoped | `namespace Irbis.Tools.Data { }` |
| Classes / Interfaces / Structs / Enums | PascalCase | `ClientService`, `IRepository` |
| Interfaces | Préfixées par `I` | `IRepository`, `IAIClient` |
| Méthodes | PascalCase | `GetClients()`, `CalculateAmount()` |
| Méthodes async | Suffixe `Async` | `GetClientsAsync()`, `SendAsync()` |
| Propriétés publiques | PascalCase | `CustomerId`, `CreatedDate` |
| Champs privés | `_camelCase` | `_repository`, `_logger`, `_client` |
| Champs publics | PascalCase | `TotalCount`, `IsEnabled` |
| Variables locales | camelCase | `totalCount`, `isActive` |
| Constantes | PascalCase | `DefaultTimeout`, `MaxRetryCount` |
| Paramètres | camelCase | `userId`, `startDate`, `cancellationToken` |
| Fichiers | Identique à la classe principale | `ClientService.cs` |
| Méthodes booléennes | Commencent par `Is`, `Has`, `Can`, `Should` | `IsValid()`, `HasAccess()` |
| Tests unitaires | `Methode_Scenario_ResultatAttendu` | `GetClientsAsync_RetourneListeVide_QuandReponseEst404` |

---

## Règles absolues

- **Pas de `var`** — types explicites partout (`IDE0007 = none`)
- **Modificateurs d'accès** — toujours explicites sur toutes les déclarations
- **Accolades** — obligatoires sur tous les blocs sans exception
- **Namespaces** — block-scoped uniquement (`namespace Foo { }`, jamais `namespace Foo;`)
- **Primary constructors** — non utilisés (`IDE0290 = none`)
- **Records** — non utilisés, tous les modèles sont des `class { get; set; }`
- **Pas de `.Result` / `.Wait()`** — risque de deadlock, utiliser `await` systématiquement
- **Pas d'abréviations non standards** — `customer` pas `cust`, `repository` pas `repo`
- **Une classe = un fichier** (sauf classes internes fortement liées)
- **Pas de `public` inutiles** — respecter le principe du moindre accès
- **Valider les arguments** des méthodes publiques — `throw new ArgumentNullException(nameof(param))`
- **Éviter les magic numbers** — utiliser des constantes nommées

---

## Paramètres .csproj obligatoires

Tout `.csproj` doit déclarer explicitement ces deux paramètres — ne jamais laisser les valeurs par défaut du template Visual Studio :

```xml
<PropertyGroup>
  <Nullable>disable</Nullable>
  <ImplicitUsings>disable</ImplicitUsings>
</PropertyGroup>
```

Tous les `using` doivent être déclarés explicitement en tête de fichier. Aucun using implicite.

---

## Structure de démarrage — Program.cs / Startup.cs

Toujours utiliser le pattern classique. Jamais de Minimal API inline dans `Program.cs`.

**Program.cs** — minimal, délègue à Startup :
```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    })
    .Build()
    .Run();
```

**Startup.cs** — contient toute la configuration :
```csharp
public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // enregistrement des services
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // pipeline HTTP
    }
}
```

---

## Nullable

`Nullable` est **désactivé** sur tous les projets. Ne jamais annoter les types avec `?` sauf pour les types valeur nullables explicites (`int?`, `DateTime?`). Ne jamais initialiser les strings avec `= ""` pour satisfaire le compilateur nullable.

---

## Collections vides (C# 12)

Toujours `[]` — jamais `new List<T>()` ni `Enumerable.Empty<T>()`.

---

## `using` et directives

- Ordre : System → Microsoft → tiers → projet (par namespace alphabétique)
- Pas de `using static` sauf cas explicitement discuté
- Supprimer les `using` inutilisés

---

## Classes et héritage

- Sceller (`sealed`) les classes qui ne sont pas conçues pour l'héritage
- Pas plus de 2 niveaux d'héritage
- Préférer la composition à l'héritage

---

## Structs et enums

- `struct` : réservé aux value objects immutables et légers
- `enum` : toujours typer explicitement si valeur persiste en base (`int` par défaut)

---

## Gestion des exceptions

- Catcher uniquement les exceptions attendues — jamais `catch (Exception)` sans log
- Ne jamais swallower une exception silencieusement
- Pattern catch spécifique : `catch (HttpRequestException)`, `catch (BulkheadRejectedException)`

---

## Formatage

- 4 espaces · CRLF · UTF-8
- Pas de nouvelle ligne finale
- Expression-bodied : acceptable pour propriétés simples, bloc préféré pour méthodes

---

## Exemple conforme

```csharp
namespace Irbis.Tools.Data.Services
{
    public sealed class ClientService : IClientService
    {
        private readonly ILogger<ClientService> _logger;
        private readonly IClientRepository _repository;

        public ClientService(IClientRepository repository, ILogger<ClientService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Client>> GetClientsAsync()
        {
            _logger.LogInformation("Fetching clients");
            return await _repository.GetAllAsync();
        }
    }
}
```
