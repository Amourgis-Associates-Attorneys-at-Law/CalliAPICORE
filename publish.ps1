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

# Define project settings
$project = "CalliAPI.csproj"
[xml]$csproj = Get-Content $project
$version = $csproj.Project.PropertyGroup.Version
$versionParts = $version.Split('.')
$versionShort = "$($versionParts[0]).$($versionParts[1]).$($versionParts[2])"

# Define paths and variables
$publishDir = "publish"
$outputDir = "Releases"
$templateHtml = "UI\InstallPage\index.template.html"
$outputHtml = "docs\index.html"
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

# Step 2: Generate index.html with version injected
$dir = Split-Path $outputHtml
if (-not (Test-Path $dir)) {
    New-Item -ItemType Directory -Path $dir | Out-Null
}

(Get-Content $templateHtml) -replace '{{VERSION}}', $versionShort | Set-Content $outputHtml

Write-Host "Generated index.html with version $versionShort"

# Step 2.5: Inject version history links
$versionsFile = "versions.txt"
$versionLinks = ""

if (Test-Path $versionsFile) {
    $versions = Get-Content $versionsFile
    foreach ($v in $versions) {
        $versionLinks += "<li><a href='https://github.com/Amourgis-Associates-Attorneys-at-Law/CalliAPICORE/releases/download/v$v/CalliAPI-win-Setup.exe'>v$v</a></li>`n"
    }

    $html = Get-Content $outputHtml -Raw
    $html = $html -replace '<!--VERSIONS-->', "<ul>`n$versionLinks</ul>"
    Set-Content $outputHtml $html
    Write-Host "Injected version history into index.html"
} else {
    Write-Host "versions.txt not found. Skipping version history injection."
}



# Step 3: Publish release to GitHub
vpk upload github `
--outputDir $outputDir `
--repoUrl $repoUrl `
--token $VELOPACK_GITHUB_TOKEN `
$stableStr `
$publishStr `
--releaseName "CalliAPI $versionShort" `
--tag "v$versionShort"

Write-Host "Updated to release on GitHub with arguments ${$stable.ToString()} ${$publish.ToString()} $version."

# Step 4: Commit and push to GitHub (only unlocked, changed files)
$changedFiles = git status --porcelain | ForEach-Object {
    $_.Substring(3)  # Extract file path from status line
}

$unlockedFiles = @()

foreach ($file in $changedFiles) 
{
    if (Test-FileUnlocked $file) { $unlockedFiles += $file }
    else
    {
        Write-Host "Skipping locked file: $file"
    }
}

if ($unlockedFiles.Count -eq 0) {
    Write-Host "No unlocked files to commit."
} 
else 
{
    git add $unlockedFiles
    git commit -m "Update site for version $versionShort"
    try 
    {
    git push origin main
    Write-Host "Committed and pushed unlocked changes to GitHub."
    } 
    catch
    {
        Write-Host "Failed to push changes: $_"
    }
}