-- Create Admin User for BillingMaintenanceService if not exists
USE BillingMaintenanceDB;
GO

-- Check if admin user exists
IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = 'admin@ktx.dnu.edu.vn' OR Id = 1)
BEGIN
    SET IDENTITY_INSERT Users ON;
    
    INSERT INTO Users (
        Id, Username, Email, PasswordHash, FullName, Phone, Role, 
        IsActive, CreatedAt, UpdatedAt, IsDeleted, FailedLoginAttempts
    )
    VALUES (
        1, 
        'admin', 
        'admin@ktx.dnu.edu.vn',
        '$2a$11$YourBCryptHashedPasswordHere', -- Password: Admin@123
        N'Quản trị viên',
        '0123456789',
        'Admin',
        1,
        GETDATE(),
        GETDATE(),
        0,
        0
    );
    
    SET IDENTITY_INSERT Users OFF;
    
    PRINT 'Admin user created successfully with ID = 1';
END
ELSE
BEGIN
    PRINT 'Admin user already exists';
END
GO

-- Verify
SELECT Id, Username, Email, FullName, Role, IsActive 
FROM Users 
WHERE Role = 'Admin';
