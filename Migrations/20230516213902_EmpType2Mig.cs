using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class EmpType2Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employers_LookupEmployersTypes_Type",
                table: "Employers");

            migrationBuilder.DropTable(
                name: "LookupEmployersTypes");

            migrationBuilder.DropIndex(
                name: "IX_Employers_Type",
                table: "Employers");

            migrationBuilder.CreateTable(
                name: "LookupEmployersSectors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ArName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupEmployersSectors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employers_Sector",
                table: "Employers",
                column: "Sector");

            migrationBuilder.AddForeignKey(
                name: "FK_Employers_LookupEmployersSectors_Sector",
                table: "Employers",
                column: "Sector",
                principalTable: "LookupEmployersSectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employers_LookupEmployersSectors_Sector",
                table: "Employers");

            migrationBuilder.DropTable(
                name: "LookupEmployersSectors");

            migrationBuilder.DropIndex(
                name: "IX_Employers_Sector",
                table: "Employers");

            migrationBuilder.CreateTable(
                name: "LookupEmployersTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LookupEmployersTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employers_Type",
                table: "Employers",
                column: "Type");

            migrationBuilder.AddForeignKey(
                name: "FK_Employers_LookupEmployersTypes_Type",
                table: "Employers",
                column: "Type",
                principalTable: "LookupEmployersTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
