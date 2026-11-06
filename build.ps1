param([string]$OutputDir = (Join-Path $PSScriptRoot 'dist'), [string]$Version = '1.2.0', [switch]$Offline)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression.FileSystem
Add-Type -AssemblyName System.IO.Compression
$cacheRoot = Join-Path $PSScriptRoot '.cache'
$stageRoot = Join-Path $PSScriptRoot ('.build\' + [Guid]::NewGuid().ToString('N'))
$appRoot = Join-Path $stageRoot 'app'
New-Item -ItemType Directory -Force $cacheRoot,$appRoot,$OutputDir,(Join-Path $appRoot 'dependencies'),(Join-Path $appRoot 'licenses'),(Join-Path $appRoot 'Ornek-sesler') | Out-Null
function Get-PinnedFile([string]$Name,[string]$Url,[string]$ExpectedHash) {
    $file = Join-Path $cacheRoot $Name
    if (-not (Test-Path -LiteralPath $file)) {
        if ($Offline) { throw "Offline cache is missing: $Name" }
        [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
        Invoke-WebRequest -Uri $Url -OutFile $file -UseBasicParsing
    }
    if ((Get-FileHash -LiteralPath $file -Algorithm SHA256).Hash -ne $ExpectedHash) { throw "SHA-256 mismatch: $Name" }
    return $file
}
function Copy-ZipEntry([string]$Archive,[string]$Entry,[string]$Target) {
    $zip = [IO.Compression.ZipFile]::OpenRead($Archive)
    try {
        $item = $zip.GetEntry($Entry)
        if ($null -eq $item) { throw "Missing entry: $Entry" }
        $inputStream = $item.Open(); $outputStream = [IO.File]::Create($Target)
        try { $inputStream.CopyTo($outputStream) } finally { $inputStream.Dispose(); $outputStream.Dispose() }
    } finally { $zip.Dispose() }
}
$packages = @(
    @('naudio','lib/net472/NAudio.dll','NAudio.dll','d1402cd555b1fa6a2b5eb6dc25139f0dba0aed72e4b033f791d3759ce2ade958'),
    @('naudio.core','lib/netstandard2.0/NAudio.Core.dll','NAudio.Core.dll','794645dbfd30e4880663d52d9d9224d55c301fa54228ff623c95d572c7887347'),
    @('naudio.wasapi','lib/netstandard2.0/NAudio.Wasapi.dll','NAudio.Wasapi.dll','c8396b8dbab86a2619edfd9158beaf18c21cb86677099b5594950fd8651b9d1a'),
    @('naudio.winmm','lib/netstandard2.0/NAudio.WinMM.dll','NAudio.WinMM.dll','48da978d715f219489c050ee8c32c64f80ff73718eba237ebcbc636ba5902be3')
)
foreach ($package in $packages) {
    $name = $package[0]
    $archive = Get-PinnedFile "$name.2.2.1.nupkg" "https://api.nuget.org/v3-flatcontainer/$name/2.2.1/$name.2.2.1.nupkg" $package[3]
    Copy-ZipEntry $archive $package[1] (Join-Path $appRoot $package[2])
    if ($name -eq 'naudio') { Copy-ZipEntry $archive 'license.txt' (Join-Path $appRoot 'licenses\NAudio.txt') }
}
$driver = Get-PinnedFile 'VBCABLE_Driver_Pack46.zip' 'https://download.vb-audio.com/Download_CABLE/VBCABLE_Driver_Pack46.zip' 'bc44e8974191a5a2427d6c979e2dab08dbb09925f3f435dfa2f0ba3e45b6eb90'
Copy-Item -LiteralPath $driver -Destination (Join-Path $appRoot 'dependencies\VBCABLE_Driver_Pack46.zip')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'THIRD-PARTY-NOTICES.md') -Destination (Join-Path $appRoot 'licenses\THIRD-PARTY-NOTICES.md')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'LICENSE') -Destination (Join-Path $appRoot 'licenses\BroPad.txt')
Copy-Item -Path (Join-Path $PSScriptRoot 'samples\*.wav') -Destination (Join-Path $appRoot 'Ornek-sesler')
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'packaging\BroPad.App.exe.config') -Destination (Join-Path $appRoot 'BroPad.App.exe.config')
$framework = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319'
$compiler = Join-Path $framework 'csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) { throw '.NET Framework 4.8 build tools are required on Windows x64.' }
$refs = @('System.Drawing.dll','System.Windows.Forms.dll','System.Runtime.Serialization.dll','System.IO.Compression.dll','System.IO.Compression.FileSystem.dll',
    (Join-Path $framework 'netstandard.dll'))
$refs += $packages | ForEach-Object { Join-Path $appRoot $_[2] }
$manifest = Join-Path $PSScriptRoot 'packaging\app.manifest'
$icon = Join-Path $PSScriptRoot 'assets\BroPad.ico'
$logo = Join-Path $PSScriptRoot 'assets\BroPad.png'
$appArgs = @('/nologo','/target:winexe','/platform:x64',"/out:$appRoot\BroPad.App.exe", "/win32manifest:$manifest", "/win32icon:$icon", "/resource:$icon,BroPad.Icon", "/resource:$logo,BroPad.Logo")
$appArgs += $refs | ForEach-Object { "/reference:$_" }
$appArgs += Get-ChildItem -LiteralPath (Join-Path $PSScriptRoot 'src') -Filter '*.cs' | Select-Object -ExpandProperty FullName
& $compiler @appArgs
if ($LASTEXITCODE -ne 0) { throw 'Application build failed.' }
$payload = Join-Path $stageRoot 'payload.zip'
$payloadStream = [IO.File]::Create($payload)
$payloadArchive = [IO.Compression.ZipArchive]::new($payloadStream,[IO.Compression.ZipArchiveMode]::Create,$false)
try {
    foreach ($file in Get-ChildItem -LiteralPath $appRoot -File -Recurse) {
        $entryName = $file.FullName.Substring($appRoot.Length + 1).Replace('\','/')
        [IO.Compression.ZipFileExtensions]::CreateEntryFromFile($payloadArchive,$file.FullName,$entryName,[IO.Compression.CompressionLevel]::Optimal) | Out-Null
    }
} finally { $payloadArchive.Dispose(); $payloadStream.Dispose() }
$payloadHash = (Get-FileHash -LiteralPath $payload -Algorithm SHA256).Hash.ToLowerInvariant()
if ($Version -notmatch '^\d+\.\d+\.\d+([-.][a-zA-Z0-9.]+)?$') { throw 'Invalid version.' }
$infoFile = Join-Path $stageRoot 'BuildInfo.cs'
$info = 'namespace BroPad.Launcher { internal static class BuildInfo { public const string Version="' + $Version + '"; public const string PayloadHash="' + $payloadHash + '"; public const string DriverHash="bc44e8974191a5a2427d6c979e2dab08dbb09925f3f435dfa2f0ba3e45b6eb90"; } }'
[IO.File]::WriteAllText($infoFile,$info)
$launcher = Join-Path ([IO.Path]::GetFullPath($OutputDir)) 'BroPad.exe'
& $compiler /nologo /target:winexe /platform:x64 "/out:$launcher" "/win32manifest:$manifest" "/win32icon:$icon" "/resource:$payload,BroPad.Payload" /reference:System.Windows.Forms.dll /reference:System.IO.Compression.dll /reference:System.IO.Compression.FileSystem.dll (Join-Path $PSScriptRoot 'packaging\Bootstrap.cs') $infoFile
if ($LASTEXITCODE -ne 0) { throw 'Single-file package build failed.' }
$hashLine = (Get-FileHash -LiteralPath $launcher -Algorithm SHA256).Hash.ToLowerInvariant() + '  BroPad.exe'
[IO.File]::WriteAllText((Join-Path $OutputDir 'SHA256SUMS.txt'),$hashLine + [Environment]::NewLine)
[IO.File]::WriteAllText((Join-Path $OutputDir 'app-path.txt'),$appRoot)
Write-Output "Built: $launcher"
