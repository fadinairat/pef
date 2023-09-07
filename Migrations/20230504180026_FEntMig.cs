using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class FEntMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormsEntries_Jobs_JobId",
                table: "FormsEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_FormsEntries_Members_MemberId",
                table: "FormsEntries");

            migrationBuilder.AlterColumn<int>(
                name: "MemberId",
                table: "FormsEntries",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "JobId",
                table: "FormsEntries",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_FormsEntries_Jobs_JobId",
                table: "FormsEntries",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormsEntries_Members_MemberId",
                table: "FormsEntries",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormsEntries_Jobs_JobId",
                table: "FormsEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_FormsEntries_Members_MemberId",
                table: "FormsEntries");

            migrationBuilder.AlterColumn<int>(
                name: "MemberId",
                table: "FormsEntries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "JobId",
                table: "FormsEntries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FormsEntries_Jobs_JobId",
                table: "FormsEntries",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormsEntries_Members_MemberId",
                table: "FormsEntries",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
