# Audit Status Report - 2025-10-24 (Detailed Analysis)

## Executive Summary

This report provides a comprehensive snapshot of the ISWC audit progress, highlighting completed milestones, pending access requirements, key findings from multiple workshops, and planned investigation areas. The audit has revealed significant governance, technical, and relationship challenges with Spanish Point.

---

## 1. Completed Milestones

- ✅ Design documentation review
- ✅ API subscription setup
- ✅ Azure Portal access (read-only)
- ✅ Teams documentation channel access
- ✅ Architecture diagram walkthrough (Workshop 2)
- ✅ Contract review confirming code access rights

---

## 2. Pending Access Requirements

### 2.1 From Spanish Point

- [ ] **Agency portal access** - Required for use case understanding
- [ ] **Source code access** - Critical for technical audit (BLOCKED - approval process ongoing)
- [x] **Database access** - SQL credentials for Dev/UAT environments (in progress)
- [ ] **CI/CD access** - To review deployment and testing processes
- [ ] **API Management subscriptions** - For all four API types (societies, publishers, labels, third parties)
  - [ ] Bastien+Guillaume : verify accesses

### 2.2 From CISAC

- [ ] **Hyperscale evolution proposal** - Architecture upgrade documentation
- [ ] **Agency portal access** - Required for use case understanding --> Yann to provide access in UAT

---

## 3. Key Findings & Observations

### 3.1 Code Access Challenges ⚠️ 🔴

**Issue:** Major difficulties obtaining source code access

**Timeline:**

- Workshop 1 (Oct 20): Initial refusal citing legal concerns
- Post-workshop: Yann confirmed contractual rights to access code
- Workshop 2 (Oct 21): Internal approval process, "early next week" estimate
- **Impact:** 25% of audit duration lost waiting for access

**Evidence from meetings:**

> **Xiyuan Zeng** in [Workshop 1 - Spanish Point Audit Relaunch](../meetings/20251020-SpanishPoint-AuditRelaunch.md): "For Cisac component... that one probably more likely to be allowed. For the matching part is probably not going to be allowed."
>
> **Yann LEBREUILLY** at 00:03:16 in [Workshop 2 - Documentations and Infrastructure](../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt): "I found that it's in the terms of a contract to have access to both the code for especially the ISWC Cisac code and the matching engine."

**Three-phase explanation evolution:**

1. Legal/NDA concerns
2. Technical difficulties opening access
3. Proprietary Matching Engine code separation

### 3.2 Vendor Relationship Issues ⚠️ 🔴

**Yann's Strategic Vision:**

> **Yann LEBREUILLY** at 27:54 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Ma vision moyen terme, c'est de me passer de eux, en fait, tout simplement. C'est ça la réalité aujourd'hui, parce que je ne peux pas gérer un SI sur lequel je n'ai pas la main, et actuellement je n'ai pas la main du tout."
>
> Translation: "My medium-term vision is to get rid of them, quite simply. That's the reality today, because I cannot manage an IS that I don't control, and currently I have no control at all."

**Initial Meeting Atmosphere:**

> **Guillaume Jay** in [Meeting Notes - October 20](../meetings/2025-10-20-NotesGuillaume.txt): "Ambiance froide" (Cold atmosphere)
>
> **Guillaume Jay** at 08:14 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Moi, je suis moins indulgent que Bastien, effectivement. j'ai pris une telle baffe hier, une telle froideur, j'avais fait un audit en début d'année... eux aussi ils ont plein de trucs hyper propriétaires, mais les gens ils étaient enthousiastes, ils participaient au truc, il y avait un partenariat."

**Yann's Assessment:**

> **Yann LEBREUILLY** at 03:20 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Pour moi, il y a beaucoup de choses qui ne vont pas sur le management de ce produit... la gouvernance est bancale... Donc, pour moi, c'est un marasme, ce truc-là."

**Defensive Attitude Observations:**

> **Bastien Gallay** at 05:57 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Oui, alors ils sont beaucoup sur la défensive... le moment d'incompréhension où je t'ai rappelé, il était énorme, mais ils étaient deux, ils écoutaient, on voyait qu'ils écoutaient. ils ne venaient pas à notre aide."

### 3.3 Technical Complexity & Admissions ⚠️

**Xiyuan's Complexity Warning:**

> **Xiyuan Zeng** at 1:32:02 in [Workshop 2 - Documentations and Infrastructure](../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt): "You're going to be surprised for the code because code is not just one one project... it's actually very complicated... You won't find it easy to read the source code you won't find that guarantee it has its own life cycles and with I think many developers touched it and modified it."

**Development Environment:**

- No local dev environment feasible within audit timeframe
- Cloud-based access only approach recommended
- Multiple developers modified code over time
- No clear onboarding process

### 3.4 Documentation Quality 📚

#### Positive Aspects ✅

- **Volume:** Extensive documentation available in Teams channel
- **Quality:** Generally good quality (apparent from initial review)
- **Core Design docs:** Dedicated list of essential documents

**Xiyuan's Confirmation:**

> **Xiyuan Zeng** at 51:58 in [Workshop 2 - Documentations and Infrastructure](../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt): "Any feature... those were the specification or whatever, those were the documentation at that point in time, and then the system would then be implemented."

#### Areas of Concern ⚠️

- **Outdated:** Most documents last modified during original implementation (some specs from 2019)
- **No digest/summary:** No consolidated overview or quick-start guide
- **Disorganized:** Documentation scattered across many folders

**Yann's Criticism:**

> **Yann LEBREUILLY** at 03:20 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "La documentation n'est pas disponible quand on le veut, quand on le demande. Il faut que nous repassions derrière le fournisseur pour pouvoir classer notre documentation parce que c'est confus tout ça."

### 3.5 Governance & Process Failures ⚠️ 🔴

#### Major Production Incident (May-June 2024)

> **Yann LEBREUILLY** at 41:40 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "On a mis six mois à fixer tous les problèmes qu'il y a eu à ce moment-là. Il y a eu un merge qui a eu lieu avec du code qui était en POC. Un POC sur un autre projet, ça a été mergé avec la branche principale et c'est sorti en prod. Et ça nous a valu pratiquement six mois de Gallay, d'incidents incessants."

**Impact:** 6 months of continuous incidents from POC code accidentally merged to production

#### CAB Implementation

> **Yann LEBREUILLY** at 11:00 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "J'ai mis en place un CAB... parce qu'il n'y avait rien, il faisait tout quand il voulait sans même prévenir les déploiements."

**Before CAB (pre-May 2024):**

- Deployments without notification
- No change control
- No deployment history

**After CAB (since May 2024):**

- Controlled deployment process
- Deployment history tracking
- Expert group review

#### Definition of Done (DoD) Failures

> **Yann LEBREUILLY** at 03:20 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Donc pour moi, un DOD digne de ce nom doit embarquer la mise à jour des et des documentations, qu'elles soient techniques ou fonctionnelles."

### 3.6 Technical Architecture Concerns ⚠️

#### Matching Engine Coupling

**Suspected tight coupling between Matching Engine and ISWC:**

> **Bastien Gallay** at 06:41 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Ce qui est plus embêtant, c'est que dans ce cas-là, ça veut dire qu'ils ont un couplage fort."
>
> **Yann LEBREUILLY** at 06:50 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Extrêmement fort. Le cœur de notre produit, c'est leur outil. C'est ça la difficulté."

**Architectural Principle Violation:**

> **Bastien Gallay** at 07:18 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Pour moi, c'est en fait, s'il y a une partie du code auquel on ne peut pas avoir accès, c'est ce que je veux dire, c'est que ça devrait être physiquement séparé."

#### System Architecture (from WS2)

**External Dependencies:**

- FastTrack SSO (authentication)
- Swiss API
- External SFTP service

**Core Components:**

- Agency Portal & Public Portal
- API Management (reverse proxy)
- Background Jobs (ISWC Jobs)
- Data Factory + Databricks
- SQL Server + Cosmos DB
- Storage Account + SFTP

**Matching Engine:** Treated as separate product but deployed alongside, creating coupling concerns

#### Technical Debt

**Outdated Technologies:**

> **Guillaume Jay** at 18:44 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): ".NET 3.1, qui est plus supporté depuis 2022."
>
> **Yann LEBREUILLY** at 18:44 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Et donc là, il est mis à jour ces jours-ci... Il n'hésite pas à dire, bon, on va vous facturer la mise à jour du .NET. Non, mais franchement, si la maintenance ne prend pas ça en compte, je ne sais plus quoi dire."

**Databricks:**

> **Yann LEBREUILLY** at 19:30 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Databricks n'est plus à jour de plusieurs versions... vous ne pouvez pas bénéficier, par exemple, de l'IA dans vos requêtes."

### 3.7 Cost Insights & Concerns 💰

#### Excessive Infrastructure Costs

**Monthly Cloud Costs:**

> **Yann LEBREUILLY** at 22:23 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Les coûts sont énormes. J'en ai pour 50K chaque mois de cloud, en gros, que ce soit de la transaction ou de l'hébergement de données."

**Environment Scaling Costs:**

- **New pre-prod environment:** €25k + (originally quoted even higher)
- **UAT database size increase:** €20k + 20 person-days

> **Yann LEBREUILLY** at 23:10 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Là, j'ai fait, là, il y a un vrai problème. On ne peut pas me demander pour une extension de taille d'environnement, 20 jours de travail, ça n'a pas de sens. Ça n'a aucun sens. Normalement, ça se fait en deux heures ou une demi-journée."

**Architecture Upgrade Proposal:**

> **Guillaume Jay** at 19:44 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Parce que là, tu parles de changement d'architecture, mais ce que je me souviens du changement d'architecture, c'était surtout de la montée en puissance."
>
> **Guillaume Jay** at 19:46 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "C'était aller payer plus cher le cloud. Ce n'est pas un vrai changement d'architecture."

#### Business Model Concerns

**CISAC as Major Client:**

> **Yann LEBREUILLY** at 16:34 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Oui, on est un gros client sur l'appli de base... Mais depuis les petites évolutions, ne rapportent rien, et c'est de la maintenance. Et de ce fait, je suis convaincu qu'ils perdent de l'argent à chaque fois qu'ils nous parlent."

**Evolution Suspicion:**

> **Bastien Gallay** at 37:14 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "J'ai peur en plus là, s'il y a une vraie intrication, que même ce qu'il vous propose comme évolution, ce soit une évolution qui profite à tous ceux qui utilisent le matching engine."

### 3.8 CISAC's Strategic Position 🎯

#### Market Importance

**CISAC as Data Source:**

> **Guillaume Jay** at 34:17 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Donc, leur business model est basé sur vos données. Il n'existe pas sans vos données."
>
> **Yann LEBREUILLY** at 34:18 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "C'est la source autoritative unique et ISO d'ailleurs. C'est nous qui avons ce référentiel unique."

**Homepage Prominence:**

> **Bastien Gallay** at 32:56 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Le truc qu'ils mettent le plus en avant, c'est leur Matching Module... En fait, quand on va sur Matching Module, on voit ça."

**Volume Leadership:**

> **Guillaume Jay** at 31:42 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Vous êtes dans les plus gros clients en termes de volume, j'imagine, non ?"
>
> **Yann LEBREUILLY** at 31:48 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Oui, oui, bien sûr."

#### Other Clients' Feedback

> **Yann LEBREUILLY** at 22:00 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "En fait, c'est plutôt moi qui leur fais peur. C'est-à-dire qu'ils s'embarquent avec eux souvent en se disant c'est le top du top, c'est la Rolls. Je leur dis peut-être dans les mots, oui, mais moi je ne suis pas content jusqu'à présent."

### 3.9 Team & Communication Patterns

#### Spanish Point Team

**Key Contacts:**

- **Curnan Reidy:** Lead tech, monotone communication style
- **Nicholas Randles:** Appeared ~1 year ago
- **Xiyuan Zeng:** Infrastructure architect
- **Mark Stadler:** Original system developer (appeared in WS2)
- **John Corley:** CEO, also Product Owner, creator of Matching Engine

**Communication Style:**

> **Yann LEBREUILLY** at 40:21 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Curnan lui, il est toujours sur un ton monotone. Au début, je pensais qu'il se foutait de moi, mais en fait, il est comme ça avec tout le monde."
>
> **Yann LEBREUILLY** at 40:21 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Et puis, vous avez son collègue. On a toujours l'impression qu'il se fout de vous quand il répond quelque chose... 'attendez, c'est évident ça, c'est évident ça, bah oui, puis ça c'est évident, bah non, on ferait pas comme ça, c'est évident.'"

#### Transparency Issues

> **Yann LEBREUILLY** at 26:48 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "C'est-à-dire qu'il y a un vrai problème de transparence. Je ne peux pas travailler avec des gens en transparence... je n'ai jamais eu de discussion comme j'ai avec vous là, avec l'équipe."

### 3.10 Collaboration & Communication Evolution ⚠️

#### Workshop 1 (October 20) - Negative Experience

**Noota Summary:**

- Cold atmosphere ("ambiance froide")
- Defensive attitude from Spanish Point
- Systematic challenges to access requests
- Three-phase explanation evolution for code access refusal

**Yann's Authorization Required:**

> **Yann LEBREUILLY** in [Workshop 1 - Spanish Point Audit Relaunch](../meetings/20251020-SpanishPoint-AuditRelaunch.md): "Just to confirm, we have NDA signed with these guys concerning everything, not the matching engine... for all the other parts, yes, please give them access."

#### Workshop 2 (October 21) - Slight Improvement

**More Cooperative:**

- Mark Stadler (original developer) provided valuable context
- Architecture walkthrough conducted
- Access provisioning in progress

**But Still Blocking:**

> **Xiyuan Zeng** at 04:40 in [Workshop 2 - Documentations and Infrastructure](../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt): "In terms of access right so... the repo or source code access so regardless... it won't be done today and it has to wait because there's a procedure need to go through internally."

**Bastien's Alternative Request Denied:**

> **Bastien Gallay** at 05:41 in [Workshop 2 - Documentations and Infrastructure](../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt): "Could we have an extract of the code? Maybe a zip put in the SharePoint would be enough to start with."
>
> **Xiyuan Zeng** at 05:53 in [Workshop 2 - Documentations and Infrastructure](../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt): "Not really. No, it's not whether I share with you, it's some procedure configuration you to go through."

### 3.11 Development & Evolution Patterns

#### Limited Recent Evolution

> **Yann LEBREUILLY** at 15:10 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Alors sur les 5 ans... Moi je vois que des petites évolutions... c'est par exemple rajouter des éléments de verbosité à un log... ou alors au lieu d'ingérer l'ensemble d'une liste d'oeuvres, le faire par batch."

#### Most Active Components (from WS2)

**Primary Development Areas:**

1. **APIs** - Most common change point
2. **Databricks** - File processing for new file types

> **Mark Stadler** at 1:18:44 in [Workshop 2 - Documentations and Infrastructure](../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt): "The API are the kind of Databricks. So if you're writing like a new file, like if you dropped a new file into SFTP... there would be a lot of work in processing that file in Databricks. So the API and Databricks would be probably the two biggest, two most involved things."

### 3.12 Onboarding & Knowledge Management ⚠️

**No Defined Process:**

> **Bastien Gallay** at 55:28 in [Workshop 2 - Documentations and Infrastructure](../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt): "What can we do to understand from this documentation? what is really important and how we can understand the business, the context and everything."
>
> **Xiyuan Zeng** at 53:39 in [Workshop 2 - Documentations and Infrastructure](../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt): "That's not practical... There's no shortcut, guys."

**Recommended Starting Points (from WS2):**

1. Agency Portal (visual interface)
2. API Management Developer Portal
3. Validation rules documentation

**Gaps Identified:**

- No documentation digest or quick-start guide
- No "how to contribute" guide
- No systematic onboarding for new developers

---

## 4. Investigation Roadmap

### 4.1 Immediate Priorities (No Source Code Required)

#### Organizational & Process

- [ ] **Maintenance organization and scope** (Contact: Yann + Moaiz)
  - Review CAB deployment history file
  - Analyze maintenance contract scope
- [ ] **Team composition and resource allocation** (Contact: Curnan)
  - How SP employee schedules are assigned to CISAC project
  - Team turnover analysis
- [ ] **Jira organization review**
  - Request access/demo from Spanish Point
  - Review Yann's Excel tracking file
  - Assess workflow and status coherence
- [ ] **May-June 2024 POC merge incident investigation**
  - Root cause analysis
  - Process improvements implemented
  - Current safeguards

#### Self-Study Topics

- [ ] **Azure Hyperscale** - Understand proposed architecture upgrade vs cost implications
- [ ] **Azure Databricks** - Data processing pipeline review and version currency
- [ ] **Azure Cosmos DB** - NoSQL implementation patterns and backup policies
- [ ] **Infrastructure as Code** - Review Azure ARM templates

#### Strategic Analysis

- [ ] **Interview with John Corley** (CEO/PO/Matching Engine creator)
  - Understand vision and strategy
  - Matching Engine evolution roadmap
  - CISAC-specific vs general enhancements
- [ ] **Cost-benefit analysis**
  - €50k/month cloud costs justification
  - Infrastructure scaling cost validation
  - Comparison with industry standards

### 4.2 When Source Code Becomes Available

#### Testing & Quality

- [ ] **Unit test investigation** - Review 9x% CI success rate claims
- [ ] **Code structure and style** - Assess maintainability and patterns
- [ ] **Multi-developer impact** - Code consistency analysis
- [ ] **.NET migration review** - Verify 3.1 to latest upgrade quality

#### Architecture & Integration

- [ ] **Coupling analysis** - Evaluate Matching Engine separation
  - Physical code separation verification
  - API contract analysis
  - Dependency mapping
- [ ] **Matching Engine interface** - Integration points and contracts
- [ ] **SPOF identification** - Database updates without queues or similar safeguards
- [ ] **Component interaction** - Data flow and communication patterns

#### Operations & Deployment

- [ ] **Batch processes and workflows** - Data processing pipelines in Databricks
- [ ] **Validation mechanisms** - What gets validated and how
- [ ] **Infrastructure as Code (IaC)** - ARM templates and deployment automation
- [ ] **CI/CD pipeline analysis** - Build and deployment process review
- [ ] **Rollback capabilities** - Incident recovery procedures

#### Evolution & Maintenance

- [ ] **Change request history** - Last 12 months of modifications
- [ ] **Technical debt assessment** - Outdated dependencies and frameworks
- [ ] **Documentation currency** - Code vs documentation alignment
- [ ] **Refactoring needs** - Maintainability improvements

---

## 5. Critical Quotes & Evidence

### On Governance

> **Yann LEBREUILLY** at 03:20 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Pour moi, il y a beaucoup de choses qui ne vont pas sur le management de ce produit, donc voilà, je suis déjà au sens large. Déjà, la gouvernance est bancale."

### On Transparency

> **Yann LEBREUILLY** at 28:07 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "C'est que c'est habituel. C'est-à-dire, quand on demande quelque chose, c'est toujours dans quel but, etc."

### On Control

> **Yann LEBREUILLY** at 27:54 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Je ne peux pas gérer un SI sur lequel je n'ai pas la main, et actuellement je n'ai pas la main du tout... c'est une boîte noire totale."

### On Costs

> **Yann LEBREUILLY** at 23:10 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "On ne peut pas me demander pour une extension de taille d'environnement, 20 jours de travail, ça n'a pas de sens. Normalement, ça se fait en deux heures ou une demi-journée."

### On Maintenance

> **Yann LEBREUILLY** at 18:44 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Il n'hésite pas à dire, bon, on va vous facturer la mise à jour du .NET. Non, mais franchement, si la maintenance ne prend pas ça en compte, je ne sais plus quoi dire."

### On Experience

> **Guillaume Jay** at 08:14 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "J'ai pris une telle baffe hier, une telle froideur... eux aussi ils ont plein de trucs hyper propriétaires, mais les gens ils étaient enthousiastes, ils participaient au truc, il y avait un partenariat."

---

## 6. Risk Assessment

### Critical Risks 🔴

1. **Vendor Lock-in** - Extremely tight coupling with proprietary Matching Engine
2. **Loss of Audit Time** - 25% duration consumed by access delays
3. **Governance Failure** - Production incidents from lack of process
4. **Cost Escalation** - Disproportionate infrastructure and enhancement costs
5. **Technical Debt** - Unsupported frameworks (.NET 3.1, outdated Databricks)

### High Risks 🟠

1. **Knowledge Concentration** - Limited team, complex codebase, no onboarding
2. **Documentation Obsolescence** - Specs from 2019 still in use
3. **Relationship Deterioration** - Defensive posture hampering collaboration
4. **Partial Audit** - Cannot audit Matching Engine code

### Medium Risks 🟡

1. **Development Environment** - No local setup possible for troubleshooting
2. **Onboarding Time** - New developers face steep learning curve
3. **Cost Visibility** - Lack of transparency in cloud cost drivers

---

## 7. Recommendations for Audit Approach

### Adapted Methodology

Based on Workshop 2 discussion:

> **Bastien Gallay** at 56:00 in [Workshop 2 - Documentations and Infrastructure](../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt): "Je pense qu'on va plus être dans un mode où on leur pose des questions, on produit, et on leur montre nos productions, on leur demande de confirmer qu'on a bien compris. Donc d'être en vrai mode interactif."

**Approach:**

1. **Question-driven** rather than interactive workshops
2. **Batch questions** for efficient meetings
3. **Focus on most-changed components** (APIs, Databricks)
4. **Code browsing** rather than local execution
5. **Document validation** sessions

### Meeting with John Corley

To better understand strategic vision and Matching Engine:

> **Bastien Gallay** at 38:54 in [Internal Discussion - Yann/Guillaume/Bastien](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt): "Une approche aussi à mon avis qui peut être intéressante est ce que ce serait intéressant aussi carrément on rencontre le père de la machine jane moi je suis convaincu que oui vous allez comprendre aussi un état d'esprit derrière oui."

### Next Workshops Focus

1. **Organizational processes** (Maintenance, Jira, Team allocation)
2. **API deep-dive** (Most active component)
3. **Databricks workflows** (File processing)
4. **Cost analysis** (Cloud spending breakdown)
5. **Incident post-mortem** (May-June 2024 POC merge)

---

## 8. Timeline & Deliverables

### Key Dates

- **October 20, 2025:** Workshop 1 - Audit Kickoff
- **October 21, 2025:** Internal discussion + Workshop 2
- **November 12, 2025:** Review meeting (30 min)
- **November 14, 2025:** First restitution to CISAC piloting committee (80% complete)
- **November 21, 2025:** Final version delivery

### Current Status

**Days Elapsed:** 4 days (Oct 20-24)
**Days to First Restitution:** 21 days
**Access Delays:** ~4 days lost (19% of time)

### Escalation Needs

> **Yann LEBREUILLY** at 06:53 in [Workshop 2 - Documentations and Infrastructure](../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt): "I will have a look with John on how it's possible to be quicker than that."

---

## 9. Legend

- ✅ Positive finding
- ⚠️ Concern or risk
- 🔴 Critical issue
- 🟠 High priority
- 🟡 Medium priority
- 💰 Cost-related item
- 📚 Documentation-related
- 🎯 Strategic position
- [ ] Pending action item
- [*To confirm*] Hypothesis requiring verification

---

## 10. Next Steps

1. **Escalate source code access** with Spanish Point management (Yann → John Corley)
2. **Schedule focused workshops** based on 4.1 priorities
3. **Self-study** Azure services (Hyperscale, Databricks, Cosmos DB)
4. **Prepare code review plan** for when access is granted
5. **Request Jira access** and schedule demo
6. **Interview John Corley** to understand strategic vision
7. **Analyze CAB deployment history** to understand change patterns
8. **Review May-June 2024 incident** documentation and remediation

---

## 11. Document References

### Meeting Transcripts

- [Workshop 1 - Oct 20, 2025](../meetings/20251020-SpanishPoint-AuditRelaunch.md)
- [Discussion Yann/Guillaume/Bastien - Oct 21, 2025](../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt)
- [Workshop 2 - Documentation & Infrastructure - Oct 21, 2025](../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt)

### Noota Reports

- Workshop 1 Summary (PDF)
- Internal Discussion Summary (PDF)
- Workshop 2 Summary (PDF)

### Other Notes

- [Guillaume's Notes - Oct 20, 2025](../meetings/2025-10-20-NotesGuillaume.txt)

---

**Report Date:** October 24, 2025
**Status:** In Progress - Critical Access Blocked
**Next Update:** Post source code access OR November 12, 2025 (review meeting)
**Audit Health:** 🟠 **AT RISK** - Access delays and relationship challenges impacting timeline

---

## Appendix A: Team Contact Information

### Spanish Point

- **Curnan Reidy** - Development Team Lead
- **Nicholas Randles** - Development Team
- **Xiyuan Zeng** - Infrastructure Architect
- **Mark Stadler** - Original System Developer
- **John Corley** - CEO/Product Owner/Matching Engine Creator

### CISAC

- **Yann LEBREUILLY** - Head of Programs
- **Moaiz BEN DHAOU** - (Role TBD - Techical contact)
- **Sylvain PIAT** - (Stats documentation contributor)

### Audit Team (Teragone Factory)

- **Bastien Gallay** - Technical Auditor
- **Guillaume Jay** - Technical Auditor

---

## Appendix B: Architecture Components

### Azure Services Used

- App Services (APIs, Portals)
- Azure Functions (Background Jobs)
- SQL Server (Relational data)
- Cosmos DB (NoSQL - audit data, submission history)
- Data Factory (Orchestration)
- Databricks (File processing)
- Data Lake (Binary files)
- Storage Account (SFTP files)
- API Management (Public API gateway)
- Key Vault (Secrets)
- Application Insights (Monitoring)

### External Integrations

- FastTrack SSO (Authentication)
- Swiss API
- External SFTP (File delivery)
- Matching Engine (Proprietary - separate product)

### Data Sources (CISAC-owned)

- ISWC (International Standard Musical Work Code)
- IPI (Interested Parties Information)
- Cisnet/WID (Work database)
- CWR (Common Works Registration)
