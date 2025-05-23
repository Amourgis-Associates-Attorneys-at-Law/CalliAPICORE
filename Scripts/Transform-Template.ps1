# Transform-Template.ps1

param (
    [string]$Version = "DEV",
    [string]$TemplatePath = "..\UI\InstallPage\index.template.html",
    [string]$OutputPath = "..\docs\index.html"
)

# Get the script's directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Read, replace, and write
(Get-Content $templatePath -Raw) -replace '{{VERSION}}', $version | Set-Content $outputPath

Write-Host "✅ index.html generated at $outputPath with version $version"
