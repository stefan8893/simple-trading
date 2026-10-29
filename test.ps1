[CmdletBinding()]
param (
    [Parameter()]
    [ValidateSet('Release', 'Debug')]
    [string]$Configuration = 'Release',

    [Parameter()]
    [switch]$NoBuild
)

$testDirectory = Join-Path $PSScriptRoot 'test'

if (-not (Test-Path -Path $testDirectory -PathType Container)) {
    Write-Error "Test directory not found: $testDirectory"
    exit 1
}

$testProjects = @(
    Get-ChildItem -Path $testDirectory -Directory -Filter '*.Tests' |
        ForEach-Object { Get-ChildItem -Path $_.FullName -Filter '*.csproj' -File } |
        Sort-Object -Property FullName
)

if ($testProjects.Count -eq 0) {
    Write-Error "No *.Tests/*.csproj projects found in $testDirectory"
    exit 1
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error 'dotnet CLI not found in PATH'
    exit 1
}

Write-Host "Test projects:"
$testProjects | ForEach-Object { Write-Host "  $($_.FullName)" }
Write-Host "`nConfiguration: $Configuration"
if ($NoBuild) {
    Write-Host 'Build: skipped'
}
Write-Host

$failures = @()
Push-Location $testDirectory
try {
    foreach ($project in $testProjects) {
        $coverageOutput = Join-Path $project.DirectoryName 'coverage.cobertura.xml'
        $dotnetArguments = @(
            'run'
            '--project', $project.FullName
            '--configuration', $Configuration
        )
        if ($NoBuild) {
            $dotnetArguments += '--no-build'
        }
        $dotnetArguments += @(
            '--'
            '--coverage'
            '--coverage-output-format', 'cobertura'
            '--coverage-output', $coverageOutput
        )

        Write-Host "Running tests in $($project.Name)"
        & dotnet @dotnetArguments
        if ($LASTEXITCODE -ne 0) {
            $failures += [pscustomobject]@{
                Project  = $project.Name
                ExitCode = $LASTEXITCODE
            }
        }
    }
}
finally {
    Pop-Location
}

if ($failures.Count -gt 0) {
    Write-Host 'One or more test projects failed:' -ForegroundColor Red
    $failures | ForEach-Object {
        Write-Host "  $($_.Project): exit code $($_.ExitCode)" -ForegroundColor Red
    }
    exit $failures[0].ExitCode
}

Write-Host 'All test projects passed.'
