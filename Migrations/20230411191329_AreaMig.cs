using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class AreaMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Villages_VillagesId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_VillagesId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "Members");

            migrationBuilder.RenameColumn(
                name: "VillagesId",
                table: "Members",
                newName: "SickNature");

            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_AreaId",
                table: "Members",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Villages_AreaId",
                table: "Members",
                column: "AreaId",
                principalTable: "Villages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Villages_AreaId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_AreaId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "Members");

            migrationBuilder.RenameColumn(
                name: "SickNature",
                table: "Members",
                newName: "VillagesId");

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "Members",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_VillagesId",
                table: "Members",
                column: "VillagesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Villages_VillagesId",
                table: "Members",
                column: "VillagesId",
                principalTable: "Villages",
                principalColumn: "Id");
        }
    }
}
