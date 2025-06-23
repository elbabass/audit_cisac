---
description: "Guidelines for creating and maintaining project documentation"
globs: ["**/*.md"]
alwaysApply: false
---

# Documentation Rules

## General Documentation Guidelines

1. Always maintain consistency between related documents
2. Use markdown format for all documentation files
3. Follow existing document structure and formatting
4. Include clear section headers and descriptions
5. Add links between related documents when necessary
6. Ensure proper markdown formatting with line breaks before and after titles, lists, and tables
7. Include a link back to the nearest index file at the top of each document

## Creating New Documentation

1. Create the new markdown file in the appropriate docs/ subdirectory
2. Follow the naming convention:
   - Use numerical prefixes (e.g., `001-`, `002-`) for ordering
   - Use kebab-case for file names
3. Update docs/index.md with:
   - Link to the new document
   - Brief description of its contents
   - Update directory structure if needed
4. Add references to related documents
5. Follow existing documentation patterns
6. Create an index.md file for each new directory
7. Add the back to index link at the top of each document using the format: *[ Retour à l\'index](../index.md)*`

## Diagram Management

For detailed diagram creation standards and guidelines, refer to the [Diagram Creation Standards](mdc:.cursor/rules/diagrams.mdc) document.

Key points for diagram documentation:

1. Store diagrams in a dedicated `diagrams/` subdirectory within their respective documentation category
2. For each diagram, create a companion markdown file with the same name
3. Reference diagrams using direct links rather than embedding them
4. Avoid using the `{{...}}` syntax as it may not render properly in all viewers

## Updating Documentation

1. Check docs/index.md first to understand document relationships
2. Update all related documents to maintain consistency
3. Update docs/index.md if changes affect:
   - Document structure
   - Document relationships
   - Project organization
4. Add TODO items in docs/index.md for pending documentation needs

## Document Cross-Referencing

1. Use relative paths for links between documents
2. Format: `[Document Title](relative/path/to/document.md)`
3. Check and update all affected cross-references
4. Maintain bidirectional links where appropriate

## Documentation Structure

1. Each document should have:
   - Clear title (H1)
   - Brief introduction/purpose
   - Logical section hierarchy
   - Links to related documents
   - Back to index link at the top, right after the level one heading: `*[➡️ Retour à l\'index](../index.md)*`
2. Use consistent heading levels (H1 > H2 > H3)
3. Include code blocks with proper language tags
4. Add file paths for code-related documentation
5. Always include a blank line before and after:
   - Headings
   - Lists
   - Tables
   - Code blocks

## Markdown Formatting Rules

1. Always include a blank line before and after:
   - Headings (H1, H2, H3, etc.)
   - Lists (ordered and unordered)
   - Tables
   - Code blocks
2. Do not use tab characters for indentation; use spaces
3. Use proper list continuation (indent content under list items by 2 or 3 spaces)
4. Use proper code block formatting with language specification, e.g., ```javascript
5. Keep line length reasonable (typically < 120 characters)

## Documentation Directory Structure

```
docs/
 index.md              # Project Overview
 features/             # Feature Documentation
    index.md          # Features Index
    diagrams/         # Feature-specific diagrams
        index.md      # Diagrams Index
 development/          # Development Guidelines
    index.md          # Development Index
│   └── diagrams/         # Development-related diagrams
│       └── index.md      # Diagrams Index
 architecture/         # Architecture Documentation
     index.md          # Architecture Index
    └── diagrams/         # Architecture diagrams
         index.md      # Diagrams Index
```

### Infrastructure Documentation

- Location: infra/docs/index.md
- Include deployment procedures
- Document security practices
- Store infrastructure diagrams in infra/docs/diagrams/
- Ensure each subdirectory has its own index.md file

### Application Documentation

- Location: apps/docs/index.md
- Include development guidelines
- Define testing requirements
- Maintain Definition of Done
- Store application diagrams in apps/docs/diagrams/
- Ensure each subdirectory has its own index.md file

### Services Documentation

- Location: services/docs/index.md
- Document service architecture
- Include API documentation
- Define development workflow
- Store service diagrams in services/docs/diagrams/
- Ensure each subdirectory has its own index.md file
