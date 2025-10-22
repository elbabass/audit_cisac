# CISAC Azure Infrastructure - Text Diagram

## Architecture Overview

This document provides a text-based representation of the CISAC Azure infrastructure architecture.

---

## High-Level Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           EXTERNAL INTERFACES                               │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────────┐     ┌──────────────────┐     ┌──────────────────┐     │
│  │ External/        │     │    Suisa Api     │     │  External/Suisa  │     │
│  │ FastTrack SSO    │     │                  │     │      SFTP        │     │
│  └────────┬─────────┘     └────────┬─────────┘     └──────────────────┘     │
│           │                        │                                        │
└───────────┼────────────────────────┼────────────────────────────────────────┘
            │                        │
            │                        │
┌───────────┼────────────────────────┼─────────────────────────────────────────┐
│           │         ┌──────────────┴──────────────┐                          │
│           │         │                             │                          │
│           ▼         ▼              ┌──────────────▼───────────┐              │
│  ┌────────────────────────┐        │                          │              │
│  │   API Management       │◄───────┤   App Service Plan       │              │
│  │                        │        │                          │              │
│  └───────┬────────────────┘        └──────────────┬───────────┘              │
│          │                                        │                          │
│          │                                        │                          │
├──────────┼────────────────────────────────────────┼──────────────────────────┤
│          │         PORTALS & APIS                 │                          │
├──────────┼────────────────────────────────────────┼──────────────────────────┤
│          │                                        │                          │
│  ┌───────┼────────────────────────────────────────┼──────────┐               │
│  │       ▼                                        ▼          │               │
│  │  ┌──────────────┐    ┌──────────────┐    ┌───────────┐    │               │
│  │  │ ISWC Agency  │    │ ISWC Public  │    │ ISWC Jobs │    │               │
│  │  │   Portal     │───►│   Portal     │    │(Functions)│    │               │
│  │  └──────┬───────┘    └──────┬───────┘    └─────┬─────┘    │               │
│  │         │                   │ │                 │         │               │
│  │         │                   │ │                 │         │               │
│  │         │     ┌─────────────┘ └────────┐        │         │               │
│  │         │     │                        │        │         │               │
│  │         └────►│      ISWC Api          │◄───────┘         │               │
│  │               │                        │                  │               │
│  │               └──────────┬─────────────┘                  │               │
│  │                          │                                │               │
│  └──────────────────────────┼────────────────────────────────┼───────────────┘
│                             │                                │
│                             │                                │
├─────────────────────────────┼────────────────────────────────┼─────────────────┤
│          MONITORING & SECURITY                               │                 │
├─────────────────────────────┼────────────────────────────────┼─────────────────┤
│                             │                               │                 │
│                    ┌────────┴────────┐            ┌─────────┴────────┐        │
│                    │  Application    │            │    Key Vault     │        │
│                    │    Insights     │            │                  │        │
│                    └────────┬────────┘            └─────────┬────────┘        │
│                             │                               │                 │
│                             │                               │                 │
├─────────────────────────────┼───────────────────────────────┼─────────────────┤
│          MATCHING ENGINE LAYER                              │                 │
├─────────────────────────────┼───────────────────────────────┼─────────────────┤
│                             │                               │                 │
│  ┌──────────────────────────┼───────────────────────────────┼──────────┐      │
│  │                          │                               │          │      │
│  │   ┌──────────┐    ┌──────▼──────┐                       │          │      │
│  │   │ ME Portal│    │   ME Api    │                       │          │      │
│  │   └────┬─────┘    └──────┬──────┘                       │          │      │
│  │        │                 │                               │          │      │
│  │        └────────┬────────┘                               │          │      │
│  │                 │                                        │          │      │
│  │                 ▼                                        │          │      │
│  │         ┌───────────────┐                                │          │      │
│  │         │ Search Service│                                │          │      │
│  │         └───────┬───────┘                                │          │      │
│  │                 │                                        │          │      │
│  └─────────────────┼────────────────────────────────────────┼──────────┘      │
│                    │                                        │                 │
│                    │                                        │                 │
├────────────────────┼────────────────────────────────────────┼─────────────────┤
│          DATA PROCESSING & ANALYTICS                        │                 │
├────────────────────┼────────────────────────────────────────┼─────────────────┤
│                    │                                        │                 │
│         ┌──────────┼────────────────────────────────────────┼────────┐        │
│         │          │                                        │        │        │
│         │     ┌────┴──────────┐                    ┌────────┴──────┐ │        │
│         │     │  Databricks   │                    │ Data Factory  │ │        │
│         │     │               │                    │               │ │        │
│         │     └───────┬───────┘                    └───────┬───────┘ │        │
│         │             │                                    │         │        │
│         │             │          ┌─────────────────────────┘         │        │
│         │             │          │                                   │        │
│         └─────────────┼──────────┼───────────────────────────────────┘        │
│                       │          │                                            │
│                       │          │                                            │
├───────────────────────┼──────────┼────────────────────────────────────────────┤
│          DATA STORAGE LAYER                                                   │
├───────────────────────┼──────────┼────────────────────────────────────────────┤
│                       │          │                                            │
│  ┌────────────────────┼──────────┼──────────────────────────┐                 │
│  │                    │          │                          │                 │
│  │   ┌────────────────┴──────┐   │   ┌───────────────────┐  │                 │
│  │   │   Cosmos DB           │   │   │   SQL Server      │  │                 │
│  │   │                       │   │   │                   │  │                 │
│  │   │  ┌─────────┐          │   │   │  ┌──────┐ ┌─────┐│  │                 │
│  │   │  │ WID     │  JSON    │   │   │  │ ISWC │ │ IPI ││  │  ┌───────────┐  │
│  │   │  │ (JSON)  │          │   │   │  │  DB  │ │ DB  ││  │  │ Data Lake │  │
│  │   │  └─────────┘          │   │   │  └──────┘ └─────┘│  │  │           │  │
│  │   │                       │   │   │                   │  │  └───────────┘  │
│  │   │  ┌─────────┐          │   │   └───────────────────┘  │                 │
│  │   │  │ ISWC    │  JSON    │   │                          │                 │
│  │   │  │ (JSON)  │          │   │                          │                 │
│  │   │  └─────────┘          │   │                          │                 │
│  │   └───────────────────────┘   │                          │                 │
│  │                               │                          │                 │
│  └───────────────────────────────┴──────────────────────────┘                 │
│                                                                                │
└────────────────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────────────┐
│                      NETWORKING INFRASTRUCTURE                              │
├────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────────────────────────────────────────────────┐              │
│  │              Virtual Network                             │              │
│  │                                                           │              │
│  │    ┌───────────────┐         ┌──────────────────┐        │              │
│  │    │   Public IP   │────────►│   ISWC SFTP      │        │              │
│  │    │               │         │   (VM)           │        │              │
│  │    └───────────────┘         └──────────────────┘        │              │
│  │                                                           │              │
│  └──────────────────────────────────────────────────────────┘              │
│                                                                             │
└────────────────────────────────────────────────────────────────────────────┘
```

---

## Component Relationships

### External Interfaces
- **External/FastTrack SSO** → API Management → ISWC Agency Portal
- **Suisa Api** → API Management → ISWC Api
- **External/Suisa SFTP** → Public IP → ISWC SFTP (VM)

### API Gateway
- **API Management** serves as the central gateway for:
  - External SSO authentication
  - External API access (Suisa)
  - Internal API routing

### Application Layer
- **ISWC Agency Portal** → ISWC Api
- **ISWC Public Portal** → ISWC Api
- **ISWC Jobs** → ISWC Api
- **App Service Plan** hosts all web applications and APIs

### Monitoring & Security
- **Application Insights** monitors:
  - ISWC Api
  - ISWC Jobs
  - All App Services
- **Key Vault** provides secrets and certificates to:
  - API Management
  - ISWC Jobs
  - Data Factory
  - App Services

### Matching Engine
- **ME Portal** → ME Api → Search Service
- **Search Service** indexes data from:
  - Cosmos DB (WID, ISWC JSON)
  - SQL Server (ISWC, IPI databases)

### Data Processing
- **Databricks** processes data for:
  - Analytics
  - Data transformations
  - Machine learning workflows
- **Data Factory** orchestrates:
  - ETL pipelines
  - Data movement between storage systems
  - Integration with external sources

### Data Storage
- **Cosmos DB** stores:
  - WID documents (JSON)
  - ISWC documents (JSON)
- **SQL Server** hosts:
  - ISWC database
  - IPI database
- **Data Lake** stores:
  - Raw data
  - Processed data
  - Archive data

### Networking
- **Virtual Network** provides:
  - Secure communication between services
  - Network isolation
- **Public IP** enables external access to:
  - ISWC SFTP server
- **ISWC SFTP** (VM) handles:
  - File transfers with external partners
  - Integration with Databricks and Data Factory

---

## Data Flow Examples

### 1. External SSO Authentication Flow
```
External/FastTrack SSO
    ↓
API Management
    ↓
ISWC Agency Portal
    ↓
ISWC Api
    ↓
Application Insights (monitoring)
    ↓
SQL Server / Cosmos DB
```

### 2. Suisa API Integration Flow
```
Suisa Api
    ↓
API Management
    ↓
ISWC Jobs / ISWC Api
    ↓
Data Factory
    ↓
Data Lake / Cosmos DB / SQL Server
```

### 3. SFTP Data Ingestion Flow
```
External/Suisa SFTP
    ↓
Public IP
    ↓
ISWC SFTP (VM in Virtual Network)
    ↓
Databricks / Data Factory
    ↓
Data Lake / Cosmos DB / SQL Server
```

### 4. Matching Engine Query Flow
```
ME Portal
    ↓
ME Api
    ↓
Search Service
    ↓
Cosmos DB (WID/ISWC JSON) + SQL Server (ISWC/IPI DB)
```

### 5. Data Processing Pipeline Flow
```
Data Factory (orchestration)
    ↓
Data Lake (raw data)
    ↓
Databricks (processing)
    ↓
Cosmos DB / SQL Server (processed data)
    ↓
Search Service (indexing)
```

---

## Security & Monitoring Architecture

### Security Components
- **Key Vault**: Centralized secret management for all services
- **API Management**: API gateway with authentication and authorization
- **Virtual Network**: Network isolation and security
- **Public IP**: Controlled external access point

### Monitoring Components
- **Application Insights**: Application performance monitoring for:
  - ISWC Api
  - ISWC Jobs (Azure Functions)
  - All App Services
  - API Management

---

## Storage Architecture

### NoSQL Storage (Cosmos DB)
- **WID Database**: JSON documents for Work Identifier data
- **ISWC Database**: JSON documents for International Standard Musical Work Code

### Relational Storage (SQL Server)
- **ISWC Database**: Structured work registration data
- **IPI Database**: Interested Party Information

### Big Data Storage
- **Data Lake**: Large-scale data storage for:
  - Raw ingestion data
  - Processed analytics data
  - Historical archives

---

## Compute Resources

### App Services
- ISWC Agency Portal
- ISWC Public Portal
- ISWC Api
- ME Portal
- ME Api

### Serverless Compute
- **ISWC Jobs**: Azure Functions for background processing

### Analytics Compute
- **Databricks**: Spark-based data processing and analytics

### Integration Compute
- **Data Factory**: ETL and data integration pipelines

### Virtual Machines
- **ISWC SFTP**: File transfer server

---

## Notes

- All App Services are hosted on a shared **App Service Plan**
- **API Management** serves as the single entry point for external access
- **Key Vault** is referenced by all services requiring secrets
- **Application Insights** provides centralized monitoring
- **Virtual Network** ensures secure communication between services
- **Databricks** and **Data Factory** work together for data processing workflows
- **Search Service** provides fast querying capabilities for the Matching Engine
