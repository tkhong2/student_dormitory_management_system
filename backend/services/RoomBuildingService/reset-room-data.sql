-- Script để reset dữ liệu phòng và loại phòng
-- Sử dụng khi cần chạy lại DataSeeder với logic mới

USE RoomBuildingDb;
GO

-- Xóa dữ liệu theo thứ tự (foreign key constraints)
PRINT 'Đang xóa RoomImages...';
DELETE FROM RoomImages;

PRINT 'Đang xóa RoomTypeAmenities...';
DELETE FROM RoomTypeAmenities;

PRINT 'Đang xóa Rooms...';
DELETE FROM Rooms;

PRINT 'Đang xóa RoomTypes...';
DELETE FROM RoomTypes;

PRINT 'Đang xóa Amenities...';
DELETE FROM Amenities;

PRINT 'Đang xóa Floors...';
DELETE FROM Floors;

PRINT 'Đang xóa Buildings...';
DELETE FROM Buildings;

-- Reset identity columns (nếu cần)
DBCC CHECKIDENT ('RoomImages', RESEED, 0);
DBCC CHECKIDENT ('RoomTypeAmenities', RESEED, 0);
DBCC CHECKIDENT ('Rooms', RESEED, 0);
DBCC CHECKIDENT ('RoomTypes', RESEED, 0);
DBCC CHECKIDENT ('Amenities', RESEED, 0);
DBCC CHECKIDENT ('Floors', RESEED, 0);
DBCC CHECKIDENT ('Buildings', RESEED, 0);

PRINT '';
PRINT '✅ Đã reset toàn bộ dữ liệu RoomBuildingDb';
PRINT '🔄 Khởi động lại RoomBuildingService.API để chạy DataSeeder';
GO
