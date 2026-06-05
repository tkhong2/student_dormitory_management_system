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
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<MaintenanceStatusLog> MaintenanceStatusLogs { get; set; }
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

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("Invoices");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.InvoiceCode).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.InvoiceCode).IsUnique();
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.Property(e => e.PaidAmount).HasPrecision(18, 2);
                entity.Property(e => e.DebtAmount).HasPrecision(18, 2);
                entity.Property(e => e.DueDate);
                entity.HasIndex(e => new { e.StudentId, e.BillingYear, e.BillingMonth });
                entity.HasIndex(e => e.ContractId);
                entity.Ignore(e => e.CreatedByUser);
                entity.HasMany(e => e.Items)
                    .WithOne(e => e.Invoice)
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.ToTable("InvoiceItems");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ItemName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ItemDescription).HasMaxLength(500);
                entity.Property(e => e.Quantity).HasPrecision(18, 2);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("Payments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.TransactionCode).HasMaxLength(100);
                entity.Property(e => e.Note).HasMaxLength(500);
                entity.Property(e => e.PaidAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.InvoiceId);
                entity.HasIndex(e => e.ReceivedByUserId);
                entity.Ignore(e => e.ReceivedByUser);
                entity.HasOne(e => e.Invoice)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MaintenanceRequest>(entity =>
            {
                entity.ToTable("MaintenanceRequests");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.RoomNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.BuildingName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.RequestedByStudentName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Priority).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.EstimatedCost).HasPrecision(18, 2);
                entity.Property(e => e.ActualCost).HasPrecision(18, 2);
                entity.HasIndex(e => e.RequestedByStudentId);
                entity.HasIndex(e => e.RoomId);
                entity.HasIndex(e => e.Status);
                entity.Ignore(e => e.AssignedToUser);
                entity.HasMany(e => e.StatusLogs)
                    .WithOne(e => e.MaintenanceRequest)
                    .HasForeignKey(e => e.MaintenanceRequestId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<MaintenanceStatusLog>(entity =>
            {
                entity.ToTable("MaintenanceStatusLogs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OldStatus).IsRequired().HasMaxLength(50);
                entity.Property(e => e.NewStatus).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Note).HasMaxLength(1000);
                entity.Property(e => e.ChangedByName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ChangedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.MaintenanceRequestId);
                entity.HasIndex(e => e.ChangedByUserId);
            });

            modelBuilder.Entity<Debt>(entity =>
            {
                entity.ToTable("Debts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.Property(e => e.PaidAmount).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.HasIndex(e => e.InvoiceId);
                entity.HasIndex(e => e.StudentId);
                entity.HasIndex(e => e.Status);
                entity.HasOne(e => e.Invoice)
                    .WithMany()
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens");
                entity.HasKey(e => e.Id);
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
                entity.Property(e => e.Note).HasMaxLength(1000);
                entity.Property(e => e.AssignedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.MaintenanceRequestId);
                entity.HasIndex(e => e.StaffUserId);
                entity.HasOne(e => e.MaintenanceRequest)
                    .WithMany()
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
                entity.Property(e => e.Status).HasConversion<int>();
                entity.Property(e => e.Note).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.MaintenanceRequestId);
                entity.HasIndex(e => e.MaintenanceAssignmentId);
                entity.HasIndex(e => e.CreatedByUserId);
                entity.HasOne(e => e.MaintenanceRequest)
                    .WithMany()
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
