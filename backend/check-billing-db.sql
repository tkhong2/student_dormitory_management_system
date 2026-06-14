-- ==========================================
-- DATABASE HEALTH CHECK SCRIPT
-- ==========================================
-- Purpose: Quick verification of seeded data and database state
-- Usage: Run in SQL Server Management Studio or Azure Data Studio
-- Target: BillingMaintenanceDB (Service 3 - Billing & Maintenance Service)
-- ==========================================

PRINT '=========================================='
PRINT 'DATABASE HEALTH CHECK - BillingMaintenanceDB'
PRINT 'Timestamp: ' + CONVERT(VARCHAR, GETDATE(), 120)
PRINT '=========================================='
PRINT ''

-- ==========================================
-- 1. USERS DATA
-- ==========================================
PRINT '1. USERS DATA'
PRINT '------------------'
SELECT COUNT(*) as TotalUsers FROM Users;
PRINT ''

-- Users by role
SELECT 
    Role,
    COUNT(*) as Count,
    SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) as Active,
    SUM(CASE WHEN IsActive = 0 THEN 1 ELSE 0 END) as Inactive
FROM Users
GROUP BY Role
ORDER BY 
    CASE Role
        WHEN 'Admin' THEN 1
        WHEN 'Staff' THEN 2
        WHEN 'Student' THEN 3
        ELSE 4
    END;
PRINT ''

-- Recently logged in users
SELECT TOP 10
    Username,
    FullName,
    Role,
    LastLoginAt,
    LastLoginIp
FROM Users
WHERE LastLoginAt IS NOT NULL
ORDER BY LastLoginAt DESC;
PRINT ''

-- Locked accounts
SELECT 
    'Locked accounts' as Check,
    COUNT(*) as Count
FROM Users
WHERE LockedUntil IS NOT NULL AND LockedUntil > GETDATE();
PRINT ''

-- Users with failed login attempts
SELECT 
    'Users with failed attempts' as Check,
    COUNT(*) as Count
FROM Users
WHERE FailedLoginAttempts > 0;
PRINT ''

-- ==========================================
-- 2. BUILDING ASSIGNMENTS
-- ==========================================
PRINT '2. BUILDING ASSIGNMENTS'
PRINT '------------------'
SELECT COUNT(*) as TotalAssignments FROM BuildingAssignments;
PRINT ''

-- Active assignments by building
SELECT 
    ba.BuildingName,
    COUNT(DISTINCT ba.UserId) as AssignedStaff,
    u.FullName as StaffName,
    u.Role
FROM BuildingAssignments ba
LEFT JOIN Users u ON ba.UserId = u.Id
WHERE ba.IsActive = 1
GROUP BY ba.BuildingName, u.FullName, u.Role
ORDER BY ba.BuildingName;
PRINT ''

-- Buildings without assigned staff
SELECT 
    'Buildings without staff' as Check,
    COUNT(*) as Count
FROM (
    SELECT DISTINCT BuildingName
    FROM BuildingAssignments
) allBuildings
WHERE BuildingName NOT IN (
    SELECT DISTINCT BuildingName 
    FROM BuildingAssignments 
    WHERE IsActive = 1
);
PRINT ''

-- ==========================================
-- 3. INVOICES DATA
-- ==========================================
PRINT '3. INVOICES DATA'
PRINT '------------------'
SELECT COUNT(*) as TotalInvoices FROM Invoices;
PRINT ''

-- Invoices by status
SELECT 
    Status,
    COUNT(*) as Count,
    FORMAT(SUM(TotalAmount), 'N0') as TotalAmount,
    FORMAT(SUM(PaidAmount), 'N0') as PaidAmount,
    FORMAT(SUM(DebtAmount), 'N0') as DebtAmount
FROM Invoices
GROUP BY Status
ORDER BY 
    CASE Status
        WHEN 'Unpaid' THEN 1
        WHEN 'PartialPaid' THEN 2
        WHEN 'Paid' THEN 3
        WHEN 'Overdue' THEN 4
        WHEN 'Cancelled' THEN 5
        ELSE 6
    END;
PRINT ''

-- Invoices by type
SELECT 
    InvoiceType,
    COUNT(*) as Count,
    FORMAT(SUM(TotalAmount), 'N0') as TotalAmount
FROM Invoices
GROUP BY InvoiceType;
PRINT ''

-- Financial summary
SELECT 
    FORMAT(SUM(TotalAmount), 'N0') as TotalRevenue,
    FORMAT(SUM(PaidAmount), 'N0') as CollectedAmount,
    FORMAT(SUM(DebtAmount), 'N0') as OutstandingDebt,
    CAST(SUM(PaidAmount) * 100.0 / NULLIF(SUM(TotalAmount), 0) AS DECIMAL(5,2)) as CollectionRate
FROM Invoices
WHERE Status != 'Cancelled';
PRINT ''

-- Monthly revenue breakdown
SELECT TOP 12
    BillingYear,
    BillingMonth,
    COUNT(*) as InvoiceCount,
    FORMAT(SUM(TotalAmount), 'N0') as TotalAmount,
    FORMAT(SUM(PaidAmount), 'N0') as PaidAmount,
    FORMAT(SUM(DebtAmount), 'N0') as DebtAmount,
    CAST(SUM(PaidAmount) * 100.0 / NULLIF(SUM(TotalAmount), 0) AS DECIMAL(5,2)) as CollectionRate
FROM Invoices
WHERE InvoiceType = 'Monthly'
GROUP BY BillingYear, BillingMonth
ORDER BY BillingYear DESC, BillingMonth DESC;
PRINT ''

-- Overdue invoices
SELECT 
    'Overdue invoices' as Check,
    COUNT(*) as Count,
    FORMAT(SUM(DebtAmount), 'N0') as TotalDebt,
    AVG(OverdueDays) as AvgOverdueDays
FROM Invoices
WHERE Status = 'Overdue';
PRINT ''

-- Top 10 debtors
SELECT TOP 10
    StudentName,
    StudentCode,
    COUNT(*) as UnpaidInvoices,
    FORMAT(SUM(DebtAmount), 'N0') as TotalDebt,
    MAX(OverdueDays) as MaxOverdueDays
FROM Invoices
WHERE Status IN ('Unpaid', 'PartialPaid', 'Overdue')
GROUP BY StudentName, StudentCode
ORDER BY SUM(DebtAmount) DESC;
PRINT ''

-- Invoices with previous debt
SELECT 
    'Invoices with previous debt' as Check,
    COUNT(*) as Count,
    FORMAT(SUM(PreviousDebt), 'N0') as TotalPreviousDebt
FROM Invoices
WHERE PreviousDebt > 0;
PRINT ''

-- ==========================================
-- 4. INVOICE ITEMS
-- ==========================================
PRINT '4. INVOICE ITEMS'
PRINT '------------------'
SELECT COUNT(*) as TotalInvoiceItems FROM InvoiceItems;
PRINT ''

-- Revenue breakdown by item type
SELECT 
    ItemName,
    COUNT(*) as Count,
    FORMAT(SUM(Amount), 'N0') as TotalAmount,
    FORMAT(AVG(Amount), 'N0') as AvgAmount
FROM InvoiceItems
GROUP BY ItemName
ORDER BY SUM(Amount) DESC;
PRINT ''

-- Average invoice composition
SELECT 
    'Avg Rent' as ItemType,
    FORMAT(AVG(RentAmount), 'N0') as AvgAmount
FROM Invoices
WHERE InvoiceType = 'Monthly'
UNION ALL
SELECT 
    'Avg Electricity' as ItemType,
    FORMAT(AVG(ElectricityAmount), 'N0') as AvgAmount
FROM Invoices
WHERE InvoiceType = 'Monthly'
UNION ALL
SELECT 
    'Avg Water' as ItemType,
    FORMAT(AVG(WaterAmount), 'N0') as AvgAmount
FROM Invoices
WHERE InvoiceType = 'Monthly'
UNION ALL
SELECT 
    'Avg Service' as ItemType,
    FORMAT(AVG(ServiceAmount), 'N0') as AvgAmount
FROM Invoices
WHERE InvoiceType = 'Monthly';
PRINT ''

-- ==========================================
-- 5. PAYMENTS DATA
-- ==========================================
PRINT '5. PAYMENTS DATA'
PRINT '------------------'
SELECT COUNT(*) as TotalPayments FROM Payments;
PRINT ''

-- Payments by method
SELECT 
    Method,
    COUNT(*) as Count,
    FORMAT(SUM(Amount), 'N0') as TotalAmount,
    FORMAT(AVG(Amount), 'N0') as AvgAmount
FROM Payments
GROUP BY Method
ORDER BY SUM(Amount) DESC;
PRINT ''

-- Payments by date (last 30 days)
SELECT 
    CAST(PaidAt AS DATE) as PaymentDate,
    COUNT(*) as TransactionCount,
    FORMAT(SUM(Amount), 'N0') as TotalAmount
FROM Payments
WHERE PaidAt >= DATEADD(DAY, -30, GETDATE())
GROUP BY CAST(PaidAt AS DATE)
ORDER BY CAST(PaidAt AS DATE) DESC;
PRINT ''

-- Top collectors (staff who received most payments)
SELECT TOP 10
    ReceivedByName,
    COUNT(*) as PaymentsReceived,
    FORMAT(SUM(Amount), 'N0') as TotalCollected
FROM Payments
GROUP BY ReceivedByName
ORDER BY SUM(Amount) DESC;
PRINT ''

-- Payments with transaction code
SELECT 
    'Payments with transaction code' as Check,
    COUNT(*) as Count,
    FORMAT(SUM(Amount), 'N0') as TotalAmount
FROM Payments
WHERE TransactionCode IS NOT NULL;
PRINT ''

-- ==========================================
-- 6. MAINTENANCE REQUESTS DATA
-- ==========================================
PRINT '6. MAINTENANCE REQUESTS DATA'
PRINT '------------------'
SELECT COUNT(*) as TotalRequests FROM MaintenanceRequests;
PRINT ''

-- Requests by status
SELECT 
    Status,
    COUNT(*) as Count,
    CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM MaintenanceRequests) AS DECIMAL(5,2)) as Percentage
FROM MaintenanceRequests
GROUP BY Status
ORDER BY 
    CASE Status
        WHEN 'New' THEN 1
        WHEN 'Assigned' THEN 2
        WHEN 'InProgress' THEN 3
        WHEN 'Done' THEN 4
        WHEN 'Cancelled' THEN 5
        WHEN 'Rejected' THEN 6
        ELSE 7
    END;
PRINT ''

-- Requests by category
SELECT 
    Category,
    COUNT(*) as Count,
    AVG(DATEDIFF(HOUR, CreatedAt, ISNULL(ResolvedAt, GETDATE()))) as AvgResolutionHours
FROM MaintenanceRequests
GROUP BY Category
ORDER BY COUNT(*) DESC;
PRINT ''

-- Requests by priority
SELECT 
    Priority,
    COUNT(*) as Count,
    SUM(CASE WHEN Status = 'Done' THEN 1 ELSE 0 END) as Completed,
    SUM(CASE WHEN Status IN ('New', 'Assigned', 'InProgress') THEN 1 ELSE 0 END) as Pending
FROM MaintenanceRequests
GROUP BY Priority
ORDER BY 
    CASE Priority
        WHEN 'Urgent' THEN 1
        WHEN 'High' THEN 2
        WHEN 'Medium' THEN 3
        WHEN 'Low' THEN 4
        ELSE 5
    END;
PRINT ''

-- Pending urgent requests
SELECT 
    mr.Title,
    mr.Category,
    mr.Priority,
    mr.RequestedByStudentName,
    mr.RoomNumber,
    mr.BuildingName,
    mr.CreatedAt,
    mr.Status,
    mr.AssignedToName
FROM MaintenanceRequests mr
WHERE mr.Priority IN ('Urgent', 'High')
    AND mr.Status IN ('New', 'Assigned', 'InProgress')
ORDER BY 
    CASE mr.Priority
        WHEN 'Urgent' THEN 1
        WHEN 'High' THEN 2
        ELSE 3
    END,
    mr.CreatedAt;
PRINT ''

-- Average resolution time by category
SELECT 
    Category,
    COUNT(*) as CompletedRequests,
    AVG(DATEDIFF(HOUR, CreatedAt, ResolvedAt)) as AvgResolutionHours,
    MIN(DATEDIFF(HOUR, CreatedAt, ResolvedAt)) as MinResolutionHours,
    MAX(DATEDIFF(HOUR, CreatedAt, ResolvedAt)) as MaxResolutionHours
FROM MaintenanceRequests
WHERE Status = 'Done' AND ResolvedAt IS NOT NULL
GROUP BY Category
ORDER BY AVG(DATEDIFF(HOUR, CreatedAt, ResolvedAt));
PRINT ''

-- Recurring issues
SELECT 
    'Recurring issues' as Check,
    COUNT(*) as Count
FROM MaintenanceRequests
WHERE IsRecurring = 1;
PRINT ''

-- Cost analysis
SELECT 
    FORMAT(SUM(EstimatedCost), 'N0') as TotalEstimatedCost,
    FORMAT(SUM(ActualCost), 'N0') as TotalActualCost,
    FORMAT(AVG(ActualCost), 'N0') as AvgActualCost,
    COUNT(CASE WHEN ActualCost > EstimatedCost THEN 1 END) as OverBudgetCount
FROM MaintenanceRequests
WHERE Status = 'Done';
PRINT ''

-- Staff performance (technicians)
SELECT TOP 10
    mr.AssignedToName as Technician,
    COUNT(*) as TotalAssigned,
    SUM(CASE WHEN mr.Status = 'Done' THEN 1 ELSE 0 END) as Completed,
    AVG(CASE WHEN mr.Status = 'Done' THEN DATEDIFF(HOUR, mr.AssignedAt, mr.ResolvedAt) END) as AvgResolutionHours,
    AVG(CASE WHEN mr.Rating IS NOT NULL THEN mr.Rating ELSE NULL END) as AvgRating
FROM MaintenanceRequests mr
WHERE mr.AssignedToUserId IS NOT NULL
GROUP BY mr.AssignedToName
ORDER BY COUNT(*) DESC;
PRINT ''

-- Requests with feedback
SELECT 
    'Requests with student feedback' as Check,
    COUNT(*) as Count,
    FORMAT(AVG(CAST(Rating AS FLOAT)), 'N2') as AvgRating
FROM MaintenanceRequests
WHERE Rating IS NOT NULL;
PRINT ''

-- ==========================================
-- 7. MAINTENANCE STATUS LOGS
-- ==========================================
PRINT '7. MAINTENANCE STATUS LOGS'
PRINT '------------------'
SELECT COUNT(*) as TotalStatusChanges FROM MaintenanceStatusLogs;
PRINT ''

-- Recent status changes
SELECT TOP 10
    msl.ChangedAt,
    mr.Title,
    mr.RoomNumber,
    mr.BuildingName,
    msl.OldStatus,
    msl.NewStatus,
    msl.ChangedByName,
    msl.Note
FROM MaintenanceStatusLogs msl
LEFT JOIN MaintenanceRequests mr ON msl.MaintenanceRequestId = mr.Id
ORDER BY msl.ChangedAt DESC;
PRINT ''

-- ==========================================
-- 8. NOTIFICATIONS
-- ==========================================
PRINT '8. NOTIFICATIONS'
PRINT '------------------'
SELECT COUNT(*) as TotalNotifications FROM Notifications;
PRINT ''

-- Notifications by type
SELECT 
    Type,
    COUNT(*) as Count,
    SUM(CASE WHEN IsRead = 1 THEN 1 ELSE 0 END) as Read,
    SUM(CASE WHEN IsRead = 0 THEN 1 ELSE 0 END) as Unread
FROM Notifications
GROUP BY Type
ORDER BY COUNT(*) DESC;
PRINT ''

-- Unread notifications by user
SELECT 
    u.Username,
    u.FullName,
    u.Role,
    COUNT(n.Id) as UnreadCount
FROM Users u
LEFT JOIN Notifications n ON u.Id = n.UserId AND n.IsRead = 0
WHERE n.Id IS NOT NULL
GROUP BY u.Username, u.FullName, u.Role
ORDER BY COUNT(n.Id) DESC;
PRINT ''

-- Recent notifications
SELECT TOP 10
    n.CreatedAt,
    u.Username,
    u.FullName,
    n.Type,
    n.Title,
    n.IsRead
FROM Notifications n
LEFT JOIN Users u ON n.UserId = u.Id
ORDER BY n.CreatedAt DESC;
PRINT ''

-- ==========================================
-- 9. AUDIT LOGS (if exists)
-- ==========================================
IF OBJECT_ID('AuditLogs', 'U') IS NOT NULL
BEGIN
    PRINT '9. AUDIT LOGS'
    PRINT '------------------'
    SELECT COUNT(*) as TotalAuditLogs FROM AuditLogs;
    PRINT ''

    -- Recent audit entries
    SELECT TOP 20
        al.CreatedAt,
        u.Username,
        al.Action,
        al.EntityType,
        al.IpAddress
    FROM AuditLogs al
    LEFT JOIN Users u ON al.UserId = u.Id
    ORDER BY al.CreatedAt DESC;
    PRINT ''
END
ELSE
BEGIN
    PRINT '9. AUDIT LOGS'
    PRINT '------------------'
    PRINT 'AuditLogs table does not exist yet.'
    PRINT ''
END

-- ==========================================
-- 10. DATA INTEGRITY CHECKS
-- ==========================================
PRINT '10. DATA INTEGRITY CHECKS'
PRINT '------------------'

-- Invoices with no items
SELECT 
    'Invoices without items' as Check,
    COUNT(*) as Count
FROM Invoices i
LEFT JOIN InvoiceItems ii ON i.Id = ii.InvoiceId
WHERE ii.Id IS NULL;

-- Paid invoices with PaidAmount != TotalAmount
SELECT 
    'Paid invoices with amount mismatch' as Check,
    COUNT(*) as Count
FROM Invoices
WHERE Status = 'Paid' AND ABS(PaidAmount - TotalAmount) > 0.01;

-- Invoices with negative debt
SELECT 
    'Invoices with negative debt' as Check,
    COUNT(*) as Count
FROM Invoices
WHERE DebtAmount < 0;

-- Payments exceeding invoice total
SELECT 
    'Payments exceeding invoice total' as Check,
    COUNT(*) as Count
FROM (
    SELECT 
        i.Id,
        i.TotalAmount,
        SUM(p.Amount) as TotalPayments
    FROM Invoices i
    LEFT JOIN Payments p ON i.Id = p.InvoiceId
    GROUP BY i.Id, i.TotalAmount
    HAVING SUM(p.Amount) > i.TotalAmount
) as OverPayments;

-- Maintenance requests without assigned staff (not New/Rejected)
SELECT 
    'Assigned requests without staff' as Check,
    COUNT(*) as Count
FROM MaintenanceRequests
WHERE Status IN ('Assigned', 'InProgress') AND AssignedToUserId IS NULL;

-- Done requests without resolution time
SELECT 
    'Done requests without resolution time' as Check,
    COUNT(*) as Count
FROM MaintenanceRequests
WHERE Status = 'Done' AND ResolvedAt IS NULL;

-- Users with null or empty email
SELECT 
    'Users with invalid email' as Check,
    COUNT(*) as Count
FROM Users
WHERE Email IS NULL OR Email = '';

PRINT ''
PRINT '=========================================='
PRINT 'HEALTH CHECK COMPLETED'
PRINT '=========================================='
