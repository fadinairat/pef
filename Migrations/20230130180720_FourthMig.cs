using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class FourthMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_LookupProjectsProgs_LookupProjectsProgsId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_LookupProjectsProgsId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LookupProjectsProgsId",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProgramType",
                table: "Projects",
                column: "ProgramType");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_LookupProjectsProgs_ProgramType",
                table: "Projects",
                column: "ProgramType",
                principalTable: "LookupProjectsProgs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_LookupProjectsProgs_ProgramType",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProgramType",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "LookupProjectsProgsId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LookupProjectsProgsId",
                table: "Projects",
                column: "LookupProjectsProgsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_LookupProjectsProgs_LookupProjectsProgsId",
                table: "Projects",
                column: "LookupProjectsProgsId",
                principalTable: "LookupProjectsProgs",
                principalColumn: "Id");
        }
    }
}
