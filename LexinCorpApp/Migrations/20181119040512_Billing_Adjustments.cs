using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class Billing_Adjustments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillDiscount",
                table: "BillHeaders");

            migrationBuilder.DropColumn(
                name: "BillDiscountType",
                table: "BillHeaders");

            migrationBuilder.DropColumn(
                name: "BillMonth",
                table: "BillHeaders");

            migrationBuilder.DropColumn(
                name: "BillName",
                table: "BillHeaders");

            migrationBuilder.DropColumn(
                name: "BillYear",
                table: "BillHeaders");

            migrationBuilder.DropColumn(
                name: "TaxesAmount",
                table: "BillDetails");

            migrationBuilder.RenameColumn(
                name: "UnitRate",
                table: "BillDetails",
                newName: "UnitCost");

            migrationBuilder.RenameColumn(
                name: "FixedAmount",
                table: "BillDetails",
                newName: "Time");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalExpenses",
                table: "BillHeaders",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPayments",
                table: "BillHeaders",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Attorney",
                table: "BillDetails",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "BillDetails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "BillExpenses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BillHeaderId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Cost = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Subtotal = table.Column<decimal>(nullable: false),
                    Month = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    SpanishMonth = table.Column<string>(nullable: true),
                    EnglishMonth = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillExpenses_BillHeaders_BillHeaderId",
                        column: x => x.BillHeaderId,
                        principalTable: "BillHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillSummary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BillHeaderId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Time = table.Column<decimal>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    Total = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillSummary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillSummary_BillHeaders_BillHeaderId",
                        column: x => x.BillHeaderId,
                        principalTable: "BillHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillExpenses_BillHeaderId",
                table: "BillExpenses",
                column: "BillHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_BillSummary_BillHeaderId",
                table: "BillSummary",
                column: "BillHeaderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillExpenses");

            migrationBuilder.DropTable(
                name: "BillSummary");

            migrationBuilder.DropColumn(
                name: "TotalExpenses",
                table: "BillHeaders");

            migrationBuilder.DropColumn(
                name: "TotalPayments",
                table: "BillHeaders");

            migrationBuilder.DropColumn(
                name: "Attorney",
                table: "BillDetails");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "BillDetails");

            migrationBuilder.RenameColumn(
                name: "UnitCost",
                table: "BillDetails",
                newName: "UnitRate");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "BillDetails",
                newName: "FixedAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "BillDiscount",
                table: "BillHeaders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillDiscountType",
                table: "BillHeaders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillMonth",
                table: "BillHeaders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BillName",
                table: "BillHeaders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillYear",
                table: "BillHeaders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxesAmount",
                table: "BillDetails",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
