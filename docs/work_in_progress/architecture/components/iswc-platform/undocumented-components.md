# Undocumented Components - ISWC Platform

**Version:** 2.0
**Date:** October 29, 2025
**Total Components:** 9 undocumented components requiring documentation

**Purpose:** Track components identified in the ISWC system architecture that lack comprehensive C4 Level 3 component documentation.

---

## Overview

During the audit, we identified **9 documented components** with C4 Level 3 structure:

✅ **Documented:**

1. [validation-pipeline.md](validation-pipeline.md)
2. [matching-pipeline.md](matching-pipeline.md)
3. [processing-pipeline.md](processing-pipeline.md)
4. [post-matching-pipeline.md](post-matching-pipeline.md)
5. [pipeline-architecture-overview.md](pipeline-architecture-overview.md)
6. [validation-rules-catalog.md](validation-rules-catalog.md)
7. [web-portals.md](web-portals.md) - Agency + Public Portals (shared codebase)
8. [databricks.md](databricks.md)
9. [cosmos-db.md](cosmos-db.md)

❌ **Undocumented:**
9 components lack comprehensive documentation (listed below)

---

## Priority Matrix

| Priority | Criteria |
|----------|----------|
| 🔴 **Critical** | Core system functionality, frequently referenced, high complexity |
| ⚠️ **High** | Important integration points, vendor dependencies, security concerns |
| 📝 **Medium** | Supporting infrastructure, well-understood patterns |
| 💡 **Low** | Standard Azure services, documented elsewhere, minimal custom logic |

---

## Undocumented Components List

### UC-002: SFTP Server

| Field | Value |
|-------|-------|
| **Priority** | 🔴 Critical |
| **Component Type** | File Transfer Infrastructure |
| **Technology** | Azure Storage Account with SFTP endpoint |
| **Purpose** | Agency file upload/download for EDI/JSON submissions and CSN/ACK responses |
| **Why Important** | Primary integration channel for many agencies; Security-sensitive (credentials, access control); Operational complexity (monitoring, alerting, folder structure) |
| **Effort Estimate** | 1-2 days |

**Key Questions:**

- How are agency folders structured?
- What authentication mechanism is used?
- How are credentials rotated?
- What file retention policies exist?
- How is quota management handled?

**Source Reference:** Mentioned in Workshop 2, [SPE_20190806_ISWC_EDI_FileFormat.md](../../../resources/core_design_documents/SPE_20190806_ISWC_EDI_FileFormat/SPE_20190806_ISWC_EDI_FileFormat.md)

---

### UC-003: Azure Data Factory

| Field | Value |
|-------|-------|
| **Priority** | 🔴 Critical |
| **Component Type** | Orchestration Engine |
| **Technology** | Azure Data Factory (ADF) |
| **Purpose** | Orchestrate file processing pipelines (SFTP → Databricks → SQL Server) |
| **Why Important** | Central orchestration layer; Triggers Databricks notebooks; Handles file archival and error routing; Critical for understanding end-to-end data flows |
| **Effort Estimate** | 2-3 days |

**Key Questions:**

- How many pipelines exist?
- What triggers are configured (file arrival, schedule, manual)?
- How are errors handled?
- What is the retry strategy?
- Are there pipeline dependencies?
- How is monitoring configured?

**Source Reference:** [SPE_20191001_ISWC_IPI_Integration.md](../../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md), Workshop 2

---

### UC-004: IPI WebJob (Incremental Updates)

| Field | Value |
|-------|-------|
| **Priority** | ⚠️ High |
| **Component Type** | Background Service (Azure WebJob or Function) |
| **Technology** | C# .NET (Azure WebJob or Azure Function) |
| **Purpose** | Incremental IPI updates via SUISA API (between quarterly full dumps) |
| **Why Important** | Keeps IPI data current; API integration with external vendor (SUISA); Complements Databricks IPI full resynch |
| **Effort Estimate** | 1-2 days |

**Key Questions:**

- What is the update frequency? (Daily? Hourly?)
- How are incremental updates applied to SQL Server?
- What happens if the SUISA API is unavailable?
- How are conflicts resolved between incremental and full dumps?
- Is there a high-water-mark tracking mechanism?

**Source Reference:** [SPE_20191001_ISWC_IPI_Integration.md](../../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md) → Section 4 "IPI REST API

 Integration"

**Source Code:** [src/Jobs/Functions/](../../../resources/source-code/ISWC/src/Jobs/Functions/) or [src/AdHoc/AddMissingIPIsConsoleApp/](../../../resources/source-code/ISWC/src/AdHoc/AddMissingIPIsConsoleApp/)

---

### UC-005: SQL Server (Azure SQL Database)

| Field | Value |
|-------|-------|
| **Priority** | ⚠️ High |
| **Component Type** | Relational Database |
| **Technology** | Azure SQL Server |
| **Purpose** | Primary metadata storage (works, ISWCs, IPs, titles, relationships) |
| **Why Important** | Central data store; Database schema design; Performance tuning (indexes, views); Backup and disaster recovery; Cost optimization |
| **Effort Estimate** | 2-3 days |

**Key Questions:**

- What is the database schema? (ERD diagram)
- What are the key tables and relationships?
- What indexes exist?
- What is the backup strategy? (Retention, RTO/RPO)
- What are the performance characteristics? (p50/p95/p99 query times)
- What is the monthly cost?

**Source Reference:** [SPE_20190218_ISWCDataModel_REV (PM).md](../../../resources/core_design_documents/SPE_20190218_ISWCDataModel_REV%20(PM)/SPE_20190218_ISWCDataModel_REV%20(PM).md)

**Source Code:** [src/Database/](../../../resources/source-code/ISWC/src/Database/) (SQL scripts, views, stored procedures)

---

### UC-006: Matching Engine (Spanish Point Vendor Dependency)

| Field | Value |
|-------|-------|
| **Priority** | ⚠️ High |
| **Component Type** | External API (Vendor Service) |
| **Technology** | Spanish Point proprietary matching engine (REST API) |
| **Purpose** | Fuzzy matching for title and creator similarity detection |
| **Why Important** | **Critical vendor dependency (single point of failure)**; Proprietary algorithms (vendor lock-in); Performance bottleneck (80-second timeout); Cost implications; No fallback strategy |
| **Effort Estimate** | 1-2 days |

**Key Questions:**

- What is the SLA? (Uptime, response time)
- What is the cost model? (Per-request? Fixed monthly?)
- What matching algorithms are used?
- Can the system operate without the Matching Engine?
- What is the disaster recovery plan?
- Who operates the service? (Spanish Point hosted? CISAC hosted?)

**Source Reference:** [SPE_20190424_MVPMatchingRules.md](../../../resources/core_design_documents/SPE_20190424_MVPMatchingRules/SPE_20190424_MVPMatchingRules.md)

**Related:** [matching-pipeline.md](matching-pipeline.md) documents the integration but not the vendor service itself

---

### UC-007: IPI Database (SUISA External Source)

| Field | Value |
|-------|-------|
| **Priority** | 📝 Medium |
| **Component Type** | External Data Source (Replica) |
| **Technology** | IPI Database (maintained by SUISA on behalf of CISAC) |
| **Purpose** | Authoritative source for Interested Party Information (IP names, numbers, agreements) |
| **Why Important** | External dependency; Data synchronization strategy; Quarterly full dumps + incremental updates; **Data quality impacts ISWC system** |
| **Effort Estimate** | 1 day |

**Key Questions:**

- What is the data format? (IPI EDI specification)
- How often are quarterly dumps available?
- What is the incremental update API?
- What is the SLA for API availability?
- How are data quality issues reported and resolved?
- Who operates the IPI database? (SUISA?)

**Source Reference:** [SPE_20191001_ISWC_IPI_Integration.md](../../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md)

**Related:** [databricks.md](databricks.md) documents the processing of IPI data but not the external source

---

### UC-008: FastTrack SSO (CIS-Net Authentication)

| Field | Value |
|-------|-------|
| **Priority** | 📝 Medium |
| **Component Type** | External Authentication Service |
| **Technology** | FastTrack SSO (SOAP API) |
| **Purpose** | Single sign-on authentication for Agency Portal users (CIS-Net credentials) |
| **Why Important** | **Critical vendor dependency (single point of failure for Portal)**; No fallback authentication; Security implications; SLA requirements |
| **Effort Estimate** | 1 day |

**Key Questions:**

- Who operates FastTrack? (CISAC or third-party vendor?)
- What is the uptime SLA?
- What happens if FastTrack is unavailable?
- Is there a backup authentication method?
- How are credentials managed?
- What is the session management strategy?

**Source Reference:** [SPE_20190806_ISWC_Portal.md](../../../resources/core_design_documents/SPE_20190806_ISWC_Portal/SPE_20190806_ISWC_Portal.md)

**Related:** [web-portals.md](web-portals.md) documents the integration but not the service itself

---

### UC-009: Application Insights (Monitoring & Telemetry)

| Field | Value |
|-------|-------|
| **Priority** | 💡 Low |
| **Component Type** | Monitoring & Observability |
| **Technology** | Azure Application Insights |
| **Purpose** | Centralized logging, telemetry, performance monitoring, alerting |
| **Why Important** | Operational visibility; Performance baselining; Alert configuration; Cost tracking; Integration with Azure Monitor |
| **Effort Estimate** | 1 day |

**Key Questions:**

- What telemetry is collected?
- What alerts are configured?
- What dashboards exist?
- What is the log retention period?
- What is the monthly cost?
- Are there performance baselines defined?

**Source Reference:** Standard Azure service, configuration in each component

**Related:** Multiple components reference Application Insights integration

---

### UC-010: Azure Key Vault (Secrets Management)

| Field | Value |
|-------|-------|
| **Status** | 💡 Low |
| **Component Type** | Secrets Management |
| **Technology** | Azure Key Vault |
| **Purpose** | Centralized storage for connection strings, API keys, credentials |
| **Why Important** | Security best practice; Credential rotation strategy; Access control (RBAC); Audit trail for secret access |
| **Effort Estimate** | 1 day |

**Key Questions:**

- What secrets are stored?
- How are secrets rotated?
- Who has access to Key Vault?
- What is the backup/disaster recovery strategy?
- Are there access alerts configured?

**Source Reference:** Standard Azure service, referenced across multiple components

**Related:** All components that require credentials reference Key Vault

---

## Documentation Roadmap

### Phase 1: Critical Components (3-5 days)

**Priority:** Core system functionality

1. **UC-002: SFTP Server** (1-2 days)
   - Focus on folder structure, authentication, monitoring

2. **UC-003: Azure Data Factory** (2-3 days)
   - Document all pipelines, triggers, error handling

---

### Phase 2: High-Priority Dependencies (5-7 days)

**Priority:** Vendor dependencies and data sources

3. **UC-004: IPI WebJob** (1-2 days)
   - Incremental update mechanism
   - API integration with SUISA

4. **UC-005: SQL Server** (2-3 days)
   - Database schema (ERD)
   - Performance characteristics
   - Backup strategy

5. **UC-006: Matching Engine** (1-2 days)
   - Vendor service documentation
   - SLA requirements
   - Disaster recovery

---

### Phase 3: Supporting Components (4 days)

**Priority:** External dependencies and infrastructure

6. **UC-007: IPI Database** (1 day)
   - External data source
   - Synchronization strategy

7. **UC-008: FastTrack SSO** (1 day)
   - Authentication service
   - SLA and fallback

8. **UC-009: Application Insights** (1 day)
   - Monitoring configuration
   - Alert definitions

9. **UC-010: Azure Key Vault** (1 day)
    - Secrets inventory
    - Rotation strategy

---

## Total Estimated Effort

**Note:** All effort estimates are **[Experience-based estimates]** from the audit team and will be refined as documentation work progresses.

- **Phase 1 (Critical):** 3-5 days
- **Phase 2 (High):** 5-7 days
- **Phase 3 (Supporting):** 4 days
- **Total:** 12-16 days (reduced from 13-18 days due to Public Portal now documented)

---

## Next Steps

1. **Prioritize with CISAC:** Confirm documentation priorities based on audit goals
2. **Gather Access:** Ensure access to Azure Portal, Data Factory, Key Vault, etc.
3. **Use `/document-component` Command:** Leverage existing command for consistent structure
4. **Cross-Reference:** Link new documents to existing component documentation
5. **Update Index:** Add new components to [index.md](index.md) with status indicators

---

## Component Status Legend

| Icon | Status | Description |
|------|--------|-------------|
| ✅ | Documented | C4 Level 3 documentation complete |
| 🔄 | In Progress | Documentation underway |
| ⏳ | Planned | Scheduled for documentation |
| ❌ | Undocumented | No comprehensive documentation exists |

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-10-29 | Audit Team | Initial list of 10 undocumented components; Prioritization matrix; Documentation roadmap with effort estimates |
| 2.0 | 2025-10-29 | Audit Team | **UC-001 COMPLETED:** Removed UC-001 (Public Portal) - now documented in web-portals.md with Agency Portal; Updated component count from 10 to 9; Updated total effort from 13-18 days to 12-16 days; Updated agency-portal.md references to web-portals.md; Updated Phase 1 effort from 5-8 days to 3-5 days |

---

**Next Review:** After Phase 1 completion

**Owner:** CISAC Audit Team + Spanish Point Documentation Team
