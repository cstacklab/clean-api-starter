#!/usr/bin/env bash
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
name="CleanApiStarter.Template.0.0.0.nupkg"
pkg="$root/artifacts/$name"

cd "$root"

dotnet new uninstall CleanApiStarter.Template 2>/dev/null || true
dotnet pack "CleanApiStarter.Template.csproj" --configuration Release --output "artifacts"
dotnet new install "$pkg" --force
rm -f "$pkg"
