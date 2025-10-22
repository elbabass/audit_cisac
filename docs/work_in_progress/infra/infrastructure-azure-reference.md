# CISAC Azure Infrastructure - Reference Document

**Export Date:** October 21, 2025
**Subscription:** CISAC
**Primary Region:** West Europe
**Source Document:** [CSV Export of all Azure Resources](../../resources/Azureresources-export-20251021.csv)

---

## Table of Contents

1. [Overview](#overview)
2. [Dev Environment](#dev-environment)
3. [UAT Environment](#uat-environment)
4. [Prod Environment](#prod-environment)
5. [Management Resources](#management-resources)
6. [Core Resources](#core-resources)
7. [Global Resources](#global-resources)
8. [Resource Types Glossary](#resource-types-glossary)

---

## Overview

This documentation references all Azure resources deployed for the CISAC ISWC (International Standard Work Code) project. The infrastructure is organized into three main environments: **Dev** (Development), **UAT** (User Acceptance Testing), and **Prod** (Production).

**Global Statistics:**

- Total number of resources: 343
- Environments: 3 (Dev, UAT, Prod)
- Resource groups: 15

---

## Dev Environment

### Resource Group: ISWCDev

#### Core Services

##### cisaciswcdev (Multi-service)

- **Type:** API Management Service
- **Description:** API management service for the development environment
- **Relationships:**
  - Connected to virtual network [CISACDev](#cisacdev)
  - Uses key vault [cisaciswcdev](#cisaciswcdev-key-vault)
  - Monitored by Application Insights [cisaciswcdev](#cisaciswcdev-application-insights)

##### cisaciswcdev (Data Factory)

- **Type:** Data Factory (V2)
- **Description:** Data orchestration and transformation
- **Relationships:**
  - Connected to database [ISWC](#iswc-dev)
  - Uses key vault [cisaciswcdev](#cisaciswcdev-key-vault)

##### cisaciswcdev (Azure Cosmos DB)

- **Type:** Azure Cosmos DB account
- **Description:** NoSQL database for distributed storage
- **Relationships:**
  - Used by Dev environment App Services
  - Keys stored in [cisaciswcdev](#cisaciswcdev-key-vault)

##### cisaciswcdev (Key Vault)

- **Type:** Key Vault
- **Description:** Centralized secret and certificate management
- **Relationships:**
  - Used by all Dev environment services
  - Referenced by Data Factory, API Management, App Services

##### cisaciswcdev (Application Insights)

- **Type:** Application Insights
- **Description:** Dev environment monitoring and telemetry
- **Relationships:**
  - Collects logs from all Dev App Services
  - Alert configured: [Failure Anomalies - cisaciswcdev](#failure-anomalies---cisaciswcdev)

#### App Services

##### cisaciswcapidev

- **Type:** App Service + Application Insights
- **Description:** Main ISWC API for development environment
- **Relationships:**
  - Hosted on [ISWCDev](#iswcdev-app-service-plan) (App Service Plan)
  - Monitored by Application Insights
  - Uses database [ISWC](#iswc-dev)
  - Availability test: [cisaciswcuat-cisaciswcapidev](#cisaciswcuat-cisaciswcapidev)
  - Alert: [Failure Anomalies - cisaciswcapidev](#failure-anomalies---cisaciswcapidev)

##### cisaciswcapilabeldev

- **Type:** App Service + Application Insights
- **Description:** Label management API for development environment
- **Relationships:**
  - Hosted on [ISWCDev](#iswcdev-app-service-plan) (App Service Plan)
  - Monitored by Application Insights
  - Alert: [Failure Anomalies - cisaciswcapilabeldev](#failure-anomalies---cisaciswcapilabeldev)

##### cisaciswcapipublisherdev

- **Type:** App Service + Application Insights
- **Description:** Publisher API in development
- **Relationships:**
  - Hosted on [ISWCDev](#iswcdev-app-service-plan) (App Service Plan)
  - Monitored by Application Insights
  - Alert: [Failure Anomalies - cisaciswcapipublisherdev](#failure-anomalies---cisaciswcapipublisherdev)

##### cisaciswcapithirdpartydev

- **Type:** App Service + Application Insights
- **Description:** Third-party integration API in development
- **Relationships:**
  - Hosted on [ISWCDev](#iswcdev-app-service-plan) (App Service Plan)
  - Monitored by Application Insights
  - Alert: [Failure Anomalies - cisaciswcapithirdpartydev](#failure-anomalies---cisaciswcapithirdpartydev)

##### cisaciswcjobsdev

- **Type:** Function App + Storage Account + Application Insights
- **Description:** Azure Functions for scheduled tasks and background jobs
- **Relationships:**
  - Uses storage account cisaciswcjobsdev for runtime
  - Monitored by Application Insights cisaciswcjobsdev
  - Alert: [Failure Anomalies - cisaciswcjobsdev](#failure-anomalies---cisaciswcjobsdev)

##### cisaciswcportaldev

- **Type:** App Service + Application Insights
- **Description:** User web portal in development
- **Relationships:**
  - Hosted on [ISWCDev](#iswcdev-app-service-plan) (App Service Plan)
  - Monitored by Application Insights
  - Alert: [Failure Anomalies - cisaciswcportaldev](#failure-anomalies---cisaciswcportaldev)

##### cisaciswcpublicdev

- **Type:** App Service + Application Insights
- **Description:** ISWC public API in development
- **Relationships:**
  - Hosted on [ISWCDev](#iswcdev-app-service-plan) (App Service Plan)
  - Monitored by Application Insights
  - Alert: [Failure Anomalies - cisaciswcpublicdev](#failure-anomalies---cisaciswcpublicdev)

#### Network Infrastructure

##### CISACAzDSFTP (Virtual Machine + components)

- **Type:** Linux/Windows Virtual Machine
- **Description:** SFTP server for Dev environment
- **Associated Components:**
  - Network interface: CISACAzDSFTP
  - Network security group: CISACAzDSFTP
  - Public IP address: CISACAzDSFTP
  - OS Disk: CISACAzDSFTP-OS
- **Relationships:**
  - Connected to network [CISACDev](#cisacdev)

#### Databricks

##### ISWCDev (Azure Databricks Service)

- **Type:** Azure Databricks Service
- **Description:** Data analytics and Machine Learning platform
- **Relationships:**
  - Uses virtual network [workers-vnet](#workers-vnet-dev)
  - Security group: [workers-sg](#workers-sg-dev)
  - Storage account: [dbstoragenl5sveqhpn3kq](#dbstoragenl5sveqhpn3kq)
  - Jobs storage account: [cisaciswcdatabricksdev](#cisaciswcdatabricksdev)
  - Managed identity: [dbmanagedidentity](#dbmanagedidentity-dev)

##### Databricks Dev Virtual Machines

The following VMs are automatically managed by Databricks:

1. **093fe79ecf134d3fbf8c53f1e330fa21**
   - Private network interface: 093fe79ecf134d3fbf8c53f1e330fa21-privateNIC
   - Public network interface: 093fe79ecf134d3fbf8c53f1e330fa21-publicNIC
   - Public IP address: 093fe79ecf134d3fbf8c53f1e330fa21-publicIP
   - OS disk: 093fe79ecf134d3fbf8c53f1e330fa21-osDisk
   - Container disk: 093fe79ecf134d3fbf8c53f1e330fa21-containerRootVolume

2. **638d108370ca4bb8ab667544c1ff1c6c**
   - Private network interface: 638d108370ca4bb8ab667544c1ff1c6c-privateNIC
   - Public network interface: 638d108370ca4bb8ab667544c1ff1c6c-publicNIC
   - Public IP address: 638d108370ca4bb8ab667544c1ff1c6c-publicIP
   - OS disk: 638d108370ca4bb8ab667544c1ff1c6c-osDisk
   - Container disk: 638d108370ca4bb8ab667544c1ff1c6c-containerRootVolume

3. **bcd7b0b1ccae4831b0af0b7be0a0f165**
   - Private network interface: bcd7b0b1ccae4831b0af0b7be0a0f165-privateNIC
   - Public network interface: bcd7b0b1ccae4831b0af0b7be0a0f165-publicNIC
   - Public IP address: bcd7b0b1ccae4831b0af0b7be0a0f165-publicIP
   - OS disk: bcd7b0b1ccae4831b0af0b7be0a0f165-osDisk
   - Container disk: bcd7b0b1ccae4831b0af0b7be0a0f165-containerRootVolume

##### workers-vnet (Dev)

- **Type:** Virtual Network
- **Description:** Virtual network dedicated to Databricks Dev workers
- **Resource Group:** ISWCDatabricksDev

##### workers-sg (Dev)

- **Type:** Network Security Group
- **Description:** Security rules for Databricks Dev workers
- **Resource Group:** ISWCDatabricksDev

##### dbmanagedidentity (Dev)

- **Type:** Managed Identity
- **Description:** Identity for secure access to Azure resources
- **Resource Group:** ISWCDatabricksDev

##### dbstoragenl5sveqhpn3kq

- **Type:** Storage Account
- **Description:** Storage for Databricks Dev
- **Resource Group:** ISWCDatabricksDev

##### cisaciswcdatabricksdev

- **Type:** Storage Account
- **Description:** Storage for Databricks Dev jobs
- **Resource Group:** ISWCDev

#### SFTP Storage

##### cisaciswcsftpdev

- **Type:** Storage Account
- **Description:** Storage for SFTP transfers in development
- **Relationships:**
  - Admin keys in [cisaciswcsftpadmindev](#cisaciswcsftpadmindev)
  - User keys in [cisaciswcsftpuserdev](#cisaciswcsftpuserdev)

##### cisaciswcsftpadmindev

- **Type:** Key Vault
- **Description:** SFTP Dev admin secrets

##### cisaciswcsftpuserdev

- **Type:** Key Vault
- **Description:** SFTP Dev user secrets

#### Database

##### cisaciswcwedev

- **Type:** SQL Server
- **Description:** Azure SQL Server for Dev environment
- **Relationships:**
  - Hosts database [ISWC](#iswc-dev)

##### ISWC (Dev)

- **Type:** SQL Database
- **Description:** Main ISWC Dev database
- **Resource Group:** ISWCDev
- **Relationships:**
  - Hosted on server [cisaciswcwedev](#cisaciswcwedev)
  - Used by all Dev App Services

#### Support Infrastructure

##### ISWCDev (App Service Plan)

- **Type:** App Service Plan
- **Description:** Hosting plan for Dev App Services
- **Relationships:**
  - Hosts all Dev environment App Services

##### ISWCDev (Application Insights)

- **Type:** Application Insights
- **Description:** Global Dev environment monitoring
- **Relationships:**
  - Alert: [Failure Anomalies - ISWCDev](#failure-anomalies---iswcdev)

#### Dashboards and Monitoring

##### 660ec011-e52e-4621-9393-b5177005fdad-dashboard

- **Type:** Shared Dashboard
- **Description:** Dev monitoring dashboard

##### 6c3b842b-2450-4b88-b443-bc3d321f4204-dashboard

- **Type:** Shared Dashboard
- **Description:** Dev monitoring dashboard (secondary)

##### d1c643a8-e6fc-4b21-b918-ddd7371c13e2-dashboard

- **Type:** Shared Dashboard
- **Description:** Dev monitoring dashboard (tertiary)

##### f1dde0d2-e6af-4da3-88d1-07ecb06f1241

- **Type:** Azure Workbook
- **Description:** Azure workbook for advanced Dev analytics

---

## UAT Environment

### Resource Group: ISWCUAT

#### Core Services

##### cisaciswcuat (Multi-service)

- **Type:** API Management Service
- **Description:** API management service for UAT environment
- **Relationships:**
  - Connected to virtual network [CISACUAT](#cisacuat)
  - Uses key vault [cisaciswcuat](#cisaciswcuat-key-vault)
  - Monitored by Application Insights [cisaciswcuat](#cisaciswcuat-application-insights)

##### cisaciswcuat (Data Factory)

- **Type:** Data Factory (V2)
- **Description:** UAT data orchestration and transformation
- **Relationships:**
  - Connected to database [ISWC](#iswc-uat)
  - Uses key vault [cisaciswcuat](#cisaciswcuat-key-vault)

##### cisaciswcuat (Azure Cosmos DB)

- **Type:** Azure Cosmos DB account
- **Description:** NoSQL database for UAT distributed storage
- **Relationships:**
  - Used by UAT environment App Services
  - Keys stored in [cisaciswcuat](#cisaciswcuat-key-vault)

##### cisaciswcuat (Key Vault)

- **Type:** Key Vault
- **Description:** Centralized UAT secret and certificate management
- **Relationships:**
  - Used by all UAT environment services
  - Referenced by Data Factory, API Management, App Services

##### cisaciswcuat (Application Insights)

- **Type:** Application Insights
- **Description:** UAT environment monitoring and telemetry
- **Relationships:**
  - Collects logs from all UAT App Services
  - Alert configured: [Failure Anomalies - cisaciswcuat](#failure-anomalies---cisaciswcuat)

#### App Services

##### cisaciswcapiuat

- **Type:** App Service + Application Insights
- **Description:** Main ISWC API for UAT environment
- **Relationships:**
  - Hosted on [ISWCUAT](#iswcuat-app-service-plan) (App Service Plan)
  - Deployment slot: cisaciswcapiuat/temp
  - Monitored by Application Insights
  - Uses database [ISWC](#iswc-uat)
  - Alert: [Failure Anomalies - cisaciswcapiuat](#failure-anomalies---cisaciswcapiuat)

##### cisaciswcapilabeluat

- **Type:** App Service + Application Insights
- **Description:** Label management API for UAT environment
- **Relationships:**
  - Hosted on [ISWCUAT](#iswcuat-app-service-plan) (App Service Plan)
  - Deployment slot: cisaciswcapilabeluat/cisaciswcapilabeluat-temp
  - Monitored by Application Insights
  - Alert: [Failure Anomalies - cisaciswcapilabeluat](#failure-anomalies---cisaciswcapilabeluat)

##### cisaciswcapipublisheruat

- **Type:** App Service + Application Insights
- **Description:** Publisher API in UAT
- **Relationships:**
  - Hosted on [ISWCUAT](#iswcuat-app-service-plan) (App Service Plan)
  - Deployment slot: cisaciswcapipublisheruat/temp
  - Monitored by Application Insights
  - Alert: [Failure Anomalies - cisaciswcapipublisheruat](#failure-anomalies---cisaciswcapipublisheruat)

##### cisaciswcapithirdpartyuat

- **Type:** App Service + Application Insights
- **Description:** Third-party integration API in UAT
- **Relationships:**
  - Hosted on [ISWCUAT](#iswcuat-app-service-plan) (App Service Plan)
  - Monitored by Application Insights
  - Alert: [Failure Anomalies - cisaciswcapithirdpartyuat](#failure-anomalies---cisaciswcapithirdpartyuat)

##### cisaciswcjobsuat

- **Type:** Function App + Storage Account + Application Insights
- **Description:** UAT Azure Functions for scheduled tasks and background jobs
- **Relationships:**
  - Uses storage account cisaciswcjobsuat for runtime
  - Monitored by Application Insights cisaciswcjobsuat
  - Alert: [Failure Anomalies - cisaciswcjobsuat](#failure-anomalies---cisaciswcjobsuat)

##### cisaciswcportaluat

- **Type:** App Service + Application Insights
- **Description:** User web portal in UAT
- **Relationships:**
  - Hosted on [ISWCUAT](#iswcuat-app-service-plan) (App Service Plan)
  - Monitored by Application Insights
  - Alert: [Failure Anomalies - cisaciswcportaluat](#failure-anomalies---cisaciswcportaluat)

##### cisaciswcpublicuat

- **Type:** App Service + Application Insights
- **Description:** ISWC public API in UAT
- **Relationships:**
  - Hosted on [ISWCUAT](#iswcuat-app-service-plan) (App Service Plan)
  - Monitored by Application Insights
  - Alert: [Failure Anomalies - cisaciswcpublicuat](#failure-anomalies---cisaciswcpublicuat)

#### Network Infrastructure

##### CISACAzUSFTP (Virtual Machine + components)

- **Type:** Linux/Windows Virtual Machine
- **Description:** SFTP server for UAT environment
- **Associated Components:**
  - Network interface: CISACAzUSFTP
  - Network security group: CISACAzUSFTP
  - Public IP address: CISACAzUSFTP
  - OS Disk: CISACAzUSFTP-OS
- **Relationships:**
  - Connected to network [CISACUAT](#cisacuat)

#### Databricks

##### ISWCUAT (Azure Databricks Service)

- **Type:** Azure Databricks Service
- **Description:** UAT data analytics and Machine Learning platform
- **Relationships:**
  - Uses virtual network [workers-vnet](#workers-vnet-uat)
  - Security group: [workers-sg](#workers-sg-uat)
  - Storage account: [dbstorageiblnramgsqgyw](#dbstorageiblnramgsqgyw)
  - Jobs storage account: [cisaciswcdatabricksuat](#cisaciswcdatabricksuat)
  - Managed identity: [dbmanagedidentity](#dbmanagedidentity-uat)

##### Databricks UAT Virtual Machines

The following VMs are automatically managed by Databricks:

1. **146c6be7f20f43f095fefcd15b1eddf1**
   - Private network interface: 146c6be7f20f43f095fefcd15b1eddf1-privateNIC
   - Public network interface: 146c6be7f20f43f095fefcd15b1eddf1-publicNIC
   - Public IP address: 146c6be7f20f43f095fefcd15b1eddf1-publicIP
   - OS disk: 146c6be7f20f43f095fefcd15b1eddf1-osDisk
   - Container disk: 146c6be7f20f43f095fefcd15b1eddf1-containerRootVolume

2. **1890bc7010d64e01b2ba442e89306150**
   - Private network interface: 1890bc7010d64e01b2ba442e89306150-privateNIC
   - Public network interface: 1890bc7010d64e01b2ba442e89306150-publicNIC
   - Public IP address: 1890bc7010d64e01b2ba442e89306150-publicIP
   - Container disk: 1890bc7010d64e01b2ba442e89306150-containerRootVolume

3. **4402143c5798468ea98f2c96cdc72909**
   - Private network interface: 4402143c5798468ea98f2c96cdc72909-privateNIC
   - Public network interface: 4402143c5798468ea98f2c96cdc72909-publicNIC
   - Public IP address: 4402143c5798468ea98f2c96cdc72909-publicIP
   - Container disk: 4402143c5798468ea98f2c96cdc72909-containerRootVolume

4. **47aff4f021564934b6a0370cf0613828**
   - Private network interface: 47aff4f021564934b6a0370cf0613828-privateNIC
   - Public network interface: 47aff4f021564934b6a0370cf0613828-publicNIC
   - Public IP address: 47aff4f021564934b6a0370cf0613828-publicIP
   - Container disk: 47aff4f021564934b6a0370cf0613828-containerRootVolume

5. **73d6b6e155ed4fc48fe1538426861c62**
   - Private network interface: 73d6b6e155ed4fc48fe1538426861c62-privateNIC
   - Public network interface: 73d6b6e155ed4fc48fe1538426861c62-publicNIC
   - Public IP address: 73d6b6e155ed4fc48fe1538426861c62-publicIP
   - OS disk: 73d6b6e155ed4fc48fe1538426861c62-osDisk
   - Container disk: 73d6b6e155ed4fc48fe1538426861c62-containerRootVolume

6. **9c0bdcd837344b2d98c6a86c32651cf7**
   - Private network interface: 9c0bdcd837344b2d98c6a86c32651cf7-privateNIC
   - Public network interface: 9c0bdcd837344b2d98c6a86c32651cf7-publicNIC
   - Public IP address: 9c0bdcd837344b2d98c6a86c32651cf7-publicIP
   - Container disk: 9c0bdcd837344b2d98c6a86c32651cf7-containerRootVolume

7. **a6f222d76f6a4fb7b9fabfc6487fe1cb**
   - Private network interface: a6f222d76f6a4fb7b9fabfc6487fe1cb-privateNIC
   - Public network interface: a6f222d76f6a4fb7b9fabfc6487fe1cb-publicNIC
   - Public IP address: a6f222d76f6a4fb7b9fabfc6487fe1cb-publicIP
   - Container disk: a6f222d76f6a4fb7b9fabfc6487fe1cb-containerRootVolume

8. **eaff7c3c506a4b359d6c28e267bb541e**
   - Private network interface: eaff7c3c506a4b359d6c28e267bb541e-privateNIC
   - Public network interface: eaff7c3c506a4b359d6c28e267bb541e-publicNIC
   - Public IP address: eaff7c3c506a4b359d6c28e267bb541e-publicIP
   - OS disk: eaff7c3c506a4b359d6c28e267bb541e-osDisk
   - Container disk: eaff7c3c506a4b359d6c28e267bb541e-containerRootVolume

##### workers-vnet (UAT)

- **Type:** Virtual Network
- **Description:** Virtual network dedicated to Databricks UAT workers
- **Resource Group:** ISWCDatabricksUAT

##### workers-sg (UAT)

- **Type:** Network Security Group
- **Description:** Security rules for Databricks UAT workers
- **Resource Group:** ISWCDatabricksUAT

##### dbmanagedidentity (UAT)

- **Type:** Managed Identity
- **Description:** Identity for secure access to Azure UAT resources
- **Resource Group:** ISWCDatabricksUAT

##### dbstorageiblnramgsqgyw

- **Type:** Storage Account
- **Description:** Storage for Databricks UAT
- **Resource Group:** ISWCDatabricksUAT

##### cisaciswcdatabricksuat

- **Type:** Storage Account
- **Description:** Storage for Databricks UAT jobs
- **Resource Group:** ISWCUAT

#### SFTP Storage

##### cisaciswcsftpuat

- **Type:** Storage Account
- **Description:** Storage for SFTP transfers in UAT
- **Relationships:**
  - Admin keys in [cisaciswcsftpadminuat](#cisaciswcsftpadminuat)
  - User keys in [cisaciswcsftpuseruat](#cisaciswcsftpuseruat)

##### cisaciswcsftpadminuat

- **Type:** Key Vault
- **Description:** SFTP UAT admin secrets

##### cisaciswcsftpuseruat

- **Type:** Key Vault
- **Description:** SFTP UAT user secrets

#### Database

##### cisaciswcweuat

- **Type:** SQL Server
- **Description:** Azure SQL Server for UAT environment
- **Relationships:**
  - Hosts database [ISWC](#iswc-uat)

##### ISWC (UAT)

- **Type:** SQL Database
- **Description:** Main ISWC UAT database
- **Resource Group:** ISWCUAT
- **Relationships:**
  - Hosted on server [cisaciswcweuat](#cisaciswcweuat)
  - Used by all UAT App Services

#### Support Infrastructure

##### ISWCUAT (App Service Plan)

- **Type:** App Service Plan
- **Description:** Hosting plan for UAT App Services
- **Relationships:**
  - Hosts all UAT environment App Services

##### ISWCUAT (Application Insights)

- **Type:** Application Insights
- **Description:** Global UAT environment monitoring
- **Relationships:**
  - Alert: [Failure Anomalies - ISWCUAT](#failure-anomalies---iswcuat)

#### Dashboards and Monitoring

##### 0485f560-69a2-4b3b-8ead-4a0bb844120a-dashboard

- **Type:** Shared Dashboard
- **Description:** UAT monitoring dashboard

##### cisaciswcuat-cisaciswcapidev

- **Type:** Availability test
- **Description:** Availability test for Dev API from UAT

#### Alerts and Action Groups

##### Application Insights Smart Detection

- **Type:** Action Group
- **Description:** Action group for UAT smart detection
- **Location:** Global

##### Migration_AG1

- **Type:** Action Group
- **Description:** Migration action group (1)
- **Location:** Global

##### Migration_AG2

- **Type:** Action Group
- **Description:** Migration action group (2)
- **Location:** Global

---

## Prod Environment

### Resource Group: ISWCProd

#### Core Services

##### cisaciswcprod (Multi-service)

- **Type:** API Management Service
- **Description:** API management service for production environment
- **Relationships:**
  - Connected to virtual network [CISACProd](#cisacprod)
  - Uses key vault [cisaciswcprod](#cisaciswcprod-key-vault)
  - Monitored by Application Insights [cisaciswcprod](#cisaciswcprod-application-insights)
  - Alert: [cisaciswcprod-APIManagement Failures](#cisaciswcprod-apimanagement-failures)
  - Health Check: [Health Check - cisaciswcprod - API Management Service - Capacity](#health-check---cisaciswcprod---api-management-service---capacity)

##### cisaciswcprod (Data Factory)

- **Type:** Data Factory (V2)
- **Description:** Production data orchestration and transformation
- **Relationships:**
  - Connected to database [ISWC](#iswc-prod)
  - Uses key vault [cisaciswcprod](#cisaciswcprod-key-vault)

##### cisaciswcprod (Azure Cosmos DB)

- **Type:** Azure Cosmos DB account
- **Description:** NoSQL database for production distributed storage
- **Relationships:**
  - Used by Prod environment App Services
  - Keys stored in [cisaciswcprod](#cisaciswcprod-key-vault)
  - Health Check: [Health Check - cisaciswcprod - CosmosDB - RU Consumption](#health-check---cisaciswcprod---cosmosdb---ru-consumption)

##### cisaciswcprod (Key Vault)

- **Type:** Key Vault
- **Description:** Centralized production secret and certificate management
- **Relationships:**
  - Used by all Prod environment services
  - Referenced by Data Factory, API Management, App Services

##### cisaciswcprod (Application Insights)

- **Type:** Application Insights
- **Description:** Prod environment monitoring and telemetry
- **Relationships:**
  - Collects logs from all Prod App Services
  - Alert configured: [Failure Anomalies - cisaciswcprod](#failure-anomalies---cisaciswcprod)
  - Alert: [cisaciswcprod - EDI failures](#cisaciswcprod---edi-failures)
  - Alert: [cisaciswcprod - REST API Errors](#cisaciswcprod---rest-api-errors)

##### cisaciswcprod (Recovery Services Vault)

- **Type:** Recovery Services Vault
- **Description:** Backup and disaster recovery service for production
- **Resource Group:** ManagementProd

#### App Services

##### cisaciswcapiprod

- **Type:** App Service + Application Insights
- **Description:** Main ISWC API for production environment
- **Relationships:**
  - Hosted on [ISWCProd](#iswcprod-app-service-plan) (App Service Plan)
  - Monitored by Application Insights cisaciswcapiprod
  - Uses database [ISWC](#iswc-prod)
  - Availability test: [root-cisaciswcapiprod](#root-cisaciswcapiprod-availability-test)
  - Metric alert: [root-cisaciswcapiprod](#root-cisaciswcapiprod-metric-alert-rule)
  - Alert: [Failure Anomalies - cisaciswcapiprod](#failure-anomalies---cisaciswcapiprod)
  - Alert: [cisaciswcapiprod-POST SearchByTitleAndContributor](#cisaciswcapiprod-post-searchbytitleandcontributor)
  - Health Check Handle Count: [Health Check - cisaciswcapiprod - App Service - Handle Count](#health-check---cisaciswcapiprod---app-service---handle-count)
  - Health Check Response Time: [Health Check - cisaciswcapiprod - App Service - Response Time](#health-check---cisaciswcapiprod---app-service---response-time)

##### cisaciswcapilabelprod

- **Type:** App Service + Application Insights
- **Description:** Label management API for production environment
- **Relationships:**
  - Hosted on [ISWCProd](#iswcprod-app-service-plan) (App Service Plan)
  - Monitored by Application Insights
  - Alert: [Failure Anomalies - cisaciswcapilabelprod](#failure-anomalies---cisaciswcapilabelprod)
  - Alert: [cisaciswcapilabelprod - POST AddLabelSubmissionBatch](#cisaciswcapilabelprod---post-addlabelsubmissionbatch)

##### cisaciswcapipublisherprod

- **Type:** App Service + Application Insights
- **Description:** Publisher API in production
- **Relationships:**
  - Hosted on [ISWCProd](#iswcprod-app-service-plan) (App Service Plan)
  - Monitored by Application Insights
  - Alert: [Failure Anomalies - cisaciswcapipublisherprod](#failure-anomalies---cisaciswcapipublisherprod)

##### cisaciswcapithirdpartyprod

- **Type:** App Service + Application Insights
- **Description:** Third-party integration API in production
- **Relationships:**
  - Hosted on [ISWCProd](#iswcprod-app-service-plan) (App Service Plan)
  - Monitored by Application Insights
  - Alert: [Failure Anomalies - cisaciswcapithirdpartyprod](#failure-anomalies---cisaciswcapithirdpartyprod)

##### cisaciswcjobsprod

- **Type:** Function App + Storage Account + Application Insights
- **Description:** Production Azure Functions for scheduled tasks and background jobs
- **Relationships:**
  - Uses storage account cisaciswcjobsprod for runtime
  - Monitored by Application Insights cisaciswcjobsprod
  - Alert: [Failure Anomalies - cisaciswcjobsprod](#failure-anomalies---cisaciswcjobsprod)
  - Alert: [cisaciswcjobsprod-IPI Scheduled Sync Failure](#cisaciswcjobsprod-ipi-scheduled-sync-failure)
  - Health Check Handle Count: [Health Check - cisaciswcjobsprod - App Service - Handle Count](#health-check---cisaciswcjobsprod---app-service---handle-count)
  - Health Check Response Time: [Health Check - cisaciswcjobsprod - App Service - Response Time](#health-check---cisaciswcjobsprod---app-service---response-time)

##### cisaciswcportalprod

- **Type:** App Service + Application Insights
- **Description:** User web portal in production
- **Relationships:**
  - Hosted on [ISWCProd](#iswcprod-app-service-plan) (App Service Plan)
  - Monitored by Application Insights
  - Alert: [Failure Anomalies - cisaciswcportalprod](#failure-anomalies---cisaciswcportalprod)
  - Health Check Handle Count: [Health Check - cisaciswcportalprod - App Service - Handle Count](#health-check---cisaciswcportalprod---app-service---handle-count)
  - Health Check Response Time: [Health Check - cisaciswcportalprod - App Service - Response Time](#health-check---cisaciswcportalprod---app-service---response-time)

##### cisaciswcpublicprod

- **Type:** App Service + Application Insights
- **Description:** ISWC public API in production
- **Relationships:**
  - Hosted on [ISWCProd](#iswcprod-app-service-plan) (App Service Plan)
  - Monitored by Application Insights
  - Alert: [Failure Anomalies - cisaciswcpublicprod](#failure-anomalies---cisaciswcpublicprod)
  - Health Check Handle Count: [Health Check - cisaciswcpublicprod - App Service - Handle Count](#health-check---cisaciswcpublicprod---app-service---handle-count)
  - Health Check Response Time: [Health Check - cisaciswcpublicprod - App Service - Response Time](#health-check---cisaciswcpublicprod---app-service---response-time)

#### Network Infrastructure

##### CISACAzPSFTP (Virtual Machine + components)

- **Type:** Linux/Windows Virtual Machine
- **Description:** SFTP server for production environment
- **Associated Components:**
  - Network interface: CISACAzPSFTP
  - Network security group: CISACAzPSFTP
  - Public IP address: CISACAzPSFTP
  - OS Disk: CISACAzPSFTP-OS
- **Relationships:**
  - Connected to network [CISACProd](#cisacprod)
- **Health Checks:**
  - [Health Check - CISACAzPSFTP - Virtual Machine - CPU Credits](#health-check---cisacazpsftp---virtual-machine---cpu-credits)
  - [Health Check - CISACAzPSFTP - Virtual Machine - Percentage CPU](#health-check---cisacazpsftp---virtual-machine---percentage-cpu)

#### Databricks

##### ISWCProd (Azure Databricks Service)

- **Type:** Azure Databricks Service
- **Description:** Production data analytics and Machine Learning platform
- **Relationships:**
  - Uses virtual network [workers-vnet](#workers-vnet-prod)
  - Security group: [workers-sg](#workers-sg-prod)
  - Storage account: [dbstorage7gfy2dooo4lw2](#dbstorage7gfy2dooo4lw2)
  - Jobs storage account: [cisaciswcdatabricksprod](#cisaciswcdatabricksprod)
  - Managed identity: [dbmanagedidentity](#dbmanagedidentity-prod)

##### Databricks Prod Virtual Machines

The following VMs are automatically managed by Databricks:

1. **202fcf371e214e0abf2face52263bc9b**
   - Private network interface: 202fcf371e214e0abf2face52263bc9b-privateNIC
   - Public network interface: 202fcf371e214e0abf2face52263bc9b-publicNIC
   - Public IP address: 202fcf371e214e0abf2face52263bc9b-publicIP
   - OS disk: 202fcf371e214e0abf2face52263bc9b-osDisk
   - Container disk: 202fcf371e214e0abf2face52263bc9b-containerRootVolume

2. **45697b8abee44d77b6d4ab0d453658cb**
   - Private network interface: 45697b8abee44d77b6d4ab0d453658cb-privateNIC
   - Public network interface: 45697b8abee44d77b6d4ab0d453658cb-publicNIC
   - Public IP address: 45697b8abee44d77b6d4ab0d453658cb-publicIP
   - OS disk: 45697b8abee44d77b6d4ab0d453658cb-osDisk
   - Container disk: 45697b8abee44d77b6d4ab0d453658cb-containerRootVolume

3. **4c7a8765fc514cfeb9eb7291296ad92d**
   - Private network interface: 4c7a8765fc514cfeb9eb7291296ad92d-privateNIC
   - Public network interface: 4c7a8765fc514cfeb9eb7291296ad92d-publicNIC
   - Public IP address: 4c7a8765fc514cfeb9eb7291296ad92d-publicIP
   - OS disk: 4c7a8765fc514cfeb9eb7291296ad92d-osDisk
   - Container disk: 4c7a8765fc514cfeb9eb7291296ad92d-containerRootVolume

4. **66fcbcb23b8b42c98d96f8037c76907a**
   - Private network interface: 66fcbcb23b8b42c98d96f8037c76907a-privateNIC
   - Public network interface: 66fcbcb23b8b42c98d96f8037c76907a-publicNIC
   - Public IP address: 66fcbcb23b8b42c98d96f8037c76907a-publicIP
   - OS disk: 66fcbcb23b8b42c98d96f8037c76907a-osDisk
   - Container disk: 66fcbcb23b8b42c98d96f8037c76907a-containerRootVolume

5. **a2e66feafb3f4ddcb05a50c28e52a9d9**
   - Private network interface: a2e66feafb3f4ddcb05a50c28e52a9d9-privateNIC
   - Public network interface: a2e66feafb3f4ddcb05a50c28e52a9d9-publicNIC
   - Public IP address: a2e66feafb3f4ddcb05a50c28e52a9d9-publicIP
   - OS disk: a2e66feafb3f4ddcb05a50c28e52a9d9-osDisk
   - Container disk: a2e66feafb3f4ddcb05a50c28e52a9d9-containerRootVolume

6. **abcf07817fff485ea23c48b8da8b525f**
   - Private network interface: abcf07817fff485ea23c48b8da8b525f-privateNIC
   - Public network interface: abcf07817fff485ea23c48b8da8b525f-publicNIC
   - Public IP address: abcf07817fff485ea23c48b8da8b525f-publicIP
   - OS disk: abcf07817fff485ea23c48b8da8b525f-osDisk
   - Container disk: abcf07817fff485ea23c48b8da8b525f-containerRootVolume

7. **c44d46f4811144fdb13edb6682be2307**
   - Private network interface: c44d46f4811144fdb13edb6682be2307-privateNIC
   - Public network interface: c44d46f4811144fdb13edb6682be2307-publicNIC
   - Public IP address: c44d46f4811144fdb13edb6682be2307-publicIP
   - OS disk: c44d46f4811144fdb13edb6682be2307-osDisk
   - Container disk: c44d46f4811144fdb13edb6682be2307-containerRootVolume

8. **cfa3955e7b364c85a18d70a30ab56bb2**
   - Private network interface: cfa3955e7b364c85a18d70a30ab56bb2-privateNIC
   - Public network interface: cfa3955e7b364c85a18d70a30ab56bb2-publicNIC
   - Public IP address: cfa3955e7b364c85a18d70a30ab56bb2-publicIP
   - OS disk: cfa3955e7b364c85a18d70a30ab56bb2-osDisk
   - Container disk: cfa3955e7b364c85a18d70a30ab56bb2-containerRootVolume

9. **d4bc2ab57a704bd2a8715d7aaa92deaa**
   - Private network interface: d4bc2ab57a704bd2a8715d7aaa92deaa-privateNIC
   - Public network interface: d4bc2ab57a704bd2a8715d7aaa92deaa-publicNIC
   - Public IP address: d4bc2ab57a704bd2a8715d7aaa92deaa-publicIP
   - OS disk: d4bc2ab57a704bd2a8715d7aaa92deaa-osDisk
   - Container disk: d4bc2ab57a704bd2a8715d7aaa92deaa-containerRootVolume

10. **e72e9c48e32542d5b83ea9c799cb5160**
    - Private network interface: e72e9c48e32542d5b83ea9c799cb5160-privateNIC
    - Public network interface: e72e9c48e32542d5b83ea9c799cb5160-publicNIC
    - Public IP address: e72e9c48e32542d5b83ea9c799cb5160-publicIP
    - OS disk: e72e9c48e32542d5b83ea9c799cb5160-osDisk
    - Container disk: e72e9c48e32542d5b83ea9c799cb5160-containerRootVolume

11. **fc9089d2537745088b79e1b8d5b1098b**
    - Private network interface: fc9089d2537745088b79e1b8d5b1098b-privateNIC
    - Public network interface: fc9089d2537745088b79e1b8d5b1098b-publicNIC
    - Public IP address: fc9089d2537745088b79e1b8d5b1098b-publicIP
    - OS disk: fc9089d2537745088b79e1b8d5b1098b-osDisk
    - Container disk: fc9089d2537745088b79e1b8d5b1098b-containerRootVolume

##### workers-vnet (Prod)

- **Type:** Virtual Network
- **Description:** Virtual network dedicated to Databricks Prod workers
- **Resource Group:** ISWCDatabricksProd

##### workers-sg (Prod)

- **Type:** Network Security Group
- **Description:** Security rules for Databricks Prod workers
- **Resource Group:** ISWCDatabricksProd

##### dbmanagedidentity (Prod)

- **Type:** Managed Identity
- **Description:** Identity for secure access to Azure Prod resources
- **Resource Group:** ISWCDatabricksProd

##### dbstorage7gfy2dooo4lw2

- **Type:** Storage Account
- **Description:** Storage for Databricks Prod
- **Resource Group:** ISWCDatabricksProd

##### cisaciswcdatabricksprod

- **Type:** Storage Account
- **Description:** Storage for Databricks Prod jobs
- **Resource Group:** ISWCProd

#### SFTP Storage

##### cisaciswcsftpprod

- **Type:** Storage Account
- **Description:** Storage for SFTP transfers in production
- **Relationships:**
  - Admin keys in [cisaciswcsftpadminprod](#cisaciswcsftpadminprod)
  - User keys in [cisaciswcsftpuserprod](#cisaciswcsftpuserprod)

##### cisaciswcsftpadminprod

- **Type:** Key Vault
- **Description:** SFTP Prod admin secrets

##### cisaciswcsftpuserprod

- **Type:** Key Vault
- **Description:** SFTP Prod user secrets

#### Database

##### cisaciswcweprod

- **Type:** SQL Server
- **Description:** Azure SQL Server for production environment
- **Relationships:**
  - Hosts database [ISWC](#iswc-prod)

##### ISWC (Prod)

- **Type:** SQL Database
- **Description:** Main ISWC Prod database
- **Resource Group:** ISWCProd
- **Relationships:**
  - Hosted on server [cisaciswcweprod](#cisaciswcweprod)
  - Used by all Prod App Services
- **Health Checks:**
  - [Health Check - ISWC - SQLDB - CPUPercentage](#health-check---iswc---sqldb---cpupercentage)
  - [Health Check - ISWC - SQLDB - Data IO](#health-check---iswc---sqldb---data-io)
  - [Health Check - ISWC - SQLDB - Data Space Used](#health-check---iswc---sqldb---data-space-used)
  - [Health Check - ISWC - SQLDB - Deadlock](#health-check---iswc---sqldb---deadlock)
  - [Health Check - ISWC - SQLDB - Failed Connections](#health-check---iswc---sqldb---failed-connections)
  - [Health Check - ISWC - SQLDB - Log IO](#health-check---iswc---sqldb---log-io)

#### Support Infrastructure

##### ISWCProd (App Service Plan)

- **Type:** App Service Plan
- **Description:** Hosting plan for Prod App Services
- **Relationships:**
  - Hosts all Prod environment App Services
- **Health Checks:**
  - [Health Check - ISWCProd - App Plan - CPU Percentage](#health-check---iswcprod---app-plan---cpu-percentage)
  - [Health Check - ISWCProd - App Plan - Memory Percentage](#health-check---iswcprod---app-plan---memory-percentage)

##### ISWCProd (Application Insights)

- **Type:** Application Insights
- **Description:** Global Prod environment monitoring
- **Relationships:**
  - Alert: [Failure Anomalies - ISWCProd](#failure-anomalies---iswcprod)

#### Dashboards and Monitoring

##### 615bf233-73d4-41db-9529-5aeeed7d7cde-dashboard

- **Type:** Shared Dashboard
- **Description:** Prod monitoring dashboard

##### f1cc2358-16d6-4464-ab1f-a6c4839889f9-dashboard

- **Type:** Shared Dashboard
- **Description:** Prod monitoring dashboard (secondary)

##### 7c2b865d-51f5-46ea-96de-418d3ddd3a9f

- **Type:** Azure Workbook
- **Description:** Azure workbook for advanced Prod analytics

#### Action Groups and Alerts

##### Health Check

- **Type:** Action Group
- **Description:** Action group for production health checks
- **Resource Group:** ManagementProd
- **Location:** Global

##### spanish point support

- **Type:** Action Group
- **Description:** Action group for Spanish Point support
- **Location:** Global

---

## Management Resources

### ManagementDev

##### cisacdev

- **Type:** Key Vault
- **Description:** Main key vault for Dev management

##### cisacdiagdev

- **Type:** Storage Account
- **Description:** Storage for Dev diagnostics

##### cisaciswcdev

- **Type:** Automation Account
- **Description:** Automation account for Dev environment
- **Relationships:**
  - Uses storage account [cisaciswcdevautomation](#cisaciswcdevautomation)
  - Contains runbooks: [ScaleAppServicePlan](#scaleappserviceplan-dev), [ScaleCosmoDB](#scalecosmodb-dev), [StartVM](#startvm-dev), [StopVM](#stopvm-dev)

##### cisaciswcdevautomation

- **Type:** Storage Account
- **Description:** Storage for Dev Automation account

#### Dev Runbooks

##### ScaleAppServicePlan (Dev)

- **Type:** Runbook
- **Description:** Automation script for scaling Dev App Service Plans
- **Resource Group:** ManagementDev

##### ScaleCosmoDB (Dev)

- **Type:** Runbook
- **Description:** Automation script for scaling Dev CosmosDB
- **Resource Group:** ManagementDev

##### StartVM (Dev)

- **Type:** Runbook
- **Description:** Automation script for starting Dev virtual machines
- **Resource Group:** ManagementDev

##### StopVM (Dev)

- **Type:** Runbook
- **Description:** Automation script for stopping Dev virtual machines
- **Resource Group:** ManagementDev

---

### ManagementUAT

##### cisacuat

- **Type:** Key Vault
- **Description:** Main key vault for UAT management

##### cisacdiaguat

- **Type:** Storage Account
- **Description:** Storage for UAT diagnostics

##### cisaciswcuat

- **Type:** Automation Account
- **Description:** Automation account for UAT environment
- **Relationships:**
  - Uses storage account [cisaciswcuatautomation](#cisaciswcuatautomation)
  - Contains runbooks: [ScaleAppServicePlan](#scaleappserviceplan-uat), [ScaleCosmoDB](#scalecosmodb-uat), [StartVM](#startvm-uat), [StopVM](#stopvm-uat)

##### cisaciswcuatautomation

- **Type:** Storage Account
- **Description:** Storage for UAT Automation account

#### UAT Runbooks

##### ScaleAppServicePlan (UAT)

- **Type:** Runbook
- **Description:** Automation script for scaling UAT App Service Plans
- **Resource Group:** ManagementUAT

##### ScaleCosmoDB (UAT)

- **Type:** Runbook
- **Description:** Automation script for scaling UAT CosmosDB
- **Resource Group:** ManagementUAT

##### StartVM (UAT)

- **Type:** Runbook
- **Description:** Automation script for starting UAT virtual machines
- **Resource Group:** ManagementUAT

##### StopVM (UAT)

- **Type:** Runbook
- **Description:** Automation script for stopping UAT virtual machines
- **Resource Group:** ManagementUAT

---

### ManagementProd

##### cisacprod

- **Type:** Key Vault
- **Description:** Main key vault for Prod management

##### cisacdiagprod

- **Type:** Storage Account
- **Description:** Storage for Prod diagnostics

##### cisaciswcprod

- **Type:** Automation Account
- **Description:** Automation account for Prod environment
- **Relationships:**
  - Uses storage account [cisaciswcprodautomation](#cisaciswcprodautomation)
  - Contains runbooks: [ScaleAppServicePlan](#scaleappserviceplan-prod), [ScaleCosmoDB](#scalecosmodb-prod), [StartVM](#startvm-prod), [StopVM](#stopvm-prod)

##### cisaciswcprodautomation

- **Type:** Storage Account
- **Description:** Storage for Prod Automation account

#### Prod Runbooks

##### ScaleAppServicePlan (Prod)

- **Type:** Runbook
- **Description:** Automation script for scaling Prod App Service Plans
- **Resource Group:** ManagementProd

##### ScaleCosmoDB (Prod)

- **Type:** Runbook
- **Description:** Automation script for scaling Prod CosmosDB
- **Resource Group:** ManagementProd

##### StartVM (Prod)

- **Type:** Runbook
- **Description:** Automation script for starting Prod virtual machines
- **Resource Group:** ManagementProd

##### StopVM (Prod)

- **Type:** Runbook
- **Description:** Automation script for stopping Prod virtual machines
- **Resource Group:** ManagementProd

#### Log Search Alert Rules (Prod)

##### cisaciswcapiprod-POST SearchByTitleAndContributor

- **Type:** Log search alert rule
- **Description:** Alert on API POST SearchByTitleAndContributor calls
- **Resource Group:** ManagementProd
- **Relationships:**
  - Monitors App Service [cisaciswcapiprod](#cisaciswcapiprod)

##### cisaciswcprod - EDI failures

- **Type:** Log search alert rule
- **Description:** Alert on EDI (Electronic Data Interchange) failures
- **Resource Group:** ManagementProd
- **Relationships:**
  - Monitors [cisaciswcprod](#cisaciswcprod-application-insights)

##### cisaciswcprod - REST API Errors

- **Type:** Log search alert rule
- **Description:** Alert on REST API errors
- **Resource Group:** ManagementProd
- **Relationships:**
  - Monitors [cisaciswcprod](#cisaciswcprod-application-insights)

##### cisaciswcprod-APIManagement Failures

- **Type:** Log search alert rule
- **Description:** Alert on API Management failures
- **Resource Group:** ManagementProd
- **Relationships:**
  - Monitors service [cisaciswcprod](#cisaciswcprod-multi-service) (API Management)

##### cisaciswcjobsprod-IPI Scheduled Sync Failure

- **Type:** Log search alert rule
- **Description:** Alert on IPI scheduled sync failures
- **Resource Group:** ManagementProd
- **Relationships:**
  - Monitors function app [cisaciswcjobsprod](#cisaciswcjobsprod)

##### cisaciswcapilabelprod - POST AddLabelSubmissionBatch

- **Type:** Log search alert rule
- **Description:** Alert on API POST AddLabelSubmissionBatch calls
- **Resource Group:** ISWCProd
- **Relationships:**
  - Monitors App Service [cisaciswcapilabelprod](#cisaciswcapilabelprod)

#### Health Check Metric Alert Rules (Prod)

##### Health Check - CISACAzPSFTP - Virtual Machine - CPU Credits

- **Type:** Metric alert rule
- **Description:** Monitors CPU credits of Prod SFTP VM
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors VM [CISACAzPSFTP](#cisacazpsftp-virtual-machine--components)

##### Health Check - CISACAzPSFTP - Virtual Machine - Percentage CPU

- **Type:** Metric alert rule
- **Description:** Monitors CPU percentage of Prod SFTP VM
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors VM [CISACAzPSFTP](#cisacazpsftp-virtual-machine--components)

##### Health Check - cisaciswcapiprod - App Service - Handle Count

- **Type:** Metric alert rule
- **Description:** Monitors handle count of Prod API
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors App Service [cisaciswcapiprod](#cisaciswcapiprod)

##### Health Check - cisaciswcapiprod - App Service - Response Time

- **Type:** Metric alert rule
- **Description:** Monitors response time of Prod API
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors App Service [cisaciswcapiprod](#cisaciswcapiprod)

##### Health Check - cisaciswcjobsprod - App Service - Handle Count

- **Type:** Metric alert rule
- **Description:** Monitors handle count of Prod Functions
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors function app [cisaciswcjobsprod](#cisaciswcjobsprod)

##### Health Check - cisaciswcjobsprod - App Service - Response Time

- **Type:** Metric alert rule
- **Description:** Monitors response time of Prod Functions
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors function app [cisaciswcjobsprod](#cisaciswcjobsprod)

##### Health Check - cisaciswcportalprod - App Service - Handle Count

- **Type:** Metric alert rule
- **Description:** Monitors handle count of Prod Portal
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors App Service [cisaciswcportalprod](#cisaciswcportalprod)

##### Health Check - cisaciswcportalprod - App Service - Response Time

- **Type:** Metric alert rule
- **Description:** Monitors response time of Prod Portal
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors App Service [cisaciswcportalprod](#cisaciswcportalprod)

##### Health Check - cisaciswcprod - API Management Service - Capacity

- **Type:** Metric alert rule
- **Description:** Monitors capacity of Prod API Management service
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors service [cisaciswcprod](#cisaciswcprod-multi-service) (API Management)

##### Health Check - cisaciswcprod - CosmosDB - RU Consumption

- **Type:** Metric alert rule
- **Description:** Monitors RU (Request Units) consumption of Prod CosmosDB
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors [cisaciswcprod](#cisaciswcprod-multi-service) (Cosmos DB)

##### Health Check - cisaciswcpublicprod - App Service - Handle Count

- **Type:** Metric alert rule
- **Description:** Monitors handle count of Prod public API
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors App Service [cisaciswcpublicprod](#cisaciswcpublicprod)

##### Health Check - cisaciswcpublicprod - App Service - Response Time

- **Type:** Metric alert rule
- **Description:** Monitors response time of Prod public API
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors App Service [cisaciswcpublicprod](#cisaciswcpublicprod)

##### Health Check - ISWC - SQLDB - CPUPercentage

- **Type:** Metric alert rule
- **Description:** Monitors CPU percentage of SQL database
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors database [ISWC](#iswc-prod)

##### Health Check - ISWC - SQLDB - Data IO

- **Type:** Metric alert rule
- **Description:** Monitors data I/O of SQL database
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors database [ISWC](#iswc-prod)

##### Health Check - ISWC - SQLDB - Data Space Used

- **Type:** Metric alert rule
- **Description:** Monitors disk space used by SQL database
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors database [ISWC](#iswc-prod)

##### Health Check - ISWC - SQLDB - Deadlock

- **Type:** Metric alert rule
- **Description:** Monitors deadlocks in SQL database
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors database [ISWC](#iswc-prod)

##### Health Check - ISWC - SQLDB - Failed Connections

- **Type:** Metric alert rule
- **Description:** Monitors failed connections to SQL database
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors database [ISWC](#iswc-prod)

##### Health Check - ISWC - SQLDB - Log IO

- **Type:** Metric alert rule
- **Description:** Monitors log I/O of SQL database
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors database [ISWC](#iswc-prod)

##### Health Check - ISWCProd - App Plan - CPU Percentage

- **Type:** Metric alert rule
- **Description:** Monitors CPU percentage of Prod App Service Plan
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors App Service Plan [ISWCProd](#iswcprod-app-service-plan)

##### Health Check - ISWCProd - App Plan - Memory Percentage

- **Type:** Metric alert rule
- **Description:** Monitors memory percentage of Prod App Service Plan
- **Resource Group:** ManagementProd
- **Location:** Global
- **Relationships:**
  - Uses action group [Health Check](#health-check)
  - Monitors App Service Plan [ISWCProd](#iswcprod-app-service-plan)

---

## Core Resources

### CoreDev

##### CISACDev

- **Type:** Virtual Network
- **Description:** Main virtual network for Dev environment
- **Relationships:**
  - Used by all Dev environment services
  - Connected to VM [CISACAzDSFTP](#cisacazdsft-virtual-machine--components)

---

### CoreUAT

##### CISACUAT

- **Type:** Virtual Network
- **Description:** Main virtual network for UAT environment
- **Relationships:**
  - Used by all UAT environment services
  - Connected to VM [CISACAzUSFTP](#cisacazusftp-virtual-machine--components)

---

### CoreProd

##### CISACProd

- **Type:** Virtual Network
- **Description:** Main virtual network for Prod environment
- **Relationships:**
  - Used by all Prod environment services
  - Connected to VM [CISACAzPSFTP](#cisacazpsftp-virtual-machine--components)

---

## Global Resources

The following resources have a Global location and apply to multiple environments:

### Smart Detection Alerts (Failure Anomalies)

All alerts below are of type "smart detector alert rule" and have global scope.

#### Dev

- **Failure Anomalies - cisaciswcapidev** (ISWCDev)
- **Failure Anomalies - cisaciswcapilabeldev** (ISWCDev)
- **Failure Anomalies - cisaciswcapipublisherdev** (ISWCDev)
- **Failure Anomalies - cisaciswcapithirdpartydev** (ISWCDev)
- **Failure Anomalies - cisaciswcdev** (ISWCDev)
- **Failure Anomalies - cisaciswcjobsdev** (ISWCDev)
- **Failure Anomalies - cisaciswcportaldev** (ISWCDev)
- **Failure Anomalies - cisaciswcpublicdev** (ISWCDev)
- **Failure Anomalies - cisacspmedev** (ISWCDev)
- **Failure Anomalies - ISWCDev** (ISWCDev)

#### UAT

- **Failure Anomalies - cisaciswcapilabeluat** (ISWCUAT)
- **Failure Anomalies - cisaciswcapipublisheruat** (ISWCUAT)
- **Failure Anomalies - cisaciswcapithirdpartyuat** (ISWCUAT)
- **Failure Anomalies - cisaciswcapiuat** (ISWCUAT)
- **Failure Anomalies - cisaciswcjobsuat** (ISWCUAT)
- **Failure Anomalies - cisaciswcportaluat** (ISWCUAT)
- **Failure Anomalies - cisaciswcpublicuat** (ISWCUAT)
- **Failure Anomalies - cisaciswcuat** (ISWCUAT)
- **Failure Anomalies - cisacspmeuat** (ISWCUAT)
- **Failure Anomalies - ISWCUAT** (ISWCUAT)

#### Prod

- **Failure Anomalies - cisaciswcapilabelprod** (ISWCProd)
- **Failure Anomalies - cisaciswcapiprod** (ISWCProd)
- **Failure Anomalies - cisaciswcapipublisherprod** (ISWCProd)
- **Failure Anomalies - cisaciswcapithirdpartyprod** (ISWCProd)
- **Failure Anomalies - cisaciswcjobsprod** (ISWCProd)
- **Failure Anomalies - cisaciswcportalprod** (ISWCProd)
- **Failure Anomalies - cisaciswcprod** (ISWCProd)
- **Failure Anomalies - cisaciswcpublicprod** (ISWCProd)
- **Failure Anomalies - ISWCProd** (ISWCProd)

### Availability Tests and Alerts

#### Prod

##### root-cisaciswcapiprod (Metric alert rule)

- **Type:** Metric alert rule
- **Resource Group:** ISWCProd
- **Location:** Global
- **Relationships:**
  - Associated with availability test [root-cisaciswcapiprod](#root-cisaciswcapiprod-availability-test)
  - Monitors App Service [cisaciswcapiprod](#cisaciswcapiprod)

##### root-cisaciswcapiprod (Availability test)

- **Type:** Availability test
- **Resource Group:** ISWCProd
- **Location:** West Europe
- **Relationships:**
  - Availability test for [cisaciswcapiprod](#cisaciswcapiprod)
  - Triggers alert [root-cisaciswcapiprod](#root-cisaciswcapiprod-metric-alert-rule)

---

## Resource Types Glossary

### Action Group

Set of notifications and actions triggered by Azure Monitor alerts. Can include emails, SMS, webhooks, Azure functions, etc.

### API Management Service

Complete API gateway for publishing, securing, transforming, maintaining, and monitoring APIs. Offers developer portal, rate limiting, authentication, etc.

### App Service

Managed service for hosting web applications, REST APIs, and mobile backends. Supports multiple languages (.NET, Java, Node.js, Python, PHP, Ruby).

### App Service Plan

Underlying infrastructure that defines compute resources (CPU, memory, storage) for hosting App Services. Determines scaling capabilities and available features.

### App Service Slot

Secondary deployment environment (slot) for an App Service, enabling zero-downtime deployments (blue-green deployment) and production testing.

### Application Insights

Application Performance Management (APM) monitoring and analytics service. Collects telemetry, logs, traces, and metrics to diagnose issues and understand application behavior.

### Automation Account

Service for automating repetitive tasks and configuration management. Hosts PowerShell or Python runbooks to orchestrate processes.

### Availability Test

Automated test verifying availability and responsiveness of a web application from multiple geographic locations. Generates alerts on failure.

### Azure Cosmos DB Account

Multi-model, globally distributed NoSQL database service with guaranteed low latency. Supports multiple APIs (SQL, MongoDB, Cassandra, Gremlin, Table).

### Azure Databricks Service

Data analytics platform based on Apache Spark, optimized for Azure. Used for Big Data, Machine Learning, and advanced analytics.

### Azure Workbook

Interactive reporting tool for creating rich reports combining text, analytics queries, metrics, and parameters.

### Data Factory (V2)

Cloud data integration service for creating, scheduling, and orchestrating ETL (Extract, Transform, Load) and ELT pipelines at scale.

### Disk

Persistent block storage for Azure virtual machines. Available in multiple types: Premium SSD, Standard SSD, Standard HDD.

### Function App

Serverless service for executing code on-demand without managing infrastructure. Ideal for scheduled tasks, event processing, and system integration.

### Key Vault

Service for secure management of secrets, encryption keys, and certificates. Enables centralized access control and automatic secret rotation.

### Log Search Alert Rule

Alert based on KQL (Kusto Query Language) queries executed on Azure Monitor logs or Application Insights.

### Managed Identity

Identity automatically managed by Azure allowing Azure services to authenticate to other services without storing credentials in code.

### Metric Alert Rule

Alert triggered when an Azure metric exceeds a defined threshold (e.g., CPU > 80%, memory > 90%).

### Network Interface

Virtual network card enabling an Azure virtual machine to communicate with Internet, Azure, and on-premises resources.

### Network Security Group (NSG)

Virtual firewall containing security rules to filter inbound and outbound network traffic of Azure resources in a virtual network.

### Public IP Address

Publicly routed IP address enabling Azure resources to communicate with the Internet. Primarily used for virtual machines and load balancers.

### Recovery Services Vault

Backup and disaster recovery service for virtual machines, files, databases, and other Azure resources.

### Runbook

Automated script hosted in Azure Automation to perform operational tasks (VM start/stop, scaling, maintenance).

### Shared Dashboard

Customizable Azure dashboard displaying visualizations of metrics, logs, and resource status. Can be shared between users.

### Smart Detector Alert Rule

Machine learning-based alert that automatically detects anomalies in telemetry patterns (e.g., sudden spike in exceptions).

### SQL Database

Relational database as a service (PaaS) based on Microsoft SQL Server. Offers high availability, automatic backups, and elastic scaling.

### SQL Server

Logical server hosting one or more Azure SQL databases. Provides centralized connection point and common security settings.

### Storage Account

Versatile cloud storage service offering blobs (objects), files, queues, and tables. Supports different performance tiers and redundancy levels.

### Virtual Machine

Scalable virtual server in Azure cloud. Supports Windows and Linux with complete control over the operating system.

### Virtual Network (VNet)

Logically isolated network in Azure enabling secure communication between Azure resources, Internet, and on-premises networks.

---

## Important Notes

### Naming Conventions

- **Environment suffix:** Resources are suffixed with `dev`, `uat`, or `prod` depending on environment
- **CISAC prefix:** Most resources begin with `cisac` or `CISAC`
- **ISWC:** Refers to the International Standard Work Code project

### Databricks

- Databricks virtual machines are created and managed automatically by Azure Databricks service
- Each VM has up to 5 associated resources: private/public interface, public IP, OS disk, container disk
- Resources are named with automatically generated hexadecimal identifiers

### SFTP

- Each environment has its own SFTP server with 3 associated key vaults
- Architecture: VM + NSG + public IP + storage account + 2 key vaults (admin/user)

### Monitoring

- Application Insights is deployed at multiple levels: per service AND globally per environment
- "Failure Anomalies" alerts use AI to automatically detect anomalies
- Prod environment has a complete "Health Checks" system via metric alerts

### Automation

- Each environment has 4 standard runbooks: ScaleAppServicePlan, ScaleCosmoDB, StartVM, StopVM
- Runbooks enable infrastructure management automation

---

**Document generated on:** October 21, 2025
**Source:** [Azureresources-export-20251021.csv](../resources/Azureresources-export-20251021.csv)
