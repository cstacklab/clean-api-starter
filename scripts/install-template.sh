#!/usr/bin/env bash
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
package="CleanApiStarter.Template"
pkg="$root/artifacts/$package.0.0.0.nupkg"

cd "$root"

dotnet new uninstall "$package" 2>/dev/null || true
dotnet pack "CleanApiStarter.Template.csproj" --configuration Release --output "artifacts"
dotnet new install "$pkg" --force
rm -f "$pkg"
