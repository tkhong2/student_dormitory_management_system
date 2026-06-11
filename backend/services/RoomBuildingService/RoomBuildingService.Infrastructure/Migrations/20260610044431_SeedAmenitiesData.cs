using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomBuildingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAmenitiesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed Amenities
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Điều hòa')
                    INSERT INTO Amenities (Name, Category, IconUrl, IsActive, CreatedAt) VALUES (N'Điều hòa', 'Electric', '❄️', 1, GETUTCDATE());
                
                IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Nóng lạnh')
                    INSERT INTO Amenities (Name, Category, IconUrl, IsActive, CreatedAt) VALUES (N'Nóng lạnh', 'Electric', '🚿', 1, GETUTCDATE());
                
                IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'WC riêng')
                    INSERT INTO Amenities (Name, Category, IconUrl, IsActive, CreatedAt) VALUES (N'WC riêng', 'Sanitary', '🚽', 1, GETUTCDATE());
                
                IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Tủ lạnh')
                    INSERT INTO Amenities (Name, Category, IconUrl, IsActive, CreatedAt) VALUES (N'Tủ lạnh', 'Electric', '🧊', 1, GETUTCDATE());
                
                IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Giường tầng')
                    INSERT INTO Amenities (Name, Category, IconUrl, IsActive, CreatedAt) VALUES (N'Giường tầng', 'Furniture', '🛏️', 1, GETUTCDATE());
                
                IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Tủ quần áo')
                    INSERT INTO Amenities (Name, Category, IconUrl, IsActive, CreatedAt) VALUES (N'Tủ quần áo', 'Furniture', '👔', 1, GETUTCDATE());
                
                IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Bàn học')
                    INSERT INTO Amenities (Name, Category, IconUrl, IsActive, CreatedAt) VALUES (N'Bàn học', 'Furniture', '📚', 1, GETUTCDATE());
                
                IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Kệ sách')
                    INSERT INTO Amenities (Name, Category, IconUrl, IsActive, CreatedAt) VALUES (N'Kệ sách', 'Furniture', '📖', 1, GETUTCDATE());
                
                IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Quạt trần')
                    INSERT INTO Amenities (Name, Category, IconUrl, IsActive, CreatedAt) VALUES (N'Quạt trần', 'Electric', '💨', 1, GETUTCDATE());
                
                IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Ban công')
                    INSERT INTO Amenities (Name, Category, IconUrl, IsActive, CreatedAt) VALUES (N'Ban công', 'Other', '🪟', 1, GETUTCDATE());
                
                IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'Cửa sổ lớn')
                    INSERT INTO Amenities (Name, Category, IconUrl, IsActive, CreatedAt) VALUES (N'Cửa sổ lớn', 'Other', '🪟', 1, GETUTCDATE());
                
                IF NOT EXISTS (SELECT 1 FROM Amenities WHERE Name = N'TV')
                    INSERT INTO Amenities (Name, Category, IconUrl, IsActive, CreatedAt) VALUES (N'TV', 'Electric', '📺', 1, GETUTCDATE());
            ");

            // Seed RoomTypeAmenities for each room type
            migrationBuilder.Sql(@"
                DECLARE @RoomType2P INT, @RoomType4P INT, @RoomType6P INT, @RoomType4PVIP INT;
                DECLARE @DieuHoa INT, @NongLanh INT, @WC INT, @TuLanh INT, @GiuongTang INT, @TuQuanAo INT, @BanHoc INT, @KeSach INT, @QuatTran INT, @BanCong INT, @CuaSo INT, @TV INT;

                -- Get RoomType IDs
                SELECT @RoomType2P = Id FROM RoomTypes WHERE Code = '2P';
                SELECT @RoomType4P = Id FROM RoomTypes WHERE Code = '4P';
                SELECT @RoomType6P = Id FROM RoomTypes WHERE Code = '6P';
                SELECT @RoomType4PVIP = Id FROM RoomTypes WHERE Code = '4P-VIP';

                -- Get Amenity IDs
                SELECT @DieuHoa = Id FROM Amenities WHERE Name = N'Điều hòa';
                SELECT @NongLanh = Id FROM Amenities WHERE Name = N'Nóng lạnh';
                SELECT @WC = Id FROM Amenities WHERE Name = N'WC riêng';
                SELECT @TuLanh = Id FROM Amenities WHERE Name = N'Tủ lạnh';
                SELECT @GiuongTang = Id FROM Amenities WHERE Name = N'Giường tầng';
                SELECT @TuQuanAo = Id FROM Amenities WHERE Name = N'Tủ quần áo';
                SELECT @BanHoc = Id FROM Amenities WHERE Name = N'Bàn học';
                SELECT @KeSach = Id FROM Amenities WHERE Name = N'Kệ sách';
                SELECT @QuatTran = Id FROM Amenities WHERE Name = N'Quạt trần';
                SELECT @BanCong = Id FROM Amenities WHERE Name = N'Ban công';
                SELECT @CuaSo = Id FROM Amenities WHERE Name = N'Cửa sổ lớn';
                SELECT @TV = Id FROM Amenities WHERE Name = N'TV';

                -- Phòng 2 người: Điều hòa, Nóng lạnh, Tủ quần áo, Bàn học, Kệ sách, Cửa sổ lớn
                IF @RoomType2P IS NOT NULL
                BEGIN
                    IF @DieuHoa IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType2P AND AmenityId = @DieuHoa)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType2P, @DieuHoa, 1);
                    
                    IF @NongLanh IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType2P AND AmenityId = @NongLanh)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType2P, @NongLanh, 1);
                    
                    IF @TuQuanAo IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType2P AND AmenityId = @TuQuanAo)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType2P, @TuQuanAo, 2);
                    
                    IF @BanHoc IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType2P AND AmenityId = @BanHoc)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType2P, @BanHoc, 2);
                    
                    IF @KeSach IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType2P AND AmenityId = @KeSach)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType2P, @KeSach, 1);
                    
                    IF @CuaSo IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType2P AND AmenityId = @CuaSo)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType2P, @CuaSo, 1);
                END

                -- Phòng 4 người: Điều hòa, Giường tầng, Tủ quần áo, Bàn học, Quạt trần
                IF @RoomType4P IS NOT NULL
                BEGIN
                    IF @DieuHoa IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType4P AND AmenityId = @DieuHoa)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType4P, @DieuHoa, 1);
                    
                    IF @GiuongTang IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType4P AND AmenityId = @GiuongTang)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType4P, @GiuongTang, 2);
                    
                    IF @TuQuanAo IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType4P AND AmenityId = @TuQuanAo)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType4P, @TuQuanAo, 4);
                    
                    IF @BanHoc IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType4P AND AmenityId = @BanHoc)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType4P, @BanHoc, 4);
                    
                    IF @QuatTran IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType4P AND AmenityId = @QuatTran)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType4P, @QuatTran, 1);
                END

                -- Phòng 6 người: Giường tầng, Tủ quần áo, Bàn học, Quạt trần
                IF @RoomType6P IS NOT NULL
                BEGIN
                    IF @GiuongTang IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType6P AND AmenityId = @GiuongTang)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType6P, @GiuongTang, 3);
                    
                    IF @TuQuanAo IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType6P AND AmenityId = @TuQuanAo)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType6P, @TuQuanAo, 6);
                    
                    IF @BanHoc IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType6P AND AmenityId = @BanHoc)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType6P, @BanHoc, 6);
                    
                    IF @QuatTran IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType6P AND AmenityId = @QuatTran)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType6P, @QuatTran, 1);
                END

                -- Phòng 4 người VIP: Điều hòa, Nóng lạnh, WC riêng, Tủ lạnh, Tủ quần áo, Bàn học, TV, Ban công
                IF @RoomType4PVIP IS NOT NULL
                BEGIN
                    IF @DieuHoa IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType4PVIP AND AmenityId = @DieuHoa)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType4PVIP, @DieuHoa, 1);
                    
                    IF @NongLanh IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType4PVIP AND AmenityId = @NongLanh)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType4PVIP, @NongLanh, 1);
                    
                    IF @WC IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType4PVIP AND AmenityId = @WC)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType4PVIP, @WC, 1);
                    
                    IF @TuLanh IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType4PVIP AND AmenityId = @TuLanh)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType4PVIP, @TuLanh, 1);
                    
                    IF @TuQuanAo IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType4PVIP AND AmenityId = @TuQuanAo)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType4PVIP, @TuQuanAo, 4);
                    
                    IF @BanHoc IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType4PVIP AND AmenityId = @BanHoc)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType4PVIP, @BanHoc, 4);
                    
                    IF @TV IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType4PVIP AND AmenityId = @TV)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType4PVIP, @TV, 1);
                    
                    IF @BanCong IS NOT NULL AND NOT EXISTS (SELECT 1 FROM RoomTypeAmenities WHERE RoomTypeId = @RoomType4PVIP AND AmenityId = @BanCong)
                        INSERT INTO RoomTypeAmenities (RoomTypeId, AmenityId, Quantity) VALUES (@RoomType4PVIP, @BanCong, 1);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
