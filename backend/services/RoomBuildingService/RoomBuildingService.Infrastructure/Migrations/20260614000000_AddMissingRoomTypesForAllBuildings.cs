using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomBuildingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingRoomTypesForAllBuildings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm các loại phòng còn thiếu cho mỗi tòa nhà
            // Mỗi tòa nhà sẽ có đủ 4 loại: 2P, 4P, 6P, 4P-VIP
            
            migrationBuilder.Sql(@"
                -- Lấy danh sách tất cả các tòa nhà
                DECLARE @Buildings TABLE (Id INT, Name NVARCHAR(100))
                INSERT INTO @Buildings SELECT Id, Name FROM Buildings

                -- Template cho các loại phòng
                DECLARE @RoomTypeTemplates TABLE (
                    Code NVARCHAR(20),
                    Name NVARCHAR(100),
                    Capacity INT,
                    PricePerMonth DECIMAL(18,2),
                    DepositAmount DECIMAL(18,2),
                    Area DECIMAL(18,2),
                    BedType NVARCHAR(50),
                    HasAirConditioner BIT,
                    HasWaterHeater BIT,
                    HasPrivateBathroom BIT,
                    HasWindowView BIT,
                    ElectricityRate DECIMAL(18,2),
                    WaterRate DECIMAL(18,2),
                    Description NVARCHAR(500)
                )

                INSERT INTO @RoomTypeTemplates VALUES
                ('2P', N'Phòng 2 người', 2, 800000, 800000, 15.0, 'Single', 1, 1, 0, 1, 3500, 20000, N'Phòng 2 người, có điều hòa, view đẹp'),
                ('4P', N'Phòng 4 người', 4, 500000, 500000, 20.0, 'Bunk', 1, 0, 0, 1, 3500, 20000, N'Phòng 4 người, tiết kiệm'),
                ('6P', N'Phòng 6 người', 6, 350000, 350000, 25.0, 'Bunk', 0, 0, 0, 1, 3500, 15000, N'Phòng 6 người, giá rẻ'),
                ('4P-VIP', N'Phòng 4 người VIP', 4, 750000, 750000, 30.0, 'Single', 1, 1, 1, 1, 3500, 20000, N'Phòng VIP, đầy đủ tiện nghi')

                -- Thêm các loại phòng còn thiếu cho mỗi tòa
                INSERT INTO RoomTypes (BuildingId, Code, Name, Capacity, PricePerMonth, DepositAmount, ElectricityRate, WaterRate, Area, BedType, HasAirConditioner, HasWaterHeater, HasPrivateBathroom, HasWindowView, Description, IsActive, CreatedAt)
                SELECT 
                    b.Id,
                    t.Code,
                    t.Name,
                    t.Capacity,
                    t.PricePerMonth,
                    t.DepositAmount,
                    t.ElectricityRate,
                    t.WaterRate,
                    t.Area,
                    t.BedType,
                    t.HasAirConditioner,
                    t.HasWaterHeater,
                    t.HasPrivateBathroom,
                    t.HasWindowView,
                    t.Description,
                    1, -- IsActive
                    GETDATE() -- CreatedAt
                FROM @Buildings b
                CROSS JOIN @RoomTypeTemplates t
                WHERE NOT EXISTS (
                    SELECT 1 FROM RoomTypes rt 
                    WHERE rt.BuildingId = b.Id AND rt.Code = t.Code
                )

                PRINT 'Đã thêm các loại phòng còn thiếu cho tất cả các tòa nhà'
            ");

            // Thêm amenities cho các room types mới
            migrationBuilder.Sql(@"
                -- Thêm RoomTypeAmenities cho các loại phòng vừa tạo
                
                -- Phòng 2 người: Điều hòa, Nóng lạnh, Tủ quần áo, Bàn học, Kệ sách, Cửa sổ lớn
                INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity)
                SELECT rt.Id, a.Id, 
                    CASE 
                        WHEN a.Name IN (N'Bàn học', N'Tủ quần áo') THEN 2
                        ELSE 1
                    END
                FROM RoomTypes rt
                CROSS JOIN Amenities a
                WHERE rt.Code = '2P'
                AND a.Name IN (N'Điều hòa', N'Nóng lạnh', N'Tủ quần áo', N'Bàn học', N'Kệ sách', N'Cửa sổ lớn')
                AND NOT EXISTS (
                    SELECT 1 FROM RoomTypeAmenities rta 
                    WHERE rta.RoomTypeId = rt.Id AND rta.AmenityId = a.Id
                )

                -- Phòng 4 người: Điều hòa, Giường tầng, Tủ quần áo, Bàn học, Quạt trần
                INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity)
                SELECT rt.Id, a.Id,
                    CASE 
                        WHEN a.Name = N'Giường tầng' THEN 2
                        WHEN a.Name IN (N'Bàn học', N'Tủ quần áo') THEN 4
                        ELSE 1
                    END
                FROM RoomTypes rt
                CROSS JOIN Amenities a
                WHERE rt.Code = '4P'
                AND a.Name IN (N'Điều hòa', N'Giường tầng', N'Tủ quần áo', N'Bàn học', N'Quạt trần')
                AND NOT EXISTS (
                    SELECT 1 FROM RoomTypeAmenities rta 
                    WHERE rta.RoomTypeId = rt.Id AND rta.AmenityId = a.Id
                )

                -- Phòng 6 người: Giường tầng, Tủ quần áo, Bàn học, Quạt trần
                INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity)
                SELECT rt.Id, a.Id,
                    CASE 
                        WHEN a.Name = N'Giường tầng' THEN 3
                        WHEN a.Name IN (N'Bàn học', N'Tủ quần áo') THEN 6
                        ELSE 1
                    END
                FROM RoomTypes rt
                CROSS JOIN Amenities a
                WHERE rt.Code = '6P'
                AND a.Name IN (N'Giường tầng', N'Tủ quần áo', N'Bàn học', N'Quạt trần')
                AND NOT EXISTS (
                    SELECT 1 FROM RoomTypeAmenities rta 
                    WHERE rta.RoomTypeId = rt.Id AND rta.AmenityId = a.Id
                )

                -- Phòng 4 người VIP: Điều hòa, Nóng lạnh, WC riêng, Tủ lạnh, Tủ quần áo, Bàn học, TV, Ban công
                INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity)
                SELECT rt.Id, a.Id,
                    CASE 
                        WHEN a.Name IN (N'Bàn học', N'Tủ quần áo') THEN 4
                        ELSE 1
                    END
                FROM RoomTypes rt
                CROSS JOIN Amenities a
                WHERE rt.Code = '4P-VIP'
                AND a.Name IN (N'Điều hòa', N'Nóng lạnh', N'WC riêng', N'Tủ lạnh', N'Tủ quần áo', N'Bàn học', N'TV', N'Ban công')
                AND NOT EXISTS (
                    SELECT 1 FROM RoomTypeAmenities rta 
                    WHERE rta.RoomTypeId = rt.Id AND rta.AmenityId = a.Id
                )

                PRINT 'Đã thêm amenities cho các loại phòng mới'
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback: Xóa các RoomTypes được thêm vào (giữ lại RoomTypes ban đầu)
            migrationBuilder.Sql(@"
                -- Xóa RoomTypeAmenities của các room types bị duplicate
                DELETE FROM RoomTypeAmenities
                WHERE RoomTypeId IN (
                    SELECT rt.Id 
                    FROM RoomTypes rt
                    INNER JOIN (
                        SELECT BuildingId, Code, MIN(Id) as FirstId
                        FROM RoomTypes
                        GROUP BY BuildingId, Code
                        HAVING COUNT(*) > 1
                    ) dup ON rt.BuildingId = dup.BuildingId AND rt.Code = dup.Code AND rt.Id > dup.FirstId
                )

                -- Xóa các RoomTypes duplicate (giữ lại cái đầu tiên)
                DELETE FROM RoomTypes
                WHERE Id IN (
                    SELECT rt.Id 
                    FROM RoomTypes rt
                    INNER JOIN (
                        SELECT BuildingId, Code, MIN(Id) as FirstId
                        FROM RoomTypes
                        GROUP BY BuildingId, Code
                        HAVING COUNT(*) > 1
                    ) dup ON rt.BuildingId = dup.BuildingId AND rt.Code = dup.Code AND rt.Id > dup.FirstId
                )
            ");
        }
    }
}
