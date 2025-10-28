# Draft Communications

**Last updated:** 2025-10-28

This directory contains draft messages pending review and approval before sending to stakeholders.

## Source Code Gaps Communications (October 28, 2025)

Three variants of the same core message about missing elements in the Spanish Point source code delivery. Each variant is tailored for different recipients and communication channels.

### Variant Comparison

| File | Language | Recipients | Format | Purpose | Length |
|------|----------|-----------|--------|---------|--------|
| [Email-SourceCodeGaps.md](Email-SourceCodeGaps.md) | English | Yann (CISAC) + Curnan (Spanish Point) | Formal email | Diplomatic request to both parties | ~400 words |
| [Email-SourceCodeGaps-Yann-FR.md](Email-SourceCodeGaps-Yann-FR.md) | French | Yann (CISAC) only | Formal email | Decision framework with options | ~250 words |
| [Email-SourceCodeGaps-Yann-FR-Short.md](Email-SourceCodeGaps-Yann-FR-Short.md) | French | Yann (CISAC) only | Instant messaging | Quick alignment, schedule discussion | ~100 words |

### Content Overview

**Context:** Spanish Point delivered ISWC source code in .NET Core 3.1 format (zip file) without:

1. Current production version confirmation
2. Matching Engine source code or API specifications
3. CI/CD pipeline configurations (Azure DevOps, IaC templates)
4. Environment configuration files
5. Developer documentation (README, setup guides, ADRs)
6. Version control history (git commits, tags, branching strategy)

**Business Impact:** These gaps constrain audit depth for:

- Hyperscale proposal validation (€40K project)
- Vendor lock-in analysis (Matching Engine coupling)
- DevOps maturity assessment (IaC implementation validation)
- Cost optimization recommendations

### Usage Recommendations

**[Email-SourceCodeGaps.md](Email-SourceCodeGaps.md)** - Use when:

- First formal request to both CISAC and Spanish Point
- Need to maintain diplomatic tone with vendor
- Want to provide clear timeline and priorities (Nov 12/14/21 deadlines)
- Offering flexibility (NDAs, partial delivery acceptance)

**[Email-SourceCodeGaps-Yann-FR.md](Email-SourceCodeGaps-Yann-FR.md)** - Use when:

- Internal discussion with CISAC only (no vendor copy)
- Need to present decision framework (Option A: get materials vs Option B: adjust scope)
- Want to be direct about audit limitations
- Preparing for face-to-face scope discussion

**[Email-SourceCodeGaps-Yann-FR-Short.md](Email-SourceCodeGaps-Yann-FR-Short.md)** - Use when:

- Quick Teams/Slack message to Yann
- Want to flag the issue informally first
- Focus is on scheduling discussion (Thursday meeting or earlier)
- No need for full option framework (will discuss face-to-face)

### Tone Comparison

- **English version:** Professional, diplomatic, collaborative ("We understand IP and approval constraints")
- **French detailed:** Direct, decision-focused, less formal ("on" instead of "nous")
- **French short:** Casual, scheduling-focused, minimal context ("Salut Yann")

### Decision Notes

These variants were created after iterative refinement based on:

- Initial request for balanced positive/realistic tone
- Feedback to remove excessive formality and "caring" language
- Need for concise business impact statements
- Preference for face-to-face discussion of options

**Status:** All three drafts ready for review. Choose variant based on communication channel and desired outcome.

---

[Back to Messages Index](../index.md)
