using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class LookupsMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LookupsId",
                table: "Employers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Lookups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ArName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Editable = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lookups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employers_LookupsId",
                table: "Employers",
                column: "LookupsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employers_Lookups_LookupsId",
                table: "Employers",
                column: "LookupsId",
                principalTable: "Lookups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employers_Lookups_LookupsId",
                table: "Employers");

            migrationBuilder.DropTable(
                name: "Lookups");

            migrationBuilder.DropIndex(
                name: "IX_Employers_LookupsId",
                table: "Employers");

            migrationBuilder.DropColumn(
                name: "LookupsId",
                table: "Employers");
        }
    }
}
