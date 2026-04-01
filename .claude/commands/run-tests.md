Lance les tests unitaires et synthétise les résultats.

## Commande

```bash
dotnet test src/BlazorPortfolio.Client.Tests/BlazorPortfolio.Client.Tests.csproj --logger "console;verbosity=normal"
```

## Rapport en cas d'échec

Pour chaque test en erreur :
1. **Nom du test** et service concerné
2. **Message d'erreur** (extrait significatif, pas le stack trace complet)
3. **Piste de correction** — quelle règle de `.claude/rules/` est probablement violée ?

## Rappel CI/CD

Les tests bloquent le déploiement — tout échec doit être corrigé avant push sur `main`.
