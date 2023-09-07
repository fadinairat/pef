using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class AcceptTraMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AcceptTrainers",
                table: "Employers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CommercialRegister",
                table: "Employers",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FulltimeEmpCount",
                table: "Employers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FulltimeFemailEmpCount",
                table: "Employers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParttimeFemaleEmpCount",
                table: "Employers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PermanentFemaleEmpCount",
                table: "Employers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptTrainers",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "CommercialRegister",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "FulltimeEmpCount",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "FulltimeFemailEmpCount",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "ParttimeFemaleEmpCount",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "PermanentFemaleEmpCount",
                table: "Employers");
        }
    }
}
