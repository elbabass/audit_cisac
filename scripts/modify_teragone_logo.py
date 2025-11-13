#!/usr/bin/env python3
"""
Modify Teragone logo: replace 'solutions' with 'factory'
Creates a PNG version suitable for PowerPoint embedding.
"""

from PIL import Image
import os

# Paths
BASE_DIR = "/Users/bastiengallay/Dev/clients/CISAC"
LOGO_INPUT = f"{BASE_DIR}/docs/resources/Blanc_Teragone-logo.webp"
LOGO_OUTPUT = f"{BASE_DIR}/docs/resources/Blanc_Teragone-Factory-logo.png"

def convert_logo():
    """
    Convert Teragone WebP logo to PNG.
    Note: Text replacement requires manual editing or advanced image processing.
    This script will convert the format and prepare for manual editing if needed.
    """
    try:
        # Open the WebP logo
        img = Image.open(LOGO_INPUT)

        # Convert to RGBA if not already (for transparency support)
        if img.mode != 'RGBA':
            img = img.convert('RGBA')

        # Get image info
        width, height = img.size

        print(f"✓ Loaded Teragone logo:")
        print(f"  - Size: {width}x{height} px")
        print(f"  - Mode: {img.mode}")

        # Save as PNG with high quality
        img.save(LOGO_OUTPUT, 'PNG', quality=95, optimize=True)

        print(f"\n✓ Logo converted and saved:")
        print(f"  - Output: {LOGO_OUTPUT}")
        print(f"  - Format: PNG with transparency")

        # Get file sizes
        input_size = os.path.getsize(LOGO_INPUT) / 1024
        output_size = os.path.getsize(LOGO_OUTPUT) / 1024

        print(f"\n  File sizes:")
        print(f"  - Input (WebP): {input_size:.1f} KB")
        print(f"  - Output (PNG): {output_size:.1f} KB")

        print(f"\n⚠️  Note: Text replacement ('solutions' → 'factory') requires:")
        print(f"  1. Manual editing in image editor (Photoshop, GIMP, etc.)")
        print(f"  2. OR: Access to original vector file (SVG/AI) for clean text replacement")
        print(f"  3. OR: Advanced OCR + text removal + re-rendering")

        print(f"\n  For now, using the converted logo as-is.")
        print(f"  If text modification is critical, please edit manually.")

        return True

    except Exception as e:
        print(f"❌ Error converting logo: {e}")
        return False

if __name__ == "__main__":
    print("Converting Teragone logo from WebP to PNG...\n")
    convert_logo()
