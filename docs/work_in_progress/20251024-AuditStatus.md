# Audit Status report - 2025-10-24

## Achieved

- Review of design documentations
- API subscription

## Waiting for

### From Spanish Point

- [ ] access to agency portal
- [ ] access to source code
- [ ] access to database
- [ ] access to CI/CD

### From CISAC

- [ ] Proposition d'évolution avec Hyperscale

## Surprises

- Difficultés à ouvrir l'accès au code : process lourd ? recherche d'échappatoire ? (-)
- Aveux de Xiyuan d'une "complexité" et de difficultés à comprendre le code (-)
- Documentation
  - Beaucoup de documentation, souvent de qualité (apparemment) assez bonne (+)
  - Existence d'une liste spécifique de documents "Core Design" pour faciliter la compréhension (+)
  - La plupart des documents semblent avoir été modifiés pour la dernière fois lors de l'implémentation originale
- Collaboration
  - Attitude très fermée dès le premier rendez-vous: chaque question mène à de réponses floues et compliquée; les demandes d'accès sont systématiquement discutées (-)
  - Un peu d'amélioration par la suite, malgré le problème d'accès au code (+)
- Intégration de nouveaux membres (et onboarding des auditeurs)
  - Pas de processus défini (-)
  - Pas de "digest" des docs (-)
  - [*à confirmer* - hypothèse suite au workshop 2](../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt) Pas de "how to contribute" (-)
- Structuration technique
  - [*à confirmer* - hypothèse suite au WS 2](../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt) - [*à confirmer* - hypothèse suite au WS 1](../meetings/20251020-SpanishPoint-AuditRelaunch.md) Il semble y avoir un couplage fort entre Matching Engine et les appli ISWC, ou au moins des aspects confidentiels de ME présents dans le code ISWC (contrairement à ce que semble indiquer les spécifications) (-)
- Informations de Yann
  - Coûts des mises en places supplémentaires : 25k€ + 20 JH pour un nouvel environnement et 20k€ pour augmenter la taille de la base UAT

## To be explored

### Without need for source code

- [ ] Maintenance organisation and scope (Yann + Moaiz)
- [ ] Team composition and planning assignement (i.e. how the schedule of a SP employee is assigned to a work on CISAC project) processes (Curnan)
- [ ] Jira organization --> access/demo from SP + Doc de Yann

#### A étudier de notre côté

- [ ] Hyperscale
- [ ] Databricks
- [ ] CosmosDB

### When code is available

- [ ] Investigate Unit tests (9x% Success pass the CI)
- [ ] Different coupling
- [ ] Find SPOF : db updates without queues or similar blockers
- [ ] Code structure and style
- [ ] Batch and workflows
- [ ] Validations : what exactly is validated and how
- [ ] Matching Engine interface
- [ ] IaC
