using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class BillHeaderCreatorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "BillHeaders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BillHeaders_CreatorId",
                table: "BillHeaders",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_BillHeaders_Users_CreatorId",
                table: "BillHeaders",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillHeaders_Users_CreatorId",
                table: "BillHeaders");

            migrationBuilder.DropIndex(
                name: "IX_BillHeaders_CreatorId",
                table: "BillHeaders");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "BillHeaders");
        }
    }
}
