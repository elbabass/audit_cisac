# Message Court: Lacunes du Package Source Code (pour Yann - Instant Messaging)

**Date:** 2025-10-28
**À:** Yann Lebreuilly (CISAC)
**De:** Bastien Gallay & Guillaume Jay (Teragone-Factory)

---

Salut Yann,

On a terminé la revue du code source fourni par Spanish Point. Quelques éléments manquent qui impactent directement les objectifs prioritaires de l'audit :

**Package reçu :** .NET Core 3.1, format zip, pas de Matching Engine ni de configurations CI/CD/env.

**Manquant :**

1. **Version production actuelle** - Est-ce que .NET Core 3.1 est bien la version en prod ?
2. **Matching Engine** - Pas de code source ni specs API
3. **Configurations CI/CD** - Pas de pipelines, IaC templates
4. **Configs env + documentation** - Pas de README, configs environnement
5. **Historique git** - Juste un zip, pas de versioning

**Impact :** analyse de performance, de Matchnig engine et évaluation DevOps limités sans ces éléments.

On peut en discuter jeudi lors de notre réunion hebdo, ou plus tôt si tu préfères (dispo cet après-midi ou demain après-midi).
