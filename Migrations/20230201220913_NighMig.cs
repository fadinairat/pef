using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class NighMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployerId",
                table: "JobsStatusTracking",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_JobsStatusTracking_EmployerId",
                table: "JobsStatusTracking",
                column: "EmployerId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobsStatusTracking_Employers_EmployerId",
                table: "JobsStatusTracking",
                column: "EmployerId",
                principalTable: "Employers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobsStatusTracking_Employers_EmployerId",
                table: "JobsStatusTracking");

            migrationBuilder.DropIndex(
                name: "IX_JobsStatusTracking_EmployerId",
                table: "JobsStatusTracking");

            migrationBuilder.DropColumn(
                name: "EmployerId",
                table: "JobsStatusTracking");
        }
    }
}
