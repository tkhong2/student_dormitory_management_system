using ContractStudentService.Domain.Entities;
using ContractStudentService.Infrastructure.Persistence;

namespace ContractStudentService.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Seed Students
        if (!context.Students.Any())
        {
            var students = new List<Student>
            {
                new Student
                {
                    StudentCode = "SV001",
                    Faculty = "Công nghệ thông tin",
                    Major = "Công nghệ thông tin",
                    AcademicYear = 3,
                    ClassCode = "K65-CNTT",
                    FullName = "Trần Thị Bình",
                    Gender = "Female",
                    DateOfBirth = new DateOnly(2003, 5, 15),
                    PlaceOfBirth = "Hà Nội",
                    Ethnicity = "Kinh",
                    Religion = "Không",
                    Nationality = "Việt Nam",
                    Phone = "0903456789",
                    Email = "student01@student.dormitory.com",
                    IdentityCard = "001203012345",
                    IdentityCardIssuedDate = new DateOnly(2021, 1, 15),
                    IdentityCardIssuedPlace = "Hà Nội",
                    PermanentAddress = "123 Đường ABC, Quận 1, TP.HCM",
                    PermanentProvince = "TP. Hồ Chí Minh",
                    EmergencyContactName = "Trần Văn A",
                    EmergencyContactPhone = "0909123456",
                    EmergencyContactRelation = "Bố",
                    EmergencyContactAddress = "123 Đường ABC, Quận 1, TP.HCM",
                    UserId = 3, // Link to User 'student01' in BillingDB (assuming UserId=3)
                    IsActive = true,
                    ProfileCompletionPct = 100,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.Students.AddRangeAsync(students);
            await context.SaveChangesAsync();

            Console.WriteLine("✓ Seeded Students");
        }

        Console.WriteLine("\n=== ContractStudentService Data Seeding Complete ===\n");
    }
}
