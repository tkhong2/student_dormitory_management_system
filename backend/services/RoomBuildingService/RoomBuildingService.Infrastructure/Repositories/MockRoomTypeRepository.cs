using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.Infrastructure.Repositories
{
    public class MockRoomTypeRepository : IRoomTypeRepository
    {
        private readonly List<RoomType> _roomTypes;

        public MockRoomTypeRepository()
        {
            _roomTypes = new List<RoomType>
            {
                new RoomType
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Name = "Phòng 4 người",
                    Price = 500000,
                    Capacity = 4,
                    Description = "Phòng tiêu chuẩn 4 người, có điều hòa, nước nóng"
                },
                new RoomType
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    Name = "Phòng 6 người",
                    Price = 400000,
                    Capacity = 6,
                    Description = "Phòng tiêu chuẩn 6 người, có điều hòa, nước nóng"
                },
                new RoomType
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    Name = "Phòng 8 người",
                    Price = 350000,
                    Capacity = 8,
                    Description = "Phòng tiêu chuẩn 8 người, có quạt, nước nóng"
                },
                new RoomType
                {
                    Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    Name = "Phòng VIP 2 người",
                    Price = 1000000,
                    Capacity = 2,
                    Description = "Phòng VIP 2 người, có điều hòa, nước nóng, tủ lạnh, TV"
                }
            };
        }

        public Task<RoomType?> GetByIdAsync(Guid id)
        {
            var roomType = _roomTypes.FirstOrDefault(rt => rt.Id == id);
            return Task.FromResult(roomType);
        }

        public Task<IEnumerable<RoomType>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<RoomType>>(_roomTypes);
        }

        public Task AddAsync(RoomType roomType)
        {
            roomType.Id = Guid.NewGuid();
            _roomTypes.Add(roomType);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(RoomType roomType)
        {
            var existing = _roomTypes.FirstOrDefault(rt => rt.Id == roomType.Id);
            if (existing != null)
            {
                existing.Name = roomType.Name;
                existing.Price = roomType.Price;
                existing.Capacity = roomType.Capacity;
                existing.Description = roomType.Description;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(RoomType roomType)
        {
            _roomTypes.RemoveAll(rt => rt.Id == roomType.Id);
            return Task.CompletedTask;
        }
    }
}
