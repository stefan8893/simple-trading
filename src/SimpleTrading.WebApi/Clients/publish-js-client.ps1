[CmdletBinding()]
param (
    [Parameter(Mandatory = $true, Position = 0)]
    [ValidatePattern('^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$')]
    [string]$Version
)

Push-Location ..

dotnet run -- generate-client --target TypeScript --file-name index.ts --output-dir ./Clients/Npm/simple-trading

Pop-Location

Push-Location Npm/simple-trading

npm run build

if (-not( Test-Path -Path .\dist\package.json)) {
    Copy-Item -Path .\package.template.json -Destination .\dist
    Rename-Item -Path .\dist\package.template.json -NewName package.json
}

Push-Location .\dist

npm version $Version
npm publish --access public

Pop-Location
Pop-Location