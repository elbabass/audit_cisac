# Annex C: Validation Rules Reference

[← Back to Annexes Index](./index.md) | [← Back to Executive Summary](../executive-summary.md)

## 95+ Validation Rules Documented

A comprehensive catalog of validation rules has been created during the audit. These rules govern work submission quality and data integrity.

**Source:** `docs/work_in_progress/architecture/` - Validation Pipeline documentation

## Rule Categories

### 1. Pre-submission Validation

Format validation, required fields, data type checking

### 2. Creator Validation

IPI integration, creator existence, rights verification

### 3. Work Metadata Validation

Title, ISWC format, work type, territory restrictions

### 4. Format-Specific Validation

EDI format rules, JSON schema validation

### 5. Business Logic Validation

Duplicate detection prep, consistency checks

## Examples (Representative Sample)

- **Work Title Validation:** Must not be empty, max 512 characters, special character restrictions
- **ISWC Format Validation:** T-123456789-C format enforcement
- **Creator IPI Validation:** IPI number must exist in authoritative database, creator role must be valid
- **Work Duration Validation:** If provided, must be positive integer, format HH:MM:SS
- **Territory Validation:** ISO country codes only, territory conflicts checked

**Note:** Full catalog available in architecture documentation. Rules are enforced at API level before submission to Matching Engine.

---

[← Back to Annexes Index](./index.md) | [← Back to Executive Summary](../executive-summary.md)
