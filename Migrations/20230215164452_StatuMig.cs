using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class StatuMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobsStatusTracking_Jobs_JobId",
                table: "JobsStatusTracking");

            migrationBuilder.RenameColumn(
                name: "JobId",
                table: "JobsStatusTracking",
                newName: "JobAppId");

            migrationBuilder.RenameIndex(
                name: "IX_JobsStatusTracking_JobId",
                table: "JobsStatusTracking",
                newName: "IX_JobsStatusTracking_JobAppId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobsStatusTracking_JobsApplications_JobAppId",
                table: "JobsStatusTracking",
                column: "JobAppId",
                principalTable: "JobsApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobsStatusTracking_JobsApplications_JobAppId",
                table: "JobsStatusTracking");

            migrationBuilder.RenameColumn(
                name: "JobAppId",
                table: "JobsStatusTracking",
                newName: "JobId");

            migrationBuilder.RenameIndex(
                name: "IX_JobsStatusTracking_JobAppId",
                table: "JobsStatusTracking",
                newName: "IX_JobsStatusTracking_JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobsStatusTracking_Jobs_JobId",
                table: "JobsStatusTracking",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
