# Phase 04 — Sections Hero + About

## Objectif

Implémenter les deux premières sections visibles du portfolio : le Hero (nom, rôle, tagline, CTAs) et le About (positionnement, photo placeholder).

## Prérequis

- Phase 03 terminée (layout, nav, toggle, menu mobile fonctionnels)
- Fichier `Portfolio.html` de référence — sections HERO (lignes 910-957) et ABOUT (lignes 964-986)

## Composants à créer

### 4.1 — `Pages/Index.razor`

C'est la page principale — elle assemble toutes les sections :

```razor
@page "/"

<PathStrip />

<HeroSection />
<div class="divider" aria-hidden="true">//</div>

<AboutSection />
<div class="divider" aria-hidden="true">//</div>

@* Les sections suivantes seront ajoutées dans les phases 05 et 06 *@
```

Pas de CSS scopé sur `Index.razor` — c'est un assembleur, pas un composant visuel.

### 4.2 — `Components/Sections/HeroSection.razor`

Éléments dans l'ordre :
1. Comment eyebrow : `// Senior .NET Tech Lead — Paris, FR`
2. H1 : `# Nabil Sidhoum` (avec `::before { content: "# " }` en muted)
3. Role line : `role: Tech Lead .NET | AI Agent Engineering` (key en muted, values en accent)
4. Meta row : `11 ans d'XP · 5 apps .NET en prod · ● disponible juillet 2026`
5. Tagline : `"I build the systems behind the systems."` (guillemets en accent)
6. Subtitle comment : `// Senior .NET engineer who ships AI-powered tools in production.`
7. Curl décoratif : `$ curl -O https://nabil-sidhoum.dev/cv.pdf` (masqué sur mobile)
8. CTAs : bouton primary `⬇ Télécharger le CV` + outline `↗ GitHub` + outline `↗ LinkedIn`

**Corrections par rapport au Portfolio.html** :
- Retirer `10 packages NuGet privés` de la meta row — sauf si Nabil confirme le chiffre exact
- Remplacer `dispo Q1 2026` par `disponible juillet 2026` (déjà fait dans PathStrip, mais aussi dans la meta row)
- Ajouter LinkedIn en troisième CTA outline

Le `HeroSection.razor.css` contient tous les styles du hero. Reprendre les styles du `Portfolio.html` lignes 331-405. Points clés :
- Le H1 utilise `font-size: clamp(36px, 6vw, 56px)` pour le responsive
- Le curl est `display: none` sous 767px
- Les CTAs passent en `flex-direction: column` sous 767px

Les icônes SVG (download, GitHub, LinkedIn) sont inline dans le Razor — pas de bibliothèque d'icônes externe.

### 4.3 — `Components/Sections/AboutSection.razor`

Éléments :
1. H2 : `## about — qui je suis, ce que je livre`
2. Grid 2 colonnes : photo placeholder (130px rond) + texte
3. Paragraphe 1 : 11 ans, référent technique unique, 5 apps interconnectées
4. Paragraphe 2 : architecture modulaire, Clean Arch, CQRS, MediatR, domaine fintech
5. Paragraphe 3 : agents IA intégrés dans des workflows .NET réels, pas une démo
6. ~~Comment closing : `Audience: CTOs, tech leads...`~~ → **SUPPRIMÉ** (c'était une note de brief, pas du contenu destiné aux visiteurs)

**Correction** : le commentaire "Audience: CTOs, tech leads, fintech / wealthtech / insurtech. Pas de mission ESN." est retiré. Si Nabil veut garder un signal de positionnement sectoriel, le reformuler en contenu positif dans le texte principal, pas en commentaire code.

Le `AboutSection.razor.css` contient les styles du about. Reprendre les styles du `Portfolio.html` lignes 410-440. Points clés :
- Grid `130px 1fr` sur desktop, `1fr` sur mobile
- Photo réduite à 96px sur mobile
- Le placeholder photo a un background hachuré et un texte `[ photo ] headshot 140×140`

### 4.4 — Placer le CV

Copier le fichier CV PDF dans `wwwroot/assets/cv.pdf`.

Si le CV n'est pas encore prêt, créer un placeholder :
```bash
echo "CV placeholder" > wwwroot/assets/cv.pdf
```

Mettre à jour les liens `href="cv.pdf"` dans HeroSection et Nav vers `href="assets/cv.pdf"`.

## Validation

```bash
dotnet run
```

Vérifier :
- Le hero s'affiche avec tous les éléments dans le bon ordre
- La tagline est visible et les guillemets sont en accent bleu
- Le curl est visible sur desktop, masqué sur mobile
- Les CTAs sont côte à côte sur desktop, empilés sur mobile
- Le bouton CV déclenche un téléchargement (même si c'est un placeholder)
- Le about affiche la grid photo + texte
- La photo placeholder est un cercle avec hachures
- Sur mobile, la photo passe au-dessus du texte et réduit à 96px
- Le divider `//` sépare correctement hero et about
- Le commentaire "Audience: CTOs..." n'apparaît PAS

## Commit

```
feat: ajouter les sections Hero et About

- HeroSection avec tagline, meta row, curl décoratif et CTAs (CV + GitHub + LinkedIn)
- AboutSection avec grid photo/texte et positionnement Tech Lead
- correction "disponible juillet 2026" dans la meta row
- suppression du commentaire "Audience" (note de brief, pas du contenu public)
- ajout de LinkedIn dans les CTAs
```

## Checklist avant de passer à la phase 05

- [ ] Le hero affiche nom, rôle, tagline, meta row, CTAs
- [ ] "disponible juillet 2026" (pas "Q1 2026")
- [ ] LinkedIn est présent dans les CTAs
- [ ] Le curl est masqué sur mobile
- [ ] Le about affiche la grid photo + texte sans commentaire "Audience"
- [ ] Le responsive fonctionne (mobile : CTAs empilés, photo réduite, grid 1 colonne)
- [ ] Le build compile sans erreur
