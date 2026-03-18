#!/usr/bin/env pwsh
# Script: run-tests-with-coverage.ps1
# Descrição: Executa todos os testes com coleta de cobertura de código
# Uso: .\scripts\run-tests-with-coverage.ps1

param(
    [string]$Configuration = "Debug",
    [string]$OutputPath = "coverage",
    [switch]$GenerateReport = $true,
    [switch]$NoRestore = $false
)

$ErrorActionPreference = "Stop"

# Cores para output
$Green = "`e[32m"
$Red = "`e[31m"
$Yellow = "`e[33m"
$Blue = "`e[34m"
$Reset = "`e[0m"

Write-Host "${Blue}=== Execução de Testes com Cobertura ===${Reset}"
Write-Host "Configuração: $Configuration"
Write-Host "Diretório de saída: $OutputPath"
Write-Host ""

# Criar diretório de saída se não existir
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
    Write-Host "${Green}✓ Diretório de saída criado${Reset}"
}

# Restaurar dependências
if (-not $NoRestore) {
    Write-Host "${Blue}→ Restaurando dependências...${Reset}"
    dotnet restore
    if ($LASTEXITCODE -ne 0) {
        Write-Host "${Red}✗ Erro ao restaurar dependências${Reset}"
        exit 1
    }
    Write-Host "${Green}✓ Dependências restauradas${Reset}"
}

# Build
Write-Host "${Blue}→ Compilando projeto...${Reset}"
dotnet build -c $Configuration --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "${Red}✗ Erro ao compilar${Reset}"
    exit 1
}
Write-Host "${Green}✓ Build concluído${Reset}"

# Executar testes com cobertura
Write-Host "${Blue}→ Executando testes com coleta de cobertura...${Reset}"
dotnet test `
    --no-build `
    -c $Configuration `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat="opencover,json" `
    /p:Exclude="[*]OrderHub.Domain.*|[*]Tests*" `
    /p:CoverletOutput="$OutputPath/" `
    --results-directory "$OutputPath/" `
    --logger "console;verbosity=normal" `
    --logger "trx;LogFilePrefix=TestResults"

if ($LASTEXITCODE -ne 0) {
    Write-Host "${Red}✗ Alguns testes falharam${Reset}"
    exit 1
}
Write-Host "${Green}✓ Todos os testes passaram${Reset}"

# Gerar relatório HTML se ReportGenerator estiver instalado
if ($GenerateReport) {
    Write-Host "${Blue}→ Gerando relatório de cobertura...${Reset}"
    
    # Verificar se reportgenerator está instalado
    $reportGeneratorPath = (where.exe dotnet-reportgenerator-globaltool 2>$null) | Select-Object -First 1
    
    if ($reportGeneratorPath -or (Test-Path "$env:USERPROFILE\.dotnet\tools\dotnet-reportgenerator-globaltool.exe")) {
        reportgenerator `
            -reports:"$OutputPath/coverage.opencover.xml" `
            -targetdir:"$OutputPath/report" `
            -reporttypes:"HtmlInline_AzurePipelines;MarkdownSummary" `
            -verbosity:"Verbose"
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "${Green}✓ Relatório HTML gerado em: $OutputPath/report/index.html${Reset}"
        }
        else {
            Write-Host "${Yellow}⚠ Não foi possível gerar relatório HTML${Reset}"
        }
    }
    else {
        Write-Host "${Yellow}⚠ reportgenerator não está instalado${Reset}"
        Write-Host "   Para instalar: dotnet tool install -g dotnet-reportgenerator-globaltool"
    }
}

Write-Host ""
Write-Host "${Green}=== Execução de Testes Concluída ===${Reset}"
Write-Host "Arquivos de cobertura gerados em: $OutputPath"
if ($GenerateReport -and (Test-Path "$OutputPath/report")) {
    Write-Host "Relatório HTML disponível em: $OutputPath/report/index.html"
}
Write-Host ""
