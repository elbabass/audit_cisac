# Spanish Point Technical Meeting 2 - CI/CD Pipeline and DevOps

**Date:** November 5, 2025
**Duration:** 1 hour
**Attendees:** Spanish Point Technical Team (Curnan Reidy + technical staff), CISAC, Teragone-Factory
**Approach:** Open questions first, then specific deep-dives

---

## Meeting Objectives

1. **IaC Maturity Assessment** - Validate the 20-day/€20K environment extension estimate
2. **Incident Analysis** - Understand May-June 2024 merge incident and improvements made
3. **Deployment Governance** - Assess CI/CD pipeline, testing, and quality gates
4. **Technical Debt Roadmap** - Get concrete timelines for .NET and Databricks upgrades

---

## Phase 1: Open Process Questions (15 minutes)

**Goal:** Understand their deployment process and governance maturity

| Priority | Question | Goal | Context |
|----------|----------|------|---------|
| 🔴 | Can you walk us through your complete deployment process from code commit to production? What are the key stages and gates? | Understand end-to-end process and governance | CAB only started in 2024 |
| 🔴 | How do you ensure deployment quality and prevent incidents like the May-June 2024 merge issue? What changed after that incident? | Understand lessons learned and improvements | 6-month recovery period |
| ⚠️ | What's your typical deployment frequency, and how long does a deployment take from approval to completion? | Assess maturity and agility | - |
| ⚠️ | How do you handle rollbacks if a deployment causes issues? Can you share an example? | Understand risk mitigation capabilities | - |

**Follow-up requests:**

- Can you show us the Azure DevOps pipeline configuration?
- What's the deployment approval workflow?
- Can we see the CAB documentation and deployment logs?

---

## Phase 2: Infrastructure as Code and Environment Management (15 minutes)

**Goal:** Validate IaC coverage and understand the 20-day estimate gap

| Priority | Question | Goal | Context |
|----------|----------|------|---------|
| 🔴 | Can you explain the 20-day estimate for environment extension/replication? What manual work is involved vs automated? | Validate IaC maturity concern | Should be <1 day if fully automated |
| 🔴 | What percentage of the 343 Azure resources are defined in ARM templates vs manually configured? | Assess IaC coverage | Critical for vendor independence |
| 🔴 | What's the process for creating a new environment (Dev, UAT, Prod-like)? What configuration differs between environments? | Understand environment parity | - |
| ⚠️ | How do you manage secrets and connection strings across environments? Is there a rotation policy? | Assess security practices | Azure Key Vault usage |
| ⚠️ | What documentation exists for the infrastructure? How do you onboard new team members? | Assess knowledge transfer readiness | Documentation opacity concern |

**Follow-up requests:**

- ARM template repository (we have `deployment/` folder in source code)
- Infrastructure documentation or runbooks
- Environment configuration matrix (Dev vs UAT vs Prod differences)
- Azure Key Vault access policies and rotation procedures

**Key Focus:**

- **20-day estimate breakdown** - What manual steps require 20 days?
- **IaC gaps** - Which resources are not in code?
- **Environment drift** - Are Dev/UAT/Prod truly identical in configuration?

---

## Phase 3: Build and Release Pipeline (15 minutes)

**Goal:** Assess quality gates and prevent future incidents

| Priority | Question | Goal | Context |
|----------|----------|------|---------|
| 🔴 | What automated testing runs in your CI/CD pipeline (unit, integration, E2E)? What's the test coverage? | Assess quality gates | .NET 3.1 codebase |
| 🔴 | What's your branching strategy? How do you prevent unapproved code from reaching production? | Understand source control governance | May-June 2024 POC merged to main |
| ⚠️ | What build artifacts are generated and how are they versioned? Can you trace production deployments to source commits? | Assess traceability | - |
| ⚠️ | How long does the full build pipeline take? What are the bottlenecks? | Identify CI/CD performance issues | - |
| 📝 | What static analysis, security scanning, or code quality tools run in the pipeline? | Assess code quality gates | - |

**Follow-up requests:**

- Azure DevOps pipeline YAML files
- Test coverage reports (unit, integration, E2E)
- Branching model documentation (GitFlow, trunk-based, etc.)
- Build artifact retention and versioning strategy

**Key Focus:**

- **Branching strategy** - How did POC code reach production?
- **Branch protection** - What policies prevent unauthorized merges?
- **Testing coverage** - Would tests have caught the May-June 2024 issue?

---

## Phase 4: Technical Debt and Upgrade Planning (15 minutes)

**Goal:** Get concrete timelines and understand blockers

| Priority | Question | Goal | Context |
|----------|----------|------|---------|
| 🔴 | What's the plan for upgrading .NET 3.1 (EOL Dec 2022) to .NET 8/9? What's blocking the upgrade? | Assess security risk mitigation | 2.5 years past EOL |
| 🔴 | What's the timeline for Databricks runtime upgrade from 10.4 LTS to 15.4 LTS? What breaking changes need to be addressed? | Understand technical debt roadmap | Missing AI features, potential savings |
| ⚠️ | What dependency scanning and vulnerability management processes exist? How do you prioritize remediation? | Assess security maturity | Outdated tech stack concern |
| ⚠️ | What's the test strategy for major upgrades? Do you have a comprehensive regression test suite? | Assess upgrade risk management | - |
| 📝 | How do you document and communicate deployment changes to stakeholders? | Understand transparency improvements | DoD compliance |

**Follow-up requests:**

- .NET upgrade plan and timeline (if it exists)
- Databricks upgrade plan and breaking changes analysis
- Dependency scanning reports (NuGet, npm, Python packages)
- Regression test suite documentation

**Key Focus:**

- **Timelines** - When will .NET and Databricks be upgraded?
- **Blockers** - What's preventing upgrades? (technical debt, testing, budget?)
- **Security risk** - Are they aware of vulnerabilities in EOL frameworks?

---

## Meeting Execution Notes

### Pre-Meeting Preparation

- [ ] Review May-June 2024 incident details from workshop transcripts
- [ ] Understand 20-day estimate context (€20K for environment extension)
- [ ] Review ARM templates in `deployment/` folder
- [ ] Prepare to diagram the CI/CD pipeline as they explain

### During Meeting

**Do:**

- ✅ **Start with process** - Let them walk through deployment flow
- ✅ **Request live demos** - See the pipeline, show a deployment
- ✅ **Focus on learnings** - What changed after May-June 2024?
- ✅ **Be constructive** - Frame as helping improve, not blaming
- ✅ **Acknowledge challenges** - Infrastructure automation is hard
- ✅ **Take detailed notes** - Document exact process steps

**Don't:**

- ❌ **Blame for past incidents** - Focus on forward-looking improvements
- ❌ **Compare to ideal state** - Understand constraints they face
- ❌ **Rush through IaC discussion** - The 20-day estimate is critical

### Key Data Points to Capture

**Deployment Process:**

- Deployment frequency: _____ per week/month
- Average deployment duration: _____ minutes/hours
- Approval gates: _____________________________
- Rollback time: _____ minutes/hours
- Last rollback date: _____

**IaC Coverage:**

- ARM templates coverage: _____%
- Manual configuration: _____%
- Environment parity: Dev/UAT/Prod differences: _____
- Secrets management: Key Vault? Manual? _____

**Testing:**

- Unit test coverage: _____%
- Integration test coverage: _____%
- E2E test coverage: _____%
- Pipeline test execution time: _____ minutes

**Branching Strategy:**

- Model used: GitFlow / Trunk-based / Other: _____
- Branch protection rules: _____
- Code review requirements: _____
- Merge approval process: _____

**Upgrade Timeline:**

- .NET upgrade plan: _____ (Q1 2026? Q2 2026? None?)
- Databricks upgrade plan: _____ (Q1 2026? Q2 2026? None?)
- Blockers: _____________________________

### Post-Meeting Actions

- [ ] Document CI/CD pipeline flow diagram
- [ ] Create IaC maturity scorecard
- [ ] Analyze 20-day estimate breakdown
- [ ] List technical debt priorities
- [ ] Prepare findings summary for final report

---

## Expected Outcomes

By the end of this meeting, we should have:

1. ✅ **IaC Maturity Assessment** - Understanding of why environment replication requires 20 days
2. ✅ **Incident Root Cause** - Clear picture of May-June 2024 issue and corrective actions
3. ✅ **CI/CD Pipeline Documentation** - Complete understanding of build/test/deploy process
4. ✅ **Technical Debt Roadmap** - Concrete timelines for .NET and Databricks upgrades
5. ✅ **Vendor Handover Assessment** - Can another vendor take over? What's missing?

---

## Critical Questions to Answer

**For IaC Assessment:**

- Why 20 days? (Should be <1 day if fully automated)
- What resources are manually configured?
- Can we replicate environments today?

**For Incident Prevention:**

- What branch protection rules exist now?
- How did POC code reach main branch?
- What testing would have caught the issue?

**For Vendor Independence:**

- Is infrastructure documented enough for handover?
- Can another vendor deploy changes?
- What knowledge is locked in people's heads?

**For Technical Debt:**

- Why hasn't .NET been upgraded in 2.5 years?
- What's blocking Databricks upgrade?
- Are security vulnerabilities being tracked?

---

**Related Documents:**

- [Investigation Planning](Investigation-Planning.md) - Critical Priorities #2 (IaC) and #4 (Technical Debt)
- [Questions Tracker](questions-tracker.md) - Complete list of 242 technical questions
- [Workshop 1 Transcript](../meetings/20251020-ISWC Audit - Workshop 1.txt) - Lines 39:00-46:00 (pipeline demo)
- [Infrastructure Reference](../work_in_progress/infra/infrastructure-azure-reference.md) - 343 Azure resources documented
