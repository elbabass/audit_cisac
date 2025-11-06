# Spanish Point Technical Meeting 3 - Cloud Costs and Optimization

**Date:** November 6, 2025
**Duration:** 30 minutes
**Attendees:** Spanish Point (Xiyuan Zeng), Teragone-Factory (Bastien Gallay, Guillaume Jay)
**Approach:** Azure Portal walkthrough with live cost data

---

## Meeting Objectives

1. **Cost Transparency** - Understand €50K/month breakdown by Azure service
2. **PSA5499 Validation** - Verify claimed €3,300/month savings from Hyperscale proposal
3. **Quick Wins** - Identify immediate optimization opportunities

---

## Phase 1: Cost Breakdown Walkthrough (10 minutes)

**Goal:** Map the €50K/month to actual Azure resources

### Questions

| Priority | Question | Goal |
|----------|----------|------|
| 🔴 | Can you show us the cost breakdown in Azure Portal by resource type and service? | Get visual understanding of cost distribution |
| 🔴 | What are the top 5 cost contributors? What percentage of total? | Identify optimization targets |
| ⚠️ | How much variability do you see month-to-month? What drives the changes? | Understand cost predictability |

**Azure Portal Views:**

- Cost Management + Billing → Cost analysis
- Filter by resource group, resource type, service
- View last 3 months trend

---

## Phase 2: Major Cost Components Deep-Dive (10 minutes)

**Goal:** Focus on top 3 cost drivers only

### Key Questions by Service

**Azure SQL Database:**

- Current SKU/tier, vCores, actual utilization?
- PSA5499 Hyperscale savings calculation walkthrough
- Any idle or oversized databases?

**Azure Databricks:**

- DBU consumption (avg monthly)
- Cluster idle time vs active time
- All-purpose vs job clusters split?

**Cosmos DB / Storage / App Services:**

- Current configuration and utilization
- Any obvious rightsizing opportunities?

---

## Phase 3: Optimization Assessment (10 minutes)

**Goal:** Quick wins and PSA5499 validation

### Questions

| Priority | Question | Goal |
|----------|----------|------|
| 🔴 | Are Reserved Instances or Savings Plans in use? Potential savings? | Assess commitment-based discounts |
| 🔴 | Can you show Azure Advisor cost recommendations? | Quick native optimization check |
| 🔴 | PSA5499: Show before/after SQL Hyperscale cost calculation | Validate €3,300/month savings claim |
| ⚠️ | Dev/test environments: 24/7 or auto-shutdown? | Quick win identification |

**Azure Advisor:**

- Quick review of top 3-5 cost recommendations

---

## Key Data to Capture

**Top 5 Cost Contributors:**

1. [Service]: €[Amount] /month ([%])
2. [Service]: €[Amount] /month ([%])
3. [Service]: €[Amount] /month ([%])
4. [Service]: €[Amount] /month ([%])
5. [Service]: €[Amount] /month ([%])

**PSA5499 Validation:**

- Current SQL cost: €[Amount] /month
- Proposed Hyperscale cost: €[Amount] /month
- Net savings: €[Amount] /month (validate €3,300 claim)

**Quick Wins:**

- Reserved Instances: €[Amount] /month potential
- Other opportunities: [List]

---

## Expected Outcomes

By end of meeting:

1. ✅ Cost breakdown by top 5 services
2. ✅ PSA5499 savings calculation validated
3. ✅ 2-3 optimization opportunities identified
4. ✅ Understanding of cost management practices

---

## Notes

- Spanish Point will demonstrate Azure Portal Cost Management views
- Request permission for screenshots of anonymized cost data
- Optional: Request exported cost analysis CSV for detailed post-meeting analysis

---

**Related Documents:**

- [Production and Performance Meeting](20251105-SpanishPoint-Meeting1-ProdPerf.md) - Background on PSA5499 proposal
- [PSA5499 Performance Proposition](../resources/performance_proposition/CISACAzureInfrastructureUpdate/CISACAzureInfrastructureUpdate/CISACAzureInfrastructureUpdate.md) - Original proposal document
- [Investigation Planning](Investigation-Planning.md) - Critical Priority #1: Hyperscale Proposal Validation
