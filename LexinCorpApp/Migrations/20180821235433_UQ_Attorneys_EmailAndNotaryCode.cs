using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class UQ_Attorneys_EmailAndNotaryCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SpanishDescription",
                table: "Expenses",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EnglishDescription",
                table: "Expenses",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NotaryCode",
                table: "Attorneys",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Attorneys",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_Attorneys_Email",
                table: "Attorneys",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attorneys_NotaryCode",
                table: "Attorneys",
                column: "NotaryCode",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attorneys_Email",
                table: "Attorneys");

            migrationBuilder.DropIndex(
                name: "IX_Attorneys_NotaryCode",
                table: "Attorneys");

            migrationBuilder.AlterColumn<string>(
                name: "SpanishDescription",
                table: "Expenses",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "EnglishDescription",
                table: "Expenses",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "NotaryCode",
                table: "Attorneys",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Attorneys",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
