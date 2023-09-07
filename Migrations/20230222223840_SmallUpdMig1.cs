using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class SmallUpdMig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Jobs_LookupSocialStatus_SocialStatusId",
            //    table: "Jobs");

            //migrationBuilder.DropIndex(
            //    name: "IX_Jobs_SocialStatusId",
            //    table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SocialStatusId",
                table: "Jobs");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_SelSocialId",
                table: "Jobs",
                column: "SelSocialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_LookupSocialStatus_SelSocialId",
                table: "Jobs",
                column: "SelSocialId",
                principalTable: "LookupSocialStatus",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_LookupSocialStatus_SelSocialId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_SelSocialId",
                table: "Jobs");

            migrationBuilder.AddColumn<int>(
                name: "SocialStatusId",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_SocialStatusId",
                table: "Jobs",
                column: "SocialStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_LookupSocialStatus_SocialStatusId",
                table: "Jobs",
                column: "SocialStatusId",
                principalTable: "LookupSocialStatus",
                principalColumn: "Id");
        }
    }
}
