using Microsoft.EntityFrameworkCore;
using RoomBuildingService.Domain.Entities;
using RoomBuildingService.Infrastructure.Persistence;

namespace RoomBuildingService.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        Console.WriteLine("=== Starting RoomBuildingDb Data Seeding ===");

        // 1. Seed Buildings (only add if not exists)
        var existingBuildingNames = context.Buildings.Select(b => b.Name).ToList();
        var buildings = new List<Building>();

        var buildingsToAdd = new[]
        {
            new { Name = "Tòa A", Gender = "Male", TotalFloors = 8, TotalRooms = 96, Manager = "Nguyễn Văn An", Phone = "0901234567", Year = "2018" },
            new { Name = "Tòa B", Gender = "Female", TotalFloors = 8, TotalRooms = 96, Manager = "Trần Thị Bình", Phone = "0902345678", Year = "2019" },
            new { Name = "Tòa C", Gender = "Mixed", TotalFloors = 10, TotalRooms = 120, Manager = "Lê Văn Cường", Phone = "0903456789", Year = "2022" }
        };

        foreach (var b in buildingsToAdd)
        {
            if (!existingBuildingNames.Contains(b.Name))
            {
                var building = new Building
                {
                    Name = b.Name,
                    Gender = b.Gender,
                    TotalFloors = b.TotalFloors,
                    TotalRooms = b.TotalRooms,
                    Address = "Số 1 Đại Cồ Việt, Hai Bà Trưng, Hà Nội",
                    Description = $"Tòa nhà {(b.Gender == "Male" ? "dành cho sinh viên nam" : b.Gender == "Female" ? "dành cho sinh viên nữ" : "dành cho cả nam và nữ")}, được xây dựng năm {b.Year}",
                    ManagerName = b.Manager,
                    ManagerPhone = b.Phone,
                    ConstructionYear = b.Year,
                    Status = "Active",
                    HasElevator = true,
                    HasParking = true,
                    HasLaundry = true,
                    IsActive = true
                };
                buildings.Add(building);
                await context.Buildings.AddAsync(building);
            }
        }

        if (buildings.Any())
        {
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Added {buildings.Count} new buildings");
        }
        else
        {
            // Load existing buildings for room type seeding
            buildings = await context.Buildings.ToListAsync();
            Console.WriteLine($"ℹ️  All buildings already exist, using existing {buildings.Count} buildings");
        }

        // 2. Seed RoomTypes (only add if not exists)
        // Tạo các loại phòng chung cho TẤT CẢ các tòa nhà
        var existingRoomTypeCombos = context.RoomTypes
            .Select(rt => new { rt.Code, rt.BuildingId })
            .ToList();
        var roomTypes = new List<RoomType>();

        var roomTypeTemplates = new[]
        {
            new { Code = "2P", Name = "Phòng 2 người", Capacity = 2, Price = 800000, Deposit = 800000, Area = 15.0m, BedType = "Single", AC = true, Water = true, Bath = false, Window = true, Desc = "Phòng 2 người, có điều hòa, view đẹp" },
            new { Code = "4P", Name = "Phòng 4 người", Capacity = 4, Price = 500000, Deposit = 500000, Area = 20.0m, BedType = "Bunk", AC = true, Water = false, Bath = false, Window = true, Desc = "Phòng 4 người, tiết kiệm" },
            new { Code = "6P", Name = "Phòng 6 người", Capacity = 6, Price = 350000, Deposit = 350000, Area = 25.0m, BedType = "Bunk", AC = false, Water = false, Bath = false, Window = true, Desc = "Phòng 6 người, giá rẻ" },
            new { Code = "4P-VIP", Name = "Phòng 4 người VIP", Capacity = 4, Price = 750000, Deposit = 750000, Area = 30.0m, BedType = "Single", AC = true, Water = true, Bath = true, Window = true, Desc = "Phòng VIP, đầy đủ tiện nghi" }
        };

        // Tạo loại phòng cho MỖI tòa nhà
        foreach (var building in buildings)
        {
            foreach (var template in roomTypeTemplates)
            {
                var combo = new { Code = template.Code, BuildingId = building.Id };
                if (!existingRoomTypeCombos.Any(x => x.Code == combo.Code && x.BuildingId == combo.BuildingId))
                {
                    var roomType = new RoomType
                    {
                        BuildingId = building.Id,
                        Code = template.Code,
                        Name = template.Name,
                        Capacity = template.Capacity,
                        PricePerMonth = template.Price,
                        DepositAmount = template.Deposit,
                        ElectricityRate = 3500,
                        WaterRate = template.Capacity <= 4 ? 20000 : 15000,
                        Area = template.Area,
                        BedType = template.BedType,
                        HasAirConditioner = template.AC,
                        HasWaterHeater = template.Water,
                        HasPrivateBathroom = template.Bath,
                        HasWindowView = template.Window,
                        Description = template.Desc,
                        IsActive = true
                    };
                    roomTypes.Add(roomType);
                    await context.RoomTypes.AddAsync(roomType);
                }
            }
        }

        if (roomTypes.Any())
        {
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Added {roomTypes.Count} new room types");
        }
        else
        {
            // Load existing room types
            roomTypes = await context.RoomTypes.ToListAsync();
            Console.WriteLine($"ℹ️  All room types already exist, using existing {roomTypes.Count} room types");
        }

        // 3. Seed Amenities (only add if not exists)
        var existingAmenityNames = context.Amenities.Select(a => a.Name).ToList();
        var amenities = new List<Amenity>();

        var amenitiesToAdd = new[]
        {
            new { Name = "Điều hòa", Category = "Electric", IconUrl = "❄️" },
            new { Name = "Nóng lạnh", Category = "Electric", IconUrl = "🚿" },
            new { Name = "WC riêng", Category = "Sanitary", IconUrl = "🚽" },
            new { Name = "Tủ lạnh", Category = "Electric", IconUrl = "🧊" },
            new { Name = "Giường tầng", Category = "Furniture", IconUrl = "🛏️" },
            new { Name = "Tủ quần áo", Category = "Furniture", IconUrl = "👔" },
            new { Name = "Bàn học", Category = "Furniture", IconUrl = "📚" },
            new { Name = "Kệ sách", Category = "Furniture", IconUrl = "📖" },
            new { Name = "Quạt trần", Category = "Electric", IconUrl = "💨" },
            new { Name = "Ban công", Category = "Other", IconUrl = "🪟" },
            new { Name = "Cửa sổ lớn", Category = "Other", IconUrl = "🪟" },
            new { Name = "TV", Category = "Electric", IconUrl = "📺" }
        };

        foreach (var a in amenitiesToAdd)
        {
            if (!existingAmenityNames.Contains(a.Name))
            {
                var amenity = new Amenity
                {
                    Name = a.Name,
                    Category = a.Category,
                    IconUrl = a.IconUrl,
                    IsActive = true
                };
                amenities.Add(amenity);
                await context.Amenities.AddAsync(amenity);
            }
        }

        if (amenities.Any())
        {
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Added {amenities.Count} new amenities");
        }
        else
        {
            // Load existing amenities
            amenities = await context.Amenities.ToListAsync();
            Console.WriteLine($"ℹ️  All amenities already exist, using existing {amenities.Count} amenities");
        }

        // 4. Seed RoomTypeAmenities (only add if not exists)
        var existingRoomTypeAmenities = await context.RoomTypeAmenities
            .Select(rta => new { rta.RoomTypeId, rta.AmenityId })
            .ToListAsync();

        var roomTypeAmenities = new List<RoomTypeAmenity>();

        // Định nghĩa amenities cho từng loại phòng (theo Code)
        var amenityMappings = new Dictionary<string, (string[] Names, Dictionary<string, int> Quantities)>
        {
            ["2P"] = (
                new[] { "Điều hòa", "Nóng lạnh", "Tủ quần áo", "Bàn học", "Kệ sách", "Cửa sổ lớn" },
                new Dictionary<string, int> { ["Bàn học"] = 2, ["Tủ quần áo"] = 2 }
            ),
            ["4P"] = (
                new[] { "Điều hòa", "Giường tầng", "Tủ quần áo", "Bàn học", "Quạt trần" },
                new Dictionary<string, int> { ["Giường tầng"] = 2, ["Bàn học"] = 4, ["Tủ quần áo"] = 4 }
            ),
            ["6P"] = (
                new[] { "Giường tầng", "Tủ quần áo", "Bàn học", "Quạt trần" },
                new Dictionary<string, int> { ["Giường tầng"] = 3, ["Bàn học"] = 6, ["Tủ quần áo"] = 6 }
            ),
            ["4P-VIP"] = (
                new[] { "Điều hòa", "Nóng lạnh", "WC riêng", "Tủ lạnh", "Tủ quần áo", "Bàn học", "TV", "Ban công" },
                new Dictionary<string, int> { ["Bàn học"] = 4, ["Tủ quần áo"] = 4 }
            )
        };

        // Áp dụng amenities cho TẤT CẢ room types (mỗi tòa đều có đầy đủ loại phòng)
        foreach (var roomTypeCode in amenityMappings.Keys)
        {
            var allRoomTypesWithCode = roomTypes.Where(rt => rt.Code == roomTypeCode).ToList();
            var (amenityNames, quantities) = amenityMappings[roomTypeCode];

            foreach (var roomType in allRoomTypesWithCode)
            {
                foreach (var amenityName in amenityNames)
                {
                    var amenity = amenities.FirstOrDefault(a => a.Name == amenityName);
                    if (amenity != null && !existingRoomTypeAmenities.Any(rta => rta.RoomTypeId == roomType.Id && rta.AmenityId == amenity.Id))
                    {
                        var quantity = quantities.ContainsKey(amenityName) ? quantities[amenityName] : 1;
                        roomTypeAmenities.Add(new RoomTypeAmenity
                        {
                            RoomTypeId = roomType.Id,
                            AmenityId = amenity.Id,
                            Quantity = quantity
                        });
                    }
                }
            }
        }

        if (roomTypeAmenities.Any())
        {
            await context.RoomTypeAmenities.AddRangeAsync(roomTypeAmenities);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Added {roomTypeAmenities.Count} room type amenities");
        }
        else
        {
            Console.WriteLine($"ℹ️  Room type amenities already exist");
        }

        // 5. Seed Floors for each building (only add if not exists)
        var existingFloorCount = await context.Floors.CountAsync();
        var floors = new List<Floor>();
        
        if (existingFloorCount == 0)
        {
            foreach (var building in buildings)
            {
                for (int i = 1; i <= building.TotalFloors; i++)
                {
                    var floor = new Floor
                    {
                        BuildingId = building.Id,
                        FloorNumber = i,
                        Label = $"Tầng {i}",
                        TotalRooms = 12,
                        IsActive = true
                    };
                    floors.Add(floor);
                    await context.Floors.AddAsync(floor);
                }
            }

            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Added {floors.Count} floors");
        }
        else
        {
            floors = await context.Floors.ToListAsync();
            Console.WriteLine($"ℹ️  Floors already exist, using existing {floors.Count} floors");
        }

        // 6. Seed Rooms (only add if not exists)
        var existingRoomNumbers = context.Rooms.Select(r => r.RoomNumber).ToList();
        var rooms = new List<Room>();
        var roomStatuses = new[] { "Available", "Available", "Available", "Available", "Maintenance" };
        int roomIndex = 0;
        int newRoomsCount = 0;

        // Only seed rooms if there are few existing rooms
        if (existingRoomNumbers.Count < 50)
        {
            // Building A - Floors 1-8
            var buildingAFloors = floors.Where(f => buildings.Any(b => b.Name == "Tòa A" && b.Id == f.BuildingId)).ToList();
            foreach (var floor in buildingAFloors)
            {
                for (int i = 1; i <= 12; i++)
                {
                    var roomNumber = $"{floor.FloorNumber}{i:D2}";
                    if (!existingRoomNumbers.Contains(roomNumber))
                    {
                        var roomType = i % 2 == 0 && roomTypes.Any(rt => rt.Code == "2P") 
                            ? roomTypes.First(rt => rt.Code == "2P")
                            : roomTypes.FirstOrDefault(rt => rt.Code == "4P");
                        
                        if (roomType != null)
                        {
                            rooms.Add(new Room
                            {
                                RoomNumber = roomNumber,
                                FloorId = floor.Id,
                                RoomTypeId = roomType.Id,
                                Status = roomStatuses[roomIndex % roomStatuses.Length],
                                CurrentOccupants = 0, // Chỉ hiển thị sinh viên đăng ký thật
                                MaxOccupants = roomType.Capacity,
                                Orientation = i % 4 == 0 ? "Đông" : i % 4 == 1 ? "Tây" : i % 4 == 2 ? "Nam" : "Bắc",
                                IsLocked = false
                            });
                            newRoomsCount++;
                        }
                    }
                    roomIndex++;
                }
            }

            // Building B - Floors 1-8
            var buildingBFloors = floors.Where(f => buildings.Any(b => b.Name == "Tòa B" && b.Id == f.BuildingId)).ToList();
            foreach (var floor in buildingBFloors)
            {
                for (int i = 1; i <= 12; i++)
                {
                    var roomNumber = $"{floor.FloorNumber}{i:D2}";
                    if (!existingRoomNumbers.Contains(roomNumber))
                    {
                        var roomType = roomTypes.FirstOrDefault(rt => rt.Code == "6P");
                        if (roomType != null)
                        {
                            rooms.Add(new Room
                            {
                                RoomNumber = roomNumber,
                                FloorId = floor.Id,
                                RoomTypeId = roomType.Id,
                                Status = roomStatuses[roomIndex % roomStatuses.Length],
                                CurrentOccupants = 0, // Chỉ hiển thị sinh viên đăng ký thật
                                MaxOccupants = roomType.Capacity,
                                Orientation = i % 4 == 0 ? "Đông" : i % 4 == 1 ? "Tây" : i % 4 == 2 ? "Nam" : "Bắc",
                                IsLocked = false
                            });
                            newRoomsCount++;
                        }
                    }
                    roomIndex++;
                }
            }

            // Building C - Floors 1-10
            var buildingCFloors = floors.Where(f => buildings.Any(b => b.Name == "Tòa C" && b.Id == f.BuildingId)).ToList();
            foreach (var floor in buildingCFloors)
            {
                for (int i = 1; i <= 12; i++)
                {
                    var roomNumber = $"{floor.FloorNumber}{i:D2}";
                    if (!existingRoomNumbers.Contains(roomNumber))
                    {
                        var roomType = roomTypes.FirstOrDefault(rt => rt.Code == "4P-VIP");
                        if (roomType != null)
                        {
                            rooms.Add(new Room
                            {
                                RoomNumber = roomNumber,
                                FloorId = floor.Id,
                                RoomTypeId = roomType.Id,
                                Status = roomStatuses[roomIndex % roomStatuses.Length],
                                CurrentOccupants = 0, // Chỉ hiển thị sinh viên đăng ký thật
                                MaxOccupants = roomType.Capacity,
                                Orientation = i % 4 == 0 ? "Đông" : i % 4 == 1 ? "Tây" : i % 4 == 2 ? "Nam" : "Bắc",
                                IsLocked = false
                            });
                            newRoomsCount++;
                        }
                    }
                    roomIndex++;
                }
            }

            if (rooms.Any())
            {
                await context.Rooms.AddRangeAsync(rooms);
                await context.SaveChangesAsync();
                Console.WriteLine($"✅ Added {newRoomsCount} new rooms");
            }
        }
        else
        {
            Console.WriteLine($"ℹ️  Rooms already exist ({existingRoomNumbers.Count} rooms), skipping room seeding");
        }

        Console.WriteLine("=== RoomBuildingDb Data Seeding Complete ===");
    }
}
