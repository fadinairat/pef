using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class AreaEmMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CityName",
                table: "Employers",
                newName: "AreaId");

            migrationBuilder.AddColumn<int>(
                name: "AreaId1",
                table: "Employers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employers_AreaId1",
                table: "Employers",
                column: "AreaId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Employers_Villages_AreaId1",
                table: "Employers",
                column: "AreaId1",
                principalTable: "Villages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employers_Villages_AreaId1",
                table: "Employers");

            migrationBuilder.DropIndex(
                name: "IX_Employers_AreaId1",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "AreaId1",
                table: "Employers");

            migrationBuilder.RenameColumn(
                name: "AreaId",
                table: "Employers",
                newName: "CityName");
        }
    }
}
