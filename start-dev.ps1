# PowerShell script to start full stack application
Write-Host "`n===================================" -ForegroundColor Cyan
Write-Host "  Starting Full Stack Application" -ForegroundColor Cyan
Write-Host "===================================" -ForegroundColor Cyan

Write-Host "`n[1/2] Starting Backend Services..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-File", "$PSScriptRoot\backend\start-all.ps1"

Start-Sleep -Seconds 3

Write-Host "[2/2] Starting Frontend..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\frontend'; `$host.ui.RawUI.WindowTitle = 'Frontend (Vite Dev Server)'; npm run dev"

Write-Host "`n===================================" -ForegroundColor Green
Write-Host "  All services started!" -ForegroundColor Green
Write-Host "===================================" -ForegroundColor Green

Write-Host "`nBackend Services:" -ForegroundColor Cyan
Write-Host "  - BillingMaintenanceService: http://localhost:5002" -ForegroundColor White
Write-Host "  - ContractStudentService: http://localhost:5001" -ForegroundColor White
Write-Host "  - RoomBuildingService: http://localhost:5003" -ForegroundColor White
Write-Host "  - API Gateway: http://localhost:5000" -ForegroundColor Yellow

Write-Host "`nFrontend:" -ForegroundColor Cyan
Write-Host "  - Vite Dev Server: http://localhost:5173" -ForegroundColor Magenta

Write-Host "`nPress any key to exit (services will continue running)..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
