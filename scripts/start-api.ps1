# OrderHub - Start API Server
# Inicia o servidor API ASP.NET Core

param(
    [switch]$Watch = $false
)

$ProjectPath = Join-Path (Join-Path $PSScriptRoot "..") "src\OrderHub.Adapters.Inbound.Api"

Write-Host ""
Write-Host "================================================" -ForegroundColor Magenta
Write-Host " Starting OrderHub API Server" -ForegroundColor Magenta
Write-Host "================================================" -ForegroundColor Magenta
Write-Host ""

if (-not (Test-Path $ProjectPath)) {
    Write-Host "  [X] API project not found at: $ProjectPath" -ForegroundColor Red
    exit 1
}

try {
    Push-Location $ProjectPath
    
    Write-Host "  [*] Starting API..." -ForegroundColor Cyan
    Write-Host ""
    
    if ($Watch) {
        Write-Host "  [*] Watch mode enabled - API will restart on file changes" -ForegroundColor Yellow
        dotnet watch run
    }
    else {
        dotnet run
    }
    
    Pop-Location
}
catch {
    Write-Host "  [X] Error starting API: $_" -ForegroundColor Red
    Pop-Location
    exit 1
}
