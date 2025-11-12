# Executive Summary - ISWC System Audit First Restitution

[← Back to Deliverables Index](./index.md)

**Document:** ISWC System Audit - Draft Restitution Presentation
**Date:** November 24, 2025
**Audience:** CISAC Piloting Committee
**Duration:** 1.5 hours (45 min presentation + 45 min discussion)

## 1. Main Findings and Recommendations

### FINDING 1: Platform is Technically Solid

- ✅ Architecture well-designed (12 containers, clean separation, industry-standard Azure patterns)
- ✅ Code quality acceptable (well-structured, navigable, follows .NET best practices)
- ✅ Recently upgraded (.NET 3.1 → 8.0, React 16 → 18, security vulnerabilities addressed)
- ✅ DevOps mature (700+ automated tests, fully automated CI/CD, 20-30 min build+deploy)
- ✅ Auto-scaling functioning correctly (cost variations = feature, not bug)

**Conclusion:** Technical quality is not the problem. The platform works.

### FINDING 2: Governance and Control Are the Challenges

- 🔴 **May 2024 production incident** - POC code merged to production, 6 months recovery
- 🔴 **No deployment tracking pre-CAB** - "They did everything when they wanted without warning"
- 🔴 **Access challenges** - 25% of audit duration lost waiting for code access
- 🔴 **Cost control gap** - €600K/year spending, no automated correlation with usage
- 🔴 **Transparency issues** - Defensive posture, information shared reluctantly
- ⚠️ **Documentation drift** - Specs from 2019-2020, Definition of Done incomplete

**Conclusion:** Organizational and relationship issues, not technical failures.

### FINDING 3: Vendor Lock-in is Real But Manageable

**Three lock-in mechanisms identified:**

1. **Matching Engine** (CRITICAL)

   - ✅ Technically: Clean REST API separation (could be replaced)
   - 🔴 Contractually: Source code only on termination
   - ❓ Unknown: Do alternative matching engines exist?

2. **IaC Templates** (CRITICAL)

   - 🔴 Not included in source delivery (proprietary Smart AIM library)
   - Impact: Environment replication impossible without rebuild or licensing
   - Vendor switch blocker

3. **Knowledge Transfer** (HIGHEST RISK)
   - ⚠️ Minimal code comments, significant duplication
   - 🔴 No onboarding process
   - ❓ **CRITICAL UNKNOWN:** Can independent vendor maintain this code?

**Vendor switch preliminary estimate:** 12-24 months, €300-600K (VERY LOW CONFIDENCE)

**Conclusion:** Technical coupling is low. Organizational coupling is high. Knowledge transfer viability is UNKNOWN (must test).

### KEY RECOMMENDATIONS

**IMMEDIATE (High Priority):**

1. **✅ Approve Knowledge Transfer Pilot Test** (€10-20K, Dec 2025 - Feb 2026)

   - Assign small feature to independent vendor
   - Validate maintainability with available materials
   - **CRITICAL data point** for vendor independence strategy

2. **✅ Initiate Contract Renegotiation** (Dec 2025 - Mar 2026)

   - Use audit findings as leverage
   - Priority terms: Git history, IaC templates, cost correlation tooling
   - Better terms achievable whether CISAC stays or switches

3. **✅ Research Alternative Matching Engines** (€5-10K, Jan-Feb 2026)
   - Market research for music rights matching technology
   - Strengthens negotiating position even if CISAC never switches
   - Informs long-term strategy

**SECONDARY:**

4. **⚠️ HOLD Hyperscale Proposal** (€40K migration + €1,800/month savings)
   - Validate SQL bottleneck first (meet with Moïse)
   - Explore query optimization alternative (€5-10K vs €40K)
   - Decouple networking security from SQL tier decision
   - Decision by March 2026 with validation data

**DEFERRED:**

5. **⏸️ Final Vendor Strategy Decision** (March 2026 after Phase 1 results)
   - Don't decide today - wait for pilot, research, negotiation outcomes
   - Three possible paths: Vendor Transition / Improved Relationship / Partial Independence
   - Decide with evidence, not assumptions

## 2. Items Needing Confirmation from User

### TECHNICAL VALIDATION NEEDED (from Moïse/CISAC Operations)

- [ ] What are actual production performance metrics? (response times, throughput)
- [ ] What user performance complaints have been reported?
- [ ] Is ISWC API actually rate-limited to 3-5 req/sec? (Yann's Oct 30 claim)
- [ ] What features/improvements are agencies requesting?
- [ ] What are typical production issues encountered?

### STRATEGIC DECISIONS NEEDED (from CISAC Leadership)

- [ ] Approve €10-20K pilot test budget? (Recommendation: YES)
- [ ] Approve €5-10K market research budget? (Recommendation: YES)
- [ ] Authorize contract renegotiation? (Recommendation: YES)
- [ ] Which pilot feature is appropriate for testing?
- [ ] Which contract terms are highest priority?
- [ ] Is March 2026 decision timeline feasible?

### VERIFICATION OF ASSUMPTIONS

- [ ] Confirm €600K/year cloud spending figure is accurate
- [ ] Validate that no formal performance SLAs exist in current contract
- [ ] Confirm CAB was established in May 2024 (not earlier)
- [ ] Verify 20-day environment extension quote (Yann's testimony)

## 3. Items Needing Further Exploration

### CRITICAL UNKNOWNS (Must Investigate)

1. **Knowledge Transfer Viability** - €10-20K pilot test required

   - Can independent vendor deliver with available materials?
   - What is Spanish Point's cooperation level with handover?
   - This determines vendor switch feasibility entirely

2. **Alternative Matching Engines** - €5-10K market research

   - Do viable alternatives exist in music rights technology market?
   - API compatibility with current integration?
   - This determines severity of Matching Engine lock-in

3. **SQL Server Bottleneck** - Performance validation with Moïse
   - Is SQL Server actually causing performance issues?
   - Can query optimization achieve similar results to Hyperscale?
   - This determines Hyperscale proposal justification

### MEDIUM PRIORITY UNKNOWNS

4. **Smart AIM Library Licensing** - Negotiate with Spanish Point

   - What does it cost to license IaC templates for third-party vendor?
   - This affects vendor switch cost calculation

5. **Environment Extension Cost** - Spanish Point justification needed

   - Why 20 person-days for environment setup if IaC exists?
   - Should be 0.5 days if properly automated

6. **Cost Optimization Potential** - Detailed Azure cost analysis
   - How much can be saved through reserved instances, archival policies, right-sizing?
   - Estimated 10-20% (€60-120K/year) but needs validation

### LOW PRIORITY EXPLORATION

7. **Git History Analysis** - If/when access granted

   - Code evolution patterns, developer turnover, knowledge concentration
   - Informs maintainability risk assessment

8. **Vendor Switch Detailed Scoping** - IF pilot test succeeds
   - Detailed RFP process with vendor proposals
   - Refine 12-24 month timeline and €300-600K cost estimate

## 4. Key Facts Requiring Proof/Mitigation

**Facts presented in slides that should be highlighted as needing verification:**

### ESTIMATES WITH LOW CONFIDENCE

- **Vendor switch cost €300-600K** - Educated guess, NOT scoped. Requires detailed vendor proposals to validate.
- **Vendor switch timeline 12-24 months** - Preliminary estimate with HIGH uncertainty. Depends on pilot test results and detailed scoping.
- **Cost optimization 10-20% potential** - Order of magnitude estimate. Requires detailed Azure cost breakdown analysis to confirm.
- **Hyperscale savings €3,300/month** - Spanish Point estimate based on single month. Auto-scaling means actual savings may vary.

### ASSUMPTIONS PRESENTED AS CONCERNS

- **Knowledge transfer HIGH RISK** - Based on code review observations (minimal comments, duplication, no onboarding). **Must test with pilot** to validate or refute.
- **ISWC API rate limit 3-5 req/sec** - Yann's Oct 30 statement, NOT confirmed in Nov 5 workshop. Needs Moïse validation.
- **SQL Server bottleneck** - Not validated. Spanish Point claims "no issues." Performance audit needed before Hyperscale approval.

### FACTS THAT ARE SOLID (Can Present with Confidence)

- ✅ May 2024 production incident (Yann testimony, well-documented)
- ✅ 25% audit time lost to access delays (Timeline documented)
- ✅ No cost correlation tooling (Spanish Point confirmation Nov 6)
- ✅ IaC templates not included (Spanish Point Nov 5 workshop)
- ✅ Matching Engine contractual restriction (Confirmed by all parties)
- ✅ .NET 3.1 → 8.0 upgrade completed Nov 4 (Code delivery evidence)
- ✅ 700+ automated tests (Nov 5 workshop testimony)
- ✅ Pipeline test runner blocked post-upgrade (Nov 5 workshop)

## 5. Presentation Readiness Assessment

### STRENGTHS

- ✅ Comprehensive narrative (29 slides, 45 min target)
- ✅ All facts referenced with sources (file paths, quotes, timestamps)
- ✅ Direct and transparent tone (governance failures called out)
- ✅ Clear strategic recommendations with rationale
- ✅ Actionable next steps with timeline and responsibilities
- ✅ Discussion prompts throughout for audience engagement

### AREAS REQUIRING USER REVIEW

- ⚠️ **Verify all Yann quotes** - Ensure comfortable with direct French quotes and translations being presented
- ⚠️ **Validate cost figures** - Confirm €600K/year and other financial numbers are appropriate to share
- ⚠️ **Review vendor criticism tone** - Ensure transparency issues and defensive posture claims are acceptable to present
- ⚠️ **Confirm pilot feature examples** - Are suggested pilot features (validation rule, report, UI enhancement) realistic?
- ⚠️ **Timeline feasibility** - Can CISAC realistically execute Phase 1 by March 2026?

### RECOMMENDED ADJUSTMENTS (Guillaume & Bastien to discuss)

- Consider merging some slides to fit 45 min strictly (currently 29 slides = ~90 sec/slide avg)
- Optional slides strategy: Mark slides 11-12 (Performance/Cost detail), 19-20 (Matching Engine detail), 26 (Hyperscale) as "optional deep-dives" that can be skipped if time is short
- Priority slides: 1-6 (Journey + Executive Summary), 13-17 (Governance), 22-29 (Recommendations + Next Steps)

### DELIVERABLES STATUS

- ✅ Presentation structure complete (29 slides + annexes)
- ✅ Detailed speaker notes for each slide
- ✅ Supporting annexes with references
- ✅ Executive summary for user review
- ⏳ **PENDING:** User review and feedback
- ⏳ **PENDING:** Mermaid diagrams rendering check (if presenting in format that supports them)
- ⏳ **PENDING:** Linting fixes for markdown

### NEXT STEPS

1. **User review** - Bastien & Guillaume review this document
2. **Fact verification** - Double-check quotes, costs, timelines
3. **Slide merging** - Optimize for 45 min strict timing
4. **Practice run** - Rehearse to validate flow and timing
5. **CISAC feedback** - Share outline/key messages for pre-approval
6. **Final polish** - Incorporate feedback, fix linting
7. **Presentation** - Deliver to CISAC piloting committee

---

**END OF EXECUTIVE SUMMARY**

---

**Document Statistics:**

- Total Slides: 29
- Presentation Duration Target: 45 minutes (~90 seconds per slide average)
- Discussion Time: 45 minutes
- Total Session: 1.5 hours
- Word Count: ~38,000 words (comprehensive with speaker notes)
- References: 20+ source documents cited
- Annexes: 4 (Findings Reference, Stakeholder Quotes, Validation Rules, Verification Items)

**Created:** November 7, 2025
**Authors:** Teragone-Factory (Guillaume Jay, Bastien Gallay)
**For:** CISAC ISWC System Audit - First Restitution

---

[← Back to Deliverables Index](./index.md)
