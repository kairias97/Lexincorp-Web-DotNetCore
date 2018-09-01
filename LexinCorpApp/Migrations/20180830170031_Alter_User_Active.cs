using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class Alter_User_Active : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Users_CreatorUserId",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_CreatorUserId",
                table: "Packages");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "AttorneyId",
                table: "Attorneys",
                newName: "Id");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Users",
                nullable: true,
                defaultValue: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Users_CreatorId",
                table: "Packages");

            migrationBuilder.DropIndex(
                name: "IX_Packages_CreatorId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Packages");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Attorneys",
                newName: "AttorneyId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_CreatorUserId",
                table: "Packages",
                column: "CreatorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Users_CreatorUserId",
                table: "Packages",
                column: "CreatorUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
