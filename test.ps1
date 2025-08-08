[CmdletBinding()]
param (
    [Parameter()]
    [ValidateSet('Release', 'Debug')]
    [string]$Configuration = 'Release'
)

Push-Location ./test

$testProjects = Get-ChildItem -Path "*.Tests/*.csproj" -File

Write-Host "Test projects:"
$testProjects | Write-Host
Write-Host "`n"

$exitCodes = @();
foreach ($project in $testProjects) {
    Write-Host "Running tests for $project"
    dotnet run --project $project -- `
        --coverage `
        --coverage-output-format cobertura `
        --coverage-output ../../../../coverage.cobertura.xml

    if ($LASTEXITCODE -ne 0) {
        $exitCodes += $LASTEXITCODE
    }
}

Pop-Location

foreach ($exitCode in $exitCodes) {
    if ($exitCode -ne 0) {
        Exit $LastExitCode
    }
}

