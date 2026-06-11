# PowerShell script to stop all dotnet processes
Write-Host "Stopping all .NET services..." -ForegroundColor Red

# Kill all dotnet.exe processes
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host "All services stopped!" -ForegroundColor Green
