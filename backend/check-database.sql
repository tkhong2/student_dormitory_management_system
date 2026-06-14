-- ==========================================
-- DATABASE HEALTH CHECK SCRIPT
-- ==========================================
-- Purpose: Quick verification of seeded data and database state
-- Usage: Run in SQL Server Management Studio or Azure Data Studio
-- Target: ContractStudentDB (Service 2 - Contract & Student Service)
-- ==========================================

PRINT '=========================================='
PRINT 'DATABASE HEALTH CHECK - ContractStudentDB'
PRINT 'Timestamp: ' + CONVERT(VARCHAR, GETDATE(), 120)
PRINT '=========================================='
PRINT ''

-- ==========================================
-- 1. STUDENTS DATA
-- ==========================================
PRINT '1. STUDENTS DATA'
PRINT '------------------'
SELECT COUNT(*) as TotalStudents FROM Students;
PRINT ''

-- Sample students with key info
SELECT TOP 5
    StudentCode,
    FullName,
    Email,
    PhoneNumber,
    Gender,
    IsLocalStudent,
    CreatedAt
FROM Students
ORDER BY CreatedAt DESC;
PRINT ''

-- Students by gender breakdown
SELECT 
    Gender,
    COUNT(*) as Count,
    CAST(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM Students) AS DECIMAL(5,2)) as Percentage
FROM Students
GROUP BY Gender;
PRINT ''

-- ==========================================
-- 2. STUDENT DOCUMENTS
-- ==========================================
PRINT '2. STUDENT DOCUMENTS'
PRINT '------------------'
SELECT COUNT(*) as TotalDocuments FROM StudentDocuments;
PRINT ''

-- Documents by type
SELECT 
    DocumentType,
    COUNT(*) as Count,
    SUM(CASE WHEN IsVerified = 1 THEN 1 ELSE 0 END) as Verified,
    SUM(CASE WHEN IsVerified = 0 THEN 1 ELSE 0 END) as Unverified
FROM StudentDocuments
GROUP BY DocumentType;
PRINT ''

-- Sample documents with student info
SELECT TOP 10
    sd.DocumentName,
    sd.DocumentType,
    sd.IsVerified,
    s.FullName as StudentName,
    s.StudentCode,
    sd.FileSizeKB,
    sd.CreatedAt
FROM StudentDocuments sd
LEFT JOIN Students s ON sd.StudentId = s.Id
ORDER BY sd.CreatedAt DESC;
PRINT ''

-- ==========================================
-- 3. ROOM APPLICATIONS
-- ==========================================
PRINT '3. ROOM APPLICATIONS'
PRINT '------------------'
SELECT COUNT(*) as TotalApplications FROM RoomApplications;
PRINT ''

-- Applications by status
SELECT 
    Status,
    COUNT(*) as Count
FROM RoomApplications
GROUP BY Status
ORDER BY 
    CASE Status
        WHEN 'Pending' THEN 1
        WHEN 'Approved' THEN 2
        WHEN 'Rejected' THEN 3
        WHEN 'Cancelled' THEN 4
        ELSE 5
    END;
PRINT ''

-- ==========================================
-- 4. CONTRACTS
-- ==========================================
PRINT '4. CONTRACTS'
PRINT '------------------'
SELECT COUNT(*) as TotalContracts FROM Contracts;
PRINT ''

-- Contracts by status
SELECT 
    Status,
    COUNT(*) as Count,
    SUM(MonthlyRent) as TotalMonthlyRevenue
FROM Contracts
GROUP BY Status
ORDER BY 
    CASE Status
        WHEN 'Active' THEN 1
        WHEN 'Pending' THEN 2
        WHEN 'Terminated' THEN 3
        WHEN 'Expired' THEN 4
        ELSE 5
    END;
PRINT ''

-- ==========================================
-- 5. CHECK-IN / CHECK-OUT STATUS
-- ==========================================
PRINT '5. CHECK-IN / CHECK-OUT STATUS'
PRINT '------------------'

-- Check-ins
SELECT 
    'CheckIns' as Type,
    COUNT(*) as Count
FROM CheckIns;

-- Check-outs
SELECT 
    'CheckOuts' as Type,
    COUNT(*) as Count
FROM CheckOuts;
PRINT ''

-- ==========================================
-- 6. DATA INTEGRITY CHECKS
-- ==========================================
PRINT '6. DATA INTEGRITY CHECKS'
PRINT '------------------'

-- Students without applications
SELECT 
    'Students without applications' as Check,
    COUNT(*) as Count
FROM Students s
LEFT JOIN RoomApplications ra ON s.Id = ra.StudentId
WHERE ra.Id IS NULL;

-- Approved applications without contracts
SELECT 
    'Approved apps without contracts' as Check,
    COUNT(*) as Count
FROM RoomApplications ra
LEFT JOIN Contracts c ON ra.Id = c.ApplicationId
WHERE ra.Status = 'Approved' AND c.Id IS NULL;

-- Active contracts without check-ins
SELECT 
    'Active contracts without check-ins' as Check,
    COUNT(*) as Count
FROM Contracts c
LEFT JOIN CheckIns ci ON c.Id = ci.ContractId
WHERE c.Status = 'Active' AND ci.Id IS NULL;

PRINT ''
PRINT '=========================================='
PRINT 'HEALTH CHECK COMPLETED'
PRINT '=========================================='
