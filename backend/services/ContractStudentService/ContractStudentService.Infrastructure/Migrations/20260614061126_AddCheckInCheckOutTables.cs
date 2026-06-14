using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractStudentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckInCheckOutTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CheckIns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BuildingName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CheckInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckedInByUserId = table.Column<int>(type: "int", nullable: false),
                    CheckedInByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IdCardImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDepositPaid = table.Column<bool>(type: "bit", nullable: false),
                    DepositAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DepositPaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RoomConditionChecklist = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomCondition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitialElectricityReading = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    InitialWaterReading = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    KeysProvided = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyCount = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckIns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckIns_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CheckIns_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CheckOuts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BuildingName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CheckOutDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckedOutByUserId = table.Column<int>(type: "int", nullable: false),
                    CheckedOutByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CurrentRoomImageUrls = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomCondition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DamageDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepositAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CompensationCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RefundAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CompensationDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinalElectricityReading = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    FinalWaterReading = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    IsKeyReturned = table.Column<bool>(type: "bit", nullable: false),
                    IsDepositRefunded = table.Column<bool>(type: "bit", nullable: false),
                    DepositRefundedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefundMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RefundReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckOuts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckOuts_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CheckOuts_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_CheckInDate",
                table: "CheckIns",
                column: "CheckInDate");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_ContractId",
                table: "CheckIns",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_RoomId",
                table: "CheckIns",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_StudentId",
                table: "CheckIns",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckOuts_CheckOutDate",
                table: "CheckOuts",
                column: "CheckOutDate");

            migrationBuilder.CreateIndex(
                name: "IX_CheckOuts_ContractId",
                table: "CheckOuts",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckOuts_RoomId",
                table: "CheckOuts",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckOuts_StudentId",
                table: "CheckOuts",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckIns");

            migrationBuilder.DropTable(
                name: "CheckOuts");
        }
    }
}
