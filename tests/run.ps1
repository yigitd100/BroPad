param([string]$OutputDir = (Join-Path (Split-Path $PSScriptRoot -Parent) 'dist'))
$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$appRoot = [IO.File]::ReadAllText((Join-Path $OutputDir 'app-path.txt'))
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$checks = Join-Path $appRoot 'Checks.exe'
Copy-Item -LiteralPath (Join-Path $appRoot 'BroPad.App.exe.config') -Destination ($checks + '.config')
& $compiler /nologo "/out:$checks" "/win32manifest:$repo\packaging\app.manifest" "/reference:$appRoot\BroPad.App.exe" /reference:System.Drawing.dll /reference:System.Windows.Forms.dll (Join-Path $PSScriptRoot 'Checks.cs')
if ($LASTEXITCODE -ne 0) { throw 'Test build failed.' }
$lines = & $checks
if ($LASTEXITCODE -ne 0) { throw ($lines -join [Environment]::NewLine) }
$packageReport = Join-Path $OutputDir 'package-check.txt'
$process = Start-Process -FilePath (Join-Path $OutputDir 'BroPad.exe') -ArgumentList @('--verify-package',('"' + $packageReport + '"')) -Wait -PassThru -WindowStyle Hidden
if ($process.ExitCode -ne 0) {
    if (Test-Path -LiteralPath $packageReport) { Get-Content -LiteralPath $packageReport | Write-Output }
    throw "Embedded package validation failed (exit code: $($process.ExitCode))."
}
$lines += Get-Content -LiteralPath $packageReport
[IO.File]::WriteAllLines((Join-Path $OutputDir 'validation.txt'),[string[]]$lines)
$lines | Write-Output
