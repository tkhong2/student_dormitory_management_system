using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractStudentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomApplicationNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SpecialRequirements",
                table: "RoomApplications",
                newName: "Preferences");

            migrationBuilder.AddColumn<bool>(
                name: "AgreedToRegulations",
                table: "RoomApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ConfirmedInformationAccuracy",
                table: "RoomApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DurationMonths",
                table: "RoomApplications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactName",
                table: "RoomApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactPhone",
                table: "RoomApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactRelationship",
                table: "RoomApplications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgreedToRegulations",
                table: "RoomApplications");

            migrationBuilder.DropColumn(
                name: "ConfirmedInformationAccuracy",
                table: "RoomApplications");

            migrationBuilder.DropColumn(
                name: "DurationMonths",
                table: "RoomApplications");

            migrationBuilder.DropColumn(
                name: "EmergencyContactName",
                table: "RoomApplications");

            migrationBuilder.DropColumn(
                name: "EmergencyContactPhone",
                table: "RoomApplications");

            migrationBuilder.DropColumn(
                name: "EmergencyContactRelationship",
                table: "RoomApplications");

            migrationBuilder.RenameColumn(
                name: "Preferences",
                table: "RoomApplications",
                newName: "SpecialRequirements");
        }
    }
}
