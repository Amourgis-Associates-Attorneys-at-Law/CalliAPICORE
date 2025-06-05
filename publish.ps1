param (
    [switch]$publish = $true,
    [switch]$stable = $true
)

# Check if a file exists and is unlocked
function Test-FileUnlocked {
    param ([string]$path)

    if (-not (Test-Path $path)) {
        return $false
    }

    try {
        $stream = [System.IO.File]::Open($path, 'Open', 'ReadWrite', 'None')
        $stream.Close()
        return $true
    } catch {
        return $false
    }
}

# Get the short major.minor.patch string
function Get-VersionShort {
    param ([string]$csprojPath)

    [xml]$csproj = Get-Content $csprojPath
    $version = $csproj.Project.PropertyGroup.Version
    $versionParts = $version.Split('.')
    return "$($versionParts[0]).$($versionParts[1]).$($versionParts[2])"
}

# Get all the links to the versions in versions.txt
function Generate-VersionLinks {
    param ([string]$versionsFile)

    if (-not (Test-Path $versionsFile)) {
        Write-Host "versions.txt not found."
        return ""
    }

    $links = ""
    Get-Content $versionsFile | ForEach-Object {
        $links += "<li><a href='https://github.com/Amourgis-Associates-Attorneys-at-Law/CalliAPICORE/releases/download/v$_/CalliAPI-win-Setup.exe'>v$_</a></li>`n"
    }
    return "<ul>`n$links</ul>"
}

# Inject the previous versions and the changelog into the index file
function Inject-Content {
    param (
        [string]$templatePath,
        [string]$outputPath,
        [string]$version,
        [string]$versionLinks,
        [string]$changelogPath
    )

    $html = Get-Content $templatePath -Raw
    $html = $html -replace '{{VERSION}}', $version

    if ($html -match '<!--\s*VERSIONS\s*-->') {
        $html = $html -replace '<!--\s*VERSIONS\s*-->', $versionLinks
        Write-Host "Injected version history."
    }

    if (Test-Path $changelogPath) {
        $changelog = Get-Content $changelogPath -Raw
        $html = $html -replace '<!--\s*CHANGELOG\s*-->', "<pre>$changelog</pre>"
        Write-Host "Injected changelog."
    }

    Set-Content $outputPath $html -Encoding UTF8
    Write-Host "Final index.html written."
}

# Define project settings
$project = "CalliAPI.csproj"
$versionShort = Get-VersionShort -csprojPath $project

# Define paths and variables
$publishDir = "publish"
$outputDir = "Releases"
$templateHtml = "docs\index.template.html"
$outputHtml = "docs\index.html"
$versionsFile = "versions.txt"
$changelogFile = "ReadMe.MD"
$repoUrl = 'https://github.com/Amourgis-Associates-Attorneys-at-Law/CalliAPICORE'
$stableStr = if ($stable) { "" } else { "--pre" }
$publishStr = if ($publish) { "--publish" } else { "" }

# Read environment variables
$VELOPACK_GITHUB_TOKEN = if ($env:VELOPACK_GITHUB_TOKEN) { $env:VELOPACK_GITHUB_TOKEN } else { Read-Host -Prompt "Enter your GitHub token: " }

# Early escapes
if (-not (Test-FileUnlocked $templateHtml)) {
    Write-Host "Template HTML file not found or is locked: $templateHtml"
    exit 1
}

if (-not (Test-FileUnlocked $outputHtml)) {
    Write-Host "Output HTML file is locked or inaccessible: $outputHtml"
    exit 1
}

# Begin script execution
Write-Host "Running Velopack upload for version $versionShort and set to full release: ${$publish.ToString()}..."

# Step 1: Build and publish
dotnet publish $project -c Release -r win-x64 -o $publishDir --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true

# Step 2: Generate and inject content
$versionLinks = Generate-VersionLinks -versionsFile $versionsFile
Inject-Content -templatePath $templateHtml -outputPath $outputHtml -version $versionShort -versionLinks $versionLinks -changelogPath $changelogFile

# Step 3: Publish release to GitHub
vpk upload github `
--outputDir $outputDir `
--repoUrl $repoUrl `
--token $VELOPACK_GITHUB_TOKEN `
$stableStr `
$publishStr `
--releaseName "CalliAPI $versionShort" `
--tag "v$versionShort"

Write-Host "Updated to release on GitHub with arguments ${$stable.ToString()} ${$publish.ToString()} $versionShort."

# Step 4: Commit and push to GitHub (only unlocked, changed files)
$changedFiles = git status --porcelain | ForEach-Object {
    $_.Substring(3)
}

$unlockedFiles = @()

foreach ($file in $changedFiles) {
    if (Test-FileUnlocked $file) {
        $unlockedFiles += $file
    } else {
        Write-Host "Skipping locked file: $file"
    }
}

if ($unlockedFiles.Count -eq 0) {
    Write-Host "No unlocked files to commit."
} else {
    git add $unlockedFiles
    git commit -m "Update site for version $versionShort"
    try {
        git push origin main
        Write-Host "Committed and pushed unlocked changes to GitHub."
    } catch {
        Write-Host "Failed to push changes: $_"
    }
}