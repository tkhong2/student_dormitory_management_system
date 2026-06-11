@echo off
echo ===================================
echo   Starting Full Stack Application
echo ===================================
echo.

echo [1/2] Starting Backend Services...
cd backend
start "Backend Services" cmd /k "start-all.bat"
cd ..

echo [2/2] Starting Frontend...
timeout /t 3 /nobreak > nul
cd frontend
start "Frontend (Vite Dev Server)" cmd /k "npm run dev"
cd ..

echo.
echo ===================================
echo   All services started!
echo ===================================
echo.
echo Backend Services:
echo   - BillingMaintenanceService: http://localhost:5002
echo   - ContractStudentService: http://localhost:5001
echo   - RoomBuildingService: http://localhost:5003
echo   - API Gateway: http://localhost:5000
echo.
echo Frontend:
echo   - Vite Dev Server: http://localhost:5173
echo.
echo Press any key to exit (services will continue running)
pause > nul
