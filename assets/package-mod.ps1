$ModName = "SimpleHealthBar"

# Define paths
$AssetDir = $PSScriptRoot
$ProjectRoot = Resolve-Path "$AssetDir\.."
$IL2CPPAssembly = Join-Path $ProjectRoot "bin\Release IL2CPP\net6\$ModName-IL2CPP.dll"
$MonoAssembly = Join-Path $ProjectRoot "bin\Release\netstandard2.1\$ModName-Mono.dll"
$TSZip = Join-Path $AssetDir "$ModName-TS.zip"
$NexusIL2CPPZip = Join-Path $AssetDir "$ModName-IL2CPP.zip"
$NexusMonoZip = Join-Path $AssetDir "$ModName-Mono.zip"

# Check existence
$hasIL2CPP = Test-Path -Path $IL2CPPAssembly -PathType Leaf
$hasMono   = Test-Path -Path $MonoAssembly   -PathType Leaf


if (-not ($hasIL2CPP -or $hasMono)) {
    Write-Error "Error: Neither IL2CPP nor Mono assembly was found. Aborting packaging."
    exit 1
}


# Clean up any existing zips
Remove-Item -Path $TSZip, $NexusIL2CPPZip, $NexusMonoZip -ErrorAction SilentlyContinue

# --- Package TS ---
$TSFiles = @(
    "$AssetDir\icon.png",
    "$ProjectRoot\README.md",
    "$ProjectRoot\CHANGELOG.md",
    "$AssetDir\manifest.json"
)
if ($hasIL2CPP) {
    $TSFiles += $IL2CPPAssembly
} elseif ($hasMono) {
    $TSFiles += $MonoAssembly
}

Compress-Archive -Path $TSFiles -DestinationPath $TSZip -Force
Write-Host "Created Thunderstore package: $TSZip"

# --- Package Nexus ---
if ($hasIL2CPP) {
    Compress-Archive -Path $IL2CPPAssembly -DestinationPath $NexusIL2CPPZip -Force
    Write-Host "Created Nexus IL2CPP zip: $NexusIL2CPPZip"
}

if ($hasMono) {
    Compress-Archive -Path $MonoAssembly -DestinationPath $NexusMonoZip -Force
    Write-Host "Created Nexus Mono zip: $NexusMonoZip"
}
