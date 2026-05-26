using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using RoomBuildingService.Infrastructure.Persistence;

#nullable disable

namespace RoomBuildingService.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("RoomBuildingService.Domain.Entities.Building", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<DateTime>("CreatedAt")
                    .HasColumnType("datetime2");

                b.Property<string>("Description")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("ImageUrl")
                    .HasMaxLength(255)
                    .HasColumnType("nvarchar(255)");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("nvarchar(100)");

                b.Property<int>("NumberOfFloors")
                    .HasColumnType("int");

                b.Property<int>("Type")
                    .HasColumnType("int");

                b.Property<DateTime>("UpdatedAt")
                    .HasColumnType("datetime2");

                b.HasKey("Id");

                b.ToTable("Buildings");
            });

            modelBuilder.Entity("RoomBuildingService.Domain.Entities.RoomType", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<string>("Description")
                    .IsRequired()
                    .HasColumnType("nvarchar(max)");

                b.Property<int>("Capacity")
                    .HasColumnType("int");

                b.Property<string>("Name")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("nvarchar(50)");

                b.Property<decimal>("Price")
                    .HasColumnType("decimal(18,2)");

                b.HasKey("Id");

                b.ToTable("RoomTypes");
            });

            modelBuilder.Entity("RoomBuildingService.Domain.Entities.Room", b =>
            {
                b.Property<Guid>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("uniqueidentifier");

                b.Property<Guid>("BuildingId")
                    .HasColumnType("uniqueidentifier");

                b.Property<int>("CurrentOccupancy")
                    .HasColumnType("int");

                b.Property<int>("Floor")
                    .HasColumnType("int");

                b.Property<string>("ImageUrl")
                    .HasMaxLength(255)
                    .HasColumnType("nvarchar(255)");

                b.Property<string>("RoomNumber")
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnType("nvarchar(20)");

                b.Property<Guid>("RoomTypeId")
                    .HasColumnType("uniqueidentifier");

                b.Property<int>("Status")
                    .HasColumnType("int");

                b.HasKey("Id");

                b.HasIndex("BuildingId");

                b.HasIndex("RoomTypeId");

                b.ToTable("Rooms");
            });

            modelBuilder.Entity("RoomBuildingService.Domain.Entities.Room", b =>
            {
                b.HasOne("RoomBuildingService.Domain.Entities.Building", "Building")
                    .WithMany("Rooms")
                    .HasForeignKey("BuildingId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("RoomBuildingService.Domain.Entities.RoomType", "RoomType")
                    .WithMany("Rooms")
                    .HasForeignKey("RoomTypeId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });
        }
    }
}
