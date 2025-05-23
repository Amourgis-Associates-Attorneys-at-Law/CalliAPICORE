# Transform-Template.ps1

# Get the script's directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Define paths relative to the script location
$templatePath = Join-Path $scriptDir "..\UI\InstallPage\index.template.html"
$outputPath = Join-Path $scriptDir "..\docs\index.html"

# Optional: Get version from environment or pass it as an argument
$version = $env:VERSION
if (-not $version) {
    $version = "DEV"
}

# Read, replace, and write
(Get-Content $templatePath -Raw) -replace '{{VERSION}}', $version | Set-Content $outputPath

Write-Host "✅ index.html generated at $outputPath with version $version"
