using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Playtesters.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalHoursPlayedProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalHoursPlayed",
                table: "Tester",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalHoursPlayed",
                table: "Tester");
        }
    }
}
