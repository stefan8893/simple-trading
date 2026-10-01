#!/usr/bin/env bash
# Created by AI
# Runs all *.Tests test projects with code coverage (Cobertura).
# Bash port of test.ps1; compatible with bash (incl. macOS bash 3.2)
# and zsh on macOS and Linux.
#
# Usage:
#   ./test.sh [-Configuration Release|Debug]

set -u

CONFIGURATION="Release"

usage() {
    echo "Usage: $0 [-Configuration Release|Debug]"
}

# --- Parameter handling (equivalent to [ValidateSet('Release','Debug')]) ---
while [[ $# -gt 0 ]]; do
    case "$1" in
        -Configuration|--configuration|-c)
            if [[ $# -lt 2 ]]; then
                echo "Error: $1 requires a value (Release or Debug)" >&2
                exit 1
            fi
            CONFIGURATION="$2"
            shift 2
            ;;
        -h|--help)
            usage
            exit 0
            ;;
        --)
            shift
            break
            ;;
        *)
            CONFIGURATION="$1"
            shift
            ;;
    esac
done

# ValidateSet is case-insensitive in PowerShell, so normalize here too.
case "$(printf '%s' "$CONFIGURATION" | tr '[:upper:]' '[:lower:]')" in
    release) CONFIGURATION="Release" ;;
    debug)   CONFIGURATION="Debug" ;;
    *)
        echo "Error: invalid configuration '$CONFIGURATION'. Allowed: Release, Debug" >&2
        usage
        exit 1
        ;;
esac

# --- Sanity checks -----------------------------------------------------------
if ! command -v dotnet >/dev/null 2>&1; then
    echo "Error: dotnet CLI not found in PATH" >&2
    exit 1
fi
if [[ ! -d ./test ]]; then
    echo "Error: ./test directory not found (run from the repository root)" >&2
    exit 1
fi

# --- Enter ./test; restore the original directory on exit, error or Ctrl+C --
original_dir="$PWD"
cd ./test || exit 1
trap 'cd "$original_dir"' EXIT

# --- Discover test projects (Get-ChildItem "*.Tests/*.csproj" equivalent) -----
test_projects=()
while IFS= read -r proj; do
    test_projects+=("$proj")
done < <(find . -maxdepth 2 -type f -path './*.Tests/*.csproj' | sort)

if [[ ${#test_projects[@]} -eq 0 ]]; then
    echo "Error: no *.Tests/*.csproj projects found in ./test" >&2
    exit 1
fi

echo "Test projects:"
printf '  %s\n' "${test_projects[@]}"
echo
echo "Configuration: $CONFIGURATION"
echo

# --- Run tests; remember the first non-zero exit code -------------------------
first_failure=0

for project in "${test_projects[@]}"; do
    echo "Running tests in $project"
    dotnet run --project "$project" --configuration "$CONFIGURATION" -- \
        --coverage \
        --coverage-output-format cobertura \
        --coverage-output ../../../../coverage.cobertura.xml

    status=$?
    if [[ $status -ne 0 ]]; then
        echo "FAILED: $project (exit code $status)" >&2
        if [[ $first_failure -eq 0 ]]; then
            first_failure=$status
        fi
    fi
    echo
done

# --- Summary ------------------------------------------------------------------
if [[ $first_failure -ne 0 ]]; then
    echo "Result: one or more test projects failed." >&2
    exit "$first_failure"
fi

echo "Result: all test projects passed."