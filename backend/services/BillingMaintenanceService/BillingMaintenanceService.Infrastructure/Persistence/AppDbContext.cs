using Microsoft.EntityFrameworkCore;
using BillingMaintenanceService.Domain.Entities;

namespace BillingMaintenanceService.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
    public DbSet<MaintenanceStatusLog> MaintenanceStatusLogs => Set<MaintenanceStatusLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<ContactInquiry> ContactInquiries => Set<ContactInquiry>();
    public DbSet<BuildingAssignment> BuildingAssignments => Set<BuildingAssignment>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Soft delete filters
        mb.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
        mb.Entity<Invoice>().HasQueryFilter(x => !x.IsDeleted);
        mb.Entity<Payment>().HasQueryFilter(x => !x.IsDeleted);
        mb.Entity<MaintenanceRequest>().HasQueryFilter(x => !x.IsDeleted);
        mb.Entity<Notification>().HasQueryFilter(x => !x.IsDeleted);
        mb.Entity<SystemSetting>().HasQueryFilter(x => !x.IsDeleted);
        mb.Entity<ContactInquiry>().HasQueryFilter(x => !x.IsDeleted);
        mb.Entity<BuildingAssignment>().HasQueryFilter(x => !x.IsDeleted);

        // User
        mb.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Username).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Username).IsUnique();
            e.Property(x => x.PasswordHash).IsRequired();
            e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Phone).HasMaxLength(15);
            e.Property(x => x.Role).HasMaxLength(10).IsRequired();
            e.Property(x => x.StudentCode).HasMaxLength(20);
            e.Property(x => x.LastLoginIp).HasMaxLength(50);
            e.Property(x => x.PasswordResetToken).HasMaxLength(100);
        });

        // Invoice
        mb.Entity<Invoice>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.InvoiceCode).HasMaxLength(20).IsRequired();
            e.HasIndex(x => x.InvoiceCode).IsUnique();
            e.HasIndex(x => new { x.ContractId, x.BillingMonth, x.BillingYear, x.InvoiceType }).IsUnique();
            e.Property(x => x.InvoiceType).HasMaxLength(20).HasDefaultValue("Monthly");
            e.Property(x => x.StudentName).HasMaxLength(100).IsRequired();
            e.Property(x => x.StudentCode).HasMaxLength(20).IsRequired();
            e.Property(x => x.RoomNumber).HasMaxLength(20).IsRequired();
            e.Property(x => x.BuildingName).HasMaxLength(150).IsRequired();
            e.Property(x => x.Status).HasMaxLength(15).HasDefaultValue("Unpaid");
            e.Property(x => x.RentAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.ElectricityAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.WaterAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.ServiceAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.PreviousDebt).HasColumnType("decimal(18,2)");
            e.Property(x => x.Discount).HasColumnType("decimal(18,2)");
            e.Property(x => x.PenaltyAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.PaidAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.DebtAmount).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.CreatedByUser)
             .WithMany(x => x.CreatedInvoices)
             .HasForeignKey(x => x.CreatedByUserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // InvoiceItem
        mb.Entity<InvoiceItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ItemName).HasMaxLength(100).IsRequired();
            e.Property(x => x.ItemDescription).HasMaxLength(200);
            e.Property(x => x.Unit).HasMaxLength(10);
            e.Property(x => x.Quantity).HasColumnType("decimal(10,3)");
            e.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
            e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Invoice)
             .WithMany(x => x.Items)
             .HasForeignKey(x => x.InvoiceId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Payment
        mb.Entity<Payment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            e.Property(x => x.Method).HasMaxLength(20).IsRequired();
            e.Property(x => x.TransactionCode).HasMaxLength(100);
            e.Property(x => x.BankName).HasMaxLength(100);
            e.Property(x => x.BankAccountNumber).HasMaxLength(30);
            e.Property(x => x.ReceivedByName).HasMaxLength(100).IsRequired();
            e.HasOne(x => x.Invoice)
             .WithMany(x => x.Payments)
             .HasForeignKey(x => x.InvoiceId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.ReceivedByUser)
             .WithMany(x => x.ReceivedPayments)
             .HasForeignKey(x => x.ReceivedByUserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // MaintenanceRequest
        mb.Entity<MaintenanceRequest>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.RoomNumber).HasMaxLength(20).IsRequired();
            e.Property(x => x.BuildingName).HasMaxLength(150).IsRequired();
            e.Property(x => x.RequestedByStudentName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Category).HasMaxLength(30).IsRequired();
            e.Property(x => x.Priority).HasMaxLength(10).HasDefaultValue("Medium");
            e.Property(x => x.Status).HasMaxLength(15).HasDefaultValue("New");
            e.Property(x => x.AssignedToName).HasMaxLength(100);
            e.Property(x => x.EstimatedCost).HasColumnType("decimal(18,2)");
            e.Property(x => x.ActualCost).HasColumnType("decimal(18,2)");
            e.Property(x => x.RejectedReason).HasMaxLength(300);
            e.HasOne(x => x.AssignedToUser)
             .WithMany(x => x.AssignedRequests)
             .HasForeignKey(x => x.AssignedToUserId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // MaintenanceStatusLog
        mb.Entity<MaintenanceStatusLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.OldStatus).HasMaxLength(15).IsRequired();
            e.Property(x => x.NewStatus).HasMaxLength(15).IsRequired();
            e.Property(x => x.ChangedByName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Note).HasMaxLength(300);
            e.HasOne(x => x.MaintenanceRequest)
             .WithMany(x => x.StatusLogs)
             .HasForeignKey(x => x.MaintenanceRequestId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Notification
        mb.Entity<Notification>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Body).IsRequired();
            e.Property(x => x.Type).HasMaxLength(50).IsRequired();
            e.Property(x => x.ActionUrl).HasMaxLength(200);
            e.Property(x => x.IconType).HasMaxLength(10);
            e.Property(x => x.RelatedEntityType).HasMaxLength(50);
            // Make User relationship optional since UserId can be from different service (StudentId)
            e.HasOne(x => x.User)
             .WithMany(x => x.Notifications)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired(false); // Allow UserId to not exist in Users table
        });

        // SystemSetting
        mb.Entity<SystemSetting>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Key).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Key).IsUnique();
            e.Property(x => x.DataType).HasMaxLength(10).HasDefaultValue("string");
            e.Property(x => x.Group).HasMaxLength(30);
            e.Property(x => x.Description).HasMaxLength(300);
            e.Property(x => x.UpdatedByName).HasMaxLength(100).IsRequired();
        });

        // AuditLog
        mb.Entity<AuditLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.UserName).HasMaxLength(100).IsRequired();
            e.Property(x => x.UserRole).HasMaxLength(10).IsRequired();
            e.Property(x => x.Action).HasMaxLength(20).IsRequired();
            e.Property(x => x.EntityType).HasMaxLength(50).IsRequired();
            e.Property(x => x.Description).HasMaxLength(300);
            e.Property(x => x.IpAddress).HasMaxLength(50);
            e.HasOne(x => x.User)
             .WithMany(x => x.AuditLogs)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ContactInquiry
        mb.Entity<ContactInquiry>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(100).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(15);
            e.Property(x => x.Subject).HasMaxLength(200).IsRequired();
            e.Property(x => x.Status).HasMaxLength(15).HasDefaultValue("New");
            e.Property(x => x.RepliedByName).HasMaxLength(100);
            e.Property(x => x.IpAddress).HasMaxLength(50);
        });

        // BuildingAssignment
        mb.Entity<BuildingAssignment>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UserId, x.BuildingId }).IsUnique();
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.BuildingId);
            e.HasOne(x => x.User)
             .WithMany()
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(ct);
    }
}
