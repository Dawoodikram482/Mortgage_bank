# BuyMyHouse - Start All Services Script
# This script starts all required services for local development

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  BuyMyHouse - Starting All Services" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Get the script directory (project root)
$ProjectRoot = $PSScriptRoot
if (-not $ProjectRoot) {
    $ProjectRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
}

Write-Host "Project Root: $ProjectRoot" -ForegroundColor Gray
Write-Host ""

# Check if Azurite is installed
$azuriteInstalled = Get-Command azurite -ErrorAction SilentlyContinue
if (-not $azuriteInstalled) {
    Write-Host "WARNING: Azurite not found!" -ForegroundColor Yellow
    Write-Host "Install it with: npm install -g azurite" -ForegroundColor Yellow
    Write-Host "Or install the Azurite VS Code extension" -ForegroundColor Yellow
    Write-Host ""
}

# Start Azurite
Write-Host "[1/4] Starting Azurite (Azure Storage Emulator)..." -ForegroundColor Green
if ($azuriteInstalled) {
    $azuriteDir = Join-Path $env:TEMP "azurite"
    if (-not (Test-Path $azuriteDir)) {
        New-Item -ItemType Directory -Path $azuriteDir -Force | Out-Null
    }
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "Write-Host 'Azurite Storage Emulator' -ForegroundColor Cyan; azurite --silent --location '$azuriteDir'"
    Start-Sleep -Seconds 3
} else {
    Write-Host "Skipping Azurite (not installed)" -ForegroundColor Yellow
}

# Start Listings API
Write-Host "[2/4] Starting Listings API (Port 5001)..." -ForegroundColor Green
$listingsPath = Join-Path $ProjectRoot "BuyMyHouse.Listings"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$listingsPath'; Write-Host 'BuyMyHouse Listings API' -ForegroundColor Cyan; dotnet run"

Start-Sleep -Seconds 2

# Start Mortgage API
Write-Host "[3/4] Starting Mortgage API (Port 5002)..." -ForegroundColor Green
$mortgagePath = Join-Path $ProjectRoot "BuyMyHouse.Mortgage"
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$mortgagePath'; Write-Host 'BuyMyHouse Mortgage API' -ForegroundColor Cyan; dotnet run"

Start-Sleep -Seconds 2

# Start Azure Functions
Write-Host "[4/4] Starting Azure Functions (Port 7071)..." -ForegroundColor Green
$funcInstalled = Get-Command func -ErrorAction SilentlyContinue
if ($funcInstalled) {
    $functionsPath = Join-Path $ProjectRoot "BuyMyHouse.Functions"
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$functionsPath'; Write-Host 'BuyMyHouse Azure Functions' -ForegroundColor Cyan; func start"
} else {
    Write-Host "WARNING: Azure Functions Core Tools not found!" -ForegroundColor Yellow
    Write-Host "Install with: npm install -g azure-functions-core-tools@4 --unsafe-perm true" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  All Services Started!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Access the services at:" -ForegroundColor White
Write-Host "  • Listings API:  https://localhost:5001/swagger" -ForegroundColor Yellow
Write-Host "  • Mortgage API:  https://localhost:5002/swagger" -ForegroundColor Yellow
Write-Host "  • Azure Functions: http://localhost:7071" -ForegroundColor Yellow
Write-Host ""
Write-Host "Press any key to open browsers..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# Open browsers
Start-Process "https://localhost:5001/swagger"
Start-Process "https://localhost:5002/swagger"

Write-Host ""
Write-Host "Tip: See TESTING.md for API testing commands" -ForegroundColor Cyan
