-- ==========================================
-- DATABASE HEALTH CHECK SCRIPT
-- ==========================================
-- Purpose: Quick verification of seeded data and database state
-- Usage: Run in SQL Server Management Studio or Azure Data Studio
-- Target: RoomBuildingDB (Service 1 - Room & Building Service)
-- ==========================================

PRINT '=========================================='
PRINT 'DATABASE HEALTH CHECK - RoomBuildingDB'
PRINT 'Timestamp: ' + CONVERT(VARCHAR, GETDATE(), 120)
PRINT '=========================================='
PRINT ''

-- ==========================================
-- 1. BUILDINGS DATA
-- ==========================================
PRINT '1. BUILDINGS DATA'
PRINT '------------------'
SELECT COUNT(*) as TotalBuildings FROM Buildings;
PRINT ''

-- Buildings overview
SELECT 
    Name,
    Gender,
    TotalFloors,
    TotalRooms,
    Status,
    HasElevator,
    HasParking,
    HasLaundry,
    ManagerName,
    ManagerPhone
FROM Buildings
ORDER BY Name;
PRINT ''

-- Buildings by status
SELECT 
    Status,
    COUNT(*) as Count,
    SUM(TotalRooms) as TotalRooms
FROM Buildings
GROUP BY Status;
PRINT ''

-- ==========================================
-- 2. ROOM TYPES DATA
-- ==========================================
PRINT '2. ROOM TYPES DATA'
PRINT '------------------'
SELECT COUNT(*) as TotalRoomTypes FROM RoomTypes;
PRINT ''

-- Room types with pricing
SELECT 
    rt.Code,
    rt.Name,
    rt.Capacity,
    FORMAT(rt.PricePerMonth, 'N0') as PricePerMonth,
    FORMAT(rt.DepositAmount, 'N0') as DepositAmount,
    FORMAT(rt.ElectricityRate, 'N0') as ElectricityRate,
    FORMAT(rt.WaterRate, 'N0') as WaterRate,
    rt.Area,
    rt.HasAirConditioner as AC,
    b.Name as Building,
    rt.IsActive
FROM RoomTypes rt
LEFT JOIN Buildings b ON rt.BuildingId = b.Id
ORDER BY b.Name, rt.Capacity;
PRINT ''

-- Price range statistics
SELECT 
    MIN(PricePerMonth) as MinPrice,
    MAX(PricePerMonth) as MaxPrice,
    AVG(PricePerMonth) as AvgPrice,
    MIN(Capacity) as MinCapacity,
    MAX(Capacity) as MaxCapacity
FROM RoomTypes
WHERE IsActive = 1;
PRINT ''

-- ==========================================
-- 3. FLOORS DATA
-- ==========================================
PRINT '3. FLOORS DATA'
PRINT '------------------'
SELECT COUNT(*) as TotalFloors FROM Floors;
PRINT ''

-- Floors by building
SELECT 
    b.Name as Building,
    COUNT(f.Id) as TotalFloors,
    SUM(f.TotalRooms) as TotalRoomsInFloors
FROM Buildings b
LEFT JOIN Floors f ON b.Id = f.BuildingId
GROUP BY b.Name
ORDER BY b.Name;
PRINT ''

-- ==========================================
-- 4. ROOMS DATA
-- ==========================================
PRINT '4. ROOMS DATA'
PRINT '------------------'
SELECT COUNT(*) as TotalRooms FROM Rooms;
PRINT ''

-- Rooms by status
SELECT 
    Status,
    COUNT(*) as Count,
    CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Rooms) AS DECIMAL(5,2)) as Percentage
FROM Rooms
GROUP BY Status
ORDER BY 
    CASE Status
        WHEN 'Available' THEN 1
        WHEN 'Full' THEN 2
        WHEN 'Maintenance' THEN 3
        WHEN 'Reserved' THEN 4
        WHEN 'Closed' THEN 5
        ELSE 6
    END;
PRINT ''

-- Occupancy statistics
SELECT 
    SUM(CurrentOccupants) as TotalOccupants,
    SUM(MaxOccupants) as TotalCapacity,
    SUM(MaxOccupants - CurrentOccupants) as AvailableSlots,
    CAST(SUM(CurrentOccupants) * 100.0 / NULLIF(SUM(MaxOccupants), 0) AS DECIMAL(5,2)) as OccupancyRate
FROM Rooms
WHERE Status NOT IN ('Closed', 'Maintenance');
PRINT ''

-- Occupancy by building
SELECT 
    b.Name as Building,
    b.Gender,
    COUNT(DISTINCT r.Id) as TotalRooms,
    SUM(r.CurrentOccupants) as CurrentOccupants,
    SUM(r.MaxOccupants) as Capacity,
    SUM(r.MaxOccupants - r.CurrentOccupants) as AvailableSlots,
    CAST(SUM(r.CurrentOccupants) * 100.0 / NULLIF(SUM(r.MaxOccupants), 0) AS DECIMAL(5,2)) as OccupancyRate
FROM Buildings b
LEFT JOIN Floors f ON b.Id = f.BuildingId
LEFT JOIN Rooms r ON f.Id = r.FloorId
WHERE r.Status NOT IN ('Closed', 'Maintenance')
GROUP BY b.Name, b.Gender
ORDER BY b.Name;
PRINT ''

-- Locked rooms
SELECT 
    'Locked rooms' as Check,
    COUNT(*) as Count
FROM Rooms
WHERE IsLocked = 1;
PRINT ''

-- ==========================================
-- 5. ROOM IMAGES
-- ==========================================
PRINT '5. ROOM IMAGES'
PRINT '------------------'
SELECT COUNT(*) as TotalRoomImages FROM RoomImages;
PRINT ''

-- Images per room statistics
SELECT 
    AVG(ImageCount) as AvgImagesPerRoom,
    MIN(ImageCount) as MinImages,
    MAX(ImageCount) as MaxImages
FROM (
    SELECT RoomId, COUNT(*) as ImageCount
    FROM RoomImages
    GROUP BY RoomId
) as ImageStats;
PRINT ''

-- Rooms without images
SELECT 
    'Rooms without images' as Check,
    COUNT(*) as Count
FROM Rooms r
LEFT JOIN RoomImages ri ON r.Id = ri.RoomId
WHERE ri.Id IS NULL;
PRINT ''

-- ==========================================
-- 6. AMENITIES
-- ==========================================
PRINT '6. AMENITIES'
PRINT '------------------'
SELECT COUNT(*) as TotalAmenities FROM Amenities;
PRINT ''

-- Amenities list
SELECT 
    Name,
    Icon,
    IsActive
FROM Amenities
ORDER BY Name;
PRINT ''

-- Room types with amenities count
SELECT 
    rt.Name as RoomType,
    COUNT(rta.AmenityId) as AmenitiesCount,
    b.Name as Building
FROM RoomTypes rt
LEFT JOIN RoomTypeAmenities rta ON rt.Id = rta.RoomTypeId
LEFT JOIN Buildings b ON rt.BuildingId = b.Id
GROUP BY rt.Name, b.Name
ORDER BY b.Name, rt.Name;
PRINT ''

-- ==========================================
-- 7. ROOM INSPECTIONS
-- ==========================================
PRINT '7. ROOM INSPECTIONS'
PRINT '------------------'
SELECT COUNT(*) as TotalInspections FROM RoomInspections;
PRINT ''

-- Inspections by result
SELECT 
    Result,
    COUNT(*) as Count
FROM RoomInspections
GROUP BY Result;
PRINT ''

-- Recent inspections
SELECT TOP 10
    ri.InspectedAt,
    r.RoomNumber,
    b.Name as Building,
    ri.Result,
    ri.InspectorName,
    ri.Notes
FROM RoomInspections ri
LEFT JOIN Rooms r ON ri.RoomId = r.Id
LEFT JOIN Floors f ON r.FloorId = f.Id
LEFT JOIN Buildings b ON f.BuildingId = b.Id
ORDER BY ri.InspectedAt DESC;
PRINT ''

-- Rooms never inspected
SELECT 
    'Rooms never inspected' as Check,
    COUNT(*) as Count
FROM Rooms r
LEFT JOIN RoomInspections ri ON r.Id = ri.RoomId
WHERE ri.Id IS NULL;
PRINT ''

-- ==========================================
-- 8. ROOM RESERVATIONS
-- ==========================================
PRINT '8. ROOM RESERVATIONS'
PRINT '------------------'
SELECT COUNT(*) as TotalReservations FROM RoomReservations;
PRINT ''

-- Reservations by status
SELECT 
    Status,
    COUNT(*) as Count
FROM RoomReservations
GROUP BY Status;
PRINT ''

-- Active reservations
SELECT 
    rr.ReservedForStudentCode,
    rr.ReservedForStudentName,
    r.RoomNumber,
    b.Name as Building,
    rr.ReservedFrom,
    rr.ReservedUntil,
    rr.Status
FROM RoomReservations rr
LEFT JOIN Rooms r ON rr.RoomId = r.Id
LEFT JOIN Floors f ON r.FloorId = f.Id
LEFT JOIN Buildings b ON f.BuildingId = b.Id
WHERE rr.Status = 'Active'
ORDER BY rr.ReservedUntil;
PRINT ''

-- ==========================================
-- 9. ROOM STATUS LOGS
-- ==========================================
PRINT '9. ROOM STATUS LOGS'
PRINT '------------------'
SELECT COUNT(*) as TotalStatusChanges FROM RoomStatusLogs;
PRINT ''

-- Recent status changes
SELECT TOP 10
    rsl.ChangedAt,
    r.RoomNumber,
    b.Name as Building,
    rsl.OldStatus,
    rsl.NewStatus,
    rsl.Reason,
    rsl.ChangedByName
FROM RoomStatusLogs rsl
LEFT JOIN Rooms r ON rsl.RoomId = r.Id
LEFT JOIN Floors f ON r.FloorId = f.Id
LEFT JOIN Buildings b ON f.BuildingId = b.Id
ORDER BY rsl.ChangedAt DESC;
PRINT ''

-- ==========================================
-- 10. BUILDING ANNOUNCEMENTS
-- ==========================================
PRINT '10. BUILDING ANNOUNCEMENTS'
PRINT '------------------'
SELECT COUNT(*) as TotalAnnouncements FROM BuildingAnnouncements;
PRINT ''

-- Active announcements
SELECT 
    ba.Title,
    ba.Priority,
    ba.PublishedAt,
    ba.ExpiresAt,
    b.Name as Building
FROM BuildingAnnouncements ba
LEFT JOIN Buildings b ON ba.BuildingId = b.Id
WHERE ba.IsActive = 1
    AND (ba.ExpiresAt IS NULL OR ba.ExpiresAt > GETDATE())
ORDER BY ba.PublishedAt DESC;
PRINT ''

-- ==========================================
-- 11. DATA INTEGRITY CHECKS
-- ==========================================
PRINT '11. DATA INTEGRITY CHECKS'
PRINT '------------------'

-- Rooms with CurrentOccupants > MaxOccupants (ERROR!)
SELECT 
    'Rooms OVER capacity' as Check,
    COUNT(*) as Count
FROM Rooms
WHERE CurrentOccupants > MaxOccupants;

-- Full rooms with available slots
SELECT 
    'Full status but has slots' as Check,
    COUNT(*) as Count
FROM Rooms
WHERE Status = 'Full' AND CurrentOccupants < MaxOccupants;

-- Available rooms at max capacity
SELECT 
    'Available but full capacity' as Check,
    COUNT(*) as Count
FROM Rooms
WHERE Status = 'Available' AND CurrentOccupants >= MaxOccupants;

-- Floors without rooms
SELECT 
    'Floors without rooms' as Check,
    COUNT(*) as Count
FROM Floors f
LEFT JOIN Rooms r ON f.Id = r.FloorId
WHERE r.Id IS NULL;

-- Room types without rooms
SELECT 
    'Room types without rooms' as Check,
    COUNT(*) as Count
FROM RoomTypes rt
LEFT JOIN Rooms r ON rt.Id = r.RoomTypeId
WHERE r.Id IS NULL AND rt.IsActive = 1;

-- Locked rooms without reason
SELECT 
    'Locked rooms without reason' as Check,
    COUNT(*) as Count
FROM Rooms
WHERE IsLocked = 1 AND LockReason IS NULL;

PRINT ''

-- ==========================================
-- 12. CAPACITY SUMMARY BY ROOM TYPE
-- ==========================================
PRINT '12. CAPACITY SUMMARY BY ROOM TYPE'
PRINT '------------------'
SELECT 
    rt.Name as RoomType,
    rt.Capacity,
    COUNT(DISTINCT r.Id) as TotalRooms,
    SUM(r.CurrentOccupants) as CurrentOccupants,
    SUM(r.MaxOccupants) as TotalCapacity,
    SUM(r.MaxOccupants - r.CurrentOccupants) as AvailableSlots,
    CAST(SUM(r.CurrentOccupants) * 100.0 / NULLIF(SUM(r.MaxOccupants), 0) AS DECIMAL(5,2)) as OccupancyRate,
    FORMAT(rt.PricePerMonth, 'N0') as PricePerMonth
FROM RoomTypes rt
LEFT JOIN Rooms r ON rt.Id = r.RoomTypeId
WHERE r.Status NOT IN ('Closed', 'Maintenance')
GROUP BY rt.Name, rt.Capacity, rt.PricePerMonth
ORDER BY rt.Capacity;
PRINT ''

PRINT '=========================================='
PRINT 'HEALTH CHECK COMPLETED'
PRINT '=========================================='
