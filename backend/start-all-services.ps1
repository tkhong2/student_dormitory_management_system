# Script để chạy tất cả microservices và API Gateway

Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  Starting All Microservices & API Gateway" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""

# Function to start a service in a new window
function Start-Service {
    param (
        [string]$ServiceName,
        [string]$Path,
        [string]$Port
    )
    
    Write-Host "Starting $ServiceName on port $Port..." -ForegroundColor Green
    
    Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd '$Path'; Write-Host '=== $ServiceName ===' -ForegroundColor Yellow; dotnet run"
    
    Start-Sleep -Seconds 2
}

# Start services
Start-Service -ServiceName "RoomBuildingService" -Path "$PSScriptRoot\services\RoomBuildingService\RoomBuildingService.API" -Port "5001"

Start-Service -ServiceName "ContractStudentService" -Path "$PSScriptRoot\services\ContractStudentService\ContractStudentService.API" -Port "5059"

Start-Service -ServiceName "BillingMaintenanceService" -Path "$PSScriptRoot\services\BillingMaintenanceService\BillingMaintenanceService.API" -Port "5002"

Write-Host ""
Write-Host "Waiting for services to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Start API Gateway
Start-Service -ServiceName "APIGateway" -Path "$PSScriptRoot\APIGateway" -Port "5052"

Write-Host ""
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "  All Services Started!" -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Services running on:" -ForegroundColor White
Write-Host "  - RoomBuildingService:       http://localhost:5001" -ForegroundColor Gray
Write-Host "  - ContractStudentService:    http://localhost:5059" -ForegroundColor Gray
Write-Host "  - BillingMaintenanceService: http://localhost:5002" -ForegroundColor Gray
Write-Host "  - API Gateway:               http://localhost:5052" -ForegroundColor Yellow
Write-Host ""
Write-Host "Press any key to stop all services..." -ForegroundColor Red
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# Stop all dotnet processes
Write-Host ""
Write-Host "Stopping all services..." -ForegroundColor Yellow
Stop-Process -Name "dotnet" -Force -ErrorAction SilentlyContinue
Write-Host "All services stopped." -ForegroundColor Green
