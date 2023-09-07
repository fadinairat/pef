using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class LookupsMig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Employers_Lookups_TypeLookupId",
            //    table: "Employers");

            //migrationBuilder.DropIndex(
            //    name: "IX_Employers_TypeLookupId",
            //    table: "Employers");

            //migrationBuilder.DropColumn(
            //    name: "TypeLookupId",
            //    table: "Employers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeLookupId",
                table: "Employers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employers_TypeLookupId",
                table: "Employers",
                column: "TypeLookupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employers_Lookups_TypeLookupId",
                table: "Employers",
                column: "TypeLookupId",
                principalTable: "Lookups",
                principalColumn: "Id");
        }
    }
}
