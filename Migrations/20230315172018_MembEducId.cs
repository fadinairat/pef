using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class MembEducId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembersFamily_MembersEducation_EducationId",
                table: "MembersFamily");

            migrationBuilder.AlterColumn<int>(
                name: "HealthStatus",
                table: "MembersFamily",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddForeignKey(
                name: "FK_MembersFamily_LookupEducation_EducationId",
                table: "MembersFamily",
                column: "EducationId",
                principalTable: "LookupEducation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembersFamily_LookupEducation_EducationId",
                table: "MembersFamily");

            migrationBuilder.AlterColumn<bool>(
                name: "HealthStatus",
                table: "MembersFamily",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_MembersFamily_MembersEducation_EducationId",
                table: "MembersFamily",
                column: "EducationId",
                principalTable: "MembersEducation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
