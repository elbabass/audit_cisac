# Questions Tracker - ISWC Platform Components

**Version:** 1.0
**Date:** October 29, 2025
**Total Questions:** 242 questions across 9 component documents

**Purpose:** Consolidated list of all open questions from component documentation to guide further investigation during the CISAC ISWC audit.

---

## How to Use This Document

- **Status:** ❓ = Unanswered | ✅ = Answered | 🔄 = Investigation in progress
- **Priority:** 🔴 = Critical | ⚠️ = High | 📝 = Medium | 💡 = Low
- **Links:** Click document names to jump to source section

---

## Questions by Category

### 1. Performance and Scalability (45 questions)

#### Pipeline Performance

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What's the p50/p95/p99 validation time? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | 🔴 | How does validation scale with batch size? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What's the largest batch size in production? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Are there batch size limits? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | 🔴 | What's the guaranteed uptime for Matching Engine? (99%, 99.9%, 99.99%?) | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |
| ❓ | 🔴 | What's the Matching Engine response time SLA? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What's the maximum works per Matching Engine request? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the expected frequency of concurrency conflicts? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | 🔴 | Can AddWorkInfoAsync return IswcModel directly to reduce round trips? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the expected batch size and throughput? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | 🔴 | What is the p99 latency for end-to-end pipeline execution? | [pipeline-architecture-overview](pipeline-architecture-overview.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Can reflection-discovered components be cached? | [pipeline-architecture-overview](pipeline-architecture-overview.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Can database queries be batched or cached across pipeline stages? | [pipeline-architecture-overview](pipeline-architecture-overview.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Can rule discovery be cached at startup instead of per-batch? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Can database queries be batched across multiple rules? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |
| ❓ | 📝 | What is the p99 latency for rule execution? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |

#### Portal Performance

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | Are there SLAs for Portal response times? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 🔴 | What are the p50/p95/p99 latencies for search endpoints? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 🔴 | What are the p50/p95/p99 latencies for submission endpoints? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | ⚠️ | How many concurrent users does Portal support? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Are there rate limits for Portal API calls? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the actual bundle size after webpack build? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 📝 | What is the average database query time for search operations? | [web-portals](web-portals.md#questions-for-further-investigation) |

#### Databricks Performance

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What is the average IPI full resynch duration? (Expected: hours) | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | 🔴 | What is the p50/p95/p99 for agency file processing? (Expected: seconds to minutes) | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the average file processing time (small, medium, large files)? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What percentage of files fail validation? (Error rate) | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Are there processing SLAs for agency file turnaround? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the average cluster utilization percentage? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | How much time is spent in cold start vs warm execution? | [databricks](databricks.md#questions-for-further-investigation) |

#### Cosmos DB Performance

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What is the current RU (Request Units) provisioning in production vs initial 1,000 RU? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | 🔴 | Have there been any RU throttling events (HTTP 429 errors)? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Is auto-scaling enabled for RU throughput? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the average RU consumption per hour/day? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What are the slowest queries and their RU costs? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | 📝 | How many documents are in each collection? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |

#### Cache Performance

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | ⚠️ | What's the actual cache hit rate for GetCacheIswcs? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | Cache warming strategy? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | Cache invalidation policy? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Can work lookups be cached from ProcessingPipeline? | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |

---

### 2. Cost and Resource Optimization (24 questions)

#### Databricks Cost

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What is the monthly Databricks cost breakdown (compute vs DBU)? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | 🔴 | What percentage of monthly cost is wasted on idle clusters? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | 🔴 | What would be the cost savings from runtime upgrade? (20-40% potential) | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the cost per job (DBU consumption attribution)? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Is Photon engine enabled? (Requires newer runtime + additional cost) | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | 📝 | Are Spot instances used for non-critical workloads? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What cluster sizing (nodes, cores, memory) is configured? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the auto-scaling configuration (min/max nodes)? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | How much idle time occurs (cluster running but not processing)? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the cluster idle timeout setting? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | 📝 | Are there separate clusters for different workloads (IPI vs SFTP)? | [databricks](databricks.md#questions-for-further-investigation) |

#### Cosmos DB Cost

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What is the monthly Cosmos DB cost breakdown (RU vs storage vs backup)? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Are there partition hotspots (uneven RU distribution)? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the indexing policy configuration? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Are there unused indexes consuming RU? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Could TTL reduce storage costs? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | 📝 | What is the actual data volume in production Cosmos DB? (Expected: 300-500GB) | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |

#### General Cost

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What is the monthly €50K cloud cost breakdown by service? | Multiple sources |
| ❓ | ⚠️ | Are there cost alerts configured? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What monitoring alerts exist for Portal performance issues? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 📝 | What is the average number of workflow tasks per agency per day? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 📝 | What percentage of submissions result in workflow tasks? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 📝 | What is the average file size distribution? | [databricks](databricks.md#questions-for-further-investigation) |

---

### 3. Security and Authentication (14 questions)

#### FastTrack SSO

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What happens if FastTrack SSO is unavailable? (No fallback documented) | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 🔴 | Who operates FastTrack SSO (CISAC or third-party vendor)? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 🔴 | What is FastTrack SSO uptime SLA? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Is there a backup authentication method for critical operations? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | ⚠️ | How are JWT tokens issued and validated? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 📝 | What is the token expiry duration? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 📝 | Are there session timeout warnings for users? | [web-portals](web-portals.md#questions-for-further-investigation) |

#### Permission and Authorization

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | ⚠️ | What permission model is used for multi-user agencies? (Spec mentions "wish list" item) | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the `CommonIPs.PublicDomainIps` list? (Static configuration? Database table?) | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Why 80 years for public domain? (Standard copyright term? Configurable?) | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | How does `interestedPartyManager.IsAuthoritative()` work? | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | How are Cosmos DB connection strings rotated and managed? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |

#### Access Control

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 📝 | How is death date populated for IPs? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | Is public domain rule configurable per country? (EU: 70 years, US: different rules) | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |

---

### 4. Data Integrity and Transaction Management (26 questions)

#### ISWC Lifecycle

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What is the ISWC generation algorithm? (Format, check digit calculation, uniqueness enforcement) | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | 🔴 | How are ISWC collisions handled? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | 🔴 | What is IswcStatus=1 vs IswcStatus=2? (Active vs Allocated? Validated vs Pending?) | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | How are ISWC merge chains resolved (A→B→C)? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What happens to works linked to deprecated ISWCs? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | When is the `IsReplaced` flag set? (During MER transaction? Post-processing?) | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | How are ISWC merge chains validated recursively (A→B→C)? | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Can ISWCs become replaced between validation checks? | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |

#### Transaction Boundaries

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What is the Entity Framework Core transaction scope? (Per-request? Per-operation?) | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | 🔴 | Are there explicit `TransactionScope` declarations in the API layer? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | 🔴 | Should the system use distributed transactions (saga pattern) or accept eventual consistency? | [pipeline-architecture-overview](pipeline-architecture-overview.md#questions-for-further-investigation) |
| ❓ | ⚠️ | How frequently do concurrent requests target the same work? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Are there database-level locks or only EF optimistic concurrency? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | How are concurrent edits to the same work handled? | [web-portals](web-portals.md#questions-for-further-investigation) |

#### Merge and Parent-Child Logic

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What is the data structure of the IswcLinkedTo table? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | 🔴 | How are ISWC merge conflicts resolved? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Can merge operations be rolled back? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the impact of merging ISWCs on downstream systems? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | How are parent-child relationships established? (MultipleAgencyWorkCodesChild flag) | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | How is batch ordering enforced for parent-child dependencies? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What happens if parent fails but child succeeds? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Can circular dependencies occur? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Can merge target validation be batched into single query? | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |

#### Data Synchronization

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | ⚠️ | How is the IPI database replica synchronized? (Frequency, mechanism) | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the replication lag between IPI source and replica? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 📝 | How does `GetCacheIswcs()` relate to the ISWC cache in SearchComponent? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |

---

### 5. Configuration and Rules Management (28 questions)

#### Rule Configuration

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | Where are rule parameters stored? (Cosmos DB collection name?) | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | 🔴 | Who manages rule configuration? (Portal UI or manual?) | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | 🔴 | How are configuration changes deployed? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Is there version control for configuration? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What are the default values for all configurable parameters? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Can rule parameters be changed at runtime or require deployment? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Is there a UI for managing rule configuration? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |
| ❓ | 📝 | How is rule configuration versioned? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |

#### Rule Logic and Ordering

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What specific rules set `ToBeProcessed = true`? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What's the typical iteration count in production? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Are there cases requiring 5+ iterations? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | Should there be a max iteration limit? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | Is alphanumeric ordering intentional? (IV_02 before IV_05) | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | Are there rule dependencies? (Rule A must execute before Rule B?) | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | Can rules be reordered dynamically? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |

#### Configuration Parameters

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | ⚠️ | What are the excluded title types in `ExcludeTitleTypes`? | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is `EnablePVTitleStandardization` default value? | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is `ValidateDisambiguationISWCs` default value? | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Can rule parameters be changed at runtime or require deployment? | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is `IncludeAgenciesInEligibilityCheck` rule parameter? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | How are partition keys chosen for new collections beyond Audit? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |

#### Match Configuration

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | ⚠️ | How is IsDefinite flag determined? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What RankScore threshold defines "definite"? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | Can this be configured per agency? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |

#### Databricks Configuration

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | ⚠️ | What is the exact Databricks runtime version currently deployed? (Expected: 11.3 LTS) | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the upgrade path and breaking changes for latest runtime? (11.3 → 15.4 LTS) | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | 🔴 | Why hasn't Databricks runtime been updated despite maintenance contract? (Contractual dispute) | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What features are blocked by outdated runtime? (Photon, Unity Catalog, AI queries) | [databricks](databricks.md#questions-for-further-investigation) |

---

### 6. Error Handling and Monitoring (30 questions)

#### Error Codes and Messages

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What error codes are `_180` and `_181` (observed in ErrorCode enum)? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |
| ❓ | 🔴 | Should the system collect all validation errors instead of short-circuiting? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |
| ❓ | 🔴 | Can error codes be more granular (SQL errors, network errors, etc.)? | [pipeline-architecture-overview](pipeline-architecture-overview.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Is _100 generic error sufficient for production diagnostics? | [pipeline-architecture-overview](pipeline-architecture-overview.md#questions-for-further-investigation) |
| ❓ | 📝 | Can error messages be localized for different agencies? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |
| ❓ | 📝 | Are error messages localized? (French, Spanish, etc.) | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | How is locale determined? (HTTP header? User preference?) | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |

#### Matching Engine Error Handling

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What's the disaster recovery plan for Matching Engine? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |
| ❓ | 🔴 | What happens if Matching Engine returns partial results? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | How are transient vs. permanent errors distinguished? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Is there telemetry for failure rates? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |

#### Monitoring and Telemetry

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | Are there monitoring dashboards for rule execution times? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |
| ❓ | 🔴 | Can rule execution be traced in Application Insights/logging? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |
| ❓ | 🔴 | Is RulesApplied persisted to database or only in API response? | [pipeline-architecture-overview](pipeline-architecture-overview.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What error rate exists for file processing (% failures)? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What monitoring/alerts exist for Databricks job failures? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Is there a dead-letter queue for unparseable files? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the log retention period for Databricks cluster logs? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Are there dashboards for job duration trends and resource utilization? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the incident response process for Databricks job failures? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What monitoring/alerting exists for Cosmos DB performance and costs? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |

#### Operational Procedures

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | ⚠️ | How often is Portal deployed to production? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the rollback procedure for failed deployments? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 📝 | Are there A/B testing capabilities? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 📝 | What is the incident response process for Portal outages? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 📝 | Who has production access for troubleshooting? | [web-portals](web-portals.md#questions-for-further-investigation) |

#### Audit Trail

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | ⚠️ | What is the data retention policy for audit trail? | [pipeline-architecture-overview](pipeline-architecture-overview.md#questions-for-further-investigation) |
| ❓ | 📝 | Are there monitoring dashboards for rule performance? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |
| ❓ | 📝 | What is the expected frequency of validation failures? | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |

---

### 7. Business Logic and Workflows (34 questions)

#### Disambiguation Logic

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | When is the `Disambiguation` flag set? (Frontend? ValidationPipeline?) | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |
| ❓ | 🔴 | How does disambiguation affect ISWC assignment in ProcessingPipeline? | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What happens to `DisambiguateFrom` data after validation? | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Why two disambiguation rules? (PV_30 always-on + IV_40 configurable) | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | Why is IV_40 in PostMatchingValidator instead of StaticDataValidator? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |

#### Label Submission Logic

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What is "PRS" prefix? (Agency? Work type? Temporary identifier?) | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | 🔴 | What does `UpdateLabelSubmissionsAsync()` do? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Why is this only triggered for single CAR submissions? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | How does Label request handling differ from Agency/Publisher requests? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |

#### Approval Workflow

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What is the approval workflow (`AddUpdateApprovalWorkflow`)? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the time limit for automatic workflow approval? (Spec says ~30 days - need confirmation) | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 📝 | Are there bulk approval operations for workflows? (Spec mentions "Approve All") | [web-portals](web-portals.md#questions-for-further-investigation) |

#### Matching Strategies

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | ⚠️ | When does ISRC matching execute vs. title matching? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Priority order if both match? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Conflict resolution if ISRCs differ? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | How often does fallback execute in production? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | What percentage of submissions have no "Eligible" matches? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | Performance impact measurement? | [matching-pipeline](matching-pipeline.md#questions-for-further-investigation) |

#### Assignment Strategy Logic

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What is the logic in `interestedPartyManager.IsAuthoritative()`? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | How does authoritative flag affect ISWC eligibility? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Can authoritative flags change over time? | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | What is `UpdateAllocatedIswc` flag? | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |

#### Agency Agreements

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | ⚠️ | What agreement fields affect eligibility? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Can agreements change mid-processing? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | How are agreement conflicts resolved? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |

#### Batch Rules

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 📝 | Why were batch rules designed but not implemented? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | What scenarios require batch-level validation? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | Should batch rules be removed or implemented? | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |

#### Usage Patterns

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | ⚠️ | How many agencies actively use Portal vs API vs SFTP? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the Portal vs API vs SFTP submission volume breakdown? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 📝 | What are the peak usage hours/days for Portal? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 📝 | What is the most frequently used Portal feature? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 📝 | What is the average session duration? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 📝 | What reports are most frequently generated? | [web-portals](web-portals.md#questions-for-further-investigation) |

---

### 8. Database Schema and Data Model (17 questions)

#### Schema Design

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What is the complete database schema? (ERD diagram) | [pipeline-architecture-overview](pipeline-architecture-overview.md#questions-for-further-investigation) |
| ❓ | ⚠️ | How are database indexes optimized for pipeline queries? | [pipeline-architecture-overview](pipeline-architecture-overview.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Are there database indexes on search columns (Title, ISWC, WorkCode)? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Are there database-level optimizations (query plan caching, indexes)? | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |

#### Data Structures

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What is the `DetailLevel` enum? (Full, Summary, Minimal?) | [processing-pipeline](processing-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | What is the exact 80-year calculation? (Date of death + 80 years?) | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |
| ❓ | 📝 | How are edge cases handled? (Unknown death date?) | [validation-pipeline](validation-pipeline.md#questions-for-further-investigation) |

#### Unread Rules and Missing Logic

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | ⚠️ | What do PV_11, PV_12, PV_13, PV_14 validate? (CUR consistency, CDR permissions) | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What do PV_22, PV_23, PV_24, PV_25, PV_26 validate? (CUR/CDR additional checks) | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What do PV_29, PV_31, PV_33, PV_34 validate? (Unknown transaction types) | [post-matching-pipeline](post-matching-pipeline.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What are the complete implementations of IV_30 through IV_39? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What are the complete implementations of PV_11-14, PV_22-26, PV_29, PV_31, PV_33, PV_34? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |
| ❓ | 📝 | What is the logic for "ToBeProcessed" flag? Which rules set it? | [validation-rules-catalog](validation-rules-catalog.md#questions-for-further-investigation) |

#### Features

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 📝 | Is there an export feature for bulk ISWC lists? | [web-portals](web-portals.md#questions-for-further-investigation) |
| ❓ | 📝 | Can users download submission history as CSV/Excel? | [web-portals](web-portals.md#questions-for-further-investigation) |

---

### 9. Development and Testing (16 questions)

#### Development Environment

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | ⚠️ | Is there a local Cosmos DB emulator configured for development? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | ⚠️ | How is notebook versioning managed (Git integration)? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Is there a test/dev Databricks workspace? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | 📝 | How many notebooks exist in the workspace? | [databricks](databricks.md#questions-for-further-investigation) |

#### Code Quality

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | ⚠️ | What is the notebook code quality (complexity, modularity, testability)? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Are there unit tests for notebook transformations? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | 📝 | What is the code review process for notebook changes? | [databricks](databricks.md#questions-for-further-investigation) |

#### Specifications and Documentation

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | Where is the SFTP file processing specification? (80% of workload undocumented) | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | 📝 | What is the data volume processed per day/week/month? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | 📝 | How often is IPI full resynch actually executed? (Quarterly dumps available but processing frequency unclear) | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | 📝 | What is the Delta Lake storage growth rate? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | 📝 | Are VACUUM operations scheduled for Delta Lake? | [databricks](databricks.md#questions-for-further-investigation) |

#### Platform Upgrades

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | When does vendor support end for Runtime 11.3 LTS? (Expected: September 2025 - already expired) | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | 🔴 | What is the timeline for runtime upgrade? | [databricks](databricks.md#questions-for-further-investigation) |
| ❓ | ⚠️ | What is the maximum acceptable downtime for IPI resynch? | [databricks](databricks.md#questions-for-further-investigation) |

---

### 10. Backup and Disaster Recovery (8 questions)

#### Backup Procedures

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What is the backup restoration procedure and has it been tested? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | 🔴 | How long does backup restoration take? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | 🔴 | Is there a long-term archival strategy for audit data beyond 8 hours? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Are there any retention policies for old audit data? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |

#### Recovery Objectives

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What is the disaster recovery RTO/RPO for Cosmos DB? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |

#### Data Governance

| Status | Priority | Question | Source |
|--------|----------|----------|--------|
| ❓ | 🔴 | What is the data retention policy for audit logs (GDPR compliance)? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | 🔴 | How is "right to be forgotten" handled for Cosmos DB audit data? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |
| ❓ | ⚠️ | Are there compliance requirements for audit data retention? | [cosmos-db](cosmos-db.md#questions-for-further-investigation) |

---

## Summary Statistics

**Total Questions:** 242
**By Priority:**

- 🔴 Critical: 66 questions (27%)
- ⚠️ High: 98 questions (40%)
- 📝 Medium/Low: 78 questions (33%)

**By Category:**

- Performance and Scalability: 45 questions (19%)
- Cost and Resource Optimization: 24 questions (10%)
- Security and Authentication: 14 questions (6%)
- Data Integrity and Transaction Management: 26 questions (11%)
- Configuration and Rules Management: 28 questions (12%)
- Error Handling and Monitoring: 30 questions (12%)
- Business Logic and Workflows: 34 questions (14%)
- Database Schema and Data Model: 17 questions (7%)
- Development and Testing: 16 questions (7%)
- Backup and Disaster Recovery: 8 questions (3%)

**Documents with Most Questions:**

1. databricks.md: 45 questions
2. processing-pipeline.md: 36 questions
3. post-matching-pipeline.md: 36 questions
4. web-portals.md: 32 questions
5. cosmos-db.md: 25 questions

---

## Next Steps

1. **Prioritize Critical Questions (🔴)** - Focus on 66 critical questions first
2. **Schedule Investigation Sessions** - Group by category for efficient research
3. **Update Status** - Mark questions as ✅ when answered or 🔄 when in progress
4. **Document Answers** - Add findings back to source documents
5. **Cross-Reference** - Many questions span multiple components (e.g., transaction boundaries)

---

**Document History:**

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-10-29 | Audit Team | Initial consolidated questions tracker; 242 questions extracted from 9 component documents; Organized into 10 categories with priority levels and source links |
