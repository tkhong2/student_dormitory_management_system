using Microsoft.EntityFrameworkCore;
using BillingMaintenanceService.Domain.Entities;

namespace BillingMaintenanceService.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<Debt> Debts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<MaintenanceAssignment> MaintenanceAssignments { get; set; }
        public DbSet<MaintenanceLog> MaintenanceLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Accounts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.ReferenceId);

                entity.Property(e => e.Role)
                    .HasColumnName("RoleId")
                    .HasConversion<int>();

                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt);
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.ReferenceId);
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("Bills");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.BillCode).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.BillCode).IsUnique();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => new { e.StudentId, e.BillingYear, e.BillingMonth });
                entity.HasIndex(e => e.ContractId);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.PaymentMethod).HasConversion<int>();
                entity.Property(e => e.TransactionCode).HasMaxLength(100);
                entity.Property(e => e.Note).HasMaxLength(500);
                entity.Property(e => e.PaidAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.BillId);
                entity.HasIndex(e => e.StudentId);
                entity.HasIndex(e => e.TransactionCode);
                entity.HasOne(e => e.Bill)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(e => e.BillId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MaintenanceRequest>(entity =>
            {
                entity.ToTable("MaintenanceRequests");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.RequestCode).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.RequestCode).IsUnique();
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.StudentId);
                entity.HasIndex(e => e.RoomId);
                entity.HasIndex(e => e.Status);
            });

            modelBuilder.Entity<Debt>(entity =>
            {
                entity.ToTable("Debts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.PaidAmount).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.BillId);
                entity.HasIndex(e => e.StudentId);
                entity.HasIndex(e => e.Status);
                entity.HasOne(e => e.Bill)
                    .WithMany(e => e.Debts)
                    .HasForeignKey(e => e.BillId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.Token).IsUnique();
                entity.HasIndex(e => e.UserId);
                entity.HasOne(e => e.User)
                    .WithMany(e => e.RefreshTokens)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MaintenanceAssignment>(entity =>
            {
                entity.ToTable("MaintenanceAssignments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Note).HasMaxLength(1000);
                entity.Property(e => e.AssignedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.MaintenanceRequestId);
                entity.HasIndex(e => e.StaffUserId);
                entity.HasOne(e => e.MaintenanceRequest)
                    .WithMany(e => e.Assignments)
                    .HasForeignKey(e => e.MaintenanceRequestId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.StaffUser)
                    .WithMany(e => e.MaintenanceAssignments)
                    .HasForeignKey(e => e.StaffUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MaintenanceLog>(entity =>
            {
                entity.ToTable("MaintenanceLogs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.Note).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.MaintenanceRequestId);
                entity.HasIndex(e => e.MaintenanceAssignmentId);
                entity.HasIndex(e => e.CreatedByUserId);
                entity.HasOne(e => e.MaintenanceRequest)
                    .WithMany(e => e.Logs)
                    .HasForeignKey(e => e.MaintenanceRequestId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.MaintenanceAssignment)
                    .WithMany(e => e.Logs)
                    .HasForeignKey(e => e.MaintenanceAssignmentId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany(e => e.MaintenanceLogs)
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
