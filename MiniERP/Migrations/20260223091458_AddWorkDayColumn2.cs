using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniERP.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkDayColumn2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkDay",
                table: "Attendance",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkDay",
                table: "Attendance");
        }
    }
}
