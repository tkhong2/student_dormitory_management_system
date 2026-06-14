using Microsoft.EntityFrameworkCore;
using ContractStudentService.Domain.Entities;

namespace ContractStudentService.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<StudentDocument> StudentDocuments => Set<StudentDocument>();
        public DbSet<RoomApplication> RoomApplications => Set<RoomApplication>();
        public DbSet<Contract> Contracts => Set<Contract>();
        public DbSet<ContractExtension> ContractExtensions => Set<ContractExtension>();
        public DbSet<RoomTransfer> RoomTransfers => Set<RoomTransfer>();
        public DbSet<ContractTemplate> ContractTemplates => Set<ContractTemplate>();
        public DbSet<ContractTerm> ContractTerms => Set<ContractTerm>();
        public DbSet<CheckIn> CheckIns => Set<CheckIn>();
        public DbSet<CheckOut> CheckOuts => Set<CheckOut>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Student
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Students");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.StudentCode).IsUnique();
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.StudentCode).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Faculty).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Major).HasMaxLength(200).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Gender).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Phone).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
                entity.Property(e => e.IdentityCard).HasMaxLength(20).IsRequired();
                entity.Property(e => e.PermanentAddress).HasMaxLength(500).IsRequired();
                entity.Property(e => e.EmergencyContactName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.EmergencyContactPhone).HasMaxLength(20).IsRequired();
                entity.Property(e => e.EmergencyContactRelation).HasMaxLength(50).IsRequired();
            });

            // StudentDocument
            modelBuilder.Entity<StudentDocument>(entity =>
            {
                entity.ToTable("StudentDocuments");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.StudentId);

                entity.Property(e => e.DocumentType).HasMaxLength(50).IsRequired();
                entity.Property(e => e.DocumentName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.FileUrl).HasMaxLength(500).IsRequired();
                entity.Property(e => e.FileType).HasMaxLength(100).IsRequired();

                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Documents)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // RoomApplication
            modelBuilder.Entity<RoomApplication>(entity =>
            {
                entity.ToTable("RoomApplications");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.StudentId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);

                entity.Property(e => e.PreferredBuildingName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.PreferredRoomTypeName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.PreferredRoomPrice).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();

                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Applications)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Contract
            modelBuilder.Entity<Contract>(entity =>
            {
                entity.ToTable("Contracts");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.StudentId);
                entity.HasIndex(e => e.ApplicationId);
                entity.HasIndex(e => e.ContractCode).IsUnique();
                entity.HasIndex(e => e.Status);

                entity.Property(e => e.ContractCode).HasMaxLength(20).IsRequired();
                entity.Property(e => e.RoomNumber).HasMaxLength(20).IsRequired();
                entity.Property(e => e.BuildingName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.RoomTypeName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.MonthlyRent).HasPrecision(18, 2);
                entity.Property(e => e.DepositAmount).HasPrecision(18, 2);
                entity.Property(e => e.ElectricityRate).HasPrecision(18, 2);
                entity.Property(e => e.WaterRate).HasPrecision(18, 2);
                entity.Property(e => e.DepositReturnedAmount).HasPrecision(18, 2);
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();

                entity.HasOne(e => e.Student)
                    .WithMany(s => s.Contracts)
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Application)
                    .WithOne(a => a.Contract)
                    .HasForeignKey<Contract>(e => e.ApplicationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ContractExtension
            modelBuilder.Entity<ContractExtension>(entity =>
            {
                entity.ToTable("ContractExtensions");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ContractId);

                entity.Property(e => e.ApprovedByName).HasMaxLength(200).IsRequired();

                entity.HasOne(e => e.Contract)
                    .WithMany(c => c.Extensions)
                    .HasForeignKey(e => e.ContractId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // RoomTransfer
            modelBuilder.Entity<RoomTransfer>(entity =>
            {
                entity.ToTable("RoomTransfers");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ContractId);
                entity.HasIndex(e => e.Status);

                entity.Property(e => e.CurrentRoomNumber).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Reason).HasMaxLength(1000).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();

                entity.HasOne(e => e.Contract)
                    .WithMany(c => c.RoomTransfers)
                    .HasForeignKey(e => e.ContractId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ContractTemplate
            modelBuilder.Entity<ContractTemplate>(entity =>
            {
                entity.ToTable("ContractTemplates");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.IsDefault);
                entity.HasIndex(e => e.IsActive);

                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.TemplateContent).HasMaxLength(10000);
            });

            // ContractTerm
            modelBuilder.Entity<ContractTerm>(entity =>
            {
                entity.ToTable("ContractTerms");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ContractTemplateId);
                entity.HasIndex(e => e.OrderIndex);

                entity.Property(e => e.Title).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Content).HasMaxLength(2000).IsRequired();
                entity.Property(e => e.Icon).HasMaxLength(50);

                entity.HasOne(e => e.ContractTemplate)
                    .WithMany(ct => ct.Terms)
                    .HasForeignKey(e => e.ContractTemplateId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CheckIn
            modelBuilder.Entity<CheckIn>(entity =>
            {
                entity.ToTable("CheckIns");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ContractId);
                entity.HasIndex(e => e.StudentId);
                entity.HasIndex(e => e.RoomId);
                entity.HasIndex(e => e.CheckInDate);

                entity.Property(e => e.RoomNumber).HasMaxLength(20).IsRequired();
                entity.Property(e => e.BuildingName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.CheckedInByName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.DepositAmount).HasPrecision(18, 2);
                entity.Property(e => e.InitialElectricityReading).HasPrecision(18, 2);
                entity.Property(e => e.InitialWaterReading).HasPrecision(18, 2);
                entity.Property(e => e.RoomCondition).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();

                entity.HasOne(e => e.Contract)
                    .WithMany()
                    .HasForeignKey(e => e.ContractId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // CheckOut
            modelBuilder.Entity<CheckOut>(entity =>
            {
                entity.ToTable("CheckOuts");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ContractId);
                entity.HasIndex(e => e.StudentId);
                entity.HasIndex(e => e.RoomId);
                entity.HasIndex(e => e.CheckOutDate);

                entity.Property(e => e.RoomNumber).HasMaxLength(20).IsRequired();
                entity.Property(e => e.BuildingName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.CheckedOutByName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.DepositAmount).HasPrecision(18, 2);
                entity.Property(e => e.CompensationCost).HasPrecision(18, 2);
                entity.Property(e => e.RefundAmount).HasPrecision(18, 2);
                entity.Property(e => e.FinalElectricityReading).HasPrecision(18, 2);
                entity.Property(e => e.FinalWaterReading).HasPrecision(18, 2);
                entity.Property(e => e.RoomCondition).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
                entity.Property(e => e.RefundMethod).HasMaxLength(50);

                entity.HasOne(e => e.Contract)
                    .WithMany()
                    .HasForeignKey(e => e.ContractId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Student)
                    .WithMany()
                    .HasForeignKey(e => e.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
