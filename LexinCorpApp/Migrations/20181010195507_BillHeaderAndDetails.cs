using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class BillHeaderAndDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(nullable: false),
                    BillDate = table.Column<DateTime>(nullable: false),
                    BillDiscountType = table.Column<int>(nullable: true),
                    BillDiscount = table.Column<decimal>(nullable: true),
                    BillMonth = table.Column<int>(nullable: false),
                    BillYear = table.Column<int>(nullable: false),
                    BillName = table.Column<string>(nullable: true),
                    BillSubtotal = table.Column<decimal>(nullable: false),
                    Taxes = table.Column<decimal>(nullable: false),
                    Total = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillHeaders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BillHeaderId = table.Column<int>(nullable: false),
                    ActivityId = table.Column<int>(nullable: true),
                    BillDetailType = table.Column<int>(nullable: false),
                    FixedAmount = table.Column<decimal>(nullable: true),
                    UnitRate = table.Column<decimal>(nullable: true),
                    Quantity = table.Column<decimal>(nullable: true),
                    Subtotal = table.Column<decimal>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    TaxesAmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillDetails_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BillDetails_BillHeaders_BillHeaderId",
                        column: x => x.BillHeaderId,
                        principalTable: "BillHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillDetails_ActivityId",
                table: "BillDetails",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_BillDetails_BillHeaderId",
                table: "BillDetails",
                column: "BillHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_BillHeaders_ClientId",
                table: "BillHeaders",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillDetails");

            migrationBuilder.DropTable(
                name: "BillHeaders");
        }
    }
}
