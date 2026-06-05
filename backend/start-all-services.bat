@echo off
echo ==================================================
echo   Starting All Microservices and API Gateway
echo ==================================================
echo.

REM Start RoomBuildingService
echo Starting RoomBuildingService on port 5001...
start "RoomBuildingService" cmd /k "cd /d %~dp0services\RoomBuildingService\RoomBuildingService.API && dotnet run"
timeout /t 3 /nobreak >nul

REM Start ContractStudentService
echo Starting ContractStudentService on port 5059...
start "ContractStudentService" cmd /k "cd /d %~dp0services\ContractStudentService\ContractStudentService.API && dotnet run"
timeout /t 3 /nobreak >nul

REM Start BillingMaintenanceService
echo Starting BillingMaintenanceService on port 5002...
start "BillingMaintenanceService" cmd /k "cd /d %~dp0services\BillingMaintenanceService\BillingMaintenanceService.API && dotnet run"
timeout /t 3 /nobreak >nul

echo.
echo Waiting for services to start...
timeout /t 10 /nobreak >nul

REM Start API Gateway
echo Starting API Gateway on port 5052...
start "APIGateway" cmd /k "cd /d %~dp0APIGateway && dotnet run"

echo.
echo ==================================================
echo   All Services Started!
echo ==================================================
echo.
echo Services running on:
echo   - RoomBuildingService:       http://localhost:5001
echo   - ContractStudentService:    http://localhost:5059
echo   - BillingMaintenanceService: http://localhost:5002
echo   - API Gateway:               http://localhost:5052
echo.
echo Close this window to keep services running.
echo To stop all services, close all cmd windows.
pause
