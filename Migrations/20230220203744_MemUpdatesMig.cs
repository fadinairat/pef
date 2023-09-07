using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class MemUpdatesMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Period",
                table: "MembersExpert");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "MembersTraining",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "MembersTraining",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrainingTasks",
                table: "MembersTraining",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "IdNum",
                table: "MembersFamily",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsWork",
                table: "MembersFamily",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "MembersExpert",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExpDescription",
                table: "MembersExpert",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "MembersExpert",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "MembersEducation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EducationLevelType",
                table: "MembersEducation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Grade",
                table: "MembersEducation",
                type: "decimal(3,2)",
                precision: 3,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Mobile2",
                table: "Members",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                table: "Members",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "IdNum",
                table: "Members",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Attach1",
                table: "Members",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Attach2",
                table: "Members",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Attach3",
                table: "Members",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisabilitySize",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisabilityType",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HealthAtt1",
                table: "Members",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HealthAtt2",
                table: "Members",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HealthAtt3",
                table: "Members",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HouseNature",
                table: "Members",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HouseSize",
                table: "Members",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IncomeExist",
                table: "Members",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "IncomeIdNumber",
                table: "Members",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MembersLess18",
                table: "Members",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MembersMore18",
                table: "Members",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "NeedTraining",
                table: "Members",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TrainingName",
                table: "Members",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "LookupEducation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LookupCountries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ArName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Flag = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupCountries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MembersFamily_EducationId",
                table: "MembersFamily",
                column: "EducationId");

            migrationBuilder.CreateIndex(
                name: "IX_MembersEducation_CountryId",
                table: "MembersEducation",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_MembersEducation_LookupCountries_CountryId",
                table: "MembersEducation",
                column: "CountryId",
                principalTable: "LookupCountries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MembersFamily_MembersEducation_EducationId",
                table: "MembersFamily",
                column: "EducationId",               
                principalTable: "MembersEducation",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembersEducation_LookupCountries_CountryId",
                table: "MembersEducation");

            migrationBuilder.DropForeignKey(
                name: "FK_MembersFamily_MembersEducation_EducationId",
                table: "MembersFamily");

            migrationBuilder.DropTable(
                name: "LookupCountries");

            migrationBuilder.DropIndex(
                name: "IX_MembersFamily_EducationId",
                table: "MembersFamily");

            migrationBuilder.DropIndex(
                name: "IX_MembersEducation_CountryId",
                table: "MembersEducation");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "MembersTraining");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "MembersTraining");

            migrationBuilder.DropColumn(
                name: "TrainingTasks",
                table: "MembersTraining");

            migrationBuilder.DropColumn(
                name: "IsWork",
                table: "MembersFamily");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "MembersExpert");

            migrationBuilder.DropColumn(
                name: "ExpDescription",
                table: "MembersExpert");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "MembersExpert");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "MembersEducation");

            migrationBuilder.DropColumn(
                name: "EducationLevelType",
                table: "MembersEducation");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "MembersEducation");

            migrationBuilder.DropColumn(
                name: "Attach1",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Attach2",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Attach3",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "DisabilitySize",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "DisabilityType",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "HealthAtt1",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "HealthAtt2",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "HealthAtt3",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "HouseNature",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "HouseSize",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "IncomeExist",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "IncomeIdNumber",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "MembersLess18",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "MembersMore18",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "NeedTraining",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "TrainingName",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "LookupEducation");

            migrationBuilder.AlterColumn<string>(
                name: "IdNum",
                table: "MembersFamily",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(9)",
                oldMaxLength: 9);

            migrationBuilder.AddColumn<int>(
                name: "Period",
                table: "MembersExpert",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Mobile2",
                table: "Members",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                table: "Members",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<int>(
                name: "IdNum",
                table: "Members",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(9)",
                oldMaxLength: 9);
        }
    }
}
