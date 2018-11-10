using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class AttorneyPermissions_AndAlias : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alias",
                table: "Attorneys",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "CanAdminDeposits",
                table: "Attorneys",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanBill",
                table: "Attorneys",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanPreBill",
                table: "Attorneys",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanReviewBillDetail",
                table: "Attorneys",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alias",
                table: "Attorneys");

            migrationBuilder.DropColumn(
                name: "CanAdminDeposits",
                table: "Attorneys");

            migrationBuilder.DropColumn(
                name: "CanBill",
                table: "Attorneys");

            migrationBuilder.DropColumn(
                name: "CanPreBill",
                table: "Attorneys");

            migrationBuilder.DropColumn(
                name: "CanReviewBillDetail",
                table: "Attorneys");
        }
    }
}
