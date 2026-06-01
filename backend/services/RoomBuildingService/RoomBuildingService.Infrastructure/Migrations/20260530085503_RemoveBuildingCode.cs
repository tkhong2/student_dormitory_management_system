using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoomBuildingService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBuildingCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Buildings_Code",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Buildings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Buildings",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_Code",
                table: "Buildings",
                column: "Code",
                unique: true);
        }
    }
}
