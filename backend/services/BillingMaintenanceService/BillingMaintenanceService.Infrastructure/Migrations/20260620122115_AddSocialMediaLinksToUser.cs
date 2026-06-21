using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingMaintenanceService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSocialMediaLinksToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FacebookUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZaloPhone",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FacebookUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InstagramUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LinkedInUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ZaloPhone",
                table: "Users");
        }
    }
}
