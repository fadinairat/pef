using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class ForPubMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FormId",
                table: "Pages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsJobForm",
                table: "Forms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Forms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Pages_FormId",
                table: "Pages",
                column: "FormId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_Forms_FormId",
                table: "Pages",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_Forms_FormId",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Pages_FormId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "FormId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "IsJobForm",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Forms");
        }
    }
}
