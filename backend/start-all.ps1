# PowerShell script to run all services concurrently
Write-Host "Starting all services..." -ForegroundColor Green

# Start each service in a new PowerShell window
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'f:\DocumentsDNU\Web\FullStack\repo\backend\services\BillingMaintenanceService\BillingMaintenanceService.API'; Write-Host 'Starting BillingMaintenanceService on port 5002...' -ForegroundColor Cyan; dotnet run"

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'f:\DocumentsDNU\Web\FullStack\repo\backend\services\ContractStudentService\ContractStudentService.API'; Write-Host 'Starting ContractStudentService on port 5001...' -ForegroundColor Cyan; dotnet run"

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'f:\DocumentsDNU\Web\FullStack\repo\backend\services\RoomBuildingService\RoomBuildingService.API'; Write-Host 'Starting RoomBuildingService on port 5003...' -ForegroundColor Cyan; dotnet run"

# Wait a bit for services to start
Start-Sleep -Seconds 5

# Start API Gateway last
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd 'f:\DocumentsDNU\Web\FullStack\repo\backend\APIGateway'; Write-Host 'Starting API Gateway on port 5000...' -ForegroundColor Yellow; dotnet run"

Write-Host "`nAll services started in separate windows!" -ForegroundColor Green
Write-Host "BillingMaintenanceService: http://localhost:5002" -ForegroundColor Cyan
Write-Host "ContractStudentService: http://localhost:5001" -ForegroundColor Cyan
Write-Host "RoomBuildingService: http://localhost:5003" -ForegroundColor Cyan
Write-Host "API Gateway: http://localhost:5000" -ForegroundColor Yellow
Write-Host "`nPress any key to exit (services will continue running)..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
