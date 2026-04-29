
# File: Scripts/Sync-SharedDotFiles.ps1

# Version: 0.2.0

# Purpose:

#   Copy shared RiverMochi dotfiles into this mod repo:

#     .editorconfig

#     .gitattributes

#     .gitignore

#

# Usage from repo root:

#   powershell -NoProfile -ExecutionPolicy Bypass -File Scripts\Sync-SharedDotFiles.ps1

#

# Preview only:

#   powershell -NoProfile -ExecutionPolicy Bypass -File Scripts\Sync-SharedDotFiles.ps1 -Preview

#

# Optional shared repo path:

#   powershell -NoProfile -ExecutionPolicy Bypass -File Scripts\Sync-SharedDotFiles.ps1 -SharedRepoPath "C:\Users\kldan\source\repos\CS2-Shared-RiverMochi"



param(

    [string]$SharedRepoPath = "",

    [switch]$Preview

)



Set-StrictMode -Version Latest

$ErrorActionPreference = "Stop"



$modRepoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path



if ([string]::IsNullOrWhiteSpace($SharedRepoPath)) {

    $reposRoot = Split-Path -Parent $modRepoRoot

    $SharedRepoPath = Join-Path $reposRoot "CS2-Shared-RiverMochi"

}



if (-not (Test-Path -LiteralPath $SharedRepoPath -PathType Container)) {

    throw "Shared repo folder not found: $SharedRepoPath"

}



$files = @(

    ".editorconfig",

    ".gitattributes",

    ".gitignore"

)



Write-Host "Mod repo:    $modRepoRoot"

Write-Host "Shared repo: $SharedRepoPath"



if ($Preview) {

    Write-Host "Mode:        PREVIEW"

} else {

    Write-Host "Mode:        COPY"

}



Write-Host ""



foreach ($fileName in $files) {

    $sourcePath = Join-Path $SharedRepoPath $fileName

    $targetPath = Join-Path $modRepoRoot $fileName



    if (-not (Test-Path -LiteralPath $sourcePath -PathType Leaf)) {

        Write-Warning "Missing shared file: $sourcePath"

        continue

    }



    if ($Preview) {

        Write-Host "WOULD COPY $fileName"

        continue

    }



    Copy-Item -LiteralPath $sourcePath -Destination $targetPath -Force

    Write-Host "COPIED     $fileName"

}



Write-Host ""

Write-Host "Done."

exit 0

