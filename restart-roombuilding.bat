@echo off
echo Restarting RoomBuildingService...

REM Kill any running dotnet process on port 5003
for /f "tokens=5" %%a in ('netstat -ano ^| findstr :5003 ^| findstr LISTENING') do taskkill /F /PID %%a 2>nul

echo Waiting 2 seconds...
timeout /t 2 /nobreak >nul

echo Starting RoomBuildingService...
cd backend\services\RoomBuildingService\RoomBuildingService.API
start "RoomBuildingService" dotnet run --urls "http://localhost:5003"

echo RoomBuildingService restarted!
echo Check the console window for seeding progress...
pause
