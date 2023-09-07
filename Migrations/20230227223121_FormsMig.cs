using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PEF.Migrations
{
    /// <inheritdoc />
    public partial class FormsMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FormId",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LangId = table.Column<int>(type: "int", nullable: false),
                    SubmitLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArSubmitLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddedBy = table.Column<int>(type: "int", nullable: false),
                    AddedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedBy = table.Column<int>(type: "int", nullable: true),
                    LastEdit = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Forms_Languages_LangId",
                        column: x => x.LangId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Forms_Users_AddedBy",
                        column: x => x.AddedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormsFieldsTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormsFieldsTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormsEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    FormId = table.Column<int>(type: "int", nullable: false),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormsEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormsEntries_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormsEntries_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormsEntries_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormsFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ArTitle = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ArLabel = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PlaceHolder = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ArPlaceHolder = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MinAnsNumber = table.Column<int>(type: "int", nullable: false),
                    Required = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinLength = table.Column<int>(type: "int", nullable: false),
                    MaxLength = table.Column<int>(type: "int", nullable: false),
                    Rows = table.Column<int>(type: "int", nullable: false),
                    AllowMultiple = table.Column<bool>(type: "bit", nullable: false),
                    EnableOther = table.Column<bool>(type: "bit", nullable: false),
                    Toggle = table.Column<bool>(type: "bit", nullable: false),
                    Inline = table.Column<bool>(type: "bit", nullable: false),
                    Class = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Style = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddedBy = table.Column<int>(type: "int", nullable: false),
                    AddedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormsFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormsFields_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormsFields_Users_AddedBy",
                        column: x => x.AddedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "FormsEntriesFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntryId = table.Column<int>(type: "int", nullable: false),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormsEntriesFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormsEntriesFields_FormsEntries_EntryId",
                        column: x => x.EntryId,
                        principalTable: "FormsEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormsEntriesFields_FormsFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "FormsFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "FormsFieldsOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ArValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ArLabel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Selected = table.Column<bool>(type: "bit", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    AddedBy = table.Column<int>(type: "int", nullable: false),
                    AddedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormsFieldsOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormsFieldsOptions_FormsFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "FormsFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormsFieldsOptions_Users_AddedBy",
                        column: x => x.AddedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_FormId",
                table: "Projects",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_AddedBy",
                table: "Forms",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_LangId",
                table: "Forms",
                column: "LangId");

            migrationBuilder.CreateIndex(
                name: "IX_FormsEntries_FormId",
                table: "FormsEntries",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormsEntries_JobId",
                table: "FormsEntries",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_FormsEntries_MemberId",
                table: "FormsEntries",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_FormsEntriesFields_EntryId",
                table: "FormsEntriesFields",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_FormsEntriesFields_FieldId",
                table: "FormsEntriesFields",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FormsFields_AddedBy",
                table: "FormsFields",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FormsFields_FormId",
                table: "FormsFields",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormsFieldsOptions_AddedBy",
                table: "FormsFieldsOptions",
                column: "AddedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FormsFieldsOptions_FieldId",
                table: "FormsFieldsOptions",
                column: "FieldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Forms_FormId",
                table: "Projects",
                column: "FormId",
                principalTable: "Forms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Forms_FormId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "FormsEntriesFields");

            migrationBuilder.DropTable(
                name: "FormsFieldsOptions");

            migrationBuilder.DropTable(
                name: "FormsFieldsTypes");

            migrationBuilder.DropTable(
                name: "FormsEntries");

            migrationBuilder.DropTable(
                name: "FormsFields");

            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DropIndex(
                name: "IX_Projects_FormId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "FormId",
                table: "Projects");
        }
    }
}
