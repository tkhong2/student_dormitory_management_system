using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractStudentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeApplicationIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contracts_ApplicationId",
                table: "Contracts");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationId",
                table: "Contracts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ApplicationId",
                table: "Contracts",
                column: "ApplicationId",
                unique: true,
                filter: "[ApplicationId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Contracts_ApplicationId",
                table: "Contracts");

            migrationBuilder.AlterColumn<int>(
                name: "ApplicationId",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ApplicationId",
                table: "Contracts",
                column: "ApplicationId",
                unique: true);
        }
    }
}
