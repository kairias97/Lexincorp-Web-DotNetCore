using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class VacationsMovementCreation2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttorneyId",
                table: "VacationsMovement",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VacationsMovement_AttorneyId",
                table: "VacationsMovement",
                column: "AttorneyId");

            migrationBuilder.AddForeignKey(
                name: "FK_VacationsMovement_Attorneys_AttorneyId",
                table: "VacationsMovement",
                column: "AttorneyId",
                principalTable: "Attorneys",
                principalColumn: "AttorneyId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VacationsMovement_Attorneys_AttorneyId",
                table: "VacationsMovement");

            migrationBuilder.DropIndex(
                name: "IX_VacationsMovement_AttorneyId",
                table: "VacationsMovement");

            migrationBuilder.DropColumn(
                name: "AttorneyId",
                table: "VacationsMovement");
        }
    }
}
