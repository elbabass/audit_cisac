# Spanish Point Technical Meeting 1 - Production and Performance Data

**Date:** November 5, 2025
**Duration:** 1 hour
**Attendees:** Spanish Point Technical Team (Curnan Reidy + technical staff), CISAC, Teragone-Factory
**Approach:** Open questions first, then specific deep-dives

---

## Meeting Objectives

1. **Validate Hyperscale Proposal** - Understand performance metrics that justify the €3,300/month claimed savings
2. **Cost Transparency** - Get breakdown of €50K/month cloud costs by service
3. **Performance Baseline** - Establish current performance characteristics (latencies, throughput)
4. **Optimization Opportunities** - Identify quick wins before major architecture changes

---

## Phase 1: Open Discovery Questions (15 minutes)

**Goal:** Let Spanish Point explain their perspective on performance and monitoring

| Priority | Question | Goal |
|----------|----------|------|
| 🔴 | What are the roles of Databricks, CosmosDB and the Datalake for the end users ? | Understand data access patterns and user workflows |
| 🔴 | Can you walk us through how you monitor and measure the current system performance in production? What are your key performance indicators? | Understand monitoring approach and what metrics they consider important |
| 🔴 | What are the most significant performance challenges you've observed in production over the past 6-12 months? | Identify real bottlenecks vs assumptions |
| ⚠️ | How do you typically troubleshoot performance issues when they arise? Can you share an example? | Understand operational processes and diagnostic capabilities |
| ⚠️ | What patterns have you noticed in system behavior during peak vs normal usage? | Understand load characteristics and scalability patterns |

**Follow-up requests:**

- Can you share your monitoring dashboards? (Application Insights, Azure Monitor)
- What alerts do you have configured?
- Can we see historical performance trends?

---

## Phase 2: Database and Storage Performance (15 minutes)

**Goal:** Validate the Hyperscale proposal business case

| Priority | Question | Goal | Context |
|----------|----------|------|---------|
| 🔴 | What specific performance metrics led to the Hyperscale proposal? What's the current SQL Server tier and its performance characteristics? | Validate Hyperscale business case | PSA5499 claims €3,300/month savings |
| 🔴 | What are the p50/p95/p99 query latencies for the most common database operations (work search, submission processing, ISWC assignment)? | Establish performance baseline | Need quantitative data |
| 🔴 | What percentage of queries benefit from the current Business Critical tier features vs would work fine on lower tiers? | Assess if tier is right-sized | May be over-provisioned |
| ⚠️ | What are the slowest queries in production? Have you identified optimization opportunities (indexes, query rewrites)? | Identify quick wins before infrastructure upgrade | Query optimization cheaper than Hyperscale |
| ⚠️ | What's the current Cosmos DB RU provisioning, utilization, and any throttling events (HTTP 429)? | Assess Cosmos DB sizing | Initial design: 1,000 RU |
| ⚠️ | What's the actual data volume growth rate? How does this impact cost projections? | Validate capacity planning | - |

**Follow-up requests:**

- SQL Server Query Store data (top resource-consuming queries)
- Execution plans for slowest queries
- Cosmos DB metrics dashboard (RU consumption, throttling)
- Database growth trends (storage, transaction volume)

---

## Phase 3: Application and Pipeline Performance (15 minutes)

**Goal:** Understand end-to-end user experience and identify bottlenecks

| Priority | Question | Goal | Context |
|----------|----------|------|---------|
| 🔴 | What's the end-to-end latency for work submission processing (validation → matching → ISWC assignment)? What are the p95/p99 values? | Understand user experience | Critical user workflow |
| 🔴 | What's the typical response time and availability SLA for the Matching Engine? How often do you experience timeout or degradation? | Assess Matching Engine impact | Critical vendor dependency |
| ⚠️ | What's the average Databricks job duration and cost for IPI full resynch vs incremental agency file processing? | Understand data processing efficiency | Databricks 10.4 LTS outdated |
| ⚠️ | What percentage of Databricks costs are wasted on idle clusters vs active processing? | Identify cost optimization opportunities | Part of €50K/month cloud costs |
| 📝 | What are the Portal response times (p50/p95/p99) for search and submission operations? | Assess frontend performance | React 16.12.0 + .NET 3.1 backend |

**Follow-up requests:**

- Application Insights end-to-end transaction traces
- Matching Engine SLA documentation and actual uptime metrics
- Databricks job execution history and cluster utilization reports
- Portal performance metrics (page load times, API response times)

---

## Phase 4: Resource Utilization and Costs (15 minutes)

**Goal:** Create transparent cost breakdown and identify optimization opportunities

| Priority | Question | Goal | Context |
|----------|----------|------|---------|
| 🔴 | What's the current vs peak resource utilization for each major component? Which resources are over-provisioned? | Identify right-sizing opportunities | Quick wins for cost reduction |
| 🔴 | Have you analyzed reserved instance or savings plan opportunities? What's the current commitment vs on-demand split? | Assess cost optimization maturity | Could save 20-40% on stable workloads |
| ⚠️ | What monitoring and alerting exists for cost anomalies? Have you had unexpected cost spikes? | Understand cost governance | - |
| 📝 | What's the auto-scaling configuration for App Services and Databricks? How often does it trigger? | Assess elasticity vs waste | - |

**Follow-up requests:**

- Azure Cost Management reports (breakdown by service, resource group, tags)
- Resource utilization metrics (CPU, memory, DTU/RU consumption)
- Reserved instance analysis or recommendations
- Auto-scaling configuration and trigger history

---

## Meeting Execution Notes

### Pre-Meeting Preparation

- [ ] Review PSA5499 Hyperscale proposal details
- [ ] Have cost baseline questions ready (€50K/month)
- [ ] Prepare to take detailed quantitative notes
- [ ] Set up screen recording/note-taking

### During Meeting

**Do:**

- ✅ **Start open** - Let them tell their story first
- ✅ **Request screen shares** - See actual dashboards and metrics
- ✅ **Take quantitative notes** - Record all numbers (latencies, costs, percentages)
- ✅ **Be collaborative** - Frame as optimization exercise, not criticism
- ✅ **Ask for examples** - Real incidents, troubleshooting stories
- ✅ **Acknowledge complexity** - Show appreciation for their work

**Don't:**

- ❌ **Reveal all concerns** - Don't lead with accusations or assumptions
- ❌ **Rush to conclusions** - Let data guide the conversation
- ❌ **Be adversarial** - This is joint problem-solving

### Key Data Points to Capture

**Performance Metrics:**

- SQL Server tier: _____________
- p95/p99 query latency: _____________
- Cosmos DB RU provisioning: _____________
- End-to-end submission latency: _____________
- Matching Engine response time: _____________
- Databricks job duration: _____________

**Cost Breakdown:**

- SQL Server: €____________/month
- Cosmos DB: €____________/month
- Databricks: €____________/month
- App Services: €____________/month
- Storage: €____________/month
- Networking: €____________/month
- Other: €____________/month

**Resource Utilization:**

- SQL DTU/vCore usage: \_\_\_% average, \_\_\_% peak
- Cosmos RU usage: \_\_\_% average, \_\_\_% peak
- Databricks cluster idle time: \_\_\_%
- App Service CPU/Memory: \_\_\_% average

### Post-Meeting Actions

- [ ] Consolidate notes and quantitative data
- [ ] Request any missing documentation or reports
- [ ] Analyze cost breakdown vs Hyperscale proposal
- [ ] Identify optimization opportunities
- [ ] Prepare findings for Meeting 2 context

---

## Expected Outcomes

By the end of this meeting, we should have:

1. ✅ **Hyperscale Justification** - Clear understanding of performance issues that led to proposal
2. ✅ **Cost Transparency** - Detailed breakdown of €50K/month by service
3. ✅ **Performance Baseline** - Quantitative metrics for current system performance
4. ✅ **Quick Win Opportunities** - List of optimization actions before architecture changes
5. ✅ **Documentation Access** - Dashboards, reports, and metrics to review offline

---

**Related Documents:**

- [Investigation Planning](Investigation-Planning.md) - Critical Priority #1: Hyperscale Proposal Validation
- [Questions Tracker](questions-tracker.md) - Complete list of 242 technical questions
- [PSA5499 Performance Proposition](../resources/performance_proposition/CISACAzureInfrastructureUpdate/CISACAzureInfrastructureUpdate/CISACAzureInfrastructureUpdate.md)
