#!/bin/bash

# Structurizr Lite Local Launcher
# Visualizes C4 architecture diagrams from workspace.dsl
# Documentation: https://structurizr.com/help/lite

echo "🚀 Starting Structurizr Lite..."
echo ""
echo "Mounting: $(pwd)/docs/work_in_progress/"
echo "Workspace: workspace.dsl"
echo ""
echo "Once started, open your browser to:"
echo "👉 http://localhost:8080"
echo ""
echo "Press Ctrl+C to stop"
echo ""

docker run -it --rm \
  -p 8080:8080 \
  -v "$(pwd)/docs/work_in_progress:/usr/local/structurizr" \
  structurizr/lite
