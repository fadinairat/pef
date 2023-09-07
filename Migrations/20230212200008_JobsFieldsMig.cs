using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class JobsFieldsMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_City_CityId",
                table: "Jobs");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "Jobs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "JobsFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    FieldId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobsFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobsFields_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobsFields_LookupJobsFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "LookupJobsFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobsFields_FieldId",
                table: "JobsFields",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_JobsFields_JobId",
                table: "JobsFields",
                column: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_City_CityId",
                table: "Jobs",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_City_CityId",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "JobsFields");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_City_CityId",
                table: "Jobs",
                column: "CityId",
                principalTable: "City",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
