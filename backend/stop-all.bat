@echo off
echo Stopping all .NET services...
taskkill /F /IM dotnet.exe
echo All services stopped!
pause
