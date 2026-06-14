@echo off
echo ========================================
echo Reset RoomBuildingDb Database
echo ========================================
echo.

sqlcmd -S localhost -E -d RoomBuildingDb -Q "DELETE FROM RoomImages; DELETE FROM RoomTypeAmenities; DELETE FROM Rooms; DELETE FROM RoomTypes; DELETE FROM Amenities; DELETE FROM Floors; DELETE FROM Buildings; DBCC CHECKIDENT ('RoomImages', RESEED, 0); DBCC CHECKIDENT ('RoomTypeAmenities', RESEED, 0); DBCC CHECKIDENT ('Rooms', RESEED, 0); DBCC CHECKIDENT ('RoomTypes', RESEED, 0); DBCC CHECKIDENT ('Amenities', RESEED, 0); DBCC CHECKIDENT ('Floors', RESEED, 0); DBCC CHECKIDENT ('Buildings', RESEED, 0);"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✅ Da reset thanh cong!
    echo 🔄 Vui long khoi dong lai RoomBuildingService.API de chay DataSeeder
) else (
    echo.
    echo ❌ Co loi xay ra. Kiem tra lai SQL Server.
)

pause
