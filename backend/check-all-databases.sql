-- ==========================================
-- MASTER DATABASE HEALTH CHECK SCRIPT
-- ==========================================
-- Purpose: Run health checks on ALL 3 microservices databases
-- Usage: Run in SQL Server Management Studio or Azure Data Studio
-- Targets: All 3 databases (RoomBuildingDB, ContractStudentDB, BillingMaintenanceDB)
-- ==========================================

PRINT ''
PRINT '######################################################'
PRINT '#                                                    #'
PRINT '#    DORMITORY MANAGEMENT SYSTEM                    #'
PRINT '#    FULL DATABASE HEALTH CHECK                     #'
PRINT '#                                                    #'
PRINT '######################################################'
PRINT ''
PRINT 'Timestamp: ' + CONVERT(VARCHAR, GETDATE(), 120)
PRINT ''
PRINT 'This script will check all 3 microservices databases:'
PRINT '  1. RoomBuildingDB (Service 1 - Room & Building)'
PRINT '  2. ContractStudentDB (Service 2 - Contract & Student)'
PRINT '  3. BillingMaintenanceDB (Service 3 - Billing & Maintenance)'
PRINT ''
PRINT '======================================================='
PRINT ''

-- ==========================================
-- DATABASE CONNECTIVITY CHECK
-- ==========================================
PRINT 'STEP 1: Checking database connectivity...'
PRINT ''

DECLARE @DbExists1 BIT = 0
DECLARE @DbExists2 BIT = 0
DECLARE @DbExists3 BIT = 0

IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'RoomBuildingDB')
BEGIN
    SET @DbExists1 = 1
    PRINT '✓ RoomBuildingDB: Connected'
END
ELSE
BEGIN
    PRINT '✗ RoomBuildingDB: NOT FOUND'
END

IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'ContractStudentDB')
BEGIN
    SET @DbExists2 = 1
    PRINT '✓ ContractStudentDB: Connected'
END
ELSE
BEGIN
    PRINT '✗ ContractStudentDB: NOT FOUND'
END

IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'BillingMaintenanceDB')
BEGIN
    SET @DbExists3 = 1
    PRINT '✓ BillingMaintenanceDB: Connected'
END
ELSE
BEGIN
    PRINT '✗ BillingMaintenanceDB: NOT FOUND'
END

PRINT ''
PRINT '======================================================='
PRINT ''

-- ==========================================
-- QUICK SUMMARY FROM ALL DATABASES
-- ==========================================
PRINT 'STEP 2: Quick summary from all databases...'
PRINT ''

-- Summary table
DECLARE @SummaryTable TABLE (
    Service VARCHAR(50),
    Metric VARCHAR(50),
    Value VARCHAR(100)
)

-- Service 1: RoomBuildingDB
IF @DbExists1 = 1
BEGIN
    INSERT INTO @SummaryTable
    SELECT 'Service 1 (Room)', 'Total Buildings', CAST(COUNT(*) AS VARCHAR)
    FROM RoomBuildingDB.dbo.Buildings

    INSERT INTO @SummaryTable
    SELECT 'Service 1 (Room)', 'Total Rooms', CAST(COUNT(*) AS VARCHAR)
    FROM RoomBuildingDB.dbo.Rooms

    INSERT INTO @SummaryTable
    SELECT 'Service 1 (Room)', 'Total Capacity', CAST(SUM(MaxOccupants) AS VARCHAR)
    FROM RoomBuildingDB.dbo.Rooms

    INSERT INTO @SummaryTable
    SELECT 'Service 1 (Room)', 'Current Occupants', CAST(SUM(CurrentOccupants) AS VARCHAR)
    FROM RoomBuildingDB.dbo.Rooms

    INSERT INTO @SummaryTable
    SELECT 'Service 1 (Room)', 'Available Slots', CAST(SUM(MaxOccupants - CurrentOccupants) AS VARCHAR)
    FROM RoomBuildingDB.dbo.Rooms
    WHERE Status NOT IN ('Closed', 'Maintenance')

    INSERT INTO @SummaryTable
    SELECT 'Service 1 (Room)', 'Occupancy Rate', 
        CAST(CAST(SUM(CurrentOccupants) * 100.0 / NULLIF(SUM(MaxOccupants), 0) AS DECIMAL(5,2)) AS VARCHAR) + '%'
    FROM RoomBuildingDB.dbo.Rooms
    WHERE Status NOT IN ('Closed', 'Maintenance')
END

-- Service 2: ContractStudentDB
IF @DbExists2 = 1
BEGIN
    INSERT INTO @SummaryTable
    SELECT 'Service 2 (Contract)', 'Total Students', CAST(COUNT(*) AS VARCHAR)
    FROM ContractStudentDB.dbo.Students

    INSERT INTO @SummaryTable
    SELECT 'Service 2 (Contract)', 'Total Applications', CAST(COUNT(*) AS VARCHAR)
    FROM ContractStudentDB.dbo.RoomApplications

    INSERT INTO @SummaryTable
    SELECT 'Service 2 (Contract)', 'Pending Applications', CAST(COUNT(*) AS VARCHAR)
    FROM ContractStudentDB.dbo.RoomApplications
    WHERE Status = 'Pending'

    INSERT INTO @SummaryTable
    SELECT 'Service 2 (Contract)', 'Total Contracts', CAST(COUNT(*) AS VARCHAR)
    FROM ContractStudentDB.dbo.Contracts

    INSERT INTO @SummaryTable
    SELECT 'Service 2 (Contract)', 'Active Contracts', CAST(COUNT(*) AS VARCHAR)
    FROM ContractStudentDB.dbo.Contracts
    WHERE Status = 'Active'

    INSERT INTO @SummaryTable
    SELECT 'Service 2 (Contract)', 'Total Check-ins', CAST(COUNT(*) AS VARCHAR)
    FROM ContractStudentDB.dbo.CheckIns
END

-- Service 3: BillingMaintenanceDB
IF @DbExists3 = 1
BEGIN
    INSERT INTO @SummaryTable
    SELECT 'Service 3 (Billing)', 'Total Users', CAST(COUNT(*) AS VARCHAR)
    FROM BillingMaintenanceDB.dbo.Users

    INSERT INTO @SummaryTable
    SELECT 'Service 3 (Billing)', 'Total Invoices', CAST(COUNT(*) AS VARCHAR)
    FROM BillingMaintenanceDB.dbo.Invoices

    INSERT INTO @SummaryTable
    SELECT 'Service 3 (Billing)', 'Unpaid Invoices', CAST(COUNT(*) AS VARCHAR)
    FROM BillingMaintenanceDB.dbo.Invoices
    WHERE Status IN ('Unpaid', 'PartialPaid', 'Overdue')

    INSERT INTO @SummaryTable
    SELECT 'Service 3 (Billing)', 'Total Revenue (VNĐ)', 
        FORMAT(SUM(TotalAmount), 'N0')
    FROM BillingMaintenanceDB.dbo.Invoices
    WHERE Status != 'Cancelled'

    INSERT INTO @SummaryTable
    SELECT 'Service 3 (Billing)', 'Collected (VNĐ)', 
        FORMAT(SUM(PaidAmount), 'N0')
    FROM BillingMaintenanceDB.dbo.Invoices
    WHERE Status != 'Cancelled'

    INSERT INTO @SummaryTable
    SELECT 'Service 3 (Billing)', 'Outstanding Debt (VNĐ)', 
        FORMAT(SUM(DebtAmount), 'N0')
    FROM BillingMaintenanceDB.dbo.Invoices
    WHERE Status IN ('Unpaid', 'PartialPaid', 'Overdue')

    INSERT INTO @SummaryTable
    SELECT 'Service 3 (Billing)', 'Maintenance Requests', CAST(COUNT(*) AS VARCHAR)
    FROM BillingMaintenanceDB.dbo.MaintenanceRequests

    INSERT INTO @SummaryTable
    SELECT 'Service 3 (Billing)', 'Pending Requests', CAST(COUNT(*) AS VARCHAR)
    FROM BillingMaintenanceDB.dbo.MaintenanceRequests
    WHERE Status IN ('New', 'Assigned', 'InProgress')
END

-- Display summary
SELECT 
    Service,
    Metric,
    Value
FROM @SummaryTable
ORDER BY Service, Metric

PRINT ''
PRINT '======================================================='
PRINT ''

-- ==========================================
-- CROSS-SERVICE DATA CONSISTENCY CHECKS
-- ==========================================
PRINT 'STEP 3: Cross-service data consistency checks...'
PRINT ''

IF @DbExists1 = 1 AND @DbExists2 = 1
BEGIN
    PRINT '--- Checking Room Occupancy vs Active Contracts ---'
    
    -- Compare room occupancy with active contracts count
    SELECT 
        r.RoomNumber,
        b.Name as Building,
        r.CurrentOccupants as SystemOccupants,
        COUNT(c.Id) as ActiveContractsCount,
        CASE 
            WHEN r.CurrentOccupants = COUNT(c.Id) THEN '✓ Match'
            ELSE '✗ MISMATCH'
        END as Status
    FROM RoomBuildingDB.dbo.Rooms r
    LEFT JOIN RoomBuildingDB.dbo.Floors f ON r.FloorId = f.Id
    LEFT JOIN RoomBuildingDB.dbo.Buildings b ON f.BuildingId = b.Id
    LEFT JOIN ContractStudentDB.dbo.Contracts c ON r.Id = c.RoomId AND c.Status = 'Active'
    GROUP BY r.Id, r.RoomNumber, b.Name, r.CurrentOccupants
    HAVING r.CurrentOccupants != COUNT(c.Id)
    ORDER BY b.Name, r.RoomNumber
    
    IF @@ROWCOUNT = 0
        PRINT '✓ All room occupancy counts match active contracts'
    PRINT ''
END

IF @DbExists2 = 1 AND @DbExists3 = 1
BEGIN
    PRINT '--- Checking Active Contracts vs Invoices ---'
    
    -- Active contracts without invoices
    SELECT 
        'Active contracts without invoices' as Check,
        COUNT(*) as Count
    FROM ContractStudentDB.dbo.Contracts c
    LEFT JOIN BillingMaintenanceDB.dbo.Invoices i ON c.Id = i.ContractId
    WHERE c.Status = 'Active' AND i.Id IS NULL
    
    PRINT ''
END

IF @DbExists1 = 1 AND @DbExists3 = 1
BEGIN
    PRINT '--- Checking Maintenance Requests for Non-existent Rooms ---'
    
    -- This requires RoomId comparison (if you store RoomId in maintenance)
    -- Note: Since MaintenanceRequest only has RoomNumber (snapshot), we can't do direct FK check
    PRINT '⚠ Skipped: MaintenanceRequests use snapshot data (RoomNumber), not direct FK'
    PRINT ''
END

PRINT '======================================================='
PRINT ''

-- ==========================================
-- CRITICAL ISSUES SUMMARY
-- ==========================================
PRINT 'STEP 4: Critical issues summary...'
PRINT ''

DECLARE @IssuesTable TABLE (
    Severity VARCHAR(10),
    Service VARCHAR(50),
    Issue VARCHAR(200),
    Count INT
)

-- Service 1 issues
IF @DbExists1 = 1
BEGIN
    INSERT INTO @IssuesTable
    SELECT 'ERROR', 'Service 1', 'Rooms over capacity', COUNT(*)
    FROM RoomBuildingDB.dbo.Rooms
    WHERE CurrentOccupants > MaxOccupants
    HAVING COUNT(*) > 0

    INSERT INTO @IssuesTable
    SELECT 'WARNING', 'Service 1', 'Rooms never inspected', COUNT(*)
    FROM RoomBuildingDB.dbo.Rooms r
    LEFT JOIN RoomBuildingDB.dbo.RoomInspections ri ON r.Id = ri.RoomId
    WHERE ri.Id IS NULL
    HAVING COUNT(*) > 0

    INSERT INTO @IssuesTable
    SELECT 'WARNING', 'Service 1', 'Locked rooms without reason', COUNT(*)
    FROM RoomBuildingDB.dbo.Rooms
    WHERE IsLocked = 1 AND LockReason IS NULL
    HAVING COUNT(*) > 0
END

-- Service 2 issues
IF @DbExists2 = 1
BEGIN
    INSERT INTO @IssuesTable
    SELECT 'WARNING', 'Service 2', 'Pending applications', COUNT(*)
    FROM ContractStudentDB.dbo.RoomApplications
    WHERE Status = 'Pending'
    HAVING COUNT(*) > 0

    INSERT INTO @IssuesTable
    SELECT 'ERROR', 'Service 2', 'Active contracts without check-in', COUNT(*)
    FROM ContractStudentDB.dbo.Contracts c
    LEFT JOIN ContractStudentDB.dbo.CheckIns ci ON c.Id = ci.ContractId
    WHERE c.Status = 'Active' AND ci.Id IS NULL
    HAVING COUNT(*) > 0
END

-- Service 3 issues
IF @DbExists3 = 1
BEGIN
    INSERT INTO @IssuesTable
    SELECT 'ERROR', 'Service 3', 'Overdue invoices', COUNT(*)
    FROM BillingMaintenanceDB.dbo.Invoices
    WHERE Status = 'Overdue'
    HAVING COUNT(*) > 0

    INSERT INTO @IssuesTable
    SELECT 'WARNING', 'Service 3', 'Urgent maintenance requests', COUNT(*)
    FROM BillingMaintenanceDB.dbo.MaintenanceRequests
    WHERE Priority = 'Urgent' AND Status IN ('New', 'Assigned', 'InProgress')
    HAVING COUNT(*) > 0

    INSERT INTO @IssuesTable
    SELECT 'WARNING', 'Service 3', 'Locked user accounts', COUNT(*)
    FROM BillingMaintenanceDB.dbo.Users
    WHERE LockedUntil IS NOT NULL AND LockedUntil > GETDATE()
    HAVING COUNT(*) > 0
END

-- Display issues
IF EXISTS (SELECT 1 FROM @IssuesTable)
BEGIN
    SELECT 
        Severity,
        Service,
        Issue,
        Count
    FROM @IssuesTable
    ORDER BY 
        CASE Severity
            WHEN 'ERROR' THEN 1
            WHEN 'WARNING' THEN 2
            ELSE 3
        END,
        Service
END
ELSE
BEGIN
    PRINT '✓ No critical issues found'
END

PRINT ''
PRINT '======================================================='
PRINT ''

-- ==========================================
-- RECOMMENDATIONS
-- ==========================================
PRINT 'STEP 5: Recommendations...'
PRINT ''

PRINT 'To view detailed reports for each service, run:'
PRINT '  - check-roombuilding-db.sql'
PRINT '  - check-database.sql (ContractStudentDB)'
PRINT '  - check-billing-db.sql'
PRINT ''

PRINT 'For performance monitoring, consider:'
PRINT '  - Add indexes on frequently queried foreign keys'
PRINT '  - Monitor query execution plans'
PRINT '  - Setup database maintenance jobs (backup, update stats, rebuild indexes)'
PRINT ''

PRINT 'For data integrity:'
PRINT '  - Implement cross-service validation jobs'
PRINT '  - Setup alerts for data inconsistencies'
PRINT '  - Regular audit log reviews'
PRINT ''

PRINT '======================================================='
PRINT ''
PRINT '######################################################'
PRINT '#                                                    #'
PRINT '#    HEALTH CHECK COMPLETED                         #'
PRINT '#                                                    #'
PRINT '######################################################'
PRINT ''
PRINT 'Completed at: ' + CONVERT(VARCHAR, GETDATE(), 120)
PRINT ''
