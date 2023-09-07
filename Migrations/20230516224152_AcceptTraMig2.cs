using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class AcceptTraMig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FulltimeFemailEmpCount",
                table: "Employers",
                newName: "FulltimeFemaleEmpCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FulltimeFemaleEmpCount",
                table: "Employers",
                newName: "FulltimeFemailEmpCount");
        }
    }
}
