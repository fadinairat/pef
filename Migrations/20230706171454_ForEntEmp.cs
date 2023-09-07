using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class ForEntEmp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormsEntries_Employers_EmployerId",
                table: "FormsEntries");

            migrationBuilder.DropIndex(
                name: "IX_FormsEntries_EmployerId",
                table: "FormsEntries");

            migrationBuilder.DropColumn(
                name: "EmployerId",
                table: "FormsEntries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployerId",
                table: "FormsEntries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormsEntries_EmployerId",
                table: "FormsEntries",
                column: "EmployerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormsEntries_Employers_EmployerId",
                table: "FormsEntries",
                column: "EmployerId",
                principalTable: "Employers",
                principalColumn: "Id");
        }
    }
}
