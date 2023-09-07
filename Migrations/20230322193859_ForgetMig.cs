using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class ForgetMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ForgetCode",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ForgetTime",
                table: "Members",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForgetCode",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ForgetTime",
                table: "Members");
        }
    }
}
