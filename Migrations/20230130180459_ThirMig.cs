using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class ThirMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_LookupCurrencies_LookupCurrenciesId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_LookupCurrenciesId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "LookupCurrenciesId",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CurrencyId",
                table: "Projects",
                column: "CurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_LookupCurrencies_CurrencyId",
                table: "Projects",
                column: "CurrencyId",
                principalTable: "LookupCurrencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_LookupCurrencies_CurrencyId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_CurrencyId",
                table: "Projects");

            migrationBuilder.AddColumn<int>(
                name: "LookupCurrenciesId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LookupCurrenciesId",
                table: "Projects",
                column: "LookupCurrenciesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_LookupCurrencies_LookupCurrenciesId",
                table: "Projects",
                column: "LookupCurrenciesId",
                principalTable: "LookupCurrencies",
                principalColumn: "Id");
        }
    }
}
