workspace "CISAC ISWC System" "C4 Architecture Model for the International Standard Musical Work Code (ISWC) System" {

    model {
        // ==============================================
        // PEOPLE (Actors)
        // ==============================================

        agencyUser = person "Agency User" "Music rights society employee managing work registrations and ISWC assignments" "User"
        publicUser = person "Public User" "General public searching for musical work information" "User"

        // ==============================================
        // EXTERNAL SYSTEMS
        // ==============================================

        fastTrackSSO = softwareSystem "FastTrack SSO" "External OAuth2 authentication provider for Agency Portal single sign-on" "External System" {
            tags "External"
        }

        suisaAPI = softwareSystem "Suisa API" "Swiss music society REST API for work data exchange" "External System" {
            tags "External"
        }

        suisaSFTP = softwareSystem "Suisa SFTP" "Swiss music society SFTP server for file-based work data exchange" "External System" {
            tags "External"
        }

        matchingEngine = softwareSystem "Matching Engine" "Spanish Point external product providing work matching, duplicate detection, and search capabilities using Azure Search" "External System" {
            tags "External"
            description "Deployed in same Azure subscription but separate codebase. Integration via REST API with OAuth2 authentication."

            // OPEN: Access to Matching Engine source code pending per contract terms
            // See: Workshop 2, Line 87 (Yann) - Contract includes access rights
            // See: Workshop 2, Line 249 (Xiyuan) - Currently no access granted
        }

        // ==============================================
        // ISWC PLATFORM (Main System)
        // ==============================================

        iswcPlatform = softwareSystem "ISWC Platform" "Core CISAC platform for International Standard Musical Work Code (ISWC) work registration, management, and global music rights coordination" "Main System" {
            !docs architecture/c4-views

            // ==============================================
            // CONTAINER: WEB APPLICATIONS
            // ==============================================

            agencyPortal = container "Agency Portal" "Single-page React application providing music societies with ISWC work management, registration, and reporting capabilities" "React 16.12, TypeScript 3.7.3, Redux 4.0.4" "Web App" {
                !docs architecture/components/iswc-platform
                tags "Web Application"
                description "Hosted on Azure App Service (ASP.NET Core 3.1 - EOL December 2022). Authenticated via FastTrack SSO (OAuth2)."

                // Components from web-portals.md
                registrationUI = component "Work Registration UI" "React components for creating and updating musical works (CAR, CUR, CDR, MER transactions)" "React Components" "UI Component"
                searchUI = component "Work Search UI" "React components for searching and viewing musical works and submission history" "React Components" "UI Component"
                reportsUI = component "Reports UI" "React components for generating agency statistics and audit reports" "React Components" "UI Component"
                authUI = component "Authentication UI" "React components for user login, session management, and authorization" "React Components" "UI Component"

                stateManagement = component "Redux State Management" "Centralized application state management with actions, reducers, and thunks" "Redux, Redux-Thunk" "State Management"
                apiClient = component "API Client Services" "TypeScript services for HTTP communication with Agency API" "TypeScript, Axios" "HTTP Client"
            }

            publicPortal = container "Public Portal" "Public-facing React application for searching musical works and viewing ISWC information" "React 16.12, TypeScript 3.7.3, ASP.NET Core 3.1" "Web App" {
                tags "Web Application"
                description "Hosted on Azure App Service (ASP.NET Core 3.1 - EOL December 2022). No authentication required for public access. ⚠️ Source code missing from repository."
                // OPEN: Public Portal source code location unknown - not in repository
            }

            // ==============================================
            // CONTAINER: APIs
            // ==============================================

            agencyAPI = container "Agency API" "RESTful API for agency work submissions, queries, and ISWC assignment operations" "ASP.NET Core 3.1, C#" "API" {
                !docs architecture/components/iswc-platform
                tags "API"
                description "Hosted on Azure App Service. ASP.NET Core 3.1 (EOL December 2022). Handles CAR, CUR, CDR, MER, FSQ transactions."

                // Components from agency-api.md
                workController = component "Work Controller" "REST endpoints for work CRUD operations (/api/works, /api/works/{id})" "ASP.NET Core MVC Controller" "Controller"
                submissionController = component "Submission Controller" "REST endpoints for file and API submissions (/api/submissions)" "ASP.NET Core MVC Controller" "Controller"
                searchController = component "Search Controller" "REST endpoints for work and contributor searches (/api/search)" "ASP.NET Core MVC Controller" "Controller"

                pipelineManager = component "Pipeline Manager" "Orchestrates submission processing through validation, matching, and assignment pipelines" "C# Business Logic" "Orchestrator"
                validationPipeline = component "Validation Pipeline" "Executes validation rules (IV, PV, SV) against submissions" "C# Pipeline Components" "Pipeline"
                matchingPipeline = component "Matching Pipeline" "Calls Matching Engine API to find duplicate works" "C# Pipeline Components" "Pipeline"
                processingPipeline = component "Processing Pipeline" "Handles ISWC assignment, work linking, and database persistence" "C# Pipeline Components" "Pipeline"

                auditManager = component "Audit Manager" "Manages audit logging to Cosmos DB for compliance and submission history" "C# Business Logic" "Manager"
            }

            labelAPI = container "Label API" "RESTful API for label/publisher work submissions with IP-removed matching support" "ASP.NET Core 3.1, C#" "API" {
                tags "API"
                description "Hosted on Azure App Service. ASP.NET Core 3.1 (EOL December 2022). Handles label submissions with incomplete contributor data."
                // OPEN: Is Label API deployed separately or combined with other APIs?
            }

            publisherAPI = container "Publisher API" "RESTful API for publisher work submissions and royalty-related operations" "ASP.NET Core 3.1, C#" "API" {
                tags "API"
                description "Hosted on Azure App Service. ASP.NET Core 3.1 (EOL December 2022)."
                // OPEN: Is Publisher API deployed separately or combined with other APIs?
            }

            thirdPartyAPI = container "Third Party API" "RESTful API for external integrations and third-party system access" "ASP.NET Core 3.1, C#" "API" {
                tags "API"
                description "Hosted on Azure App Service. ASP.NET Core 3.1 (EOL December 2022). Public-facing API with API Management gateway."
                // OPEN: Is ThirdParty API deployed separately or combined with other APIs?
            }

            // ==============================================
            // CONTAINER: BACKGROUND PROCESSING
            // ==============================================

            iswcJobs = container "ISWC Jobs" "Azure Functions for scheduled background processing tasks" "Azure Functions v3, C#" "Background Processing" {
                tags "Background Jobs"
                description "Azure Functions v3 (EOL December 2022). 8 functions: IPI sync, audit processing, statistics, CSN notifications, etc."
                // OPEN: Should be upgraded to Azure Functions v4+ (in-process or isolated worker model)
            }

            databricks = container "Databricks" "Azure Databricks cluster for batch file processing, IPI synchronization, and ETL workflows" "Azure Databricks 10.4 LTS, Python, PySpark" "Data Processing" {
                !docs architecture/components/iswc-platform
                tags "Data Processing"
                description "Databricks Runtime 10.4 LTS (outdated - current LTS is 13.x+). Processes EDI/JSON files from SFTP, performs IPI data transformations."

                // Components from databricks.md
                ediParser = component "EDI Parser" "Parses EDI-format submission files (CAR, CUR, CDR, MER) into structured data" "Python" "Parser"
                jsonParser = component "JSON Parser" "Parses JSON-format submission files into structured data" "Python" "Parser"
                ipiProcessor = component "IPI Processor" "Processes Interested Party Information (IPI) files and synchronizes to SQL database" "PySpark" "Processor"
                dataValidator = component "Data Validator" "Validates parsed file data against business rules before database insertion" "Python" "Validator"
                cosmosWriter = component "Cosmos Writer" "Writes audit logs and submission data to Cosmos DB via pymongo" "Python, pymongo" "Database Client"
                sqlWriter = component "SQL Writer" "Writes validated work data to SQL Server database" "Python, pyodbc" "Database Client"
            }

            dataFactory = container "Data Factory" "Azure Data Factory for pipeline orchestration and file movement" "Azure Data Factory v2" "Orchestration" {
                tags "Orchestration"
                description "14 pipelines orchestrating 100+ datasets. Monitors SFTP for new files, triggers Databricks jobs, manages data lake transfers."
                // OPEN: Data Factory pipeline logic needs detailed analysis - not in source code
            }

            // ==============================================
            // CONTAINER: DATA STORAGE
            // ==============================================

            cosmosDB = container "Cosmos DB" "NoSQL database storing audit logs, submission history, and ISWC counter cache" "Azure Cosmos DB, MongoDB API" "Database" {
                !docs architecture/components/iswc-platform
                tags "Database"
                description "MongoDB API. 822M+ audit documents. Partition key: Society Code + Month (XXXDDD). Initial provisioning: 1,000 RU. Backup: Every 4 hours, 2 backups retained (8-hour recovery window)."

                // Components from cosmos-db.md (Collections as components)
                auditCollection = component "Audit Collection" "Stores top-level submission audit logs (822M+ documents)" "Cosmos DB Collection" "Collection"
                auditRequestCollection = component "AuditRequest Collection" "Stores individual request-level audit data" "Cosmos DB Collection" "Collection"
                auditTransactionCollection = component "AuditRequestTransaction Collection" "Stores transaction-level audit details and errors" "Cosmos DB Collection" "Collection"
                iswcCounterCollection = component "ISWC Collection" "Stores ISWC counter for generating new ISWC codes" "Cosmos DB Collection" "Collection"
                cacheCollection = component "CacheIswcs Collection" "Caches next available ISWC codes for performance" "Cosmos DB Collection" "Collection"

                auditService = component "CosmosDB Audit Service" "Service layer for writing audit logs (IAuditService implementation)" "C# Service" "Service"
                cacheService = component "CosmosDB Cache Service" "Service layer for ISWC counter caching (ICacheIswcService)" "C# Service" "Service"
            }

            sqlISWC = container "SQL Server - ISWC Database" "Relational database storing work metadata, titles, contributors, and ISWCs" "Azure SQL Server" "Database" {
                tags "Database"
                description "Azure SQL Server. Contains Work, Title, Contributor, ISWC tables. Primary data store for musical work information."
                // OPEN: Is this on same SQL Server instance as IPI Database?
            }

            sqlIPI = container "SQL Server - IPI Database" "Relational database storing Interested Party Information (IPI) for rights holders" "Azure SQL Server" "Database" {
                tags "Database"
                description "Azure SQL Server. Contains IPI.Name, IPI.NameNumber tables synchronized from external IPI sources."
                // OPEN: Is this on same SQL Server instance as ISWC Database?
            }

            dataLake = container "Data Lake" "Azure Data Lake Storage Gen2 for raw and processed file storage" "Azure Data Lake Storage Gen2" "Storage" {
                tags "Storage"
                description "ADLS Gen2. Stores raw files from SFTP, processed files from Databricks, and archived data."
                // OPEN: What is the storage account name and structure?
            }

            // ==============================================
            // CONTAINER: FILE TRANSFER
            // ==============================================

            sftpServer = container "ISWC SFTP Server" "Linux SFTP server providing file-based work submission and notification delivery for agencies" "Linux VM, Native OpenSSH SFTP" "SFTP Server" {
                !docs architecture/components/networking
                tags "File Transfer"
                description "Azure Linux VM with native OpenSSH SFTP daemon (no third-party software). Per-agency isolated directories backed by Azure Storage Account File Service."

                // Components from sftp-server.md
                sftpDaemon = component "SFTP Daemon" "Native Linux OpenSSH SFTP service handling file uploads and downloads" "OpenSSH" "Server"
                storageMount = component "Azure Storage Mount" "Mounts Azure Storage Account file shares as SFTP backend storage" "Azure Files" "Storage Integration"
                agencyFolders = component "Agency-Specific Folders" "Isolated per-agency directories for secure file segregation" "File System" "Storage"
            }
        }

        // ==============================================
        // RELATIONSHIPS: PEOPLE -> SYSTEMS
        // ==============================================

        agencyUser -> agencyPortal "Manages works, views submission history, generates reports using" "HTTPS"
        agencyUser -> sftpServer "Uploads/downloads EDI and JSON files via" "SFTP"
        publicUser -> publicPortal "Searches for musical works using" "HTTPS"

        // ==============================================
        // RELATIONSHIPS: SYSTEMS -> SYSTEMS
        // ==============================================

        agencyPortal -> fastTrackSSO "Authenticates users via" "OAuth2/HTTPS"
        agencyPortal -> agencyAPI "Makes API calls to" "REST/HTTPS"
        publicPortal -> thirdPartyAPI "Searches works via" "REST/HTTPS"

        agencyAPI -> matchingEngine "Calls for duplicate detection and work matching" "REST/HTTPS, OAuth2"
        labelAPI -> matchingEngine "Calls for label matching (IP-removed)" "REST/HTTPS, OAuth2"
        publisherAPI -> matchingEngine "Calls for publisher work matching" "REST/HTTPS, OAuth2"
        thirdPartyAPI -> matchingEngine "Calls for public work searches" "REST/HTTPS, OAuth2"

        thirdPartyAPI -> suisaAPI "Exchanges work data with" "REST/HTTPS"
        dataFactory -> suisaSFTP "Retrieves files from" "SFTP"

        // ==============================================
        // RELATIONSHIPS: CONTAINERS -> CONTAINERS
        // ==============================================

        // Portal -> API
        agencyPortal -> agencyAPI "Sends work submissions and queries to" "REST/HTTPS/JSON"

        // API -> Databases
        agencyAPI -> cosmosDB "Writes audit logs to" "MongoDB Protocol/TCP"
        agencyAPI -> sqlISWC "Reads/writes work data to" "TDS/TCP"
        agencyAPI -> sqlIPI "Reads IPI data from" "TDS/TCP"

        labelAPI -> cosmosDB "Writes audit logs to" "MongoDB Protocol/TCP"
        labelAPI -> sqlISWC "Reads/writes work data to" "TDS/TCP"
        labelAPI -> sqlIPI "Reads IPI data from" "TDS/TCP"

        publisherAPI -> cosmosDB "Writes audit logs to" "MongoDB Protocol/TCP"
        publisherAPI -> sqlISWC "Reads/writes work data to" "TDS/TCP"
        publisherAPI -> sqlIPI "Reads IPI data from" "TDS/TCP"

        thirdPartyAPI -> sqlISWC "Reads work data from" "TDS/TCP"

        // Background Jobs -> Databases
        iswcJobs -> cosmosDB "Processes audit data, updates statistics" "MongoDB Protocol/TCP"
        iswcJobs -> sqlISWC "Updates work data, runs scheduled tasks" "TDS/TCP"
        iswcJobs -> sqlIPI "Synchronizes IPI data" "TDS/TCP"

        // SFTP -> Data Pipeline
        sftpServer -> dataLake "Writes uploaded files to" "Azure Storage API/HTTPS"
        dataFactory -> dataLake "Reads files from, writes processed files to" "Azure Storage API/HTTPS"
        dataFactory -> databricks "Triggers processing jobs on" "Databricks API/HTTPS"

        databricks -> dataLake "Reads raw files from, writes processed files to" "Azure Storage API/HTTPS"
        databricks -> cosmosDB "Writes audit logs and file processing results to" "MongoDB Protocol/TCP"
        databricks -> sqlISWC "Writes validated work data to" "TDS/TCP"
        databricks -> sqlIPI "Writes IPI data to" "TDS/TCP"

        // ==============================================
        // RELATIONSHIPS: COMPONENTS -> COMPONENTS
        // (Agency Portal)
        // ==============================================

        authUI -> fastTrackSSO "Redirects for authentication" "OAuth2/HTTPS"

        registrationUI -> stateManagement "Dispatches actions to" "Redux Actions"
        searchUI -> stateManagement "Dispatches actions to" "Redux Actions"
        reportsUI -> stateManagement "Dispatches actions to" "Redux Actions"
        authUI -> stateManagement "Dispatches actions to" "Redux Actions"

        stateManagement -> apiClient "Calls API services from thunks" "TypeScript Function Calls"
        apiClient -> agencyAPI "Makes HTTP requests to" "REST/HTTPS/JSON"

        // ==============================================
        // RELATIONSHIPS: COMPONENTS -> COMPONENTS
        // (Agency API)
        // ==============================================

        workController -> pipelineManager "Sends submissions to" "C# Method Calls"
        submissionController -> pipelineManager "Sends file submissions to" "C# Method Calls"
        searchController -> matchingEngine "Queries for works" "REST/HTTPS"

        pipelineManager -> validationPipeline "Executes validation rules via" "C# Method Calls"
        pipelineManager -> matchingPipeline "Executes matching via" "C# Method Calls"
        pipelineManager -> processingPipeline "Executes ISWC assignment via" "C# Method Calls"
        pipelineManager -> auditManager "Logs submission events via" "C# Method Calls"

        matchingPipeline -> matchingEngine "Calls GET /Work/Match" "REST/HTTPS, OAuth2"

        auditManager -> auditService "Writes audit logs via" "C# Method Calls"
        processingPipeline -> sqlISWC "Persists work data to" "Entity Framework Core/TDS"
        validationPipeline -> sqlIPI "Validates IPI numbers against" "Entity Framework Core/TDS"

        // ==============================================
        // RELATIONSHIPS: COMPONENTS -> COMPONENTS
        // (Databricks)
        // ==============================================

        ediParser -> dataLake "Reads EDI files from" "Azure Storage SDK/HTTPS"
        jsonParser -> dataLake "Reads JSON files from" "Azure Storage SDK/HTTPS"

        ediParser -> dataValidator "Passes parsed data to" "Python Function Calls"
        jsonParser -> dataValidator "Passes parsed data to" "Python Function Calls"
        ipiProcessor -> dataValidator "Passes IPI data to" "Python Function Calls"

        dataValidator -> cosmosWriter "Writes audit logs via" "Python Function Calls"
        dataValidator -> sqlWriter "Writes validated data via" "Python Function Calls"

        cosmosWriter -> cosmosDB "Inserts documents to" "pymongo/MongoDB Protocol"
        sqlWriter -> sqlISWC "Inserts work data to" "pyodbc/TDS"
        sqlWriter -> sqlIPI "Inserts IPI data to" "pyodbc/TDS"

        // ==============================================
        // RELATIONSHIPS: COMPONENTS -> COMPONENTS
        // (Cosmos DB)
        // ==============================================

        auditService -> auditCollection "Writes submission logs to" "MongoDB Protocol"
        auditService -> auditRequestCollection "Writes request logs to" "MongoDB Protocol"
        auditService -> auditTransactionCollection "Writes transaction logs to" "MongoDB Protocol"

        cacheService -> iswcCounterCollection "Reads/updates counter from" "MongoDB Protocol"
        cacheService -> cacheCollection "Reads/writes cache to" "MongoDB Protocol"

        auditManager -> auditService "Calls for audit logging" "C# Method Calls"
        processingPipeline -> cacheService "Retrieves next ISWC from" "C# Method Calls"

        // ==============================================
        // RELATIONSHIPS: COMPONENTS -> COMPONENTS
        // (SFTP Server)
        // ==============================================

        sftpDaemon -> storageMount "Writes uploaded files to" "File System I/O"
        storageMount -> dataLake "Persists files to Azure Storage" "Azure Files Protocol"
        sftpDaemon -> agencyFolders "Enforces per-agency isolation via" "POSIX Permissions"

    }

    views {

        // ==============================================
        // SYSTEM LANDSCAPE VIEW
        // ==============================================

        systemLandscape "SystemLandscape" "System Landscape view showing all systems and actors in the ISWC ecosystem" {
            include *
            autoLayout lr
        }

        // ==============================================
        // LEVEL 1: SYSTEM CONTEXT VIEW
        // ==============================================

        systemContext iswcPlatform "SystemContext" "System Context view showing ISWC Platform and its relationships with users and external systems" {
            include *
            autoLayout lr
        }

        // ==============================================
        // LEVEL 2: CONTAINER VIEW
        // ==============================================

        container iswcPlatform "Containers" "Container view showing web applications, APIs, background processing, and data storage within ISWC Platform" {
            include *
            autoLayout lr
        }

        // ==============================================
        // LEVEL 3: COMPONENT VIEWS
        // ==============================================

        component agencyPortal "AgencyPortalComponents" "Component view of Agency Portal showing React UI components, Redux state management, and API client services" {
            include *
            autoLayout lr
        }

        component agencyAPI "AgencyAPIComponents" "Component view of Agency API showing controllers, pipeline orchestration, and submission processing workflow" {
            include *
            autoLayout tb
        }

        component databricks "DatabricksComponents" "Component view of Databricks showing file parsers, validators, and database writers for batch processing" {
            include *
            autoLayout tb
        }

        component cosmosDB "CosmosDBComponents" "Component view of Cosmos DB showing collections (Audit, Cache, ISWC Counter) and service layers" {
            include *
            autoLayout lr
        }

        component sftpServer "SFTPComponents" "Component view of SFTP Server showing daemon, Azure Storage mount, and agency folder isolation" {
            include *
            autoLayout tb
        }

        // ==============================================
        // DYNAMIC VIEWS (Workflows)
        // ==============================================

        dynamic iswcPlatform "WorkSubmissionFlow" "Work submission flow from Agency Portal through validation, matching, and ISWC assignment" {
            agencyUser -> agencyPortal "Submits new work"
            agencyPortal -> agencyAPI "POST /api/submissions"
            agencyAPI -> matchingEngine "Calls for duplicate detection"
            matchingEngine -> agencyAPI "Returns match results"
            agencyAPI -> sqlISWC "Persists work data"
            agencyAPI -> cosmosDB "Logs audit"
            agencyAPI -> agencyPortal "Returns success + ISWC"
            agencyPortal -> agencyUser "Displays result"
            autoLayout lr
        }

        dynamic iswcPlatform "FileProcessingFlow" "File processing flow from SFTP upload through Databricks to database persistence" {
            agencyUser -> sftpServer "Uploads EDI file"
            sftpServer -> dataLake "Stores file"
            dataFactory -> dataLake "Detects new file"
            dataFactory -> databricks "Triggers job"
            databricks -> dataLake "Reads file"
            databricks -> sqlISWC "Writes work data"
            databricks -> cosmosDB "Writes audit log"
            autoLayout lr
        }

        // ==============================================
        // STYLES
        // ==============================================

        styles {
            element "Software System" {
                background #1168bd
                color #ffffff
                shape RoundedBox
            }
            element "Main System" {
                background #0d47a1
                color #ffffff
                shape RoundedBox
            }
            element "External System" {
                background #999999
                color #ffffff
                shape RoundedBox
            }
            element "External" {
                background #999999
                color #ffffff
            }
            element "Person" {
                background #08427b
                color #ffffff
                shape Person
            }
            element "User" {
                background #08427b
                color #ffffff
                shape Person
            }
            element "Container" {
                background #438dd5
                color #ffffff
                shape RoundedBox
            }
            element "Web Application" {
                background #1976d2
                color #ffffff
                shape WebBrowser
            }
            element "API" {
                background #2196f3
                color #ffffff
                shape Hexagon
            }
            element "Database" {
                background #0d47a1
                color #ffffff
                shape Cylinder
            }
            element "Storage" {
                background #1565c0
                color #ffffff
                shape Folder
            }
            element "Data Processing" {
                background #4caf50
                color #ffffff
                shape Component
            }
            element "Background Jobs" {
                background #66bb6a
                color #ffffff
                shape Component
            }
            element "Orchestration" {
                background #8bc34a
                color #ffffff
                shape Component
            }
            element "File Transfer" {
                background #ff9800
                color #ffffff
                shape Pipe
            }
            element "Component" {
                background #85bbf0
                color #000000
                shape Component
            }
            element "UI Component" {
                background #64b5f6
                color #ffffff
            }
            element "Controller" {
                background #42a5f5
                color #ffffff
            }
            element "Service" {
                background #2196f3
                color #ffffff
            }
            element "Pipeline" {
                background #1e88e5
                color #ffffff
            }
            element "Manager" {
                background #1976d2
                color #ffffff
            }
            element "Orchestrator" {
                background #1565c0
                color #ffffff
            }
            element "Collection" {
                background #0d47a1
                color #ffffff
                shape Cylinder
            }
            element "Parser" {
                background #66bb6a
                color #ffffff
            }
            element "Processor" {
                background #4caf50
                color #ffffff
            }
            element "Validator" {
                background #388e3c
                color #ffffff
            }
            element "Database Client" {
                background #2e7d32
                color #ffffff
            }
            element "Server" {
                background #ff9800
                color #ffffff
            }
            element "Storage Integration" {
                background #f57c00
                color #ffffff
            }

            relationship "Relationship" {
                thickness 2
                color #707070
                dashed false
            }
        }

    }

    configuration {
        scope softwaresystem
    }

}
