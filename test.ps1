[CmdletBinding()]
param (
    [Parameter()]
    [ValidateSet('Release', 'Debug')]
    [string]$Configuration = 'Release'
)

Push-Location ./test

try {

    $testProjects = Get-ChildItem -Path "*.Tests/*.csproj" -File

    Write-Host "Test projects:"
    $testProjects | Write-Host
    Write-Host "`n"

    $exitCodes = @();
    foreach ($project in $testProjects) {
        Write-Host "Running tests in $project"
        dotnet run --project $project --configuration $Configuration -- `
            --coverage `
            --coverage-output-format cobertura `
            --coverage-output ../../../../coverage.cobertura.xml

        if ($LASTEXITCODE -ne 0) {
            $exitCodes += $LASTEXITCODE
        }
    }
}
finally {
    Pop-Location
}

foreach ($exitCode in $exitCodes) {
    if ($exitCode -ne 0) {
        Exit $exitCode
    }
}

