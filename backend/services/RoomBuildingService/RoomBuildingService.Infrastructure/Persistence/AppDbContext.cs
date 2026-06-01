using Microsoft.EntityFrameworkCore;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<BuildingAnnouncement> BuildingAnnouncements => Set<BuildingAnnouncement>();
    public DbSet<Floor> Floors => Set<Floor>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<Amenity> Amenities => Set<Amenity>();
    public DbSet<RoomTypeAmenity> RoomTypeAmenities => Set<RoomTypeAmenity>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RoomImage> RoomImages => Set<RoomImage>();
    public DbSet<RoomStatusLog> RoomStatusLogs => Set<RoomStatusLog>();
    public DbSet<RoomInspection> RoomInspections => Set<RoomInspection>();
    public DbSet<RoomReservation> RoomReservations => Set<RoomReservation>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<Building>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Gender).HasMaxLength(10).IsRequired();
            e.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("Active");
        });

        mb.Entity<BuildingAnnouncement>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Category).HasMaxLength(30).HasDefaultValue("General");
            e.Property(x => x.Priority).HasMaxLength(10).HasDefaultValue("Normal");
            e.Property(x => x.CreatedByName).HasMaxLength(100);
            e.HasOne(x => x.Building)
             .WithMany(x => x.Announcements)
             .HasForeignKey(x => x.BuildingId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<Floor>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Label).HasMaxLength(50).IsRequired();
            e.HasOne(x => x.Building)
             .WithMany(x => x.Floors)
             .HasForeignKey(x => x.BuildingId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<RoomType>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(20).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.PricePerMonth).HasColumnType("decimal(18,2)");
            e.Property(x => x.DepositAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.ElectricityRate).HasColumnType("decimal(10,2)");
            e.Property(x => x.WaterRate).HasColumnType("decimal(10,2)");
            e.Property(x => x.Area).HasColumnType("decimal(8,2)");
            e.HasOne(x => x.Building)
             .WithMany(x => x.RoomTypes)
             .HasForeignKey(x => x.BuildingId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<Amenity>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Category).HasMaxLength(50).IsRequired();
        });

        mb.Entity<RoomTypeAmenity>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.RoomType)
             .WithMany(x => x.RoomTypeAmenities)
             .HasForeignKey(x => x.RoomTypeId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Amenity)
             .WithMany(x => x.RoomTypeAmenities)
             .HasForeignKey(x => x.AmenityId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<Room>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.RoomNumber).HasMaxLength(20).IsRequired();
            e.HasIndex(x => new { x.FloorId, x.RoomNumber }).IsUnique();
            e.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("Available");
            e.Property(x => x.QRCode).HasMaxLength(100);
            e.HasOne(x => x.Floor)
             .WithMany(x => x.Rooms)
             .HasForeignKey(x => x.FloorId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.RoomType)
             .WithMany(x => x.Rooms)
             .HasForeignKey(x => x.RoomTypeId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<RoomImage>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Room)
             .WithMany(x => x.Images)
             .HasForeignKey(x => x.RoomId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<RoomStatusLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.OldStatus).HasMaxLength(20);
            e.Property(x => x.NewStatus).HasMaxLength(20);
            e.Property(x => x.ChangedByName).HasMaxLength(100);
            e.HasOne(x => x.Room)
             .WithMany(x => x.StatusLogs)
             .HasForeignKey(x => x.RoomId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<RoomInspection>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.InspectorName).HasMaxLength(100).IsRequired();
            e.Property(x => x.InspectionType).HasMaxLength(20).HasDefaultValue("Routine");
            e.Property(x => x.OverallCondition).HasMaxLength(10);
            e.Property(x => x.ElectricalStatus).HasMaxLength(20).HasDefaultValue("OK");
            e.Property(x => x.PlumbingStatus).HasMaxLength(20).HasDefaultValue("OK");
            e.Property(x => x.FurnitureStatus).HasMaxLength(20).HasDefaultValue("OK");
            e.Property(x => x.WallStatus).HasMaxLength(20).HasDefaultValue("OK");
            e.Property(x => x.FloorStatus).HasMaxLength(20).HasDefaultValue("OK");
            e.Property(x => x.ElectricityReading).HasColumnType("decimal(10,2)");
            e.Property(x => x.WaterReading).HasColumnType("decimal(10,2)");
            e.Property(x => x.ApprovedByName).HasMaxLength(100);
            e.HasOne(x => x.Room)
             .WithMany(x => x.Inspections)
             .HasForeignKey(x => x.RoomId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<RoomReservation>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.StudentName).HasMaxLength(100);
            e.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("Holding");
            e.Property(x => x.CreatedByName).HasMaxLength(100);
            e.Property(x => x.ReleaseReason).HasMaxLength(50);
            e.HasIndex(x => new { x.RoomId, x.Status });
            e.HasOne(x => x.Room)
             .WithMany(x => x.Reservations)
             .HasForeignKey(x => x.RoomId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(ct);
    }
}
