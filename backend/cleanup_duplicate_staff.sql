-- Script để xóa nhân viên thừa trong BillingMaintenanceDB

USE BillingMaintenanceDB;
GO

-- 1. Xem tất cả users hiện có
SELECT Id, Username, FullName, Role, IsActive, IsDeleted 
FROM Users 
WHERE Role IN ('Staff', 'Admin')
ORDER BY Role, Username;
GO

-- 2. Xóa tất cả staff KHÔNG phải 'staff01'
-- Chỉ giữ lại: admin và staff01
DELETE FROM Users 
WHERE Role = 'Staff' 
  AND Username != 'staff01';
GO

-- 3. Kiểm tra lại
SELECT Id, Username, FullName, Role, IsActive, IsDeleted 
FROM Users 
WHERE Role IN ('Staff', 'Admin')
ORDER BY Role, Username;
GO

PRINT 'Done! Đã xóa các staff thừa, chỉ giữ lại staff01 (Nguyễn Văn An)';
