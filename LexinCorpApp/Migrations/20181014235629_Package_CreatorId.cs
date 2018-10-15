using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class Package_CreatorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Users_CreatorId",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_CreatorId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Packages");

            migrationBuilder.AddColumn<int>(
                name: "FK_Packages_Users_CreatorUserId",
                table: "Packages",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_FK_Packages_Users_CreatorUserId",
                table: "Packages",
                column: "FK_Packages_Users_CreatorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Users_FK_Packages_Users_CreatorUserId",
                table: "Packages",
                column: "FK_Packages_Users_CreatorUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Users_FK_Packages_Users_CreatorUserId",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_FK_Packages_Users_CreatorUserId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "FK_Packages_Users_CreatorUserId",
                table: "Packages");

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "Packages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_CreatorId",
                table: "Packages",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Users_CreatorId",
                table: "Packages",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
