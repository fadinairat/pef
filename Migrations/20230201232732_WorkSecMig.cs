using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class WorkSecMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employers_LookupWorkSector_WorkSectorId",
                table: "Employers");

            migrationBuilder.DropTable(
                name: "LookupWorkSector");

            migrationBuilder.CreateTable(
                name: "LookupWorkSectors",
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
                    table.PrimaryKey("PK_LookupWorkSectors", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Employers_LookupWorkSectors_WorkSectorId",
                table: "Employers",
                column: "WorkSectorId",
                principalTable: "LookupWorkSectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employers_LookupWorkSectors_WorkSectorId",
                table: "Employers");

            migrationBuilder.DropTable(
                name: "LookupWorkSectors");

            migrationBuilder.CreateTable(
                name: "LookupWorkSector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupWorkSector", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Employers_LookupWorkSector_WorkSectorId",
                table: "Employers",
                column: "WorkSectorId",
                principalTable: "LookupWorkSector",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
