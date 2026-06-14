using ContractStudentService.Domain.Entities;
using ContractStudentService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

        // Seed Student Documents
        if (!context.StudentDocuments.Any())
        {
            var student = await context.Students.FirstOrDefaultAsync();
            if (student != null)
            {
                var documents = new List<StudentDocument>
                {
                    new StudentDocument
                    {
                        StudentId = student.Id,
                        DocumentType = "CMND/CCCD",
                        DocumentName = "Chứng minh nhân dân - Trần Thị Bình",
                        FileUrl = "/uploads/documents/cmnd-sv001.pdf",
                        FileType = "application/pdf",
                        FileSizeBytes = 512000,
                        IsVerified = true,
                        VerifiedByUserId = 1,
                        VerifiedAt = DateTime.UtcNow,
                        Notes = "Đã xác minh",
                        CreatedAt = DateTime.UtcNow
                    },
                    new StudentDocument
                    {
                        StudentId = student.Id,
                        DocumentType = "Giấy xác nhận sinh viên",
                        DocumentName = "Giấy xác nhận SV - K65-CNTT",
                        FileUrl = "/uploads/documents/xacnhan-sv001.pdf",
                        FileType = "application/pdf",
                        FileSizeBytes = 256000,
                        IsVerified = false,
                        Notes = "Chờ xác minh",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.StudentDocuments.AddRangeAsync(documents);
                await context.SaveChangesAsync();

                Console.WriteLine("✓ Seeded Student Documents");
            }
        }

        Console.WriteLine("\n=== ContractStudentService Data Seeding Complete ===\n");
    }
}
