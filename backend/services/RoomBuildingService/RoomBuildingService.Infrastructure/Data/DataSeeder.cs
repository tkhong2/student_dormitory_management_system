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
            new { Name = "Tòa A", Gender = "Male", TotalFloors = 8, TotalRooms = 96, Manager = "Nguyễn Văn A", Phone = "0901234567", Year = "2018" },
            new { Name = "Tòa B", Gender = "Female", TotalFloors = 8, TotalRooms = 96, Manager = "Trần Thị B", Phone = "0902345678", Year = "2019" },
            new { Name = "Tòa C", Gender = "Mixed", TotalFloors = 10, TotalRooms = 120, Manager = "Lê Văn C", Phone = "0903456789", Year = "2022" }
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
        var existingRoomTypeCodes = context.RoomTypes.Select(rt => rt.Code).ToList();
        var roomTypes = new List<RoomType>();

        var roomTypesToAdd = new[]
        {
            new { Code = "2P", Name = "Phòng 2 người", BuildingIndex = 0, Capacity = 2, Price = 800000, Deposit = 800000, Area = 15.0m, BedType = "Single", AC = true, Water = true, Bath = false, Window = true, Desc = "Phòng 2 người, có điều hòa, view đẹp" },
            new { Code = "4P", Name = "Phòng 4 người", BuildingIndex = 0, Capacity = 4, Price = 500000, Deposit = 500000, Area = 20.0m, BedType = "Bunk", AC = true, Water = false, Bath = false, Window = true, Desc = "Phòng 4 người, tiết kiệm" },
            new { Code = "6P", Name = "Phòng 6 người", BuildingIndex = 1, Capacity = 6, Price = 350000, Deposit = 350000, Area = 25.0m, BedType = "Bunk", AC = false, Water = false, Bath = false, Window = true, Desc = "Phòng 6 người, giá rẻ" },
            new { Code = "4P-VIP", Name = "Phòng 4 người VIP", BuildingIndex = 2, Capacity = 4, Price = 750000, Deposit = 750000, Area = 30.0m, BedType = "Single", AC = true, Water = true, Bath = true, Window = true, Desc = "Phòng VIP, đầy đủ tiện nghi" }
        };

        foreach (var rt in roomTypesToAdd)
        {
            if (!existingRoomTypeCodes.Contains(rt.Code) && buildings.Count > rt.BuildingIndex)
            {
                var roomType = new RoomType
                {
                    BuildingId = buildings[rt.BuildingIndex].Id,
                    Code = rt.Code,
                    Name = rt.Name,
                    Capacity = rt.Capacity,
                    PricePerMonth = rt.Price,
                    DepositAmount = rt.Deposit,
                    ElectricityRate = 3500,
                    WaterRate = rt.Capacity <= 4 ? 20000 : 15000,
                    Area = rt.Area,
                    BedType = rt.BedType,
                    HasAirConditioner = rt.AC,
                    HasWaterHeater = rt.Water,
                    HasPrivateBathroom = rt.Bath,
                    HasWindowView = rt.Window,
                    Description = rt.Desc,
                    IsActive = true
                };
                roomTypes.Add(roomType);
                await context.RoomTypes.AddAsync(roomType);
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

        // 3. Seed Floors for each building (only add if not exists)
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

        // 4. Seed Rooms (only add if not exists)
        var existingRoomNumbers = context.Rooms.Select(r => r.RoomNumber).ToList();
        var rooms = new List<Room>();
        var roomStatuses = new[] { "Available", "Available", "Available", "Full", "Maintenance" };
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
                                CurrentOccupants = roomStatuses[roomIndex % roomStatuses.Length] == "Full" ? roomType.Capacity : 0,
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
                                CurrentOccupants = roomStatuses[roomIndex % roomStatuses.Length] == "Full" ? roomType.Capacity : roomIndex % 3,
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
                                CurrentOccupants = roomStatuses[roomIndex % roomStatuses.Length] == "Full" ? roomType.Capacity : roomIndex % 2,
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
