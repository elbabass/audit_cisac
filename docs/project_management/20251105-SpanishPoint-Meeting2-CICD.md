# Spanish Point Technical Meeting 2 - CI/CD Pipeline and DevOps

**Date:** November 5, 2025
**Duration:** 46 minutes
**Attendees:**

- Spanish Point: Xiyuan Zeng, Nicholas Randles
- Teragone-Factory: Bastien Gallay, Guillaume Jay

**Meeting Type:** Technical deep-dive with screen sharing

---

## Meeting Summary

This meeting focused on understanding Spanish Point's CI/CD pipeline implementation, testing practices, deployment governance, and Infrastructure as Code (IaC) approach. The discussion revealed that the pipeline and IaC definitions are proprietary assets based on Spanish Point's SmartM library and not included in the ISWC source code delivery. Spanish Point provided a live demonstration of their Azure DevOps pipeline and explained their deployment process, branching strategy, and licensing model for third-party access to their pipeline assets

---

## Topics Discussed

### 1. CI/CD Pipeline Access and Ownership

**Question:** Can we have access to CI/CD? If we can't, can you explain why it's an issue?

**Answer:**

- The entitlement is for CISAC to receive the source code of the ISWC application, which has been provided
- The Infrastructure as Code (IaC) and CI/CD pipeline definitions are **not part of the ISWC application**
- These assets are **proprietary** and based on Spanish Point's **SmartM library**
- The ISWC application itself does not need IaC or CI/CD pipeline definitions to be executed or deployed
- Spanish Point offered screen sharing demonstrations as an alternative to direct access

### 2. Deployment Process Walkthrough

**Question:** Can you walk us through your complete deployment process from code commit to production? What are the key stages and gates?

**Answer:**

Spanish Point demonstrated their Azure DevOps pipeline with the following stages:

**Pipeline Stages:**

1. **CISAC Build** (4-5 minutes)
   - Unit Tests JSON (28s)
   - Unit Tests Python (1m 3s)
   - Unit Tests .NET Core (4m 28s)
   - Resource Manager (26s)
   - Scripts (24s)
   - Databricks (37s)
   - Sites Web ApiAgency (1m 28s)
   - Sites Web ApiLabel (1m 33s)
   - Sites Web ApiPublisher (1m 25s)
   - Sites Web ApiThirdParty (1m 23s)
   - Sites Web Portal Public (3m 33s)
   - Sites Web Portal Private (3m 31s)
   - Sites Function Jobs (2m 3s)
   - Data Factory (1m 44s)
   - Test result: 99.5% tests passed, 12 artifacts generated

2. **CISAC Development** (skipped in demo)
   - Multiple deployment jobs for different components
   - Environment-specific configuration deployment

3. **CISAC User Acceptance** (30m 21s total)
   - 33 jobs completed
   - 76.6% tests passed
   - 1 check passed
   - Deployment jobs include:
     - Management Resource Manager
     - Core Resource Manager
     - Management Automation Runner
     - ISWC Resource Manager
     - ISWC SFTP
     - ISWC Data Factory Key Vault
     - ISWC Databricks Access Token
     - ISWC Sites Key Vault secrets
     - ISWC Databricks notebooks
     - ISWC Tests (Ready Check, Sites Portal, Databricks EDI, Integration tests)
     - Management Monitor

4. **CISAC Production** (skipped in demo)
   - Same deployment pattern as UAT
   - Requires manual approval

**Deployment Characteristics:**

- **Trigger:** Automatic on PR merge to master branch
- **Build Duration:** ~4-5 minutes
- **Full Deployment Duration:** ~27-30 minutes per environment
- **Approval Required:** Yes, for all environment deployments (Dev, UAT, Production)
- **Manual Steps:** Only the approval button click; everything else is automated
- **Deployment Frequency:** Based on agreement with CISAC per feature implementation

### 3. Testing Strategy and Quality Gates

**Question:** What automated testing runs in your CI/CD pipeline (unit, integration, E2E)? What's the test coverage?

**Answer:**

**Test Types:**

- **Unit tests:** JSON, Python, .NET Core (executed during build stage)
- **Integration tests:** Run during deployment stages
- **UI tests:** Automated browser-based tests (not calling APIs, launching browsers)
- **Test execution:** Fully automated, no manual test marking

**Test Status Observations:**

- One pipeline run showed 99.5% pass rate (729 total tests, 719 passed, 7 failed)
- A more recent run showed 100% pass rate
- Some test failures attributed to:
  - Recent .NET 8 upgrade in progress (still updating tests)
  - Networking issues (since resolved)
  - Some older integration tests can be "brittle" (timeout issues, need reruns)
- Tests may be conditionally executed based on environment configuration

**Test Management:**

- Tests are not manually marked as "okay to fail"
- Dedicated QA team manages test suites
- Test updates aligned with feature updates
- Open bug on Azure DevOps board for pipeline test updates post-.NET 8 upgrade
- Workaround: Tests run locally against deployed APIs when pipeline tests are blocked

**Test Code Access:**

- Test source code is available in the shared repository

### 4. Branching Strategy and PR Policy

**Question:** What's your branching strategy? How do you prevent unapproved code from reaching production?

**Answer:**

**Branching Model:**

- Standard Git flow using master/main branch
- Developers create task-specific feature branches
- Branch naming follows task/feature identification

**Development Workflow:**

1. Task created on Azure DevOps board (assigned to developer)
2. Developer creates feature branch for the task
3. Code implementation on feature branch
4. Pull request (PR) created to merge to master
5. **PR requires approval** before merge
6. Merge to master **automatically triggers** build pipeline
7. Build creates deployment-ready artifacts
8. Deployment to environments requires separate approval

**PR Policy:**

- **Approval required:** Yes, for all PRs
- **Code review:** Required before merge
- **Branch protection:** PR approval process enforced

**Board Access:**

- Azure DevOps board is accessible by CISAC and various music agencies from different countries
- Access can be granted to audit team with CISAC authorization (email to Yan with CC to CISAC)

### 5. Deployment Frequency and Lead Time

**Question:** What's your typical deployment frequency, and how long does a deployment take from approval to completion?

**Answer:**

**Deployment Timing:**

- **Build to deployment-ready:** ~5 minutes after PR merge
- **Full environment deployment:** ~27-30 minutes
- **Deployment frequency:** Variable, based on agreement with CISAC
- Features may be:
  - Deployed immediately if urgent
  - Batched with other changes for scheduled release
  - Held back if not ready for end users

**Lead Time Considerations:**

- No formal Dora Metrics tracking for this project
- Deployment schedule driven by CISAC approval, not technical constraints
- "Time to production" depends on business agreement, not technical limitations
- Repository not connected to engineering intelligence tooling platforms

### 6. Infrastructure as Code (IaC) Coverage

**Question:** What would be the complexity or time estimate to create a new environment to work with internally? (20-day estimate for environment extension/replication)

**Answer:**

**IaC Approach:**

- All infrastructure managed through Azure Resource Manager (ARM) templates
- IaC deployment is part of the pipeline (multiple pipeline steps update infrastructure)
- Covers: binaries, secrets, connection strings, external services, Key Vault updates, Databricks configurations
- Spanish Point stated: "Our IaC is far more advanced than any other IaC in most other companies"
- Spanish Point advises other companies on IaC implementation

**IaC Assets Ownership:**

- IaC definitions are **not included in ISWC source code delivery**
- Based on Spanish Point's **proprietary SmartM library**
- Not considered part of the ISWC application itself

**Environment Extension Scenarios:**

If CISAC wants to add a new environment (e.g., staging):

1. **Option 1: Spanish Point manages**
   - Effort would be very small
   - Copy existing pipeline definition with environment-specific parameters
   - No entirely new code needed

2. **Option 2: CISAC builds from scratch**
   - Build pipeline without Spanish Point input using source code
   - Work with another vendor to build from scratch

3. **Option 3: Third-party vendor using Spanish Point assets**
   - Spanish Point offers a **licensing program** for the SmartM library
   - Third-party must be **licensed** to access IaC and CI/CD assets
   - Public information available on Spanish Point website: [https://www.spanishpoint.ie/developer-solutions/](https://www.spanishpoint.ie/developer-solutions/)
   - Licensing allows third party to use pipeline and IaC source code entirely

**Note:** The 20-day estimate was not directly addressed or validated during this meeting.

### 7. Pipeline Components and Build Process

**Question:** What are you building in the Databricks job?

**Answer:**

- Python libraries (wheel files)
- Notebooks
- Standard Python packaging (nothing out of ordinary)
- No third-party connections during build
- Source code for Databricks components included in delivery

**Pipeline Coverage:**

- Tests (unit, integration, UI)
- IaC deployment
- Binary deployment
- Secret management
- Connectivity and connection strings
- External services integration
- Full automated deployment (no manual steps except approval)

### 8. .NET 8 Upgrade Status

**Observation from discussion:**

- .NET 8 upgrade was **recently completed** (week of meeting)
- Deployment to production occurred recently
- Some tests still being updated to align with .NET 8
- Pipeline configuration still being adjusted post-upgrade
- Temporary workaround: local test execution against deployed environments

**Note:** This represents progress on the technical debt item identified in investigation planning (.NET 3.1 EOL).

---

## Key Findings

### Pipeline Maturity

**Strengths:**

- Fully automated deployment process (click-button deploy)
- Multi-stage pipeline with environment segregation
- Comprehensive test coverage (unit, integration, UI)
- Automated secret management and infrastructure updates
- PR approval required before merge
- Build traceability from PR to deployment

**Areas of Concern:**

- Test pass rates <100% accepted during active development
- Some integration tests described as "brittle" (flaky)
- No Dora Metrics or formal deployment analytics
- .NET 8 upgrade caused temporary pipeline disruption

### Vendor Independence Assessment

**Critical Finding: Proprietary IaC and Pipeline Assets**

The CI/CD pipeline and Infrastructure as Code definitions are **not part of the ISWC deliverable**. They are based on Spanish Point's proprietary **SmartM library**.

**Implications:**

1. **Vendor lock-in risk:** Pipeline knowledge is not transferable without licensing
2. **Third-party costs:** Another vendor would need to either:
   - Rebuild pipeline from scratch (using available source code)
   - License SmartM library from Spanish Point
3. **Environment replication:** Cannot independently create new environments without Spanish Point involvement or licensing

**Mitigation Options:**

- Negotiate SmartM library license if changing vendors
- Budget for pipeline rebuild if moving to another vendor
- Maintain Spanish Point for infrastructure management even if changing application development vendor

### Documentation and Knowledge Transfer

**Board Access:**

- Azure DevOps board tracks all tasks, PRs, and changes
- Access can be granted with CISAC authorization
- Board already shared with multiple international music agencies

**Pipeline Documentation:**

- Pipeline definitions not shared (proprietary)
- Live demonstrations provided as alternative
- Source code is complete and deployment-ready

---

## Open Questions and Follow-ups

### Access Requests

- [ ] **Azure DevOps Board Access:** Request requires email to Yan (Spanish Point) with CC to CISAC for authorization
- [ ] **Git History Access:** Already addressed - source code with history has been shared

### Unaddressed from Original Agenda

The following topics were **not discussed** in detail:

- May-June 2024 merge incident and recovery actions
- Rollback procedures and examples
- Secrets rotation policies
- Infrastructure documentation completeness
- Databricks upgrade timeline
- Dependency scanning and vulnerability management
- Static analysis and security scanning tools

### Questions Raised During Meeting

- [ ] **20-day environment estimate:** Not validated - what drives this timeline if IaC is "advanced"?
- [ ] **SmartM library licensing costs:** What are the commercial terms?
- [ ] **IaC coverage percentage:** What portion of 343 Azure resources are in templates vs. manual?

---

## Next Steps

### For Audit Team

1. Analyze vendor lock-in implications of proprietary pipeline/IaC
2. Assess cost/effort of pipeline rebuild vs. licensing
3. Evaluate completeness of source code delivery vs. entitlement agreement
4. Request Azure DevOps board access (pending CISAC approval)
5. Document findings on vendor independence risk

### For CISAC

1. Decide on Azure DevOps board access for audit team
2. Clarify entitlement: Should IaC/pipeline be part of source code delivery?
3. Request SmartM library licensing terms for future vendor scenarios
4. Request formal documentation on environment replication process

---

## Related Documents

- [Meeting Transcript](../meetings/20251105-[ISWC Audit]CI_CD Pipeline-transcript.txt) - Full conversation record
- [Investigation Planning](Investigation-Planning.md) - Critical Priorities #2 (IaC) and #4 (Technical Debt)
- [Workshop 1 Transcript](../meetings/20251020-ISWC Audit - Workshop 1.txt) - Previous pipeline demonstration
- [Infrastructure Reference](../work_in_progress/infra/infrastructure-azure-reference.md) - 343 Azure resources documented
- [Spanish Point Developer Solutions](https://www.spanishpoint.ie/developer-solutions/) - SmartM library information
