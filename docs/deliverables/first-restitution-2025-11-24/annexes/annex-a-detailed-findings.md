# Annex A: Detailed Findings Reference

[← Back to Annexes Index](./index.md) | [← Back to Executive Summary](../executive-summary.md)

This annex provides pointers to the full detailed documentation created during the audit. All findings presented in the main presentation are supported by comprehensive analysis in the `docs/work_in_progress/` directory.

## Architecture Documentation

### C4 Models (4 levels)

**Location:** `docs/work_in_progress/architecture/`

- System Context, Container, Component, Code levels documented
- 12 containers, 30+ relationships mapped
- 6 major subsystems analyzed (Validation, Matching, Processing, Post-Matching, API, Portals)

### Component Deep-Dives

- **Validation Pipeline:** 95+ rules documented
- **Matching Engine Integration:** 42+ files analyzed
- **Processing Pipeline:** Databricks workflows mapped

## Code Analysis

### Integration Patterns

**Location:** `docs/work_in_progress/code_analysis/MatchingEngine_integration.md`

- REST API integration analysis
- Interface abstractions documented
- Performance coupling identified

### Upgrade Analysis

**Location:** `docs/work_in_progress/code_analysis/iswc-v2-upgrade-analysis-2025-11-04.md`

- .NET 3.1 → 8.0 changes documented
- React 16 → 18 migration details
- Security improvements cataloged

## Infrastructure

### Azure Resources

**Location:** `docs/work_in_progress/infra/infrastructure-azure-reference.md`

- 343 resources inventoried across 4 environments
- Resource categories and dependencies mapped
- Cost drivers identified

## Meeting Transcripts (Primary Sources)

All findings are traceable to these source meetings:

- **Oct 7, 2025:** Audit relaunch (`docs/meetings/20251007-recadrage.md`)
- **Oct 20, 2025:** Workshop 1 with Spanish Point
- **Oct 21, 2025:** Internal discussion revealing strategic context
- **Oct 21, 2025:** Workshop 2 - Documentation & infrastructure
- **Oct 30, 2025:** Checkpoint meeting
- **Nov 5, 2025:** Two workshops (Production/Performance + CI/CD)
- **Nov 6, 2025:** Cloud cost breakdown workshop

## Status Reports

**Location:** `docs/project_management/*-AuditStatus*.md`

Evolution of audit approach and findings documented in timestamped status reports.

---

**All references in the presentation slides link back to these source documents.**

---

[← Back to Annexes Index](./index.md) | [← Back to Executive Summary](../executive-summary.md)
