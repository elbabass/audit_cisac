# Spanish Point Meeting Topics - Technical Deep-Dive Sessions

**Date:** October 30, 2025
**Purpose:** Plan focused technical sessions with Spanish Point subject matter experts
**Context:** ISWC Audit - Investigation Phase

---

## Executive Summary

We need to schedule **5 focused technical sessions** with Spanish Point to gather production data, understand operational processes, and access technical artifacts necessary for completing our audit.

**Prerequisites:** We received .NET Core 3.1 source code, but production is being upgraded to .NET 8.0 next week. **Please provide the .NET 8.0 version** to ensure our analysis reflects the actual production system.

---

## Topics Overview (Flat List)

1. Production Performance Data & Metrics
2. Cloud Cost Breakdown & Infrastructure Configuration
3. CI/CD Pipelines & DevOps Practices
4. Package Versions & Technical Debt Management
5. Documentation Maintenance & Code Alignment

---

## 1. Production Performance Data & Metrics

We need to understand actual system performance in production to validate our findings and recommendations.

**What we need:**

- Real performance metrics (latency, throughput, response times) from Application Insights or monitoring dashboards
- Evidence supporting the Hyperscale proposal claims (baseline vs target measurements)
- Operational data: error rates, incident frequency, typical workload patterns
- Any performance testing results or load testing reports

**Who should attend:** Operations/infrastructure team, someone with access to monitoring dashboards and production metrics

---

## 2. Cloud Cost Breakdown & Infrastructure Configuration

We need a shared understanding of how the €50k/month cloud spending is distributed to identify optimization opportunities.

**What we need:**

- Detailed cost attribution by Azure service (Cosmos DB, Databricks, SQL, Storage, etc.)
- Current resource configurations (RU provisioning, cluster sizing, auto-scaling settings)
- Cost trends over the last 6-12 months
- Explanation of infrastructure scaling costs (e.g., UAT database size increase estimate)

**Who should attend:** Infrastructure architect (Xiyuan?), someone with access to Azure cost management and billing

---

## 3. CI/CD Pipelines & DevOps Practices

We need to understand the deployment process, build chain, and Infrastructure as Code implementation to assess DevOps maturity.

**What we need:**

- Access to CI/CD pipeline configurations (Azure DevOps/GitHub Actions)
- Infrastructure as Code templates (ARM/Bicep/Terraform files)
- Deployment process walkthrough (dev → UAT → prod)
- Environment configuration management approach
- Build automation and testing strategies

**Who should attend:** DevOps engineer, deployment manager, someone who manages the CI/CD pipelines

---

## 4. Package Versions & Technical Debt Management

We need to understand the upgrade strategy and why certain dependencies are outdated.

**What we need:**

- Complete inventory of package versions (backend + frontend)
- Timeline for .NET 8 upgrade completion
- Current Databricks runtime version and upgrade roadmap
- Strategy for keeping dependencies current
- Explanation of maintenance scope regarding framework upgrades

**Who should attend:** Development lead (Curnan?), technical architect, someone responsible for platform upgrades

---

## 5. Documentation Maintenance & Code Alignment

We need to understand how documentation stays synchronized with code changes.

**What we need:**

- Process for updating documentation when code changes
- How to identify which 2019-2020 documents are still accurate vs outdated
- Definition of Done: what deliverables are required for a completed feature
- Architecture Decision Records (ADRs) or change documentation practices
- Documentation created for recent major changes (e.g., .NET migration)

**Who should attend:** Technical lead, someone responsible for documentation standards, project manager

---

## Meeting Objectives

The goal is to have **collaborative technical discussions** with the right subject matter experts to:

1. Understand production system behavior and operational realities
2. Gain access to technical artifacts needed for our analysis
3. Clarify processes and practices around deployment, upgrades, and documentation
4. Ensure our audit findings reflect the actual system state, not assumptions

---

## Recommended Format

**Separate focused sessions** (1-2 hours each) with relevant experts rather than one large meeting with everyone.

**Proposed Schedule:**

- Session 1: Performance & Operations (Topics 1 + 2)
- Session 2: CI/CD & DevOps (Topic 3)
- Session 3: Technical Debt & Documentation (Topics 4 + 5)

---

**Document Owner:** Bastien Gallay / Guillaume Jay
**Next Action:** Send to Spanish Point to identify appropriate attendees for each session
