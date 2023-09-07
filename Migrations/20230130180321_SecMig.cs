using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class SecMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramType = table.Column<int>(type: "int", nullable: false),
                    ProjectType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ArName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DonorLogo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    Partners = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArPartners = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectActivities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArProjectActivities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TargetGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArTargetGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BeneficiaryNumbers = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<int>(type: "int", nullable: true),
                    Duration = table.Column<float>(type: "real", nullable: false),
                    DurationUnit = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProjectCoordinator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArProjectCoordinator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Outputs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArOutputs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Conditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Standards = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArStandards = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedBy = table.Column<int>(type: "int", nullable: false),
                    AddedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedBy = table.Column<int>(type: "int", nullable: false),
                    LastEdit = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    LookupCurrenciesId = table.Column<int>(type: "int", nullable: true),
                    LookupProjectsProgsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_City_Location",
                        column: x => x.Location,
                        principalTable: "City",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Projects_LookupCurrencies_LookupCurrenciesId",
                        column: x => x.LookupCurrenciesId,
                        principalTable: "LookupCurrencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Projects_LookupProjectsProgs_LookupProjectsProgsId",
                        column: x => x.LookupProjectsProgsId,
                        principalTable: "LookupProjectsProgs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Projects_Users_AddedBy",
                        column: x => x.AddedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_AddedBy",
                table: "Projects",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Location",
                table: "Projects",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LookupCurrenciesId",
                table: "Projects",
                column: "LookupCurrenciesId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LookupProjectsProgsId",
                table: "Projects",
                column: "LookupProjectsProgsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
