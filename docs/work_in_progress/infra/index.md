# CISAC Infrastructure Documentation

This directory contains comprehensive documentation of the CISAC Azure infrastructure, including reference documentation and various architectural diagrams.

## Overview

The CISAC infrastructure is deployed on Microsoft Azure and consists of three main environments:

- **Dev** (Development)
- **UAT** (User Acceptance Testing)
- **Prod** (Production)

The platform includes several key systems:

- ISWC Platform (work registration and management)
- Matching Engine (work matching and search)
- Data Platform (data processing and analytics)
- Networking Infrastructure

## Reference Documentation

- [Azure Infrastructure Reference](infrastructure-azure-reference.md) - Complete inventory of all 343 Azure resources across all environments

## Architecture Diagrams

These diagrams were first generated using [diagram provided by Spanish Point](../../resources/InfrastructureDiagram.png)

### Overview Diagrams

High-level architecture representations showing the complete system:

- [Mermaid Diagram](overview/infrastructure-diagram-mermaid.md) - Interactive visual diagram using Mermaid syntax
- [Text Diagram](overview/infrastructure-diagram-text.md) - ASCII-based architecture diagram for text-based viewing

### Structurizr C4 Model

The infrastructure is documented using the C4 model methodology via Structurizr:

- [Structurizr DSL Source](overview/infrastructure-diagram-structurizr.dsl) - Complete C4 model definition in Structurizr DSL format

#### Generated Diagrams from Structurizr

Individual component diagrams generated from the Structurizr model:

- [System Landscape](overview/structurizr-SystemLandscape.mmd) - Top-level view of all systems and their relationships
- [System Context](overview/structurizr-SystemContext.mmd) - Context diagram showing system boundaries
- [Deployment Diagram](overview/structurizr-Deployment-001.mmd) - Deployment architecture across environments
- [ISWC Platform Containers](overview/structurizr-ISWCPlatformContainers.mmd) - Detailed view of ISWC Platform components
- [Data Platform Containers](overview/structurizr-DataPlatformContainers.mmd) - Data platform architecture components
- [Matching Engine Containers](overview/structurizr-MatchingEngineContainers.mmd) - Matching engine system components
- [Networking Containers](overview/structurizr-NetworkingContainers.mmd) - Network infrastructure components

## Navigation

- [Back to Work in Progress Index](../index.md)
- [Main Documentation](../../index.md)

---

*Last updated: October 2025*
