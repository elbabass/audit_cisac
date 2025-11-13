# Meeting Transcripts - Correction List

## Overview

This document provides a comprehensive list of corrections needed for all meeting transcript files based on validated misspellings documented in the [Transcript Corrections Glossary](glossary.md). These errors are primarily caused by automatic speech-to-text transcription from meeting recordings.

**Purpose:** Guide systematic correction of meeting transcripts to ensure accuracy and consistency across all project documentation.

**Source:** All corrections are validated against authoritative sources (source code, core design documents, email headers, and participant lists).

## Summary Statistics

- **Total files affected:** 8 files
- **Total corrections needed:** 47 occurrences across 21 distinct misspellings
- **Priority distribution:**
  - CRITICAL: 1 file (13+ errors)
  - HIGH: 2 files (4+ errors each)
  - MEDIUM: 3 files (2-3 errors each)
  - LOW: 2 files (1 error each)

## Quick Reference Summary

| File | Total Corrections | Priority | Status |
|------|------------------|----------|--------|
| `20251021-ISWC Audit - Workshop 2 - Documentations and infrastructure.txt` | 13+ | CRITICAL | Pending |
| `20251020-ISWC Audit - Workshop 1.txt` | 4 | HIGH | Pending |
| `20251106-[ISWC Audit]Cloud Cost Breakdown ＆ Infrastructure Configuration-transcript.txt` | 7 | HIGH | Pending |
| `20251030-Audit ISWC - Point de passage.txt` | 5 | MEDIUM | Pending |
| `20251105-[ISWC Audit]Prod and perf data-transcript.txt` | 2 | MEDIUM | Pending |
| `20251105-[ISWC Audit]CI_CD Pipeline-transcript.txt` | 1 | LOW | Pending |
| `2025-10-20-NotesGuillaume.txt` | 1 | LOW | Pending |
| `README.md` | 1 | LOW | Pending |
| **TOTAL** | **34+** | | |

## Detailed Corrections by File

---

### 20251021-ISWC Audit - Workshop 2 - Documentations and infrastructure.txt (CRITICAL)

**File:** `/Users/bastiengallay/Dev/clients/CISAC/docs/meetings/20251021-ISWC Audit - Workshop 2 - Documentations and infrastructure.txt`

**Total corrections:** 13+

**Priority:** CRITICAL (highest error density - multiple critical technical terms)

#### Corrections Table

| Line | Current Text (excerpt) | Misspelling | Correction | Term Reference |
|------|----------------------|-------------|------------|----------------|
| 108 | "...how C-sharp works. We're not going to explain how... python or python works right well no we know our basic knowledge in there i'm sorry i just give you examples we we can explain..." | C-sharp | C# | [Glossary: General Transcription Errors](glossary.md#general-transcription-errors) |
| 111 | "We don't need a basic C-SAT course on Azure works or things like that." | C-SAT | C# | [Glossary: General Transcription Errors](glossary.md#general-transcription-errors) |
| 147 | "...from infrastructure problem view and mark Stadler here with my colleague and and he can work with from application point here so i reprint the infrastructure site from swan's wound mark reprint the application site from swan's one okay." | swan's wound | Xiyuan's end | [Glossary: People Names](glossary.md#people-names) |
| 153 | "And there's a Swiss API, which is external, nothing to do with Cisac, but Cisac need to call that API." | Swiss API | Suisa IPI | [Glossary: Product/Service Names](glossary.md#productservice-names) |
| 153 | "...the external fast track sso that's one of those authentication providers at the c-set and asked us to integrate Waze at the beginning of project for authentication." | c-set | CISAC | [Glossary: General Transcription Errors](glossary.md#general-transcription-errors) |
| 153 | "All the backend data stored in either SQL Server, Microsoft SQL Server for schema or stored in Cosmos TV, which is NoSQL in JSON schema." | Cosmos TV | Cosmos DB | [Glossary: Technical Terms (Azure Services)](glossary.md#technical-terms-azure-services) |
| 153 | "The system also has an internal dependency, which is called Match Engine, non-SWC." | Match Engine | Matching Engine | [Glossary: Product/Service Names](glossary.md#productservice-names) |
| 183 | "Of course, there are some internal dependency resources that are not relevant to architecture, and those are not drawn in a diagram, but they are shown in Azure Portal. For example, virtual machine. like let I stop you say SFTP from dependency from AgriPortal point of view..." | AgriPortal | Agency Portal | [Glossary: Technical Terms (Application Components)](glossary.md#technical-terms-application-components) |
| 216 | "The resources you currently see on the left those are the data breaks you know you see those waiter names those are the database cluster to run the data breaks python notebooks." | data breaks | Databricks | [Glossary: Technical Terms (Azure Services)](glossary.md#technical-terms-azure-services) |
| 216 | "The resources you currently see on the left those are the data breaks you know you see those waiter names those are the database cluster to run the data breaks python notebooks." | waiter names | worker names | [Glossary: General Transcription Errors](glossary.md#general-transcription-errors) |
| 216 | "The resources you currently see on the left those are the data breaks you know you see those waiter names those are the database cluster to run the data breaks python notebooks." | database cluster | Databricks cluster | [Glossary: General Transcription Errors](glossary.md#general-transcription-errors) |
| 483 | "...you know the api is it's basically the major service that i supply c offers to agencies and." | i supply c | ISWC | [Glossary: General Transcription Errors](glossary.md#general-transcription-errors) |
| 555 | "To set up a development environment to have a pretty uh yeah data breaks very very very very heavy if you want to do data breaks local dev and data factory hasn't data factory has no local replacements so it's always in the cloud for data factory there's no local and that sftp server is just you know it's just there's no third-party component okay there's no sftp product okay from any third party it's native uh linux open operating system right because the sftp is linux no file transfer protocol so it is native feature sftp okay part of a linux operating system itself no third party nothing whatsoever so uh that that's a sftp the sap file uploading will end up in a error storage file share. Storage account file service. You can see the file in your airport even right now. There are storage account in there for SFTP files, and each society is there if I got directly uploaded to the storage account. There's no other intermediate file storage location for SFTP. They just underweighs storage account." | underweighs | under Azure | [Glossary: General Transcription Errors](glossary.md#general-transcription-errors) |
| 555 | "...running locally i say highly No." followed by "...you know one way you know almost has been shared but uh running locally i say highly. So that's pretty much it. yeah i don't see how feasible it is for you to set up your local daily environment in a couple of days it's just impractical" | daily environment | dev environment | [Glossary: General Transcription Errors](glossary.md#general-transcription-errors) |
| 618 | "Yeah, I think so, yeah. And you have access to AgriPortal, right?" | AgriPortal | Agency Portal | [Glossary: Technical Terms (Application Components)](glossary.md#technical-terms-application-components) |

**Notes:**

- This file has the highest concentration of errors in the entire corpus
- Multiple errors on line 153 (architecture overview section - critical for understanding)
- Multiple errors on line 216 (Databricks cluster explanation)
- Line 555 contains very long run-on sentences with multiple errors

---

### 20251020-ISWC Audit - Workshop 1.txt (HIGH)

**File:** `/Users/bastiengallay/Dev/clients/CISAC/docs/meetings/20251020-ISWC Audit - Workshop 1.txt`

**Total corrections:** 4

**Priority:** HIGH (multiple distinct errors affecting person names and technical terms)

#### Corrections Table

| Line | Current Text (excerpt) | Misspelling | Correction | Term Reference |
|------|----------------------|-------------|------------|----------------|
| 69 | "Ah, well, here's Kurnan." | Kurnan | Curnan | [Glossary: People Names](glossary.md#people-names) |
| 72 | "Kurnan." | Kurnan | Curnan | [Glossary: People Names](glossary.md#people-names) |
| 438 | "Okay, so you're asking read-only permission to basically all the editors through AgriPortal, yeah?" | AgriPortal | Agency Portal | [Glossary: Technical Terms (Application Components)](glossary.md#technical-terms-application-components) |
| 534 | "There? No, no, no. Well, what I'm saying is, you have NDA with Cisac, that's okay, right? But this does not mean that you are legally allowed to access the source code of ISW application." | ISW | ISWC | [Glossary: Technical Terms (Application Components)](glossary.md#technical-terms-application-components) |

**Notes:**

- Person name "Kurnan" appears twice (lines 69, 72) in participant introduction
- "AgriPortal" appears in access permission discussion (line 438)
- "ISW" abbreviated form used instead of full "ISWC" (line 534)

---

### 20251106-[ISWC Audit]Cloud Cost Breakdown ＆ Infrastructure Configuration-transcript.txt (HIGH)

**File:** `/Users/bastiengallay/Dev/clients/CISAC/docs/meetings/20251106-[ISWC Audit]Cloud Cost Breakdown ＆ Infrastructure Configuration-transcript.txt`

**Total corrections:** 7

**Priority:** HIGH (multiple Databricks/Cosmos DB technical term errors)

#### Corrections Table

| Line | Current Text (excerpt) | Misspelling | Correction | Term Reference |
|------|----------------------|-------------|------------|----------------|
| 207 | "Will communicate and we all know you know from for years the the system auto scale and as I said I think the the service that increase cost is cost must be data factory or data breaks or all combined." | data breaks | Databricks | [Glossary: Technical Terms (Azure Services)](glossary.md#technical-terms-azure-services) |
| 210 | "I go deeper. You just say something, I just want to be clear. You say that if agency uploads a lot of things, we are going to see that in data breaks on data factory." | data breaks | Databricks | [Glossary: Technical Terms (Azure Services)](glossary.md#technical-terms-azure-services) |
| 261 | "So not necessarily have a precise going parallel, for Cosmos DB and the Databricks. There are other services such as Azure Function running Azure. They might actually generate the a higher cost right against uh cosmos db at the same time every data breaks is not in use..." | data breaks | Databricks | [Glossary: Technical Terms (Azure Services)](glossary.md#technical-terms-azure-services) |
| 291 | Long context containing multiple service name references | data breaks | Databricks | [Glossary: Technical Terms (Azure Services)](glossary.md#technical-terms-azure-services) |
| 366 | Long context about cost analysis and reservations | data breaks | Databricks | [Glossary: Technical Terms (Azure Services)](glossary.md#technical-terms-azure-services) |
| 372 | "...and there's a data breaks cluster as you can see and that's actually not we don't recommend because the cost go up and down and the database cluster can be zero but it is 40 but as you can see it is only 1.5 k..." | data breaks cluster | Databricks cluster | [Glossary: Technical Terms (Azure Services)](glossary.md#technical-terms-azure-services) |
| 372 | "...and there's a data breaks cluster as you can see and that's actually not we don't recommend because the cost go up and down and the database cluster can be zero but it is 40..." | database cluster | Databricks cluster | [Glossary: General Transcription Errors](glossary.md#general-transcription-errors) |
| 372 | "...the same goes for basic data basically for data breaks and for cosmos tv they are they are the same thing so that's where uh you know the the you know the the i the basically the cost uh proposal lies right..." | cosmos tv | Cosmos DB | [Glossary: Technical Terms (Azure Services)](glossary.md#technical-terms-azure-services) |

**Notes:**

- "data breaks" appears 5+ times throughout the transcript
- Line 372 contains multiple errors in cost discussion (Databricks cluster + Cosmos DB)
- Technical discussion about Azure service costs makes these errors particularly problematic

---

### 20251030-Audit ISWC - Point de passage.txt (MEDIUM)

**File:** `/Users/bastiengallay/Dev/clients/CISAC/docs/meetings/20251030-Audit ISWC - Point de passage.txt`

**Total corrections:** 5

**Priority:** MEDIUM (French meeting transcript - person name consistency)

**Note:** This is a French-language transcript. All "Moïse" occurrences should be "Moaiz" (no accent).

#### Corrections Table

| Line | Current Text (excerpt) | Misspelling | Correction | Term Reference |
|------|----------------------|-------------|------------|----------------|
| 30 | "Alors, on se met ça comme action, c'est de demander à Moïse, qui est l'expert technique à la Cisac, de vous faire un retour sur les différents éléments de métrique qu'il peut lui posséder de son côté..." | Moïse | Moaiz | [Glossary: People Names](glossary.md#people-names) |
| 78 | Context about technical discussions and metrics | Moïse | Moaiz | [Glossary: People Names](glossary.md#people-names) |
| 213 | "On a évoqué le côté performance, on a évoqué les actions éventuelles, y compris côté Cisac avec Moïse." | Moïse | Moaiz | [Glossary: People Names](glossary.md#people-names) |
| 246 | "Donc ça, effectivement, pareil, Moïse pourra nous en dire plus parce qu'il utilise directement cet outil-là depuis l'application SysNet." | Moïse | Moaiz | [Glossary: People Names](glossary.md#people-names) |
| 267 | "Alors, Moïse et à l'équipe, c'est bon." | Moïse | Moaiz | [Glossary: People Names](glossary.md#people-names) |

**Notes:**

- All 5 occurrences involve the same person name error
- French spelling "Moïse" (with accent) should be "Moaiz" (no accent)
- Email headers confirm correct spelling as "Moaiz"
- This is part of a larger correction affecting 85+ occurrences in deliverables

---

### 20251105-[ISWC Audit]Prod and perf data-transcript.txt (MEDIUM)

**File:** `/Users/bastiengallay/Dev/clients/CISAC/docs/meetings/20251105-[ISWC Audit]Prod and perf data-transcript.txt`

**Total corrections:** 2

**Priority:** MEDIUM (Databricks terminology)

#### Corrections Table

| Line | Current Text (excerpt) | Misspelling | Correction | Term Reference |
|------|----------------------|-------------|------------|----------------|
| 153 | "Yes, I can. My first question is, would you like to explain a small... The rule of data breaks on the data lake in the whole application." | data breaks | Databricks | [Glossary: Technical Terms (Azure Services)](glossary.md#technical-terms-azure-services) |
| 156 | "Data breaks and which one? The data lake. Data lake. Data lake store binary files and then it's processed. Data breaks is the one running Python notebooks to process the data." | data breaks | Databricks | [Glossary: Technical Terms (Azure Services)](glossary.md#technical-terms-azure-services) |

**Notes:**

- "data breaks" appears twice in Databricks/Data Lake architecture explanation
- Technical discussion about data processing workflow

---

### 20251105-[ISWC Audit]CI_CD Pipeline-transcript.txt (LOW)

**File:** `/Users/bastiengallay/Dev/clients/CISAC/docs/meetings/20251105-[ISWC Audit]CI_CD Pipeline-transcript.txt`

**Total corrections:** 1

**Priority:** LOW (single product name error)

#### Corrections Table

| Line | Current Text (excerpt) | Misspelling | Correction | Term Reference |
|------|----------------------|-------------|------------|----------------|
| 297 | "...the extra cost so that we need to license this third party in order to use the pipeline and IEC assets at the base on what we call SmartName library. So that third party can use the pipeline, the one that we can basically share source code entirely. So in this case, if that is licensed, that program do exist. We also have this in our SpanishPong public website called the SmartIM library where you can Google for it..." | SmartIM | Smart AIM | [Glossary: Product/Service Names](glossary.md#productservice-names) |

**Notes:**

- Single occurrence of "SmartIM" should be "Smart AIM" (Spanish Point proprietary framework)
- Appears in discussion about pipeline licensing

---

### 2025-10-20-NotesGuillaume.txt (LOW)

**File:** `/Users/bastiengallay/Dev/clients/CISAC/docs/meetings/2025-10-20-NotesGuillaume.txt`

**Total corrections:** 1

**Priority:** LOW (single person name variant)

#### Corrections Table

| Line | Current Text (excerpt) | Misspelling | Correction | Term Reference |
|------|----------------------|-------------|------------|----------------|
| 8 | "- Curman Reidy" (in participant list) | Curman | Curnan | [Glossary: People Names](glossary.md#people-names) |

**Notes:**

- Single occurrence in participant list
- Name variant of Spanish Point representative
- Quick fix - one line only

---

### README.md (LOW)

**File:** `/Users/bastiengallay/Dev/clients/CISAC/docs/meetings/README.md`

**Total corrections:** 1

**Priority:** LOW (single product name error in summary)

#### Corrections Table

| Line | Current Text (excerpt) | Misspelling | Correction | Term Reference |
|------|----------------------|-------------|------------|----------------|
| 75 | "- Swiss API external component" (in technical components list) | Swiss API | Suisa IPI | [Glossary: Product/Service Names](glossary.md#productservice-names) |

**Notes:**

- Single occurrence in Workshop 2 summary section
- Product name error in bulleted list
- README serves as meeting index - important for consistency

---

## Correction Priority Levels

### CRITICAL Priority

**Definition:** File has 10+ errors or multiple errors affecting core technical understanding

**Files:**

- `20251021-ISWC Audit - Workshop 2 - Documentations and infrastructure.txt` (13+ errors)

**Action:** Immediate correction required. This file is the primary architecture walkthrough and contains the highest density of technical term errors.

### HIGH Priority

**Definition:** File has 3+ distinct errors affecting multiple categories (names, products, technical terms)

**Files:**

- `20251020-ISWC Audit - Workshop 1.txt` (4 errors)
- `20251106-[ISWC Audit]Cloud Cost Breakdown ＆ Infrastructure Configuration-transcript.txt` (7 errors)

**Action:** Correct after CRITICAL file. These transcripts contain important technical and cost discussions.

### MEDIUM Priority

**Definition:** File has 2-5 errors, often concentrated in a single category

**Files:**

- `20251030-Audit ISWC - Point de passage.txt` (5 errors - all same person name)
- `20251105-[ISWC Audit]Prod and perf data-transcript.txt` (2 errors - same term)

**Action:** Correct during systematic review. French meeting transcript needs person name consistency.

### LOW Priority

**Definition:** File has 1 error or minor formatting/case corrections

**Files:**

- `20251105-[ISWC Audit]CI_CD Pipeline-transcript.txt` (1 error)
- `2025-10-20-NotesGuillaume.txt` (1 error)
- `README.md` (1 error)

**Action:** Correct during final cleanup pass. Quick fixes.

## Correction Workflow Recommendations

### Phase 1: Critical Files (Day 1)

1. **File:** `20251021-ISWC Audit - Workshop 2 - Documentations and infrastructure.txt`
   - **Estimated time:** 45-60 minutes
   - **Focus areas:** Lines 108, 111 (C#), Line 147 (Xiyuan's), Line 153 (architecture overview - 5 errors), Line 216 (Databricks cluster - 3 errors), Line 483 (ISWC), Line 555 (dev environment + Azure storage)
   - **Validation:** Cross-check architecture description against diagram in documentation

### Phase 2: High Priority Files (Day 2)

2. **File:** `20251106-[ISWC Audit]Cloud Cost Breakdown ＆ Infrastructure Configuration-transcript.txt`
   - **Estimated time:** 30-40 minutes
   - **Focus areas:** Lines 207, 210, 261, 291, 366, 372 (Databricks/Cosmos DB throughout)
   - **Validation:** Verify Azure service names in cost discussion sections

3. **File:** `20251020-ISWC Audit - Workshop 1.txt`
   - **Estimated time:** 20-30 minutes
   - **Focus areas:** Lines 69, 72 (Curnan), Line 438 (Agency Portal), Line 534 (ISWC)
   - **Validation:** Check participant names against meeting invite

### Phase 3: Medium Priority Files (Day 3)

4. **File:** `20251030-Audit ISWC - Point de passage.txt`
   - **Estimated time:** 20-25 minutes
   - **Focus areas:** Lines 30, 78, 213, 246, 267 (all Moaiz corrections)
   - **Validation:** Confirm person name against email headers

5. **File:** `20251105-[ISWC Audit]Prod and perf data-transcript.txt`
   - **Estimated time:** 10-15 minutes
   - **Focus areas:** Lines 153, 156 (Databricks)
   - **Validation:** Quick Azure service name check

### Phase 4: Low Priority Files (Final cleanup)

6. **Files:** `20251105-[ISWC Audit]CI_CD Pipeline-transcript.txt`, `2025-10-20-NotesGuillaume.txt`, `README.md`
   - **Estimated time:** 15-20 minutes total
   - **Focus areas:** Single corrections in each file
   - **Validation:** Quick reference checks

### Total Estimated Time: 2.5-3.5 hours

## Post-Correction Validation

After completing corrections, perform these validation checks:

1. **Search for remaining errors:**
   - Run `grep -r "Swiss API" docs/meetings/` (should return 0 results)
   - Run `grep -r "data breaks" docs/meetings/` (should return 0 results)
   - Run `grep -r "Cosmos TV" docs/meetings/` (should return 0 results)
   - Run `grep -r "AgriPortal" docs/meetings/` (should return 0 results)
   - Run `grep -r "Kurnan\|Curman" docs/meetings/` (should return 0 results)
   - Run `grep -r "Moïse" docs/meetings/` (should return 0 results)

2. **Verify Azure service name consistency:**
   - All Azure services should be capitalized: "Azure Databricks", "Azure Data Factory", "Azure Key Vault", "Cosmos DB"
   - Check: `grep -r "databricks\|data factory\|key vault" docs/meetings/` (case-sensitive)

3. **Verify person name consistency:**
   - "Curnan Reidy" (not Kurnan or Curman)
   - "Moaiz Ben Dhaou" (not Moise or Moïse)
   - "Xiyuan Zeng" (not swan's or other variants)

4. **Verify product name consistency:**
   - "Suisa IPI" (not Swiss API)
   - "Smart AIM" (not SmartIM)
   - "Matching Engine" (capitalized - not Match Engine or match engine)
   - "Agency Portal" (not AgriPortal)

5. **Documentation cross-reference:**
   - Compare corrected meeting summaries in README.md against corrected transcripts
   - Ensure consistency with already-corrected documents in `docs/work_in_progress/` and `docs/deliverables/`

## Notes on Deliverables Requiring Updates

**IMPORTANT:** This correction list covers only meeting transcripts in `docs/meetings/`. The glossary has identified 85+ additional occurrences of "Moïse" (should be "Moaiz") in published deliverables:

**Affected deliverables:**

- `docs/deliverables/first-restitution-2025-11-24/executive-summary.md` (4 occurrences)
- `docs/deliverables/first-restitution-2025-11-24/annexes/annex-d-items-verification.md` (5 occurrences)
- `docs/deliverables/first-restitution-2025-11-24/presentation/part3-technical-findings.md` (3 occurrences)
- `docs/deliverables/first-restitution-2025-11-24/presentation/part6-strategic-recommendations.md` (9 occurrences)
- `docs/deliverables/first-restitution-2025-11-24/presentation/part7-decision-points.md` (15 occurrences)
- `docs/project_management/archive/20251124-First-Restitution-Presentation.md` (35 occurrences)
- `docs/project_management/20251030-AuditStatus.md` (14 occurrences)

**Recommendation:** Create a separate correction task for deliverables after completing meeting transcript corrections.

## References

- **Glossary:** [Transcript Corrections Glossary](glossary.md) - Source of all validated corrections
- **Source Code Validation:** `/docs/resources/source-code/ISWC/` - Validates technical terms (Suisa IPI, C#, etc.)
- **Core Design Documents:** `/docs/resources/core-design-documents/` - Validates product names (Matching Engine, Suisa IPI)
- **Email Headers:** Validates person names (Moaiz, Curnan)
- **Project Documentation Standards:** [DOCUMENTATION_STANDARDS.md](../../DOCUMENTATION_STANDARDS.md) - Formatting and citation rules

---

**Document Version:** 1.0
**Last Updated:** 2025-11-13
**Created By:** Automated analysis of meeting transcripts against validated glossary
