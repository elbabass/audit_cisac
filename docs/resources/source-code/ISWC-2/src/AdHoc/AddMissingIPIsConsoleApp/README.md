# AddMissingIPIsConsoleApp

This console application is designed to identify and add missing IPIs to the ISWC database by coordinating data from the SUISA SFTP and executing Missing IPI Sync Databricks job.

## Method 1:  Run the Missing IPI Sync Databricks job first on Databricks then run the ConsoleApp

### Step 1 – Retrieve Latest IPI Data from SUISA FTP

1. Access the SUISA SFTP server:
   - **Host**: `ftp.suisa.ch`
   - **Port**: `22`
   - **Username**: `cisac-sp`
   - **Password**: Stored in Key Vault under `AzureKeyVaultSecret-DataFactory-Secret-SuisaIpiPassword`

2. Navigate to the latest folder under `/IPI/IPI_Pub/Complete_IPI/`.
   - Example folder: `/IPI/IPI_Pub/Complete_IPI/20250301`
   - Note the folder name (e.g., `20250301`) — this will be used in the Databricks job parameter and appsettings.json.

---

### Step 2 – Run Missing IPI Sync Job in Databricks

1. Access the Databricks workspace via browser.
2. Trigger the Databricks job responsible for syncing missing IPI data:
   - Navigate to **Workflows** > **Missing IPI Sync**
   - Run the job with the parameter:
     - `folder_name = "<SUISA_FOLDER_NAME>"` (e.g., `20250301`)
3. Wait for the job to complete and confirm how many records were processed. This job populates the `ipi.missinginterestedparty` table.

---
### Step 3 – Agency API Configuration 

1. Agency API must be running locally in the target environment.
-  Ensure `suisaipiclienturl`, `suisaipiuserid`, and `suisaipipassword` secrets are configured
2. Make sure the Agency API is fully operational before proceeding to the next step.

2. Get the local URL generated and update it in the `appsettings.json` for the MissingIpisConsoleApp
  

---

### Step 4 – Configure and Run This Console Application

1. Open `appsettings.json` and update the following fields:

```json
`"IswcApi-BaseAddress": URL of the running Agency API
"DatabricksClusterId": "<Cluster ID>",
"DatabricksToken": "<Personal Access Token>",
"DatabricksJobId": "<Job ID for Missing IPI Sync>",
"DatabricksBaseUrl": "https://adb-<cluster>.azuredatabricks.net",
"ApiBatchSize": <Batch Size> *Batch size for API requests. Adjust as needed.*
"RefreshMissingIpiData": false,
"MissingIpiFilePath": "<Do not modify - internal use>",
"SftpFolderName": "<Same SUISA_FOLDER_NAME used in Databricks job>"
```
- **Important**: Set `"RefreshMissingIpiData": false` as the Databricks job from Step 2 has already refreshed the data.
- Ensure `"SftpFolderName"` matches the folder used in Step 2.

2. Run the application:
   - This ingests the generated missing IPI found and the add them on the ISWC Database.

---

### Step 5 – Reindex Azure Search (if needed)

If necessary to refresh search results:

1. Navigate to the Azure Search service for your environment (e.g., `cisacmedev`, `cisacmeuat`, or `cisacmeprod`)
2. Select **Indexers** > `repertoire-contributors-indexer`
3. Click **Reset** and then **Run**

## Method 2:  Run the full Missing IPI Sync locally 

In progress.