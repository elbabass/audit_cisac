# Estimates Validation Tracker

**Purpose:** Track and classify all estimates found in audit documentation to identify which require validation through experimentation, vendor data, or client historical data.

**Last Updated:** 2025-10-30

## Overview

This document catalogs **200+ estimates** found across **37 markdown files** in `docs/work_in_progress/`. Each estimate is classified by its nature to determine validation requirements.

### Classification System

| Classification | Description | Validation Need |
|----------------|-------------|-----------------|
| **Guess/Experience-based** | No justification provided | **HIGH** - Requires experimentation or data collection |
| **Vendor-Provided** | From SpanishPoint proposals | **MEDIUM** - Should verify against alternatives |
| **Experience-Based** | Audit team estimates from experience | **LOW** - Refine as work progresses |
| **Calculated** | Derived from other values | **LOW** - Verify source values only |
| **Source-Documented** | From specs, code, or configs | **NONE** - Already proven |

### Summary Statistics

- **Priority 1 (Needs Validation):** 15+ critical estimates
- **Priority 2 (Should Verify):** 25+ vendor-provided estimates
- **Priority 3 (Refine as Needed):** 100+ experience-based estimates
- **Reference Only:** 60+ source-documented values

---

## Priority 1: Estimates Requiring Validation

These estimates lack justification and should be validated through experimentation, measurements, or external data before being used in recommendations.

### COST-001: Databricks Idle Cluster Waste

**Estimate:** €5,000 - €10,000/month

**Source:** `docs/work_in_progress/architecture/components/iswc-platform/databricks.md:745`

**Context:** Potentially wasted on idle clusters that should auto-terminate

**Classification:** Guess/Experience-based - NO JUSTIFICATION PROVIDED

**Validation Required:**

- [ ] Request actual Databricks billing data from client for past 6 months
- [ ] Analyze cluster utilization metrics from Azure Monitor
- [ ] Calculate actual idle time vs active time for each cluster
- [ ] Cross-reference with job schedules to identify optimization opportunities

**Impact:** High - Could represent €60-120K annual waste if accurate

**Acceptance Criteria:** Variance within ±20% of actual billing analysis

---

### COST-002: Application Insights Monthly Cost

**Estimate:** €50 - €200/month

**Source:** `docs/work_in_progress/architecture/integration-patterns/performance.md:882-884`

**Context:** "Depending on traffic volume, first 5GB/month free"

**Classification:** Guess/Experience-based - NO TRAFFIC DATA PROVIDED

**Validation Required:**

- [ ] Request actual Application Insights billing from client
- [ ] Analyze current ingestion volume (GB/month)
- [ ] Review retention policy settings
- [ ] Validate if within free tier or paid tier

**Impact:** Low - Small cost but important for total cost analysis

**Acceptance Criteria:** Actual cost obtained from Azure billing

---

### COST-003: Cosmos DB Storage Cost

**Estimate:** ~$183/month (breakdown: ~$58 RU + ~$125 storage for 500GB)

**Source:** `docs/work_in_progress/architecture/components/iswc-platform/cosmos-db.md:698-702`

**Context:** Calculated estimate for 822M documents at 1,000 RU provisioned

**Classification:** Calculated estimate - NEEDS VERIFICATION against actual bill

**Validation Required:**

- [ ] Request actual Cosmos DB billing from client
- [ ] Verify actual provisioned RU/s (may not be 1,000)
- [ ] Verify actual storage size (may not be 500GB)
- [ ] Check if using reserved capacity discounts

**Impact:** Medium - Base cost affects optimization recommendations

**Acceptance Criteria:** Actual Azure bill for Cosmos DB service

---

### PERF-001: Matching Engine Response Times

**Estimate:**

- Simple match (1-2 contributors): 500ms - 2s
- Complex match (5+ contributors): 2s - 10s
- Batch match (10 works): 5s - 30s
- Worst case timeout: 80 seconds

**Source:** `docs/work_in_progress/architecture/components/iswc-platform/matching-pipeline.md:1110-1113`

**Context:** Matching Engine external API performance estimates

**Classification:** Experience-based estimate - NO ACTUAL MEASUREMENTS

**Validation Required:**

- [ ] Request performance metrics from SpanishPoint (Matching Engine vendor)
- [ ] Analyze Application Insights data for actual response times
- [ ] Review timeout logs to understand typical vs worst-case scenarios
- [ ] Perform controlled load testing with representative data

**Impact:** CRITICAL - Core to performance analysis and Hyperscale proposal validation

**Acceptance Criteria:**

- P50, P95, P99 response times from production data
- Correlation between work complexity and response time
- Validation that 80-second timeout is appropriate or excessive

---

### PERF-002: Rule Execution Times

**Estimate:**

- Simple rules (IV_05, IV_06): 1-5ms
- Database lookup rules (IV_24): 10-50ms (cached after first)
- Eligibility rules (EL_01): 20-100ms (agreement database query)

**Source:** `docs/work_in_progress/architecture/components/iswc-platform/validation-pipeline.md:1154-1157`

**Context:** Validation rule execution performance

**Classification:** Experience-based estimate - NO MEASUREMENTS

**Validation Required:**

- [ ] Add instrumentation to validation pipeline to measure actual rule execution
- [ ] Collect metrics for each rule type over representative workload
- [ ] Analyze cache effectiveness for database lookup rules
- [ ] Identify slowest rules and optimization opportunities

**Impact:** Medium - Affects pipeline performance optimization priorities

**Acceptance Criteria:** Actual execution time distribution per rule from instrumentation

---

### PERF-003: Cache Hit Rates

**Estimate:**

- CMQ queries (frequent ISWCs): 70-80%
- CIQ queries (agency work codes): 30-40%

**Source:** `docs/work_in_progress/architecture/components/iswc-platform/matching-pipeline.md:1152-1153`

**Context:** Expected cache effectiveness for work lookups

**Classification:** Experience-based estimate - NO ACTUAL DATA

**Validation Required:**

- [ ] Extract cache hit/miss metrics from application logs
- [ ] Analyze query patterns from Application Insights
- [ ] Calculate actual hit rates per query type
- [ ] Validate that caching strategy is effective

**Impact:** Medium - Affects performance optimization recommendations

**Acceptance Criteria:** Actual cache hit rates from production logs within ±10%

---

### PERF-004: Cache Performance Estimates

**Estimate:**

- Cache hit: ~1ms
- Cache miss: ~50-200ms (SQL query)
- Reference data first query: ~200ms (database)
- Reference data subsequent: ~1ms (in-memory)
- Lookup data first query: ~50ms (Cosmos DB)
- Lookup data subsequent: ~1ms (cached)

**Sources:**

- `matching-pipeline.md:1157-1158`
- `validation-pipeline.md:1210-1211, 1224-1225`

**Classification:** Experience-based estimates - NO MEASUREMENTS

**Validation Required:**

- [ ] Instrument caching layer to measure actual hit/miss latencies
- [ ] Measure SQL query response times under load
- [ ] Measure Cosmos DB query response times
- [ ] Validate cache warming effectiveness

**Impact:** Medium - Cache optimization is recommended strategy

**Acceptance Criteria:** Actual latency measurements within ±50% of estimates

---

### PERF-005: Reflection Overhead

**Estimate:**

- Assembly scan: ~50-100ms (first call)
- Type filtering: ~10-20ms
- Instance creation: ~5-10ms per component
- **Total per batch: ~150-200ms**

**Sources:**

- `matching-pipeline.md:1179-1182`
- `validation-pipeline.md:1413`

**Classification:** Experience-based estimate - NO PROFILING DATA

**Validation Required:**

- [ ] Profile application startup and component discovery
- [ ] Measure actual reflection overhead using .NET diagnostics
- [ ] Validate that lazy loading would provide benefit
- [ ] Compare against alternative dependency injection approaches

**Impact:** Low - Small optimization but easy to measure

**Acceptance Criteria:** Profiler results showing actual reflection costs

---

### PERF-006: Early Rejection Savings

**Estimate:** 70-90% reduction in rule execution for invalid submissions

**Source:** `docs/work_in_progress/architecture/components/iswc-platform/validation-pipeline.md:1179`

**Context:** Performance benefit of implementing batch rejection rules

**Classification:** Experience-based estimate - NO DATA

**Validation Required:**

- [ ] Analyze historical submission data for rejection rates
- [ ] Calculate potential savings based on actual invalid submission %
- [ ] Measure current pipeline processing time distribution
- [ ] Model impact of early rejection implementation

**Impact:** High - Recommended optimization strategy

**Acceptance Criteria:** Actual rejection rate from historical data validates estimate

---

### SCALE-001: Frontend Bundle Size

**Estimate:** Total estimated bundle: ~800KB - 1MB

- React 16.12.0 + Dependencies: ~450KB (minified + gzipped)
- Redux + Middleware: ~50KB
- Bootstrap + Reactstrap: ~100KB
- Chart.js: ~180KB

**Source:** `docs/work_in_progress/architecture/components/iswc-platform/web-portals.md:1105-1109`

**Context:** Estimated frontend bundle size affecting load performance

**Classification:** Calculated estimate - REQUIRES BUILD ANALYSIS CONFIRMATION

**Validation Required:**

- [ ] Run webpack bundle analyzer on actual build
- [ ] Measure actual bundle sizes (vendor, main, chunks)
- [ ] Analyze code splitting opportunities
- [ ] Measure actual page load times in production

**Impact:** Medium - Affects modernization recommendations

**Acceptance Criteria:** Actual webpack build output and bundle analysis report

---

### SCALE-002: Cosmos DB Storage Size

**Estimate:** 822M+ documents = ~300-500GB estimated

**Source:** `docs/work_in_progress/architecture/components/iswc-platform/cosmos-db.md:687, 833`

**Context:** Calculated from document count, but size per document unknown

**Classification:** Calculated estimate - NEEDS VERIFICATION

**Validation Required:**

- [ ] Query Cosmos DB for actual storage metrics
- [ ] Calculate average document size
- [ ] Validate 300-500GB estimate against actual usage
- [ ] Review growth rate for capacity planning

**Impact:** Medium - Affects storage cost calculations

**Acceptance Criteria:** Actual Cosmos DB storage metrics from Azure Portal

---

### SCALE-003: RU Consumption Estimates

**Estimate:**

- Write operations: 5-10 RU per audit document
- Read operations: 1-5 RU (simple lookups)
- Complex queries: 10-100+ RU

**Source:** `docs/work_in_progress/architecture/components/iswc-platform/cosmos-db.md:651-653`

**Classification:** Experience-based estimate - NO ACTUAL METRICS

**Validation Required:**

- [ ] Enable Cosmos DB query metrics
- [ ] Analyze actual RU consumption per operation type
- [ ] Review most expensive queries
- [ ] Validate if 1,000 RU provisioned is sufficient or over-provisioned

**Impact:** Medium - Affects cost optimization recommendations

**Acceptance Criteria:** Actual RU consumption metrics from Cosmos DB diagnostics

---

### SCALE-004: Cosmos DB Hyperscale Proposal (CRITICAL)

**Estimate:** 100,000 - 220,000 RU/s proposed (vs 1,000 RU/s original spec)

**Scale Increase:** **100-220x**

**Source:** `docs/work_in_progress/architecture/integration-patterns/performance.md:441, 447`

**Context:** SpanishPoint Hyperscale proposal includes massive Cosmos DB scaling with NO JUSTIFICATION

**Classification:** Vendor-provided - UNEXPLAINED INCREASE 🚨

**Validation Required:**

- [ ] **CRITICAL:** Request justification from SpanishPoint for 100x increase
- [ ] Analyze actual Cosmos DB throughput requirements
- [ ] Review current RU throttling incidents (if any)
- [ ] Calculate actual required RU based on workload analysis
- [ ] Challenge vendor to provide performance modeling

**Impact:** CRITICAL - This is a **€40-50K+ annual cost increase** with no explanation

**Recommendation:** **DO NOT APPROVE** this scaling without:

1. Documented justification from vendor
2. Performance baseline showing current RU consumption
3. Load testing demonstrating requirement
4. Independent validation of sizing

**Acceptance Criteria:**

- Vendor provides detailed performance model
- Actual current RU consumption analyzed
- Load testing validates requirement
- Independent review confirms sizing

---

## Priority 2: Vendor-Provided Estimates (Should Verify)

These estimates come from SpanishPoint proposals. While they have a source, they should be verified against alternatives and market rates.

### VEN-001: Infrastructure Proposal Total Cost

**Estimate:** €74,201.85 total investment

**Breakdown:**

- Core infrastructure: €39,658.05 (48.5 days: 9 PM + 39.5 Dev)
- Optional WAF: €34,543.80 (43 days: 3 PM + 40 Dev)

**Source:** `docs/work_in_progress/architecture/integration-patterns/performance.md:64, 171`

**Original Source:** Performance Proposition PSA5499, CISACAzureInfrastructureUpdate.md

**Classification:** Vendor-provided

**Verification Recommended:**

- [ ] Compare day rates against market rates for Azure consultancy
- [ ] Review work breakdown for padding or unnecessary items
- [ ] Consider alternative implementation approaches (IaC templates, etc.)
- [ ] Validate PM vs Dev allocation ratios

**Impact:** High - Major investment decision

**Notes:** Audit recommendation is to NOT approve without performance baseline

---

### VEN-002: Monthly Recurring Cost Impact

**Estimate:** -€1,800/month net savings

**Calculation:** €3,300 database savings - €1,500 new costs

**Annual Impact:** €21,600/year savings

**Source:** `docs/work_in_progress/architecture/integration-patterns/performance.md:66, 176, 529`

**Classification:** Vendor-provided (calculated)

**Verification Recommended:**

- [ ] Validate SQL Database cost savings (Business Critical → Hyperscale)
- [ ] Verify new component costs (VPN, Data Factory IR, etc.) against Azure pricing
- [ ] Check for reserved instance discounts applied correctly
- [ ] Review actual current Azure bill for baseline comparison

**Impact:** High - ROI justification depends on this

---

### VEN-003: SQL Database Migration Savings

**Estimate:** -€3,300/month savings (Business Critical → Hyperscale)

**Annual Impact:** €39,600/year savings

**Source:** `docs/work_in_progress/architecture/integration-patterns/performance.md:66, 196, 204, 524`

**Classification:** Vendor-provided

**Verification Recommended:**

- [ ] Confirm current SQL Database tier and actual cost
- [ ] Validate Hyperscale pricing for equivalent configuration (20 vCore)
- [ ] Review replica reduction impact (4 → 2 replicas)
- [ ] Consider Hyperscale limitations vs Business Critical features

**Impact:** High - Largest component of savings claim

---

### VEN-004: Individual Component Monthly Costs

| Component | Monthly Cost | Source Line |
|-----------|--------------|-------------|
| VPN Gateway | +€200 | performance.md:260, 526 |
| Data Factory Managed VNet IR | +€500 | performance.md:261, 528 |
| Front Door/WAF | +€600 | performance.md:321, 525 |
| Databricks Standard→Premium | +€200 | performance.md:376, 527 |

**Total New Costs:** +€1,500/month (or +€900/month if WAF deferred)

**Source:** Performance Proposition PSA5499

**Classification:** Vendor-provided

**Verification Recommended:**

- [ ] Validate each against Azure pricing calculator
- [ ] Check for alternative configurations (e.g., Azure Bastion vs VPN Gateway)
- [ ] Review WAF necessity (audit recommends deferring)
- [ ] Confirm Databricks Premium features are needed

**Impact:** Medium - Affects net savings calculation

---

### VEN-005: Net Monthly (Defer WAF Scenario)

**Estimate:** -€2,400/month if WAF implementation deferred

**Calculation:** €3,300 - €900 = -€2,400

**Source:** `docs/work_in_progress/architecture/integration-patterns/performance.md:530`

**Classification:** Calculated from vendor estimates

**Impact:** Medium - Alternative scenario for phased approach

---

### VEN-006: Reserved Instance Discounts

**Estimates:**

- Databricks: 40-41% discount (1-year reserved)
- Databricks Pre-purchase DBCU: 4-6% discount
- Cosmos DB: 20% discount (1-year reserved)
- SQL License Bundling: 35% discount

**Source:** `docs/work_in_progress/architecture/integration-patterns/performance.md:195, 378-379, 390, 440, 455`

**Classification:** Vendor-provided (Azure standard discounts)

**Verification Recommended:**

- [ ] Confirm discount rates against current Azure pricing
- [ ] Review commitment terms and break-even analysis
- [ ] Validate that 1-year commitment is appropriate given system modernization plans

**Impact:** Medium - Affects cost calculations

---

### VEN-007: Databricks Runtime Performance Improvement

**Estimate:** 20-40% improvement with newer runtime

**Source:** `docs/work_in_progress/architecture/components/iswc-platform/databricks.md:644, 694, 825`

**Original Source:** "From Yann Discussion" (client-provided)

**Classification:** Vendor/Client-provided

**Verification Recommended:**

- [ ] Request actual performance data from vendor showing improvement
- [ ] Review Databricks release notes for performance optimizations
- [ ] Consider running benchmark with current notebooks on newer runtime (test cluster)

**Impact:** Medium - Affects upgrade priority recommendations

---

### VEN-008: SQL Database Configuration

**Current Configuration:**

- 24 vCore, 122.4 GB Memory
- Business Critical tier
- 4 replicas

**Proposed Configuration:**

- 20 vCore Hyperscale
- 2 replicas

**Source:** `docs/work_in_progress/architecture/integration-patterns/performance.md:197, 1239, 1244`

**Original Source:** SPE_20190218_ISWCDataModel_REV specification

**Classification:** Source-documented (current) + Vendor-provided (proposed)

**Verification Recommended:**

- [ ] Validate current configuration in Azure Portal
- [ ] Review actual vCore utilization metrics
- [ ] Assess if 20 vCore is sufficient or over/under-provisioned
- [ ] Understand impact of replica reduction on DR strategy

**Impact:** High - Core infrastructure change

---

## Priority 3: Experience-Based Estimates (Refine as Needed)

These are audit team estimates based on experience. They can be refined as actual work progresses.

### Technical Debt Remediation Efforts

**Summary:** 67 technical debt items with effort estimates ranging from 1 day to 8 weeks

**Source:** `docs/project_management/technical-debt-tracker.md`

**Totals:**

- **Critical items:** 25-38 weeks total (17 items)
- **High priority items:** 28-41 weeks total (26 items)
- **Quick wins (<1 week):** 12 items

**Classification:** Experience-based estimates from audit team

**Top 10 Largest Effort Items:**

| ID | Issue | Estimate | Priority |
|----|-------|----------|----------|
| TD-013 | TypeScript 3.7.3 outdated | 4-6 weeks | Critical |
| TD-012 | React 16.12.0 outdated | 3-5 weeks | Critical |
| TD-004 | Matching Engine SPOF | 4-6 weeks | Critical |
| TD-015 | Databricks Runtime upgrade | 2-4 weeks | Critical |
| TD-018 | ASP.NET Core 3.1 EOL | 2-3 weeks | High |
| TD-026 | Entity Framework Core 3.1 | 2-3 weeks | High |
| TD-014 | Redux 4.0.4 outdated | 2-3 weeks | Critical |
| TD-005 | No retry with exponential backoff | 2-3 weeks | Critical |
| TD-019 | No correlation IDs | 2-3 days | High |
| TD-008 | Generic error code _100 | 2 weeks | Critical |

**Validation Approach:**

- Start with quick wins to establish velocity baseline
- Refine estimates after first few items completed
- Track actual vs estimated time for continuous improvement

**Impact:** High - Total remediation effort is substantial (53-79 weeks across all priorities)

**Note:** These are planning estimates. Actual effort will be tracked as work progresses.

---

### Component Documentation Efforts

**Summary:** Undocumented components with documentation effort estimates

**Source:** `docs/project_management/undocumented-components.md:332-335`

**Phase-based Estimates:**

- **Phase 1 (Critical):** 3-5 days
- **Phase 2 (High):** 5-7 days
- **Phase 3 (Supporting):** 4 days
- **Total:** 12-16 days

**Individual Components:**

| Component | Effort | Priority |
|-----------|--------|----------|
| Azure Blob Storage | 1-2 days | Critical |
| SFTP Server | 2-3 days | Critical |
| Azure Data Factory | 1-2 days | High |
| IPI WebJob | 2-3 days | High |
| SQL Server | 1-2 days | High |
| IPI Database | 1 day | High |
| FastTrack SSO | 1 day | Supporting |
| Application Insights | 1 day | Supporting |
| Azure Key Vault | 1 day | Supporting |

**Classification:** Experience-based estimates

**Validation Approach:**

- Track actual documentation time for first few components
- Adjust estimates for remaining components based on actual velocity

**Impact:** Medium - Documentation backlog needs to be completed

---

### Databricks Component Recommendations

**Source:** `docs/work_in_progress/architecture/components/iswc-platform/databricks.md`

| Recommendation | Effort Estimate | Line |
|----------------|-----------------|------|
| Runtime Upgrade | 2-4 weeks | 710 |
| Automated Testing Framework | 3-4 weeks | 720 |
| SFTP Processing Spec | 2 weeks | 730 |
| Dev/Test Workspace Setup | 1 week | 738 |
| Idle Timeout Configuration | 1 day | 747 |
| Dead-Letter Queue | 1 week | 754 |
| Delta Lake VACUUM | 2 days | 761 |
| Code Quality Review | 4-6 weeks | 768 |
| Unity Catalog Migration | 2-3 weeks | 784 |

**Classification:** Experience-based estimates

**Total Effort:** ~13-18 weeks across all recommendations

---

### Web Portals Component Recommendations

**Source:** `docs/work_in_progress/architecture/components/iswc-platform/web-portals.md`

| Recommendation | Effort Estimate | Line |
|----------------|-----------------|------|
| React 16→18 Upgrade | 3-5 weeks | 1192 |
| TypeScript 3.7→5.x | 4-6 weeks | 1200 |
| Redux DevTools Integration | 1-2 weeks | 1224 |
| Redux Toolkit Migration | 6-8 weeks | 1233 |
| Code Splitting Implementation | 2-3 weeks | 1241 |
| Testing Framework Migration | 3-4 weeks | 1259 |
| Material-UI Migration | 2-3 weeks | 1267 |
| API Cleanup | 1 week | 1274 |
| Workflow Auto-Approval | 4-6 weeks | 1285 |
| Error Boundary Implementation | 1 day | 1294 |
| Accessibility Audit | 2 weeks | 1301 |
| Portal Analytics | 2-3 weeks | 1308 |

**Classification:** Experience-based estimates

**Total Effort:** ~27-42 weeks across all recommendations

---

### Cosmos DB Component Recommendations

**Source:** `docs/work_in_progress/architecture/components/iswc-platform/cosmos-db.md`

| Recommendation | Effort Estimate | Line |
|----------------|-----------------|------|
| Backup Strategy | 1-2 weeks | 742 |
| Disaster Recovery Testing | 2 days | 752 |
| Data Retention Policy | 2-3 weeks | 763 |
| RU Optimization | 1 week | 773 |
| Monitoring Setup | 2-3 days | 782 |
| Partition Analysis | 1 week | 791 |
| Query Performance Review | 1 week | 798 |
| Indexing Policy Review | 1 day | 805 |
| Serverless Evaluation | 2-3 days | 814 |

**Classification:** Experience-based estimates

**Total Effort:** ~6-9 weeks across all recommendations

---

### Performance Investigation Program

**Source:** `docs/work_in_progress/architecture/integration-patterns/performance.md:94-139, 1153`

**Timeline Phases:**

1. **Immediate Actions:** 1-2 weeks
    - Establish performance baseline
    - Request vendor data
    - Validate proposal assumptions

2. **Short-Term Investigation:** 2-4 weeks
    - Application performance audit
    - Infrastructure analysis
    - Cost-benefit analysis

3. **Medium-Term Planning:** 2-3 months
    - Application modernization path
    - Infrastructure decision
    - Testing strategy

**Total Investigation Effort:** 40-60 person-days

**Priority Distribution:**

- Priority 1 (Critical baselines): 20-26 days
- Priority 2 (Supporting): 12-20 days
- Priority 3 (Long-term): 8-14 days

**Classification:** Experience-based estimates

**Impact:** High - This is the recommended approach before making infrastructure decisions

---

## Reference: Source-Documented Values (No Validation Needed)

These are facts from specifications, code, or configuration files. They don't require validation as they represent current state.

### Timeout Configurations

| Configuration | Value | Source File | Line |
|---------------|-------|-------------|------|
| Matching Engine Timeout | 80 seconds | matching-pipeline.md | 80 |
| SQL CommandTimeout | 3 minutes | agency-api.md | 607, 765 |
| Retry Wait | 10 seconds | agency-api.md | 733 |
| JWT ClockSkew | 5 minutes | agency-api.md | 855 |
| Config Reload Interval | 30 minutes | agency-api.md | 892 |
| Circuit Breaker Half-Open | 30 seconds | ../../project_management/technical-debt-tracker.md | 92 |
| Databricks Auto-termination | 15-30 minutes | databricks.md | 746 |

**Source:** Code configuration values from repository analysis

---

### Backup and Recovery

| Configuration | Value | Source File | Line |
|---------------|-------|-------------|------|
| Cosmos DB Backup Frequency | Every 4 hours | cosmos-db.md | 91, 230, 688 |
| Cosmos DB Backup Window | 8 hours | cosmos-db.md | 91, 230, 737, 849 |
| Cosmos DB Backup Retention | 2 backups | audit-logging.md | 91, 103, 833, 952 |
| SQL Server Backup Retention | 35 days | audit-logging.md | 954 |

**Source:** SPE_20190218_ISWCDataModel_REV specification

**Note:** 8-hour Cosmos DB backup window vs 35-day SQL Server is a significant limitation documented as technical debt

---

### Cache Durations

| Cache Type | Duration | Source File | Line |
|------------|----------|-------------|------|
| Application Cache | 24 hours | agency-api.md, validation-pipeline.md | 351, 1313 |

**Source:** Configuration analysis

---

### File Processing

| Item | Value | Source File | Lines |
|------|-------|-------------|-------|
| IPI File Size | ~1GB per file (unzipped) | databricks.md | 39, 139, 280, 429, 495, 599 |

**Source:** IPI specifications (quarterly full dumps)

---

### Workflow Configuration

| Configuration | Value | Source File | Line |
|---------------|-------|-------------|------|
| Auto-Approval Time Limit | ~30 days | web-portals.md | 594, 1342, 1433 |

**Source:** Specification (note: "~30 days" indicates this may not be exact - should verify in code)

---

### Search Configuration

| Parameter | Value | Purpose |
|-----------|-------|---------|
| WorkTitleSimilarity | 70% | Lower threshold for inclusive results |
| MinMatchingWritersByNumberPct | 0% | Enables name-only searches |

**Source:** `web-portals.md:778, 782` (configuration analysis)

---

### Scheduled Job Frequencies

| Job | Frequency | Source File | Line |
|-----|-----------|-------------|------|
| IPI Synchronization | Quarterly (full) + incremental | level1-system-context.md | 102 |
| DCI Assessment | Weekly | level2-containers.md | 591 |

**Source:** Data Factory trigger configuration

---

### Resource Counts (From Azure Exports)

| Resource | Count | Source |
|----------|-------|--------|
| Total Production Azure Resources | 344 | level2-containers.md:21, 45, 1123 |
| Databricks Worker VMs | ~10 | level2-containers.md:544, 991 |
| Data Factory Pipelines | 14 | level2-containers.md:561 |
| Data Factory Datasets | 100+ | level2-containers.md:562 |
| Data Factory Linked Services | 11 | level2-containers.md:563 |
| Data Factory Triggers | 1 | level2-containers.md:564 |
| Core Design Documents | 12 | level1-system-context.md:24 |

**Source:** Azure Resources Export CSV dated 2025-10-21 + repository analysis

---

### Code Metrics

| Metric | Value | Source |
|--------|-------|--------|
| IPI FullSync Notebook | 763 lines | level2-containers.md:523 |
| Web Portals Code Reuse | 99% (only 5 of 368 files are portal-specific) | web-portals.md:42, 59 |
| IRule Implementation Coverage | 99% of rules | validation-pipeline.md:454 |
| Databricks Undocumented Work | 80% (only IPI spec exists) | databricks.md:727, 849 |

**Source:** Source code analysis

---

### Databricks Runtime Specifications

| Item | Value | Source |
|------|-------|--------|
| Current Runtime | 10.4 LTS | Level 2 containers |
| Python Version | 3.8 | Level 2 containers |
| Cold Start Time | 5-10 minutes | databricks.md:582 |

**Source:** Azure configuration + typical Databricks behavior

**Note:** Cold start time is data-backed (standard Databricks behavior) not estimated

---

### Technology Versions (EOL Tracking)

| Technology | Version | EOL Status | Source |
|------------|---------|------------|--------|
| React | 16.12.0 | Outdated (current 18.x) | Multiple files |
| TypeScript | 3.7.3 | Outdated (current 5.x) | Multiple files |
| Redux | 4.0.4 | Outdated (current 5.x) | Multiple files |
| ASP.NET Core | 3.1 | **EOL Dec 2022** | Multiple files |
| Databricks Runtime | 10.4 LTS | Outdated (current 13.x LTS) | Multiple files |
| Azure Functions | v3 | **EOL Dec 2022** | Multiple files |
| Entity Framework Core | 3.1 | EOL with ASP.NET Core 3.1 | Multiple files |

**Source:** Specification documents + Microsoft EOL dates

---

### Documentation Progress

**Source:** `docs/work_in_progress/architecture/components/iswc-platform/index.md:152-159`

| Component Category | Progress | Count |
|--------------------|----------|-------|
| Pipeline Components | 100% | 6/6 |
| Web Applications | 100% | 1/1 |
| APIs | 0% | 0/4 |
| Background Processing | 25% | 1/4 |
| Data Storage | 25% | 1/4 |
| External Dependencies | 0% | 0/3 |
| Infrastructure | 0% | 0/2 |
| **Overall** | **38%** | **9/24** |

---

### Technical Debt Distribution

**Source:** `../../project_management/technical-debt-tracker.md:25-28`

| Priority | Percentage | Count |
|----------|------------|-------|
| Critical | 25% | 17 items |
| High | 39% | 26 items |
| Medium | 28% | 19 items |
| Low | 7% | 5 items |
| **Total** | **100%** | **67 items** |

---

### Questions Distribution

**Source:** `../../project_management/questions-tracker.md:518-533`

**Priority Distribution:**

- Critical: 27% (66 questions)
- High: 40% (98 questions)
- Medium/Low: 33% (78 questions)

**Category Distribution:**

- Performance and Scalability: 19% (45 questions)
- Cost and Resource: 10% (24 questions)
- Security: 6% (14 questions)
- Data Integrity: 11% (26 questions)
- Configuration/Rules: 12% (28 questions)
- Error Handling/Monitoring: 12% (30 questions)
- Business Logic/Workflows: 14% (34 questions)
- Database Schema: 7% (17 questions)
- Development/Testing: 7% (16 questions)
- Backup/DR: 3% (8 questions)

**Total Questions:** 242

---

### C4 Architecture Documentation Progress

**Source:** `c4-architecture-master.md:324-327`

| Level | Progress | Count |
|-------|----------|-------|
| Level 1 (System Context) | 0% | 0/1 validated |
| Level 2 (Containers) | 0% | 0/21 validated |
| Level 3 (Components) | 35% | 7/20 documented |
| Integration Patterns | 25% | 2/8 documented |

---

## Validation Recommendations

### Immediate Priorities (Before Making Recommendations)

1. **SCALE-004: Cosmos DB 100-220x Scaling** 🚨 CRITICAL
   - **Action:** Demand detailed justification from SpanishPoint
   - **Data Needed:** Current RU consumption, throttling incidents, performance model
   - **Timeline:** Block proposal until explained
   - **Impact:** Could save €40-50K+ annually if over-provisioned

2. **PERF-001: Matching Engine Response Times**
   - **Action:** Request production metrics from vendor + analyze Application Insights
   - **Data Needed:** P50/P95/P99 response times, correlation analysis
   - **Timeline:** 1 week
   - **Impact:** Core to performance baseline and optimization strategy

3. **COST-001: Databricks Idle Cluster Waste**
   - **Action:** Analyze actual billing + cluster utilization
   - **Data Needed:** 6 months Databricks bills, Azure Monitor metrics
   - **Timeline:** 2-3 days
   - **Impact:** €60-120K annual potential savings

4. **VEN-002/VEN-003: Monthly Cost Claims**
   - **Action:** Validate all cost claims against actual Azure bill
   - **Data Needed:** Current itemized Azure bill for production
   - **Timeline:** 1 day
   - **Impact:** Validate €1,800/month savings claim

### Data Collection Requests

#### From Client (CISAC)

- [ ] **Azure billing data:** Last 6 months itemized bills
- [ ] **Application Insights:** Production telemetry access
- [ ] **Azure Monitor:** Cluster utilization metrics
- [ ] **Cosmos DB:** Storage and RU consumption metrics
- [ ] **Historical data:** Submission volumes, processing times, rejection rates

#### From Vendor (SpanishPoint)

- [ ] **Matching Engine SLA:** Uptime guarantees and performance targets
- [ ] **Matching Engine metrics:** Actual response time data
- [ ] **Cosmos DB justification:** Why 100-220x scaling is required
- [ ] **Performance model:** Calculations supporting Hyperscale proposal
- [ ] **Databricks optimization:** Evidence for 20-40% improvement claim

### Experimentation Opportunities

If vendor/client data is insufficient, consider these experiments:

1. **Load Testing Program**
   - Matching Engine: Controlled load test to measure response times
   - Validation Pipeline: Benchmark rule execution with representative data
   - API Endpoints: Stress test to establish baseline performance

2. **Infrastructure Analysis**
   - Bundle analyzer: Measure actual frontend bundle sizes
   - Profiler: Measure reflection overhead and identify hotspots
   - Cache analysis: Instrument caching layer to measure hit rates and latencies

3. **Cost Optimization Testing**
   - Databricks idle timeout: Monitor impact of shorter timeout on costs
   - Cosmos DB scaling: Test with lower RU to find actual requirement
   - Reserved instances: Model break-even analysis for commitments

### Acceptance Criteria Framework

For each Priority 1 estimate, validation is complete when:

1. **Source identified:** Real data, measurement, or calculation documented
2. **Variance acceptable:** Within ±20% for costs, ±50% for performance
3. **Confidence level:** Can defend estimate in client presentation
4. **Impact assessed:** Know if estimate affects recommendations

### Next Steps

1. **Review this tracker** with audit team
2. **Prioritize data requests** based on recommendation impact
3. **Schedule vendor meetings** to address critical unknowns (especially SCALE-004)
4. **Plan experimentation** for gaps that can't be filled with data requests
5. **Update estimates** as validation progresses
6. **Track validation progress** in this document (checkboxes above)

---

## Document Maintenance

**Update Frequency:** As estimates are validated or new estimates are added

**Ownership:** Audit team (Guillaume Jay, Bastien Gallay)

**Last Review:** 2025-10-30

**Next Review:** Before client presentation / vendor meetings

---

**Legend:**

- 🚨 = Critical blocker
- [ ] = Validation action not started
- [x] = Validation action completed
- ±X% = Acceptable variance for validation
