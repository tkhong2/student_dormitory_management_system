-- Fix: Ensure admin user exists in BillingMaintenanceDB
USE BillingMaintenanceDB;
GO

-- First, check what users exist
PRINT '=== Current Users in BillingMaintenanceDB ===';
SELECT Id, Username, Email, FullName, Role, IsActive, CreatedAt
FROM Users
ORDER BY Id;
GO

-- If no user with ID=1, create one
IF NOT EXISTS (SELECT 1 FROM Users WHERE Id = 1)
BEGIN
    PRINT 'Creating admin user with ID = 1...';
    
    SET IDENTITY_INSERT Users ON;
    
    INSERT INTO Users (
        Id, 
        Username, 
        Email, 
        PasswordHash, 
        FullName, 
        Phone, 
        Role, 
        IsActive, 
        CreatedAt, 
        UpdatedAt, 
        IsDeleted, 
        FailedLoginAttempts
    )
    VALUES (
        1, 
        'admin', 
        'admin@ktx.dnu.edu.vn',
        -- BCrypt hash of "Admin@123"
        '$2a$11$YNu5JZQrKGLxH9qVMxZ3D.K8qZKZQJ0GZCvP1QqJZQrKGLxH9qVM',
        N'Quản trị viên hệ thống',
        '0123456789',
        'Admin',
        1,
        GETDATE(),
        GETDATE(),
        0,
        0
    );
    
    SET IDENTITY_INSERT Users OFF;
    
    PRINT 'Admin user created successfully!';
END
ELSE
BEGIN
    PRINT 'User with ID = 1 already exists';
END
GO

-- Remove duplicate system@ktx.dnu.edu.vn users if any (keep only one)
DECLARE @KeepUserId INT;
DECLARE @DeleteCount INT = 0;

SELECT TOP 1 @KeepUserId = Id 
FROM Users 
WHERE Email = 'system@ktx.dnu.edu.vn'
ORDER BY Id;

IF @KeepUserId IS NOT NULL
BEGIN
    -- Delete other system users with same email
    DELETE FROM Users 
    WHERE Email = 'system@ktx.dnu.edu.vn' 
    AND Id != @KeepUserId;
    
    SET @DeleteCount = @@ROWCOUNT;
    
    IF @DeleteCount > 0
        PRINT CONCAT('Removed ', @DeleteCount, ' duplicate system user(s)');
    ELSE
        PRINT 'No duplicate system users found';
END
GO

-- Verify final state
PRINT '';
PRINT '=== Final User List ===';
SELECT Id, Username, Email, FullName, Role, IsActive
FROM Users
ORDER BY Id;
GO
