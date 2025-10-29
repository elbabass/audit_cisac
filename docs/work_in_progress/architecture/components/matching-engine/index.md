# Matching Engine Components

This directory contains documentation for the Matching Engine system - an external Spanish Point product integrated with the ISWC Platform.

## Components

- [Matching Engine](matching-engine.md) - External work matching and search platform (v1.0)
  - ME Portal - Web interface for matching operations (vendor-managed)
  - ME API - REST API for work matching (HTTP integration)
  - Search Service - Azure Cognitive Search indexing

## System Status

**Type:** External System
**Owner:** Spanish Point Technologies
**Integration:** HTTP REST API
**Access:** ISWC Platform calls Matching Engine API for work duplicate detection

## Related Documentation

- [C4 Level 1: System Context](../../c4-views/level1-system-context.md) - Matching Engine shown as external system
- [C4 Level 2: Container View](../../c4-views/level2-containers.md) - Integration patterns

---

**Note:** Matching Engine is modeled as an external system because:

- Separate codebase (no source code in ISWC repository)
- HTTP API integration pattern
- Spanish Point product (vendor dependency)
- Could theoretically be replaced with alternative matching solution
