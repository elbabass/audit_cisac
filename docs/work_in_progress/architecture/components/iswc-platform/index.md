# ISWC Platform Components

This directory contains Level 3 component documentation for containers within the ISWC Platform system.

## Web Applications

- [Agency Portal](agency-portal.md) - Web portal for agencies to manage work registrations (v2.0)
- **Public Portal** - Public search portal (same codebase as Agency Portal, different mode) - *Source code in Portal project*

## APIs

- **Agency API** - REST API for agency work submissions - *Documentation pending*
- **Label API** - REST API for label submissions - *Documentation pending*
- **Publisher API** - REST API for publisher submissions - *Documentation pending*
- **Third Party API** - REST API for external integrations - *Documentation pending*

## Background Processing

- **ISWC Jobs** - Azure Functions for scheduled background jobs - *Documentation pending*
- [Databricks](databricks.md) - Big data processing workspace (v1.1)
- **Data Factory** - ETL pipeline orchestration - *Documentation pending*

## Data Storage

- [Cosmos DB](cosmos-db.md) - NoSQL document database (MongoDB API) (v1.0)
- **SQL Server** - Azure SQL databases (ISWC + IPI) - *Documentation pending*
- **Data Lake** - Azure Data Lake Storage Gen2 - *Documentation pending*

## Related Documentation

- [C4 Level 2: Container View](../../c4-views/level2-containers.md) - Complete container overview
- [C4 Architecture Master](../../c4-architecture-master.md) - Navigation hub

---

**Note:** The ISWC Platform system includes all application, processing, and data containers. There is no separate "Data Platform" system - all data containers (Databricks, Data Factory, databases) are part of the ISWC Platform architecture.
