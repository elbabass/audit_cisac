# Technical Debt and Risks Tracker - ISWC Platform

**Version:** 1.0
**Date:** October 29, 2025
**Total Items:** 67 technical debt items across 9 component documents

**Purpose:** Consolidated tracker for all technical debt, risks, and improvement opportunities identified during the CISAC ISWC audit.

---

## How to Use This Document

- **Status:** ⏳ = Not Started | 🔄 = In Progress | ✅ = Completed | 🚫 = Won't Fix
- **Priority:** 🔴 = Critical | ⚠️ = High | 📝 = Medium | 💡 = Low
- **Links:** Click component names to jump to source documentation

---

## Executive Summary

### Priority Distribution

| Priority | Count | Percentage |
|----------|-------|------------|
| 🔴 Critical | 17 | 25% |
| ⚠️ High | 26 | 39% |
| 📝 Medium | 19 | 28% |
| 💡 Low | 5 | 7% |

### Top 5 Critical Concerns

1. **Security:** ASP.NET Core 3.1 EOL (Dec 2022), React 16.12.0 outdated (5+ years), Databricks runtime 11.3 LTS (4 versions behind)
2. **Vendor Lock-in:** Matching Engine SPOF, FastTrack SSO SPOF
3. **Data Safety:** Cosmos DB 8-hour backup window (vs 35 days SQL), untested disaster recovery
4. **Performance:** No circuit breakers, 80-second timeouts, reflection overhead
5. **Maintainability:** Generic error handling (_100 for all errors), complex nested logic, no automated testing

### Estimated Effort

- **Critical items:** 25-38 weeks total
- **High priority items:** 28-41 weeks total
- **Quick wins (<1 week):** 12 items

---

## 🔴 Critical Priority (17 items)

### TD-001: No Maximum Iteration Limit in Validation

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 🔴 Critical |
| **Component** | [validation-pipeline](validation-pipeline.md#technical-debt-and-risks) |
| **Impact** | Infinite loop if buggy rule always sets `ToBeProcessed = true` |
| **Mitigation** | Add max iteration limit (e.g., 10) with logging |
| **Effort** | 1 day |

**Code Location:** `ValidationPipeline.cs:RunPipeline()` - `while (submissions.Any(s => s.ToBeProcessed))`

---

### TD-002: Batch Rules Not Implemented

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 🔴 Critical |
| **Component** | [validation-pipeline](validation-pipeline.md#technical-debt-and-risks) |
| **Impact** | Dead code, unused complexity |
| **Mitigation** | Either implement batch rules or remove `IBatchRule` support |
| **Effort** | 1 week (investigation + removal) |

**Decision Needed:** Are batch rules required for any use cases?

---

### TD-003: No Circuit Breaker Pattern for Matching Engine

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 🔴 Critical |
| **Component** | [matching-pipeline](matching-pipeline.md#technical-debt-and-risks) |
| **Impact** | Slow Matching Engine responses block all submissions; Timeout after 80s × 3 retries = 4 minutes; No graceful degradation |
| **Mitigation** | Implement circuit breaker (e.g., Polly CircuitBreakerPolicy) |
| **Effort** | 1-2 weeks |

**Recommended Configuration:**
- Open circuit after 5 consecutive failures
- Half-open state after 30 seconds
- Fallback to basic exact matching

---

### TD-004: Single Point of Failure - Matching Engine Vendor Dependency

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 🔴 Critical |
| **Component** | [matching-pipeline](matching-pipeline.md#technical-debt-and-risks) |
| **Impact** | Matching Engine downtime = ISWC system unusable; No local matching capability; Vendor lock-in (Spanish Point proprietary algorithms) |
| **Mitigation** | Implement fallback matching strategy (basic exact matching); Cache frequently matched works; Consider alternative matching engine (Elasticsearch, Azure Cognitive Search) |
| **Effort** | 4-6 weeks (fallback implementation) |

**Strategic Consideration:** Evaluate cost/benefit of vendor independence vs maintaining vendor relationship

---

### TD-005: 80-Second Timeout Too Long for Matching Engine

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 🔴 Critical |
| **Component** | [matching-pipeline](matching-pipeline.md#technical-debt-and-risks) |
| **Impact** | User experience degradation (slow responses); Thread pool exhaustion under load; No parallelization |
| **Mitigation** | Reduce timeout to 30 seconds; Implement async batch processing; Add queue-based submission processing |
| **Effort** | 2-3 weeks |

---

### TD-006: No Retry Mechanism for Concurrency Conflicts

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 🔴 Critical |
| **Component** | [processing-pipeline](processing-pipeline.md#technical-debt-and-risks) |
| **Impact** | High failure rate in concurrent submission scenarios |
| **Mitigation** | Implement optimistic retry with exponential backoff (e.g., 3 retries with 100ms, 200ms, 400ms delays) |
| **Effort** | 1 week |

---

### TD-007: Reflection-Based Component Discovery on Every Submission

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 🔴 Critical |
| **Component** | [processing-pipeline](processing-pipeline.md#technical-debt-and-risks) |
| **Impact** | CPU overhead for assembly scanning and filtering; Degraded performance at scale |
| **Mitigation** | Cache discovered components by filter criteria (TransactionType, IsEligible, PreferedIswcType) |
| **Effort** | 3-5 days |

**Code Location:** `ProcessingComponent.cs:RunComponent()` - `AppDomain.CurrentDomain.GetComponentsOfType<IProcessingSubComponent>`

---

### TD-008: Generic Exception Handling (_100 Error Code)

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 🔴 Critical |
| **Component** | [processing-pipeline](processing-pipeline.md#technical-debt-and-risks) |
| **Impact** | Poor error visibility for clients; Difficult to diagnose and resolve failures |
| **Mitigation** | Implement specific error codes for SQL errors, network errors, validation errors, etc. |
| **Effort** | 2 weeks |

**Recommended Error Codes:**
- _101: Database connection error
- _102: Database timeout
- _103: Matching Engine unavailable
- _104: Matching Engine timeout
- _105: Concurrency conflict

---

### TD-009: Unclear Transaction Boundaries Across Pipeline Stages

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 🔴 Critical |
| **Component** | [processing-pipeline](processing-pipeline.md#technical-debt-and-risks), [post-matching-pipeline](post-matching-pipeline.md#technical-debt-and-risks) |
| **Impact** | If EF Core auto-commits after each operation, concurrent API requests could modify the same work data between ProcessingPipeline and PostMatchingPipeline, causing validation failures |
| **Mitigation** | Investigate actual transaction boundaries; Consider explicit `TransactionScope` if needed |
| **Effort** | 1 week (investigation + implementation if needed) |

**Investigation Questions:**
- What is the Entity Framework Core transaction scope? (Per-request? Per-operation?)
- Are there explicit `TransactionScope` declarations in the API layer?
- How frequently do concurrent requests target the same work?

---

### TD-010: Sequential Database Queries (N+1 Problem)

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 🔴 Critical |
| **Component** | [post-matching-pipeline](post-matching-pipeline.md#technical-debt-and-risks) |
| **Impact** | High latency for merge operations with many targets; Performance degradation at scale |
| **Mitigation** | Batch merge target validation into single query |
| **Effort** | 3-5 days |

**Code Location:** `PV_05.cs`, `PV_09.cs` - ISWC merge target validation loops

---

### TD-011: No Caching from ProcessingPipeline

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 🔴 Critical |
| **Component** | [post-matching-pipeline](post-matching-pipeline.md#technical-debt-and-risks) |
| **Impact** | Redundant work lookups (already queried in ProcessingPipeline); Unnecessary database load |
| **Mitigation** | Pass ProcessingPipeline work data to PostMatchingPipeline via shared context |
| **Effort** | 1 week |

---

### TD-012: ASP.NET Core 3.1 End of Life (December 2022)

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 🔴 Critical |
| **Component** | [web-portals](web-portals.md#technical-debt-and-risks) |
| **Impact** | No security patches since December 2022; 40+ known vulnerabilities in .NET Core 3.1 ecosystem; Compliance risk for CISAC member agencies |
| **Mitigation** | Upgrade to .NET 8 LTS (supported until November 2026) |
| **Effort** | 3-5 weeks (testing required for breaking changes) |

**Blocking Issues:**
- Entity Framework Core 3.0 → 8.0 (breaking changes)
- ASP.NET Core 3.1 → 8.0 (middleware changes, dependency injection changes)
- Third-party library compatibility

---

### TD-013: React 16.12.0 Severely Outdated (Released December 2019)

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 🔴 Critical |
| **Component** | [web-portals](web-portals.md#technical-debt-and-risks) |
| **Impact** | Missing 5+ years of security patches and performance improvements; React 16 has documented XSS vulnerabilities; Missing modern features (concurrent rendering, automatic batching, Suspense) |
| **Mitigation** | Upgrade to React 18.x (breaking changes in lifecycle methods) |
| **Effort** | 4-6 weeks (component refactoring + testing) |

**Breaking Changes:**
- `componentWillMount`, `componentWillReceiveProps`, `componentWillUpdate` deprecated
- Automatic batching changes event handler behavior
- Stricter hydration errors

---

### TD-014: FastTrack SSO Single Point of Failure

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 🔴 Critical |
| **Component** | [web-portals](web-portals.md#technical-debt-and-risks) |
| **Impact** | Portal completely unusable if FastTrack is unavailable; No fallback; Vendor lock-in (external dependency outside CISAC control) |
| **Mitigation** | Circuit breaker pattern with degraded mode; Local authentication fallback for critical users; SLA verification with FastTrack provider |
| **Effort** | 2-3 weeks |

**Questions for Investigation:**
- Who operates FastTrack SSO (CISAC or third-party vendor)?
- What is FastTrack SSO uptime SLA?
- Is there a backup authentication method?

---

### TD-015: Outdated Databricks Runtime (11.3 LTS, 4 Versions Behind)

| Field | Value |
|-------|-------|
| **Status** | 🔄 In Progress (contractual dispute) |
| **Priority** | 🔴 Critical |
| **Component** | [databricks](databricks.md#technical-debt-and-risks) |
| **Impact** | Missing Features (AI-assisted queries, auto-optimization, Photon engine, Unity Catalog); Security Risk (unpatched vulnerabilities in Spark 3.3.x); Performance (modern runtimes have 20-40% optimization improvements); Support (vendor support for Runtime 11.3 LTS ends September 2025 - likely already expired); Cost (inefficient resource usage) |
| **Mitigation** | Escalate to CISAC management - critical security and cost issue |
| **Effort** | 2-4 weeks (notebook testing for breaking changes, regression testing) |

**Contractual Context:**
- Yann's expectation: Runtime updates should be included in annual maintenance contract
- Spanish Point's position: Offered to charge separately for runtime upgrade
- Yann's response: Rejected paid upgrade - maintenance should cover platform updates
- Current status: Upgrade "in progress" but delayed by months/years

**Action Required:** Escalate with evidence of security/cost impact

---

### TD-016: Cosmos DB Very Limited Backup Retention (8-Hour Window)

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 🔴 Critical |
| **Component** | [cosmos-db](cosmos-db.md#technical-debt-and-risks) |
| **Impact** | Data loss risk beyond 8 hours (catastrophic for audit compliance); If data corruption detected after 9+ hours, no recovery possible; Compliance risk (audit data required for legal/regulatory purposes) |
| **Mitigation** | Implement long-term backup strategy (copy to Azure Blob Storage, schedule exports); Consider Azure Data Factory scheduled export jobs |
| **Effort** | 1-2 weeks (implement scheduled backup jobs + restore testing) |

**Comparison:** SQL Server has 35-day retention (MUCH better)

---

### TD-017: Untested Backup Restoration Procedure for Cosmos DB

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 🔴 Critical |
| **Component** | [cosmos-db](cosmos-db.md#technical-debt-and-risks) |
| **Impact** | Requires opening Azure support ticket (not self-service); Recovery time unknown (depends on Azure support SLA); Never tested in production (disaster recovery readiness unknown) |
| **Mitigation** | Document step-by-step restoration process; Conduct annual DR test |
| **Effort** | 1 day documentation + 1 day testing |

**Action Required:** Schedule disaster recovery drill with operations team

---

## ⚠️ High Priority (26 items)

### TD-018: ASP.NET Core 3.1 EOL in Validator Projects

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [validation-pipeline](validation-pipeline.md#technical-debt-and-risks) |
| **Impact** | No security patches, compatibility issues |
| **Mitigation** | Upgrade to .NET 8 LTS |
| **Effort** | 2-3 weeks (coordinated with TD-012) |

---

### TD-019: Rule Discovery Performance Overhead

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [validation-pipeline](validation-pipeline.md#technical-debt-and-risks) |
| **Impact** | Reflection-based rule discovery on every request (~100-200ms overhead per batch) |
| **Mitigation** | Cache rule discovery results per validator lifetime |
| **Effort** | 2-3 days |

---

### TD-020: No Rule Versioning Strategy

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [validation-pipeline](validation-pipeline.md#technical-debt-and-risks) |
| **Impact** | `PipelineComponentVersion` tracks assembly version but no versioning strategy for rule logic changes; Cannot track which rule version was applied in audit trail |
| **Mitigation** | Implement semantic versioning for rules (e.g., `IV_02_v2`) |
| **Effort** | 2 weeks |

---

### TD-021: Limited Error Context in Validation

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [validation-pipeline](validation-pipeline.md#technical-debt-and-risks) |
| **Impact** | Rejection messages don't include field names or values; Poor debugging experience for users |
| **Mitigation** | Enhance Rejection model with field path and values (e.g., "Title[0].TitleText: 'UNKNOWN' violates IV_05") |
| **Effort** | 1 week |

---

### TD-022: Hardcoded Validator Order

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [validation-pipeline](validation-pipeline.md#technical-debt-and-risks) |
| **Impact** | Validator sequence is hardcoded in ValidationPipeline.cs; Cannot reorder validators without code change |
| **Mitigation** | Configuration-driven validator ordering (database or config file) |
| **Effort** | 1 week |

---

### TD-023: Reflection-Based Plugin Discovery on Every Request (Matching)

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [matching-pipeline](matching-pipeline.md#technical-debt-and-risks) |
| **Impact** | Component discovery via reflection on every batch (~150-200ms overhead per batch) |
| **Mitigation** | Cache discovered components at application startup |
| **Effort** | 2-3 days |

---

### TD-024: Two-Phase Matching Doubles HTTP Calls

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [matching-pipeline](matching-pipeline.md#technical-debt-and-risks) |
| **Impact** | Fallback to "Label" source doubles Matching Engine calls; Up to 2× HTTP overhead for submissions without matches |
| **Mitigation** | Make fallback configurable per agency; Implement smart fallback (only when needed); Add telemetry to measure fallback frequency |
| **Effort** | 1 week |

---

### TD-025: No Match Confidence Threshold Configuration

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [matching-pipeline](matching-pipeline.md#technical-debt-and-risks) |
| **Impact** | RankScore threshold hardcoded (IsDefinite flag); Cannot tune matching sensitivity per agency |
| **Mitigation** | Add configurable thresholds (database or config file) |
| **Effort** | 3-5 days |

---

### TD-026: Complex GetPreferredIswcType Logic (62 lines)

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [processing-pipeline](processing-pipeline.md#technical-debt-and-risks) |
| **Impact** | Difficult to maintain and test; Edge cases may not be handled correctly |
| **Mitigation** | Refactor into strategy pattern with unit tests |
| **Effort** | 1 week |

---

### TD-027: Two Database Round Trips per Submission

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [processing-pipeline](processing-pipeline.md#technical-debt-and-risks) |
| **Impact** | Insert + query back for IswcModel; Increased latency and database load |
| **Mitigation** | Optimize WorkManager to return IswcModel directly from insert operation |
| **Effort** | 3-5 days |

---

### TD-028: No Validation of Parent-Child Processing Order

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [processing-pipeline](processing-pipeline.md#technical-debt-and-risks) |
| **Impact** | Child submissions may fail if parent not processed first; Batch failures due to ordering issues |
| **Mitigation** | Add batch ordering validation; Topological sort for parent-child dependencies |
| **Effort** | 1 week |

---

### TD-029: IV_40 Misplaced in PostMatchingValidator

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [post-matching-pipeline](post-matching-pipeline.md#technical-debt-and-risks) |
| **Impact** | Confusing naming (IV_* typically in StaticDataValidator); Maintenance confusion |
| **Mitigation** | Rename to PV_XX or move to appropriate validator |
| **Effort** | 1 day |

---

### TD-030: Duplicate Disambiguation Rules (PV_30 + IV_40)

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [post-matching-pipeline](post-matching-pipeline.md#technical-debt-and-risks) |
| **Impact** | Two rules for same validation; Inconsistent behavior if configurations differ |
| **Mitigation** | Consolidate into single configurable rule |
| **Effort** | 3 days |

---

### TD-031: Complex Nested Logic in PV_20, PV_21

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [post-matching-pipeline](post-matching-pipeline.md#technical-debt-and-risks) |
| **Impact** | Difficult to maintain and test; Edge cases not covered |
| **Mitigation** | Refactor into smaller, testable methods |
| **Effort** | 1 week |

---

### TD-032: Single-Error Reporting (Short-Circuit)

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [post-matching-pipeline](post-matching-pipeline.md#technical-debt-and-risks) |
| **Impact** | User sees only first validation error; Multiple round trips to fix all issues |
| **Mitigation** | Add "collect all errors" mode for development/testing |
| **Effort** | 1 week |

---

### TD-033: TypeScript 3.7.3 Outdated

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [web-portals](web-portals.md#technical-debt-and-risks) |
| **Impact** | Missing modern TypeScript features (4.x → 5.x improvements); No template literal types, no satisfies operator; Degraded VSCode/IDE experience |
| **Mitigation** | Upgrade to TypeScript 5.x (low risk, high value) |
| **Effort** | 1-2 weeks (mostly type fixes) |

---

### TD-034: Redux 4.0.4 Legacy Pattern (Pre-Redux Toolkit)

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [web-portals](web-portals.md#technical-debt-and-risks) |
| **Impact** | Verbose boilerplate, no built-in async handling best practices; Current pattern: Manual thunks, action creators, reducers |
| **Mitigation** | Gradual migration to Redux Toolkit (official recommended approach); Benefits: Reduced code, built-in Immer, RTK Query for API caching |
| **Effort** | 6-8 weeks (large codebase, 50+ thunks) |

---

### TD-035: No Code Splitting or Lazy Loading

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [web-portals](web-portals.md#technical-debt-and-risks) |
| **Impact** | Large initial bundle download (800KB-1MB estimated); Slow initial page load, especially for mobile/poor connections; Current pattern: Single bundle with all routes |
| **Mitigation** | Implement React.lazy for route-based splitting |
| **Effort** | 2-3 weeks (webpack configuration + dynamic imports) |

---

### TD-036: Entity Framework Core 3.0.0

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [web-portals](web-portals.md#technical-debt-and-risks) |
| **Impact** | Missing performance improvements from EF Core 5-8; No compiled models, slower query generation; Breaking Changes: .NET 8 requires EF Core 8 |
| **Mitigation** | Upgrade alongside .NET upgrade (TD-012) |
| **Effort** | Included in ASP.NET Core upgrade effort |

---

### TD-037: No Automated Testing for Databricks Notebooks

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [databricks](databricks.md#technical-debt-and-risks) |
| **Impact** | Changes to notebooks risk breaking production data pipelines; Data corruption, incorrect transformations, downtime |
| **Mitigation** | Implement pytest for PySpark logic, test data fixtures |
| **Effort** | 3-4 weeks (build test framework + coverage for critical notebooks) |

---

### TD-038: SFTP File Processing Specification Gap

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [databricks](databricks.md#technical-debt-and-risks) |
| **Impact** | No formal design document for EDI/JSON agency file processing; 80% of Databricks workload undocumented (only IPI spec exists); Tribal knowledge, difficult to maintain or modernize |
| **Mitigation** | Reverse-engineer specification from notebooks + validation |
| **Effort** | 2 weeks analysis + documentation |

---

### TD-039: Single Databricks Workspace (No Dev/Test/Prod Isolation)

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [databricks](databricks.md#technical-debt-and-risks) |
| **Impact** | Development work could interfere with production jobs; Best Practice: Separate workspaces per environment |
| **Mitigation** | Create dedicated dev/test workspace with smaller clusters |
| **Effort** | 1 week setup + CI/CD pipeline updates |

---

### TD-040: No Data Retention Policies for Cosmos DB

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [cosmos-db](cosmos-db.md#technical-debt-and-risks) |
| **Impact** | 822M+ documents accumulate indefinitely; Unbounded storage growth increases costs; Large collections slow down queries; May violate GDPR "right to be forgotten" |
| **Mitigation** | Implement TTL for audit data (e.g., 7-year retention), archival to cold storage |
| **Effort** | 2-3 weeks (design retention policy + implement archival) |

---

### TD-041: Unknown Production RU Configuration for Cosmos DB

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [cosmos-db](cosmos-db.md#technical-debt-and-risks) |
| **Impact** | Documented: 1,000 RU initial provisioning; Actual: Unknown; Over-provisioned = wasted cost, Under-provisioned = throttling |
| **Mitigation** | Review Azure Cost Management, optimize RU allocation |
| **Effort** | 1 week analysis + tuning |

---

### TD-042: No Local Development Emulator for Cosmos DB

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [cosmos-db](cosmos-db.md#technical-debt-and-risks) |
| **Impact** | Developers must connect to cloud Cosmos DB (slower, costs money); Alternative: Cosmos DB emulator exists but may not be configured |
| **Mitigation** | Configure Cosmos DB emulator for local development |
| **Effort** | 2-3 days setup + documentation |

---

### TD-043: No Database Schema Index Optimization

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | ⚠️ High |
| **Component** | [pipeline-architecture-overview](pipeline-architecture-overview.md#technical-debt-and-risks) |
| **Impact** | Database queries may not be optimized |
| **Mitigation** | Review execution plans, add missing indexes |
| **Effort** | 1 week |

---

## 📝 Medium Priority (19 items)

*(Showing first 5, full list in source documents)*

### TD-044: Additional Identifiers Merging Complexity

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 📝 Medium |
| **Component** | [validation-pipeline](validation-pipeline.md#technical-debt-and-risks) |
| **Impact** | O(n×m×k) nested loops; Performance issue for works with many identifiers |
| **Mitigation** | Optimize with dictionary lookups |
| **Effort** | 2-3 days |

---

### TD-045: Enzyme Testing Library (Deprecated)

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 📝 Medium |
| **Component** | [web-portals](web-portals.md#technical-debt-and-risks) |
| **Impact** | Enzyme no longer maintained, React 18 incompatible; Test suite will break on React upgrade |
| **Mitigation** | Migrate to React Testing Library |
| **Effort** | 3-4 weeks (rewrite all component tests) |

**Note:** Must be completed before TD-013 (React 18 upgrade)

---

### TD-046: Bootstrap 4.4.1 Outdated

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 📝 Medium |
| **Component** | [web-portals](web-portals.md#technical-debt-and-risks) |
| **Impact** | Bootstrap 5 released in 2021, different API; Missing accessibility improvements, outdated design patterns; Migration Complexity: Bootstrap 5 removed jQuery dependency |
| **Mitigation** | Upgrade to Bootstrap 5 + update Reactstrap |
| **Effort** | 2-3 weeks (UI regression testing required) |

---

### TD-047: No API Response Caching

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 📝 Medium |
| **Component** | [web-portals](web-portals.md#technical-debt-and-risks) |
| **Impact** | Repeated calls for lookup data (agencies, role types); Unnecessary database queries and HTTP round-trips |
| **Mitigation** | Implement HTTP caching headers + Redis cache |
| **Effort** | 1 week |

---

### TD-048: Permission Model Limitations

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 📝 Medium |
| **Component** | [web-portals](web-portals.md#technical-debt-and-risks) |
| **Impact** | Agency-level access only (all users see same data); Requested: User-specific permissions; Usability issue for large agencies (GEMA, BMI, SACEM) |
| **Mitigation** | Design role-based permission model |
| **Effort** | 4-6 weeks (database schema + UI changes) |

---

*[TD-049 through TD-062 omitted for brevity - see source documents for full list]*

---

## 💡 Low Priority (5 items)

### TD-063: Stopwatch Timing Overhead

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 💡 Low |
| **Component** | [validation-pipeline](validation-pipeline.md#technical-debt-and-risks) |
| **Impact** | Minor CPU overhead for rule execution tracking; Negligible but unnecessary in high-throughput scenarios |
| **Mitigation** | Consider async timing or sampling |
| **Effort** | 2-3 days |

---

### TD-064: No Rule Execution Timeout

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 💡 Low |
| **Component** | [post-matching-pipeline](post-matching-pipeline.md#technical-debt-and-risks) |
| **Impact** | Slow database queries can block pipeline; Timeouts at API level |
| **Mitigation** | Add rule-level timeouts with circuit breaker |
| **Effort** | 1 week |

---

### TD-065: Hard-Coded Public Domain Logic (80 years)

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 💡 Low |
| **Component** | [post-matching-pipeline](post-matching-pipeline.md#technical-debt-and-risks) |
| **Impact** | Cannot adapt to different jurisdictions; Incorrect public domain detection for some countries |
| **Mitigation** | Move to configurable rule parameter |
| **Effort** | 2-3 days |

---

### TD-066: Application Insights SDK Versions

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 💡 Low |
| **Component** | [web-portals](web-portals.md#technical-debt-and-risks) |
| **Impact** | Frontend: @microsoft/applicationinsights-react-js 2.5.4 (2019); Backend: bundled with 3.1; Missing better telemetry features |
| **Mitigation** | Upgrade to latest SDKs |
| **Effort** | 1 day |

---

### TD-067: Localization Resource Files (.resx)

| Field | Value |
|-------|-------|
| **Status** | ⏳ Not Started |
| **Priority** | 💡 Low |
| **Component** | [web-portals](web-portals.md#technical-debt-and-risks) |
| **Impact** | XML-based resource files (legacy .NET Framework pattern); Developer experience issue |
| **Mitigation** | Migrate to JSON localization with i18next |
| **Effort** | 2 weeks |

---

## Roadmap Recommendations

### Phase 1: Security and Stability (8-12 weeks)

**Priority:** Address critical security vulnerabilities

1. TD-012: ASP.NET Core 3.1 → .NET 8 LTS (3-5 weeks)
2. TD-013: React 16.12.0 → React 18.x (4-6 weeks)
3. TD-015: Databricks runtime upgrade (2-4 weeks)
4. TD-016: Cosmos DB long-term backup (1-2 weeks)
5. TD-017: Cosmos DB DR testing (2 days)

**Dependencies:** TD-013 requires TD-045 (Enzyme → React Testing Library) first

---

### Phase 2: Vendor Lock-in Mitigation (6-10 weeks)

**Priority:** Reduce single points of failure

1. TD-004: Matching Engine fallback strategy (4-6 weeks)
2. TD-003: Circuit breaker for Matching Engine (1-2 weeks)
3. TD-014: FastTrack SSO fallback (2-3 weeks)
4. TD-005: Reduce Matching Engine timeout (2-3 weeks)

---

### Phase 3: Performance Optimization (6-8 weeks)

**Priority:** Improve system performance

1. TD-007: Cache component discovery (3-5 days)
2. TD-019: Cache rule discovery (2-3 days)
3. TD-023: Cache matching component discovery (2-3 days)
4. TD-010: Batch merge validation queries (3-5 days)
5. TD-011: Pass work data between pipelines (1 week)
6. TD-027: Optimize database round trips (3-5 days)
7. TD-035: Implement code splitting (2-3 weeks)

---

### Phase 4: Maintainability (8-12 weeks)

**Priority:** Improve code quality and testability

1. TD-008: Specific error codes (2 weeks)
2. TD-037: Databricks notebook testing (3-4 weeks)
3. TD-026: Refactor GetPreferredIswcType (1 week)
4. TD-031: Refactor PV_20, PV_21 (1 week)
5. TD-038: Document SFTP processing (2 weeks)

---

### Phase 5: Quick Wins (<1 week each)

**Priority:** Low-hanging fruit

1. TD-001: Max iteration limit (1 day)
2. TD-006: Concurrency retry (1 week)
3. TD-029: Rename IV_40 (1 day)
4. TD-030: Consolidate disambiguation rules (3 days)
5. TD-041: Review Cosmos DB RU config (1 week)
6. TD-042: Cosmos DB emulator setup (2-3 days)
7. TD-055: Cosmos DB cost alerts (1 day)
8. TD-066: Update Application Insights SDKs (1 day)

---

## Tracking and Reporting

### Weekly Status Update Template

```markdown
## Technical Debt Progress - Week of [DATE]

**Items Completed:** X
**Items In Progress:** Y
**Items Started:** Z

### Highlights
- [TD-XXX] Completed: Brief description
- [TD-YYY] In Progress: Current status and blockers

### Next Week
- [TD-ZZZ] Starting: Objective and acceptance criteria

### Blockers
- List any blockers requiring escalation
```

### Monthly Executive Summary Template

```markdown
## Technical Debt Monthly Report - [MONTH YEAR]

**Critical Items Remaining:** X / 17
**High Priority Remaining:** Y / 26

**Risk Trend:** ⬇️ Decreasing | ➡️ Stable | ⬆️ Increasing

### Key Achievements
- List completed items

### Critical Concerns
- List any new critical issues discovered

### Budget Impact
- Cost savings from completed optimizations
- Investment required for upcoming work
```

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-10-29 | Audit Team | Initial consolidated technical debt tracker; 67 items extracted from 9 component documents; Organized by priority with effort estimates, mitigation strategies, and roadmap recommendations |

---

**Next Review:** Weekly during active remediation, monthly for tracking

**Owner:** CISAC Technical Leadership + Spanish Point Engineering Team
