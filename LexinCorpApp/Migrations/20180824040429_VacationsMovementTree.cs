using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class VacationsMovementTree : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VacationsMovement_Attorneys_AttorneyId",
                table: "VacationsMovement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VacationsMovement",
                table: "VacationsMovement");

            migrationBuilder.RenameTable(
                name: "VacationsMovement",
                newName: "VacationsMovements");

            migrationBuilder.RenameIndex(
                name: "IX_VacationsMovement_AttorneyId",
                table: "VacationsMovements",
                newName: "IX_VacationsMovements_AttorneyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VacationsMovements",
                table: "VacationsMovements",
                column: "VacationsMovementId");

            migrationBuilder.AddForeignKey(
                name: "FK_VacationsMovements_Attorneys_AttorneyId",
                table: "VacationsMovements",
                column: "AttorneyId",
                principalTable: "Attorneys",
                principalColumn: "AttorneyId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VacationsMovements_Attorneys_AttorneyId",
                table: "VacationsMovements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VacationsMovements",
                table: "VacationsMovements");

            migrationBuilder.RenameTable(
                name: "VacationsMovements",
                newName: "VacationsMovement");

            migrationBuilder.RenameIndex(
                name: "IX_VacationsMovements_AttorneyId",
                table: "VacationsMovement",
                newName: "IX_VacationsMovement_AttorneyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VacationsMovement",
                table: "VacationsMovement",
                column: "VacationsMovementId");

            migrationBuilder.AddForeignKey(
                name: "FK_VacationsMovement_Attorneys_AttorneyId",
                table: "VacationsMovement",
                column: "AttorneyId",
                principalTable: "Attorneys",
                principalColumn: "AttorneyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
