$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

$services = @(
  @{ Name = 'BillingMaintenanceService'; Path = 'backend\services\BillingMaintenanceService\BillingMaintenanceService.API\BillingMaintenanceService.API.csproj' },
  @{ Name = 'ContractStudentService'; Path = 'backend\services\ContractStudentService\ContractStudentService.API\ContractStudentService.API.csproj' },
  @{ Name = 'RoomBuildingService'; Path = 'backend\services\RoomBuildingService\RoomBuildingService.API\RoomBuildingService.API.csproj' }
)

foreach ($svc in $services) {
  $projectPath = Join-Path $scriptRoot $svc.Path
  if (-not (Test-Path $projectPath)) {
    Write-Error "Project file not found: $projectPath"
    continue
  }

  Write-Host "Starting $($svc.Name) ..."
  Start-Process powershell -ArgumentList @(
    '-NoExit',
    '-Command', "cd `"$(Split-Path -Parent $projectPath)`"; dotnet run"
  ) -NoNewWindow:$false
}

Write-Host 'Started all backend services. Each service runs in its own terminal window.'
