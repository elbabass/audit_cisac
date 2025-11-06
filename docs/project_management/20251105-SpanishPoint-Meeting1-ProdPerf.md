# Spanish Point Technical Meeting 1 - Production and Performance Data

**Date:** November 5, 2025
**Duration:** 1 hour
**Actual Attendees:** Spanish Point (Xiyuan Zeng, Nicholas Randles), Teragone-Factory (Bastien Gallay, Guillaume Jay)
**Approach:** Open questions first, then specific deep-dives

---

## Planned Objectives

1. **Validate Hyperscale Proposal** - Understand performance metrics that justify the €3,300/month claimed savings
2. **Cost Transparency** - Get breakdown of €50K/month cloud costs by service
3. **Performance Baseline** - Establish current performance characteristics (latencies, throughput)
4. **Optimization Opportunities** - Identify quick wins before major architecture changes

## Actual Meeting Flow

The meeting covered the following topics in order:

1. **Git logs access discussion** (~15 minutes) - Request for full Git history to understand development patterns
2. **Data components roles** (~15 minutes) - Databricks, CosmosDB, and Data Lake architecture
3. **Monitoring and performance KPIs** (~10 minutes) - Azure Monitor approach and alerting
4. **Performance challenges** (~5 minutes) - Historical performance issues discussion
5. **PSA5499 Hyperscale proposal clarification** (~15 minutes) - Understanding the proposal scope and intent
6. **Code review findings** (~5 minutes) - Minor bug identification in date filtering
7. **Follow-up scheduling** (~5 minutes) - Cost/pricing deep-dive meeting scheduled

---

## 1. Git Logs Access Discussion

**Context:** Teragone-Factory requested full Git history to analyze development patterns, commit trends, change frequency, and code hotspots.

**Spanish Point Response:**

- Xiyuan needs to check internally whether this level of information can be shared
- Concerns raised about sharing internal development team working habits
- This involves compliance review before sharing
- Spanish Point will follow up after internal discussion

**Status:** Pending internal compliance review

---

## 2. Data Components Architecture

### Databricks Role

**Functionality:**

- Processes JSON files and flat files from agencies and publishers
- Files uploaded to FTP folders by agencies/publishers (works submissions, updates, searches)
- Workflow runs every 30 minutes to check for new files
- Creates workflows to process files based on file type
- Calls ISWC APIs to make submissions (batch calls)
- Processes API responses and outputs to FTP output folder

**Key Jobs:**

- **EDI Process** - Scans FTP folders every 30 minutes for agency/third-party files
- **Process File** - Handles agency files
- **Allocation** - Processes publisher files
- **Resolution** - Processes publisher-type files
- **Third Party** - Handles third-party submissions
- **Process Reports** - Generates large data extracts (full ILS extracts)
- **Process Audit** - Runs every 20 minutes or hourly, replicates Cosmos DB data to Delta Lake using change feed

**Architecture Clarification:**

- Databricks is a **background/asynchronous process**
- Not used directly by Portal or API (no synchronous calls)
- All data processed eventually used by end users/clients
- Uses Delta Lake for replicated data (for report generation)

### Cosmos DB Role

**Primary Purpose:** Audit tracking

**Contents:**

- Audit request container with all submission data
- Metadata for debugging and troubleshooting submissions
- Replicated to Delta Lake via Databricks change feed connector (every 20-60 minutes)

**Usage Pattern:** Reports using audit data run against Delta Lake replica, not directly against Cosmos DB

### Data Lake

**Purpose:** Stores binary files and processed data from Databricks workflows

---

## 3. Monitoring and Performance KPIs

### Monitoring Approach

**Platform:** Azure Monitor (Microsoft first-party integration)

**Metrics Monitored:**

- CPU usage (database, App Service)
- RAM usage
- DTU/RU consumption
- Platform-level performance metrics exposed by Azure services

**Access:** All metrics viewable in Azure Portal and Azure Monitor (audit team has access to subscription)

### Alerting Configuration

**Alert Examples:**

- CPU > 80% for 5+ minutes → Email to support team
- Alert definitions available in Azure Monitor

**Alert Routing:** Emails sent to Spanish Point support team for investigation

### SLA Structure

**Incident Response SLA:**

- Defined in support contract between Spanish Point and CISAC
- Covers response time to support incidents (not performance thresholds)

**Platform SLA:**

- Microsoft provides SLA for Azure services based on SKU/tier configuration
- Spanish Point monitors application behavior within those SKU limits

**No performance-based SLAs:** No defined performance targets (e.g., p95 latency < Xms) in contracts

---

## 4. Performance Challenges (Past 6-12 Months)

### Overall Assessment

**No significant platform-level performance issues reported** in the past year according to Spanish Point.

### Exception Cases Identified

**Large ISWC Data Sets:**

- Issue: ISWCs with thousands of works (e.g., 8,000+ works) cause SQL exceptions when returning all data in single API call
- Impact: Browser rendering issues with very large result sets
- Classification: Usability issue, not platform/infrastructure performance issue
- Proposed Solution: Implement pagination for work results (feature request in chat - not yet approved, pending post-audit review)

**Third-Party Service (Historical):**

- Performance issues when first launched (couple years ago)
- Resolved through code optimizations and detail level selection
- Detail levels allow clients to request minimal data vs full data (including all work information)

### Auto-Scaling Behavior

- Platform configured with auto-scaling
- App Services auto-scale based on load
- Databricks auto-provisions machines as needed, deprovisions when jobs finish
- If performance issues arise, resources scaled up (e.g., more cores for database)

---

## 5. PSA5499 Hyperscale Proposal Clarification

### Proposal Scope (Per Spanish Point)

**Primary Goals:**

1. **Network-level security improvements** - Private connectivity between components via virtual network and private endpoints (instead of public IP)
2. **SQL Server SKU change** - Business Critical tier → Hyperscale tier for cost optimization

**What It Is NOT:**

- Not driven by performance issues or bottlenecks
- Not addressing platform-level performance problems

### SQL Server SKU Change

**Rationale:**

- **Cost optimization** - Hyperscale tier is cheaper for same/better performance
- Not because current tier has performance problems
- Microsoft service-level differences between SKUs (platform internals, not customer configuration)

### Network Security Improvements

**Background:**

- Original platform built years ago without current security technologies
- Technologies for private networking now available in Azure
- Proposal aligns with Microsoft best practices for secure component communication

**Proposed Changes:**

- Component-to-component communication via private IPs instead of public IPs
- Uses virtual network and private endpoints
- Optional: Web Application Firewall (WAF) / Azure Front Door (Layer 7 reverse proxy)

**Trigger:** Not based on security breach or known issues - proactive modernization based on evolved Azure capabilities

### Clarification on "Optimization"

When proposal mentions "optimization," it refers to:

- **Cost optimization** (cheaper SKU)
- **Security optimization** (private networking)
- **Not performance optimization** (no performance problems identified)

---

## 6. Code Review Findings

### Date Filter Bug

**Location:** Date filtering logic in search/query code

**Issue:** Comparison uses `CreatedDate > ToDate` when it should use `CreatedDate <= ToDate`

**Assessment:**

- Variable naming may be confusing (FromDate/ToDate)
- Appears to be logical error based on variable names
- Spanish Point will review with development team to confirm if bug or intentional logic

**Priority:** Minor - too small to include in audit report

---

## 7. Follow-Up Actions and Scheduling

### Scheduled Follow-Up Meeting

**Topic:** Cost/pricing breakdown and Azure optimization opportunities

**Attendees:** Xiyuan Zeng (Spanish Point) + Teragone-Factory

**Proposed Time:** Same time next day (November 6, 2025)

**Goals:**

- Detailed cost breakdown by Azure service
- Understand cost variability and drivers
- Review proposal in detail
- Discuss optimization opportunities

### Pending Items

**Git Logs:**

- Spanish Point to check internal compliance and respond
- Not a decision Xiyuan can make independently
- Requires discussion with Spanish Point internal team

**Documentation Requests:**

- No specific documentation requests captured during this meeting
- Cost breakdown to be discussed in follow-up meeting

---

## Meeting Outcomes vs Planned Objectives

### Objective 1: Validate Hyperscale Proposal

**Status:** ⚠️ Partially Achieved - Scope Clarified

**Findings:**

- PSA5499 is primarily for **cost optimization and security**, not performance issues
- No performance problems identified that drove the proposal
- Hyperscale tier is cheaper SKU for same/better performance
- Network security improvements based on Azure platform evolution, not security incidents

**Gap:** Did not obtain quantitative performance metrics (p50/p95/p99 latencies) - not deemed necessary by Spanish Point as no performance issues exist

### Objective 2: Cost Transparency

**Status:** ❌ Not Achieved - Deferred to Follow-Up Meeting

**Action:** Scheduled dedicated cost/pricing meeting for November 6, 2025

### Objective 3: Performance Baseline

**Status:** ⚠️ Partially Achieved

**Captured Information:**

- Monitoring approach: Azure Monitor with platform-level metrics
- Alert configuration: CPU > 80% for 5+ minutes triggers email
- Auto-scaling enabled on all components
- No significant performance issues in past year

**Missing Information:**

- Specific latency measurements (p50/p95/p99)
- Query performance metrics
- Resource utilization percentages

**Reason:** Spanish Point position is that no performance issues exist, so detailed metrics not prioritized for sharing

### Objective 4: Optimization Opportunities

**Status:** ⚠️ Partially Identified

**Findings:**

- Platform already uses auto-scaling
- PSA5499 proposes cost optimization via cheaper SQL SKU
- Pagination feature for large ISWCs proposed but not approved (pending post-audit)
- No infrastructure-level quick wins identified during meeting

---

## Key Insights

### Architectural Understanding

- **Databricks usage model clarified** - Pure asynchronous background processing (FTP-based workflows, report generation)
- **Cosmos DB scope clarified** - Audit tracking only, replicated to Delta Lake for reporting
- **No direct database access from Databricks** - All data operations via API calls

### Performance Narrative

- Spanish Point reports **no significant performance issues** in production
- Only exception case: ISWCs with 8,000+ works causing SQL exceptions (edge case, usability issue)
- Infrastructure designed for auto-scaling to handle demand

### Proposal Context

- PSA5499 is **proactive modernization**, not reactive to problems
- Primary value: Cost savings + security improvements
- Triggered by Azure platform evolution (private networking capabilities now available)

### Process Observations

- **Git logs access requires compliance review** - Information sharing policy differs from audit expectations
- **Transparency discussion** - Extended conversation about audit scope and information access rights
- **Collaborative tone maintained** - Technical focus preserved despite access discussion

---

## Open Questions for Investigation

Based on meeting findings, the following require independent verification:

1. **Performance baseline validation** - Are there hidden performance issues not surfaced by Spanish Point monitoring?
2. **Cost breakdown analysis** - What are actual Azure costs and optimization opportunities?
3. **Auto-scaling behavior** - Is auto-scaling configured optimally or masking underlying issues?
4. **Monitoring coverage** - Are Azure Monitor metrics sufficient for production system health?
5. **SQL exceptions** - How frequent are the "large ISWC" SQL exceptions? Is this truly an edge case?

---

## Action Items

### Teragone-Factory

- [ ] Attend cost/pricing meeting on November 6, 2025
- [ ] Review Azure Monitor alerts configuration (accessible in subscription)
- [ ] Analyze whether performance baseline data can be obtained from Azure metrics directly
- [ ] Investigate "large ISWC" SQL exception frequency in logs/metrics

### Spanish Point

- [ ] Internal compliance review for Git logs sharing
- [ ] Prepare detailed cost breakdown for follow-up meeting
- [ ] Review date filter bug with development team

### CISAC

- [ ] No immediate actions - will be updated after cost/pricing meeting

## Appendix: Original Planned Questions (Not All Covered)

The meeting was planned with 4 phases of questions, but actual flow diverged significantly. Below are the original planned questions for reference:

### Planned Phase 1: Open Discovery Questions

- ✅ What are the roles of Databricks, CosmosDB and the Datalake for the end users?
- ✅ Can you walk us through how you monitor and measure the current system performance in production? What are your key performance indicators?
- ✅ What are the most significant performance challenges you've observed in production over the past 6-12 months?
- ❌ How do you typically troubleshoot performance issues when they arise? Can you share an example? (Not covered)
- ❌ What patterns have you noticed in system behavior during peak vs normal usage? (Not covered)

### Planned Phase 2: Database and Storage Performance

- ⚠️ What specific performance metrics led to the Hyperscale proposal? (Answered: None - cost optimization, not performance)
- ❌ What are the p50/p95/p99 query latencies for the most common database operations? (Not provided)
- ❌ What percentage of queries benefit from the current Business Critical tier features? (Not covered)
- ❌ What are the slowest queries in production? (Not covered)
- ❌ What's the current Cosmos DB RU provisioning, utilization, and throttling events? (Not covered)
- ❌ What's the actual data volume growth rate? (Not covered)

### Planned Phase 3: Application and Pipeline Performance

- ❌ What's the end-to-end latency for work submission processing? (Not covered)
- ❌ What's the typical response time and SLA for the Matching Engine? (Not covered)
- ❌ What's the average Databricks job duration and cost? (Not covered)
- ❌ What percentage of Databricks costs are wasted on idle clusters? (Partially answered: auto-scaling handles this)
- ❌ What are the Portal response times? (Not covered)

### Planned Phase 4: Resource Utilization and Costs

- ❌ What's the current vs peak resource utilization for each major component? (Not covered - deferred to follow-up)
- ❌ Have you analyzed reserved instance or savings plan opportunities? (Not covered - deferred to follow-up)
- ❌ What monitoring and alerting exists for cost anomalies? (Not covered - deferred to follow-up)
- ⚠️ What's the auto-scaling configuration? (Partially answered: confirmed enabled, details not provided)

**Note:** Many performance-specific questions were not covered because Spanish Point's position is that no performance issues exist. Cost questions deferred to dedicated follow-up meeting.

---

**Related Documents:**

- [Investigation Planning](Investigation-Planning.md) - Critical Priority #1: Hyperscale Proposal Validation
- [PSA5499 Performance Proposition](../resources/performance_proposition/CISACAzureInfrastructureUpdate/CISACAzureInfrastructureUpdate/CISACAzureInfrastructureUpdate.md)
- [CI/CD Pipeline Meeting Transcript](../meetings/20251105-[ISWC%20Audit]CI_CD%20Pipeline-transcript.txt) - Meeting held same day
