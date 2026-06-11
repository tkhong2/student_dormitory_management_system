# Comprehensive run script with options
param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("start", "stop", "restart", "status")]
    [string]$Action = "start"
)

$services = @(
    @{Name="BillingMaintenanceService"; Port=5002; Path="services\BillingMaintenanceService\BillingMaintenanceService.API"},
    @{Name="ContractStudentService"; Port=5001; Path="services\ContractStudentService\ContractStudentService.API"},
    @{Name="RoomBuildingService"; Port=5003; Path="services\RoomBuildingService\RoomBuildingService.API"},
    @{Name="APIGateway"; Port=5000; Path="APIGateway"}
)

function Start-Services {
    Write-Host "`n=== Starting All Services ===" -ForegroundColor Green
    
    foreach ($service in $services) {
        $fullPath = Join-Path $PSScriptRoot $service.Path
        Write-Host "Starting $($service.Name) on port $($service.Port)..." -ForegroundColor Cyan
        
        $title = "$($service.Name) (Port $($service.Port))"
        Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$fullPath'; `$host.ui.RawUI.WindowTitle = '$title'; dotnet run"
        
        Start-Sleep -Milliseconds 500
    }
    
    Write-Host "`nAll services started!" -ForegroundColor Green
    Write-Host "`nService URLs:" -ForegroundColor Yellow
    foreach ($service in $services) {
        Write-Host "  $($service.Name): http://localhost:$($service.Port)" -ForegroundColor White
    }
}

function Stop-Services {
    Write-Host "`n=== Stopping All Services ===" -ForegroundColor Red
    Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force
    Write-Host "All services stopped!" -ForegroundColor Green
}

function Restart-Services {
    Stop-Services
    Start-Sleep -Seconds 2
    Start-Services
}

function Show-Status {
    Write-Host "`n=== Service Status ===" -ForegroundColor Yellow
    
    foreach ($service in $services) {
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:$($service.Port)/health" -TimeoutSec 2 -ErrorAction SilentlyContinue
            Write-Host "  $($service.Name): " -NoNewline -ForegroundColor White
            Write-Host "RUNNING" -ForegroundColor Green
        } catch {
            Write-Host "  $($service.Name): " -NoNewline -ForegroundColor White
            Write-Host "STOPPED" -ForegroundColor Red
        }
    }
}

# Execute action
switch ($Action) {
    "start" { Start-Services }
    "stop" { Stop-Services }
    "restart" { Restart-Services }
    "status" { Show-Status }
}

Write-Host "`nPress any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
