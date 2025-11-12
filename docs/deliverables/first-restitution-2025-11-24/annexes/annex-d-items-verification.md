# Annex D: Items Needing Verification

[← Back to Annexes Index](./index.md) | [← Back to Executive Summary](../executive-summary.md)

This annex clearly distinguishes between **verified facts** (high confidence, evidence-based) and **assumptions/estimates** (requiring validation).

## VERIFIED FACTS (High Confidence)

| Finding                                               | Evidence                       | Source                               |
| ----------------------------------------------------- | ------------------------------ | ------------------------------------ |
| 343 Azure resources deployed                          | Infrastructure inventory       | docs/work_in_progress/infra/         |
| .NET 3.1 EOL Dec 2022, upgraded to 8.0 Nov 4          | Microsoft docs + code analysis | docs/work_in_progress/code_analysis/ |
| 700+ tests in CI/CD pipeline                          | Nov 5 workshop testimony       | Workshop transcript                  |
| Matching Engine REST API integration                  | Code analysis (42+ files)      | MatchingEngine_integration.md        |
| May-June 2024 production incident (6 months recovery) | Yann testimony                 | Oct 21 transcript, Line 41:40        |
| CAB established May 2024                              | Yann testimony                 | Oct 21 transcript, Line 11:00        |
| €50k/month cloud costs (€600K/year)                   | Yann statement                 | Oct 21 transcript, Line 22:23        |
| No git history provided (zip format only)             | Source code delivery format    | Confirmed Nov 4                      |
| IaC templates not included (proprietary Smart AIM)    | Nov 5 workshop                 | CI/CD workshop transcript            |
| No automated cost correlation tooling                 | Spanish Point confirmation     | Nov 6 workshop, Xiyuan statement     |

## ASSUMPTIONS NEEDING VALIDATION (Medium-Low Confidence)

| Assumption                                          | Basis                               | Validation Needed                                      | Priority |
| --------------------------------------------------- | ----------------------------------- | ------------------------------------------------------ | -------- |
| **Hyperscale savings: €3,300/month**                | Spanish Point PSA5499               | Based on single month average; auto-scaling may vary   | MEDIUM   |
| **Vendor switch timeline: 12-24 months**            | Audit team estimate                 | Requires detailed vendor proposals and scoping         | HIGH     |
| **Vendor switch cost: €300-600K**                   | Audit team educated guess           | Requires detailed scoping, very low confidence         | HIGH     |
| **Knowledge transfer HIGH RISK**                    | Code review observations            | **CRITICAL: Pilot test required**                      | URGENT   |
| **ISWC API rate limit 3-5 req/sec**                 | Yann Oct 30 statement               | Not confirmed in Nov 5 workshop; need Moïse validation | MEDIUM   |
| **Cost optimization potential: 10-20%**             | Audit team estimate (€60-120K/year) | Requires detailed Azure cost breakdown                 | MEDIUM   |
| **Matching Engine replacement effort: 6-12 months** | Architecture analysis               | Depends on alternatives available (unknown)            | LOW      |
| **IaC reconstruction: 1-2 months**                  | Audit team estimate                 | Technically feasible but effort uncertain              | MEDIUM   |
| **20-day environment extension quote**              | Yann testimony                      | Need Spanish Point justification                       | MEDIUM   |

## CRITICAL UNKNOWNS (Require Investigation)

### 1. Can independent vendor maintain the code?

- **Status:** UNKNOWN
- **Validation:** €10-20K pilot test (Dec 2025 - Feb 2026)
- **Impact:** Determines vendor switch feasibility

### 2. Do alternative matching engines exist?

- **Status:** UNKNOWN
- **Validation:** Market research (Jan-Feb 2026)
- **Impact:** Determines Matching Engine lock-in severity

### 3. Is SQL Server the performance bottleneck?

- **Status:** UNVERIFIED (Spanish Point claims no issues)
- **Validation:** Meet with Moïse, review Application Insights
- **Impact:** Determines Hyperscale proposal justification

### 4. What are actual production performance metrics?

- **Status:** UNVERIFIED (no proactive dashboards shared)
- **Validation:** Request from Spanish Point or CISAC operational team
- **Impact:** Validates or refutes performance claims

### 5. What does Smart AIM library licensing cost?

- **Status:** UNKNOWN
- **Validation:** Negotiate with Spanish Point
- **Impact:** Affects vendor switch cost calculation

## ITEMS NEEDING CLARIFICATION FROM CISAC

1. **What features/improvements are agencies requesting?** (Moïse operational knowledge)
2. **What are user performance complaints?** (Moïse operational knowledge)
3. **What is acceptable pilot feature scope?** (Yann + Moïse definition)
4. **Which contract terms are highest priority?** (CISAC leadership input)
5. **Is March 2026 decision timeline feasible?** (CISAC internal planning)

## ITEMS NEEDING CLARIFICATION FROM SPANISH POINT

1. **Git history access status** (compliance review pending since Nov 5)
2. **Databricks upgrade roadmap** (version, timeline, cost)
3. **Pipeline test runner fix ETA** (urgent - blocked since Nov 4)
4. **IaC templates inclusion negotiation** (standard deliverable vs proprietary)
5. **Cost correlation tooling** (Spanish Point develops vs CISAC builds)
6. **20-day environment extension justification** (why not 0.5 days if IaC exists?)

---

[← Back to Annexes Index](./index.md) | [← Back to Executive Summary](../executive-summary.md)
