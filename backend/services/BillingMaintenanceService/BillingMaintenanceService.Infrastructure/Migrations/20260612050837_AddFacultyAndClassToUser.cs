using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingMaintenanceService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFacultyAndClassToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Faculty",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Faculty",
                table: "Users");
        }
    }
}
