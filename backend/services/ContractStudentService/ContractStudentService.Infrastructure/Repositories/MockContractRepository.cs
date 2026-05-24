using ContractStudentService.Application.Interfaces;
using ContractStudentService.Domain.Entities;
using ContractStudentService.Domain.Enums;

namespace ContractStudentService.Infrastructure.Repositories
{
    public class MockContractRepository : IContractRepository
    {
        private readonly List<Contract> _contracts;

        public MockContractRepository()
        {
            _contracts = new List<Contract>
            {
                CreateSeed(
                    "11111111-1111-1111-1111-111111111111",
                    "HD-001",
                    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1",
                    "SV001",
                    "Nguyễn Văn Quyền",
                    "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1",
                    "101-A1",
                    new DateTime(2026, 1, 1),
                    new DateTime(2026, 12, 31),
                    800000,
                    ContractStatus.Active),
                CreateSeed(
                    "22222222-2222-2222-2222-222222222222",
                    "HD-002",
                    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2",
                    "SV002",
                    "Trần Thị Quyền",
                    "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2",
                    "102-A1",
                    new DateTime(2026, 2, 1),
                    new DateTime(2027, 1, 31),
                    1500000,
                    ContractStatus.Active),
                CreateSeed(
                    "33333333-3333-3333-3333-333333333333",
                    "HD-003",
                    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3",
                    "SV003",
                    "Lê Văn Cường",
                    "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3",
                    "201-B1",
                    new DateTime(2025, 9, 1),
                    new DateTime(2026, 5, 31),
                    800000,
                    ContractStatus.Active),
                CreateSeed(
                    "44444444-4444-4444-4444-444444444444",
                    "HD-004",
                    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4",
                    "SV004",
                    "Phạm Thị Dung",
                    "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4",
                    "305-C1",
                    new DateTime(2026, 3, 1),
                    new DateTime(2027, 2, 28),
                    2500000,
                    ContractStatus.Active),
                CreateSeed(
                    "55555555-5555-5555-5555-555555555555",
                    "HD-005",
                    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5",
                    "SV005",
                    "Ngô Thị Giang",
                    "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5",
                    "103-A2",
                    new DateTime(2025, 1, 15),
                    new DateTime(2026, 1, 14),
                    1500000,
                    ContractStatus.Terminated)
            };
        }

        public Task<Contract?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_contracts.FirstOrDefault(c => c.Id == id));
        }

        public Task<IEnumerable<Contract>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Contract>>(_contracts);
        }

        public Task AddAsync(Contract contract)
        {
            contract.Id = Guid.NewGuid();
            contract.CreatedAt = DateTime.UtcNow;
            _contracts.Add(contract);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Contract contract)
        {
            var existing = _contracts.FirstOrDefault(c => c.Id == contract.Id);
            if (existing != null)
            {
                existing.Code = contract.Code;
                existing.StudentId = contract.StudentId;
                existing.Student = contract.Student;
                existing.RoomId = contract.RoomId;
                existing.RoomNumber = contract.RoomNumber;
                existing.StartDate = contract.StartDate;
                existing.EndDate = contract.EndDate;
                existing.Price = contract.Price;
                existing.Status = contract.Status;
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(Contract contract)
        {
            _contracts.RemoveAll(c => c.Id == contract.Id);
            return Task.CompletedTask;
        }

        private static Contract CreateSeed(
            string id,
            string code,
            string studentId,
            string studentCode,
            string studentName,
            string roomId,
            string roomNumber,
            DateTime startDate,
            DateTime endDate,
            decimal price,
            ContractStatus status)
        {
            var parsedStudentId = Guid.Parse(studentId);
            return new Contract
            {
                Id = Guid.Parse(id),
                Code = code,
                StudentId = parsedStudentId,
                Student = new Student
                {
                    Id = parsedStudentId,
                    StudentCode = studentCode,
                    FullName = studentName
                },
                RoomId = Guid.Parse(roomId),
                RoomNumber = roomNumber,
                StartDate = startDate,
                EndDate = endDate,
                Price = price,
                Status = status,
                CreatedAt = DateTime.UtcNow.AddMonths(-3)
            };
        }
    }
}
