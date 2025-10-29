# C4 Architecture Model - CISAC ISWC System

**Version:** 1.0
**Last Updated:** 2025-10-29
**Status:** 🚧 Under Construction - Validation Phase

## Overview

This document serves as the central navigation hub for the CISAC ISWC system's C4 architecture model. The model is being rebuilt from primary sources with validation at each level to ensure audit-quality accuracy.

### What is C4?

The C4 model is a lean graphical notation technique for modeling software architecture. It consists of hierarchical diagrams at four levels:

1. **Level 1 - System Context:** Shows the system in scope and its relationships with users and other systems
2. **Level 2 - Containers:** Shows the high-level technology choices and how containers communicate
3. **Level 3 - Components:** Shows how containers are made up of components and their interactions
4. **Level 4 - Code:** Shows how a component is implemented (typically out of scope for architecture docs)

### Validation Methodology

This C4 model is being built using a **validation-first approach**:

**Primary Source:** [InfrastructureDiagram.png](../../resources/InfrastructureDiagram.png) - Authoritative Azure deployment diagram
**Secondary Sources:** [Core Design Documents](../../resources/core_design_documents/) - Functional specifications
**Tertiary Sources:** [Source Code](../../resources/source-code/ISWC/src/) - Implementation reality

Each level is researched from these sources, documented, and **validated by domain experts** before proceeding to the next level.

## Quick Navigation

- [Validation Status](#validation-status-tracker)
- [Level 1: System Context](#level-1-system-context)
- [Level 2: Container Views](#level-2-container-views)
- [Level 3: Component Details](#level-3-component-details)
- [Integration Patterns](#integration-patterns)
- [Documentation Progress](#documentation-progress)
- [Open Questions](#open-questions)

---

## Validation Status Tracker

| Level | Status | Last Validated | Validator | Notes |
|-------|--------|----------------|-----------|-------|
| Level 1 - System Context | 🔴 **Not Validated** | - | - | In progress |
| Level 2 - Container View | 🔴 **Not Started** | - | - | Awaiting Level 1 validation |
| Level 3 - Components | 🟡 **Partial** | Various | - | 7 of 20 components documented |

**Legend:**

- ✅ **Validated** - Reviewed and approved by domain expert
- 🟡 **Partial** - Some elements validated, others pending
- 🔴 **Not Validated** - Awaiting validation
- 🔴 **Not Started** - Research not begun

---

## Level 1: System Context

**Status:** 🚧 Under Construction

### Proposed System Boundaries

Based on preliminary analysis of InfrastructureDiagram.png and source code:

**System 1: ISWC Platform** (Main System)

- The core CISAC ISWC work registration and management platform
- Includes all application services, data processing, and storage
- Developed and maintained by Spanish Point Technologies
- Deployed in Azure West Europe

**System 2: Matching Engine** (External Vendor System)

- Work matching and search platform
- Developed and maintained by Spanish Point Technologies
- Integrated via HTTP REST API
- Shares Azure infrastructure with ISWC Platform
- Evidence: Source code shows HTTP client with external base URL

**External Systems:**

- **FastTrack SSO** - External authentication provider for agency users
- **Suisa API** - External API for Swiss society integration
- **Suisa SFTP** - External SFTP server for file exchange

### Key Findings from Validation Research

**Critical Discovery #1: Data Platform is NOT a separate system**

- Databricks, Data Factory, Cosmos DB, SQL Server, Data Lake are part of ISWC Platform
- They are deployment containers within the main system, not a separate logical system
- All accessed directly by ISWC APIs and Jobs

**Critical Discovery #2: Matching Engine is External**

- Source code proof: `BaseAddress-SpanishPointMatchingEngine` configuration
- No Matching Engine source code in repository
- ISWC APIs make HTTP calls to it as external dependency
- However, shares Azure infrastructure (same subscription, resource groups)

**Critical Discovery #3: Networking is Infrastructure**

- Virtual Network, Public IP are Azure infrastructure, not software systems
- ISWC SFTP should be a container in ISWC Platform

### Documentation

📄 **[Full System Context Documentation](c4-views/level1-system-context.md)** - 🔴 Not yet created

---

## Level 2: Container Views

**Status:** 🔴 Not Started (Awaiting Level 1 validation)

### Preliminary Container Inventory

Based on InfrastructureDiagram.png and source code analysis:

#### ISWC Platform Containers (Proposed)

**Web Applications:**

- **ISWC Agency Portal** - Azure App Service (React + ASP.NET Core 3.1)
  - Source: `Portal` project (found)
  - Status: ✅ Documented in [agency-portal.md](components/iswc-platform/agency-portal.md)
- **ISWC Public Portal** - Azure App Service (React + ASP.NET Core 3.1)
  - Source: ⚠️ **NOT FOUND** in repository
  - Status: ⚠️ Needs investigation

**APIs:**

- **ISWC Agency API** - Azure App Service (ASP.NET Core 3.1)
  - Source: `Api.Agency` project (found)
- **ISWC Label API** - Azure App Service (ASP.NET Core 3.1)
  - Source: `Api.Label` project (found)
- **ISWC Publisher API** - Azure App Service (ASP.NET Core 3.1)
  - Source: `Api.Publisher` project (found)
- **ISWC Third Party API** - Azure App Service (ASP.NET Core 3.1)
  - Source: `Api.ThirdParty` project (found)

⚠️ **Open Question:** InfrastructureDiagram.png shows single "ISWC Api" box, but source code has 4 API projects. Are these deployed as separate App Services or combined?

**Background Processing:**

- **ISWC Jobs** - Azure Functions v3
  - Source: `Jobs` project (found)
  - 8 function jobs for scheduled tasks

**Data Processing:**

- **Databricks** - Azure Databricks 10.4 LTS
  - Status: ✅ Documented in [databricks.md](components/iswc-platform/databricks.md)
  - ⚠️ Technical debt: Outdated runtime version
- **Data Factory** - Azure Data Factory v2
  - Status: 🔴 Not documented
  - ⚠️ Pipeline definitions not found in source code

**Databases:**

- **Cosmos DB** - Azure Cosmos DB (MongoDB API)
  - Status: ✅ Documented in [cosmos-db.md](components/iswc-platform/cosmos-db.md)
  - Collections: WID JSON, ISWC JSON
- **SQL Server - ISWC Database** - Azure SQL Database
  - Status: 🔴 Not documented
  - ⚠️ Schema needs ERD
- **SQL Server - IPI Database** - Azure SQL Database
  - Status: 🔴 Not documented
  - IPI = Interested Party Information

**Storage:**

- **Data Lake** - Azure Data Lake Storage Gen2
  - Status: 🔴 Not documented

**File Transfer:**

- **ISWC SFTP Server** - Azure Virtual Machine
  - Status: ✅ Documented in [sftp-server.md](components/networking/sftp-server.md)

**Infrastructure (NOT modeled as containers in C4):**

- API Management - Gateway (may show at Level 1 or deployment view)
- App Service Plan - Hosting infrastructure
- Key Vault - Secret management infrastructure
- Application Insights - Monitoring infrastructure
- Virtual Network - Network infrastructure
- Public IP - Network infrastructure

#### Matching Engine Containers

⚠️ **Note:** External system, no source code in repository

- **ME Portal** - Web interface for matching operations
- **ME API** - REST API for work matching
- **Search Service** - Azure Cognitive Search for indexing

### Documentation

📄 **[Full Container View Documentation](c4-views/level2-containers.md)** - 🔴 Not yet created

---

## Level 3: Component Details

**Status:** 🟡 Partial (7 of 20 components documented)

### Completed Component Documentation

#### ISWC Platform Components

**Web Applications:**

- ✅ [Agency Portal](components/iswc-platform/agency-portal.md) (v2.0) - React + ASP.NET Core frontend
- 🔴 Public Portal - Not documented (source code missing)

**Data Platform:**

- ✅ [Databricks](components/iswc-platform/databricks.md) (v1.1) - Data processing notebooks and jobs
- ✅ [Cosmos DB](components/iswc-platform/cosmos-db.md) (v1.0) - NoSQL document storage
- 🔴 Data Factory - Not documented **[CRITICAL]**
- 🔴 SQL Server - Not documented **[HIGH PRIORITY]**
- 🔴 Data Lake - Not documented

**APIs:**

- 🔴 ISWC API(s) - Not documented **[CRITICAL]** - Core submission pipeline

**Background Processing:**

- 🔴 ISWC Jobs - Not documented **[HIGH PRIORITY]** - Azure Functions catalog

**File Transfer:**

- ✅ [SFTP Server](components/networking/sftp-server.md) (v1.0) - File exchange patterns

#### Matching Engine Components

- ✅ [Matching Engine](components/matching-engine/matching-engine.md) (v1.0) - External HTTP API integration

### Priority Queue for New Documentation

#### 🔴 CRITICAL (Week 1)

1. **ISWC API(s)** - Core submission pipeline (4 API projects)
   - Understanding if deployed as 1 or 4 App Services
   - Pipeline orchestration (Validation → Matching → Processing)
   - Matching Engine HTTP integration
   - Database access patterns

2. **Data Factory** - ETL orchestration
   - Pipeline definitions and triggers
   - Databricks notebook invocation
   - File processing workflows
   - ⚠️ Pipeline definitions not in source code - needs investigation

3. **Pipeline Orchestration** (Integration Pattern doc)
   - How validation, matching, and processing pipelines work
   - Component interaction flow
   - Error handling and retry logic

#### 🟠 HIGH PRIORITY (Week 2)

4. **ISWC Jobs** - Azure Functions catalog
   - Job types and schedules
   - IPI synchronization jobs
   - Workflow update jobs
   - Agent run monitoring

5. **SQL Server** - Database schema
   - Entity Relationship Diagram (ERD)
   - ISWC vs IPI schema separation
   - Key tables and relationships
   - Partitioning strategy

6. **API Authentication** (Integration Pattern doc)
   - OAuth2 flows
   - FastTrack SSO integration
   - Token management
   - Authorization patterns

#### 🟡 MEDIUM PRIORITY (Week 3)

7. **API Management** - Gateway configuration
   - Routing policies
   - Rate limiting
   - External integration endpoints

8. **Public Portal** - Public-facing web app
   - ⚠️ Source code investigation needed first

9. **Data Lake** - Storage patterns
   - Raw vs processed data organization
   - Databricks integration
   - Retention policies

---

## Integration Patterns

Cross-cutting concerns and integration patterns documentation:

### Completed

- ✅ [Audit Logging](integration-patterns/audit-logging.md) (v1.0) - Audit trail implementation
- ✅ [Performance](integration-patterns/performance.md) (v1.0) - System-wide performance analysis

### Planned

- 🔴 **Pipeline Orchestration** - Validation → Matching → Processing flow **[CRITICAL]**
- 🔴 **API Authentication** - OAuth2 + FastTrack SSO patterns **[HIGH]**
- 🔴 **Database Access Patterns** - Repository pattern, connection management
- 🔴 **Error Handling** - Retry policies, circuit breakers, dead-letter queues
- 🔴 **Monitoring and Telemetry** - Application Insights integration

---

## Documentation Progress

### Coverage Statistics

- **Level 1 (System Context):** 🔴 0% Complete (0/1 validated)
- **Level 2 (Containers):** 🔴 0% Complete (0/21 containers validated)
- **Level 3 (Components):** 🟡 35% Complete (7/20 major components documented)
- **Integration Patterns:** 🟡 25% Complete (2/8 planned patterns documented)

### Documentation by System

#### ISWC Platform

| Component | Type | Status | Priority | Doc Link |
|-----------|------|--------|----------|----------|
| Agency Portal | Web App | ✅ Complete (v2.0) | - | [Link](components/iswc-platform/agency-portal.md) |
| Public Portal | Web App | 🔴 Not Started | 🟡 Medium | ⚠️ Source code missing |
| Agency API | API | 🔴 Not Started | 🔴 Critical | Part of API investigation |
| Label API | API | 🔴 Not Started | 🔴 Critical | Part of API investigation |
| Publisher API | API | 🔴 Not Started | 🔴 Critical | Part of API investigation |
| Third Party API | API | 🔴 Not Started | 🔴 Critical | Part of API investigation |
| ISWC Jobs | Functions | 🔴 Not Started | 🟠 High | - |
| Databricks | Data Processing | ✅ Complete (v1.1) | - | [Link](components/iswc-platform/databricks.md) |
| Data Factory | ETL | 🔴 Not Started | 🔴 Critical | - |
| Cosmos DB | Database | ✅ Complete (v1.0) | - | [Link](components/iswc-platform/cosmos-db.md) |
| SQL Server (ISWC) | Database | 🔴 Not Started | 🟠 High | - |
| SQL Server (IPI) | Database | 🔴 Not Started | 🟠 High | - |
| Data Lake | Storage | 🔴 Not Started | 🟡 Medium | - |
| ISWC SFTP | File Transfer | ✅ Complete (v1.0) | - | [Link](components/networking/sftp-server.md) |

#### Matching Engine (External)

| Component | Type | Status | Priority | Doc Link |
|-----------|------|--------|----------|----------|
| Matching Engine | External API | ✅ Complete (v1.0) | - | [Link](components/matching-engine/matching-engine.md) |
| ME Portal | Web App | 🔵 Deferred | - | Vendor-managed, out of scope |
| Search Service | Search | 🔵 Deferred | - | Covered in ME doc |

#### Cross-Cutting Concerns

| Pattern | Status | Priority | Doc Link |
|---------|--------|----------|----------|
| Audit Logging | ✅ Complete (v1.0) | - | [Link](integration-patterns/audit-logging.md) |
| Performance | ✅ Complete (v1.0) | - | [Link](integration-patterns/performance.md) |
| Pipeline Orchestration | 🔴 Not Started | 🔴 Critical | - |
| API Authentication | 🔴 Not Started | 🟠 High | - |
| Database Access | 🔴 Not Started | 🟡 Medium | - |
| Error Handling | 🔴 Not Started | 🟡 Medium | - |
| Monitoring | 🔴 Not Started | 🟡 Medium | - |

---

## Open Questions

### Critical Questions (Blocking Documentation)

1. **ISWC API Deployment Architecture**
   - **Question:** Are the 4 API projects (Agency, Label, Publisher, ThirdParty) deployed as 4 separate Azure App Services, or combined into one?
   - **Evidence:** Source code has 4 separate .csproj projects
   - **Evidence:** InfrastructureDiagram.png shows single "ISWC Api" box
   - **Impact:** Affects container model at Level 2
   - **Status:** ⏳ Awaiting validation

2. **ISWC Public Portal Source Code**
   - **Question:** Where is the source code for the ISWC Public Portal?
   - **Evidence:** Shown in InfrastructureDiagram.png
   - **Evidence:** Specified in SPE_20200108_ISWC_Public_Portal design doc
   - **Evidence:** NOT found in source code repository
   - **Impact:** Cannot document component without source code
   - **Possibilities:**
     - Separate repository?
     - Not implemented yet (planned feature)?
     - Archived/deprecated?
   - **Status:** ⏳ Awaiting investigation

3. **Data Factory Pipeline Definitions**
   - **Question:** Where are the Data Factory pipeline definitions?
   - **Evidence:** Data Factory shown in InfrastructureDiagram.png
   - **Evidence:** NOT found in source code repository (no ARM templates, JSON definitions)
   - **Impact:** Cannot document ETL orchestration without pipeline definitions
   - **Possibilities:**
     - Managed in Azure Portal only (not source-controlled)?
     - Separate repository?
     - Separate infrastructure repo?
   - **Status:** ⏳ Awaiting investigation

### High Priority Questions

4. **Cosmos DB Collections**
   - **Question:** Are WID JSON and ISWC JSON separate Cosmos DB databases or collections within one database?
   - **Impact:** Affects data model documentation
   - **Status:** ⏳ Awaiting validation

5. **SQL Server Instances**
   - **Question:** Are ISWC and IPI databases on the same SQL Server instance or separate instances?
   - **Impact:** Affects infrastructure and deployment documentation
   - **Status:** ⏳ Awaiting validation

6. **Matching Engine Deployment**
   - **Question:** Is Matching Engine deployed in same Azure subscription as ISWC Platform?
   - **Evidence:** Diagram shows shared databases (Cosmos DB, SQL Server)
   - **Impact:** Affects system boundary definition and integration documentation
   - **Status:** ⏳ Awaiting validation

### Medium Priority Questions

7. **API Management Usage**
   - **Question:** Does API Management route all external traffic, or are there direct connections?
   - **Impact:** Affects integration pattern documentation
   - **Status:** ⏳ Can infer from source code, but validation preferred

8. **Key Vault Access Pattern**
   - **Question:** Do all containers access Key Vault directly, or via App Service Plan?
   - **Impact:** Affects security architecture documentation
   - **Status:** ⏳ Can infer from source code configuration

---

## Historical Context

### Previous Structurizr DSL (Archived)

The original C4 model at `../infra/overview/infrastructure-diagram-structurizr.dsl` has been archived as of 2025-10-29 due to significant architectural errors discovered during validation.

**Key Issues Identified:**

- Invented fictional system boundaries ("Data Platform", "Networking Infrastructure")
- Modeled Azure infrastructure as C4 containers (App Service Plan, Key Vault, Virtual Network)
- Unclear Matching Engine boundary (should be marked as external)
- Single "ISWC API" container vs. 4 API projects in source code
- Missing source code for some components (Public Portal)

**Archived File:** [infrastructure-diagram-structurizr.dsl.ARCHIVED](../infra/overview/infrastructure-diagram-structurizr.dsl.ARCHIVED)

The archived file includes detailed comments explaining each issue.

### Validation Sources

This rebuilt C4 model uses the following primary sources:

1. **[InfrastructureDiagram.png](../../resources/InfrastructureDiagram.png)** - Authoritative Azure deployment diagram
2. **[Core Design Documents](../../resources/core_design_documents/)** - Functional specifications (12 documents)
3. **[Source Code](../../resources/source-code/ISWC/src/)** - Implementation reality (25 projects)

---

## Incremental Documentation Workflow

For each new component to be documented:

### 1. User Selection Phase

User specifies:

```
NEXT COMPONENT: [Component Name]
PRIORITY LEVEL: [Critical/High/Medium]
SPECIFIC FOCUS: [Optional - specific aspects to emphasize]
KNOWN GAPS: [Optional - areas needing investigation]
```

### 2. Research Phase (Claude)

- Analyze design documents for specifications
- Analyze source code for implementation
- Identify integration points
- Discover technology stack
- Create preliminary component diagram
- Document open questions

### 3. User Validation Phase

- Review research findings
- Answer clarification questions
- Provide domain knowledge
- Correct assumptions

### 4. Documentation Phase (Claude)

- Create component document using standard template
- Include validated Mermaid diagrams
- Add complete source code references
- Document known gaps

### 5. Review & Approval

- User reviews final document
- ✅ Approves → Mark complete, move to next
- 🔄 Requests revisions → Refine specific sections
- ⏸️ Pauses → Defer, pick different component

---

## Quality Standards

Each component document must include:

**Content Requirements:**

- Version number and date
- Sources section (Primary, Secondary, Tertiary)
- Overview (2-3 paragraphs)
- Technical Architecture section
- At least one Mermaid diagram
- Integration with Other Components section
- Source Code References (complete file list)
- Questions for Further Investigation
- References section
- Document History
- Known Gaps flagged with ⚠️

**Diagram Requirements:**

- Component diagram showing internal structure
- At least one integration diagram (sequence or data flow)
- Proper Mermaid syntax
- Clear labels and legends
- Technology annotations

**Citation Requirements:**

- Design docs cited with relative file paths
- Source code files listed with descriptions
- Meeting transcripts referenced if relevant
- External dependencies documented

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-10-29 | C4 Rebuild Team | Initial master navigation hub created with validation methodology |

---

## Next Steps

1. ✅ **Complete:** Archive old Structurizr DSL with issue documentation
2. ✅ **Complete:** Create master navigation hub (this document)
3. ✅ **Complete:** Create directory structure for reorganized docs
4. 🚧 **In Progress:** Build Level 1 - System Context from validated sources
5. ⏳ **Pending:** USER VALIDATION - Review and approve System Context
6. ⏳ **Pending:** Build Level 2 - Container View with research
7. ⏳ **Pending:** USER VALIDATION - Review and approve Container View
8. ⏳ **Pending:** Reorganize existing component docs to new structure
9. ⏳ **Pending:** Begin incremental Level 3 component documentation

**Current Focus:** Building validated System Context diagram from InfrastructureDiagram.png and source code analysis.
