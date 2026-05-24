using ContractStudentService.Application.Interfaces;
using ContractStudentService.Domain.Entities;

namespace ContractStudentService.Infrastructure.Repositories
{
    public class MockStudentRepository : IStudentRepository
    {
        private readonly List<Student> _students = new()
        {
            CreateSeed("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1", "SV001", "Nguyễn Văn An", "101", "A1", "K65-CNTT", new DateTime(2026, 5, 15), "Active"),
            CreateSeed("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2", "SV002", "Trần Thị Dung", "102", "A1", "K64-KT", new DateTime(2026, 5, 20), "Active"),
            CreateSeed("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3", "SV003", "Lê Văn Đạt", "201", "B1", "K66-NN", new DateTime(2026, 6, 1), "Expiring"),
            CreateSeed("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4", "SV004", "Phạm Thị Ánh", "305", "C1", "K65-QTKD", new DateTime(2026, 6, 10), "Active"),
            CreateSeed("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa5", "SV005", "Ngô Thị Nhâm", "103", "A2", "K64-TCNH", new DateTime(2025, 1, 15), "Departed")
        };

        public Task<Student?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_students.FirstOrDefault(s => s.Id == id));
        }

        public Task<IEnumerable<Student>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Student>>(_students);
        }

        public Task AddAsync(Student student)
        {
            student.Id = Guid.NewGuid();
            _students.Add(student);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Student student)
        {
            var existing = _students.FirstOrDefault(s => s.Id == student.Id);
            if (existing != null)
            {
                existing.StudentCode = student.StudentCode;
                existing.FullName = student.FullName;
                existing.Email = student.Email;
                existing.PhoneNumber = student.PhoneNumber;
                existing.Address = student.Address;
                existing.DateOfBirth = student.DateOfBirth;
                existing.Gender = student.Gender;
                existing.RoomNumber = student.RoomNumber;
                existing.BuildingName = student.BuildingName;
                existing.ClassName = student.ClassName;
                existing.JoinDate = student.JoinDate;
                existing.Status = student.Status;
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(Student student)
        {
            _students.RemoveAll(s => s.Id == student.Id);
            return Task.CompletedTask;
        }

        private static Student CreateSeed(
            string id,
            string code,
            string name,
            string roomNumber,
            string buildingName,
            string className,
            DateTime joinDate,
            string status)
        {
            return new Student
            {
                Id = Guid.Parse(id),
                StudentCode = code,
                FullName = name,
                Email = $"{code.ToLowerInvariant()}@student.dnu.edu.vn",
                PhoneNumber = "0900000000",
                RoomNumber = roomNumber,
                BuildingName = buildingName,
                ClassName = className,
                JoinDate = joinDate,
                Status = status
            };
        }
    }
}
