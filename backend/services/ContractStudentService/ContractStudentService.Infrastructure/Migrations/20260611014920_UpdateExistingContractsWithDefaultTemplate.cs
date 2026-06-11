using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractStudentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExistingContractsWithDefaultTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update all existing contracts that don't have a ContractTemplateId
            // Set them to use the default template (STANDARD)
            migrationBuilder.Sql(@"
                UPDATE Contracts 
                SET ContractTemplateId = (
                    SELECT TOP 1 Id FROM ContractTemplates 
                    WHERE Code = 'STANDARD' AND IsDefault = 1
                )
                WHERE ContractTemplateId IS NULL
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
