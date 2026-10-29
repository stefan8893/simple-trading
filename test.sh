#!/usr/bin/env bash

set -u

configuration="Release"
no_build=0
configuration_set=0

usage() {
    echo "Usage: $0 [-Configuration Release|Debug] [-NoBuild]"
}

while [[ $# -gt 0 ]]; do
    case "$(printf '%s' "$1" | tr '[:upper:]' '[:lower:]')" in
        -configuration|--configuration|-c)
            if [[ $# -lt 2 ]]; then
                echo "Error: $1 requires a value (Release or Debug)" >&2
                exit 1
            fi
            configuration="$2"
            configuration_set=1
            shift 2
            ;;
        -nobuild|--no-build)
            no_build=1
            shift
            ;;
        -h|--help)
            usage
            exit 0
            ;;
        *)
            if [[ $configuration_set -eq 1 ]]; then
                echo "Error: unexpected argument '$1'" >&2
                usage
                exit 1
            fi
            configuration="$1"
            configuration_set=1
            shift
            ;;
    esac
done

case "$(printf '%s' "$configuration" | tr '[:upper:]' '[:lower:]')" in
    release) configuration="Release" ;;
    debug) configuration="Debug" ;;
    *)
        echo "Error: invalid configuration '$configuration'. Allowed: Release, Debug" >&2
        usage
        exit 1
        ;;
esac

script_directory="$(cd -- "$(dirname -- "$0")" && pwd)"
test_directory="$script_directory/test"

if [[ ! -d "$test_directory" ]]; then
    echo "Error: test directory not found: $test_directory" >&2
    exit 1
fi

test_projects=()
while IFS= read -r project; do
    test_projects+=("$project")
done < <(find "$test_directory" -mindepth 2 -maxdepth 2 -type f -path "$test_directory/*.Tests/*.csproj" -print | sort)

if [[ ${#test_projects[@]} -eq 0 ]]; then
    echo "Error: no *.Tests/*.csproj projects found in $test_directory" >&2
    exit 1
fi

if ! command -v dotnet >/dev/null 2>&1; then
    echo "Error: dotnet CLI not found in PATH" >&2
    exit 1
fi

echo "Test projects:"
printf '  %s\n' "${test_projects[@]}"
echo
echo "Configuration: $configuration"
if [[ $no_build -eq 1 ]]; then
    echo "Build: skipped"
fi
echo

cd "$test_directory" || exit 1

failures=()
first_failure=0
for project in "${test_projects[@]}"; do
    project_name="$(basename "$project")"
    project_directory="$(dirname "$project")"
    coverage_output="$project_directory/coverage.cobertura.xml"
    dotnet_arguments=(run --project "$project" --configuration "$configuration")

    if [[ $no_build -eq 1 ]]; then
        dotnet_arguments+=(--no-build)
    fi

    dotnet_arguments+=(
        --
        --coverage
        --coverage-output-format cobertura
        --coverage-output "$coverage_output"
    )

    echo "Running tests in $project_name"
    dotnet "${dotnet_arguments[@]}"
    exit_code=$?

    if [[ $exit_code -ne 0 ]]; then
        failures+=("$project_name: exit code $exit_code")
        if [[ $first_failure -eq 0 ]]; then
            first_failure=$exit_code
        fi
    fi
done

if [[ ${#failures[@]} -gt 0 ]]; then
    echo "One or more test projects failed:" >&2
    printf '  %s\n' "${failures[@]}" >&2
    exit "$first_failure"
fi

echo "All test projects passed."
