# OrderHub - Setup Environment Script
# Configuracoes completas do ambiente

param(
    [switch]$SkipTests = $false,
    [switch]$SkipCoverage = $false,
    [switch]$SkipAnalysis = $false,
    [string]$DbServer = "(localdb)\mssqllocaldb",
    [string]$DbName = "OrderHubDb"
)

# Colors
$Colors = @{
    Success  = 'Green'
    Warning  = 'Yellow'
    Error    = 'Red'
    Info     = 'Cyan'
    Section  = 'Magenta'
}

function Write-Header {
    param([string]$Message)
    Write-Host ""
    Write-Host "================================================" -ForegroundColor $Colors.Section
    Write-Host " $Message" -ForegroundColor $Colors.Section
    Write-Host "================================================" -ForegroundColor $Colors.Section
}

function Write-Success {
    param([string]$Message)
    Write-Host "  [OK] $Message" -ForegroundColor $Colors.Success
}

function Write-Info {
    param([string]$Message)
    Write-Host "  [*] $Message" -ForegroundColor $Colors.Info
}

function Write-Warning {
    param([string]$Message)
    Write-Host "  [!] $Message" -ForegroundColor $Colors.Warning
}

function Write-ErrorMsg {
    param([string]$Message)
    Write-Host "  [X] $Message" -ForegroundColor $Colors.Error
}

# Test prerequisites
function Test-Prerequisites {
    Write-Header "STAGE 1: VALIDATING PREREQUISITES"
    
    # Check .NET
    Write-Info "Checking .NET SDK..."
    try {
        $dotnetVersion = dotnet --version
        Write-Success ".NET $dotnetVersion found"
    }
    catch {
        Write-ErrorMsg ".NET SDK not found. Install from https://dotnet.microsoft.com/download"
        return $false
    }
    
    # Check solution file
    Write-Info "Checking solution file..."
    $solutionPath = Join-Path (Join-Path $PSScriptRoot "..") "OrderHub.slnx"
    if (Test-Path $solutionPath) {
        Write-Success "Solution file found"
    }
    else {
        Write-ErrorMsg "Solution file not found"
        return $false
    }
    
    return $true
}

# Restore dependencies
function Invoke-Restore {
    Write-Header "STAGE 2: RESTORING DEPENDENCIES"
    
    try {
        Push-Location (Join-Path $PSScriptRoot "..")
        Write-Info "Running dotnet restore..."
        dotnet restore 2>&1 | Out-Null
        Write-Success "Dependencies restored"
        Pop-Location
        return $true
    }
    catch {
        Write-ErrorMsg "Failed to restore dependencies"
        Pop-Location
        return $false
    }
}

# Build project
function Invoke-Build {
    Write-Header "STAGE 3: BUILDING PROJECT"
    
    try {
        Push-Location (Join-Path $PSScriptRoot "..")
        Write-Info "Running dotnet build..."
        dotnet build --configuration Release 2>&1 | Out-Null
        Write-Success "Build completed successfully"
        Pop-Location
        return $true
    }
    catch {
        Write-ErrorMsg "Build failed"
        Pop-Location
        return $false
    }
}

# Apply database migrations
function Invoke-Migrations {
    Write-Header "STAGE 4: DATABASE MIGRATIONS"
    
    try {
        Push-Location (Join-Path $PSScriptRoot ".." "src" "OrderHub.Adapters.Inbound.Api")
        Write-Info "Applying database migrations..."
        dotnet ef database update `
            --project ../OrderHub.Adapters.Outbound.Persistence `
            --connection "Server=$DbServer;Database=$DbName;Trusted_Connection=true;Encrypt=false;" 2>&1 | Out-Null
        Write-Success "Database updated"
        Pop-Location
        return $true
    }
    catch {
        Write-ErrorMsg "Database migration failed"
        Pop-Location
        return $false
    }
}

# Run tests
function Invoke-Tests {
    Write-Header "STAGE 5: RUNNING TESTS"
    
    try {
        Push-Location (Join-Path $PSScriptRoot "..")
        Write-Info "Running unit tests..."
        dotnet test --no-build --configuration Release 2>&1 | Out-Null
        Write-Success "Tests completed"
        Pop-Location
        return $true
    }
    catch {
        Write-ErrorMsg "Tests failed"
        Pop-Location
        return $false
    }
}

# Collect coverage
function Invoke-Coverage {
    Write-Header "STAGE 6: COLLECTING COVERAGE"
    
    try {
        Push-Location (Join-Path $PSScriptRoot "..")
        
        if (-not (Test-Path "coverage")) {
            New-Item -ItemType Directory -Path "coverage" | Out-Null
        }
        
        Write-Info "Collecting coverage metrics..."
        dotnet test --no-build --configuration Release `
            /p:CollectCoverage=true `
            /p:CoverletOutputFormat=json `
            /p:CoverletOutput="$(Get-Location)/coverage/" `
            --results-directory coverage/ 2>&1 | Out-Null
        
        Write-Success "Coverage collected"
        Pop-Location
        return $true
    }
    catch {
        Write-ErrorMsg "Coverage collection failed"
        Pop-Location
        return $false
    }
}

# Main setup
function Main {
    $startTime = Get-Date
    
    Write-Header "SETUP ORDERHUB ENVIRONMENT"
    
    Write-Host ""
    Write-Host "Configuration:" -ForegroundColor $Colors.Section
    Write-Host "  Database Server: $DbServer"
    Write-Host "  Database Name: $DbName"
    Write-Host "  Skip Tests: $SkipTests"
    Write-Host "  Skip Coverage: $SkipCoverage"
    Write-Host ""
    
    # Prerequisites
    if (-not (Test-Prerequisites)) {
        Write-Header "SETUP FAILED"
        return $false
    }
    
    # Restore
    if (-not (Invoke-Restore)) {
        Write-Header "SETUP FAILED"
        return $false
    }
    
    # Build
    if (-not (Invoke-Build)) {
        Write-Header "SETUP FAILED"
        return $false
    }
    
    # Migrations
    if (-not (Invoke-Migrations)) {
        Write-Warning "Database migration failed - continuing anyway"
    }
    
    # Tests
    if (-not $SkipTests) {
        if (-not (Invoke-Tests)) {
            Write-Warning "Some tests failed"
        }
        
        if (-not $SkipCoverage) {
            if (-not (Invoke-Coverage)) {
                Write-Warning "Coverage collection failed"
            }
        }
    }
    
    $endTime = Get-Date
    $duration = $endTime - $startTime
    
    Write-Header "SETUP COMPLETED"
    Write-Host ""
    Write-Host "Setup completed in $($duration.ToString('hh\:mm\:ss'))" -ForegroundColor $Colors.Success
    Write-Host ""
    Write-Host "Next Steps:" -ForegroundColor $Colors.Section
    Write-Host "  1. Start API: cd src/OrderHub.Adapters.Inbound.Api && dotnet run"
    Write-Host "  2. Access Swagger: https://localhost:7000/swagger"
    Write-Host "  3. View coverage: coverage/report/index.html"
    Write-Host ""
    
    return $true
}

# Execute
$success = Main
exit $(if ($success) { 0 } else { 1 })
