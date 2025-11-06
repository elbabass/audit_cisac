# Spanish Point Technical Meeting 3 - Cloud Costs and Optimization

**Date:** November 6, 2025
**Duration:** 55 minutes
**Attendees:**

- Spanish Point: Xiyuan Zeng
- Teragone-Factory: Bastien Gallay, Guillaume Jay

**Meeting Type:** Screen sharing - cost proposal walkthrough and Azure Portal cost analysis

---

## Meeting Summary

This meeting focused on understanding Azure cloud costs (€50k/month baseline), validating the PSA5499 Hyperscale proposal financial benefits, and investigating cost variability between months. Spanish Point explained that all cost fluctuations are expected due to auto-scaling based on agency file submissions and API usage. The Hyperscale proposal delivers net savings of ~€1,800/month after accounting for private networking cost increases. Key finding: no formal correlation tracking exists between usage metrics (API calls, file uploads) and Azure costs, making it difficult for CISAC to explain cost variations to their stakeholders.

---

## Topics Discussed

### 1. Cost Variability and Auto-Scaling Philosophy

**Question:** How much variability do you see month-to-month? What drives the changes?

**Answer:**

Spanish Point's position on cost variability:

- **All fluctuations are expected and healthy** - Environment is fully auto-scaled (not provisioned with fixed compute)
- **Cost reflects actual usage** - Based on files submitted by agencies and API calls
- **Variability is a feature, not a bug** - "System auto-scaling is working perfectly - this saves CISAC money"
- **Provisioning for peak would be wasteful** - Steady costs would mean always paying for maximum capacity

**Cost drivers identified:**

1. **File uploads via SFTP** - Agencies submit work files asynchronously
2. **API calls** - Direct agency API usage (tracked in API Management analytics)
3. **UI usage** - Portal searches and submissions (tracked in Application Insights)

**February 2025 Peak Analysis:**

- Guillaume identified **10% cost increase** in February compared to surrounding months
- Spanish Point explanation: Increased agency submissions (files or API calls)
- **No formal investigation requested by CISAC** - Spanish Point stated this is normal behavior
- Breakdown by service type would reveal specific drivers (likely Cosmos DB, Data Factory, Databricks)

**Spanish Point recommendation:** If CISAC has serious cost concerns, open support ticket for detailed investigation

### 2. PSA5499 Hyperscale Proposal - Financial Analysis

**Question:** Can you walk us through the cost savings calculation?

**Answer:**

**Proposal Scope (3 objectives):**

1. **Private networking** - Move from public IP communication to private virtual network with private endpoints
2. **Layer 7 reverse proxy** - Add Web Application Firewall (WAF) / Azure Front Door
3. **SQL tier change** - Business Critical → Hyperscale (cost optimization focus)

**Cost Impact Breakdown:**

**Increases (Private Networking):**

| Component | Monthly Cost Increase | Reason |
|-----------|----------------------|---------|
| Top 4 items | ~€1,500 total | Private endpoints, virtual network, WAF, network configuration |

**Note:** Spanish Point emphasized these components can be implemented selectively, but doing so would be "pointless from security perspective" (half-open, half-secured environment).

**Decreases (SQL Tier Change):**

| Current Tier | Proposed Tier | Monthly Savings |
|--------------|---------------|----------------|
| Business Critical (various vCore configs) | Hyperscale (1 primary + 1 secondary HA replica) | ~€3,300 |

**Net Financial Benefit:**

- **Savings:** €3,300/month (SQL tier change)
- **Costs:** €1,500/month (private networking)
- **Net benefit:** **€1,800/month reduction**

**Confidence Level:**

- Xiyuan stated "100% accuracy" on calculations
- Based on actual usage from month when proposal was created (likely May 2025)
- Calculations use Microsoft Azure pricing and documented SKU differences
- **Caveat:** Auto-scaled services (Cosmos DB, Data Factory) can never have "absolute figures" - usage-dependent

**Reservation Strategy:**

Proposal recommends reserved instances for:

- **SQL Database:** Already have reservation, will buy new reservation after Hyperscale migration
- **Cosmos DB:** Recommends 1-year reservation (~20% savings) IF workload is stable
- **Databricks:** Does NOT recommend reservation (only 4-6% savings, highly variable workload)

### 3. Top Cost Contributors

**Question:** What are the top Azure cost drivers?

**Answer:**

Based on Azure Portal cost analysis reviewed during meeting:

**Primary Cost Drivers:**

1. **Cosmos DB** - Highest variable cost component
   - Auto-scales based on Request Units (RU) consumption
   - Used for audit tracking of all application operations
   - Usage driven by: API calls, file processing, Azure Functions, Databricks jobs
   - Not exclusively tied to Databricks (multiple services write to Cosmos DB)

2. **Azure SQL Database** - Significant fixed cost
   - Currently Business Critical tier (provisioned, not auto-scaled)
   - Target of PSA5499 optimization (move to Hyperscale)
   - Has existing reservation in place

3. **Azure Databricks** - Variable cost based on DBU consumption
   - Processes files uploaded via SFTP
   - Runs scheduled jobs every 30 minutes
   - Auto-provisions clusters, deprovisions when finished

4. **Data Factory** - Auto-scales with file processing workload
   - Triggered by agency file uploads to SFTP

5. **App Service** - Lower unit cost, but can scale to 8 instances
   - Aggressive auto-scaling rules (scale down at 20% CPU, scale up at thresholds)
   - Relatively inexpensive compared to Databricks/Cosmos DB

**Cost Composition Reality:**

- Spanish Point emphasized that cost increases in one service don't necessarily correlate 1:1 with usage increases in that service
- Example: App Service may trigger more Cosmos DB writes (higher cost) without significant App Service cost increase (lower unit price)
- **"Not necessarily have a precise going parallel"** between service usage and Cosmos DB costs

### 4. Reserved Instances and Savings Plans

**Question:** Are Reserved Instances or Azure Savings Plans in use?

**Answer:**

**Current State:**

- **SQL Database:** Reservation already in place for current Business Critical tier
- **Cosmos DB:** No reservation currently
- **Databricks:** No reservation currently

**Spanish Point Recommendations:**

**SQL Database:**

- After Hyperscale migration, purchase new reservation
- Savings already factored into PSA5499 calculations

**Cosmos DB:**

- **Recommends 1-year reservation** (~20% savings)
- Showed usage trending charts (October 2024 - October 2025)
- Usage appears to increase at year-end/year-start (possibly agencies submitting new year data)
- **Risk:** If CISAC overcommits and usage drops, they waste money on unused reservation

**Databricks:**

- **Does NOT recommend reservation**
- Only 4-6% potential savings
- Workload highly variable and "slightly unpredictable"
- Current cost: ~€1,500/month, reservation might save only €400
- Risk of overcommitment outweighs minimal savings

**Philosophy:** Only commit to reservations when savings justify the risk of usage variability

### 5. Cost-to-Usage Correlation Gap

**Question:** Can we correlate costs with usage metrics to explain cost variations?

**Key Finding:**

**No formal tooling or dashboard exists** to correlate:

- API call volume (API Management analytics)
- File upload activity (SFTP, Storage analytics)
- Azure costs by service

**Impact:**

- **CISAC's Challenge:** Yann and team cannot easily explain cost variations to their financial stakeholders
- Bastien analogy: "If I'm on the board, I want to measure usage and make clients that increase usage pay more"
- Current state: Spanish Point can explain logically, but no automated usage-to-cost reporting

**Spanish Point's Response:**

- Analytics exist in multiple places:
  - **API Management:** API call statistics
  - **Application Insights:** UI usage tracking
  - **Storage:** SFTP activity logs
  - **Individual services:** Databricks jobs, Data Factory pipelines, etc.
- **But:** No unified dashboard or correlation tooling
- **Solution:** CISAC can open support tickets for cost spike investigations
- **Limitation:** Most logs retained only 3 months

**Potential Improvement:**

Bastien suggested creating usage-to-cost correlation reporting to help CISAC:

- Understand "why" costs change month-to-month
- Provide statistics (not just logical explanations) to stakeholders
- Potentially implement "noisy neighbor" tracking (high-usage agencies)

Spanish Point open to helping CISAC implement agency-level cost allocation/chargeback model if desired

### 6. "Noisy Neighbor" Agencies

**Insight from discussion:**

Xiyuan confirmed: **"There can be a term called noisy neighbor"**

- A few large agencies likely use the system far more than smaller agencies
- Larger countries with more music catalog → more files, more API calls
- API Management analytics could reveal which agencies drive most usage
- **Note:** Two levels of statistics:
  - API app service: Called by multiple internal services
  - API Management (APIM): Front gate for external API calls (better indicator of agency usage)

**CISAC Option:**

- Spanish Point willing to help CISAC develop pricing/chargeback model to allocate costs to agencies based on usage
- This is "not in the system in any way" currently
- Would require custom implementation

### 7. Azure Advisor and Optimization Opportunities

**Question:** Have you reviewed Azure Advisor cost recommendations?

**Answer:**

- No specific Azure Advisor review conducted during meeting
- Spanish Point position: **"System is already well optimized"**
- Key optimization principle: Auto-scaling means paying less during low-usage periods
- **Cost variability = cost optimization working correctly**

**App Service Scaling Review:**

- Guillaume reviewed auto-scaling rules during meeting
- Current config: Min 2 instances → Max 8 instances
- **Aggressive rules:**
  - Scale down at 20% CPU (could cause issues if too aggressive)
  - Scale up at 80-90% CPU thresholds
- Spanish Point: "Rules are already very aggressive"
- Potential tweak, but App Service is not a major cost driver

**Spanish Point's Optimization Philosophy:**

- Prefer auto-scaling over fixed provisioning
- "If we provision for peak, you see steady cost but always pay for maximum capacity"
- "Go up and down is actually super good because system saves your money"
- Cost variability is a **feature**, not a problem

### 8. Code Bug Follow-up (Side Note)

**Context:** Bastien/Guillaume identified date filtering bug in code review

**Spanish Point Response:**

- Bug report forwarded to internal dev team
- **Preliminary analysis:** Audit team probably correct - bug exists
- **Reason not caught:** "Feature was probably not barely used"
- Spanish Point appreciated the detailed code review finding

---

## Key Findings

### Financial Validation

**PSA5499 Proposal:**

- ✅ **Validated:** Net savings of ~€1,800/month
- SQL Hyperscale migration: €3,300/month savings
- Private networking costs: €1,500/month increase
- Calculations based on actual historical usage, deemed 100% accurate by Spanish Point

**Reservation Strategy:**

- ✅ SQL Database: Already reserved, will reserve again after migration
- ✅ Cosmos DB: Recommended (20% savings, 1-year commitment)
- ❌ Databricks: Not recommended (4-6% savings, too variable)

### Cost Management Maturity

**Strengths:**

- Comprehensive auto-scaling across all major services
- Aggressive scaling rules to minimize waste
- Existing SQL reservation shows cost optimization awareness
- Cost variability treated as healthy optimization indicator

**Gaps:**

- ❌ **No usage-to-cost correlation tooling** - Critical gap for CISAC's stakeholder reporting
- ❌ **No agency-level cost allocation** - Cannot identify "noisy neighbor" agencies or implement chargeback
- ❌ **No proactive cost anomaly investigation** - Relies on CISAC opening support tickets
- ❌ **Logs retained only 3 months** - Limited historical analysis capability

### Cost Predictability Challenge

**Spanish Point's Perspective:**

- Cost variability is expected, healthy, and desirable (pays only for actual usage)
- "100% based on increased usage, either through API or file upload or UI"
- Agencies control usage volume → agencies control costs indirectly

**CISAC's Perspective (via Bastien):**

- Need to explain cost variations to financial stakeholders
- "I don't want just a logical explanation, I want statistics and correlations"
- Board-level pressure to understand and control spending
- Large, worldwide organization with complex financial reporting requirements

**Mismatch:** Spanish Point provides technical/logical explanations, but CISAC needs quantitative reporting for governance

### Optimization Opportunities Identified

**Immediate:**

1. **Cosmos DB reservation** - 20% savings, low risk if usage trends hold
2. **Usage-to-cost correlation dashboard** - High value for CISAC governance (custom development needed)

**Potential:**

1. **Agency cost allocation/chargeback** - Identify high-usage agencies, potentially implement usage-based billing
2. **App Service scaling rule tuning** - Minor potential savings, not a priority (already aggressive)

**Not Recommended:**

1. **Databricks reservation** - Only 4-6% savings, high variability risk
2. **Optional private networking components** - Would create "half-open" environment (security risk)

---

## Open Questions and Follow-ups

### Clarifications Needed

- [ ] **February 2025 cost spike:** Request detailed breakdown by service type to identify specific driver
- [ ] **Usage trending:** Does CISAC have visibility into agency submission patterns (file counts, API call volumes)?
- [ ] **Cost allocation appetite:** Does CISAC want to pursue agency-level cost tracking/chargeback?

### Recommendations for CISAC

1. **Accept PSA5499 with full scope** - €1,800/month savings + improved security posture
2. **Implement Cosmos DB reservation** - 20% savings on high-cost component (1-year commitment)
3. **Request usage-to-cost correlation tooling** - Critical for stakeholder reporting and cost governance
4. **Consider agency cost allocation** - If fair usage distribution is a concern
5. **Set up cost anomaly alerts** - Proactively trigger investigations rather than reactive support tickets

### Technical Deep-Dives (If Needed)

- [ ] Service-by-service cost breakdown for February 2025 spike
- [ ] API Management analytics review to identify high-usage agencies
- [ ] Cosmos DB usage patterns and reservation sizing analysis
- [ ] Data retention policy review (currently 3 months) - extend for better historical analysis?

---

## Meeting Outcomes

### Questions Answered

1. ✅ **Cost transparency:** Confirmed top drivers (Cosmos DB, SQL, Databricks, Data Factory, App Service)
2. ✅ **PSA5499 validation:** Net savings €1,800/month validated with detailed calculations
3. ✅ **Cost variability explanation:** Auto-scaling based on agency usage (files + API calls)
4. ✅ **Optimization strategy:** Selective reservations (SQL, Cosmos DB yes; Databricks no)

### New Insights Discovered

1. ❌ **Critical gap:** No usage-to-cost correlation tooling exists
2. ⚠️ **Governance challenge:** CISAC needs quantitative reporting, not just logical explanations
3. ✅ **"Noisy neighbor" reality:** A few large agencies likely drive disproportionate costs
4. ✅ **Cost variability is intentional:** Spanish Point treats fluctuations as successful optimization

### Action Items

**For CISAC:**

- [ ] Decide on PSA5499 implementation scope (full vs. partial)
- [ ] Evaluate need for usage-to-cost correlation tooling
- [ ] Consider agency cost allocation/chargeback strategy
- [ ] Open support tickets for future cost spike investigations (if needed)

**For Audit Team:**

- [ ] Document cost governance gap in final report
- [ ] Recommend usage-to-cost correlation dashboard development
- [ ] Assess feasibility of agency-level cost tracking
- [ ] Analyze whether cost variability is acceptable for CISAC's financial reporting needs

**For Spanish Point:**

- [ ] Provide Azure DevOps board access (pending CISAC authorization email)
- [ ] Follow up on date filtering bug fix (already forwarded to dev team)

**Related Documents:**

- [Meeting Transcript](../meetings/20251106-[ISWC%20Audit]Cloud%20Cost%20Breakdown%20＆%20Infrastructure%20Configuration-transcript.txt) - Full conversation record
- [Production and Performance Meeting](20251105-SpanishPoint-Meeting1-ProdPerf.md) - Background on PSA5499 proposal context
- [CI/CD Pipeline Meeting](20251105-SpanishPoint-Meeting2-CICD.md) - DevOps and deployment governance
- [PSA5499 Performance Proposition](../resources/performance_proposition/CISACAzureInfrastructureUpdate/CISACAzureInfrastructureUpdate/CISACAzureInfrastructureUpdate.md) - Original proposal document
- [Investigation Planning](Investigation-Planning.md) - Critical Priority #1: Hyperscale Proposal Validation
