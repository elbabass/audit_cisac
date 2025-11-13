# Transcript Corrections

This directory contains resources for identifying and correcting misspellings and transcription errors in CISAC ISWC audit meeting transcripts.

## Contents

- **[glossary.md](glossary.md)** - Comprehensive glossary of misspellings found across all documentation, with corrections validated against authoritative sources (Core Design Documents, source code, specifications, email headers)
- **[meetings.md](meetings.md)** - Detailed correction list for meeting transcripts with line-level documentation (8 files, 34+ corrections)
- **[deliverables.md](deliverables.md)** - Detailed correction list for published deliverables with implementation guide (5 files, 38 corrections)
- **[project-management.md](project-management.md)** - Detailed correction list for project management documents (3 files, 49 corrections)
- **[work-in-progress.md](work-in-progress.md)** - Detailed correction list for work-in-progress analyses (4 files, 9 corrections)

## Purpose

Meeting transcripts are automatically generated from audio recordings using speech-to-text services. While generally accurate, these transcripts contain systematic errors that can confuse readers or propagate incorrect terminology into documentation.

This directory provides:

1. **Reference glossary** - Validated corrections for all identified misspellings
2. **Location-specific correction lists** - Line-by-line corrections for meetings, deliverables, project management, and WIP docs
3. **Quality assurance checklists** - Validation steps and verification commands
4. **Pattern documentation** - Common transcription errors to watch for
5. **Implementation guides** - Step-by-step instructions for applying corrections

## Usage

### For Reviewing Documents

1. **Start with the glossary** - [glossary.md](glossary.md) contains all 21 validated corrections organized by category
2. **Use location-specific lists** - Check the appropriate correction document for the type of file you're reviewing:
   - Meeting transcripts → [meetings.md](meetings.md)
   - Published deliverables → [deliverables.md](deliverables.md)
   - Project management docs → [project-management.md](project-management.md)
   - Work-in-progress analyses → [work-in-progress.md](work-in-progress.md)
3. **Prioritize CONFIRMED corrections** - These are validated against authoritative sources and should always be applied
4. **Follow implementation guides** - Each location-specific document includes step-by-step correction instructions

### For Applying Corrections

Each correction document provides:

- Exact line numbers for every error
- Context excerpts to verify you're correcting the right occurrence
- Find/replace commands for bulk corrections
- Validation checklists to verify corrections were successful
- Git commit message templates following project conventions

## Statistics

As of 2025-11-13:

### Overall Summary

- **21 unique corrections identified** across all documentation
- **130+ total correction instances** (same error appearing multiple times)
- **16 CONFIRMED corrections** (76%) - validated via source code/specifications/email headers
- **5 LIKELY corrections** (24%) - based on corrected output documents

### Breakdown by Document Location

| Location | Files Checked | Files with Errors | Total Corrections | Error Rate |
|----------|---------------|-------------------|-------------------|------------|
| Meeting Transcripts | 8 | 8 | 34+ | 100% |
| Deliverables | ~15 | 5 | 38 | 33% |
| Project Management | 14 | 3 | 49 | 21% |
| Work in Progress | 52 | 4 | 9 | 8% |
| **TOTAL** | **~89** | **20** | **130+** | **22%** |

### Most Critical Issues

1. **"Moïse" → "Moaiz"** - 36+ occurrences in deliverables, 49+ in project management (85+ total)
2. **"data breaks" → "Databricks"** - 10+ occurrences across meeting transcripts
3. **"Swiss API" → "Suisa IPI"** - Multiple occurrences in meetings and analysis docs
4. **Phonetic acronym errors** - "i supply c" → "ISWC", "c-set" → "CISAC", "C-SAT" → "C#"

### Most Common Error Patterns

1. **Phonetic acronym transcription** - ISWC, CISAC, C# misheard phonetically
2. **Homophones** - "Swiss API" → "Suisa IPI", "daily" → "dev"
3. **Similar sounds** - "data breaks" → "Databricks", "waiter" → "worker"
4. **Case inconsistency** - "cisac" → "CISAC", "databricks" → "Databricks"
5. **Name spelling** - "Moïse" → "Moaiz", "Kurnan/Curman" → "Curnan"
6. **Possessive errors** - "swan's" → "Xiyuan's"

## Recommended Correction Workflow

Based on priority and impact, here's the recommended order for applying corrections:

### Phase 1: CRITICAL - Published Deliverables (15-20 min)

**Priority:** IMMEDIATE - These are already published to CISAC

1. Review [deliverables.md](deliverables.md) for the 38 corrections needed
2. Primary issue: "Moïse" → "Moaiz" (36 occurrences)
3. Secondary issue: "Swiss API" → "Suisa IPI" (2 occurrences in diagrams)
4. Follow the implementation guide in deliverables.md
5. Estimated time: 15-20 minutes

### Phase 2: HIGH - Project Management Documents (20-30 min)

**Priority:** HIGH - These are active documents shared with CISAC

1. Review [project-management.md](project-management.md) for 49 corrections
2. Primary issue: "Moïse" → "Moaiz" (47 occurrences)
3. Includes archived presentation from piloting committee meeting
4. Estimated time: 20-30 minutes

### Phase 3: MEDIUM - Meeting Transcripts (2-3 hours)

**Priority:** MEDIUM - Source material for analysis

1. Review [meetings.md](meetings.md) for 34+ corrections across 8 files
2. Start with Workshop 2 (CRITICAL - 13+ errors)
3. Then Workshop 1 (HIGH - 4+ errors)
4. Then remaining transcripts (MEDIUM/LOW - 1-3 errors each)
5. Estimated time: 2-3 hours total

### Phase 4: LOW - Work in Progress (10-15 min)

**Priority:** LOW - Only 4 files affected, mostly quoted material

1. Review [work-in-progress.md](work-in-progress.md) for 9 corrections
2. Most are in quoted transcripts (editorial decision needed)
3. One immediate fix: "Moïse" → "Moaiz" in findings document
4. Estimated time: 10-15 minutes

**Total estimated effort:** 3-4 hours spread across phases

## Related Documentation

- Original transcripts: `/docs/meetings/`
- Source code validation: `/docs/resources/source-code/`
- Core Design Documents: `/docs/resources/core_design_documents/`
- Corrected architecture docs: `/docs/work_in_progress/architecture/`
- Documentation standards: `/docs/DOCUMENTATION_STANDARDS.md`
