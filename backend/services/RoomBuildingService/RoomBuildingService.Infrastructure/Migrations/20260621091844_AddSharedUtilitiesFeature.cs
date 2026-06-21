using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomBuildingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSharedUtilitiesFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublicFacilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildingId = table.Column<int>(type: "int", nullable: true),
                    BuildingName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BrandName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FacilityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Active"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ManagerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ManagerPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ManagerEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SocialLinks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperatingHours = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBookingRequired = table.Column<bool>(type: "bit", nullable: false),
                    MaxCapacity = table.Column<int>(type: "int", nullable: true),
                    FeePerHour = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FeePerSession = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FeeNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventSchedule = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVisibleOnHomepage = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    TotalUsageCount = table.Column<int>(type: "int", nullable: false),
                    LastMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicFacilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomAmenityInspections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomInspectionId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AmenityId = table.Column<int>(type: "int", nullable: true),
                    AmenityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AmenityCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Good"),
                    IssueDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecommendedAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaintenanceRequestId = table.Column<int>(type: "int", nullable: true),
                    NeedMaintenance = table.Column<bool>(type: "bit", nullable: false),
                    MaintenanceRequestCreated = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, defaultValue: "Medium"),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAmenityInspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomAmenityInspections_RoomInspections_RoomInspectionId",
                        column: x => x.RoomInspectionId,
                        principalTable: "RoomInspections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SharedUtilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildingId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UtilityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Available"),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ManagerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ManagerPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ManagerEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SocialLinks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FeePerUse = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OperatingHours = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsageInstructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InstallationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WarrantyMonths = table.Column<int>(type: "int", nullable: true),
                    WarrantyExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalUsageCount = table.Column<int>(type: "int", nullable: false),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedUtilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedUtilities_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacilityBookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StudentPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BookingDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    CancellationReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityBookings_PublicFacilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "PublicFacilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacilityReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacilityId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacilityReviews_PublicFacilities_FacilityId",
                        column: x => x.FacilityId,
                        principalTable: "PublicFacilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UtilityEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SharedUtilityId = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Scheduled"),
                    EstimatedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PerformedByUserId = table.Column<int>(type: "int", nullable: true),
                    PerformedByName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TechnicianCompany = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TechnicianContact = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MaintenanceRequestId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilityEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UtilityEvents_SharedUtilities_SharedUtilityId",
                        column: x => x.SharedUtilityId,
                        principalTable: "SharedUtilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UtilityUsageLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SharedUtilityId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    StudentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FeeCharged = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilityUsageLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UtilityUsageLogs_SharedUtilities_SharedUtilityId",
                        column: x => x.SharedUtilityId,
                        principalTable: "SharedUtilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FacilityBookings_FacilityId_BookingDate_Status",
                table: "FacilityBookings",
                columns: new[] { "FacilityId", "BookingDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_FacilityReviews_FacilityId_IsApproved",
                table: "FacilityReviews",
                columns: new[] { "FacilityId", "IsApproved" });

            migrationBuilder.CreateIndex(
                name: "IX_PublicFacilities_Category_IsVisibleOnHomepage",
                table: "PublicFacilities",
                columns: new[] { "Category", "IsVisibleOnHomepage" });

            migrationBuilder.CreateIndex(
                name: "IX_PublicFacilities_FacilityId",
                table: "PublicFacilities",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAmenityInspections_RoomInspectionId_AmenityId",
                table: "RoomAmenityInspections",
                columns: new[] { "RoomInspectionId", "AmenityId" });

            migrationBuilder.CreateIndex(
                name: "IX_SharedUtilities_BuildingId_Category_Status",
                table: "SharedUtilities",
                columns: new[] { "BuildingId", "Category", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SharedUtilities_UtilityId",
                table: "SharedUtilities",
                column: "UtilityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UtilityEvents_SharedUtilityId_EventDate",
                table: "UtilityEvents",
                columns: new[] { "SharedUtilityId", "EventDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UtilityEvents_Status_EventDate",
                table: "UtilityEvents",
                columns: new[] { "Status", "EventDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UtilityUsageLogs_SharedUtilityId_StartTime",
                table: "UtilityUsageLogs",
                columns: new[] { "SharedUtilityId", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_UtilityUsageLogs_StudentId_IsPaid",
                table: "UtilityUsageLogs",
                columns: new[] { "StudentId", "IsPaid" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacilityBookings");

            migrationBuilder.DropTable(
                name: "FacilityReviews");

            migrationBuilder.DropTable(
                name: "RoomAmenityInspections");

            migrationBuilder.DropTable(
                name: "UtilityEvents");

            migrationBuilder.DropTable(
                name: "UtilityUsageLogs");

            migrationBuilder.DropTable(
                name: "PublicFacilities");

            migrationBuilder.DropTable(
                name: "SharedUtilities");
        }
    }
}
