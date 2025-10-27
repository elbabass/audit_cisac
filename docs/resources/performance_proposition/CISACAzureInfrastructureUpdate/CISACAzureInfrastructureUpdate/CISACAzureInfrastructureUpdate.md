![](_page_0_Picture_0.jpeg)

![](_page_1_Picture_0.jpeg)

# **Agenda**

- Background & Introduction
- ISWC Architecture
- Matching Engine Architecture
- Consumption & Cost Optimisation
- Effort Required
- Q&A

![](_page_1_Picture_8.jpeg)

# Background

Since the system was established, there have been numerous changes to the services in Azure.

- Enhance overall security by incorporating additional layers into the network architecture.
- Benefit from improved scalability and cost-effectiveness by utilizing the new hyperscale service tier of Azure SQL Database.

![](_page_2_Picture_4.jpeg)

# Introduction

#### What are we trying to achieve

- Swich component to component communication from public networking to private networking.
- Add HTTP layer 7 web application firewall to protect all web traffic.
  - Public portal
  - Agency portal
  - APIs and API developer portal
- Upgrade Sql database edition to Hyperscale sku.

#### How are we planning to do it

- Focus on infrastructure changes only, avoid application change.
- Minimise downtime where possible.
- Dual network path and co-existence if necessary, during transition period.

![](_page_3_Picture_12.jpeg)

![](_page_4_Picture_0.jpeg)

# ISWC Architecture

![](_page_4_Picture_2.jpeg)

![](_page_5_Figure_0.jpeg)

#### Current - Azure Sql Database - Business Critical

![](_page_6_Figure_1.jpeg)

#### New – Azure Sql Database - Hyperscale

![](_page_7_Figure_1.jpeg)

![](_page_7_Figure_2.jpeg)

![](_page_8_Figure_0.jpeg)

#### New - Hub/Shared

![](_page_9_Picture_1.jpeg)

#### New - Hub/Shared Subnets

![](_page_10_Figure_1.jpeg)

#### New - Spoke Subnets

![](_page_11_Figure_1.jpeg)

#### New - Hub/Shared Resources

![](_page_12_Picture_1.jpeg)

![](_page_12_Picture_2.jpeg)

![](_page_12_Picture_3.jpeg)

![](_page_12_Picture_4.jpeg)

![](_page_12_Picture_5.jpeg)

#### New - Spoke/Dev environment Resources

![](_page_13_Picture_1.jpeg)

![](_page_13_Picture_2.jpeg)

![](_page_13_Picture_3.jpeg)

![](_page_13_Picture_4.jpeg)

![](_page_14_Picture_0.jpeg)

# Matching Engine

![](_page_14_Picture_2.jpeg)

![](_page_15_Figure_0.jpeg)

#### New - Hub/Shared

![](_page_16_Picture_1.jpeg)

#### New - Hub/Shared Subnets

![](_page_17_Figure_1.jpeg)

![](_page_18_Figure_0.jpeg)

#### New - Hub/Shared Resources

![](_page_19_Figure_1.jpeg)

![](_page_19_Figure_2.jpeg)

![](_page_19_Figure_3.jpeg)

![](_page_19_Figure_4.jpeg)

#### New - Spoke/Dev environment Resources

![](_page_20_Figure_1.jpeg)

![](_page_20_Figure_2.jpeg)

![](_page_20_Figure_3.jpeg)

![](_page_20_Picture_4.jpeg)

![](_page_20_Figure_5.jpeg)

![](_page_20_Picture_6.jpeg)

![](_page_21_Picture_0.jpeg)

# Consumption & Cost Optimisation

![](_page_21_Picture_2.jpeg)

# Impact

#### Top items with increasing cost

- €500, Data Factory Azure Hosted IR to to Azure Hosted Managed VNet IR
- €600, Front Door/WAF
- €200, VPN gateway
- €200, Databricks standard to premium

#### Top items with decreasing cost

- €3300, Database Business Critical to Hyperscale
  - Based on current reserved instance core count
  - Reserved instance will be returned and re-purchased on the new sku

We expect the decreased cost can cover the increased cost, with additional room for Sql performance upgrade.

![](_page_22_Picture_11.jpeg)

# Azure Sql Database

# Business Critical

• Compute(includes 4 replicas) + Sql License + Storage + Backup

# Hyperscale

- Compute(includes 1 primary replica) + Secondary replicas(LRS: 0…N, ZRS: 1…N) + Storage + Backup
- Sql License is bundled together with Compute, with reduced price, up to 35%.

[Azure SQL Database Hyperscale FAQ -](https://learn.microsoft.com/en-us/azure/azure-sql/database/service-tier-hyperscale-frequently-asked-questions-faq?view=azuresql#can-i-bring-my-on-premises-or-iaas-sql-server-license-to-hyperscale-) Azure SQL | Microsoft [Learn](https://learn.microsoft.com/en-us/azure/azure-sql/database/service-tier-hyperscale-frequently-asked-questions-faq?view=azuresql#can-i-bring-my-on-premises-or-iaas-sql-server-license-to-hyperscale-)

![](_page_23_Picture_7.jpeg)

# Azure Sql Database

# Recommendation:

- Reserved Instance: will review later this year
- Hyperscale: 1 Primary + 1 Secondary
- Performance: More cores, higher performance with the same amount of money

# Pay-as-You-Go

| Core | BC<br>Compute | BC<br>License | BC<br>C+L | HS<br>Per<br>Replica | HS<br>1P+1S |
|------|---------------|---------------|-----------|----------------------|-------------|
| 4    | 865           | 1044          | 1909      | 519                  | 1038        |
|      | 04            | 65            | 69        | 02                   | 04          |
|      |               |               |           |                      |             |
| 8    | 1730          | 2089          | 3819      | 1038                 | 2076        |
|      | 07            | 3             | 37        | 03                   | 06          |
|      |               |               |           |                      |             |
| 12   | 2595          | 3133          | 5729      | 1557                 | 3114        |
|      | 11            | 94            | 05        | 05                   | 1           |
|      |               |               |           |                      |             |
| 16   | 3460          | 4178          | 7638      | 2076                 | 4152        |
|      | 14            | 59            | 73        | 07                   | 14          |
|      |               |               |           |                      |             |
| 20   | 4325          | 5223          | 9548      | 2595                 | 5190        |
|      | 18            | 24            | 42        | 09                   | 18          |
|      |               |               |           |                      |             |

# 1 Year Reserved

| Core | HS<br>Per<br>Replica | HS<br>1P+1S      |
|------|----------------------|------------------|
| 4    | 365                  | 95<br>731<br>9   |
| 8    | 731                  | 91<br>1463<br>82 |
| 12   | 1097                 | 86<br>2195<br>72 |
| 16   | 1463                 | 81<br>2927<br>62 |
| 20   | 1829                 | 76<br>3659<br>52 |

![](_page_24_Picture_9.jpeg)

# Additional Options - Cosmos DB

# Recommendation:

- Reserved Instance: 1 Year with monthly payment
- 1 Year term discount: 20%

# 1 Year Reserved

| Requet<br>Units<br>Second<br>per | Pay-As-You-Go | 1<br>Year<br>Reserved |
|----------------------------------|---------------|-----------------------|
| 100<br>000<br>,                  | 8406          | 91<br>6717<br>85      |
| 120<br>000<br>,                  | 10088         | 29<br>8061<br>42      |
| 140<br>000<br>,                  | 11769         | 67<br>9404<br>99      |
| 160<br>000<br>,                  | 13451         | 06<br>10748<br>56     |
| 180<br>000<br>,                  | 15132         | 44<br>12092<br>13     |
| 200<br>000<br>,                  | 16813         | 82<br>13435<br>7      |
| 220<br>000<br>,                  | 18495         | 2<br>14779<br>27      |

# Historical Usage, under PAYG

![](_page_25_Picture_7.jpeg)

![](_page_25_Picture_8.jpeg)

# Additional Options - Azure Databricks - Unit

# Recommendation:

• If workload or application logic changes, then do not purchase.

# 1-year pre-purchase plan

| Databricks<br>commit<br>unit<br>(DBCU) | Price<br>(with<br>discount) | Discount |
|----------------------------------------|-----------------------------|----------|
| 12<br>500<br>,                         | €11<br>516<br>32<br>,       | 4%       |
| 25<br>000<br>,                         | €22<br>552<br>78<br>,       | 6%       |

# Historical Usage, under PAYG

![](_page_26_Picture_6.jpeg)

![](_page_26_Picture_7.jpeg)

# Additional Options - Azure Databricks - Cluster VM

# Recommendation:

- Reserved Instance: 1 Year with monthly payment
- Cluster VM, 1 Year term discount: 40% to 41%
- Cluster VM Sku: Upgrade to the latest hardware
- If Databricks workload or application logic changes, then do not purchase.

Historical Usage, under PAYG

![](_page_27_Picture_7.jpeg)

![](_page_27_Picture_8.jpeg)

![](_page_28_Picture_0.jpeg)

# Effort Required

![](_page_28_Picture_2.jpeg)

# ISWC

| 2000 | CI/CD<br>and<br>ISWC<br>IaC                                                    |         |  |  |
|------|--------------------------------------------------------------------------------|---------|--|--|
|      | Hub/Shared<br>Update<br>IaC                                                    | 3       |  |  |
|      | Spoke/Environment<br>Update<br>IaC                                             | 5       |  |  |
|      | deployment<br>Create<br>private<br>agents                                      |         |  |  |
|      | CI/CD<br>Update                                                                |         |  |  |
| 3000 | (Dev&UAT&Prod<br>prep)<br>ISWC<br>Conversion                                   |         |  |  |
|      | network<br>private<br>conversion                                               | 1       |  |  |
|      | Databricks<br>recreation                                                       | 4       |  |  |
|      | Sql<br>database<br>sku<br>(dev&UAT&Prod)<br>conversion                         | 25<br>1 |  |  |
|      | CISAC/FastTrack<br>public<br>allowed<br>list<br>SSO<br>IP<br>,                 | 0<br>5  |  |  |
|      | (Dev&UAT&Prod)<br>private<br>connectivity<br>Api<br>ME<br>to                   | 0<br>75 |  |  |
|      | domain<br>front<br>door<br>migration<br>custom<br>to<br>prep                   | 1       |  |  |
|      | Portal<br>notification<br>front<br>door<br>sites<br>maintenance<br>via<br>prep | 2       |  |  |
| 4000 | Prod<br>ISWC<br>Cutover                                                        |         |  |  |
|      | domain<br>front<br>door<br>migration<br>custom<br>to                           | 0<br>25 |  |  |
|      | Portal<br>notification<br>front<br>door<br>sites<br>maintenance<br>via         | 0<br>25 |  |  |
|      | network<br>private<br>conversion                                               | 0<br>25 |  |  |
|      | Databricks<br>recreation                                                       | 0<br>5  |  |  |

![](_page_29_Picture_2.jpeg)

# Matching Engine

| 5000 | CI/CD<br>Matching<br>and<br>Engine<br>IaC                             |         |  |  |
|------|-----------------------------------------------------------------------|---------|--|--|
|      | Hub/Shared<br>Update<br>IaC<br>parameter                              | 1       |  |  |
|      | Spoke/Environment<br>Update<br>IaC<br>parameter                       |         |  |  |
|      | deployment<br>Create<br>private<br>agents                             | 1       |  |  |
|      | CI/CD<br>Update                                                       |         |  |  |
| 6000 | Matching<br>Prod<br>Engine<br>Conversion<br>Non<br>-                  |         |  |  |
|      | network<br>private<br>conversion                                      | 1       |  |  |
|      | Databricks<br>recreation                                              | 2       |  |  |
|      | Sql<br>database<br>sku<br>conversion                                  | 1       |  |  |
| 7000 | Matching<br>Prod<br>Engine<br>Cutover                                 |         |  |  |
|      | network<br>private<br>conversion                                      | 0<br>25 |  |  |
|      | Sql<br>database<br>sku<br>conversion                                  | 0<br>25 |  |  |
|      | Databricks<br>recreation                                              | 0<br>5  |  |  |
|      | search<br>outbound<br>shared<br>privatelink<br>database<br>ISWC<br>to | 0<br>25 |  |  |

![](_page_30_Picture_2.jpeg)

# Continue…

| 1000 | Project<br>Management  |   |  |  |
|------|------------------------|---|--|--|
|      | PM                     | 6 |  |  |
|      | Architecture<br>design | 3 |  |  |

| 8000 | Deployment<br>Drawdown<br>Post                                                  |         |
|------|---------------------------------------------------------------------------------|---------|
|      | Handover                                                                        | 1       |
|      | fixing<br>Issue                                                                 | 5       |
|      | CISAC/FastTrack<br>old<br>public<br>allowed<br>list<br>SSO<br>IP<br>remove<br>, | 0<br>25 |

![](_page_31_Picture_3.jpeg)

![](_page_32_Picture_0.jpeg)

#### PM Cost:

9 days x €918.60 = €8267.4

#### Developer Cost:

39.5 days x €794.7 = €31390.65

#### Total Cost:

€8267.4 + €31390.65 = €39658.05

| WBS   | Description                                               | Days |      |
|-------|-----------------------------------------------------------|------|------|
| 1000  | Project Management                                        |      |      |
|       | PM                                                        |      | 6    |
|       | Architecture design                                       |      | 3    |
| 2000  | ISWC IaC and CI/CD                                        |      |      |
|       | Update Hub/Shared IaC                                     |      | 3    |
|       | Update Spoke/Environment IaC                              |      | 5    |
|       | Create private deployment agents                          |      | 1    |
|       | Update CI/CD                                              |      | 3    |
| 3000  | ISWC Conversion (Dev&UAT&Prod prep)                       |      |      |
|       | private network conversion                                |      | 1    |
|       | Databricks recreation                                     |      | 4    |
|       | Sql database sku conversion (dev&UAT&Prod)                |      | 1.25 |
|       | CISAC/FastTrack SSO, public IP allowed list               |      | 0.5  |
|       | private connectivity to ME Api (Dev&UAT&Prod)             |      | 0.75 |
|       | prep custom domain migration to front door                |      | 1    |
|       | prep Portal sites maintenance notification via front door |      | 2    |
| 4000  | ISWC Prod Cutover                                         |      |      |
|       | custom domain migration to front door                     |      | 0.25 |
|       | Portal sites maintenance notification via front door      |      | 0.25 |
|       | private network conversion                                |      | 0.25 |
|       | Databricks recreation                                     |      | 0.5  |
| 5000  | Matching Engine IaC and CI/CD                             |      |      |
|       | Update Hub/Shared IaC parameter                           |      | 1    |
|       | Update Spoke/Environment IaC parameter                    |      | 1.5  |
|       | Create private deployment agents                          |      | 1    |
|       | Update CI/CD                                              |      | 0.75 |
| 6000  | Matching Engine Non-Prod Conversion                       |      |      |
|       | private network conversion                                |      | 1    |
|       | Databricks recreation                                     |      | 2    |
|       | Sql database sku conversion                               |      | 1    |
| 7000  | Matching Engine Prod Cutover                              |      |      |
|       | private network conversion                                |      | 0.25 |
|       | Sql database sku conversion                               |      | 0.25 |
|       | Databricks recreation                                     |      | 0.5  |
|       | search outbound shared privatelink to ISWC database       |      | 0.25 |
| 8000  | Post Deployment Drawdown                                  |      |      |
|       | Handover                                                  |      | 1    |
|       | Issue fixing                                              |      | 5    |
|       | CISAC/FastTrack SSO, remove old public IP allowed list    |      | 0.25 |
| Total |                                                           |      | 48.5 |

# ISWC - Web Application Firewall - Optional

#### PM Cost:

3 days x €918.60 = €2755.8

#### Developer Cost:

40 days x €794.7 = €31788

#### Total Cost:

€2755.8 + €31788 = €34543.8

| WBS   | Description                                      | Days |  |  |
|-------|--------------------------------------------------|------|--|--|
| 1000  | Project<br>Management                            |      |  |  |
|       | PM                                               | 3    |  |  |
| 2000  | rules<br>fine<br>-tuning<br>ISWC<br>WAF<br>OWASP |      |  |  |
|       | public<br>portal                                 | 10   |  |  |
|       | portal<br>agency                                 | 15   |  |  |
|       | via<br>APIs<br>APIM                              | 15   |  |  |
| Total |                                                  | 43   |  |  |

![](_page_34_Picture_0.jpeg)

![](_page_34_Picture_1.jpeg)

#### Stay in Touch

- Spanish Point Technologies
- @Spanish\_Point
- [www.spanishpoint.ie](http://www.spanishpoint.ie/)

# Thank You