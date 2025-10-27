---
description: Create comprehensive technical documentation for an ISWC system component by synthesizing core design documents and meeting transcripts
---

# Document System Component

Please create a comprehensive technical document for **{{COMPONENT_NAME}}** in the ISWC system.

{{#if FOCUS_AREAS}}
**Focus Areas:** {{FOCUS_AREAS}}
{{/if}}

**Output Location:** `docs/work_in_progress/architecture/{{COMPONENT_NAME}}.md`

---

## Sources to Use

**Primary Sources (authoritative - source of truth):**
- Core design documents in `docs/resources/core_design_documents/`
- Search for specifications, guidelines, technical designs related to {{COMPONENT_NAME}}

**Secondary Sources (implementation context):**
- Meeting transcripts in `docs/meetings/`
- Search for implementation details, architecture discussions, current state

**Tertiary Sources (code implementation):**
- Source code in `docs/resources/source-code/ISWC/src/`
- Search for actual implementation files that reference {{COMPONENT_NAME}}

---

## Document Structure Required

### 1. Header Metadata
- Document version (start at 1.0)
- Date (today's date)
- **Sources:** Listed as:
  - **Primary:** [Core design document names]
  - **Secondary:** [Meeting transcript names]

### 2. Overview
- What is {{COMPONENT_NAME}}?
- Include direct quote from core design docs if available
- High-level purpose in the system

### 3. Primary Purpose
- Official purpose from specifications
- Key capabilities and responsibilities
- Role in the ISWC system architecture

### 4. Technical Architecture
- Infrastructure components used
- Technology stack (Azure services, languages, frameworks)
- Storage and data persistence
- Integration points with other components

### 5. How It Works
- Detailed operational description
- Data flow diagrams using ASCII art
- Step-by-step processes
- User/agency interaction model (if applicable)

### 6. File/Data Formats (if applicable)
- Naming conventions
- File types or data structures
- Format specifications from core docs
- Examples with actual patterns

### 7. Integration with Other Components
- **Upstream dependencies:** What this component depends on
- **Downstream dependencies:** What depends on this component
- **Related components:** Components it interacts with
- System architecture position

### 8. Use Cases
- Primary use cases from business perspective
- User/agency workflows
- Real-world scenarios

### 9. Workflow Details (if applicable)
- Process flows
- Error handling mechanisms
- Edge cases and special conditions

### 10. Source Code References

**IMPORTANT:** This section is mandatory and must list ALL source code files that implement or reference this component.

Structure the section as follows:

- Organize files by logical categories (e.g., "C# Framework & Integration Layer", "Python Notebooks & Scripts", "Database Objects", "Documentation & Configuration")
- For each category, create subsections with bold headers
- For each file, provide:
  - Clickable markdown link: `[filename](relative/path/to/file)`
  - Brief description of its role/purpose after the dash
  - Example: `- [DatabricksClient.cs](../../../resources/source-code/ISWC/src/Framework/Databricks/DatabricksClient.cs) - HTTP client for Databricks Jobs API`
- Add a "Key Patterns and Technologies" subsection at the end to document:
  - Technology versions used
  - Common patterns (e.g., "Job Orchestration Pattern")
  - Development approaches
  - Important configuration details

**Finding files:** Use `Grep -i "{{COMPONENT_NAME}}" docs/resources/source-code/ISWC/src` to find all references

See [Databricks.md Source Code References section](./Databricks.md#source-code-references) for a complete example

### 11. Questions for Further Investigation

- Mark answered questions: `- [x] ~~Question?~~ **ANSWERED:** Details`
- Keep open questions for code review
- Add new questions discovered during research

### 12. References

- **Core Design Documents** section with links and descriptions
- **Meeting Transcripts** section with links and dates
- **Key Information Sources** with proper citation format:
  - **Core docs:** `**[Doc Name](path) → Section X.Y "Section Title"** - Brief description`
  - **Meetings:** `**[Meeting Name](path) (Timestamp, Speaker)** - What was discussed`
- **Architecture Diagrams** with section references where they appear

### 13. Document History

- Version table: Version | Date | Author | Changes

### 14. Known Gaps and Contradictions

- Use eye-catching emojis: ⚠️ (warning), 🔔 (important), 🔍 (needs investigation), 🔴 (critical)
- Flag discrepancies between core docs and meeting discussions
- Explain contradictions clearly
- Note resolution needed
- Assess impact level (Low/Medium/High)

---

## Critical Requirements

### ✅ DO:
- **Prioritize** core design documents as authoritative source of truth
- **Include direct quotes** with proper attribution format:
  - **For core design docs:** `> **From [Doc Name](path/to/doc.md) → Section X.Y "Section Title":** "Quote"`
  - **For meeting transcripts:** `> **From [Meeting Name](path/to/transcript.txt) (Timestamp HH:MM, Speaker Name):** "Quote"`
  - Always use document links, section breadcrumbs, and section titles (not line numbers)
- **Use eye-catching emojis** to highlight contradictions, important notes, and warnings
- **Provide file references** with paths relative to repo root
- **Create ASCII diagrams** for data flows and architectures
- **Use proper markdown** with blank lines around lists for linting compliance
- **Cross-reference** related architecture documents with clickable links
- **Be specific** with technical details (exact file names, formats, protocols, endpoints)
- **Note differences** between specification and implementation

### ⛔ DON'T:
- Don't guess or infer information not present in sources
- Don't ignore contradictions - flag them prominently
- Don't omit technical details found in core docs
- Don't favor meeting notes over design documents when they conflict
- Don't create vague descriptions - be specific or note "needs investigation"
- Don't use emojis unless explicitly for warnings/contradictions

---

## Search and Research Process

**Step 1: Find Core Design Documents**

```text
Use Grep or Task tool to search for "{{COMPONENT_NAME}}" in:
- docs/resources/core_design_documents/**/*.md
Look for: specifications, guidelines, technical designs, architecture docs
```

**Step 2: Find Meeting Discussions**

```text
Use Grep or Task tool to search for "{{COMPONENT_NAME}}" in:
- docs/meetings/**/*
Look for: implementation details, discussions, current state, issues
```

**Step 3: Find Source Code References**

```text
Use Grep tool to search for "{{COMPONENT_NAME}}" (case-insensitive) in:
- docs/resources/source-code/ISWC/src/**/*
Look for: implementation files, classes, configuration, database objects
```

**Step 4: Read and Extract**

- Read all relevant core design documents completely
- Identify section structure and hierarchical organization
- Read relevant sections of meeting transcripts
- Review key source code files to understand implementation
- Extract quotes with proper attribution:
  - **Core docs:** Document name, section breadcrumb (e.g., "Section 3.2"), and section title
  - **Meetings:** Document name, timestamp, and speaker name
  - **Code:** File paths with brief descriptions
- Note contradictions and gaps

**Step 5: Cross-Reference and Synthesize**

- Identify where sources agree (reinforce these points)
- Highlight where sources contradict (flag with emojis)
- Note where specifications exist but implementation is unclear
- Flag where meetings mention details not in specs
- Document how code implementation aligns with specifications

---

## Output Quality Standards

The final document must be:

- **Comprehensive**: Cover all aspects of the component found in sources
- **Accurate**: Distinguish between specification and implementation
- **Well-Structured**: Use the exact section structure above
- **Visual**: Tables, lists, ASCII diagrams, strategic emoji use
- **Actionable**: Clear questions and gaps for follow-up
- **Traceable**: Every statement attributed to a source
- **Lint-Clean**: Proper markdown with blank lines around lists

---

## Citation Format Examples

### Core Design Document Quote

```markdown
> **From [IPI Integration Spec](../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md) → Section 3.4 "File Processing":** "The relevant information from these IPA transactions will be extracted, as per the mapping above, into a set of files that mirror the IPI tables in the ISWC database."
```

### Meeting Transcript Quote

```markdown
> **From [Workshop 2](../../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt) (1:18:19, Mark Stadler):** "If you dropped a new file into SFTP... there would be a lot of work in processing that file in Databricks."
```

### Reference Section Entry (Core Doc)

```markdown
- **[IPI Integration Spec](../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md) → Section 3.4-3.6** - File processing workflow and maintenance mode
```

### Reference Section Entry (Meeting)

```markdown
- **[Yann Discussion](../../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt) (18:44, Yann Lebreuilly)** - Outdated runtime version and missing features
```

---

## After Document Creation

1. Save to `docs/work_in_progress/architecture/{{COMPONENT_NAME}}.md`
2. Verify all markdown formatting (run linter if needed)
3. **Verify all quotes use proper citation format** (document link, section/timestamp, no line numbers)
4. **Verify Source Code References section is complete** with all relevant implementation files
5. Summary of what was documented
6. List any critical contradictions found
7. Suggest next steps for investigation

---

**Now please proceed with documenting {{COMPONENT_NAME}}.**
