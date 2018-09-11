using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class BillableRetainers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillableRetainers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    BillingDescription = table.Column<string>(nullable: true),
                    ClientId = table.Column<int>(nullable: false),
                    RetainerId = table.Column<int>(nullable: false),
                    AgreedFee = table.Column<decimal>(nullable: false),
                    AgreedHours = table.Column<decimal>(nullable: false),
                    AdditionalFeePerHour = table.Column<decimal>(nullable: false),
                    CreatorId = table.Column<int>(nullable: false),
                    IsBilled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillableRetainers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillableRetainers_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillableRetainers_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillableRetainers_Retainers_RetainerId",
                        column: x => x.RetainerId,
                        principalTable: "Retainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BillableRetainers_ClientId",
                table: "BillableRetainers",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_BillableRetainers_CreatorId",
                table: "BillableRetainers",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_BillableRetainers_RetainerId",
                table: "BillableRetainers",
                column: "RetainerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillableRetainers");
        }
    }
}
