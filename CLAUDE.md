# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Context

This repository contains documentation for a CISAC ISWC (International Standard Musical Work Code) system audit conducted by Teragone-Factory. The project involves:

- Auditing a complex Azure-based distributed system for music rights management
- Analyzing architecture, infrastructure, source code, and vendor dependencies
- Creating comprehensive technical documentation from specifications, meetings, and source code
- Providing strategic recommendations for system modernization and vendor independence

**Project Stakeholders:**

- **Client:** CISAC (main contact: Yann Lebreuilly)
- **Vendor:** Spanish Point Technology - original developer and maintainer (main contact: Curnan Reidy)
- **Auditor:** Teragone-Factory (technical consultants: Guillaume Jay and Bastien Gallay)

**Project Timeline:**

- **Budget:** 20 man-days (10 days per consultant)
- **Approach:** Three phases: Discovery → Investigation → Synthesis
- **Weekly review meetings** with CISAC to review discoveries, manage blockers, and re-adjust priorities

**Detailed timeline:** See [PROJECT_TIMELINE.md](docs/PROJECT_TIMELINE.md)

**Repository Purpose:**

This repository serves as the central workspace for the audit, containing:

- Reference materials (READ-ONLY in `resources/`)
- Meeting transcripts and notes (`meetings/`)
- Project planning and tracking (`project_management/`)
- Active analysis and investigation work (`work_in_progress/`)
- Original audit proposal (`proposal/`)

## Development Environment

This is a **MkDocs documentation site** using Python 3.12+ with Material theme.

**Setup:** See [README.md](README.md) for installation and development instructions.

**Linting:** Run `npx markdownlint docs/**/*.md` (configuration in `.markdownlint.json`)

## Git Commit Policy

**Commit message format:** Use conventional commit prefixes:

- `doc:` - Documentation changes
- `feat:` - New features
- `fix:` - Bug fixes
- `refactor:` - Code refactoring
- `chore:` - Maintenance tasks
- `test:` - Test additions or changes

**Important:** Do not add Claude signatures or AI-generated attributions to commit messages (no "Generated with Claude Code" or "Co-Authored-By: Claude"). Keep commits professional and attribution-free

## Repository Architecture

### Five-Layer Documentation Structure

1. **`docs/resources/`** - Reference materials (READ-ONLY)
   - Core design documents (authoritative source of truth)
   - Source code, performance propositions, converted PDFs
   - **CRITICAL:** Never generate or edit resources/ (except indexing/conversion)

2. **`docs/meetings/`** - Meeting documentation
   - Workshop transcripts and client meeting notes

3. **`docs/project_management/`** - Project planning and tracking
   - Investigation-Planning.md (master audit backlog)
   - Status tracking documents (dated snapshots)

4. **`docs/work_in_progress/`** - Active audit analyses (not in MkDocs navigation)
   - architecture/ - Component deep-dives
   - code_analysis/ - Integration patterns
   - infra/ - Azure infrastructure inventory
   - messages/ - Communications and correspondence

5. **`docs/proposal/`** - Original audit proposal

**Documentation standards:** All subfolders must have `index.md` files. See [DOCUMENTATION_STANDARDS.md](docs/DOCUMENTATION_STANDARDS.md) for policies and formats.

**MkDocs navigation:** Defined in `mkdocs.yml`. The `work_in_progress/` directory is private (not published).

## Custom Slash Commands

### /document-component

Create comprehensive technical documentation for ISWC system components by synthesizing specifications, meetings, and source code.

**Usage:**

```plaintext
/document-component COMPONENT_NAME="Databricks" FOCUS_AREAS="IPI processing workflow"
```

**Output:** `docs/work_in_progress/architecture/COMPONENT_NAME.md`

**Full methodology:** See [DOCUMENTATION_STANDARDS.md](docs/DOCUMENTATION_STANDARDS.md#component-documentation-methodology) for the three-phase research process and required document structure.

## Source Code Analysis

**Technology Stack:**

- Frontend: React 16.12.0 + TypeScript 3.7.3 + Redux 4.0.4
- Backend: ASP.NET Core 3.1 (EOL Dec 2022 - flag as technical debt)
- Data Processing: Azure Databricks 10.4 LTS (outdated - flag as technical debt)
- Databases: Azure SQL Server + Cosmos DB (MongoDB API)
- Integration: Matching Engine (external Spanish Point product - REST API)

**Search patterns and integration documentation guidelines:** See [DOCUMENTATION_STANDARDS.md](docs/DOCUMENTATION_STANDARDS.md#source-code-analysis)

## Investigation Planning

**Master backlog:** [Investigation-Planning.md](docs/project_management/Investigation-Planning.md)

This document contains:

- High-level audit priorities with day allocations
- Component investigation plan
- Investigation methodology
- Scope management and critical priorities

**Note:** This plan serves as a living backlog, reviewed weekly with CISAC.

## Progress Tracking

**Status reports:** Dated snapshots in `docs/project_management/`

- Pattern: `YYYYMMDD-AuditStatus.md` (quick snapshot)
- Pattern: `YYYYMMDD-AuditStatus-Analysis.md` (comprehensive findings)

**Latest status:** Check `docs/project_management/` directory for most recent files.

**Update status documents:**

- After major milestones or discoveries
- Before weekly CISAC meetings
- When significant blockers are encountered
- After each restitution delivery

## Key Technical Focus Areas

**For latest findings and specific details, see status reports in `docs/project_management/`**

**Critical areas being investigated:**

1. Hyperscale proposal validation (Performance Proposition PSA5499)
2. IaC & DevOps maturity assessment
3. Matching Engine technical coupling and vendor dependency
4. Technical debt and upgrade paths (ASP.NET Core 3.1 EOL, Databricks outdated)
5. Cost optimization opportunities

**Note:** Findings evolve weekly - always check latest status documents for current details.

## Working with Documentation

**Adding architecture documents:**

1. Use `/document-component` slash command
2. Follow three-phase research process
3. Update relevant `index.md` files

**Updating investigation planning:**

- Keep aligned with 20-day budget
- Mark completed items with ✅
- Track progress weekly with CISAC feedback

**Citation rules, quality standards, and diagram guidelines:** See [DOCUMENTATION_STANDARDS.md](docs/DOCUMENTATION_STANDARDS.md)
