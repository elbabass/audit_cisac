# Audit Status Report Generator

Create a weekly audit status report for the date: $ARGUMENTS

Instructions:

1. Parse the date from $ARGUMENTS (format: YYYYMMDD or YYYY-MM-DD or relative like "last friday", "7 days ago", "today"). Date should be converted to YYYY-MM-DD format for file naming and "Month DD, YYYY" for report header.
2. Find the most recent audit status file in `docs/project_management/` (pattern: `*AuditStatus*.md`)
3. Read the last audit status file to understand:
   - Previous progress and status
   - Blockers that may still be active
   - Budget consumption
   - Priorities from last week
4. Use git log to get all commits since the last audit status file was created:
   - Find the creation/modification date of the last audit status file using `git log`
   - Run: `git log --since="YYYY-MM-DD" --pretty=format:"%h - %s (%an, %ar)" --no-merges`
   - Analyze commit messages to identify completed **audit-relevant work only**
   - **FILTER OUT repository maintenance:** CLAUDE.md updates, linting configs, folder reorganization, index files, documentation standards
   - **INCLUDE audit deliverables:** component analysis, findings documentation, workshops, code access obtained, technical investigations
5. Create a new file at `docs/project_management/{date}-AuditStatus.md`
6. Pre-fill the report using:
   - Git commits → Identify what actually moved forward (filter for audit-relevant work, exclude repo maintenance)
   - Last status blockers → Current blockers (update status or mark resolved)
   - Last status priorities → Progress update (what moved forward)
   - Last status budget → Updated budget calculation
7. **CRITICAL: Keep the report CONCISE**
   - Target: Under 200 lines total
   - Each point appears ONCE (no redundancy)
   - Use tables for scanning, bullets for details
   - Focus on: what changed, what's decided, what's next
   - Eliminate verbose narratives and speculation
   - Only include sections with actual content
8. Mark sections that need manual review with `<!-- TODO: Review and update -->`

Template structure (CONCISE FORMAT - keep under 200 lines):

```markdown
# Audit Status - {formatted_date}

**Meeting:** {Meeting type} with {Participants} <!-- If applicable -->
**Audit Day:** {X} / 20 | **Budget Status:** 🟢/🟡/🔴 {On Track/At Risk/Behind}

---

## Status Snapshot

| Area | Progress | Key Notes |
|------|----------|-----------|
| Documentation Review | XX% | {Brief status - what changed since last week} |
| Source Code Analysis | XX% | {Brief status - what changed since last week} |
| Infrastructure Analysis | XX% | {Brief status - what changed since last week} |
| Component Documentation | XX% | {Brief status - what changed since last week} |

---

## Key Decisions

<!-- Only include if major decisions were made this week -->
- ✅ **{Decision topic}:** {Brief description of decision and rationale}
- ✅ **{Decision topic}:** {Brief description}

---

## Critical Findings

<!-- Consolidate all findings here - no separate sections unless absolutely necessary -->

### Technical

- ✅ **{Positive finding}:** {Brief description}
- ⚠️ **{Concern}:** {Brief description}
- 🔴 **{Issue}:** {Brief description with impact}

### Strategic

<!-- Only include if there are strategic-level findings -->
- **{Finding category}:** {Brief description}

### Cost & Performance

<!-- Only include if there are cost/performance findings -->
- **{Finding}:** {Brief description}

### Risks

- 🔴 **Critical - {Risk name}:** {Brief description}
- 🟡 **Medium - {Risk name}:** {Brief description}
- 🟢 **Low - {Risk name}:** {Brief description}

---

## Blockers

| Priority | Item | Owner | Impact | Mitigation |
|----------|------|-------|--------|------------|
| 🔴 High | {Blocker} | {Who} | {Impact} | {How we're addressing} |
| 🟡 Medium | {Blocker} | {Who} | {Impact} | {How we're addressing} |

---

## Action Items

### CISAC (Yann)

- [ ] {Action item}
- [ ] {Action item}

### Audit Team (Teragone-Factory)

- [ ] {Action item}
- [ ] {Action item}

### Spanish Point

- [ ] {Action item}
- [ ] {Action item}

---

## Next Week Priorities

1. **{Priority 1}:** {Brief description of what will be done}
2. **{Priority 2}:** {Brief description}
3. **{Priority 3}:** {Brief description}

---

## Budget & Timeline

**Time Consumed:** {X} / 20 days ({XX}%)
**Phase:** {Discovery/Investigation/Synthesis}
**On Track:** 🟢/🟡/🔴

**Breakdown:**

- Discovery (Docs/workshops): {X} days 🟢/🟡/🔴 {Status}
- Investigation (Code audit): {X} days 🟢/🟡/🔴 {Status}
- Synthesis (Report): {X} days 🟢/🟡/🔴 {Status}

**Key Dates:**

- {Date}: {Milestone}
- {Date}: {Milestone}

---

## Notes

<!-- Optional section for additional context -->
**What changed from last week:**

- {Key change or progress update}
- {Key change or progress update}

