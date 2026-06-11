@echo off
echo ===================================
echo   Stopping All Services
echo ===================================
echo.

echo Stopping backend services...
taskkill /F /IM dotnet.exe 2>nul

echo Stopping frontend...
taskkill /F /IM node.exe 2>nul

echo.
echo All services stopped!
pause
