using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class MemAttUpdatesMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                table: "MembersEducation");

            migrationBuilder.AddColumn<string>(
                name: "AttachTitle1",
                table: "Members",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachTitle2",
                table: "Members",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachTitle3",
                table: "Members",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachTitle1",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "AttachTitle2",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "AttachTitle3",
                table: "Members");

            migrationBuilder.AddColumn<int>(
                name: "Path",
                table: "MembersEducation",
                type: "int",
                nullable: true);
        }
    }
}
