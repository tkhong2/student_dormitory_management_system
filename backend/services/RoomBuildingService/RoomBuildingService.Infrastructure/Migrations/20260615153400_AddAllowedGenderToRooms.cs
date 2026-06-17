using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomBuildingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAllowedGenderToRooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllowedGender",
                table: "Rooms",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedGender",
                table: "Rooms");
        }
    }
}
