#!/bin/bash

# extract-diagrams.sh - Extract and convert Mermaid diagrams from presentation markdown files
#
# Usage: ./extract-diagrams.sh <markdown-file>
#
# This script:
# 1. Extracts all Mermaid diagrams from a markdown file
# 2. Converts them to high-quality PNG files
# 3. Names them with the pattern: [filename]_[order]_slide-[number]_[slug].png
#
# Requirements:
# - @mermaid-js/mermaid-cli (npm install -g @mermaid-js/mermaid-cli)
#
# Example:
# ./extract-diagrams.sh ../docs/deliverables/first-restitution-2025-11-24/presentation/part5-vendor-lock-in.md

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if mmdc is installed
if ! command -v mmdc &> /dev/null; then
    echo -e "${RED}Error: mermaid-cli (mmdc) is not installed${NC}"
    echo "Install it with: npm install -g @mermaid-js/mermaid-cli"
    exit 1
fi

# Check arguments
if [ "$#" -ne 1 ]; then
    echo -e "${RED}Usage: $0 <markdown-file>${NC}"
    echo "Example: $0 part5-vendor-lock-in.md"
    exit 1
fi

MARKDOWN_FILE="$1"

# Check if file exists
if [ ! -f "$MARKDOWN_FILE" ]; then
    echo -e "${RED}Error: File not found: $MARKDOWN_FILE${NC}"
    exit 1
fi

# Get the directory of the markdown file and create diagrams subfolder
MARKDOWN_DIR=$(dirname "$MARKDOWN_FILE")
DIAGRAMS_DIR="$MARKDOWN_DIR/diagrams"
mkdir -p "$DIAGRAMS_DIR"

# Extract base name (e.g., "part5-vendor-lock-in" from "part5-vendor-lock-in.md")
BASE_NAME=$(basename "$MARKDOWN_FILE" .md)

echo -e "${GREEN}Extracting diagrams from: $MARKDOWN_FILE${NC}"
echo -e "${GREEN}Output directory: $DIAGRAMS_DIR${NC}"
echo ""

# Temporary directory for mermaid files
TEMP_DIR=$(mktemp -d)
trap "rm -rf $TEMP_DIR" EXIT

# Parse the markdown file to extract diagrams
diagram_count=0
current_slide=""
in_mermaid=false
mermaid_content=""
current_section=""

while IFS= read -r line; do
    # Extract slide number from headings like "### Slide 18: Title"
    if [[ $line =~ ^###[[:space:]]+Slide[[:space:]]+([0-9]+) ]]; then
        current_slide="${BASH_REMATCH[1]}"
        # Extract the section title after "Slide XX: "
        current_section=$(echo "$line" | sed -E 's/^###[[:space:]]+Slide[[:space:]]+[0-9]+:[[:space:]]*//')
    fi

    # Detect start of mermaid block
    if [[ $line =~ ^\`\`\`mermaid ]]; then
        in_mermaid=true
        mermaid_content=""
        continue
    fi

    # Detect end of mermaid block
    if [[ $in_mermaid == true ]] && [[ $line =~ ^\`\`\` ]]; then
        in_mermaid=false
        diagram_count=$((diagram_count + 1))

        # Create slug from section title
        slug=$(echo "$current_section" | tr '[:upper:]' '[:lower:]' | sed -E 's/[^a-z0-9]+/-/g' | sed -E 's/^-+|-+$//g')

        # Create temporary mermaid file
        temp_mermaid="$TEMP_DIR/diagram${diagram_count}.mmd"
        echo "$mermaid_content" > "$temp_mermaid"

        # Generate output filename
        if [ -n "$current_slide" ]; then
            output_file="$DIAGRAMS_DIR/${BASE_NAME}_${diagram_count}_slide-${current_slide}_${slug}.png"
        else
            output_file="$DIAGRAMS_DIR/${BASE_NAME}_${diagram_count}_${slug}.png"
        fi

        echo -e "${YELLOW}Converting diagram ${diagram_count}: ${slug}${NC}"

        # Convert to PNG with high quality settings
        mmdc -i "$temp_mermaid" -o "$output_file" -b transparent -w 2400 -s 3

        if [ -f "$output_file" ]; then
            file_size=$(ls -lh "$output_file" | awk '{print $5}')
            echo -e "${GREEN}✓ Created: $(basename "$output_file") (${file_size})${NC}"
        else
            echo -e "${RED}✗ Failed to create: $(basename "$output_file")${NC}"
        fi
        echo ""

        continue
    fi

    # Collect mermaid content
    if [[ $in_mermaid == true ]]; then
        mermaid_content+="$line"$'\n'
    fi

done < "$MARKDOWN_FILE"

# Summary
echo -e "${GREEN}===========================================${NC}"
echo -e "${GREEN}Extraction complete!${NC}"
echo -e "${GREEN}Total diagrams extracted: ${diagram_count}${NC}"
echo -e "${GREEN}Output directory: $DIAGRAMS_DIR${NC}"
echo -e "${GREEN}===========================================${NC}"

# List generated files
if [ $diagram_count -gt 0 ]; then
    echo ""
    echo "Generated files:"
    ls -lh "$DIAGRAMS_DIR"/${BASE_NAME}_*.png 2>/dev/null | awk '{print "  " $9 " (" $5 ")"}'
fi
