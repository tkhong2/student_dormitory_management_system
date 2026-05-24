using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;
using RoomBuildingService.Domain.Enums;

namespace RoomBuildingService.Infrastructure.Repositories
{
    public class MockBuildingRepository : IBuildingRepository
    {
        private readonly List<Building> _buildings;

        public MockBuildingRepository()
        {
            _buildings = new List<Building>
            {
                new Building
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Tòa A - Nam",
                    Description = "Tòa nhà dành cho sinh viên nam, 8 tầng",
                    NumberOfFloors = 8,
                    Type = BuildingType.Male,
                    ImageUrl = "/images/hero_dormitory.png",
                    CreatedAt = DateTime.Now.AddYears(-2),
                    UpdatedAt = DateTime.Now.AddMonths(-1)
                },
                new Building
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Tòa B - Nữ",
                    Description = "Tòa nhà dành cho sinh viên nữ, 8 tầng",
                    NumberOfFloors = 8,
                    Type = BuildingType.Female,
                    ImageUrl = "/images/student_life.png",
                    CreatedAt = DateTime.Now.AddYears(-2),
                    UpdatedAt = DateTime.Now.AddMonths(-1)
                },
                new Building
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Tòa C - Hỗn hợp",
                    Description = "Tòa nhà dành cho cả nam và nữ, 10 tầng",
                    NumberOfFloors = 10,
                    Type = BuildingType.Mixed,
                    ImageUrl = "/images/hero_dormitory.png",
                    CreatedAt = DateTime.Now.AddYears(-1),
                    UpdatedAt = DateTime.Now.AddDays(-15)
                },
                new Building
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "Tòa D - Nam",
                    Description = "Tòa nhà mới dành cho sinh viên nam, 6 tầng",
                    NumberOfFloors = 6,
                    Type = BuildingType.Male,
                    ImageUrl = "/images/student_life.png",
                    CreatedAt = DateTime.Now.AddMonths(-6),
                    UpdatedAt = DateTime.Now.AddDays(-5)
                }
            };
        }

        public Task<Building?> GetByIdAsync(Guid id)
        {
            var building = _buildings.FirstOrDefault(b => b.Id == id);
            return Task.FromResult(building);
        }

        public Task<IEnumerable<Building>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Building>>(_buildings);
        }

        public Task AddAsync(Building building)
        {
            building.Id = Guid.NewGuid();
            building.CreatedAt = DateTime.Now;
            building.UpdatedAt = DateTime.Now;
            _buildings.Add(building);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Building building)
        {
            var existing = _buildings.FirstOrDefault(b => b.Id == building.Id);
            if (existing != null)
            {
                existing.Name = building.Name;
                existing.Description = building.Description;
                existing.NumberOfFloors = building.NumberOfFloors;
                existing.Type = building.Type;
                existing.ImageUrl = building.ImageUrl;
                existing.UpdatedAt = DateTime.Now;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Building building)
        {
            _buildings.RemoveAll(b => b.Id == building.Id);
            return Task.CompletedTask;
        }
    }
}
