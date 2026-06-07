using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace BillingMaintenanceService.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // TEMPORARY: Force reset to update passwords to "123"
        var existingUsers = await context.Users.ToListAsync();
        if (existingUsers.Any())
        {
            Console.WriteLine("🔄 FORCE RESET: Deleting existing users to update passwords...");
            context.Users.RemoveRange(existingUsers);
            await context.SaveChangesAsync();
            
            // Clear change tracker to avoid conflicts
            context.ChangeTracker.Clear();
            
            Console.WriteLine($"✅ Deleted {existingUsers.Count} existing users");
        }

        Console.WriteLine("🌱 Seeding initial users...");

        var users = new List<User>
        {
            // Admin user
            new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                FullName = "Quản trị viên",
                Email = "admin@dormitory.com",
                Phone = "0901234567",
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            // Staff user
            new User
            {
                Username = "staff01",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff@123"),
                FullName = "Nguyễn Văn An",
                Email = "staff01@dormitory.com",
                Phone = "0902345678",
                Role = "Staff",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            // Student user
            new User
            {
                Username = "student01",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"),
                FullName = "Trần Thị Bình",
                Email = "student01@student.dormitory.com",
                Phone = "0903456789",
                Role = "Student",
                StudentCode = "SV001",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Seeded 3 initial users:");
        Console.WriteLine("   - Admin: username='admin', password='Admin@123'");
        Console.WriteLine("   - Staff: username='staff01', password='Staff@123'");
        Console.WriteLine("   - Student: username='student01', password='Student@123'");
    }
}
