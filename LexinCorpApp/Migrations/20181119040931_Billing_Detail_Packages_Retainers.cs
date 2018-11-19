using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class Billing_Detail_Packages_Retainers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "BillDetails",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Time",
                table: "BillDetails",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "BillDetails",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillableRetainerId",
                table: "BillDetails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PackageId",
                table: "BillDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillDetails_BillableRetainerId",
                table: "BillDetails",
                column: "BillableRetainerId");

            migrationBuilder.CreateIndex(
                name: "IX_BillDetails_PackageId",
                table: "BillDetails",
                column: "PackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_BillDetails_BillableRetainers_BillableRetainerId",
                table: "BillDetails",
                column: "BillableRetainerId",
                principalTable: "BillableRetainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BillDetails_Packages_PackageId",
                table: "BillDetails",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillDetails_BillableRetainers_BillableRetainerId",
                table: "BillDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_BillDetails_Packages_PackageId",
                table: "BillDetails");

            migrationBuilder.DropIndex(
                name: "IX_BillDetails_BillableRetainerId",
                table: "BillDetails");

            migrationBuilder.DropIndex(
                name: "IX_BillDetails_PackageId",
                table: "BillDetails");

            migrationBuilder.DropColumn(
                name: "BillableRetainerId",
                table: "BillDetails");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "BillDetails");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "BillDetails",
                nullable: true,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Time",
                table: "BillDetails",
                nullable: true,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "BillDetails",
                nullable: true,
                oldClrType: typeof(decimal));
        }
    }
}
