#!/usr/bin/env pwsh
# Script: check-coverage-threshold.ps1
# Descrição: Valida se a cobertura de código atende ao mínimo especificado
# Uso: .\scripts\check-coverage-threshold.ps1 -CoverageFilePath "coverage/coverage.json" -MinimumCoverage 0.80

param(
    [string]$CoverageFilePath = "coverage/coverage.json",
    [decimal]$MinimumCoverage = 0.80,
    [switch]$Verbose = $false
)

$ErrorActionPreference = "Stop"

# Cores para output
$Green = "`e[32m"
$Red = "`e[31m"
$Yellow = "`e[33m"
$Blue = "`e[34m"
$Reset = "`e[0m"

Write-Host "${Blue}=== Verificação de Cobertura de Código ===${Reset}"
Write-Host "Arquivo de cobertura: $CoverageFilePath"
Write-Host "Cobertura mínima esperada: $([math]::Round($MinimumCoverage * 100))%"
Write-Host ""

# Verificar se arquivo de cobertura existe
if (-not (Test-Path $CoverageFilePath)) {
    Write-Host "${Red}✗ Arquivo de cobertura não encontrado: $CoverageFilePath${Reset}"
    Write-Host "  Certifique-se de que os testes foram executados com coleta de cobertura"
    exit 1
}

# Ler arquivo JSON
try {
    $coverageData = Get-Content $CoverageFilePath | ConvertFrom-Json
}
catch {
    Write-Host "${Red}✗ Erro ao ler arquivo de cobertura: $_${Reset}"
    exit 1
}

# Extrair informações de cobertura
$lineCoverageStr = $coverageData.Summary.'Line coverage'
$branchCoverageStr = $coverageData.Summary.'Branch coverage'

# Parse percentages
if ($lineCoverageStr -match "(\d+(?:\.\d+)?)%") {
    $lineCoverage = [decimal]($matches[1]) / 100
}
else {
    Write-Host "${Red}✗ Não foi possível extrair cobertura de linha${Reset}"
    exit 1
}

if ($branchCoverageStr -match "(\d+(?:\.\d+)?)%") {
    $branchCoverage = [decimal]($matches[1]) / 100
}
else {
    $branchCoverage = 0
}

# Exibir informações de cobertura
Write-Host "Cobertura de linha: ${Green}$([math]::Round($lineCoverage * 100, 2))%${Reset}"
Write-Host "Cobertura de branch: ${Yellow}$([math]::Round($branchCoverage * 100, 2))%${Reset}"

# Detalhar por assembly se verbose
if ($Verbose -and $coverageData.Assembly) {
    Write-Host ""
    Write-Host "${Blue}Cobertura por Assembly:${Reset}"
    foreach ($assembly in $coverageData.Assembly) {
        $asemblyName = $assembly.name
        $assemblyCoverage = $assembly.Summary.'Line coverage'
        Write-Host "  - $asemblyName`: $assemblyCoverage"
    }
}

Write-Host ""

# Validar threshold
if ($lineCoverage -lt $MinimumCoverage) {
    Write-Host "${Red}✗ FALHA: Cobertura está abaixo do mínimo especificado${Reset}"
    Write-Host "  Esperado: $([math]::Round($MinimumCoverage * 100))%"
    Write-Host "  Obtido: $([math]::Round($lineCoverage * 100, 2))%"
    Write-Host "  Diferença: -$([math]::Round(($MinimumCoverage - $lineCoverage) * 100, 2))%"
    Write-Host ""
    exit 1
}

Write-Host "${Green}✓ SUCESSO: Cobertura atende ao mínimo de $([math]::Round($MinimumCoverage * 100))%${Reset}"
Write-Host ""
exit 0
