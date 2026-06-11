# PowerShell script to stop all services
Write-Host "`n===================================" -ForegroundColor Red
Write-Host "  Stopping All Services" -ForegroundColor Red
Write-Host "===================================" -ForegroundColor Red

Write-Host "`nStopping backend services..." -ForegroundColor Yellow
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host "Stopping frontend..." -ForegroundColor Yellow
Get-Process -Name "node" -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host "`nAll services stopped!" -ForegroundColor Green
Write-Host "`nPress any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
