# Email: Lacunes du Package Source Code (pour Yann)

**Date:** 2025-10-28
**À:** Yann Lebreuilly (CISAC)
**De:** Bastien Gallay & Guillaume Jay (Teragone-Factory)
**Objet:** Audit ISWC - Revue du code source et impact sur les livrables

---

Bonjour Yann,

Merci d'avoir facilité l'accès au code source. Nous avons terminé la revue initiale et relevé quelques points intéressants. Comme discuté, le format et le contenu du partage impactent directement les objectifs prioritaires de l'audit.

## Éléments manquants

**Package reçu :** .NET Core 3.1, format zip, pas de Matching Engine ni de configurations CI/CD/env.

**Impact sur les objectifs :**

1. **Version production actuelle manquante** (.NET Core 3.1 dans le package au lieu de la verion 8.0 déployée prochainement)
   - *Est-ce la dernière version ?*

2. **Matching Engine absent** (code source + API specs)
   - *Analyse vendor lock-in et goulot d'étranglement : limitée au superficiel, manque de détails*

3. **Configurations CI/CD absentes** (pipelines, IaC templates)
   - *Étude de la chaîne de build : impossible*
   - *Validation estimations IaC Spanish Point : impossible*

4. **Configurations et documentation absentes** (env configs, README, ADR)
   - *Pas de possibilité de lancer les composants en l'état*
   - *Optimisation coûts et stratégie modernisation : basées sur hypothèses*

5. **Historique versioning absent** (git history, tags)
   - *Évaluation risque migration : basée sur snapshot unique*

## Options

**Option A :** Obtenir au moins les éléments 1, 2, 3 avant le 12 novembre → on garde les objectifs d'origine

**Option B :** Ajuster le scope → on retire l'étude des performances et l'analyse Matching Engine, on réoriente les jours ailleurs

Nous pourrons en discuter lors de notre point de jeudi. Nous sommes aussi disponibles via Teams si tu le souhaite.

Cordialement,

**Bastien Gallay & Guillaume Jay**
Teragone-Factory
Équipe Audit Système ISWC
