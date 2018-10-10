using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class PackageIsBilled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBilled",
                table: "Packages",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBilled",
                table: "Packages");
        }
    }
}
