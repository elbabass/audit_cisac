# Databricks - File Processing Engine

**Document Version:** 1.0
**Date:** October 24, 2025

**Sources:**

- **Primary:** SPE_20191001_ISWC_IPI_Integration.md (Core Design Document - IPI/ISWC Integration v1.3)
- **Secondary:** Workshop 2 (Oct 21, 2025) - Documentation and Infrastructure
- **Secondary:** Discussion Yann/Guillaume/Bastien (Oct 21, 2025)

---

## Overview

Databricks is a **cloud-based Apache Spark processing engine** used in the ISWC system for large-scale file processing, data transformation, and ETL operations.

> **From [IPI Integration Spec](../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md) → Section 2.1 "IPI EDI file based full resynch process":** "This process will be developed using Azure Data Factory and Azure Databricks."

**Primary Use:** Process large files uploaded via SFTP from agencies worldwide, transforming EDI/JSON formatted data into the ISWC SQL database schema.

**Infrastructure:** Azure Databricks workspace with Python notebooks executing PySpark transformations.

---

## Primary Purpose

Databricks serves as the **heavy-duty data processing engine** for the ISWC system with these key responsibilities:

1. **SFTP File Ingestion** - Process EDI and JSON files uploaded by agencies
2. **IPI Data Synchronization** - Import quarterly IPI full dumps (1GB files) and incremental updates
3. **Data Transformation** - Convert external file formats into ISWC database schema
4. **Large Volume Processing** - Handle batch operations at scale (millions of records)
5. **Orchestration Integration** - Execute as part of Data Factory pipeline workflows

> **Workshop 2, Xiyuan Zeng (22:58):** "Data Lake store the files, whatever binary files in which are formatted. They are in Data Lake and the Databricks also used in combination of a Data Factory to process larger volume of files."

**Role in System:** The processing workhorse for all file-based data ingestion, operating in tandem with Data Factory for orchestration.

---

## Technical Architecture

### Infrastructure Components

From Workshop 2 and Azure infrastructure discussion:

| Component | Technology | Purpose |
|-----------|-----------|---------|
| **Databricks Workspace** | Azure Databricks (Runtime: outdated) | Managed Spark cluster environment |
| **Compute Clusters** | Auto-scaling Spark clusters | Execute Python/PySpark notebooks |
| **Storage Integration** | Azure Data Lake Storage | Source files and intermediate output |
| **Database Connectivity** | JDBC to SQL Server | Write transformed data to ISWC DB |
| **Orchestration** | Azure Data Factory | Trigger notebook execution, pipeline coordination |
| **Secrets Management** | Azure Key Vault | Database credentials, API keys |

### Technology Stack

**Languages:**

- **Python** - Primary language for notebooks
- **PySpark** - Distributed data processing framework
- **SQL** - Database queries and transformations

**Spark Components:**

- DataFrame API for structured data processing
- Spark SQL for query execution
- Cluster auto-scaling for cost optimization

### Storage Architecture

```
SFTP Upload
    ↓
Azure Storage Account (File Shares)
    ↓
Data Lake Storage (Input Folder)
    ↓
Databricks Processing (PySpark Notebooks)
    ↓
Data Lake Storage (Output Folder - Intermediate CSV/Parquet)
    ↓
SQL Server (ISWC Database) + Cosmos DB (Audit)
```

---

## How It Works

### Data Factory + Databricks Integration

> **Workshop 2, Xiyuan Zeng (22:58):** "There's a Data Factory to orchestrate the data movement between different components... Databricks also used in combination of a Data Factory to process larger volume of files."

**Orchestration Flow:**

1. **File Arrival Detection** (Data Factory)
   - Monitors SFTP storage account for new files
   - Triggers pipeline when file detected

2. **Notebook Execution** (Data Factory → Databricks)
   - Data Factory invokes Databricks notebook activity
   - Passes file path and processing parameters

3. **File Processing** (Databricks)
   - Notebook reads file from Data Lake
   - Applies transformations and validation
   - Writes results to SQL Server or intermediate storage

4. **Completion Handling** (Data Factory)
   - Moves processed files to archive folder
   - Logs success/failure status
   - Triggers error notifications if needed

### Development and Deployment Model

> **Workshop 2, Xiyuan Zeng (1:14:09):** "For Databricks, they are actually connected to the Cloud Databricks service. You just run local, you modify the local Python files and the PySpark notebooks. but those execution of those files are in the Cloud."

**Developer Workflow:**

- Notebooks edited locally or in Databricks workspace UI
- Execution always happens in cloud cluster (no local Spark)
- Connected to cloud SQL Server and storage for testing
- No fully local development environment feasible

**Deployment:**

- Notebooks checked into source control
- Deployed to Databricks workspace via CI/CD (details unclear)
- Cluster configurations managed via IaC (Azure ARM templates)

---

## File/Data Formats

### IPI Integration Processing

The primary documented use case is **IPI (Interested Party Information) synchronization** from SUISA (Swiss music society).

#### IPI Quarterly Full Dump

**File Specifications:**

> **From [IPI Integration Spec](../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md) → Section 3.1 "File Location, Naming Convention and Size":**
>
> - **Naming Convention:** `IPInnsss.edi.zip`
>   - `nn` = numeric sequence number
>   - `sss` = society code (always 080 for SUISA)
> - **Size:** ~1GB per file when unzipped
> - **Format:** IPI EDI specification (IPA transactions)
> - **Compression:** Zipped files, unzipped during ingestion

**File Structure:**

```
HDR  (Header - one per file)
GRH  (Group Header - one per file)
  IPA Transaction (one per Interested Party)
    IPA  (IP Add)
    BDN  (Base Data New)
    STN  (Status of new IP)
    NCN  (Name single IP Connection New) - multiple
    NUN  (Name single IP Usage New) - multiple
    MAN  (Membership Agreement New) - multiple
    MCN  (Name multi IP Connection New) - multiple
    ... [additional record types]
GRT  (Group Trailer - one per file)
TRL  (Trailer - one per file)
```

**Processing Logic:**

> **From [IPI Integration Spec](../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md) → Section 3.4 "File Processing":** "The relevant information from these IPA transactions will be extracted, as per the mapping above, into a set of files that mirror the IPI tables in the ISWC database. The process will then replace each table's contents with the corresponding file containing the up-to-date data."

**Data Mapping:**

IPA transactions are parsed and mapped to ISWC database tables:

- `IPA + BDN` → `[InterestedParty]` table
- `STN` → `[Status]` table
- `NCN/MCN/ONN` → `[Name]` + `[NameReference]` tables
- `NUN/MUN/INN/IMN` → `[IPNameUsage]` table
- `MAN` → `[Agreement]` table

**Deduplication:**

> **From [IPI Integration Spec](../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md) → Section 3.5 "Deduplicating [IPI.Name] records":** "Based on the mapping logic... there will be multiple duplicate [IPI.Name] records generated... The notebook should process this file to remove any duplicate entries before it is used to replace the data in the [IPI.Name] table."

#### SFTP Agency File Processing

While not fully specified in available docs, Workshop 2 confirms:

> **Workshop 2, Mark Stadler (1:18:19):** "If you dropped a new file into SFTP and you wanted for a certain process in a certain way, there would be a lot of work in processing that file in Databricks."

**File Types (from SFTP-Usage.md):**

- **EDI Format** - Fixed-length positional fields (legacy)
- **JSON Format** - Structured JSON objects (modern alternative)
- **Transaction Types:** CAR (add), CUR (update), CDR (delete), MER (merge), CMQ/CIQ (queries)

**Processing Complexity:**

- Format validation and parsing
- Business rule validation
- Matching Engine integration for duplicate detection
- Database updates with workflow task generation
- ACK/CSN response file generation

---

## Integration with Other Components

### Upstream Dependencies

**What Databricks depends on:**

- **Azure Data Lake Storage** - Input files and output staging
  - Files uploaded via SFTP stored here
  - IPI quarterly dumps manually placed in "IPI Full Resynch" folder

- **Azure Data Factory** - Orchestration and triggering
  - Determines which notebook to execute
  - Passes file paths and parameters
  - Manages pipeline error handling

- **Azure Key Vault** - Credentials for SQL Server and APIs

- **IPI API (SUISA)** - External API for incremental IPI updates (Note: This is handled by C# WebJob, not Databricks)

### Downstream Dependencies

**What depends on Databricks:**

- **SQL Server** - Receives transformed data
  - `[InterestedParty]`, `[Name]`, `[NameReference]`, `[Status]`, `[Agreement]`, `[IPNameUsage]` tables (IPI schema)
  - Work submission data from agency files

- **Cosmos DB** - Audit trail entries for file processing

- **Data Factory** - Waits for notebook completion
  - Success → archive file, continue pipeline
  - Failure → error handling, notifications

- **Agencies** - Receive ACK/CSN response files
  - Generated after Databricks processing completes
  - Delivered via SFTP

### Related Components

**Alternative ingestion paths:**

> **Workshop 2, Xiyuan Zeng (1:18:44):** "Two incoming paths, one through file uploading through SFTP processed by data factory Databricks... The other part is the API."

1. **SFTP → Data Factory → Databricks → SQL Server** (batch/file-based)
2. **API → App Services → SQL Server** (real-time/transactional)

Both paths result in same data model and trigger same workflows.

### System Architecture Position

From Workshop 2 architecture diagram:

```
┌────────────────────────────────────┐
│  Agency SFTP Folders               │
│  (Azure Storage Account)           │
└──────────────┬─────────────────────┘
               ↓
┌────────────────────────────────────┐
│  Data Lake Storage                 │
│  (Input/Output/Archive folders)    │
└──────────────┬─────────────────────┘
               ↓
┌────────────────────────────────────┐
│  Data Factory Pipelines            │
│  (Orchestration & Triggers)        │
└──────────────┬─────────────────────┘
               ↓
┌────────────────────────────────────┐
│  **DATABRICKS**                    │
│  (PySpark Notebooks)               │
└──────┬────────────────┬────────────┘
       ↓                ↓
┌─────────────┐  ┌─────────────┐
│ SQL Server  │  │  Cosmos DB  │
│ (Metadata)  │  │  (Audit)    │
└─────────────┘  └─────────────┘
```

---

## Use Cases

### Primary Use Cases

#### 1. IPI Quarterly Full Resynch

**Business Scenario:** Replace entire IPI database copy with latest quarterly dump from SUISA.

**Workflow:**

1. CISAC admin manually uploads 1GB zip files to Data Lake "IPI Full Resynch/input" folder
2. Admin uploads "Timestamp" file with high-water-mark datetime
3. Data Factory pipeline triggered (manual or scheduled)
4. Databricks notebook unzips files, validates structure (HDR, GRH, GRT, TRL)
5. Parses IPA transactions, extracts data per field mappings
6. Generates intermediate CSV/Parquet files mirroring IPI tables
7. Deduplicates `[IPI.Name]` records
8. **System enters maintenance mode** (no ISWC transactions accepted)
9. Replaces all IPI table data with new data
10. Updates high-water-mark from Timestamp file
11. **System exits maintenance mode**
12. Moves processed files to archive folder

> **From [IPI Integration Spec](../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md) → Section 3.4 "File Processing":** "While this is happening (the final stage replacement of the IP data in the ISWC database), the process should switch the ISWC system into maintenance mode."

**Frequency:** Occasional/as-needed (quarterly dumps available but not always processed)

#### 2. Agency Work Submission File Processing

**Business Scenario:** Agency uploads EDI/JSON file with CAR (new works) transactions via SFTP.

**Workflow:**

1. Agency uploads file to their SFTP folder (e.g., `ISWC20251024143022000001.CSN`)
2. File stored in Azure Storage Account
3. Data Factory detects file arrival
4. Databricks notebook triggered with file path
5. Reads and parses EDI/JSON format
6. Validates format, mandatory fields, business rules
7. Calls Matching Engine API for duplicate detection
8. Inserts/updates work records in SQL Server
9. Generates workflow tasks for other agencies (if split-copyright)
10. Writes audit trail to Cosmos DB
11. Generates ACK file with results
12. Data Factory places ACK in agency's SFTP folder
13. Archives processed input file

> **Workshop 2, Mark Stadler (1:18:19):** "If you dropped a new file into SFTP... there would be a lot of work in processing that file in Databricks."

**Frequency:** Continuous (agencies upload files 24/7)

#### 3. Batch Data Transformation

**Business Scenario:** Process large volumes of data that would be inefficient via API.

**Examples:**

- Bulk updates to work metadata
- Data quality corrections across thousands of works
- Historical data migration
- Report generation from large datasets

**Benefit:** Distributed processing scales horizontally, handles millions of records efficiently.

---

## Workflow Details

### IPI Full Resynch Process Flow

> **From [IPI Integration Spec](../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md) → Section 3 "IPI EDI File Based Full Resynch Process" (Architecture Diagram):**

```
IPI Quarterly Export Files (1GB each)
    ↓
[Manual Upload to Data Lake: IPI Full Resynch/input]
    ↓
Databricks Notebook Execution:
    1. Unzip files
    2. Validate file structure (HDR, GRH, GRT, TRL)
    3. Parse IPA transactions
    4. Extract data per field mappings
    5. Generate intermediate files (CSV/Parquet)
    6. Deduplicate [IPI.Name] records
    ↓
[ISWC System → Maintenance Mode]
    ↓
Replace IPI tables in SQL Server:
    - [InterestedParty]
    - [Name]
    - [NameReference]
    - [IPNameUsage]
    - [Status]
    - [Agreement]
    ↓
Update high-water-mark from Timestamp file
    ↓
[ISWC System → Normal Mode]
    ↓
Move files to archive folder
```

### Error Handling

**File-Level Validation Failures:**

> **From [IPI Integration Spec](../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md) → Section 3.2 "File Structure & File Level Validation":** "If the file fails this validation, then the process should raise an error and stop at that point."

**Timestamp File Issues:**

> **From [IPI Integration Spec](../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md) → Section 3.6 "Updating the High-Water Mark":** "If the date/time is not valid or the timestamp file can not be found then the overall process should fail before the data in the ISWC database is attempted to be replaced."

**Error Propagation:**

- Databricks notebook fails → Data Factory pipeline fails
- Data Factory logs error in Application Insights
- Email notifications sent (assumed - not documented)
- Manual intervention required

**Partial Failures:**

Not well-specified. Likely all-or-nothing for IPI full resynch (transaction-based approach for SFTP files unclear).

---

## 🔴 CRITICAL: Technical Debt Issues

### Outdated Databricks Runtime

> **From [Yann/Guillaume/Bastien Discussion](../../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt) (Timestamp 18:44, Yann Lebreuilly):** "Databricks n'est plus à jour de plusieurs versions. J'ai eu les gars de Databricks qui sont en train de faire un audit par ailleurs... Quand ils ont vu ça, ils se sont fait, mais vous n'êtes pas du tout sur la dernière version à jour, la Databricks. Donc, vous ne pouvez pas bénéficier, par exemple, de l'IA dans vos requêtes et de, en gros, de super pouvoir de requête par vous-même dans l'application."

**Translation:** "Databricks is outdated by several versions. I had the Databricks team doing an audit... When they saw this, they said, you're not at all on the latest version. So you can't benefit, for example, from AI in your queries and, basically, super query powers in the application."

**Impact:**

- **Missing Features:** AI-assisted queries, auto-optimization, latest Spark improvements
- **Security Risk:** Unpatched vulnerabilities in old runtime
- **Performance:** Modern runtimes have significant optimization improvements
- **Support:** Vendor support for old versions limited or expired
- **Cost:** Inefficient resource usage (newer runtimes optimize cluster sizing)

**Severity:** 🔴 **CRITICAL** - Should be upgraded as part of maintenance contract

---

## Questions for Further Investigation

- [ ] What is the exact Databricks runtime version currently deployed?
- [ ] What is the upgrade path and breaking changes for latest runtime?
- [ ] How many notebooks exist in the workspace?
- [ ] What is the average file processing time (small, medium, large files)?
- [ ] What cluster sizing (nodes, cores, memory) is configured?
- [ ] What is the auto-scaling configuration (min/max nodes)?
- [ ] How much idle time occurs (cluster running but not processing)?
- [ ] What is the monthly Databricks cost breakdown (compute vs DBU)?
- [ ] Are there unit tests for notebook transformations?
- [ ] How is notebook versioning managed (Git integration)?
- [ ] What error rate exists for file processing (% failures)?
- [ ] What monitoring/alerts exist for Databricks job failures?
- [ ] Is there a dead-letter queue for unparseable files?
- [ ] **NEW:** Why hasn't Databricks runtime been updated despite maintenance contract?
- [ ] **NEW:** What features are blocked by outdated runtime?
- [ ] **NEW:** What is the notebook code quality (complexity, modularity, testability)?

---

## Source Code References

The following source code files in the ISWC system interact with or reference Databricks:

### C# Framework & Integration Layer

**Databricks Client Framework** (`src/Framework/Databricks/`)

- [DatabricksClient.cs](../../../resources/source-code/ISWC/src/Framework/Databricks/DatabricksClient.cs) - HTTP client for Databricks Jobs API
- [IDatabricksClient.cs](../../../resources/source-code/ISWC/src/Framework/Databricks/IDatabricksClient.cs) - Interface for Databricks operations
- [DatabricksClientOptions.cs](../../../resources/source-code/ISWC/src/Framework/Databricks/DatabricksClientOptions.cs) - Configuration options for Databricks client

**Models** (`src/Framework/Databricks/Models/`)

- [NotebookParameters.cs](../../../resources/source-code/ISWC/src/Framework/Databricks/Models/NotebookParameters.cs) - Parameters passed to notebook executions
- [RunsListResponseModel.cs](../../../resources/source-code/ISWC/src/Framework/Databricks/Models/RunsListResponseModel.cs) - Response model for active runs
- [SubmitJobRequestModel.cs](../../../resources/source-code/ISWC/src/Framework/Databricks/Models/SubmitJobRequestModel.cs) - Request model for job submission

**Business Layer**

- [ReportManager.cs](../../../resources/source-code/ISWC/src/Business/Managers/ReportManager.cs) - Uses DatabricksClient to trigger report generation jobs

**Dependency Injection**

- [AutofacModule.cs](../../../resources/source-code/ISWC/src/Data/AutofacModule.cs) - Registers Databricks services
- [ServiceCollectionExtensions.cs](../../../resources/source-code/ISWC/src/Portal/Extensions/ServiceCollectionExtensions.cs) - Configures Databricks client
- [Startup.cs](../../../resources/source-code/ISWC/src/Portal/Startup.cs) - Application startup configuration
- [MappingProfile.cs](../../../resources/source-code/ISWC/src/Data/MappingProfile.cs) - AutoMapper configuration for Databricks models

### Python Notebooks & Scripts

**IPI Full Synchronization**

- [IPI FullSync.py](../../../resources/source-code/ISWC/src/Integration/IpiFullSync/IPI%20FullSync.py) - Main IPI quarterly import notebook (763 lines)
  - Parses IPA transactions from EDI files
  - Generates parquet files for IPI database tables
  - Handles deduplication of Name records

**EDI File Processing** (`src/Integration/Edi/ediparser/`)

- [main_csn.py](../../../resources/source-code/ISWC/src/Integration/Edi/ediparser/main_csn.py) - CSN (Work Submission) file processing notebook
- [main_cse.py](../../../resources/source-code/ISWC/src/Integration/Edi/ediparser/main_cse.py) - CSE (Search Query) file processing notebook
- [main_ack.py](../../../resources/source-code/ISWC/src/Integration/Edi/ediparser/main_ack.py) - ACK response file generation notebook
- [process_file.py](../../../resources/source-code/ISWC/src/Integration/Edi/ediparser/process_file.py) - Common file processing utilities
- [job_service.py](../../../resources/source-code/ISWC/src/Integration/Edi/ediparser/parser/services/job_service.py) - Service for checking active Databricks job runs and triggering jobs

**Generic Jobs** (`src/Integration/GenericJob/generic_job/`)

- [main_changetrackerjob.py](../../../resources/source-code/ISWC/src/Integration/GenericJob/generic_job/main_changetrackerjob.py) - Change tracking sync job
- [main_missingipisyncjob.py](../../../resources/source-code/ISWC/src/Integration/GenericJob/generic_job/main_missingipisyncjob.py) - IPI incremental update job
- [main_processauditjob.py](../../../resources/source-code/ISWC/src/Integration/GenericJob/generic_job/main_processauditjob.py) - Audit log processing job
- [cosmos_service.py](../../../resources/source-code/ISWC/src/Integration/GenericJob/generic_job/services/cosmos_service.py) - Cosmos DB integration with Delta Lake
- [delta_service.py](../../../resources/source-code/ISWC/src/Integration/GenericJob/generic_job/services/delta_service.py) - Delta Lake operations

**Reporting** (`src/Reporting/reporting/`)

- [main_report.py](../../../resources/source-code/ISWC/src/Reporting/reporting/main_report.py) - Report generation notebook
- [delta_service.py](../../../resources/source-code/ISWC/src/Reporting/reporting/services/delta_service.py) - Delta Lake read operations
- [SubmissionAnalysis.ipynb](../../../resources/source-code/ISWC/src/Reporting/reporting/SubmissionAnalysis.ipynb) - Jupyter notebook for submission data analysis

### Database Objects

**SQL Views** (`src/Database/Azure/Views/`)

- [Databricks_Iswc_Data.sql](../../../resources/source-code/ISWC/src/Database/Azure/Views/Databricks_Iswc_Data.sql) - View that consolidates ISWC work data for Databricks export (JSON aggregation)
- [Databricks_Ipi_Data.sql](../../../resources/source-code/ISWC/src/Database/Azure/Views/Databricks_Ipi_Data.sql) - View that consolidates IPI interested party data for Databricks export

**Database Project**

- [Database.sqlproj](../../../resources/source-code/ISWC/src/Database/Database.sqlproj) - SQL Server database project including Databricks views

### Documentation & Configuration

**Development Setup**

- [Integration/README.md](../../../resources/source-code/ISWC/src/Integration/README.md) - Databricks Connect setup guide for EDI integration development
- [Reporting/README.md](../../../resources/source-code/ISWC/src/Reporting/README.md) - Databricks Connect setup guide for reporting development

**Ad-Hoc Tools**

- [AddMissingIPIsConsoleApp/Program.cs](../../../resources/source-code/ISWC/src/AdHoc/AddMissingIPIsConsoleApp/Program.cs) - Console app for IPI sync (references Databricks configuration)
- [AddMissingIPIsConsoleApp/appsettings.json](../../../resources/source-code/ISWC/src/AdHoc/AddMissingIPIsConsoleApp/appsettings.json) - Configuration including Databricks settings
- [AddMissingIPIsConsoleApp/README.md](../../../resources/source-code/ISWC/src/AdHoc/AddMissingIPIsConsoleApp/README.md) - Documentation for ad-hoc IPI sync tool

### Key Patterns and Technologies

**Databricks Runtime Version:**

- Python notebooks use `databricks-connect==11.3.*` (Python 3.9)
- **Note:** Runtime is outdated by several versions (see Technical Debt section)

**Job Orchestration Pattern:**

1. C# Portal/API calls `DatabricksClient.SubmitJob()`
2. Databricks REST API (`https://westeurope.azuredatabricks.net/api/2.0/jobs/run-now`)
3. Databricks cluster executes Python notebook
4. Results written to SQL Server or Delta Lake
5. Audit trail logged to Cosmos DB

**Local Development:**

- Developers use `databricks-connect` to run notebooks locally
- Execution happens in cloud cluster (no local Spark)
- Package deployment via wheel files to `cisaciswcdatabricksdev` storage account

**Delta Lake Usage:**

- `spark.databricks.delta.schema.autoMerge.enabled` enabled in multiple jobs
- Delta format for intermediate data storage
- Cosmos DB integration via Delta Lake connector

---

## References

### Core Design Documents

- [SPE_20191001_ISWC_IPI_Integration.md](../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md) - Complete IPI integration specification v1.3 (Nov 2019) including Databricks processing details

### Meeting Transcripts

- [Workshop 2 - Documentation & Infrastructure (Oct 21, 2025)](../../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt)
- [Discussion Yann/Guillaume/Bastien (Oct 21, 2025)](../../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt)

### Key Information Sources

- **[IPI Integration Spec](../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md) → Section 2.1 "IPI EDI file based full resynch process"** - Databricks + Data Factory architecture
- **[IPI Integration Spec](../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md) → Section 3.4-3.6** - File processing workflow and maintenance mode
- **[Workshop 2](../../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt) (22:58, Xiyuan Zeng)** - Databricks role in system architecture
- **[Workshop 2](../../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt) (1:14:09, Xiyuan Zeng)** - Local development model (cloud-connected)
- **[Workshop 2](../../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt) (1:18:19, Mark Stadler)** - Databricks as most involved component for file processing
- **[Yann Discussion](../../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt) (18:44, Yann Lebreuilly)** - Outdated runtime version and missing features

### Related Specifications

- SPE_20190806_ISWC_EDI_FileFormat.md - EDI file format that Databricks parses
- SPE_20191118_ISWC_JSON_FileFormat.md - JSON file format alternative
- SFTP-Usage.md - File exchange mechanism feeding Databricks

### Architecture Diagrams

- **[IPI Integration Spec](../../resources/core_design_documents/SPE_20191001_ISWC_IPI_Integration/SPE_20191001_ISWC_IPI_Integration.md) → Section 3 "IPI EDI File Based Full Resynch Process"** - Data Factory + Databricks processing flow diagram
- **Workshop 2 (shared in chat)** - System architecture diagram showing Databricks cluster resources in Azure

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-10-24 | Audit Team | Initial document based on IPI Integration spec and Workshop 2; Documented file processing architecture, IPI use case, technical debt issues, integration points |

---

## Known Gaps and Contradictions

### 🔴 Runtime Version Urgency

**Critical maintenance gap identified:**

- **Yann's expectation:** Runtime updates should be included in annual maintenance contract
- **Spanish Point's position:** Offered to charge separately for runtime upgrade
- **Yann's response:** Rejected paid upgrade - maintenance should cover this
- **Current status:** Upgrade "in progress" but delayed by months/years
- **Impact:** **HIGH** - Security, performance, cost optimization all affected

> **From [Yann/Guillaume/Bastien Discussion](../../meetings/20251021-ISWC%20-%20Discussion%20Yann_Guillaume_Bastien.txt) (Timestamp 18:44, Yann Lebreuilly):** "Si dans la maintenance, il n'y a même pas la mise à jour du dotnet, il y a quoi dans votre maintenance ? Et là, ils ont fait, OK, fair enough."

**Similar to .NET 3.1 issue** - Basic platform updates should not be billable extras.

### 🔍 Notebook Code Access

**Design specification exists, implementation details pending:**

- Core IPI Integration doc describes processing logic at high level
- Actual notebook code not reviewed yet (access pending)
- Code quality, testing, modularity unknown
- **Next step:** Review notebooks after source code access granted

### ⚠️ SFTP File Processing Specification Gap

**Specification incomplete for primary use case:**

- IPI integration fully specified (187 lines in spec)
- Agency SFTP file processing mentioned in Workshop 2 as "a lot of work"
- No design document found for EDI/JSON parsing logic
- **Resolution needed:** Find SFTP file processing specification or confirm gap
- **Impact:** Medium - Cannot assess 80% of Databricks workload without spec

### 🔔 Cost Attribution Unknown

**Monthly €50K cloud cost breakdown needed:**

- Databricks likely significant portion (compute + DBU charges)
- Cluster idle time could be waste (auto-scaling config unclear)
- Newer runtime could reduce costs (better optimization)
- **Next step:** Azure Cost Management analysis by resource

---

**Status:** Draft - Based on IPI integration spec and workshop discussions, pending notebook code review
**Next Review:** After Databricks runtime version confirmed and notebooks accessed
**Critical Action Item:** Justify why runtime upgrade is delayed - security and cost implications
