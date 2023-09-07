using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class EmployersUpMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Branches",
                table: "Employers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BranchesLocation",
                table: "Employers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HealthInsurance",
                table: "Employers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InjuredInsurance",
                table: "Employers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OtherAttachments",
                table: "Employers",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Branches",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "BranchesLocation",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "HealthInsurance",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "InjuredInsurance",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "OtherAttachments",
                table: "Employers");
        }
    }
}
