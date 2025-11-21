using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Playtesters.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tester",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT COLLATE NOCASE", nullable: false),
                    AccessKey = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tester", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccessValidationHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TesterId = table.Column<int>(type: "INTEGER", nullable: false),
                    CheckedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessValidationHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccessValidationHistory_Tester_TesterId",
                        column: x => x.TesterId,
                        principalTable: "Tester",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessValidationHistory_CheckedAt",
                table: "AccessValidationHistory",
                column: "CheckedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AccessValidationHistory_IpAddress",
                table: "AccessValidationHistory",
                column: "IpAddress");

            migrationBuilder.CreateIndex(
                name: "IX_AccessValidationHistory_TesterId",
                table: "AccessValidationHistory",
                column: "TesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Tester_AccessKey",
                table: "Tester",
                column: "AccessKey",
                unique: true);

            migrationBuilder.Sql(
                @"CREATE UNIQUE INDEX IF NOT EXISTS IX_Tester_Name 
                ON Tester(Name COLLATE NOCASE);");

            // migrationBuilder.CreateIndex(
            //    name: "IX_Tester_Name",
            //    table: "Tester",
            //    column: "Name",
            //    unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessValidationHistory");

            migrationBuilder.DropTable(
                name: "Tester");
        }
    }
}
