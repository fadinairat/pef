using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class AttJobMig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobsAppsAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobAppId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    JobsApplicationsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobsAppsAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobsAppsAttachments_JobsApplications_JobsApplicationsId",
                        column: x => x.JobsApplicationsId,
                        principalTable: "JobsApplications",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobsAppsAttachments_Jobs_JobAppId",
                        column: x => x.JobAppId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobsAppsAttachments_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobsAppsAttachments_JobAppId",
                table: "JobsAppsAttachments",
                column: "JobAppId");

            migrationBuilder.CreateIndex(
                name: "IX_JobsAppsAttachments_JobsApplicationsId",
                table: "JobsAppsAttachments",
                column: "JobsApplicationsId");

            migrationBuilder.CreateIndex(
                name: "IX_JobsAppsAttachments_MemberId",
                table: "JobsAppsAttachments",
                column: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobsAppsAttachments");
        }
    }
}
