#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
coverage_dir="$repo_root/artifacts/coverage"
test_results_dir="$coverage_dir/test-results"
coverage_report_dir="$coverage_dir/report"

cd "$repo_root"

rm -rf "$coverage_dir"
mkdir -p "$test_results_dir" "$coverage_report_dir"

dotnet tool restore

dotnet test CleanApiStarter.slnx \
    --collect:"XPlat Code Coverage" \
    --disable-build-servers \
    --results-directory "$test_results_dir" \
    --logger trx \
    /m:1 \
    /nr:false \
    -v:minimal

coverage_files=()

while IFS= read -r coverage_file; do
    coverage_files+=("$coverage_file")
done < <(find "$test_results_dir" -path "*/In/*" -prune -o -type f -name "coverage.cobertura.xml" -print | sort)

if [ "${#coverage_files[@]}" -eq 0 ]; then
    echo "No coverage.cobertura.xml files were generated."
    exit 1
fi

coverage_reports="$(IFS=';'; echo "${coverage_files[*]}")"

dotnet reportgenerator \
    "-reports:$coverage_reports" \
    "-targetdir:$coverage_report_dir" \
    "-reporttypes:Html" \
    "-title:CleanApiStarter Coverage"

coverage_index="$coverage_report_dir/index.html"

echo "Coverage report: $coverage_index"
open -a "Google Chrome" "$coverage_index"
