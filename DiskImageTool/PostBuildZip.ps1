param(
    [string]$ExePath
)

$ErrorActionPreference = 'Stop'

Write-Host "==== Run $(Get-Date -Format 'MM/dd/yyyy HH:mm:ss') ===="

if (-not $ExePath) {
    Write-Error "ExePath parameter is required."
    exit 1
}

if (-not (Test-Path -LiteralPath $ExePath)) {
    Write-Error "ExePath does not exist: $ExePath"
    exit 1
}

# Resolve full path and output folder
$ExePath   = (Resolve-Path -LiteralPath $ExePath).ProviderPath
$OutputDir = Split-Path -Path $ExePath -Parent   # no -LiteralPath here

Write-Host "ExePath     = $ExePath"
Write-Host "OutputDir   = $OutputDir"

# Read version info from the EXE
$versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($ExePath)
$rawVersion  = $versionInfo.FileVersion

Write-Host "Raw version = $rawVersion"

# Try to parse the version string
try {
    $parsed = [version]$rawVersion
} catch {
    Write-Warning "Could not parse version '$rawVersion'. Falling back to 0.0.1."
    $parsed = [version]'0.0.1.0'
}

$major    = $parsed.Major
$minor    = $parsed.Minor
$revision = $parsed.Revision
if ($revision -lt 0) { $revision = 0 }

Write-Host "Parsed: Major=$major Minor=$minor Revision=$revision"

# Format like 2.30.01  (Major.Minor.Revision(2 digits))
$formattedVersion = '{0}.{1}.{2:00}' -f $major, $minor, $revision
Write-Host "Formatted   = $formattedVersion"

# Build zip path based on EXE name
$exeNameNoExt = [System.IO.Path]::GetFileNameWithoutExtension($ExePath)
$zipName      = "{0}_{1}.zip" -f $exeNameNoExt, $formattedVersion
$zipPath      = Join-Path -Path $OutputDir -ChildPath $zipName

Write-Host "ZipPath     = $zipPath"

# Exclusions
$excludeNames = @(
    'CompactJson.xml',
    'CompactJSON.xml',
	'UserDB.xml',
	'BootSector.xml'
)

$excludeExts = @(
    '.zip',
	'.log'
)

# Collect files to include (publish folder + all subfolders)
$files = Get-ChildItem -Path $OutputDir -Recurse -File |
    Where-Object {
        ($_.Extension -notin $excludeExts) -and
        ($excludeNames -notcontains $_.Name)
    }

$filesCount = $files.Count
Write-Host "Files to zip (after filter): $filesCount"

if ($filesCount -eq 0) {
    Write-Warning "No files found to archive after filtering. Aborting."
    exit 1
}

# Use .NET's System.IO.Compression instead of Compress-Archive
Add-Type -AssemblyName System.IO.Compression.FileSystem

# Pick maximum compression if available
$compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
try {
    $smallest = [System.IO.Compression.CompressionLevel]::SmallestSize
    if ($smallest -ne $null) {
        $compressionLevel = $smallest
    }
} catch {
    # Older frameworks won't have SmallestSize; stay on Optimal
}

# Delete existing zip if present
if (Test-Path -LiteralPath $zipPath) {
    Write-Host "Removing existing archive..."
    Remove-Item -LiteralPath $zipPath -Force
}

Write-Host "Creating archive via System.IO.Compression with compression level: $compressionLevel"

$zip = $null
$archiveSucceeded = $false

try {
    $zip = [System.IO.Compression.ZipFile]::Open(
        $zipPath,
        [System.IO.Compression.ZipArchiveMode]::Create
    )

    foreach ($file in $files) {
        # relative path inside zip
        $relativePath = [System.IO.Path]::GetRelativePath($OutputDir, $file.FullName)

        Write-Host " Adding $relativePath"

        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile(
            $zip,
            $file.FullName,
            $relativePath,
            $compressionLevel
        )
    }

    $archiveSucceeded = $true
    Write-Host "Archive created successfully."
}
catch {
    Write-Error "Failed to create archive: $($_.Exception.Message)"
    if (Test-Path -LiteralPath $zipPath) {
        Write-Host "Removing incomplete archive..."
        Remove-Item -LiteralPath $zipPath -Force
    }
    exit 1
}
finally {
    if ($zip -ne $null) {
        $zip.Dispose()
    }
}
