using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Robi_App.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToInvices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Invoices");
        }
    }
}
