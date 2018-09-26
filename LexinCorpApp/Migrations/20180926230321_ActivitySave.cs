using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class ActivitySave : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    HoursWorked = table.Column<decimal>(nullable: false),
                    ClientId = table.Column<int>(nullable: false),
                    RealizationDate = table.Column<DateTime>(nullable: false),
                    ServiceId = table.Column<int>(nullable: false),
                    Subtotal = table.Column<decimal>(nullable: false),
                    TaxesAmount = table.Column<decimal>(nullable: false),
                    PayTaxes = table.Column<bool>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    IsBilled = table.Column<bool>(nullable: false),
                    BillableRate = table.Column<decimal>(nullable: false),
                    BillableQuantity = table.Column<decimal>(nullable: false),
                    ActivityType = table.Column<int>(nullable: false),
                    BillableRetainerId = table.Column<int>(nullable: true),
                    PackageId = table.Column<int>(nullable: true),
                    ItemId = table.Column<int>(nullable: true),
                    CreatorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_BillableRetainers_BillableRetainerId",
                        column: x => x.BillableRetainerId,
                        principalTable: "BillableRetainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activities_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activities_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Activities_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activities_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Activities_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityExpenses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExpenseId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    UnitAmount = table.Column<decimal>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    RealizationDate = table.Column<DateTime>(nullable: false),
                    ActivityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityExpenses_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityExpenses_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_BillableRetainerId",
                table: "Activities",
                column: "BillableRetainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ClientId",
                table: "Activities",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_CreatorId",
                table: "Activities",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ItemId",
                table: "Activities",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_PackageId",
                table: "Activities",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ServiceId",
                table: "Activities",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityExpenses_ActivityId",
                table: "ActivityExpenses",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityExpenses_ExpenseId",
                table: "ActivityExpenses",
                column: "ExpenseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityExpenses");

            migrationBuilder.DropTable(
                name: "Activities");
        }
    }
}
