@echo off
echo Starting all services...

start "BillingMaintenanceService (Port 5002)" cmd /k "cd /d f:\DocumentsDNU\Web\FullStack\repo\backend\services\BillingMaintenanceService\BillingMaintenanceService.API && dotnet run"

start "ContractStudentService (Port 5001)" cmd /k "cd /d f:\DocumentsDNU\Web\FullStack\repo\backend\services\ContractStudentService\ContractStudentService.API && dotnet run"

start "RoomBuildingService (Port 5003)" cmd /k "cd /d f:\DocumentsDNU\Web\FullStack\repo\backend\services\RoomBuildingService\RoomBuildingService.API && dotnet run"

timeout /t 5 /nobreak

start "API Gateway (Port 5000)" cmd /k "cd /d f:\DocumentsDNU\Web\FullStack\repo\backend\APIGateway && dotnet run"

echo.
echo All services started!
echo BillingMaintenanceService: http://localhost:5002
echo ContractStudentService: http://localhost:5001
echo RoomBuildingService: http://localhost:5003
echo API Gateway: http://localhost:5000
echo.
pause
