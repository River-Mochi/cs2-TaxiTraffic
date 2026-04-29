# File: Scripts/Sync-SharedDotfiles.ps1
# Purpose: copy shared RiverMochi repo dotfiles into this mod repo.
# Usage from mod repo:
#   powershell -NoProfile -ExecutionPolicy Bypass -File Scripts\Sync-SharedDotfiles.ps1
# Preview only:
#   powershell -NoProfile -ExecutionPolicy Bypass -File Scripts\Sync-SharedDotfiles.ps1 -Preview

using Game.Buildings;
using Game.UI.Widgets;
using System.ComponentModel;
using System.IO;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;

[CmdletBinding()]
param(
    [string]$SharedRepoPath,
    [switch]$Preview
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$modRepoRoot = Split-Path -Parent $scriptDir

if ([string]::IsNullOrWhiteSpace($SharedRepoPath)) {
    $reposRoot = Split-Path -Parent $modRepoRoot
    $SharedRepoPath = Join-Path $reposRoot "CS2-Shared-RiverMochi"
}

$dotfiles = @(
    ".editorconfig",
    ".gitattributes",
    ".gitignore"
)

if (-not (Test-Path -LiteralPath $SharedRepoPath -PathType Container)) {
    throw "Shared repo folder not found: $SharedRepoPath"
}

Write-Host "Mod repo:    $modRepoRoot"
Write-Host "Shared repo: $SharedRepoPath"
Write-Host ""

foreach ($fileName in $dotfiles) {
    $sourcePath = Join-Path $SharedRepoPath $fileName
    $targetPath = Join-Path $modRepoRoot $fileName

    if (-not (Test-Path -LiteralPath $sourcePath -PathType Leaf)) {
        Write-Warning "Missing shared file: $sourcePath"
        continue
    }

    $needsCopy = $true

    if (Test-Path -LiteralPath $targetPath -PathType Leaf) {
        $sourceHash = Get-FileHash -LiteralPath $sourcePath -Algorithm SHA256
        $targetHash = Get-FileHash -LiteralPath $targetPath -Algorithm SHA256
        $needsCopy = $sourceHash.Hash -ne $targetHash.Hash
    }

    if (-not $needsCopy) {
        Write-Host "UNCHANGED  $fileName"
        continue
    }

    if ($Preview) {
        Write-Host "WOULD COPY $fileName"
        continue
    }

    Copy-Item -LiteralPath $sourcePath -Destination $targetPath -Force
    Write-Host "COPIED    $fileName"
}

Write-Host ""
Write-Host "Done."
