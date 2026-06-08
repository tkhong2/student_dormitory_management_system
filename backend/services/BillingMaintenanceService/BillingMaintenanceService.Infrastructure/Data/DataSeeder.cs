using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace BillingMaintenanceService.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        Console.WriteLine("🌱 Seeding initial users...");

        var usersToSeed = new List<User>
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
                IsDeleted = false,
                DeletedAt = null
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
                IsDeleted = false,
                DeletedAt = null
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
                IsDeleted = false,
                DeletedAt = null
            }
        };

        foreach (var seedUser in usersToSeed)
        {
            // Query including soft-deleted users
            var existingUser = await context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Username == seedUser.Username);

            if (existingUser != null)
            {
                Console.WriteLine($"🔄 User '{seedUser.Username}' already exists. Updating properties, password, and resetting IsDeleted...");
                existingUser.PasswordHash = seedUser.PasswordHash;
                existingUser.FullName = seedUser.FullName;
                existingUser.Email = seedUser.Email;
                existingUser.Phone = seedUser.Phone;
                existingUser.Role = seedUser.Role;
                existingUser.StudentCode = seedUser.StudentCode;
                existingUser.IsActive = seedUser.IsActive;
                existingUser.IsDeleted = false;
                existingUser.DeletedAt = null;
                existingUser.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                Console.WriteLine($"🆕 User '{seedUser.Username}' does not exist. Adding user...");
                seedUser.CreatedAt = DateTime.UtcNow;
                await context.Users.AddAsync(seedUser);
            }
        }

        await context.SaveChangesAsync();

        Console.WriteLine("✅ Seeded/Updated 3 initial users successfully.");
    }
}
