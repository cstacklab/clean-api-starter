#!/usr/bin/env bash
# Copies the single source-of-truth database/ (repo root) into each variant.
# The variant copies are git-ignored; this keeps one canonical set of migrations
# while letting each variant stay self-contained for `dotnet new` packaging.
#
# Run this before packing a template locally, or before running a variant's
# Aspire AppHost. Integration tests find the migrations via a directory walk-up,
# so they work without syncing. CI runs this automatically.
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
source_dir="$root/database"

if [ ! -d "$source_dir" ]; then
    echo "Source database directory not found: $source_dir" >&2
    exit 1
fi

for variant in layered modular; do
    dest="$root/$variant/database"
    rm -rf "$dest"
    cp -R "$source_dir" "$dest"
    echo "Synced database -> $variant/database"
done
