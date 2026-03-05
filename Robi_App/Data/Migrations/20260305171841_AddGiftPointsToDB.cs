using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Robi_App.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGiftPointsToDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "Gifts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "Gifts");
        }
    }
}
