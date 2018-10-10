using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class AlterPKName_ClosureNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "ClosureNotifications",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ClosureNotifications",
                newName: "ID");
        }
    }
}
