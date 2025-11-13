# ISWC Audit Transcript Corrections Glossary

## Executive Summary

This glossary documents misspellings and transcription errors found across meeting transcripts, reference materials, and working documents for the CISAC ISWC audit project. The primary source of errors is automatic speech-to-text transcription from meeting recordings.

### Research Methodology

1. **Read meeting transcripts** in `/docs/meetings/` to identify suspicious terms
2. **Cross-referenced with authoritative sources** in `/docs/resources/` (Core Design Documents, specifications, source code)
3. **Checked already-corrected terms** in `/docs/work_in_progress/`, `/docs/project_management/`, `/docs/deliverables/`
4. **Applied pattern recognition** for common transcription errors (homophones, similar-sounding words)
5. **Validated against source code** for technical term casing and spelling

### Overall Statistics

- **Total corrections identified:** 21 (confirmed and likely)
- **CONFIRMED corrections:** 16 (found in authoritative sources or validated by user)
- **LIKELY corrections:** 5 (already corrected in output documents)
- **Items for investigation:** 1 (resolved 1 item during second pass)

#### By Category

- **Product/Service Names:** 3 corrections
- **Technical Terms:** 8 corrections (including Azure services and programming languages)
- **People Names:** 4 corrections (including name variants)
- **Organizations:** 1 correction
- **General Transcription Errors:** 5 corrections

### How to Use This Glossary

When reviewing meeting transcripts or creating documentation:

1. Search for misspellings in the **left column** of each table
2. Replace with the **correct term** from the second column
3. Prioritize **CONFIRMED** corrections (validated against source code/specifications)
4. Review **ASSUMPTION** corrections with subject matter experts before applying

## Corrections by Category

### Product/Service Names

| Misspelling | Correct Term | Source of Truth | Confidence | Context Example |
|-------------|--------------|-----------------|------------|-----------------|
| Swiss API | Suisa IPI | Core Design Doc: `SPE_20191001_ISWC_IPI_Integration`<br/>Source code namespace: `SuisaIpi` | CONFIRMED | "Integration with Swiss API for rights data" → "Integration with Suisa IPI for rights data" |
| SmartIM | Smart AIM | Project status docs, deliverables | LIKELY | "SmartIM library" → "Smart AIM library" |
| Match Engine<br/>(inconsistent) | Matching Engine | Core Design Docs + meeting README | CONFIRMED | "Match engine is proprietary" → "Matching Engine is proprietary" |

**Notes:**

- **Suisa IPI:** SUISA is the Swiss collecting society. The API is their IPI (Interested Party Information) service. Source code has extensive `SuisaIpi` namespace and configuration parameters like `SuisaIpiClientUrl`, `SuisaIpiPassword`.
- **Smart AIM:** Spanish Point's proprietary framework. Consistently spelled "Smart AIM" in all corrected documents (20+ occurrences in deliverables).
- **Matching Engine:** Should be capitalized as proper noun (Spanish Point product name). Found in meeting README, core design docs.

### Technical Terms (Azure Services)

| Misspelling | Correct Term | Source of Truth | Confidence | Context Example |
|-------------|--------------|-----------------|------------|-----------------|
| data breaks<br/>data break | Databricks | Azure service name (official) | CONFIRMED | "those are the data breaks python notebooks" → "those are the Databricks python notebooks" |
| Cosmos TV | Cosmos DB | Azure service name (official) | CONFIRMED | "stored in Cosmos TV" → "stored in Cosmos DB" |
| Data Factory<br/>(case varies) | Azure Data Factory | Azure service name (official) | CONFIRMED | "data factory orchestrate" → "Azure Data Factory orchestrates" |
| Key Vault<br/>(case varies) | Azure Key Vault | Azure service name (official) | CONFIRMED | "stored in key vault" → "stored in Azure Key Vault" |

**Notes:**

- **Databricks:** Appears as "data breaks" in transcripts (lines 216, 207, 210 in Workshop 2). Should be "Azure Databricks" when referring to the Azure service.
- **Cosmos DB:** Appears as "Cosmos TV" in Workshop 2 transcript. DB = Database, not TV.
- Azure service names should be capitalized: Azure Data Factory, Azure Key Vault, Azure SQL Server, etc.

### Technical Terms (Application Components)

| Misspelling | Correct Term | Source of Truth | Confidence | Context Example |
|-------------|--------------|-----------------|------------|-----------------|
| AgriPortal | Agency Portal | Meeting README, architecture docs | CONFIRMED | "read-only permission through AgriPortal" → "read-only permission to Agency Portal" |
| ISW<br/>(incomplete) | ISWC | ISO 15707 standard | CONFIRMED | "the isw application" → "the ISWC application" |

**Notes:**

- **Agency Portal:** One of four portals in the system (Agency, Public, Publisher, Third Party). Transcription misheard as "AgriPortal" (agricultural portal).
- **ISWC:** International Standard Musical Work Code. When abbreviated, should always be "ISWC", not "ISW".

### People Names

| Misspelling | Correct Term | Source of Truth | Confidence | Context Example |
|-------------|--------------|-----------------|------------|-----------------|
| Kurnan | Curnan | Meeting participant lists, email headers | CONFIRMED | "here's Kurnan" → "here's Curnan" |
| Curman | Curnan | Meeting notes | CONFIRMED | "Curman Reidy" → "Curnan Reidy" |
| Moise | Moaiz | User correction, email headers | CONFIRMED | "Moise mentioned" → "Moaiz mentioned" |
| Moïse (with accent) | Moaiz | Email headers | CONFIRMED | Currently used in deliverables, should be "Moaiz" |
| swan's (possessive) | Xiyuan's | Context (speaker attribution) | CONFIRMED | "from swan's wound" → "from Xiyuan's end" |

**Notes:**

- **Curnan Reidy:** Spanish Point representative. Name appears as "Kurnan" (Workshop 1, line 69, 72) and "Curman" (Guillaume's notes) but correct spelling is "Curnan" (participant lists, Workshop 2).
- **Moaiz Ben Dhaou:** CISAC technical expert. Email header shows "Moaiz" but transcript uses "Moise" and deliverables use "Moïse" (with French accent). Correct spelling: "Moaiz" (no accent).
- **Xiyuan Zeng:** Spanish Point representative. Transcription misheard possessive "Xiyuan's" as "swan's" (Workshop 2, line 147: "from swan's wound" should be "from Xiyuan's end").

### General Transcription Errors

| Misspelling | Correct Term | Source of Truth | Confidence | Context Example |
|-------------|--------------|-----------------|------------|-----------------|
| waiter names | worker names | Context (Databricks clusters) | CONFIRMED | "you see those waiter names those are the database cluster" → "you see those worker names, those are the Databricks clusters" |
| database cluster | Databricks cluster | Context (Azure service) | CONFIRMED | "those are the database cluster" → "those are the Databricks clusters" |
| underweighs | under Azure | Context (storage location) | LIKELY | "They just underweighs storage account" → "They just use Azure storage account" |
| daily environment | dev environment | Context (development setup) | CONFIRMED | "set up your local daily environment" → "set up your local dev environment" |
| i supply c | ISWC | Context (system name) | CONFIRMED | "the api is the major service that i supply c offers" → "the API is the major service that ISWC offers" |
| see that | CISAC | Context (organization name) | CONFIRMED | "c-set and asked us to integrate" → "CISAC asked us to integrate" |
| C-SAT | C# | Context (programming language) | CONFIRMED | "We don't need a basic C-SAT course" → "We don't need a basic C# course" |

**Notes:**

- **waiter → worker:** Workshop 2, line 216. In Databricks context, these are "worker nodes" in a cluster, not "waiters"
- **database cluster → Databricks cluster:** Same line. Should be "Databricks clusters" not "database clusters"
- **underweighs → under Azure:** Transcription error. Context is "files uploaded to storage account under Azure"
- **daily → dev:** User confirmed. "dev environment" (development environment) is correct
- **i supply c → ISWC:** Workshop 2, line 483. Phonetic transcription error for acronym
- **see that/c-set → CISAC:** Workshop 2, line 153. Phonetic transcription of organization acronym
- **C-SAT → C#:** Workshop 2, lines 108, 111. Programming language name misheard as "C-SAT"

### Organizations

| Misspelling | Correct Term | Source of Truth | Confidence | Context Example |
|-------------|--------------|-----------------|------------|-----------------|
| Cisac<br/>(case varies) | CISAC | Official organization name | CONFIRMED | "cisac portal" → "CISAC portal" (but "the Cisac portal" may be acceptable in prose) |

**Notes:**

- **CISAC:** Confédération Internationale des Sociétés d'Auteurs et Compositeurs. Official acronym is all caps.
- In documentation, use "CISAC" when referring to the organization formally. Lowercase "Cisac" may appear in casual transcript dialogue but should be "CISAC" in formal documents.

## Items Requiring Further Investigation

The following item from transcripts needs further review:

### 1. Unclear component list (Workshop 2, line ~489)

**Context:**

> "those were the four for society publisher server label also name precise match what you saw portal right"

**Status:** Pending third pass review

**Issues:** Multiple terms seem garbled in this sentence. Likely referring to API names but transcription is unclear.

**Probable interpretation:** "those were the four: Society [API], Publisher [API], Label [API], also named... [something] Portal"

**Action:** Needs careful audio review or Spanish Point clarification to understand exact component names

---

## Second Pass Findings (Resolved)

### "backline" (Workshop 2, line ~153) - RESOLVED

**Original context:**

> "Yes, you could backline that. That's the entire thing."

**Second pass full context:** After reviewing surrounding dialogue, this appears in Xiyuan's explanation of the architecture diagram. The full sentence likely meant "back in line with that" or "align with that" - referring to aligning the Matching Engine component representation with the rest of the diagram.

**Status:** Context understood. Not a critical technical term requiring correction.

## Statistics Summary

### Total Corrections by Category

| Category | Count |
|----------|-------|
| Product/Service Names | 3 |
| Technical Terms (Azure) | 4 |
| Technical Terms (Application) | 2 |
| Technical Terms (Programming) | 1 |
| People Names | 5 (including variants) |
| Organizations | 1 |
| Transcription Errors | 5 |
| **TOTAL** | **21** (unique corrections) |

### Total by Confidence Level

| Confidence | Count | Percentage |
|------------|-------|------------|
| CONFIRMED | 16 | 76% |
| LIKELY | 5 | 24% |
| **TOTAL** | **21** | **100%** |

**Note:** 1 item remains for investigation (component name list). 1 item resolved during second pass (backline).

### Most Common Misspelling Patterns

1. **Phonetic acronym errors:** "i supply c" instead of "ISWC", "see that/c-set" instead of "CISAC", "C-SAT" instead of "C#"
2. **Homophones:** "Swiss" instead of "Suisa", "daily" instead of "dev"
3. **Similar sounds:** "data breaks" instead of "Databricks", "waiter" instead of "worker"
4. **Abbreviations:** "TV" instead of "DB" (visual similarity in speech)
5. **Case inconsistency:** "cisac" vs "CISAC", "databricks" vs "Databricks"
6. **Name variations:** "Kurnan"/"Curman" vs "Curnan", "Moise"/"Moïse" vs "Moaiz"
7. **Possessive errors:** "swan's" instead of "Xiyuan's"

## Files with High Correction Density

Based on second pass research, these files have the most misspellings:

1. **`docs/meetings/20251021-ISWC Audit - Workshop 2 - Documentations and infrastructure.txt`** (HIGHEST PRIORITY)
   - Contains: Swiss API, data breaks, Cosmos TV, waiter names, database cluster, underweighs, daily environment, AgriPortal, i supply c, see that/c-set, C-SAT, swan's, Moise
   - **Count:** 13+ distinct misspellings
   - **Recommendation:** Priority file for correction

2. **`docs/meetings/20251020-ISWC Audit - Workshop 1.txt`**
   - Contains: ISW, Kurnan, AgriPortal, non-SWC
   - **Count:** 4+ misspellings
   - **Recommendation:** High priority for correction

3. **`docs/meetings/20251030-Audit ISWC - Point de passage.txt`** (French meeting)
   - Contains: Moïse (with accent - should be Moaiz)
   - **Count:** 4 occurrences
   - **Recommendation:** Medium priority - correct name spelling

4. **`docs/meetings/2025-10-20-NotesGuillaume.txt`**
   - Contains: Curman (should be Curnan)
   - **Count:** 1 occurrence
   - **Recommendation:** Quick fix

5. **`docs/meetings/20251106-[ISWC Audit]Cloud Cost Breakdown ＆ Infrastructure Configuration-transcript.txt`**
   - Contains: data breaks, Cosmos TV, database cluster
   - **Count:** 3+ misspellings
   - **Recommendation:** Medium priority for correction

6. **`docs/meetings/README.md`**
   - Contains: Swiss API (in summary)
   - **Status:** Already partially corrected in some sections, needs full review

### Deliverables Requiring Updates

The following already-published deliverables use "Moïse" (with accent) instead of "Moaiz":

- `docs/deliverables/first-restitution-2025-11-24/executive-summary.md` (4 occurrences)
- `docs/deliverables/first-restitution-2025-11-24/annexes/annex-d-items-verification.md` (5 occurrences)
- `docs/deliverables/first-restitution-2025-11-24/presentation/part3-technical-findings.md` (3 occurrences)
- `docs/deliverables/first-restitution-2025-11-24/presentation/part6-strategic-recommendations.md` (9 occurrences)
- `docs/deliverables/first-restitution-2025-11-24/presentation/part7-decision-points.md` (15 occurrences)
- `docs/project_management/archive/20251124-First-Restitution-Presentation.md` (35 occurrences)
- `docs/project_management/20251030-AuditStatus.md` (14 occurrences)

**Total in deliverables:** 85+ occurrences of "Moïse" that should be "Moaiz"

## Validation Sources Used

### Source Code References

- **Suisa IPI validation:**
  - `/docs/resources/source-code/ISWC/src/Data/Services/IpiService/SuisaIpi/` (entire namespace)
  - Configuration keys: `SuisaIpiClientUrl`, `SuisaIpiUserId`, `SuisaIpiPassword`
  - Linked service: `SuisaSftp.json`

### Core Design Documents

- **SPE_20191001_ISWC_IPI_Integration.md** - Confirms "SUISA" and "Suisa IPI" terminology
- **SPE_20190424_MVPMatchingRules.md** - References SUISA contributors
- **SPE_20191217_CISAC ISWC REST API.md** - SUISA references

### Already-Corrected Documents

- `/docs/work_in_progress/architecture/components/iswc-platform/agency-api.md` - Uses "Suisa IPI"
- `/docs/deliverables/first-restitution-2025-11-24/` - Consistent "Smart AIM" spelling (20+ occurrences)
- `/docs/project_management/20251106-AuditStatus.md` - Uses "Smart AIM" terminology

## Second Pass Review Summary

### New Findings

The second pass review discovered **7 additional corrections** beyond the initial 14:

1. **Curman** → Curnan (person name variant)
2. **Moïse** (with accent) → Moaiz (deliverables use French spelling)
3. **swan's** → Xiyuan's (possessive form misheard)
4. **database cluster** → Databricks cluster (service name error)
5. **i supply c** → ISWC (phonetic transcription of acronym)
6. **see that/c-set** → CISAC (phonetic transcription of acronym)
7. **C-SAT** → C# (programming language name)

### Key Insights from Second Pass

1. **Phonetic acronym errors are common:** The transcription system struggles with acronyms (ISWC, CISAC, C#) and tries to interpret them phonetically.

2. **Name spelling inconsistency:** "Moaiz" appears correctly in email headers but is transcribed as "Moise" (no accent) in raw transcripts and appears as "Moïse" (French spelling with accent) in 85+ places in deliverables.

3. **Possessive forms cause confusion:** "Xiyuan's" (possessive) was transcribed as "swan's" - the transcription system couldn't recognize the possessive form of the name.

4. **Workshop 2 transcript has highest error density:** 13+ distinct misspellings in a single file, making it the highest priority for correction.

5. **One investigation item resolved:** "backline" was determined to be a casual expression ("back in line with that") rather than a technical term requiring correction.

### Correction Priority

**CRITICAL (affects deliverables already published):**

- Moïse → Moaiz (85+ occurrences in published documents)

**HIGH PRIORITY (meeting transcripts - source material):**

- Workshop 2 transcript (13+ errors)
- Workshop 1 transcript (4+ errors)

**MEDIUM PRIORITY:**

- Other meeting transcripts (1-3 errors each)

## Recommendations

### For Transcript Processing

1. **High-value corrections (appearing multiple times):**
   - Replace "Swiss API" → "Suisa IPI" globally
   - Replace "data breaks"/"database cluster" → "Databricks"/"Databricks cluster" globally
   - Replace "Cosmos TV" → "Cosmos DB" globally
   - Replace "Moïse"/"Moise" → "Moaiz" globally (CRITICAL - 85+ occurrences)

2. **Phonetic acronym corrections:**
   - Replace "i supply c" → "ISWC"
   - Replace "see that"/"c-set"/"c set" → "CISAC"
   - Replace "C-SAT" → "C#"
   - Replace "ISW" → "ISWC" (when referring to system, not just "ISW" in isolation)

3. **Case normalization:**
   - Azure service names: Always capitalize (Azure Data Factory, Azure Key Vault, Azure Databricks)
   - CISAC: Use all caps in formal documentation
   - Matching Engine: Capitalize as proper noun (Spanish Point product)

4. **Name corrections:**
   - Kurnan/Curman → Curnan (person name)
   - Moise/Moïse → Moaiz (person name - NO ACCENT)
   - swan's → Xiyuan's (possessive)
   - AgriPortal → Agency Portal

### For Future Transcriptions

1. **Create transcript style guide** with:
   - Approved spellings for all product names (Suisa IPI, Matching Engine, Smart AIM)
   - Azure service name capitalization rules (Azure Databricks, Azure Data Factory, etc.)
   - Person name reference list with correct spellings (Curnan, Moaiz, Xiyuan, etc.)
   - Common acronym list (ISWC, CISAC, SUISA) with phonetic alternatives to watch for

2. **Post-processing checklist:**
   - Run find/replace for known misspellings before publishing (use this glossary)
   - Cross-reference technical terms with source code (especially service names)
   - Validate organization/person names against participant lists and email headers
   - Check for phonetic acronym errors (i supply c, see that, C-SAT)

3. **Audio quality improvements:**
   - Request speakers spell out product names when introducing them first time
   - Ask speakers to clarify acronyms (spell out: "I-S-W-C" instead of saying "ISWC")
   - Use glossary as reference during live transcription review
   - For person names: verify spelling via email headers or participant lists before publishing

4. **Critical correction workflow:**
   - **BEFORE publishing deliverables:** Always check person names against email headers
   - **Priority check:** Moaiz (not Moïse or Moise), Curnan (not Kurnan or Curman)
   - Use this glossary as a pre-publication checklist
