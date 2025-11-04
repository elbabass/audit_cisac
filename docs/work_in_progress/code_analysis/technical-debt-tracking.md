# ISWC System Technical Debt Register

**Document Version:** 2.0
**Last Updated:** 2025-11-04
**Owner:** Teragone-Factory Audit Team
**Review Frequency:** Monthly
**Related Documents:**

- [Automated Analysis 2025-10-30](automated-analysis-2025-10-30.md) - ISWC v1 baseline
- [ISWC v2 Upgrade Analysis 2025-11-04](iswc-v2-upgrade-analysis-2025-11-04.md) - Changes in v2

---

## Purpose

This register tracks technical debt in the ISWC system after the ISWC-2 upgrade. All items use story points (Fibonacci scale) for complexity estimation.

**Story Points Scale:**

- **1:** Trivial (< 4 hours)
- **2:** Small (4-8 hours, ~1 day)
- **3:** Small-Medium (1-2 days)
- **5:** Medium (2-3 days)
- **8:** Medium-Large (3-5 days, ~1 week)
- **13:** Large (1-2 weeks)
- **21:** Very Large (2-4 weeks)

---

## Dashboard

### Current State (Post-ISWC-2)

| Priority | Count | Total Story Points |
|----------|-------|-------------------|
| 🔴 CRITICAL | 2 | 17 SP |
| ⚠️ HIGH | 3 | 37 SP |
| 📋 MEDIUM | 3 | 11 SP |
| ✅ LOW | 1 | 1 SP |
| **TOTAL** | **9** | **66 SP** |

### Changes Since ISWC v1

- ✅ **Resolved:** Backend modernization (10 items, ~60 SP)
- ❌ **Introduced:** Security analyzers removed (1 new item, 4 SP)
- ⏸️ **Unchanged:** Frontend, IdentityServer4 (34 SP)

**Net Result:** ~50% reduction in backend debt, 0% reduction in frontend debt

---

## Critical Priority (17 SP)

### TD-001: IdentityServer4 End-of-Life

| Field | Value |
|-------|-------|
| **Status** | 🔴 OPEN |
| **Impact** | CRITICAL - No security patches for 3 years |
| **Story Points** | 13 |
| **Source** | Oct 30 analysis - unchanged in ISWC-2 |

**Description:** IdentityServer4 3.0.2 reached EOL November 2022.

**Options:**

- OpenIddict (free, Apache 2.0)
- Duende IdentityServer (commercial, $1,500+/year)

**Dependencies:** None - can proceed immediately

**History:**

- 2025-10-30: Identified
- 2025-11-04: Confirmed unchanged in ISWC-2

---

### TD-002: Security Analyzers Removed

| Field | Value |
|-------|-------|
| **Status** | 🔴 OPEN (REGRESSION) |
| **Impact** | HIGH - No automated OWASP scanning |
| **Story Points** | 4 (1 restore + 3 CI/CD) |
| **Source** | ISWC-2 analysis - NEW ISSUE |

**Description:** ISWC-2 removed SecurityCodeScan and Roslynator analyzers.

**Actions:**

1. Restore SecurityCodeScan or SonarAnalyzer (1 SP)
2. Restore Roslynator.Analyzers (included in #1)
3. Enable in CI/CD pipeline (3 SP)

**Dependencies:** None

**History:**

- 2025-11-04: Discovered in ISWC-2 (regression)

---

## High Priority (37 SP)

### TD-003: React 16 Frontend Outdated

| Field | Value |
|-------|-------|
| **Status** | ⚠️ OPEN |
| **Impact** | HIGH - 5 years outdated, known CVEs |
| **Story Points** | 13 |
| **Source** | Oct 30 analysis - unchanged in ISWC-2 |

**Description:** React 16.12.0 (December 2019) unchanged.

**Phased Approach:**

- React 16 → 17 (3 SP)
- React 17 → 18 (5 SP)
- Ecosystem updates (5 SP)

**Dependencies:** TypeScript upgrade (TD-004)

**History:**

- 2025-10-30: Identified as HIGH priority
- 2025-11-04: Confirmed unchanged

---

### TD-004: TypeScript 3.7 Outdated

| Field | Value |
|-------|-------|
| **Status** | ⚠️ OPEN |
| **Impact** | HIGH - 50% slower compilation |
| **Story Points** | 5 |
| **Source** | Oct 30 analysis - unchanged in ISWC-2 |

**Description:** TypeScript 3.7.3 (November 2019) unchanged.

**Phased Approach:**

- TypeScript 3.7 → 4.9 (2 SP)
- TypeScript 4.9 → 5.x (3 SP)

**Dependencies:** Should precede React upgrade (TD-003)

**History:**

- 2025-10-30: Identified
- 2025-11-04: Confirmed unchanged

---

### TD-005: react-scripts Security Vulnerabilities

| Field | Value |
|-------|-------|
| **Status** | ⚠️ OPEN |
| **Impact** | HIGH - Known CVEs |
| **Story Points** | 3 |
| **Source** | Oct 30 analysis - unchanged in ISWC-2 |

**Description:** react-scripts 3.4.4 contains known CVEs:

- @svgr/webpack: XSS
- nth-check: ReDoS
- loader-utils: ReDoS

**Options:**

- Update to react-scripts 5.0.1+ (3 SP)
- Migrate to Vite (5 SP, better long-term)

**Dependencies:** Part of frontend modernization

**History:**

- 2025-10-30: Identified as Priority 2
- 2025-11-04: Confirmed unchanged

---

## Medium Priority (11 SP)

### TD-006: Code Quality Issues (Unknown)

| Field | Value |
|-------|-------|
| **Status** | 📋 UNKNOWN |
| **Impact** | MEDIUM - Potential bugs |
| **Story Points** | 1 |
| **Source** | Oct 30 analysis - cannot verify (analyzers removed) |

**Description:** Oct 30 analysis found:

- RCS1215: Expression always true (3 locations)
- RCS1155: String comparison issues (1 location)

**Status:** Cannot verify if fixed (analyzers removed in ISWC-2)

**Dependencies:** Restore analyzers first (TD-002)

**History:**

- 2025-10-30: Identified
- 2025-11-04: Cannot verify (analyzers removed)

---

### TD-007: No Automated Dependency Management

| Field | Value |
|-------|-------|
| **Status** | 📋 OPEN |
| **Impact** | MEDIUM - Preventative |
| **Story Points** | 2 |
| **Source** | Oct 30 analysis - not visible in ISWC-2 |

**Description:** No Dependabot or Renovate Bot configured.

**Actions:**

1. Configure Dependabot (1 SP)
2. Configure auto-merge rules (1 SP)

**Benefits:** Prevents future 5-year version drift

**Dependencies:** None

**History:**

- 2025-10-30: Recommended
- 2025-11-04: Not visible in codebase

---

### TD-008: No CI/CD Vulnerability Scanning

| Field | Value |
|-------|-------|
| **Status** | 📋 UNKNOWN |
| **Impact** | MEDIUM - No shift-left security |
| **Story Points** | 8 |
| **Source** | Oct 30 analysis - not visible in ISWC-2 |

**Description:** No evidence of automated vulnerability scanning in CI/CD.

**Layers Needed:**

1. SAST (SecurityCodeScan) - see TD-002
2. Dependency scanning (OWASP, Snyk) - 5 SP
3. Container scanning (if applicable) - 3 SP

**Dependencies:** Requires CI/CD pipeline access

**History:**

- 2025-10-30: Recommended
- 2025-11-04: Cannot verify from codebase

---

## Low Priority (1 SP)

### TD-009: .editorconfig Deleted

| Field | Value |
|-------|-------|
| **Status** | ✅ OPEN |
| **Impact** | LOW - Cosmetic |
| **Story Points** | 1 |
| **Source** | ISWC-2 analysis - NEW ISSUE |

**Description:** `.editorconfig` file deleted in ISWC-2.

**Impact:** Code formatting consistency lost (cosmetic)

**Action:** Restore file with formatting rules

**Dependencies:** None

**History:**

- 2025-11-04: Discovered in ISWC-2

---

## Resolved Technical Debt (Post-ISWC-2)

### ✅ TD-RESOLVED-001: .NET Core 3.1 EOL

| Field | Value |
|-------|-------|
| **Resolved** | 2025-11-04 (ISWC-2) |
| **Value** | 13 SP |

**Action:** All 26 projects migrated to .NET 8.0 LTS (support until Nov 2026)

---

### ✅ TD-RESOLVED-002: Entity Framework Core 3.0 Outdated

| Field | Value |
|-------|-------|
| **Resolved** | 2025-11-04 (ISWC-2) |
| **Value** | 13 SP |

**Action:** Upgraded to EF Core 9.0 (40-60% performance improvement claimed)

**Note:** Cosmos DB discriminator migration required (8 SP testing)

---

### ✅ TD-RESOLVED-003: Azure Functions v3 Outdated

| Field | Value |
|-------|-------|
| **Resolved** | 2025-11-04 (ISWC-2) |
| **Value** | 8 SP |

**Action:** Migrated to v4 isolated worker model

---

### ✅ TD-RESOLVED-004: JWT Bearer CVE-2021-34532

| Field | Value |
|-------|-------|
| **Resolved** | 2025-11-04 (ISWC-2) |
| **Value** | 2 SP |

**Action:** Upgraded to 8.0.20 (includes CVE fix)

---

### ✅ TD-RESOLVED-005: Cosmos DB Preview SDK

| Field | Value |
|-------|-------|
| **Resolved** | 2025-11-04 (ISWC-2) |
| **Value** | 5 SP |

**Action:** Upgraded 3.9.1-preview → 3.53.1 stable

---

### ✅ TD-RESOLVED-006: Autofac 4.x Outdated

| Field | Value |
|-------|-------|
| **Resolved** | 2025-11-04 (ISWC-2) |
| **Value** | 5 SP |

**Action:** Upgraded to 8.4.0 (8-12% memory reduction)

**Note:** Hierarchical scopes breaking change (5 SP testing)

---

### ✅ TD-RESOLVED-007: AutoMapper 7.x Outdated

| Field | Value |
|-------|-------|
| **Resolved** | 2025-11-04 (ISWC-2) |
| **Value** | 3 SP |

**Action:** Upgraded to 12.0.1

---

### ✅ TD-RESOLVED-008: Polly 7.x Outdated

| Field | Value |
|-------|-------|
| **Resolved** | 2025-11-04 (ISWC-2) |
| **Value** | 3 SP |

**Action:** Upgraded to 8.6.3 (75% memory reduction - verified)

---

### ✅ TD-RESOLVED-009: Swashbuckle XSS Vulnerabilities

| Field | Value |
|-------|-------|
| **Resolved** | 2025-11-04 (ISWC-2) |
| **Value** | 1 SP |

**Action:** Upgraded to 9.0.4 (XSS fixes)

---

### ✅ TD-RESOLVED-010: Azure SDK Modernization (Partial)

| Field | Value |
|-------|-------|
| **Resolved** | 2025-11-04 (ISWC-2) - Partial |
| **Value** | 3 SP |

**Action:** Added modern Azure.* SDKs alongside legacy

**Remaining:** Complete migration from legacy SDKs (5 SP)

---

## Testing Requirements

### Critical Testing Areas (Post-ISWC-2)

| Area | Risk | Story Points | Status |
|------|------|--------------|--------|
| EF Core Queries | 🔴 Critical | 13 | Required before deployment |
| Cosmos DB Migration | 🔴 Critical | 8 | Required before deployment |
| Azure Functions | ⚠️ High | 8 | Required before deployment |
| Autofac Scoping | ⚠️ Medium | 5 | Recommended |
| Other Areas | ⚠️ Medium | 10 | Recommended |
| **Total** | - | **44** | - |

---

## Roadmap

### Before Production Deployment (56 SP)

1. Restore security analyzers (4 SP) - **CRITICAL**
2. Testing program (44 SP) - **CRITICAL**
3. Cosmos DB migration (8 SP) - **CRITICAL**

### Post-Deployment Phase 1 (15 SP)

1. Automated dependency management (2 SP)
2. IdentityServer4 migration planning (3 SP)
3. Frontend upgrade planning (5 SP)
4. CI/CD vulnerability scanning (5 SP)

### Post-Deployment Phase 2 (34 SP)

1. IdentityServer4 → OpenIddict (13 SP)
2. TypeScript 3.7 → 5.x (5 SP)
3. React 16 → 18 (13 SP)
4. react-scripts update (3 SP)

**Total Remaining Work:** 105 story points

---

## Change Log

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 2.0 | 2025-11-04 | Revised to use story points, focus on delta post-ISWC-2 | Teragone-Factory |
| 1.0 | 2025-11-04 | Initial technical debt register | Teragone-Factory |

---

**Next Review Date:** 2025-12-04 (monthly)
**Document Owner:** Teragone-Factory Audit Team
**Stakeholders:** CISAC (Yann Lebreuilly), Spanish Point Technology (Curnan Reidy)
