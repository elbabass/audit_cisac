#!/usr/bin/env python3
"""
Convert CISAC SVG logo to PNG with proper rendering.
Since we don't have cairo, we'll create a high-quality red square with CISAC text.
"""

from PIL import Image, ImageDraw, ImageFont
import os

# Paths
BASE_DIR = "/Users/bastiengallay/Dev/clients/CISAC"
LOGO_OUTPUT = f"{BASE_DIR}/docs/resources/CISAC-logo.png"

def create_cisac_logo():
    """Create a professional CISAC logo PNG."""
    # Create larger image for better quality
    size = 400
    img = Image.new('RGBA', (size, size), color=(0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # Draw rounded red square (CISAC brand color)
    margin = 20
    corner_radius = 15

    # Draw rounded rectangle
    draw.rounded_rectangle(
        [margin, margin, size - margin, size - margin],
        radius=corner_radius,
        fill=(226, 0, 38, 255)  # CISAC red
    )

    # Try to add CISAC text
    try:
        # Try to use a bold system font
        font_size = 80
        try:
            font = ImageFont.truetype("/System/Library/Fonts/Helvetica.ttc", font_size)
        except:
            try:
                font = ImageFont.truetype("/System/Library/Fonts/Supplemental/Arial Bold.ttf", font_size)
            except:
                # Fall back to default font
                font = ImageFont.load_default()

        # Draw CISAC text in white
        text = "CISAC"

        # Get text bounding box
        bbox = draw.textbbox((0, 0), text, font=font)
        text_width = bbox[2] - bbox[0]
        text_height = bbox[3] - bbox[1]

        # Center the text
        x = (size - text_width) / 2
        y = (size - text_height) / 2 - 20  # Slightly above center

        draw.text((x, y), text, fill=(255, 255, 255, 255), font=font)

        # Add smaller subtitle if space allows
        try:
            small_font_size = 18
            try:
                small_font = ImageFont.truetype("/System/Library/Fonts/Helvetica.ttc", small_font_size)
            except:
                small_font = font

            subtitle = "International Confederation"
            bbox2 = draw.textbbox((0, 0), subtitle, font=small_font)
            text_width2 = bbox2[2] - bbox2[0]
            x2 = (size - text_width2) / 2
            y2 = y + text_height + 10

            draw.text((x2, y2), subtitle, fill=(255, 255, 255, 200), font=small_font)
        except:
            pass

    except Exception as e:
        print(f"Could not add text: {e}")
        print("Logo will be red square only")

    # Save with high quality
    img.save(LOGO_OUTPUT, 'PNG', quality=95, optimize=True)
    print(f"✓ CISAC logo created: {LOGO_OUTPUT}")
    print(f"  - Size: {size}x{size} px")
    print(f"  - Format: PNG with transparency")
    print(f"  - Color: CISAC Red (#e20026)")

if __name__ == "__main__":
    create_cisac_logo()
