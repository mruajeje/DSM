# Copia de seguridad de la BD. Ejecutar desde la raiz del proyecto (donde esta carGo.tdm):  .\scripts\backup-db.ps1
$db = Join-Path $PSScriptRoot "..\generated\InitializeDB\CarGo.db"
if (-not (Test-Path $db)) { Write-Host "No existe $db"; exit 1 }
$dir = Join-Path $PSScriptRoot "..\database\backups"
New-Item -ItemType Directory -Force $dir | Out-Null
$dest = Join-Path $dir ("CarGo_{0}.db" -f (Get-Date -Format "yyyyMMdd_HHmmss"))
Copy-Item $db $dest
Write-Host "Backup: $dest"
