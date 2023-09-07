using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class EmployersMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Section",
                table: "Employers",
                newName: "WorkSectorId");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Employers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AnnualReport",
                table: "Employers",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArDescription",
                table: "Employers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArExtraData",
                table: "Employers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArName",
                table: "Employers",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArSafetyProcedures",
                table: "Employers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityName",
                table: "Employers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Employers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactMobile",
                table: "Employers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactMobile2",
                table: "Employers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "Employers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DayWorkHours",
                table: "Employers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Employers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstablishYear",
                table: "Employers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ExtraData",
                table: "Employers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinancialReport",
                table: "Employers",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "Employers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperationNumber",
                table: "Employers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ParttimeEmpCount",
                table: "Employers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PermanentEmpCount",
                table: "Employers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RegNumber",
                table: "Employers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegistrationDocument",
                table: "Employers",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SafetyProcedures",
                table: "Employers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sector",
                table: "Employers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Shortcut",
                table: "Employers",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tel",
                table: "Employers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Village",
                table: "Employers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeekWorkDays",
                table: "Employers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "contactJobTitle",
                table: "Employers",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LookupWorkSector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupWorkSector", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employers_WorkSectorId",
                table: "Employers",
                column: "WorkSectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employers_LookupWorkSector_WorkSectorId",
                table: "Employers",
                column: "WorkSectorId",
                principalTable: "LookupWorkSector",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employers_LookupWorkSector_WorkSectorId",
                table: "Employers");

            migrationBuilder.DropTable(
                name: "LookupWorkSector");

            migrationBuilder.DropIndex(
                name: "IX_Employers_WorkSectorId",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "AnnualReport",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "ArDescription",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "ArExtraData",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "ArName",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "ArSafetyProcedures",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "CityName",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "ContactMobile",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "ContactMobile2",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "DayWorkHours",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "EstablishYear",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "ExtraData",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "FinancialReport",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "OperationNumber",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "ParttimeEmpCount",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "PermanentEmpCount",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "RegNumber",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "RegistrationDocument",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "SafetyProcedures",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "Sector",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "Shortcut",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "Tel",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "Village",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "WeekWorkDays",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "contactJobTitle",
                table: "Employers");

            migrationBuilder.RenameColumn(
                name: "WorkSectorId",
                table: "Employers",
                newName: "Section");
        }
    }
}
