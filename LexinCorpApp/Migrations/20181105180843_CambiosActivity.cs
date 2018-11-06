using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class CambiosActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVisibleForActivities",
                table: "RetainerSubscriptions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReadOnly",
                table: "Expenses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisibleForActivities",
                table: "BillableRetainers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVisibleForActivities",
                table: "RetainerSubscriptions");

            migrationBuilder.DropColumn(
                name: "IsReadOnly",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "IsVisibleForActivities",
                table: "BillableRetainers");
        }
    }
}
