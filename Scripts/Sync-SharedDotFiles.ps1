# File: Scripts/Sync-SharedDotFiles.ps1
# Version: 0.1.0
# Purpose:
#   Copy shared RiverMochi dotfiles into this mod repo:
#     .editorconfig
#     .gitattributes
#     .gitignore
#
# Usage from a mod repo:
#   powershell -NoProfile -ExecutionPolicy Bypass -File Scripts\Sync-SharedDotfiles.ps1
#
# Preview only:
#   powershell -NoProfile -ExecutionPolicy Bypass -File Scripts\Sync-SharedDotfiles.ps1 -Preview
#
# Optional explicit shared repo path:
#   powershell -NoProfile -ExecutionPolicy Bypass -File Scripts\Sync-SharedDotfiles.ps1 -SharedRepoPath "C:\Users\kldan\source\repos\CS2-Shared-RiverMochi"

param(
  [string]$SharedRepoPath,
  [switch]$Preview
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# --------------------------
# Helpers
# --------------------------

function Resolve-RepoRoot([string]$startDir) {
  $current = Get-Item -LiteralPath $startDir

  while ($null -ne $current) {
    if (Test-Path -LiteralPath (Join-Path $current.FullName '.git')) {
      return $current.FullName
    }

    $current = $current.Parent
  }

  throw "Could not find repo root by walking upward from: $startDir"
}

function Copy-SharedFileIfChanged(
  [string]$sourcePath,
  [string]$targetPath,
  [switch]$PreviewOnly
) {
  $fileName = Split-Path -Leaf $sourcePath

  if (-not (Test-Path -LiteralPath $sourcePath -PathType Leaf)) {
    Write-Warning "Missing shared file: $sourcePath"
    return
  }

  $needsCopy = $true

  if (Test-Path -LiteralPath $targetPath -PathType Leaf) {
    $sourceHash = Get-FileHash -LiteralPath $sourcePath -Algorithm SHA256
    $targetHash = Get-FileHash -LiteralPath $targetPath -Algorithm SHA256
    $needsCopy = ($sourceHash.Hash -ne $targetHash.Hash)
  }

  if (-not $needsCopy) {
    Write-Host "UNCHANGED  $fileName"
    return
  }

  if ($PreviewOnly) {
    Write-Host "WOULD COPY $fileName"
    return
  }

  Copy-Item -LiteralPath $sourcePath -Destination $targetPath -Force
  Write-Host "COPIED    $fileName"
}

# --------------------------
# Detect paths
# --------------------------

$modRepoRoot = Resolve-RepoRoot $PSScriptRoot

if ([string]::IsNullOrWhiteSpace($SharedRepoPath)) {
  $reposRoot = Split-Path -Parent $modRepoRoot
  $SharedRepoPath = Join-Path $reposRoot 'CS2-Shared-RiverMochi'
}

if (-not (Test-Path -LiteralPath $SharedRepoPath -PathType Container)) {
  throw "Shared repo folder not found: $SharedRepoPath"
}

Write-Host "Mod repo:    $modRepoRoot"
Write-Host "Shared repo: $SharedRepoPath"

if ($Preview) {
  Write-Host "Mode:        PREVIEW only"
} else {
  Write-Host "Mode:        COPY changed files"
}

Write-Host ""

# --------------------------
# Copy shared dotfiles
# --------------------------

$dotfiles = @(
  '.editorconfig',
  '.gitattributes',
  '.gitignore'
)

foreach ($fileName in $dotfiles) {
  $sourcePath = Join-Path $SharedRepoPath $fileName
  $targetPath = Join-Path $modRepoRoot $fileName

  Copy-SharedFileIfChanged `
    -sourcePath $sourcePath `
    -targetPath $targetPath `
    -PreviewOnly:$Preview
}

Write-Host ""
Write-Host "Done."
exit 0
