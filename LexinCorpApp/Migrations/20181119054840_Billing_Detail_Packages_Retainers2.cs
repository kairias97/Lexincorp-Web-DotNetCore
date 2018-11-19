using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class Billing_Detail_Packages_Retainers2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillExpenses");

            migrationBuilder.CreateTable(
                name: "BillExpense",
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
                    table.PrimaryKey("PK_BillExpense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillExpense_BillHeaders_BillHeaderId",
                        column: x => x.BillHeaderId,
                        principalTable: "BillHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillExpense_BillHeaderId",
                table: "BillExpense",
                column: "BillHeaderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillExpense");

            migrationBuilder.CreateTable(
                name: "BillExpenses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BillHeaderId = table.Column<int>(nullable: false),
                    Cost = table.Column<decimal>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    EnglishMonth = table.Column<string>(nullable: true),
                    Month = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    SpanishMonth = table.Column<string>(nullable: true),
                    Subtotal = table.Column<decimal>(nullable: false),
                    Year = table.Column<int>(nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_BillExpenses_BillHeaderId",
                table: "BillExpenses",
                column: "BillHeaderId");
        }
    }
}
