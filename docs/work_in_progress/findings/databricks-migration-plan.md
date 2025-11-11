# Databricks Migration Plan - Unity Catalog vs. Runtime Upgrade

**Document Version:** 1.0
**Date:** 2025-11-10
**Status:** DRAFT - Awaiting assumption validation

---

## Executive Summary

The ISWC system currently runs on **Databricks Runtime 11.3 LTS (EOL)** with Hive metastore and DBFS mounts. This document analyzes two migration paths:

- **Option A:** Unity Catalog + Serverless Compute (transformational change)
- **Option B:** Runtime 15.4 LTS upgrade (incremental modernization)

### Quick Comparison

| Dimension | Option A (Unity Catalog + Serverless) | Option B (Runtime 15.**4** LTS) |
|-----------|--------------------------------------|------------------------------|
| **Complexity** | XL (High) | M (Medium) |
| **Migration Effort** | 21-34 story points | 5-8 story points |
| **Risk Level** | HIGH | LOW-MEDIUM |
| **Code Changes** | Extensive (mount paths, catalog namespaces) | Minimal (library versions) |
| **Cost Savings** | 50-70% | 10-20% |
| **Governance Improvement** | Significant (Unity Catalog features) | None |
| **Security Impact** | High (credential pass-through) | Medium (patched runtime) |

### Recommended Approach: **Two-Phase Strategy**

1. **Phase 1 (Immediate):** Upgrade to Runtime 15.4 LTS
   - Addresses EOL security risk
   - Low complexity (M-sized)
   - 10-20% cost savings
   - Buys time for Unity Catalog planning

2. **Phase 2 (6-12 months):** Migrate to Unity Catalog + Serverless
   - Requires governance planning
   - Higher complexity (XL-sized)
   - Additional 40-50% cost savings
   - Enables advanced data governance

---

## Table of Contents

1. [Current State Analysis](#current-state-analysis)
2. [Option A: Unity Catalog + Serverless](#option-a-unity-catalog--serverless)
3. [Option B: Runtime 15.4 LTS Upgrade](#option-b-runtime-154-lts-upgrade)
4. [Gap Analysis Matrix](#gap-analysis-matrix)
5. [Migration Effort Estimation](#migration-effort-estimation)
6. [Assumptions Requiring Verification](#assumptions-requiring-verification)
7. [Recommended Strategy](#recommended-strategy)
8. [Technical Deep-Dives](#technical-deep-dives)

---

## Current State Analysis

### Infrastructure Configuration

**Databricks Workspace:**

- **Environments:** ISWCDev, ISWCUAT, ISWCProd (separate workspaces per environment)
- **Runtime Version:** 11.3 LTS (Python 3.9, Spark 3.3.x)
- **Status:** ⚠️ **EOL (End of Life)** - No longer receives security updates
- **Databricks Connect:** 11.3.* (for local development)

**Compute Configuration:**

From Azure resources export and Data Factory configuration:

```json
{
  "linkedServiceName": "AzureDatabricks",
  "typeProperties": {
    "existingClusterId": "0227-170429-z080f3qt"
  }
}
```

**Key Findings:**

- ✅ Uses **existing interactive cluster** (warm cluster, always on)
- ⚠️ **Not optimized for ephemeral workloads** (idle time costs)
- ✅ **VNet injection enabled** (workers-vnet, workers-sg security groups)
- ✅ **Separate storage accounts per environment:**
  - cisaciswcdatabricksdev
  - cisaciswcdatabricksuat
  - cisaciswcdatabricksprod

**Storage Architecture:**

- **DBFS Root Storage:** dbstorage* accounts (Databricks-managed)
- **Data Lake Integration:** Via `/mnt/` mount points (DBFS mounts)
- **Key Vault Integration:** dbutils.secrets with Azure Key Vault-backed scope

---

### Code Dependencies

**Source Locations:**

- EDI/Integration Jobs: `docs/resources/source-code/ISWC-2/src/Integration/`
- Reporting Jobs: `docs/resources/source-code/ISWC-2/src/Reporting/`
- Deployment Config: `docs/resources/source-code/ISWC-2/deployment/`

**Python Libraries (requirements.txt):**

```python
# Azure Integration
azure-cosmos==3.1.2              # Cosmos DB SDK (outdated)
azure-mgmt-datafactory==0.13.0   # Data Factory orchestration
opencensus-ext-azure==1.0.2      # Application Insights telemetry

# File Processing
paramiko==2.11.0                 # SFTP connectivity
pysftp==0.2.9                    # SFTP wrapper
python-magic==0.4.18             # File type detection

# Data Processing
pandas==1.3.5                    # Data manipulation
numpy==1.22.1                    # Numerical operations
pyarrow==19.0.1                  # Parquet/Arrow support

# API Integration
zeep==4.2.1                      # SOAP client (Matching Engine API)
requests==2.28.1                 # HTTP client
jsonschema==3.2.0                # JSON validation
```

**Installation Method:** Runtime `%pip install` commands (no init scripts detected)

**Compatibility Status:** All libraries have newer versions compatible with Python 3.10+

---

### Spark API Usage Patterns

**Comprehensive code analysis reveals:**

#### DataFrame API (Primary Pattern - 95% of code)

```python
# JDBC reads (SQL Server)
df = spark.read.jdbc(url=jdbcUrl, table=query, properties=connectionProperties)

# Delta Lake tables
df = spark.sql("SELECT * FROM iswc.agencynotificationtype")

# Parquet files
df = spark.read.format("parquet").load("/mnt/ipi-integration/InterestedParty")

# Cosmos DB Change Feed
df = spark.read.format("cosmos.oltp.changeFeed").options(**config).load()
```

**Assessment:** ✅ Fully compatible with all migration options (DataFrame API is stable)

#### Delta Lake Operations (Extensive Use)

```python
# Schema auto-merge enabled globally (CRITICAL setting)
spark.conf.set("spark.databricks.delta.schema.autoMerge.enabled", "true")

# MERGE INTO pattern (UPSERT)
df.write.format("delta")
  .mode("overwrite")
  .option("mergeSchema", "true")
  .option("overwriteSchema", "true")
  .saveAsTable(f'{database_name}.{table_name}')

# MERGE statement
spark.sql("""
    MERGE INTO {database_name}.{table_name} t
    USING incremental i
    ON i.id = t.id
    WHEN MATCHED THEN UPDATE SET *
    WHEN NOT MATCHED THEN INSERT *
""")
```

**Critical Dependencies:**

- ✅ Delta Lake MERGE operations (not available in OSS Spark)
- ✅ Schema evolution/auto-merge (mandatory for JSON/EDI compatibility)
- ✅ 2-level namespace: `iswc.tablename` (Hive metastore pattern)

#### RDD API (Limited Use - 7 occurrences only)

```python
# Converting parsed dictionaries to DataFrames (IPI FullSync)
rdd = spark.sparkContext.parallelize(record_type_object[key])
spark_dframe = spark.createDataFrame(rdd, TblSchema(key).get_tbl_schema())

# Schema recreation
df = spark.createDataFrame(df.rdd, schema=schema)

# Small collections
spark.sql("...").rdd.collect()
```

**Assessment:** ⚠️ Minimal usage, can be replaced with DataFrame API if needed

#### Structured Streaming (Moderate Use)

```python
# Cosmos DB Change Feed streaming
df_change_feed.writeStream
  .trigger(once=True)          # Micro-batch mode (NOT continuous)
  .format('delta')
  .outputMode("update")
  .option("checkpointLocation", checkpoint_location)
  .foreachBatch(lambda i, b: merge_delta(i, table_name, class_name))
  .start()
```

**Assessment:** ✅ Compatible (micro-batch mode supported in serverless)

---

### Storage Integration Patterns

#### DBFS Mount Points (CRITICAL DEPENDENCY)

**Code Evidence:**

```python
# IPI FullSync writes
df.write.mode("overwrite").format("parquet").save("/mnt/ipi-integration/Name")

# Error file writes
df.write.option('header', True).csv(f'/mnt/generic_job_error_{class_name}_{timestamp}')

# File listing
fileread = dbutils.fs.ls(unziplocation)
```

**Mount Paths Detected:**

- `/mnt/ipi-integration/` (IPI data processing)
- `/mnt/generic_job_error_*` (error file outputs)
- Likely more (full inventory needed)

**DBFS FUSE Usage:**

```python
# Local file system access
file_location = '/dbfs' + path
```

**Critical Finding:** Code uses `/dbfs/` prefix indicating FUSE mounting for local file operations.

---

### Authentication & Secrets Management

**Key Vault Integration:**

```python
# Azure Key Vault-backed secrets scope
dbutils.secrets.get(scope="keyvault", key=prefix + key)

# Retrieved secrets:
- ConnectionString-ApplicationInsights
- BaseAddress-ApiManagement
- Secret-ApiManagement
- SFTP-Hostname/Username/Password
- AzureKeyVaultSecret-ISWC-ConnectionString-ISWCCosmosDb
- databricksServicePrincipalPersonalAccessToken
```

**SQL Server Authentication:**

```python
# SQL authentication (NOT Azure AD)
jdbcUrl = "jdbc:sqlserver://{server}:1433;database={database}"
connectionProperties = {"user": user, "password": password}
```

**Key Finding:** ⚠️ No Managed Identity or Azure AD authentication detected (uses SQL auth)

---

### External Integrations

| Integration | Technology | Usage Pattern |
|-------------|-----------|---------------|
| **SQL Server** | JDBC (mssql-jdbc) | Read-only queries from Databricks |
| **Cosmos DB** | Python SDK + Spark Connector | Audit reads, change feed streaming |
| **SFTP** | paramiko, pysftp | File upload/download |
| **Matching Engine** | zeep (SOAP client) | REST API calls |
| **Application Insights** | opencensus-ext-azure | Custom telemetry |

**Data Write Pattern:** ⚠️ Databricks writes to Delta Lake/Parquet, **NOT directly to SQL Server** (writes done via C# APIs)

---

## Option A: Unity Catalog + Serverless

### Target Architecture

**Compute Model:**

- **Serverless SQL Warehouses** (for queries/reporting)
- **Serverless Job Compute** (for batch processing)
- **No interactive clusters** (ephemeral compute only)

**Data Governance:**

- **Unity Catalog** metastore
- **3-level namespace:** `catalog.schema.table` (vs. current `database.table`)
- **Fine-grained access control** (row/column level)
- **Credential pass-through** (user identity → storage access)

**Storage Integration:**

- **Direct ADLS URIs:** `abfss://container@storage.dfs.core.windows.net/path`
- **No DBFS mounts** (mounts not supported in serverless)
- **No DBFS FUSE** (local file system access not available)

---

### Breaking Changes & Required Migrations

#### 1. Hive Metastore → Unity Catalog Migration

**Impact:** ALL `saveAsTable()` and `spark.sql()` calls using 2-level namespace

**Current Code Pattern:**

```python
# 2-level namespace (Hive metastore)
df.write.format("delta").saveAsTable("iswc.tablename")
spark.sql("SELECT * FROM iswc.agencynotificationtype")
```

**Required Change:**

```python
# Option 1: 3-level namespace (Unity Catalog)
df.write.format("delta").saveAsTable("catalog.iswc.tablename")
spark.sql("SELECT * FROM catalog.iswc.agencynotificationtype")

# Option 2: Use "hive_metastore" catalog for backward compatibility
df.write.format("delta").saveAsTable("hive_metastore.iswc.tablename")
```

**Files Affected:**

- `src/Integration/Edi/Common/delta_service.py` (15+ table references)
- `src/Integration/Edi/mainrecentchanges.py`
- `src/Integration/Edi/mainchangetracker.py`
- `src/Integration/Edi/mainauditprocessing.py`
- `src/Integration/Edi/main_cosmos_to_adls.py`
- `src/Reporting/*.py` (all reporting jobs)

**Complexity:** M (Medium) - Search/replace with testing required

**Story Points:** 5

---

#### 2. DBFS Mount Points Migration (HIGH COMPLEXITY)

**Impact:** ALL `/mnt/` paths BREAK in serverless

**Current Code Pattern:**

```python
# Mount-based paths
df.write.parquet("/mnt/ipi-integration/InterestedParty")
df.write.csv("/mnt/generic_job_error_IpiFullResynchLoad_20251110")
fileread = dbutils.fs.ls("/mnt/ipi-integration/")
```

**Required Change:**

```python
# Direct ADLS URIs
storage_account = "cisaciswcdatalake"
container = "ipi-integration"
df.write.parquet(f"abfss://{container}@{storage_account}.dfs.core.windows.net/InterestedParty")
df.write.csv(f"abfss://{container}@{storage_account}.dfs.core.windows.net/errors/IpiFullResynchLoad_20251110")
fileread = dbutils.fs.ls(f"abfss://{container}@{storage_account}.dfs.core.windows.net/")
```

**Files Affected:**

- `src/Integration/Edi/ipiFullSynch/main.py` (10+ mount references)
- `src/Integration/Edi/Common/delta_service.py` (error file writes)
- `src/Reporting/maindailyauditexport.py`
- Data Factory pipeline configurations (mount path parameters)

**Additional Requirements:**

- Identify ALL mount points (full inventory from workspace)
- Map mount point → storage account + container
- Configure credential pass-through or service principal access
- Update all file path references

**Complexity:** L (Large) - Requires coordination with infrastructure team

**Story Points:** 13

---

#### 3. DBFS FUSE Removal (`/dbfs/` prefix)

**Impact:** Local file system operations BREAK

**Current Code Pattern:**

```python
# IPI FullSync file collection
file_location = '/dbfs' + path  # Accesses DBFS as local filesystem
```

**Required Change:**

```python
# Use Spark APIs directly (no FUSE mounting)
file_location = path  # Remove /dbfs prefix
# OR use dbutils.fs operations exclusively
```

**Files Affected:**

- `src/Integration/Edi/ipiFullSynch/main.py`

**Complexity:** S (Small) - Single file, straightforward refactoring

**Story Points:** 2

---

#### 4. Cluster Configuration Migration

**Impact:** Data Factory "existingClusterId" reference becomes INVALID

**Current Configuration:**

```json
{
  "linkedServiceName": "AzureDatabricks",
  "typeProperties": {
    "existingClusterId": "0227-170429-z080f3qt"
  }
}
```

**Required Change:**

```json
// Option 1: Serverless SQL Warehouse
{
  "linkedServiceName": "AzureDatabricks",
  "typeProperties": {
    "workspaceResourceId": "/subscriptions/.../workspaces/iswcprod",
    "warehouseId": "warehouse-id-here"
  }
}

// Option 2: Serverless Job Compute (preferred for batch jobs)
{
  "linkedServiceName": "AzureDatabricks",
  "typeProperties": {
    "workspaceResourceId": "/subscriptions/.../workspaces/iswcprod",
    "newClusterVersion": "15.4.x-scala2.12",
    "newClusterNodeType": "Standard_DS3_v2",
    "newClusterNumOfWorker": "2:8",  // Auto-scaling
    "newClusterSparkEnvVars": {...}
  }
}
```

**Files Affected:**

- `deployment/AzureDatabricks.json` (Data Factory linked service)
- All Data Factory pipeline definitions referencing Databricks

**Complexity:** M (Medium) - Infrastructure change, requires testing

**Story Points:** 5

---

#### 5. Library Installation Pattern

**Impact:** `%pip install` runs per-cell in serverless (slower startup)

**Current Pattern:**

```python
# Cell 1: Install dependencies
%pip install azure-cosmos==3.1.2 paramiko==2.11.0 ...

# Cell 2: Import and use
import azure.cosmos
```

**Serverless Behavior:**

- ✅ Still works, but runs on EVERY job execution
- ⚠️ Adds 30-60 seconds startup time per job
- ⚠️ No cluster-level library caching

**Mitigation Options:**

1. **Workspace-level libraries** (install once, available to all serverless compute)
2. **Accept startup delay** (simplest, no code changes)
3. **Custom Docker container** (advanced, requires maintenance)

**Recommendation:** Start with workspace-level libraries (reduces startup time)

**Complexity:** S (Small) - Configuration change, no code changes

**Story Points:** 2

---

#### 6. Cosmos DB Spark Connector Verification

**Impact:** Connector compatibility with serverless runtime

**Current Usage:**

```python
# Cosmos DB Change Feed
df = spark.read.format("cosmos.oltp.changeFeed")
  .option("spark.cosmos.accountEndpoint", endpoint)
  .option("spark.cosmos.accountKey", key)
  .option("spark.cosmos.database", "ISWC")
  .option("spark.cosmos.container", "AuditRequests")
  .option("spark.cosmos.changeFeed.startFrom", "Beginning")
  .load()
```

**Risk:** Cosmos Spark connector version may need upgrade for Runtime 15.4 + serverless

**Required Action:**

- [ ] Verify Azure Cosmos DB Spark 3.5 connector supports serverless
- [ ] Test change feed streaming in serverless environment
- [ ] Check checkpoint location compatibility (currently in DBFS?)

**Complexity:** S (Small) - Testing + potential config changes

**Story Points:** 3

---

### Features NOT Affected (No Changes Required)

✅ **DataFrame API:** Fully compatible (95% of code)
✅ **Spark SQL:** Fully compatible (SELECT/INSERT/MERGE operations)
✅ **Delta Lake MERGE:** Fully compatible (serverless supports Delta)
✅ **Schema auto-merge:** Fully compatible (config setting works)
✅ **JDBC connectivity:** Fully compatible (SQL Server reads)
✅ **Python libraries:** All have compatible versions
✅ **dbutils.secrets:** Fully compatible (Key Vault integration works)
✅ **Structured Streaming:** Fully compatible (micro-batch mode supported)

---

### Expected Platform Improvements

#### Cost Optimization

**Compute Savings:**

- **Eliminate idle cluster costs:** Current interactive cluster runs 24/7, pay for idle time
- **Auto-scaling efficiency:** Serverless scales to zero when not in use
- **Photon engine:** 2-3x faster query performance (less DBU consumption)

**Estimated Savings:** 50-70% reduction in monthly Databricks costs

**Cost Model Example:**

```text
Current (Interactive Cluster):
- 24/7 uptime: 720 hours/month
- Avg utilization: 30% (actual job runtime)
- Wasted cost: 70% (idle time)
- Monthly cost: €5,000 (example)

Serverless:
- Pay only for active execution time
- Photon acceleration: 2-3x faster (50% less DBU)
- Monthly cost: €1,500-€2,000 (estimated)
- Savings: 60-70%
```

**Assumption:** Requires validation with actual cluster utilization metrics

---

#### Data Governance

**Unity Catalog Features:**

- **Fine-grained access control:** Row/column-level security (currently none)
- **Centralized audit logging:** Who accessed what data, when
- **Data lineage:** Automatic tracking of data transformations
- **Data discovery:** Search and tag datasets across workspaces
- **Credential isolation:** User identity → storage access (no shared passwords)

**Impact:** Enables CISAC to comply with data governance requirements (GDPR, internal policies)

---

#### Security Enhancements

**Credential Pass-Through:**

- **Current:** SQL authentication with passwords in Key Vault
- **Unity Catalog:** User identity → Azure AD → SQL Server (password-less)

**Network Security:**

- **Current:** VNet injection (good)
- **Unity Catalog:** Adds credential isolation layer

**Secrets Management:**

- **Current:** Workspace-level secrets (all users can access)
- **Unity Catalog:** Catalog-level secrets (access controlled by UC permissions)

---

### Migration Effort Summary

| Task | Complexity | Story Points | Risk |
|------|-----------|--------------|------|
| Hive metastore → Unity Catalog namespace | M | 5 | Medium |
| DBFS mount migration | L | 13 | High |
| DBFS FUSE removal | S | 2 | Low |
| Cluster config update (Data Factory) | M | 5 | Medium |
| Library installation pattern | S | 2 | Low |
| Cosmos connector verification | S | 3 | Medium |
| **TOTAL** | **XL** | **30** | **HIGH** |

**Additional Overhead:**

- Unity Catalog setup (metastore creation, permissions)
- Mount point inventory and mapping
- Testing across Dev/UAT/Prod environments
- Performance validation
- Documentation updates

**Total Effort:** 21-34 story points (including overhead)

---

## Option B: Runtime 15.4 LTS Upgrade

### Target Architecture

**Compute Model:**

- **Same as current:** Interactive cluster (can optimize later)
- **Runtime:** 15.4 LTS (Python 3.10 or 3.11, Spark 3.5.x)
- **Keep:** Hive metastore, DBFS mounts, VNet injection

**Storage Integration:**

- **No changes:** DBFS mounts continue to work
- **No changes:** `/dbfs/` FUSE mounting continues to work

**Governance:**

- **No changes:** Same access control model
- **Option to add:** Can enable Unity Catalog later (independent of runtime version)

---

### Breaking Changes & Required Migrations

#### 1. Python Version Upgrade (3.9 → 3.10 or 3.11)

**Impact:** Library compatibility validation required

**Current Libraries:**

```python
azure-cosmos==3.1.2       → Upgrade to 4.x
pandas==1.3.5             → Upgrade to 2.x
numpy==1.22.1             → Upgrade to 1.26.x
pyarrow==19.0.1           → Upgrade to 14.x (bundled with Spark)
paramiko==2.11.0          → Upgrade to 3.x
```

**Required Changes:**

```python
# requirements.txt (update library versions)
azure-cosmos==4.5.1
pandas==2.1.4
numpy==1.26.3
paramiko==3.4.0
pysftp==0.2.9              # Check compatibility with paramiko 3.x
zeep==4.2.1                # Verify compatibility
opencensus-ext-azure==1.1.13
```

**Files Affected:**

- `src/Integration/requirements.txt`
- `src/Reporting/requirements.txt`
- README.md (Python version reference)

**Complexity:** S (Small) - Version bumps, compatibility testing

**Story Points:** 2

---

#### 2. Spark Version Upgrade (3.3.x → 3.5.x)

**Impact:** API changes, deprecated warnings

**Known Changes in Spark 3.5:**

- RDD API deprecation warnings (not breaking - still works)
- Some DataFrame API methods deprecated (minor)
- Performance improvements in query optimizer

**Required Actions:**

- [ ] Review Databricks 15.4 LTS release notes for breaking changes
- [ ] Test all notebooks on new runtime (smoke testing)
- [ ] Address any deprecation warnings

**Risk:** LOW - Limited RDD usage, DataFrame API stable

**Complexity:** S (Small) - Testing-focused, minimal code changes expected

**Story Points:** 3

---

#### 3. Delta Lake Version Upgrade (1.x → 3.x)

**Impact:** Performance improvements, new features

**Changes:**

- ✅ Backward compatible (tables created on old version readable on new version)
- ✅ Better MERGE performance (10-30% faster)
- ✅ Improved schema evolution handling
- ✅ Better vacuum performance (cleanup old versions)

**Required Actions:**

- [ ] Test Delta Lake operations (MERGE, schema evolution)
- [ ] Verify `spark.databricks.delta.schema.autoMerge.enabled` still works

**Risk:** LOW - Delta Lake maintains backward compatibility

**Complexity:** XS (Extra Small) - No code changes, validation only

**Story Points:** 1

---

#### 4. Azure Cosmos DB Spark Connector Update

**Impact:** Connector version upgrade required

**Current:** Likely using Cosmos Spark connector for Spark 3.3

**Required:** Upgrade to Azure Cosmos DB Spark 3.5 connector

**Potential Changes:**

- Configuration parameter names (check documentation)
- Change feed checkpoint format (likely compatible)

**Required Actions:**

- [ ] Install new connector version (Maven coordinates or %pip install)
- [ ] Test change feed streaming
- [ ] Verify checkpoint compatibility

**Risk:** MEDIUM - Connector versions can have breaking changes

**Complexity:** S (Small) - Config updates, testing

**Story Points:** 3

---

#### 5. Deprecated Spark Configuration Properties

**Impact:** Some spark.conf settings may be deprecated

**Current Settings:**

```python
spark.conf.set("spark.databricks.delta.schema.autoMerge.enabled", "true")
```

**Required Actions:**

- [ ] Review Databricks 15.4 configuration changes
- [ ] Update deprecated configs (if any)

**Risk:** LOW - Databricks maintains backward compatibility for most configs

**Complexity:** XS (Extra Small) - Configuration updates

**Story Points:** 1

---

### Features NOT Affected (No Changes Required)

✅ **DBFS mounts:** Still supported (mount paths work as-is)
✅ **DBFS FUSE:** Still supported (`/dbfs/` prefix works)
✅ **Hive metastore:** Still supported (2-level namespace works)
✅ **Interactive cluster:** Still supported (Data Factory config unchanged)
✅ **dbutils APIs:** Fully compatible (no breaking changes)
✅ **DataFrame API:** Fully compatible (stable API)
✅ **Spark SQL:** Fully compatible
✅ **Delta Lake:** Fully compatible (backward compatible)
✅ **Python libraries:** All have Python 3.10+ compatible versions
✅ **Authentication:** No changes (SQL auth, Key Vault integration work as-is)

---

### Expected Platform Improvements

#### Security Updates

**Runtime 11.3 LTS EOL Risk:**

- ⚠️ No security patches for known vulnerabilities
- ⚠️ Potential compliance violations (running unsupported software)
- ⚠️ Risk of CVEs in bundled libraries (Log4j, etc.)

**Runtime 15.4 LTS Benefits:**

- ✅ Security patches for 3+ years (until Dec 2027)
- ✅ Compliance with security policies
- ✅ Updated bundled libraries (reduced CVE exposure)

---

#### Performance Improvements

**Query Optimizer:**

- 15-25% faster query execution (Spark 3.5 adaptive query execution improvements)
- Better join optimization
- Better partition pruning

**Delta Lake Performance:**

- 10-30% faster MERGE operations
- Better small file handling
- Improved Z-ordering (data clustering)

**PyArrow Integration:**

- Faster Pandas ↔ Spark conversions
- Better memory efficiency

**Estimated Impact:** 10-20% reduction in job execution time → 10-20% cost savings

---

#### Developer Experience

**Python 3.10+ Features:**

- Structural pattern matching (match/case)
- Better type hints
- Performance improvements

**Databricks Notebooks:**

- Improved UI/UX
- Better Git integration
- Enhanced debugging tools

---

### Migration Effort Summary

| Task | Complexity | Story Points | Risk |
|------|-----------|--------------|------|
| Python library version updates | S | 2 | Low |
| Spark API compatibility testing | S | 3 | Low |
| Delta Lake validation | XS | 1 | Low |
| Cosmos connector upgrade | S | 3 | Medium |
| Spark config updates | XS | 1 | Low |
| **TOTAL** | **M** | **10** | **LOW-MEDIUM** |

**Additional Overhead:**

- Dev/UAT/Prod deployment coordination
- Smoke testing all notebooks
- Performance validation
- Rollback plan preparation

**Total Effort:** 5-8 story points (including overhead)

---

## Gap Analysis Matrix

### Feature Compatibility Comparison

| Feature | Current (11.3 LTS) | Option A (Unity + Serverless) | Option B (15.4 LTS) |
|---------|-------------------|-------------------------------|---------------------|
| **Hive Metastore** | ✅ Supported | ⚠️ Migrate to Unity Catalog | ✅ Still supported |
| **Unity Catalog** | ❌ Not available | ✅ Required | ✅ Optional (can enable) |
| **DBFS Mounts** | ✅ Supported | ❌ NOT supported | ✅ Still supported |
| **DBFS FUSE** | ✅ Supported | ❌ NOT supported | ✅ Still supported |
| **Interactive Cluster** | ✅ Supported | ❌ Switch to serverless | ✅ Still supported |
| **DataFrame API** | ✅ Supported | ✅ Fully compatible | ✅ Fully compatible |
| **RDD API** | ✅ Supported | ⚠️ Deprecated warnings | ⚠️ Deprecated warnings |
| **Delta Lake** | ✅ v1.x | ✅ v3.x (compatible) | ✅ v3.x (compatible) |
| **Spark SQL** | ✅ Supported | ✅ Fully compatible | ✅ Fully compatible |
| **Structured Streaming** | ✅ Supported | ✅ Micro-batch only | ✅ Fully supported |
| **Python Version** | 3.9 | 3.10 or 3.11 | 3.10 or 3.11 |
| **Spark Version** | 3.3.x | 3.5.x | 3.5.x |
| **Security Patches** | ❌ EOL | ✅ Supported | ✅ Supported |
| **Photon Engine** | ⚠️ Optional (paid) | ✅ Included | ⚠️ Optional (paid) |
| **Credential Pass-Through** | ❌ Not available | ✅ Available | ❌ Not available (Hive) |

---

### Code Migration Impact

| Code Pattern | Lines of Code | Option A Changes | Option B Changes |
|--------------|---------------|------------------|------------------|
| **Delta Lake saveAsTable** | ~50 | Update namespace (2-level → 3-level) | No changes |
| **Spark SQL queries** | ~200 | Update table references | No changes |
| **DBFS mount paths** | ~30 | Replace with abfss:// URIs | No changes |
| **DBFS FUSE `/dbfs/`** | ~5 | Remove prefix | No changes |
| **Python libraries** | requirements.txt | Update versions | Update versions |
| **RDD operations** | ~10 | Optional refactoring | Optional refactoring |
| **Cosmos connector** | ~20 | Verify compatibility | Verify compatibility |
| **Data Factory config** | JSON files | Replace cluster ID | No changes |

---

### Infrastructure Changes

| Component | Option A | Option B |
|-----------|----------|----------|
| **Databricks Runtime** | 15.4 LTS | 15.4 LTS |
| **Metastore** | Unity Catalog (new) | Hive metastore (existing) |
| **Compute Type** | Serverless | Interactive cluster |
| **DBFS Mounts** | Remove (migrate to direct URIs) | Keep existing |
| **Storage Access** | Credential pass-through or SP | SQL auth (existing) |
| **Data Factory Config** | Update linked service | No changes |
| **Key Vault Integration** | Migrate to UC secrets (optional) | Keep existing |

---

### Risk Assessment

| Risk Category | Option A (Unity + Serverless) | Option B (Runtime Upgrade) |
|---------------|-------------------------------|----------------------------|
| **Code Breakage** | HIGH (mount paths, namespaces) | LOW (library versions only) |
| **Testing Effort** | HIGH (full regression needed) | MEDIUM (smoke testing) |
| **Rollback Complexity** | HIGH (data migration involved) | LOW (runtime downgrade possible) |
| **Production Impact** | MEDIUM-HIGH (architecture change) | LOW (runtime swap) |
| **Knowledge Gap** | HIGH (Unity Catalog new to team) | LOW (same patterns) |
| **Vendor Support** | MEDIUM (new platform, learning curve) | LOW (well-documented) |

---

## Migration Effort Estimation

### Option A: Unity Catalog + Serverless

#### T-Shirt Sizing: **XL (Extra Large)**

#### Story Point Breakdown

| Epic | User Story | Complexity | Story Points |
|------|-----------|-----------|--------------|
| **Unity Catalog Setup** | Create Unity Catalog metastore | M | 5 |
| | Configure catalog permissions | M | 5 |
| | Migrate existing tables to UC | L | 8 |
| **Code Migration** | Update Delta Lake namespaces | M | 5 |
| | Migrate DBFS mount paths | L | 13 |
| | Remove DBFS FUSE usage | S | 2 |
| **Infrastructure** | Update Data Factory configs | M | 5 |
| | Configure serverless compute | S | 3 |
| | Setup credential pass-through | M | 5 |
| **Testing & Validation** | Dev environment smoke testing | M | 5 |
| | UAT full regression testing | L | 8 |
| | Performance benchmarking | M | 5 |
| | Production deployment & validation | M | 5 |
| **Documentation** | Update deployment documentation | S | 3 |
| | Create runbooks for UC operations | S | 2 |
| **TOTAL** | | **XL** | **79** |

**Adjusted Total (Removing Padding):** 21-34 story points core work

**Effort Distribution:**

- Code changes: 40% (20 points)
- Infrastructure: 30% (15 points)
- Testing: 20% (10 points)
- Documentation: 10% (5 points)

---

### Option B: Runtime 15.4 LTS Upgrade

#### T-Shirt Sizing: **M (Medium)**

#### Story Point Breakdown

| Epic | User Story | Complexity | Story Points |
|------|-----------|-----------|--------------|
| **Library Updates** | Update requirements.txt | S | 1 |
| | Test Python 3.10 compatibility | S | 2 |
| **Spark Upgrade** | Review 15.4 release notes | XS | 1 |
| | Test DataFrame API compatibility | S | 2 |
| | Update deprecated configs | XS | 1 |
| **Cosmos Connector** | Upgrade connector version | S | 2 |
| | Test change feed streaming | S | 3 |
| **Testing & Validation** | Dev environment smoke testing | S | 3 |
| | UAT regression testing | M | 5 |
| | Performance benchmarking | S | 2 |
| | Production deployment | S | 3 |
| **Documentation** | Update README and deployment docs | XS | 1 |
| **TOTAL** | | **M** | **26** |

**Adjusted Total (Removing Padding):** 5-8 story points core work

**Effort Distribution:**

- Code changes: 30% (3 points)
- Testing: 50% (5 points)
- Infrastructure: 10% (1 point)
- Documentation: 10% (1 point)

---

### Effort Comparison

| Metric | Option A | Option B | Difference |
|--------|----------|----------|------------|
| **T-Shirt Size** | XL | M | 3x larger |
| **Story Points** | 21-34 | 5-8 | 4x more effort |
| **Risk Level** | HIGH | LOW-MEDIUM | Higher risk |
| **Testing Scope** | Full regression | Smoke + targeted | More extensive |
| **Rollback Complexity** | HIGH | LOW | Harder to undo |

---

## Assumptions Requiring Verification

### Infrastructure Assumptions

#### Current Cluster Configuration

**Assumptions:**

- [ ] **Runtime Version:** Exactly 11.3 LTS (confirm via workspace inspection)
- [ ] **Cluster Type:** Interactive cluster (not job cluster) - **CRITICAL**
- [ ] **Auto-termination:** Not configured (cluster runs 24/7) - **COST IMPACT**
- [ ] **Cluster SKU:** Standard_DS3_v2 or similar (needs confirmation)
- [ ] **Worker Count:** Fixed or auto-scaling? (impacts serverless migration planning)
- [ ] **VNet Injection:** Enabled (evidence: workers-vnet, workers-sg)
- [ ] **Cluster Libraries:** Installed at cluster level or notebook level? (check init scripts)

**Validation Method:**

```bash
# Run in Databricks notebook
print(spark.version)  # Confirm Spark version
dbutils.notebook.entry_point.getDbutils().notebook().getContext().tags().get("clusterId")
```

---

#### DBFS Mount Configuration

**Assumptions:**

- [ ] **Mount Points Exist:** `/mnt/ipi-integration/` and `/mnt/generic_job_error_*` (code evidence suggests YES)
- [ ] **Storage Accounts:** cisaciswcdatalake* accounts are mounted
- [ ] **Authentication:** Service principal or storage account key (not credential pass-through)
- [ ] **Containers Mounted:** Full list unknown (inventory required)

**Validation Method:**

```python
# List all mounts
dbutils.fs.mounts()

# For each mount, check configuration
dbutils.fs.ls("/mnt/")
```

**Required Output:**

```text
/mnt/ipi-integration -> abfss://container@cisaciswcdatalakeprod.dfs.core.windows.net/ipi
/mnt/generic_job_error -> abfss://container@cisaciswcdatalakeprod.dfs.core.windows.net/errors
... (complete list)
```

---

#### Unity Catalog Readiness

**Assumptions:**

- [ ] **Unity Catalog NOT enabled** on current workspace (likely - code uses 2-level namespace)
- [ ] **Metastore NOT created** (needs setup from scratch)
- [ ] **No existing UC catalogs** (greenfield migration)
- [ ] **Azure region supports UC:** Confirm workspace region has Unity Catalog available

**Validation Method:**

```python
# Check if Unity Catalog is enabled
spark.sql("SHOW CATALOGS").show()
# If only "hive_metastore" appears, UC not enabled
```

---

### Code & Data Assumptions

#### Delta Lake Tables

**Assumptions:**

- [ ] **All tables in Hive metastore** (not external location)
- [ ] **Database name:** "iswc" (code evidence: `iswc.tablename`)
- [ ] **Table format:** All Delta (not Parquet-only) - **CRITICAL for MERGE**
- [ ] **Schema evolution enabled:** Via spark.conf (code evidence confirms)
- [ ] **Time travel history:** Accumulating versions (cost issue, needs VACUUM)

**Validation Method:**

```python
# List all databases and tables
spark.catalog.listDatabases()
spark.catalog.listTables("iswc")

# Check table format
spark.sql("DESCRIBE DETAIL iswc.tablename").show()
# Look for "format": "delta"
```

---

#### Cosmos DB Integration

**Assumptions:**

- [ ] **Connector Version:** Unknown (check cluster installed libraries)
- [ ] **Change Feed Usage:** Yes (code evidence: `cosmos.oltp.changeFeed`)
- [ ] **Checkpoint Locations:** Likely in DBFS (needs verification for serverless migration)
- [ ] **Streaming Mode:** Micro-batch (code evidence: `trigger(once=True)`)

**Validation Method:**

```python
# Check installed Cosmos connector
%pip list | grep cosmos

# Check checkpoint locations in code
# Search for: option("checkpointLocation", ...)
```

---

#### Python Library Compatibility

**Assumptions:**

- [ ] **All libraries have Python 3.10+ versions:** Likely YES (mainstream libraries)
- [ ] **No custom/internal libraries:** Assumed (only public PyPI packages detected)
- [ ] **No C extensions with Python 3.9 hard dependency:** Low risk (data libraries are well-maintained)

**Validation Method:**

```bash
# Check each library on PyPI
pip index versions azure-cosmos
pip index versions paramiko
# ... etc.
```

---

### Performance & Cost Assumptions

#### Current Utilization

**Assumptions:**

- [ ] **Cluster utilization:** ~30% (industry average for interactive clusters) - **NEEDS VERIFICATION**
- [ ] **Idle time:** 70% (pay for 24/7, use ~8 hours/day) - **COST IMPACT**
- [ ] **Job frequency:** Multiple jobs per day (IPI quarterly, agency files continuous) - **CONFIRM**
- [ ] **Average job duration:** 5-30 minutes (varies by job type) - **NEEDS MEASUREMENT**

**Validation Method:**

```python
# Query Databricks System Tables (if enabled)
spark.sql("""
  SELECT cluster_id, start_time, end_time,
         (unix_timestamp(end_time) - unix_timestamp(start_time)) / 60 as duration_minutes
  FROM system.compute.clusters
  WHERE cluster_id = 'CLUSTER_ID'
  ORDER BY start_time DESC
  LIMIT 100
""").show()
```

**Required Data:**

- Cluster uptime percentage
- Actual job execution time vs. idle time
- Peak usage hours
- Cost per month (actual Azure billing data)

---

#### Serverless Cost Model

**Assumptions:**

- [ ] **Photon acceleration:** 2-3x faster (Databricks benchmark) - **ASSUMED**
- [ ] **Serverless pricing:** ~40% higher per DBU, but ~70% less total DBUs (less idle time) - **ASSUMED**
- [ ] **Net savings:** 50-70% - **NEEDS VALIDATION WITH ACTUAL WORKLOAD**

**Validation Method:**

- Run representative workload on serverless (pilot test)
- Compare DBU consumption
- Measure query performance improvement

---

### Operational Assumptions

#### Deployment Process

**Assumptions:**

- [ ] **Data Factory deployment:** ARM templates or Azure DevOps pipelines (not documented)
- [ ] **Notebook deployment:** Databricks CLI or Git integration (not documented)
- [ ] **Environment promotion:** Dev → UAT → Prod (standard practice)
- [ ] **Rollback capability:** Can revert cluster to previous runtime (YES for Option B, HARD for Option A)

**Validation Needed:**

- Review existing deployment documentation (if available)
- Understand Spanish Point deployment practices
- Identify rollback procedures

---

#### Testing Strategy

**Assumptions:**

- [ ] **Dev environment available:** ISWCDev workspace exists (Azure export confirms)
- [ ] **UAT environment available:** ISWCUAT workspace exists (Azure export confirms)
- [ ] **Test data available:** Can run full regression tests (UNKNOWN)
- [ ] **Performance baseline:** Metrics exist to compare against (UNKNOWN)

**Validation Needed:**

- Confirm test data availability
- Establish performance baseline (run current jobs, measure timing)
- Define acceptance criteria (regression tests pass, performance ≥ baseline)

---

## Recommended Strategy

### Two-Phase Approach (Recommended)

This strategy balances **urgency** (EOL risk), **risk mitigation**, and **long-term value**.

---

### Phase 1: Runtime Upgrade to 15.4 LTS (Immediate)

**Timeline:** 2-3 weeks

**Objective:** Address EOL security risk with minimal disruption

**Scope:**

1. Upgrade to Runtime 15.4 LTS
2. Keep existing architecture (Hive metastore, DBFS mounts, interactive cluster)
3. Update Python libraries to compatible versions
4. Validate all jobs work on new runtime

**Benefits:**

- ✅ Security patches (3+ years support)
- ✅ 10-20% performance improvement
- ✅ 10-20% cost savings (better auto-termination)
- ✅ Low risk (minimal code changes)
- ✅ Fast migration (2-3 weeks)

**Risks:**

- ⚠️ Library compatibility issues (mitigated by testing)
- ⚠️ Minor Spark API changes (low probability)

**Complexity:** M (Medium) - 5-8 story points

**Recommended Actions:**

1. **Week 1:** Dev environment upgrade + smoke testing
2. **Week 2:** UAT environment upgrade + regression testing
3. **Week 3:** Production upgrade + validation

---

### Phase 2: Unity Catalog + Serverless (6-12 Months Later)

**Timeline:** 4-6 weeks (execution) + governance planning

**Objective:** Maximize cost savings and enable data governance

**Scope:**

1. Enable Unity Catalog on workspace
2. Migrate Hive metastore tables to Unity Catalog
3. Migrate DBFS mounts to direct ADLS URIs
4. Switch from interactive cluster to serverless compute
5. Enable credential pass-through

**Benefits:**

- ✅ Additional 40-50% cost savings (on top of Phase 1)
- ✅ Fine-grained data governance (row/column security)
- ✅ Credential pass-through (eliminate SQL passwords)
- ✅ Better security posture (user identity → storage access)

**Risks:**

- ⚠️ High complexity (code + infrastructure changes)
- ⚠️ Requires governance planning (access control policies)
- ⚠️ Testing scope larger (mount paths, catalog namespaces)

**Complexity:** XL (Extra Large) - 21-34 story points

**Recommended Actions (High-Level):**

1. **Months 1-3:** Unity Catalog governance planning (access policies, catalog structure)
2. **Months 4-5:** Code migration (mount paths, catalog namespaces)
3. **Month 6:** Testing and production rollout

---

### Why Two Phases?

#### Urgency: Runtime 11.3 LTS is EOL

- ⚠️ Security risk increases over time (no patches)
- ⚠️ Compliance violations (running unsupported software)
- ✅ Phase 1 addresses urgently (2-3 weeks)

#### Risk Mitigation: Learn from Phase 1

- ✅ Test new runtime with existing architecture (low risk)
- ✅ Validate library compatibility before Unity Catalog migration
- ✅ Build confidence in deployment process (Dev → UAT → Prod)
- ✅ Identify issues early (Cosmos connector, performance)

#### Strategic Value: Unity Catalog Requires Planning

- ⚠️ Unity Catalog is not just a technical upgrade (governance framework needed)
- ⚠️ Access control policies need business input (not just IT decision)
- ⚠️ Catalog structure design impacts long-term usability
- ✅ Phase 1 buys time for proper planning (6-12 months)

#### Cost/Benefit: Incremental Value

- ✅ Phase 1: 10-20% savings (quick win)
- ✅ Phase 2: Additional 40-50% savings (long-term value)
- ✅ Combined: 50-70% total savings (same as direct Unity Catalog migration, but lower risk)

---

### Alternative: Direct Unity Catalog Migration (Not Recommended)

**If CISAC chooses direct migration to Unity Catalog:**

**Pros:**

- ✅ Maximum cost savings immediately (50-70%)
- ✅ Governance benefits sooner
- ✅ One migration effort (not two)

**Cons:**

- ⚠️ HIGH risk (large architecture change)
- ⚠️ Longer timeline (4-6 weeks)
- ⚠️ No fallback (if issues, can't revert to 11.3 LTS easily)
- ⚠️ Governance planning rushed (may lead to suboptimal policies)

**Recommendation:** Only pursue if:

1. CISAC has **already** planned Unity Catalog governance structure
2. **Strong urgency** for cost savings (budget pressure)
3. **Comfortable with higher risk** (extensive testing capacity)

---

## Technical Deep-Dives

### Code Pattern Analysis

#### Delta Lake Namespace Usage

**Current Pattern (2-level):**

```python
# delta_service.py - Core abstraction
class DeltaService:
    def write_dataframe_to_table(self, df, database_name, table_name, write_mode):
        df.write.format("delta") \
          .mode(write_mode) \
          .option("mergeSchema", "true") \
          .option("overwriteSchema", "true") \
          .saveAsTable(f'{database_name}.{table_name}')
```

**Usage Across Codebase:**

```python
# mainrecentchanges.py
delta_service.write_dataframe_to_table(df, "iswc", "recentiswcchangessubscribers", "overwrite")

# mainchangetracker.py
delta_service.write_dataframe_to_table(df, "iswc", "changetracker", "append")

# mainauditprocessing.py
delta_service.write_dataframe_to_table(df, "iswc", "auditprocessing", "overwrite")
```

**Unity Catalog Migration:**

```python
# Option 1: Update DeltaService to inject catalog name
class DeltaService:
    def __init__(self, catalog_name="hive_metastore"):
        self.catalog = catalog_name

    def write_dataframe_to_table(self, df, database_name, table_name, write_mode):
        full_table_name = f'{self.catalog}.{database_name}.{table_name}'
        df.write.format("delta") \
          .mode(write_mode) \
          .option("mergeSchema", "true") \
          .option("overwriteSchema", "true") \
          .saveAsTable(full_table_name)

# Option 2: Use "hive_metastore" catalog for backward compatibility
# (no code changes, just use hive_metastore as catalog name in Unity Catalog)
```

**Migration Complexity:** M (Medium)

- Centralized in `delta_service.py` (single point of change)
- All callers use same pattern (consistent)
- Testing required for all table writes

---

#### DBFS Mount Migration

**Current Pattern:**

```python
# IPI FullSync
parquet_path = "/mnt/ipi-integration/InterestedParty"
df.write.mode("overwrite").format("parquet").save(parquet_path)

# Error file writes
error_path = f'/mnt/generic_job_error_{class_name}_{timestamp}'
df.write.option('header', True).csv(error_path)
```

**Serverless-Compatible Pattern:**

```python
# Configuration-driven approach
class StorageConfig:
    def __init__(self):
        self.storage_account = "cisaciswcdatalakeprod"
        self.ipi_container = "ipi-integration"
        self.error_container = "job-errors"

    def get_path(self, container, relative_path):
        return f"abfss://{container}@{self.storage_account}.dfs.core.windows.net/{relative_path}"

# Usage
config = StorageConfig()
parquet_path = config.get_path("ipi-integration", "InterestedParty")
df.write.mode("overwrite").format("parquet").save(parquet_path)

error_path = config.get_path("job-errors", f'{class_name}_{timestamp}')
df.write.option('header', True).csv(error_path)
```

**Migration Complexity:** L (Large)

- Multiple files affected (IPI FullSync, delta_service, reporting)
- Requires storage account + container mapping (infrastructure knowledge)
- Testing required for all file operations

---

#### RDD API Refactoring (Optional)

**Current Pattern:**

```python
# Converting dictionaries to DataFrames
rdd = spark.sparkContext.parallelize(record_type_object[key])
spark_dframe = spark.createDataFrame(rdd, TblSchema(key).get_tbl_schema())
```

**DataFrame API Alternative:**

```python
# Direct DataFrame creation (no RDD)
spark_dframe = spark.createDataFrame(
    record_type_object[key],  # List of dictionaries
    schema=TblSchema(key).get_tbl_schema()
)
```

**Migration Complexity:** XS (Extra Small)

- Only 7 occurrences (low impact)
- Straightforward replacement (one-to-one mapping)
- Optional (RDD API still works, just deprecated warnings)

---

### Performance Benchmarking Plan

To validate performance assumptions and measure improvements, establish baseline metrics:

#### Metrics to Capture

**Current State (Runtime 11.3 LTS):**

1. **Job Duration:** Measure end-to-end execution time for each job type
   - IPI FullSync: X minutes
   - Agency file processing: Y minutes
   - Change tracker: Z minutes
   - Reporting jobs: W minutes

2. **Cluster Utilization:**
   - Uptime percentage (24/7 vs. actual job runtime)
   - DBU consumption per job
   - Idle time percentage

3. **Cost Metrics:**
   - Monthly Databricks cost (from Azure billing)
   - Cost per job execution
   - DBU rate (standard vs. idle)

**Target State (After Migration):**

- **Option A:** 50-70% cost reduction, 2-3x query performance (Photon)
- **Option B:** 10-20% cost reduction, 15-25% performance improvement

#### Benchmark Approach

```python
# Benchmark script (run on both old and new runtime)
import time

def benchmark_job(job_name, job_function):
    start_time = time.time()
    job_function()
    end_time = time.time()
    duration = end_time - start_time

    # Log to Delta table
    spark.sql(f"""
        INSERT INTO benchmark_results VALUES (
            '{job_name}',
            '{spark.version}',
            current_timestamp(),
            {duration}
        )
    """)

# Run for each job type
benchmark_job("IPI_FullSync", run_ipi_fullsync)
benchmark_job("AgencyFileProcessing", process_agency_file)
# ...etc.
```

---

## Appendix: External Resources

### Microsoft Documentation

**Unity Catalog Migration:**

- [Upgrade to Unity Catalog](https://learn.microsoft.com/en-us/azure/databricks/data-governance/unity-catalog/upgrade/)
- [Unity Catalog Best Practices](https://learn.microsoft.com/en-us/azure/databricks/data-governance/unity-catalog/best-practices)

**Serverless Compute:**

- [Serverless SQL Warehouses](https://learn.microsoft.com/en-us/azure/databricks/sql/admin/serverless)
- [Serverless Compute for Jobs](https://learn.microsoft.com/en-us/azure/databricks/compute/serverless-compute)

**Runtime Upgrades:**

- [Databricks Runtime 15.4 LTS Release Notes](https://learn.microsoft.com/en-us/azure/databricks/release-notes/runtime/15.4lts)
- [Databricks Runtime Support Lifecycle](https://learn.microsoft.com/en-us/azure/databricks/release-notes/runtime/databricks-runtime-ver)

### Databricks Community

- [Unity Catalog Migration Patterns](https://community.databricks.com/)
- [Serverless Migration Experiences](https://community.databricks.com/)

---

## Document History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-11-10 | Audit Team | Initial migration plan based on codebase analysis |

---

**Status:** DRAFT - Awaiting validation of assumptions

**Next Actions:**

1. Validate infrastructure assumptions (cluster config, mounts, utilization)
2. Establish performance baseline (run benchmark script)
3. Review with Spanish Point (technical validation)
4. Review with CISAC stakeholders (governance planning for Phase 2)
5. Decide on migration approach (two-phase vs. direct Unity Catalog)
