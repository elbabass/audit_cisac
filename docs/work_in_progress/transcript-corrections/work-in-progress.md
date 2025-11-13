# Work in Progress Documents - Correction List

**Document Version:** 1.0
**Date:** 2025-11-13
**Purpose:** Identify and track corrections needed in work-in-progress documentation based on validated glossary

**Related Documents:**

- [Glossary](./glossary.md) - Master list of corrections
- [Meeting Corrections](./meetings.md) - Meeting transcript corrections
- [Deliverables Corrections](./deliverables.md) - Published deliverables corrections
- [Project Management Corrections](./project-management.md) - Project management document corrections

---

## Executive Summary

**Total WIP files analyzed:** 52 markdown files (excluding transcript-corrections folder)

**Total files with corrections:** 4 files

**Total corrections identified:** 9 corrections

**Priority Assessment:**

- **HIGH:** 1 file (contains direct quotes with misspellings)
- **MEDIUM:** 3 files (indirect quotes or references)
- **LOW:** 0 files

### Key Findings

1. **Work-in-progress documents are substantially cleaner than source transcripts** - Most WIP docs use correct terminology (CISAC, Databricks, Matching Engine, Suisa IPI, Smart AIM)
2. **Primary issue:** Quoted transcript text preserved misspellings
3. **Secondary issue:** Case inconsistencies in quoted speech ("Cisac" vs "CISAC")
4. **Positive:** No phonetic transcription errors found (no "data breaks", "Cosmos TV", "Swiss API", "AgriPortal", etc.)

### Statistics by Category

| Category | Total Corrections | Files Affected |
|----------|-------------------|----------------|
| **People Names** | 1 | 1 |
| **Organizations (case)** | 5 | 3 |
| **Product/Service Names** | 3 | 1 |
| **Total** | **9** | **4** |

---

## Quick Reference Table

| File | Total Corrections | Primary Issues | Priority | Status |
|------|-------------------|----------------|----------|--------|
| `architecture/components/matching-engine/matching-engine.md` | 5 | Cisac → CISAC (3)<br/>Match Engine → Matching Engine (2) | MEDIUM | Not corrected |
| `architecture/components/iswc-platform/cosmos-db.md` | 1 | Cisac → CISAC (1) | LOW | Not corrected |
| `architecture/integration-patterns/audit-logging.md` | 1 | Cisac → CISAC (1) | LOW | Not corrected |
| `findings/audit-trail-presentation-insert.md` | 1 | Moïse → Moaiz (1) | MEDIUM | Not corrected |

---

## Detailed Corrections by File

### 1. architecture/components/matching-engine/matching-engine.md

**File:** `/Users/bastiengallay/Dev/clients/CISAC/docs/work_in_progress/architecture/components/matching-engine/matching-engine.md`
**Total corrections:** 5
**Priority:** MEDIUM
**Document Type:** Architecture documentation (Component deep-dive)

#### Corrections Table

| Line | Current Text (excerpt) | Misspelling | Correction | Term Reference |
|------|------------------------|-------------|------------|----------------|
| 25 | `- Primary: Matching Engine, MatchEngine, Match Engine, Spanish Point Matching Engine` | Match Engine | Matching Engine | [Product/Service Names](./glossary.md#productservice-names) |
| 43 | `"The system also has an internal dependency, which is called Match Engine, non-SWC..."` | Match Engine | Matching Engine | [Product/Service Names](./glossary.md#productservice-names) |
| 43 | `"...We treat match engine as another application..."` | match engine | Matching Engine | [Product/Service Names](./glossary.md#productservice-names) |
| 800 | `"...access to both the code for especially the ISWC Cisac code and the matching engine."` | Cisac | CISAC | [Organizations](./glossary.md#organizations) |
| 802 | `"MatchEngine you currently do not have access to, because that's not part of Cisac application itself."` | Cisac | CISAC | [Organizations](./glossary.md#organizations) |
| 834 | `"MatchEngine is deployed alongside of Cisac, and the MatchEngine is a separate product..."` | Cisac | CISAC | [Organizations](./glossary.md#organizations) |

#### Context Analysis

**Line 25:** This is a "Search Terms Used" documentation section listing search pattern variations. The presence of "Match Engine" here is intentional - it documents that this variant was searched for. **ACTION:** Consider keeping this as-is with a note that "Match Engine" is an incorrect variant found in transcripts, or standardize to "Matching Engine" only.

**Lines 43, 800, 802, 834:** These are direct quotes from Workshop 2 transcript where speakers used informal capitalization ("Cisac" instead of "CISAC") and inconsistent product naming ("Match Engine" vs "Matching Engine").

**Recommendation:** These are quoted transcripts preserved for source attribution. According to documentation standards, quotes should preserve original speaker language. However, consider:

1. **Option A (Preserve as-is):** Keep quotes verbatim with [sic] notation: "...called Match Engine [sic]..."
2. **Option B (Normalize):** Update to correct terminology with editorial note: "...called Matching Engine..." (transcript normalized for clarity)
3. **Option C (Mixed):** Keep transcript quotes verbatim but update search terms list (line 25) to remove incorrect variants

**Priority justification:** MEDIUM - These are reference quotes, not primary documentation text. Readers understand these are transcribed speech.

---

### 2. architecture/components/iswc-platform/cosmos-db.md

**File:** `/Users/bastiengallay/Dev/clients/CISAC/docs/work_in_progress/architecture/components/iswc-platform/cosmos-db.md`
**Total corrections:** 1
**Priority:** LOW
**Document Type:** Architecture documentation (Component deep-dive)

#### Corrections Table

| Line | Current Text (excerpt) | Misspelling | Correction | Term Reference |
|------|------------------------|-------------|------------|----------------|
| 567 | `"So if you look at the Cisac portal, and if you look at the submission history of an Cisac for different works, we store that in Cosmos DB."` | Cisac (2×) | CISAC | [Organizations](./glossary.md#organizations) |

#### Context Analysis

**Line 567:** Direct quote from Workshop 2 (Mark Stadler) where speaker used lowercase "Cisac" in casual speech.

**Recommendation:** Same options as matching-engine.md. This is a verbatim transcript quote. Consider normalizing organization names in quotes for consistency, or preserve as-is with understanding these are transcribed speech patterns.

**Priority justification:** LOW - Single occurrence in quoted speech, not a primary documentation issue.

---

### 3. architecture/integration-patterns/audit-logging.md

**File:** `/Users/bastiengallay/Dev/clients/CISAC/docs/work_in_progress/architecture/integration-patterns/audit-logging.md`
**Total corrections:** 1
**Priority:** LOW
**Document Type:** Architecture documentation (Integration pattern analysis)

#### Corrections Table

| Line | Current Text (excerpt) | Misspelling | Correction | Term Reference |
|------|------------------------|-------------|------------|----------------|
| 385 | `"So if you look at the Cisac portal, and if you look at the submission history of an Cisac for different works, we store that in Cosmos DB."` | Cisac (2×) | CISAC | [Organizations](./glossary.md#organizations) |

#### Context Analysis

**Line 385:** Duplicate quote from cosmos-db.md (same Mark Stadler Workshop 2 quote).

**Recommendation:** Same as above - verbatim transcript quote.

**Priority justification:** LOW - Duplicate of same issue in cosmos-db.md.

---

### 4. findings/audit-trail-presentation-insert.md

**File:** `/Users/bastiengallay/Dev/clients/CISAC/docs/work_in_progress/findings/audit-trail-presentation-insert.md`
**Total corrections:** 1
**Priority:** MEDIUM
**Document Type:** Investigation notes / Finding documentation

#### Corrections Table

| Line | Current Text (excerpt) | Misspelling | Correction | Term Reference |
|------|------------------------|-------------|------------|----------------|
| 168 | `3. **For Moïse (Operations):**` | Moïse | Moaiz | [People Names](./glossary.md#people-names) |

#### Context Analysis

**Line 168:** Section header referencing Moaiz Ben Dhaou (CISAC technical expert). Uses French spelling "Moïse" (with accent) instead of correct "Moaiz".

**Impact:** This is NOT a quote - it's authored text in a finding document. This should be corrected to respect the person's actual name spelling.

**Priority justification:** MEDIUM - Incorrect person name spelling in authored (non-quoted) text. Should be corrected out of respect for the individual.

**Recommendation:** **CORRECTION REQUIRED** - Change "Moïse" to "Moaiz" on line 168.

---

## Analysis by Document Type

### Architecture Documentation (3 files)

**Files:**

- `architecture/components/matching-engine/matching-engine.md` (5 corrections)
- `architecture/components/iswc-platform/cosmos-db.md` (1 correction)
- `architecture/integration-patterns/audit-logging.md` (1 correction)

**Common Pattern:** Quoted transcript text preserves speaker's informal capitalization ("Cisac") and inconsistent product naming ("Match Engine")

**Criticality:** MEDIUM to LOW - Architecture docs are actively used and referenced, but errors are confined to quoted material. Readers understand these are transcriptions.

**Recommendation:** Consider editorial policy:

- **Option 1 (Academic style):** Preserve verbatim quotes with [sic] notation
- **Option 2 (Technical clarity):** Normalize organization and product names in quotes for consistency
- **Option 3 (Mixed):** Normalize organization names (CISAC) but preserve speaker's product name variants as-is

### Investigation Notes / Findings (1 file)

**Files:**

- `findings/audit-trail-presentation-insert.md` (1 correction)

**Common Pattern:** Person name misspelling in authored text (not quoted)

**Criticality:** MEDIUM - This is authored content, not a quote. Person names should be correct.

**Recommendation:** **IMMEDIATE CORRECTION REQUIRED** - Update "Moïse" to "Moaiz" (line 168)

---

## Correction Priority

### Critical (Immediate Action Required)

**No critical corrections.** All identified issues are in quoted transcript text or single name reference.

### High Priority (Affects Active Documentation)

**1 correction:**

- **findings/audit-trail-presentation-insert.md line 168:** "Moïse" → "Moaiz"
  - **Rationale:** Person name in authored text (not quote) should be correct
  - **Impact:** Respectful naming, professional accuracy
  - **Effort:** 1 minute (single line edit)

### Medium Priority (Quoted Material in Architecture Docs)

**7 corrections in quoted transcripts:**

- **matching-engine.md:** 5 occurrences (3× "Cisac", 2× "Match Engine")
- **cosmos-db.md:** 1 occurrence ("Cisac")
- **audit-logging.md:** 1 occurrence ("Cisac")

**Rationale for MEDIUM (not HIGH):**

1. These are verbatim transcript quotes (preserved for attribution)
2. Architecture docs are actively used, but errors are clearly in quoted speech
3. Readers understand transcripts may contain informal language
4. Correcting may reduce source attribution accuracy

**Decision Point:** Requires editorial policy decision:

- Preserve verbatim quotes (with optional [sic] notation)?
- Normalize for technical clarity?
- Case-by-case judgment?

### Low Priority (No Action Needed)

**No additional corrections.** WIP documents are substantially clean.

---

## Validation Steps

### After Making Corrections

1. **Verify person name correction:**

   ```bash
   # Should return 0 results in WIP docs
   grep -r "Moïse" docs/work_in_progress --include="*.md" --exclude-dir="transcript-corrections"
   ```

2. **If normalizing quoted material, verify consistency:**

   ```bash
   # Check for lowercase "Cisac" in WIP docs
   grep -rn "\bCisac\b" docs/work_in_progress --include="*.md" --exclude-dir="transcript-corrections"

   # Check for "Match Engine" (not "Matching Engine")
   grep -rn "Match Engine" docs/work_in_progress --include="*.md" --exclude-dir="transcript-corrections"
   ```

3. **Run markdown linting:**

   ```bash
   npx markdownlint docs/work_in_progress/findings/audit-trail-presentation-insert.md --fix
   npx markdownlint docs/work_in_progress/architecture/components/matching-engine/matching-engine.md --fix
   ```

4. **Verify no broken links:**

   ```bash
   # Check if any documentation links to corrected sections
   grep -r "audit-trail-presentation-insert" docs/ --include="*.md"
   ```

---

## Editorial Policy Recommendation

### Proposed Guidelines for Quoted Transcript Material

Based on this analysis, recommend establishing clear policy:

#### **Tier 1: Always Correct (Even in Quotes)**

- **Person names** - Always use correct spelling (Moaiz, not Moïse)
- **Critical technical misspellings** - Fix obvious transcription errors:
  - "data breaks" → "Databricks"
  - "Cosmos TV" → "Cosmos DB"
  - "Swiss API" → "Suisa IPI"
  - "AgriPortal" → "Agency Portal"

#### **Tier 2: Normalize Organization Names**

- **CISAC** - Always capitalize (even in casual quotes)
  - Rationale: Official organization name, should be consistent
  - Apply: "Cisac" → "CISAC" in all quotes

#### **Tier 3: Preserve as Verbatim (with notation)**

- **Product name variants** - Keep speaker's terminology:
  - "Match Engine" vs "Matching Engine"
  - "match engine" (lowercase) in casual speech
  - Add editorial note: "[Matching Engine]" or "[sic]" if needed

#### **Tier 4: Update Search Terms Lists**

- **Documentation metadata** - Use correct terms only:
  - Line 25 in matching-engine.md: Remove "Match Engine" from search terms
  - Use: "Primary: Matching Engine, MatchEngine" (no incorrect variants)

---

## Comparison with Other Document Categories

### Corrections by Document Category

| Category | Total Files | Files with Errors | Error Rate | Primary Issues |
|----------|-------------|-------------------|------------|----------------|
| **Meetings** | 8 transcripts | 5 files | 62.5% | All 21 glossary errors present |
| **Deliverables** | ~40 files | 1 file | 2.5% | Moïse → Moaiz (85 occurrences) |
| **Project Management** | ~15 files | 1 file | 6.7% | Swiss API → Suisa IPI (2 occurrences) |
| **Work in Progress** | 52 files | 4 files | 7.7% | Quoted transcripts (8 occurrences) |

### Key Insight

**Work-in-progress documents have the SECOND-LOWEST error rate (7.7%)** after deliverables.

**Why WIP is cleaner:**

1. ✅ **Authors consulted source code** - Used `SuisaIpi` namespace (not "Swiss API")
2. ✅ **Authors consulted specifications** - Used official Azure service names
3. ✅ **Authors normalized during analysis** - Standardized terminology while documenting
4. ✅ **Authors only preserved quotes for attribution** - Most text is authored (not transcribed)

**Exception:** Deliverables have lowest error rate (2.5%) but highest absolute count (85 occurrences of "Moïse" in a single issue).

---

## Files Confirmed Clean (Sample)

The following WIP files were checked and confirmed to use **correct terminology**:

### Architecture Documents

✅ `architecture/components/iswc-platform/agency-api.md` - Uses "Suisa IPI" (not Swiss API)
✅ `architecture/components/iswc-platform/databricks.md` - Uses "Databricks" and "Azure Data Factory" consistently
✅ `architecture/components/iswc-platform/web-portals.md` - Uses "Agency Portal" (not AgriPortal)
✅ `architecture/components/networking/sftp-server.md` - Uses correct Azure service names
✅ `architecture/c4-architecture-master.md` - Uses "Matching Engine" consistently
✅ `architecture/c4-views/level1-system-context.md` - Correct CISAC capitalization
✅ `architecture/c4-views/level2-containers.md` - Correct Azure service names

### Code Analysis Documents

✅ `code_analysis/MatchingEngine_integration.md` - Uses "Matching Engine" consistently
✅ `code_analysis/automated-analysis-2025-10-30.md` - Correct technical terminology
✅ `code_analysis/iswc-v2-upgrade-analysis-2025-11-04.md` - Correct Azure service names
✅ `code_analysis/technical-debt-tracking.md` - Uses correct "Curnan Reidy" name

### Infrastructure Documents

✅ `infra/infrastructure-azure-reference.md` - Uses "CISAC", "Azure Key Vault", "Azure Data Factory"
✅ `infra/overview/infrastructure-diagram-mermaid.md` - Correct service names
✅ `infra/overview/infrastructure-diagram-text.md` - Correct Azure terminology

### Findings Documents

✅ `findings/databricks-migration-plan.md` - Uses "Databricks" consistently
✅ `findings/cosmosdb-efficiency-findings.md` - Uses "Cosmos DB" consistently
✅ `findings/N+1-Problem-PipelineManager.md` - Correct technical terms

### Other Documents

✅ `estimates-validation-tracker.md` - Uses "Data Factory", "CISAC" correctly
✅ `quality-attributes/transferability.md` - Correct terminology
✅ `messages/SpanishPoint-MeetingTopics.md` - Professional communication

**Total clean files:** 48 out of 52 files (92.3%)

---

## Next Steps

### Immediate Actions (Required)

1. **Correct person name in findings document:**

   ```bash
   # Edit findings/audit-trail-presentation-insert.md line 168
   # Change: "3. **For Moïse (Operations):**"
   # To: "3. **For Moaiz (Operations):**"
   ```

### Policy Decision (Recommended)

2. **Establish editorial policy for quoted material:**
   - Decision needed: Normalize organization names in quotes (Cisac → CISAC)?
   - Decision needed: Preserve product name variants verbatim or normalize?
   - Decision needed: Use [sic] notation or editorial brackets?

3. **Apply policy consistently:**
   - If normalizing: Update matching-engine.md, cosmos-db.md, audit-logging.md
   - If preserving: Add editorial note explaining transcript preservation

### Optional Improvements

4. **Update search terms documentation:**
   - Remove incorrect variants from "Search Terms Used" sections
   - Or add notation: "Match Engine (incorrect variant found in transcripts)"

5. **Cross-reference validation:**
   - Verify no other documents link to corrected sections
   - Update any index files referencing these documents

---

## Glossary Term Usage Summary

### Terms Found in WIP Documents (Correct Usage)

✅ **CISAC** - Used correctly in 42+ files (proper capitalization in authored text)
✅ **Databricks** - Used correctly in 15+ files (proper capitalization)
✅ **Matching Engine** - Used correctly in 12+ files (capitalized as proper noun)
✅ **Suisa IPI** - Used correctly in agency-api.md and related docs
✅ **Smart AIM** - Used correctly (no instances of "SmartIM" found)
✅ **Agency Portal** - Used correctly (no instances of "AgriPortal" found)
✅ **Cosmos DB** - Used correctly (no instances of "Cosmos TV" found)
✅ **Azure Data Factory** - Used correctly with proper "Azure" prefix
✅ **Azure Key Vault** - Used correctly with proper "Azure" prefix
✅ **Curnan** - Used correctly in technical-debt-tracking.md

### Terms Found in WIP Documents (Incorrect Usage)

❌ **Cisac** (lowercase/mixed case) - Found in 3 quoted transcripts (5 occurrences)
❌ **Match Engine** (space, not capitalized) - Found in 1 file (3 occurrences in quotes/metadata)
❌ **Moïse** (French spelling with accent) - Found in 1 file (1 occurrence in authored text)

### Terms NOT Found in WIP Documents (Good!)

✅ **No "Swiss API"** - Correct "Suisa IPI" used throughout
✅ **No "SmartIM"** - Correct "Smart AIM" used
✅ **No "data breaks"** - Correct "Databricks" used
✅ **No "Cosmos TV"** - Correct "Cosmos DB" used
✅ **No "AgriPortal"** - Correct "Agency Portal" used
✅ **No "Kurnan" or "Curman"** - Correct "Curnan" used
✅ **No phonetic errors** - No "i supply c", "see that", "C-SAT", "waiter names", etc.

**Conclusion:** WIP documents demonstrate strong adherence to glossary standards, with only 9 corrections needed across 4 files (primarily in quoted material).

---

## Document Metadata

**Created:** 2025-11-13
**Author:** Teragone-Factory (Audit Team)
**Last Updated:** 2025-11-13
**Files Analyzed:** 52 markdown files in `docs/work_in_progress/` (excluding `transcript-corrections/`)
**Methodology:** Systematic grep search for all 21 glossary corrections across WIP folder

**Related Documentation:**

- [Glossary](./glossary.md) - Master correction list (21 corrections)
- [Meeting Corrections](./meetings.md) - Source transcript corrections (highest error density)
- [Deliverables Corrections](./deliverables.md) - Published deliverable corrections
- [Project Management Corrections](./project-management.md) - PM document corrections
- [Documentation Standards](../../DOCUMENTATION_STANDARDS.md) - Citation and quality guidelines

---
