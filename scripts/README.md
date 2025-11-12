# Scripts

Utility scripts for the CISAC ISWC audit project.

## extract-diagrams.sh

Extracts and converts Mermaid diagrams from presentation markdown files to high-quality PNG images.

### Requirements

- Node.js and npm
- @mermaid-js/mermaid-cli package

Install the CLI globally:

```bash
npm install -g @mermaid-js/mermaid-cli
```

### Usage

```bash
./scripts/extract-diagrams.sh <markdown-file>
```

### Example

```bash
# Extract diagrams from part 5
./scripts/extract-diagrams.sh docs/deliverables/first-restitution-2025-11-24/presentation/part5-vendor-lock-in.md

# Extract diagrams from part 6
./scripts/extract-diagrams.sh docs/deliverables/first-restitution-2025-11-24/presentation/part6-strategic-recommendations.md
```

### Output

The script:

1. Creates a `diagrams/` subfolder in the same directory as the markdown file
2. Extracts all Mermaid code blocks from the markdown
3. Converts each diagram to PNG with high-quality settings:
   - Width: 2400px
   - Scale: 3x
   - Transparent background
4. Names files with the pattern: `[filename]_[order]_slide-[number]_[slug].png`

### File Naming Pattern

- `[filename]`: Base name of the markdown file (e.g., `part5-vendor-lock-in`)
- `[order]`: Sequential number (1, 2, 3, etc.)
- `[slide-number]`: Extracted from section headings like "### Slide 18: Title"
- `[slug]`: URL-friendly version of the section title

### Example Output

For `part5-vendor-lock-in.md`:

```text
diagrams/
├── part5-vendor-lock-in_1_slide-18_three-lock-in-mechanisms.png
├── part5-vendor-lock-in_2_slide-19_matching-engine-deep-dive-heart-of-the-product.png
├── part5-vendor-lock-in_3_slide-20_what-we-can-t-access-visibility-gaps.png
├── part5-vendor-lock-in_4_slide-20_what-we-can-t-access-visibility-gaps.png
└── part5-vendor-lock-in_5_slide-21_vendor-switch-effort-estimate-preliminary-assessment.png
```

### Features

- Automatic slide number extraction from headings
- Descriptive slug generation from section titles
- High-quality PNG output (readable text at large sizes)
- Transparent backgrounds for easy insertion into presentations
- Color-coded console output for easy monitoring
- Automatic cleanup of temporary files

### Troubleshooting

**Error: "mmdc: command not found"**

Install mermaid-cli:

```bash
npm install -g @mermaid-js/mermaid-cli
```

**Error: "bad interpreter: /bin/bash^M"**

The script has Windows line endings. Fix with:

```bash
sed -i '' 's/\r$//' scripts/extract-diagrams.sh
```
