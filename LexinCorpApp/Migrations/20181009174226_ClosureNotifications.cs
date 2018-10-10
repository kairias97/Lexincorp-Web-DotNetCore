using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class ClosureNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClosureNotifications",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    PackageId = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    WasClosed = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClosureNotifications", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ClosureNotifications_Packages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "Packages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClosureNotificationId = table.Column<int>(nullable: false),
                    TargetUserId = table.Column<int>(nullable: false),
                    WasAffirmative = table.Column<bool>(nullable: true),
                    IsAnswered = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationAnswers_ClosureNotifications_ClosureNotificationId",
                        column: x => x.ClosureNotificationId,
                        principalTable: "ClosureNotifications",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationAnswers_Users_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClosureNotifications_PackageId",
                table: "ClosureNotifications",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationAnswers_ClosureNotificationId",
                table: "NotificationAnswers",
                column: "ClosureNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationAnswers_TargetUserId",
                table: "NotificationAnswers",
                column: "TargetUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationAnswers");

            migrationBuilder.DropTable(
                name: "ClosureNotifications");
        }
    }
}
