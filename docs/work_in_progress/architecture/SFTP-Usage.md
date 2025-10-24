# SFTP Usage in the ISWC System

**Document Version:** 2.0
**Date:** October 24, 2025 (Updated with Core Design Documents)

**Sources:**

- **Primary:** SPE_20190806_ISWC_EDI_FileFormat.md (Core Design Document)
- **Primary:** ISWCIA20-0312_Guidelines_ISWC_Database_Acknowledgements_Notifications_Workflow_2020-06-18_EN.md (Core Design Document)
- **Secondary:** Workshop 2 (Oct 21, 2025) - Documentation and Infrastructure

---

## Overview

SFTP (SSH File Transfer Protocol) is the **standard for transmission** in the ISWC system, allowing music rights societies (agencies) worldwide to exchange data files for work registrations, updates, queries, and notifications.

> **From EDI File Format Spec:** "The standard for transmission will be the Secure File Transfer Protocol (SFTP). Each ISWC agency will push files to an agency specific secure folder in a central ISWC Database public SFTP site and pull files from that secure folder."

---

## Primary Purpose

**SFTP** is the **official transmission standard** for the ISWC Database, enabling agencies (music rights societies) to:

1. **Push files TO** the ISWC Database:
   - Work registrations (CAR - Create/Add Records)
   - Work updates (CUR - Change/Update Records)
   - Work deletions (CDR - Cancel/Delete Records)
   - Merge requests (MER - Merge Records)
   - Queries (CMQ/CIQ - Query Records)

2. **Pull files FROM** the ISWC Database:
   - Acknowledgements (ACK files)
   - Notifications (CSN files)
   - Query responses

Each agency operates from their **own secure folder** on the central ISWC Database SFTP site.

---

## How It Works

### User Access Model

> **Xiyuan Zeng (WS2, 33:10):** "There's also a SFTP where different agencies, different societies, maybe that's the correct term, and they have their own SFTP account so that they log into the SFTP server over SFTP protocol, so that they upload the download files."

**Key Characteristics:**
- **Per-agency accounts** - Each society/agency has unique SFTP credentials
- **Isolated access** - Agencies only see their own files
- **Bidirectional** - Both upload and download capabilities
- **Protocol** - Standard SFTP over SSH

---

## Technical Architecture

### Infrastructure Components

> **Xiyuan Zeng (WS2, 33:10):** "The SFTP server is just... it's just there's no third-party component... it's native feature SFTP part of a linux operating system itself no third party nothing whatsoever."

**Technical Stack:**

| Component | Technology | Notes |
|-----------|-----------|-------|
| SFTP Server | Native Linux SFTP | No third-party software |
| Operating System | Linux VM in Azure | Built-in SSH/SFTP daemon |
| Storage Backend | Azure Storage Account | File shares for uploaded files |
| Segregation | Per-agency directories | Isolated storage per society |

### Storage Architecture

> **Xiyuan Zeng (WS2, 33:30):** "The sap file uploading will end up in a error storage file share. Storage account file service. You can see the file in your airport even right now. There are storage account in there for SFTP files, and each society is there if I got directly uploaded to the storage account."

**Flow:**
```
Agency SFTP Login
    ↓
Linux SFTP Server (Azure VM)
    ↓
Azure Storage Account (File Service)
    ↓
Agency-specific directory/file share
```

**No Intermediate Storage:**
- Files go **directly** to Azure Storage Account
- No temporary file system staging
- Immediate availability for processing

---

## File Processing Workflow

### Complete Data Pipeline

From the architecture diagram and Workshop 2 discussions:

```
┌─────────────────┐
│ Agency uploads  │
│ file via SFTP   │
└────────┬────────┘
         ↓
┌─────────────────────┐
│ Linux SFTP Server   │
│ (Azure VM)          │
└────────┬────────────┘
         ↓
┌─────────────────────┐
│ Azure Storage       │
│ Account (Files)     │
└────────┬────────────┘
         ↓
┌─────────────────────┐
│ Data Factory        │
│ (Orchestration)     │
└────────┬────────────┘
         ↓
┌─────────────────────┐
│ Databricks          │
│ (Python/PySpark)    │
└────────┬────────────┘
         ↓
┌─────────────────────┐
│ SQL Server or       │
│ Cosmos DB           │
└─────────────────────┘
```

### Processing Details

> **Mark Stadler (WS2, 1:18:44):** "If you're writing like a new file, like if you dropped a new file into SFTP and you wanted for a certain process in a certain way, there would be a lot of work in processing that file in Databricks. So the API and Databricks would be probably the two biggest, two most involved things."

**Processing Characteristics:**
- **Triggered by file arrival** - Data Factory detects new files
- **Databricks processing** - Heavy computation for file parsing and validation
- **Format-specific pipelines** - Different file types require different processing
- **Most active component** - Databricks is one of the two most frequently modified parts

---

## File Types and Naming Conventions

### File Naming Standard

All files follow this naming pattern:

```
ISWCYYYYMMDDHHMMSSRRRSSS.extension
```

Where:
- **YYYY** = Year (4 digits)
- **MM** = Month (2 digits)
- **DD** = Day (2 digits)
- **HH** = Hour (2 digits, 24-hour format)
- **MM** = Minute (2 digits)
- **SS** = Second (2 digits)
- **RRR** = Reserved (3 digits, currently unused - set to 000)
- **SSS** = Sequence number (3 digits, 001-999)

**Extensions:**
- `.CSN` - Uncompressed file
- `.zip` - Compressed file (must contain single CSN file with matching name)

**Example:** `ISWC20251024143022000001.CSN`

### Transaction File Types

| File Type | Extension | Direction | Purpose |
|-----------|-----------|-----------|---------|
| **CAR** | .CSN | Agency → ISWC | Create/Add Records - New work registrations |
| **CUR** | .CSN | Agency → ISWC | Change/Update Records - Update existing works |
| **CDR** | .CSN | Agency → ISWC | Cancel/Delete Records - Delete works |
| **MER** | .CSN | Agency → ISWC | Merge Records - Merge duplicate works |
| **CMQ** | .CSN | Agency → ISWC | Change/Update Query - Query about works |
| **CIQ** | .CSN | Agency → ISWC | Create Inquiry - Initial queries |
| **ACK** | .CSN | ISWC → Agency | Acknowledgement - Confirms file receipt and processing status |
| **CSN** | .CSN | ISWC → Agency | **Notification - Dual purpose (see note below)** |

### 🔔 CSN File Dual Purpose

**CSN files serve TWO distinct purposes:**

1. **Acknowledgements for CIS-Net submissions:**
   - When agencies submit via CIS-Net interface
   - ISWC sends CSN acknowledgement files via SFTP
   - Contains processing results and errors

2. **Notifications for database updates:**
   - When works are updated by ANY method (SFTP, API, portal, etc.)
   - ISWC sends CSN notification files to relevant agencies
   - Keeps agencies informed of changes to works in their catalog

> **From Guidelines:** "CSN files (Notifications) inform the societies of modifications (additions, updates, mergers and deletions) of works for which they hold rights (have an IP, and possibly an ISWC, in the database)."

### File Format Options

Two formats are supported:

1. **EDI (Electronic Data Interchange)** - Default format
   - Fixed-length, positional fields
   - Character-based encoding
   - Legacy format used since system inception
   - Full specification in SPE_20190806_ISWC_EDI_FileFormat.md

2. **JSON** - Alternative format
   - Structured JSON objects
   - More modern and readable
   - Same data model as EDI
   - Referenced in core documentation

### ⚠️ CONTRADICTION: File Format vs CWR

**Workshop 2 mentioned CWR files** but **core design documents specify EDI and JSON formats only**.

- CWR (Common Works Registration) is an industry standard
- EDI format in ISWC may be CWR-based or CWR-adjacent
- Need clarification: Is EDI format actually CWR, or a custom CISAC format?
- **Action item:** Verify relationship between EDI format and CWR standard

---

## Two-Way Communication

### Push (Agency → ISWC Database)

**Transaction Types:**

- **CAR** - New work registrations
- **CUR** - Update existing work information
- **CDR** - Delete works
- **MER** - Merge duplicate works
- **CMQ/CIQ** - Queries about works

**Processing:**

- Files are validated for format compliance
- Transaction-level validation (mandatory fields, data types, etc.)
- Business rule validation (e.g., ISWC uniqueness)
- Results returned via ACK files

### Pull (ISWC Database → Agency)

**Response Types:**

- **ACK** - Acknowledgement files with processing results
  - Confirms receipt and processing status
  - Contains error details for failed transactions
  - Includes assigned ISWCs for new works

- **CSN** - Notification files (dual purpose)
  - Acknowledgements for CIS-Net submissions
  - Notifications of database changes affecting agency's works

**Agency Responsibility:**

- Agencies must regularly check their SFTP folder
- Download and process ACK/CSN files
- Respond to errors and resubmit corrected data

---

## Integration with Main Data Flow

### Two Primary Ingestion Paths

> **Xiyuan Zeng (WS2, 1:19:40):** "Two incoming paths, one through file uploading through SFTP, processed by data factory Databricks and the store maybe in database for Cosmos DB. The other part is the API."

**Comparison:**

| Aspect | SFTP Path | API Path |
|--------|-----------|----------|
| **Mode** | Batch/Asynchronous | Real-time/Synchronous |
| **Volume** | High volume bulk data | Individual transactions |
| **Processing** | Data Factory + Databricks | App Services (API endpoints) |
| **Latency** | Minutes to hours | Seconds |
| **Use Case** | Bulk registrations, nightly feeds | Interactive operations, queries |
| **Complexity** | File parsing, heavy transformation | Direct CRUD operations |

### Architectural Position

From the system architecture diagram (WS2):

**SFTP sits alongside:**
- Agency Portal (interactive web UI)
- Public Portal (public access)
- API Management (programmatic access)
- Background Jobs (scheduled tasks)

**External component:** SFTP is also used to push files to external systems

---

## Agency Types and SFTP Usage

### ISWC-Eligible vs ISWC-Ineligible Agencies

**ISWC-Eligible Agencies:**

- Can receive ISWC assignments
- Full workflow participation
- Can submit all transaction types

**ISWC-Ineligible Agencies:**

- Cannot receive ISWC assignments
- Can still register works in database
- Participate in matching and notification workflows

### Use Cases

1. **Bulk Work Registration (CAR)**
   - Agencies with thousands of new works
   - Batch file-based submission
   - Efficient for high-volume operations

2. **Regular Updates (CUR)**
   - Update work metadata
   - Correct errors
   - Add missing information

3. **Database Maintenance (CDR, MER)**
   - Delete duplicate or incorrect works
   - Merge works discovered as duplicates
   - Data quality improvements

4. **Information Queries (CMQ/CIQ)**
   - Query works in database
   - Verify registration status
   - Research potential duplicates

5. **Staying Synchronized (CSN files)**
   - Receive notifications of changes
   - Update local databases
   - Maintain data consistency across societies

6. **Alternative to CIS-Net Interface**
   - SFTP provides programmatic access
   - CIS-Net provides web interface
   - Both methods trigger same workflows
   - Both receive responses via SFTP (CSN files for CIS-Net)

---

## Why SFTP Matters

### Business Importance

SFTP is critical for:
- **Global agency network** - Societies worldwide use file exchange
- **Industry standards** - CWR and other standard formats rely on file transfer
- **Operational continuity** - Many agencies have decades of file-based workflows
- **Volume handling** - Bulk operations more efficient than API calls

### Technical Importance

SFTP processing through Databricks represents:
- **Most active development area** - Alongside APIs, most frequent changes
- **Complex processing** - Significant logic in file parsing and transformation
- **Data quality** - Validation rules and error handling
- **Scalability concerns** - Large file processing impacts performance

---

## Related Components

### Upstream Dependencies
- **Linux VM** - Hosts SFTP server
- **Azure Storage Account** - File storage
- **Network security** - Firewall rules, SSH configuration

### Downstream Dependencies
- **Data Factory** - File detection and orchestration
- **Databricks** - File processing and transformation
- **SQL Server** - Processed data storage
- **Cosmos DB** - Alternative storage for certain data types

### Monitoring & Operations
- **Application Insights** - Would monitor processing metrics
- **Key Vault** - May store SFTP credentials
- **Backup policies** - Storage Account backup configuration

---

## Workflow Integration

### Split-Copyright Works

When a work has multiple copyright holders from different societies:

1. **Initial Registration:**
   - First agency submits CAR with their share
   - Work enters database with partial information

2. **Matching and Notification:**
   - System detects potential matches with other submissions
   - Creates "workflow tasks" for manual review
   - Notifies relevant agencies via CSN files

3. **Agency Review:**
   - Agencies receive CSN notifications
   - Review potential matches in their system
   - Can submit updates (CUR) or queries (CMQ) as needed

4. **ISWC Assignment:**
   - After sufficient data quality and consensus
   - ISWC assigned to work (if eligible agency involved)
   - All participating agencies notified via CSN

### CIS-Net Integration

**CIS-Net** is a web-based interface for agencies:

- Alternative to SFTP file submission
- Provides UI for manual submissions
- **Uses SFTP for responses:** CSN files delivered via SFTP
- Allows smaller agencies to participate without automation

### Error Handling

**CSE (CIS-Net Error) Files:**

- Special error format for CIS-Net submissions
- Contains detailed error descriptions
- Delivered via SFTP as CSN files
- Agencies must monitor and address errors

## Questions for Further Investigation

- [x] ~~What specific file formats are supported?~~ **ANSWERED:** EDI and JSON
- [x] ~~What file types exist?~~ **ANSWERED:** CAR, CUR, CDR, MER, CMQ, CIQ, ACK, CSN
- [ ] What is the typical file size range?
- [ ] How many agencies actively use SFTP vs API vs CIS-Net?
- [ ] What are the SLAs for file processing?
- [ ] Are there retry mechanisms for failed file processing?
- [ ] What validation occurs at SFTP level vs Databricks level?
- [ ] How is SFTP authentication managed (keys vs passwords)?
- [ ] What monitoring is in place for failed file uploads?
- [ ] How often are new file format requirements added?
- [ ] **NEW:** What is the relationship between EDI format and CWR standard?
- [ ] **NEW:** How many workflow tasks are created per day on average?
- [ ] **NEW:** What percentage of works require manual matching review?

---

## References

### Core Design Documents

- [SPE_20190806_ISWC_EDI_FileFormat.md](../../resources/core_design_documents/SPE_20190806_ISWC_EDI_FileFormat/SPE_20190806_ISWC_EDI_FileFormat.md) - Complete EDI file format specification
- [ISWCIA20-0312_Guidelines_ISWC_Database_Acknowledgements_Notifications_Workflow_2020-06-18_EN.md](../../resources/core_design_documents/ISWCIA20-0312_Guidelines_ISWC_Database_Acknowledgements_Notifications_Workflow_2020-06-18_EN/ISWCIA20-0312_Guidelines_ISWC_Database_Acknowledgements_Notifications_Workflow_2020-06-18_EN.md) - Workflow guidelines

### Meeting Transcripts

- [Workshop 2 - Documentation & Infrastructure (Oct 21, 2025)](../../meetings/20251021-ISWC%20Audit%20-%20Workshop%202%20-%20Documentations%20and%20infrastructure.txt)

### Key Information Sources

- **EDI Spec (Line 379)** - SFTP as transmission standard
- **Guidelines Doc** - CSN file dual purpose, workflow tasks, agency types
- **Xiyuan Zeng (WS2, 33:10)** - Current Azure SFTP implementation
- **Xiyuan Zeng (WS2, 1:19:40)** - Two ingestion paths (SFTP vs API)
- **Mark Stadler (WS2, 1:18:44)** - Databricks file processing complexity

### Architecture Diagrams

- System architecture diagram shared in Workshop 2 chat
- Shows SFTP as external component for both upload and download

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-10-24 | Audit Team | Initial document based on Workshop 2 findings |
| 2.0 | 2025-10-24 | Audit Team | Updated with core design documents (EDI spec, Guidelines); Added file types, naming conventions, workflow details, CSN dual purpose, agency types; Noted CWR contradiction |

---

## Known Gaps and Contradictions

### ⚠️ File Format Terminology

**Contradiction between sources:**

- **Workshop 2** mentioned "CWR files" as the expected format
- **Core Design Docs** specify "EDI format" and "JSON format"
- **Resolution needed:** Clarify if EDI = CWR or if these are different formats
- **Impact:** Medium - Affects understanding of external integrations and standards compliance

### 🔍 Implementation vs Specification

**Design specification verified, implementation details pending:**

- Core documents describe the **intended design** (2019-2020 specs)
- Workshop 2 described the **current Azure implementation**
- Both align on SFTP as the transmission method
- File processing details (Databricks pipelines) not in core docs
- **Next step:** Review source code to verify implementation matches spec

---

**Status:** Draft - Updated with core design documents, pending code review
**Next Review:** After source code access to verify implementation details and resolve CWR/EDI question
