powershell.exe -NoProfile -ExecutionPolicy unrestricted -Command "& {Import-Module '.\tools\psake\psake.psm1'; invoke-psake .\tools\psake\default.ps1 %1 -parameters @{"version"="'%2'";"appPrefix"="'%3'";"filePath"="'%4'"}; if ($lastexitcode -ne 0) {write-host "ERROR: $lastexitcode" -fore RED; exit $lastexitcode} }" 