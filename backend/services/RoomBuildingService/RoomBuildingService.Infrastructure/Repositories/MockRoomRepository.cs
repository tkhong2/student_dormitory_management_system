using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;
using RoomBuildingService.Domain.Enums;

namespace RoomBuildingService.Infrastructure.Repositories
{
    public class MockRoomRepository : IRoomRepository
    {
        private readonly List<Room> _rooms;

        public MockRoomRepository()
        {
            // Building IDs
            var buildingAId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var buildingBId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var buildingCId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var buildingDId = Guid.Parse("44444444-4444-4444-4444-444444444444");

            // RoomType IDs
            var roomType4Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var roomType6Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            var roomType8Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
            var roomTypeVipId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

            _rooms = new List<Room>();

            // Chỉ tạo 10 phòng (5 phòng cho Tòa A, 5 phòng cho Tòa B)
            for (int i = 1; i <= 10; i++)
            {
                var isA = i <= 5;
                var roomNumber = isA ? $"A1{i:D2}" : $"B1{i - 5:D2}";
                var buildingId = isA ? buildingAId : buildingBId;
                var roomTypeId = roomType4Id;
                var capacity = 4;
                var occupancy = Random.Shared.Next(0, capacity + 1);
                var status = occupancy == 0 ? RoomStatus.Available : 
                            (occupancy >= capacity ? RoomStatus.Occupied : RoomStatus.Available);

                _rooms.Add(new Room
                {
                    Id = Guid.NewGuid(),
                    RoomNumber = roomNumber,
                    Floor = 1,
                    BuildingId = buildingId,
                    RoomTypeId = roomTypeId,
                    Status = status,
                    CurrentOccupancy = occupancy,
                    ImageUrl = i % 2 == 0 ? "/images/student_life.png" : "/images/hero_dormitory.png"
                });
            }
        }

        public Task<Room?> GetByIdAsync(Guid id)
        {
            var room = _rooms.FirstOrDefault(r => r.Id == id);
            return Task.FromResult(room);
        }

        public Task<IEnumerable<Room>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Room>>(_rooms);
        }

        public Task<IEnumerable<Room>> GetByBuildingIdAsync(Guid buildingId)
        {
            var rooms = _rooms.Where(r => r.BuildingId == buildingId);
            return Task.FromResult<IEnumerable<Room>>(rooms);
        }

        public Task AddAsync(Room room)
        {
            room.Id = Guid.NewGuid();
            _rooms.Add(room);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Room room)
        {
            var existing = _rooms.FirstOrDefault(r => r.Id == room.Id);
            if (existing != null)
            {
                existing.RoomNumber = room.RoomNumber;
                existing.Floor = room.Floor;
                existing.BuildingId = room.BuildingId;
                existing.RoomTypeId = room.RoomTypeId;
                existing.Status = room.Status;
                existing.CurrentOccupancy = room.CurrentOccupancy;
                existing.ImageUrl = room.ImageUrl;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Room room)
        {
            _rooms.RemoveAll(r => r.Id == room.Id);
            return Task.CompletedTask;
        }
    }
}
