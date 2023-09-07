using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class JobsEditsUpdatesMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VillagesId",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrencyId",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Salary",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalaryType",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SelCityId",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelDistrict",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelFromAge",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelGender",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelSocialId",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelToAge",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelVillageId",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SocialStatusId",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Jobs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "WorkNature",
                table: "Jobs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "City",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Villages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Villages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Villages_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Members_VillagesId",
                table: "Members",
                column: "VillagesId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CurrencyId",
                table: "Jobs",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_SelCityId",
                table: "Jobs",
                column: "SelCityId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_SelVillageId",
                table: "Jobs",
                column: "SelVillageId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_SocialStatusId",
                table: "Jobs",
                column: "SocialStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Villages_CityId",
                table: "Villages",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_City_SelCityId",
                table: "Jobs",
                column: "SelCityId",
                principalTable: "City",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_LookupCurrencies_CurrencyId",
                table: "Jobs",
                column: "CurrencyId",
                principalTable: "LookupCurrencies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_LookupSocialStatus_SocialStatusId",
                table: "Jobs",
                column: "SocialStatusId",
                principalTable: "LookupSocialStatus",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Villages_SelVillageId",
                table: "Jobs",
                column: "SelVillageId",
                principalTable: "Villages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Villages_VillagesId",
                table: "Members",
                column: "VillagesId",
                principalTable: "Villages",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_City_SelCityId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_LookupCurrencies_CurrencyId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_LookupSocialStatus_SocialStatusId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Villages_SelVillageId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Villages_VillagesId",
                table: "Members");

            migrationBuilder.DropTable(
                name: "Villages");

            migrationBuilder.DropIndex(
                name: "IX_Members_VillagesId",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_CurrencyId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_SelCityId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_SelVillageId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_SocialStatusId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "VillagesId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SalaryType",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SelCityId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SelDistrict",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SelFromAge",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SelGender",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SelSocialId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SelToAge",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SelVillageId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SocialStatusId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "WorkNature",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "District",
                table: "City");
        }
    }
}
