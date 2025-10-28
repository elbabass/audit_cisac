# Audit Status Report Generator

Create a weekly audit status report for the date: $ARGUMENTS

Instructions:

1. Parse the date from $ARGUMENTS (format: YYYYMMDD or YYYY-MM-DD)
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
   - Git commits → Deliverables section (filter for audit-relevant work, exclude repo maintenance)
   - Last status blockers → Current blockers (update status or mark resolved)
   - Last status priorities → Progress update (what moved forward)
   - Last status budget → Updated budget calculation
7. Use the template below, adapting it to be concise and visual
8. Mark sections that need manual review with `<!-- TODO: Review and update -->`

Template structure:

```markdown
# Audit Status - {formatted_date}

## Progress Overview

| Category | Status | Notes |
|----------|--------|-------|
| Documentation Review | ⚪️ 0% / 🔵 25% / 🟡 50% / 🟢 75% / ✅ 100% | Brief note |
| Source Code Access | ⚪️ Blocked / 🟡 Pending / ✅ Granted | Brief note |
| Infrastructure Analysis | ⚪️ Not started / 🔵 In progress / ✅ Complete | Brief note |
| Vendor Meetings | {count} sessions | Brief note |

---

## 📦 Deliverables This Week

- ✅ {Completed item}
- 🔄 {In progress item}
- 📅 {Planned for next week}

---

## 🚧 Blockers

| Priority | Item | Owner | Impact |
|----------|------|-------|--------|
| 🔴 High | {blocker description} | {Spanish Point/CISAC} | {what's blocked} |
| 🟡 Medium | {blocker description} | {owner} | {impact} |

---

## 💡 Key Findings

### Technical Discoveries
- {Finding with reference to source doc/meeting}

### Risks Identified
- 🔴 Critical: {risk description}
- 🟡 Medium: {risk description}
- 🟢 Low: {risk description}

---

## 📋 Next Week Priorities

1. [ ] {Priority task with expected outcome}
2. [ ] {Priority task with expected outcome}
3. [ ] {Priority task with expected outcome}

---

## 📊 Budget Status

- **Days consumed:** {X} / 20 days
- **Days remaining:** {Y} days
- **Burn rate:** On track / Ahead / Behind

