using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class AppForwMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Forwarded",
                table: "JobsApplications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Forwarded",
                table: "JobsApplications");
        }
    }
}
