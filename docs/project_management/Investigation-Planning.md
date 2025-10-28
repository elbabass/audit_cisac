# ISWC System Audit - Investigation Planning

**Document Version:** 2.0
**Date:** October 27, 2025
**Status:** Planning Document - Updated with Workshop 1 findings and source code access

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [High-Level Audit Priorities](#high-level-audit-priorities)
3. [Audit Goals](#audit-goals-refined)
4. [Investigation Framework](#investigation-framework)
5. [Component Investigation Plan](#component-investigation-plan)
   - [1. Matching Engine](#1-matching-engine--critical--deep)
   - [2. CISAC API](#2-cisac-api-app-services--high--deep)
   - [3. Agency Portal & Public Portal](#3-agency-portal--public-portal--medium--medium)
   - [4. Data Layer](#4-data-layer-sql-server--cosmos-db--high--deep)
   - [5. Data Processing](#5-data-processing-databricks--data-factory--high--deep)
   - [6. SFTP File Exchange](#6-sftp-file-exchange--medium--medium)
   - [7. Infrastructure as Code & DevOps](#7-infrastructure-as-code--devops--critical--deep)
   - [8. Monitoring & Observability](#8-monitoring--observability--high--medium)
   - [9. Security & Compliance](#9-security--compliance--high--medium)
   - [10. Documentation & Knowledge Management](#10-documentation--knowledge-management--critical--medium)
   - [11. Proposed Architecture Upgrade](#11-proposed-architecture-upgrade--critical--deep)
6. [Investigation Methodology](#investigation-methodology)
7. [Success Criteria](#success-criteria)
8. [Risks and Constraints](#risks-and-constraints)
9. [Appendix: Component-to-Document Mapping](#appendix-component-to-document-mapping)

---

## High-Level Audit Priorities

This section provides a quick reference to the most critical audit tasks based on business impact and technical risk.

### Top 5 Critical Priorities (12 Man-Days Budget)

1. **Proposed Architecture Upgrade Validation** 🔴 **[4 days]**
   - ✅ Hyperscale proposal documented (€3,300/month savings claimed, €1,500 new costs)
   - Validate performance bottlenecks justify Hyperscale tier vs. query optimization
   - Review WAF and private networking requirements
   - **Business Impact:** €40K project decision + ongoing €1,800/month impact
   - **Reference:** [Performance Proposition PSA5499](../resources/performance_proposition/CISACAzureInfrastructureUpdate/CISACAzureInfrastructureUpdate/CISACAzureInfrastructureUpdate.md)
   - **Deliverable:** Go/No-Go recommendation with alternatives

2. **Infrastructure as Code & DevOps Maturity** 🔴 **[2 days]**
   - ✅ 343 Azure resources documented
   - Focus: 20-day/€20K environment extension estimate validation
   - IaC automation gaps assessment
   - Production incident root cause (May-June 2024)
   - **Business Impact:** Vendor switching cost, operational risk
   - **Deliverable:** IaC maturity assessment + automation roadmap

3. **Matching Engine Technical Coupling & Performance** 🔴 **[2 days]**
   - ✅ Integration patterns analyzed (clean REST API separation)
   - Focus: Synchronous blocking performance impact quantification
   - Alternative integration patterns (async, queue-based)
   - Technical feasibility of vendor replacement
   - **Business Impact:** Performance optimization, vendor independence strategy
   - **Deliverable:** Technical coupling assessment + async integration options

4. **Technical Debt & Upgrade Path** 🔴 **[2 days]**
   - ✅ .NET 3.1 EOL confirmed (Dec 2022)
   - ✅ Databricks 10.4 LTS outdated confirmed
   - Prioritize security vulnerabilities
   - Estimate upgrade effort and cost
   - **Business Impact:** Security risk, maintenance burden
   - **Deliverable:** Technical debt register + upgrade roadmap

5. **Cost Optimization Quick Wins** 🟠 **[2 days]**
   - Analyze €50K/month costs (€600K/year)
   - Reserved instance opportunities (Cosmos DB, Databricks)
   - Resource right-sizing (App Services, SQL DTUs)
   - Identify 10-20% reduction potential
   - **Business Impact:** Immediate savings €60-120K/year
   - **Deliverable:** Cost optimization plan with quick wins

### Realistic 3-Week Timeline (12 Man-Days)

**Week 1 (Nov 4-8): Foundation & Quick Analysis** [4 days]

- ✅ Architecture documentation completed (7 docs)
- ✅ Source code access obtained
- Hyperscale proposal deep-dive (2 days)
- Technical debt inventory (1 day)
- Cost analysis initiation (1 day)

**Week 2 (Nov 11-15): Focused Investigations** [4 days]

- IaC maturity assessment (1 day)
- Matching Engine performance analysis (1 day)
- Cost optimization analysis completion (1 day)
- Targeted code quality review of critical paths (1 day)

**Week 3 (Nov 18-22): Synthesis & Reporting** [4 days]

- Executive summary draft (1 day)
- Technical findings consolidation (1 day)
- Recommendations and roadmap (1 day)
- Final report and presentation prep (1 day)

**Target Delivery: November 21, 2025**

### Out of Scope (Initial Backlog)

The following items are **OUT OF SCOPE** for the initial 3-week assessment but may be re-scoped based on client priorities:

- ❌ **Contract & Legal Review** - Matching Engine licensing terms, exit clauses (not our expertise)
- ❌ **Comprehensive Code Quality Review** - Full codebase analysis (requires 5-10 days)
  - ✅ **Alternative:** Targeted review of critical/high-risk components only (included)
- ❌ **Performance Testing & Benchmarking** - Load testing, stress testing (requires 3-5 days + environment)
- ❌ **Security Penetration Testing** - Requires external security engagement
- ❌ **Complete API Documentation Review** - All API endpoints (requires 2-3 days)
- ❌ **Detailed Migration Planning** - Full vendor change implementation plan (requires 5-7 days)
  - ✅ **Alternative:** High-level migration feasibility assessment (included)

### Scope Flexibility

**This investigation plan serves as a backlog and will be continuously reviewed with the client.** Priorities may shift based on:

- Findings from early investigation phases
- Client feedback on draft deliverables (review scheduled Nov 12)
- Discovery of critical issues requiring immediate attention
- Time constraints and resource availability

**Approach:** Agile assessment with regular check-ins to adjust priorities and scope based on emerging needs.

---

## Executive Summary

This document outlines the investigation plan for the ISWC Application audit, identifying key components and focus areas based on audit goals, system architecture, and current challenges identified during initial workshops.

### Audit Goals (Refined)

**From Context Introduction:**
> "This RFP seeks a comprehensive audit and assessment of both the current ISWC application environment and the proposed upgrade, aiming to support a risk-mitigated, future-ready transformation. The initiative will strengthen data integrity, operational efficiency, and stakeholder trust across the global music ecosystem."

**From Key Success Factors:**

- Objective evaluation of current and proposed architectures
- Performance, scalability, security, and cost efficiency assessment
- Actionable migration steps and risk mitigation strategies
- Transparent knowledge sharing and documentation review

**From Yann's Discussion (Oct 21):**
> "Moi, j'ai vraiment besoin qu'on creuse le côté technique... je dois produire une vision moyen terme. Ma vision moyen terme, c'est de me passer de eux, en fait."

> "Ce qui me rassure c'est je n'étais pas parano... actuellement il y a des choses qui sont pas qui vont pas dans la bonne direction."

**Key Concerns Identified:**

1. **Opacity and transparency issues** - Documentation, decision-making, cost justification
2. **Technical debt** - .NET 3.1 (EOL 2022), outdated Databricks, missing automation
3. **Vendor lock-in** - Matching Engine coupling, code access restrictions
4. **Cost control** - €50K/month cloud costs, expensive change requests
5. **Governance gaps** - No deployment tracking (CAB only since 2024), incomplete DoD
6. **Production incident** - Major merge issue (May-June 2024) requiring 6 months to fix

---

## Investigation Framework

### Priority Levels

- **🔴 CRITICAL** - Direct impact on audit goals, system ownership, or migration decisions
- **🟠 HIGH** - Significant technical or business risk, cost optimization potential
- **🟡 MEDIUM** - Important for understanding, moderate risk/cost impact
- **🟢 LOW** - Nice to have, minimal immediate impact

### Investigation Depth

- **DEEP** - Full code review, architecture analysis, performance testing
- **MEDIUM** - Documentation review, configuration analysis, selective code review
- **SURFACE** - High-level overview, existing documentation synthesis

---

## Component Investigation Plan

### 1. Matching Engine 🔴 CRITICAL | DEEP

**Component Description:**

- Proprietary component by Spanish Point
- Core matching algorithm for work identification
- Sold to music societies worldwide
- ✅ **CONFIRMED:** Separate external service with REST API integration (not directly coupled in codebase)

**Infrastructure Location:**

- Deployed in same Azure subscription as ISWC
- Separate from ISWC application (external dependency)
- ✅ **Source code access:** Integration layer granted (42+ files), Matching Engine core remains proprietary

**Current Documentation Status:**

- ✅ [Architecture Documentation](architecture/MatchingEngine.md) - Complete overview
- ✅ [Integration Analysis](code_analysis/MatchingEngine_integration.md) - Deep dive into `MatchingEngineMatchingService.cs`
- ✅ Source code available: `docs/resources/source-code/ISWC/src/` (integration components)

**Focus Areas:**

#### 1.1 Coupling Analysis ✅ PARTIALLY COMPLETED 🟠 HIGH

**Why:** Understanding coupling level is essential for migration strategy and vendor independence.

> **Yann (Workshop 2, Line 96):** "Le cœur de notre produit, c'est leur outil. C'est ça la difficulté."

> **Bastien (Workshop 2, Line 96):** "S'il y a une partie du code auquel on ne peut pas avoir accès, c'est ce que je veux dire, c'est que ça devrait être physiquement séparé."

> **Workshop 1 (Lines 23:00-36:00):** Source code access discussion - CISAC components granted, Matching Engine core remains proprietary

**Investigation Progress:**

- ✅ API contract analysis completed - REST API with OAuth2 authentication
- ✅ Dependency mapping completed - HTTP integration via `IHttpClientFactory`, clean separation
- ✅ Assessed decoupling feasibility - Interface abstraction via `IMatchingEngineMatchingService`
- ⏳ **Pending:** Async integration pattern analysis for performance improvement
- **Finding:** Clean architectural separation via REST API, but synchronous blocking creates performance coupling

#### 1.2 Performance Characteristics 🟠 HIGH

**Why:** Performance issues may be masked by "architecture upgrade" proposals without addressing root causes.

> **Yann (Line 76):** "Il ne faut pas que ce soit une architecture qui vient cacher la misère d'un code mal foutu ou d'une application qui a la dérive."

**Investigation:**

- Query performance metrics (response times, throughput)
- Resource consumption patterns (CPU, memory, I/O)
- Scalability limits identification
- Compare against proposed Azure Hyperscale justification

#### 1.3 Configuration and Tuning 🟡 MEDIUM

**Why:** Proper configuration could improve performance without architectural changes.

**Investigation:**

- Current matching rules configuration review
- Search vs submission parameter differences
- Optimization opportunities without code changes
- **Reference:** SPE_20190424_MVPMatchingRules.md

#### 1.4 Licensing and Contractual Rights ❌ OUT OF SCOPE

**Why:** Legal/contractual review is outside our area of expertise.

> **Yann (Line 99):** "Si on n'a même pas le droit de regarder le code du moteur de matching, on fait un audit un tiers du produit en vrai."

**Recommendation:** CISAC should engage legal counsel to review:

- Contract terms for code access rights
- Modification and derivative work rights
- Exit strategy provisions and transition support
- Vendor lock-in clauses and pricing model

**Our Focus Instead:** Technical coupling analysis, performance impact, and alternative integration patterns

---

### 2. CISAC API (App Services) 🟠 HIGH | DEEP

**Component Description:**

- C# based REST API (.NET 3.1 → upgrade in progress)
- Exposed via Azure API Management
- Multiple API variants: Society, Publisher, Label, Third-Party
- Primary integration point for agencies

**Infrastructure Location:**

- Multiple Azure App Services (iswc-api, iswc-api-publisher, iswc-api-label, iswc-api-thirdparty)
- Behind Azure API Management as reverse proxy

**Current Documentation Status:**

- ✅ Source code available: `docs/resources/source-code/ISWC/src/Api.Agency/`, `Api.Label/`, etc.
- ✅ [Infrastructure Reference](infra/infrastructure-azure-reference.md) - 343 Azure resources documented
- ⏳ Detailed architecture documentation - In progress

**Focus Areas:**

#### 2.1 Technical Debt Assessment 🔴 CRITICAL

**Why:** .NET 3.1 EOL since 2022 creates security and support risks.

> **Yann (Line 232):** "Au mois de juin ou juillet... il n'était plus supporté déjà, le .NET... ils ont fait, OK, fair enough."

**Investigation:**

- Current .NET version status (3.1 vs upgrade to .NET 8/9)
- Deprecated API usage
- Security vulnerability scan results
- Breaking changes analysis for upgrade path

#### 2.2 Code Quality and Maintainability 🟠 HIGH

**Why:** Code quality impacts future evolution costs and vendor independence.

> **Bastien (Line 102):** "Dans ce cas-là, la solution va au moins nous faire un extrait du code qu'on peut regarder."

**Investigation:**

- SOLID principles adherence
- Dependency injection patterns
- Testing coverage (unit, integration, e2e)
- Code complexity metrics (cyclomatic complexity, coupling)
- **Goal:** Assess handover feasibility to another vendor

#### 2.3 API Design and Consistency 🟡 MEDIUM

**Why:** Good API design reduces agency integration costs and support burden.

**Investigation:**

- RESTful design principles compliance
- API versioning strategy
- Error handling consistency
- Documentation completeness (OpenAPI/Swagger)
- **Reference:** SPE_20191217_CISAC ISWC REST API.md

#### 2.4 Performance and Scalability 🟠 HIGH

**Why:** API is the programmatic interface for all agencies worldwide.

**Investigation:**

- Current throughput and latency metrics
- Rate limiting and throttling configuration
- Caching strategies
- Database query optimization opportunities
- Connection pooling and resource management

---

### 3. Agency Portal & Public Portal 🟡 MEDIUM | MEDIUM

**Component Description:**

- Web-based UIs for agency staff and public searches
- Responsive design (desktop, tablet, mobile)
- FastTrack SSO authentication
- Built on CISAC API

**Infrastructure Location:**

- Azure App Services (iswc-portal-agency, iswc-portal-public)

**Current Documentation Status:**

- ✅ [Agency Portal Architecture](architecture/ISWC-Agency-Portal.md) - Complete with 161+ source files documented
- ✅ Frontend: React 16.12.0 + TypeScript 3.7.3, Backend: ASP.NET Core 3.1
- ✅ Source code available: `docs/resources/source-code/ISWC/src/Portal.Agency/`, `Portal.Public/`

**Focus Areas:**

#### 3.1 User Experience and Accessibility 🟡 MEDIUM

**Why:** Portal is primary interface for agency work registration and search.

**Investigation:**

- Usability testing feedback implementation status
- Accessibility compliance (WCAG)
- Performance (page load times, client-side rendering)
- Mobile responsiveness validation
- **Reference:** SPE_20190806_ISWC_Portal.md (includes usability study results)

#### 3.2 Frontend Architecture 🟡 MEDIUM

**Why:** Modern frontend practices reduce maintenance and improve UX.

**Investigation:**

- Framework and library versions
- Bundle size and optimization
- Component reusability
- State management patterns

---

### 4. Data Layer (SQL Server + Cosmos DB) 🟠 HIGH | DEEP

**Component Description:**

- **SQL Server** - Work metadata, creators, submissions (schema/relational)
- **Cosmos DB** - Submission history, audit trails (NoSQL/JSON)
- IPI database replica for creator lookups

**Infrastructure Location:**

- Azure SQL Server (iswc-sql, ipi-sql replica)
- Azure Cosmos DB (submission history)

**Current Documentation Status:**

- ✅ [Cosmos DB Architecture](architecture/CosmosDB.md) - MongoDB API, partition strategy, 822M+ records
- ✅ [Audit Logging System](architecture/AuditLogging.md) - Three-tier collection hierarchy
- ✅ Source code: Repository pattern implementation, `ICosmosDbRepository<T>`
- ⏳ SQL Server schema analysis - In progress

**Focus Areas:**

#### 4.1 Database Schema and Design 🟠 HIGH

**Why:** Schema quality affects performance, data integrity, and evolution costs.

**Investigation:**

- Normalization level and rationale
- Indexing strategy (covered, composite, missing indexes)
- Foreign key constraints and referential integrity
- Schema evolution approach (migrations, versioning)
- **Reference:** SPE_20190218_ISWCDataModel_REV (PM).md

#### 4.2 Data Integrity and Audit Trail 🟠 HIGH

**Why:** ISWC is authoritative global registry - data corruption is unacceptable.

> **Context:** ISO standard for musical work codes

**Investigation:**

- Constraint enforcement (unique ISWCs, mandatory fields)
- Audit trail completeness (all CRUD operations logged)
- Cosmos DB usage vs SQL Server (why NoSQL for audit?)
- Data backup and recovery procedures
- Point-in-time recovery capabilities

#### 4.3 Performance Optimization 🟠 HIGH

**Why:** Database queries are often the bottleneck in web applications.

**Investigation:**

- Slow query identification (query store, DMVs)
- Execution plan analysis
- Missing index recommendations
- Table partitioning opportunities
- Read/write ratio and optimization strategies

#### 4.4 Cost Optimization 🟠 HIGH

**Why:** €50K/month cloud costs with unclear justification.

> **Yann (Workshop 2, Line 300):** "Les coûts sont énormes. J'en ai pour 50K chaque mois de cloud."

**Investigation:**

- DTU/vCore sizing analysis
- Cosmos DB RU allocation and usage
- Data retention policies
- Backup storage costs
- ✅ Azure SQL Hyperscale proposal analyzed - See [Performance Proposition](../resources/performance_proposition/CISACAzureInfrastructureUpdate/CISACAzureInfrastructureUpdate/CISACAzureInfrastructureUpdate.md)
  - **Claimed savings:** €3,300/month (Business Critical → Hyperscale with 1 Primary + 1 Secondary)
  - **Additional costs:** €1,500/month (WAF, VPN, Data Factory upgrades)
  - **Net impact:** €1,800/month savings claimed
  - **Validation needed:** Verify performance requirements justify Hyperscale tier

---

### 5. Data Processing (Databricks + Data Factory) 🟠 HIGH | DEEP

**Component Description:**

- **Data Factory** - Orchestrates data movement and pipeline triggers
- **Databricks** - Python/PySpark processing for SFTP file ingestion
- Processes EDI/JSON files from agency SFTP submissions

**Infrastructure Location:**

- Azure Data Factory (pipelines)
- Azure Databricks (Python notebooks, outdated version)
- Data Lake Storage (file staging)

**Current Documentation Status:**

- ✅ [Databricks Architecture](architecture/Databricks.md) - Complete with PySpark workflows documented
- ✅ **Runtime version confirmed:** 10.4 LTS (outdated, missing modern features)
- ✅ Source code: 65+ files (C#, Python, SQL, ARM templates)
- ✅ Data Factory pipelines documented: `deployment/DataFactory/pipeline/`

**Focus Areas:**

#### 5.1 Version Currency and Technical Debt 🔴 CRITICAL

**Why:** Outdated Databricks prevents using modern features and AI capabilities.

> **Yann (Line 237):** "Databricks n'est plus à jour de plusieurs versions... vous ne pouvez pas bénéficier, par exemple, de l'IA dans vos requêtes."

**Investigation:**

- Current Databricks runtime version
- Upgrade path and breaking changes
- Feature gaps compared to latest version
- Security vulnerabilities in old runtime

#### 5.2 Pipeline Efficiency and Cost 🟠 HIGH

**Why:** File processing is described as one of the most complex and active areas.

> **Workshop 2, Mark (1:18:44):** "If you dropped a new file into SFTP... there would be a lot of work in processing that file in Databricks."

**Investigation:**

- Pipeline execution times and bottlenecks
- Cluster sizing and auto-scaling configuration
- Idle time and cost waste
- Batch vs streaming opportunities
- Error handling and retry logic

#### 5.3 Code Quality and Maintainability 🟡 MEDIUM

**Why:** PySpark notebooks can become unmaintainable without proper structure.

**Investigation:**

- Notebook organization and modularity
- Testing strategy (unit tests for transformations)
- Code reuse and duplication
- Documentation and comments

---

### 6. SFTP File Exchange 🟡 MEDIUM | MEDIUM

**Component Description:**

- Linux VM with native SFTP server
- Agency-specific folders in Azure Storage Account
- Bidirectional file exchange (agencies push/pull)
- Standard transmission method (per EDI spec)

**Infrastructure Location:**

- Linux VM in Azure
- Azure Storage Account (file shares)

**Current Documentation Status:**

- ✅ [SFTP Usage Documentation](architecture/SFTP-Usage.md) - File exchange workflows and security

**Focus Areas:**

#### 6.1 Security and Access Control 🟡 MEDIUM

**Why:** Agencies worldwide access SFTP with credentials - must be secure.

**Investigation:**

- Authentication method (password vs SSH keys)
- Folder isolation verification (agencies can't see each other)
- Credential rotation policies
- Network security (firewall rules, IP whitelisting)

#### 6.2 File Processing Workflow 🟡 MEDIUM

**Why:** SFTP triggers Databricks pipelines - reliability is critical.

**Investigation:**

- File arrival detection mechanism
- Processing status tracking
- Error notification to agencies (ACK/CSN files)
- Retry and dead letter queue handling
- **Reference:** SFTP-Usage.md (already documented)

---

### 7. Infrastructure as Code & DevOps 🔴 CRITICAL | DEEP

**Component Description:**

- Azure Resource Manager (ARM) templates
- Deployment automation and environment replication

**Current Documentation Status:**

- ✅ [Infrastructure Reference](infra/infrastructure-azure-reference.md) - Complete inventory of 343 Azure resources
- ✅ Source code: ARM templates in `deployment/` directory
- ✅ [Workshop 1 Findings](../meetings/20251020-ISWC Audit - Workshop 1.txt) - Build pipeline demonstration (Lines 39:00-46:00)
- ✅ Azure DevOps YAML pipelines documented

**Focus Areas:**

#### 7.1 Environment Replication 🔴 CRITICAL

**Why:** 20-day estimate for environment extension reveals severe automation gaps.

> **Yann (Line 318):** "Là, ils m'ont dit, d'accord, ça sera juste 20 000. Et puis, 20 jours de travail. Là, j'ai fait, là, il y a un vrai problème."

**Investigation:**

- IaC completeness (all resources defined in code?)
- Environment parity (Dev, UAT, Prod differences)
- Automated deployment pipeline
- Configuration management (secrets, connection strings)
- **Cost Impact:** Should be 0.5 days, not 20 days

#### 7.2 Deployment Process and Governance 🔴 CRITICAL

**Why:** No deployment tracking until Yann implemented CAB in 2024.

> **Yann (Line 141):** "J'ai mis en place un CAB pour les déploiements parce qu'il n'y avait rien, il faisait tout quand il voulait sans même prévenir."

**Investigation:**

- CI/CD pipeline analysis (build, test, deploy automation)
- Deployment frequency and rollback capability
- Change approval process
- Release notes and documentation updates
- **Goal:** Assess DoD (Definition of Done) compliance

#### 7.3 Production Incident Analysis 🔴 CRITICAL

**Why:** 6-month recovery from merge incident indicates serious process failures.

> **Yann (Line 558):** "On a mis six mois à fixer tous les problèmes... Un POC sur un autre projet, ça a été mergé avec la branche principale et c'est sorti en prod."

**Investigation:**

- Root cause analysis of May-June 2024 incident
- Branching strategy and merge controls
- Testing before production deployment
- Incident response procedures
- Lessons learned and corrective actions

---

### 8. Monitoring & Observability 🟠 HIGH | MEDIUM

**Component Description:**

- Application Insights for telemetry
- Potential gaps in comprehensive observability

**Infrastructure Location:**

- Application Insights workspace

**Focus Areas:**

#### 8.1 Performance Monitoring 🟠 HIGH

**Why:** Cannot optimize what you don't measure.

**Investigation:**

- Configured metrics and alerts
- Application Performance Monitoring (APM) coverage
- Distributed tracing for API calls
- Custom metrics for business KPIs
- Dashboard completeness

#### 8.2 Cost Monitoring and Attribution 🟠 HIGH

**Why:** €50K/month without clear breakdown enables waste.

**Investigation:**

- Resource tagging for cost allocation
- Cost anomaly detection
- Budget alerts configuration
- Underutilized resource identification
- **Goal:** Create transparent cost breakdown by component

#### 8.3 Alerting and Incident Response 🟡 MEDIUM

**Why:** Proactive alerts prevent outages and data loss.

**Investigation:**

- Alert coverage (error rates, latency, availability)
- On-call rotation and escalation
- Incident response playbooks
- Post-mortem process

---

### 9. Security & Compliance 🟠 HIGH | MEDIUM

**Component Description:**

- Authentication (FastTrack SSO, API keys)
- Authorization (agency-level access control)
- Data protection (encryption, PII handling)

**Infrastructure Location:**

- Azure Key Vault (secrets)
- Azure AD integration (if applicable)

**Focus Areas:**

#### 9.1 Authentication and Authorization 🟠 HIGH

**Why:** Multi-agency system requires robust access control.

**Investigation:**

- FastTrack SSO integration security
- API key management and rotation
- Role-based access control (RBAC) implementation
- Session management and timeout policies

#### 9.2 Data Protection 🟠 HIGH

**Why:** ISWC is authoritative global registry with PII (creator names).

**Investigation:**

- Encryption at rest (SQL, Cosmos, Storage)
- Encryption in transit (TLS versions, certificate management)
- PII identification and handling
- Data residency compliance (if applicable)

#### 9.3 Vulnerability Management 🟡 MEDIUM

**Why:** .NET 3.1 EOL and outdated components create risk.

**Investigation:**

- Dependency scanning (NuGet packages, Python libraries)
- Known vulnerability remediation status
- Security patch cadence
- Penetration testing results (if available)

---

### 10. Documentation & Knowledge Management 🔴 CRITICAL | MEDIUM

**Component Description:**

- SharePoint/Teams documentation repository
- Core design documents (2019-2020)
- Meeting notes and decision records

**Focus Areas:**

#### 10.1 Documentation Currency and Organization 🔴 CRITICAL

**Why:** Outdated docs prevent effective handover to new vendor.

> **Yann (Line 76):** "La documentation n'est pas disponible quand on le veut... c'est confus tout ça, c'est pas forcément au bon endroit, il y en a partout, elle est pas forcément à jour."

> **Yann (Line 76):** "Quand je vois des specs qui datent de 2019 avec 2B Requirements MVP et que c'est ce qui est utilisé en prod, alors que je suis sûr qu'il y a eu des mises à jour et que c'est pas dedans."

**Investigation:**

- Document inventory and categorization
- Version control and update tracking
- Specification vs implementation delta
- Missing documentation identification
- **Goal:** Create documentation improvement roadmap

#### 10.2 Architecture Decision Records 🟡 MEDIUM

**Why:** Understanding "why" decisions were made is critical for future changes.

**Investigation:**

- ADR existence and completeness
- Technology choice rationale
- Trade-off documentation
- Lessons learned from production issues

---

### 11. Proposed Architecture Upgrade 🔴 CRITICAL | DEEP

**Component Description:**

- Hyperscale proposal for SQL Server
- Security enhancements (declined)
- Cost: Significant increase

**Focus Areas:**

#### 11.1 Business Case Validation 🔴 CRITICAL

**Why:** Must verify upgrade necessity vs addressing root causes.

> **Yann (Line 76):** "Il ne faut pas que ce soit une architecture qui vient cacher la misère d'un code mal foutu."

> **Guillaume (Line 246):** "Ce n'est pas un vrai changement d'architecture... C'était aller payer plus cher le cloud."

**Investigation:**

- Current bottleneck identification (is SQL the real issue?)
- Hyperscale benefits vs current tier
- Alternative solutions analysis (query optimization, indexing, caching)
- ROI calculation (cost increase vs performance gain)
- **Goal:** Independent recommendation: justified, overkill, or misdiagnosed?

#### 11.2 Migration Risk Assessment 🟠 HIGH

**Why:** Major architecture changes carry risk of downtime and data loss.

**Investigation:**

- Migration plan completeness
- Rollback strategy
- Data migration testing approach
- Downtime estimation
- Compatibility issues (application code changes required?)

---

## Investigation Methodology

**Constraint:** 12 man-days over 3 weeks (November 4-22, 2025)
**Approach:** Focused strategic assessment leveraging existing documentation

### Phase 1: Foundation (COMPLETED - Week 1) ✅

**Activities Completed:**

1. ✅ Synthesized core design documents (15+ specifications from 2019-2020)
2. ✅ Mapped Azure infrastructure (343 resources documented)
3. ✅ Obtained source code access (300+ files analyzed)
4. ✅ Documented current state architecture - 7 component documents
5. ✅ Created C4 model diagrams ([Structurizr DSL](infra/overview/infrastructure-diagram-structurizr.dsl))

**Deliverables Completed:**

- ✅ [Infrastructure Reference](infra/infrastructure-azure-reference.md) - Complete Azure inventory
- ✅ [Architecture Documentation](architecture/index.md) - 7 component deep-dives
- ✅ [Code Analysis](code_analysis/MatchingEngine_integration.md) - Matching Engine integration
- ✅ Infrastructure diagrams (Mermaid, Structurizr C4)

**Documentation Available:**

- Core specifications: `docs/resources/core-design/`
- Source code: `docs/resources/source-code/ISWC/`
- Workshop transcripts: `docs/meetings/`
- Performance proposition: `docs/resources/performance_proposition/`

### Phase 2: Focused Investigations (Week 2 - Nov 11-15) [4 days]

**Critical Path Activities:**

1. **Hyperscale Proposal Deep-Dive** [2 days]
   - Analyze claimed €3,300/month savings (Business Critical → Hyperscale)
   - Review additional costs (€1,500/month for WAF, VPN, Data Factory)
   - Validate performance requirements vs. query optimization alternatives
   - Compare against reserved instance strategies

2. **IaC & DevOps Maturity** [1 day]
   - Validate 20-day/€20K environment extension estimate
   - Review ARM templates and pipeline automation
   - Assess production incident root causes (May-June 2024)

3. **Cost Analysis** [1 day]
   - Break down €50K/month spending by service
   - Identify reserved instance opportunities
   - Right-sizing analysis for App Services, SQL, Cosmos DB

**Deliverables:**

- Hyperscale Go/No-Go recommendation
- IaC maturity scorecard
- Cost optimization quick wins list

### Phase 3: Technical Risk Assessment (Week 2 - Nov 11-15) [Parallel]

**Activities:**

1. **Matching Engine Performance Analysis** [1 day]
   - Synchronous blocking impact on submission latency
   - Alternative async integration patterns (queue-based, event-driven)
   - Technical feasibility of vendor replacement

2. **Technical Debt Prioritization** [1 day]
   - .NET 3.1 EOL security vulnerabilities
   - Databricks 10.4 LTS upgrade path
   - Estimate upgrade efforts and costs

3. **Targeted Code Quality Review** [integrated throughout]
   - Focus on critical paths: submission workflow, matching integration
   - Identify high-risk patterns or anti-patterns
   - Performance bottlenecks in hot paths

**Deliverables:**

- Technical coupling assessment (Matching Engine)
- Technical debt register with priorities
- Upgrade roadmap

### Phase 4: Synthesis & Recommendations (Week 3 - Nov 18-22) [4 days]

**Activities:**

1. **Executive Summary** [1 day]
   - Key findings and critical issues
   - Strategic recommendations
   - Risk summary

2. **Technical Report Consolidation** [1 day]
   - Architecture assessment
   - Performance analysis
   - Security and compliance findings

3. **Implementation Roadmap** [1 day]
   - Phased modernization plan
   - Cost-benefit analysis
   - Risk mitigation strategies

4. **Presentation Preparation** [1 day]
   - Steering committee deck
   - Supporting materials
   - Q&A preparation

**Deliverables:**

- Executive Summary Report
- Technical Findings Document
- Implementation Roadmap with Estimates
- Steering Committee Presentation

**Final Delivery: November 21, 2025**

---

## Success Criteria

### Must Have

- ✅ **Transparency on vendor lock-in** - Clear understanding of Matching Engine coupling
- ✅ **Cost justification** - Breakdown of €50K/month and Hyperscale proposal validation
- ✅ **Technical debt quantification** - .NET, Databricks, automation gaps documented
- ✅ **Migration feasibility** - Clear path to vendor independence or alternative
- ✅ **Documentation roadmap** - Plan to update and organize knowledge base

### Should Have

- ⭐ **Performance optimization** - Quick wins identified without architecture change
- ⭐ **Security assessment** - Vulnerability remediation priorities
- ⭐ **DevOps maturity** - CI/CD and IaC improvement recommendations
- ⭐ **Code quality baseline** - Metrics for future vendor comparison

### Nice to Have

- 💡 **Alternative architecture** - Modern cloud-native design proposal
- 💡 **Cost reduction plan** - 20-30% reduction roadmap
- 💡 **Knowledge transfer** - Vendor handover checklist

---

## Risks and Constraints

### Access Restrictions

**Risk:** Matching Engine code access denied
**Mitigation:** API contract analysis, black-box performance testing, contractual review

**Risk:** CISAC API code access delayed
**Mitigation:** Focus on infrastructure, configuration, and documentation phases first

### Time Constraints

**Risk:** 4-6 week timeline with access delays
**Mitigation:** Prioritize CRITICAL and HIGH items, defer MEDIUM/LOW if necessary

### Vendor Relationship

**Risk:** Defensive posture from Spanish Point
**Mitigation:** Frame as objective technical assessment, not blame assignment

> **Yann (Line 123):** "On est tous dans la même équipe on n'est pas en train d'essayer de mettre des buts contre notre camp."

---

## Appendix: Component-to-Document Mapping

| Component | Core Specifications (2019-2020) | Architecture Docs (2025) | Source Code | Workshop References |
|-----------|--------------------------------|-------------------------|-------------|---------------------|
| **Matching Engine** | SPE_20190424_MVPMatchingRules.md | [MatchingEngine.md](architecture/MatchingEngine.md)<br/>[Integration Analysis](code_analysis/MatchingEngine_integration.md) | `src/Services/MatchingEngine/`<br/>42+ files | WS1: 23:00-36:00 (access)<br/>WS2: 27:36-28:37 |
| **CISAC API** | SPE_20191217_CISAC ISWC REST API.md | ⏳ In progress | `src/Api.Agency/`<br/>`src/Api.Label/`<br/>`src/Api.Publisher/` | WS1: 39:00-46:00 (pipeline)<br/>WS2: 58:23-59:06 |
| **Agency Portal** | SPE_20190806_ISWC_Portal.md | [ISWC-Agency-Portal.md](architecture/ISWC-Agency-Portal.md) | `src/Portal.Agency/`<br/>161+ files | WS2: 57:18-57:32 |
| **Public Portal** | SPE_20200108_ISWC_Public_Portal.md | ⏳ Pending | `src/Portal.Public/` | - |
| **Cosmos DB** | *(Implied in data model)* | [CosmosDB.md](architecture/CosmosDB.md) | Repository pattern | - |
| **Audit Logging** | *(Implied in data model)* | [AuditLogging.md](architecture/AuditLogging.md) | `CosmosDbAuditService.cs` | - |
| **Databricks** | *(Processing specs)* | [Databricks.md](architecture/Databricks.md) | 65+ files (PySpark, C#) | WS2: IPI sync discussion |
| **SFTP** | *(File format specs)* | [SFTP-Usage.md](architecture/SFTP-Usage.md) | Linux VM config | - |
| **Data Model** | SPE_20190218_ISWCDataModel_REV (PM).md | ⏳ SQL schema analysis pending | SQL DDL scripts | WS2: 39:04-42:57 |
| **EDI Files** | SPE_20190806_ISWC_EDI_FileFormat.md | *(Covered in Databricks)* | File parsers | - |
| **JSON Files** | SPE_20191118_ISWC_JSON_FileFormat.md | *(Covered in Databricks)* | File parsers | - |
| **Validation** | SPE_20190424_MVPValidationRules.md | ⏳ Pending | Validation services | - |
| **IPI Integration** | SPE_20191001_ISWC_IPI_Integration.md | *(Covered in Databricks)* | IPI sync notebooks | - |
| **Workflows** | ISWCIA20-0312_Guidelines...EN.md | *(Covered in Portal)* | Workflow engine | - |
| **Reporting** | SPE_20200602_ISWC_Reporting.md | ⏳ Pending | Report generators | - |
| **Infrastructure** | *(N/A - implicit)* | [Infrastructure Reference](infra/infrastructure-azure-reference.md)<br/>[C4 Diagrams](infra/overview/) | ARM templates<br/>DevOps YAML | WS1: 39:00-46:00 |
| **Performance Proposal** | *(N/A - new 2025)* | [PSA5499 Analysis](../resources/performance_proposition/CISACAzureInfrastructureUpdate/CISACAzureInfrastructureUpdate/CISACAzureInfrastructureUpdate.md) | *(N/A)* | - |

### Documentation Completion Status

**✅ Completed (7 documents):**

1. Matching Engine architecture + integration analysis
2. Agency Portal architecture
3. Cosmos DB architecture
4. Audit Logging architecture
5. Databricks architecture
6. SFTP Usage
7. Infrastructure reference (343 resources)

**⏳ In Progress:**

- CISAC API detailed architecture
- SQL Server schema analysis

**📋 Planned (Out of Scope for 12-day audit):**

- Public Portal architecture
- Validation rules analysis
- Reporting system analysis
- Complete code quality review

---

**Document Status:** Version 2.0 - Updated with Workshop 1 findings and realistic timeline

**Next Steps:**

1. Week 2: Hyperscale proposal validation and cost analysis
2. Week 2: IaC maturity assessment and contract review
3. Week 3: Final report synthesis and presentation

**Revised Effort Estimate:** 12 man-days over 3 weeks (November 4-22, 2025)
