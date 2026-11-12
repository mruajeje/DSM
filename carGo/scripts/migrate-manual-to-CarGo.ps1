# Copia tu codigo MANUAL de CarGoGen.* a CarGo.* tras regenerar con projectName "CarGo".
# Ejecutar desde la raiz del proyecto (donde esta carGo.tdm), DESPUES de regenerar con HARIA:
#   .\scripts\migrate-manual-to-CarGo.ps1
# No borra nada: las carpetas CarGoGen.* antiguas se quedan para que las elimines tu cuando compruebes que todo compila.
param(
    [string]$Root = (Join-Path $PSScriptRoot ".."),
    [string]$Old = "CarGoGen",
    [string]$New = "CarGo",
    [switch]$Force
)
$gen = (Resolve-Path (Join-Path $Root "generated")).Path
if (-not (Test-Path (Join-Path $gen "$New.sln"))) {
    Write-Host "No existe $gen\$New.sln. Regenera primero con HARIA (con haria.config.json en la raiz)." -ForegroundColor Red
    exit 1
}
$bom = New-Object System.Text.UTF8Encoding($true)
$files = Get-ChildItem -Path $gen -Recurse -File -Filter *.cs | Where-Object {
    $_.FullName -match "\\$Old\.[^\\]+\\" -and $_.FullName -match "\\manual\\" -and $_.FullName -notmatch "\\(obj|bin)\\"
}
$copied = 0; $skipped = 0
foreach ($f in $files) {
    $rel = $f.FullName.Substring($gen.Length + 1)
    $newRel = $rel -replace ("^" + [regex]::Escape($Old) + "\."), ($New + ".")
    $dest = Join-Path $gen $newRel
    if ((Test-Path $dest) -and -not $Force) { Write-Host "Ya existe, se omite: $newRel" -ForegroundColor Yellow; $skipped++; continue }
    New-Item -ItemType Directory -Force (Split-Path $dest) | Out-Null
    $text = [System.IO.File]::ReadAllText($f.FullName) -replace [regex]::Escape($Old), $New
    [System.IO.File]::WriteAllText($dest, $text, $bom)
    Write-Host "OK  $newRel"
    $copied++
}
Write-Host "`nCopiados: $copied  Omitidos: $skipped. Ahora ejecuta: dotnet build generated\$New.sln" -ForegroundColor Green
