# ISWC Platform Components

This directory contains Level 3 component documentation for containers within the ISWC Platform system.

---

## 📊 Audit Tracking Documents

**Comprehensive tracking for open questions, technical debt, and documentation gaps:**

- **[Questions Tracker](questions-tracker.md)** - 242 open questions across all components (organized by 10 categories)
- **[Technical Debt Tracker](technical-debt-tracker.md)** - 67 technical debt items with priorities, mitigation strategies, and roadmap
- **[Undocumented Components](undocumented-components.md)** - 10 components requiring C4 Level 3 documentation

### Quick Links to Critical Issues

**🔴 Critical Technical Debt (17 items):**

- [TD-012: ASP.NET Core 3.1 EOL](technical-debt-tracker.md#td-012-aspnet-core-31-end-of-life-december-2022) - No security patches since Dec 2022
- [TD-013: React 16.12.0 Outdated](technical-debt-tracker.md#td-013-react-16120-severely-outdated-released-december-2019) - 5+ years behind
- [TD-015: Databricks Runtime 11.3 LTS](technical-debt-tracker.md#td-015-outdated-databricks-runtime-113-lts-4-versions-behind) - 4 major versions behind (contractual dispute)
- [TD-016: Cosmos DB 8-Hour Backup Window](technical-debt-tracker.md#td-016-cosmos-db-very-limited-backup-retention-8-hour-window) - Critical for audit compliance
- [TD-004: Matching Engine SPOF](technical-debt-tracker.md#td-004-single-point-of-failure---matching-engine-vendor-dependency) - No fallback strategy

[View All Critical Items →](technical-debt-tracker.md#-critical-priority-17-items)

---

## Component Status Legend

| Icon | Status | Description |
|------|--------|-------------|
| ✅ | C4 Level 3 | Complete component documentation with architecture, performance, technical debt sections |
| 🔄 | Legacy | Documentation exists but needs upgrade to C4 Level 3 structure |
| ⏳ | Undocumented | No comprehensive component documentation |

---

## Pipeline Components (Submission Processing)

**Core business logic for work submission validation, matching, and ISWC assignment:**

- ✅ [Validation Pipeline](validation-pipeline.md) - 73+ validation rules across 4 validators (v1.0)
- ✅ [Matching Pipeline](matching-pipeline.md) - Work matching via Spanish Point Matching Engine (v1.0)
- ✅ [Processing Pipeline](processing-pipeline.md) - ISWC assignment with 13 strategies (v1.0)
- ✅ [Post-Matching Pipeline](post-matching-pipeline.md) - 22 post-validation rules (v1.0)
- ✅ [Pipeline Architecture Overview](pipeline-architecture-overview.md) - End-to-end architecture synthesis (v1.0)
- ✅ [Validation Rules Catalog](validation-rules-catalog.md) - Comprehensive rules reference (v1.0)

**Status:** 6/6 components fully documented (100%)

---

## Web Applications

- ✅ [Web Portals (Agency + Public)](web-portals.md) - Dual-mode web portals with shared codebase (v4.0, restructured Oct 29)
  - Agency Portal: Authenticated interface for societies to manage work registrations
  - Public Portal: Anonymous public search interface (iswcnet.cisac.org replacement)
  - Architecture: 99% code reuse, environment-variable driven activation

**Status:** 1/1 components documented (100%)

---

## APIs

- ⏳ **Agency API** - REST API for agency work submissions
- ⏳ **Label API** - REST API for label submissions
- ⏳ **Publisher API** - REST API for publisher submissions
- ⏳ **Third Party API** - REST API for external integrations

**Status:** 0/4 components documented (0%)

**Note:** API documentation lower priority as they delegate to shared pipeline components (already documented)

---

## Background Processing

- ✅ [Databricks](databricks.md) - Big data processing workspace for file processing (v2.0, upgraded Oct 29)
- ⏳ **Azure Data Factory** - ETL pipeline orchestration (triggers Databricks, file archival)
  - [See undocumented-components.md: UC-003](undocumented-components.md#uc-003-azure-data-factory)
  - **Priority:** 🔴 Critical
  - Estimated effort: 2-3 days
- ⏳ **IPI WebJob** - Incremental IPI updates via SUISA API
  - [See undocumented-components.md: UC-004](undocumented-components.md#uc-004-ipi-webjob-incremental-updates)
  - **Priority:** ⚠️ High
  - Estimated effort: 1-2 days
- ⏳ **ISWC Jobs** - Azure Functions for scheduled background jobs

**Status:** 1/4 components documented (25%)

---

## Data Storage

- ✅ [Cosmos DB](cosmos-db.md) - NoSQL document database for audit logs and ISWC cache (v2.0, upgraded Oct 29)
- ⏳ **SQL Server** - Azure SQL databases (ISWC + IPI metadata)
  - [See undocumented-components.md: UC-005](undocumented-components.md#uc-005-sql-server-azure-sql-database)
  - **Priority:** ⚠️ High
  - Estimated effort: 2-3 days
- ⏳ **Data Lake** - Azure Data Lake Storage Gen2 for file storage
- ⏳ **SFTP Server** - Agency file upload/download
  - [See undocumented-components.md: UC-002](undocumented-components.md#uc-002-sftp-server)
  - **Priority:** 🔴 Critical
  - Estimated effort: 1-2 days

**Status:** 1/4 components documented (25%)

---

## External Dependencies (Vendor Services)

- ⏳ **Matching Engine** - Spanish Point proprietary matching service
  - [See undocumented-components.md: UC-006](undocumented-components.md#uc-006-matching-engine-spanish-point-vendor-dependency)
  - **Priority:** ⚠️ High (Critical vendor dependency - SPOF)
  - **Technical Debt:** [TD-004: No fallback strategy](technical-debt-tracker.md#td-004-single-point-of-failure---matching-engine-vendor-dependency)
  - Estimated effort: 1-2 days
- ⏳ **FastTrack SSO** - CIS-Net authentication service
  - [See undocumented-components.md: UC-008](undocumented-components.md#uc-008-fasttrack-sso-cis-net-authentication)
  - **Priority:** 📝 Medium
  - **Technical Debt:** [TD-014: No fallback authentication](technical-debt-tracker.md#td-014-fasttrack-sso-single-point-of-failure)
  - Estimated effort: 1 day
- ⏳ **IPI Database** - SUISA-maintained Interested Party database
  - [See undocumented-components.md: UC-007](undocumented-components.md#uc-007-ipi-database-suisa-external-source)
  - **Priority:** 📝 Medium
  - Estimated effort: 1 day

**Status:** 0/3 components documented (0%)

---

## Infrastructure & Monitoring

- ⏳ **Application Insights** - Monitoring and telemetry
  - [See undocumented-components.md: UC-009](undocumented-components.md#uc-009-application-insights-monitoring--telemetry)
  - **Priority:** 💡 Low
  - Estimated effort: 1 day
- ⏳ **Azure Key Vault** - Secrets management
  - [See undocumented-components.md: UC-010](undocumented-components.md#uc-010-azure-key-vault-secrets-management)
  - **Priority:** 💡 Low
  - Estimated effort: 1 day

**Status:** 0/2 components documented (0%)

---

## Overall Documentation Status

| Category | Documented | Total | Percentage |
|----------|------------|-------|------------|
| **Pipeline Components** | 6 | 6 | 100% ✅ |
| **Web Applications** | 1 | 1 | 100% ✅ |
| **APIs** | 0 | 4 | 0% |
| **Background Processing** | 1 | 4 | 25% |
| **Data Storage** | 1 | 4 | 25% |
| **External Dependencies** | 0 | 3 | 0% |
| **Infrastructure** | 0 | 2 | 0% |
| **TOTAL** | **9** | **24** | **38%** |

---

## Related Documentation

- [C4 Level 2: Container View](../../c4-views/level2-containers.md) - Complete container overview
- [C4 Architecture Master](../../c4-architecture-master.md) - Navigation hub
- [DOCUMENTATION_STANDARDS.md](../../../../DOCUMENTATION_STANDARDS.md) - Component documentation methodology

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-10-27 | Audit Team | Initial index with basic component listing |
| 2.0 | 2025-10-29 | Audit Team | **MAJOR UPDATE:** Added tracking documents (questions, technical debt, undocumented components); Added component status indicators (✅ C4 Level 3, 🔄 Legacy, ⏳ Undocumented); Added quick links to critical technical debt; Organized components by category with status percentages; Added priority indicators and effort estimates for undocumented components |

---

**Note:** The ISWC Platform system includes all application, processing, and data containers. There is no separate "Data Platform" system - all data containers (Databricks, Data Factory, databases) are part of the ISWC Platform architecture.

**Last Updated:** October 29, 2025
