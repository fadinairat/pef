using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class SevMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Delete",
                table: "MembersTraining",
                newName: "Deleted");

            migrationBuilder.RenameColumn(
                name: "Delete",
                table: "MembersExpert",
                newName: "Deleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "MembersTraining",
                newName: "Delete");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                table: "MembersExpert",
                newName: "Delete");
        }
    }
}
