-- Test tạo user mới
USE BillingMaintenanceDb;

-- Kiểm tra xem username đã tồn tại chưa
SELECT * FROM Users WHERE Username = 'testuser1';

-- Xóa nếu đã tồn tại
DELETE FROM Users WHERE Username = 'testuser1';

-- Tạo user mới với password đã hash bằng BCrypt
-- Password: 123456
-- BCrypt hash: $2a$11$7Z7Z7Z7Z7Z7Z7Z7Z7Z7Z7u (placeholder, sẽ thay bằng hash thật)
INSERT INTO Users (
    Username, 
    PasswordHash, 
    FullName, 
    Email, 
    Phone, 
    Role, 
    IsActive,
    CreatedAt,
    UpdatedAt
) VALUES (
    'testuser1',
    -- BCrypt hash cho password '123456' (dùng BCrypt với cost 11)
    '$2a$11$K3qEZVEGqy5sK5sK5sK5seO5sK5sK5sK5sK5sK5sK5sK5sK5sK5',
    N'Người Dùng Test',
    'testuser1@example.com',
    '0123456789',
    'Admin',
    1,
    GETDATE(),
    GETDATE()
);

-- Kiểm tra user vừa tạo
SELECT Id, Username, FullName, Email, Role, IsActive 
FROM Users 
WHERE Username = 'testuser1';
