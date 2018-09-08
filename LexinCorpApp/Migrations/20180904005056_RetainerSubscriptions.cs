using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class RetainerSubscriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Retainers",
                newName: "SpanishName");

            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "Retainers",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldNullable: true,
                oldDefaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "EnglishName",
                table: "Retainers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RetainerSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(nullable: false),
                    RetainerId = table.Column<int>(nullable: false),
                    AgreedFee = table.Column<decimal>(nullable: false),
                    AgreedHours = table.Column<decimal>(nullable: false),
                    AdditionalFeePerHour = table.Column<decimal>(nullable: false),
                    CreatorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RetainerSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RetainerSubscriptions_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RetainerSubscriptions_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RetainerSubscriptions_Retainers_RetainerId",
                        column: x => x.RetainerId,
                        principalTable: "Retainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RetainerSubscriptions_ClientId",
                table: "RetainerSubscriptions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_RetainerSubscriptions_CreatorId",
                table: "RetainerSubscriptions",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_RetainerSubscriptions_RetainerId",
                table: "RetainerSubscriptions",
                column: "RetainerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RetainerSubscriptions");

            migrationBuilder.DropColumn(
                name: "EnglishName",
                table: "Retainers");

            migrationBuilder.RenameColumn(
                name: "SpanishName",
                table: "Retainers",
                newName: "Name");

            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "Retainers",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldDefaultValue: true);
        }
    }
}
