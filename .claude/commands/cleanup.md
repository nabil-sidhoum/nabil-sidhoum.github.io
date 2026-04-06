Traite les anomalies ouvertes dans `anomalies.local.md` une par une.

## Processus

1. **Lire `anomalies.local.md`** et lister toutes les cases non cochées `- [ ]`
2. **Afficher la liste** des anomalies ouvertes avec leur priorité implicite (architecture > conventions > tests)
3. **Demander confirmation** : "Quelle anomalie traiter en premier ?" ou "Traiter toutes les anomalies de la section X ?"
4. **Pour chaque anomalie sélectionnée** :
   - Lire le fichier concerné
   - Vérifier que l'anomalie est toujours présente (elle a peut-être été corrigée manuellement)
   - Proposer le fix conforme à `.claude/rules/`
   - Attendre validation avant d'appliquer
   - Cocher la case `- [x]` dans `anomalies.local.md` après application
5. **Résumé final** : nombre d'anomalies traitées, nombre restantes

## Règle

Ne jamais corriger plusieurs anomalies en une seule opération sans validation intermédiaire.
