using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class KeyUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ReferredBy",
                table: "Clients",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Clients",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<decimal>(
                name: "FixedCostPerHour",
                table: "Clients",
                nullable: true,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<string>(
                name: "ContactPhone",
                table: "Clients",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ContactJobName",
                table: "Clients",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ContactEmail",
                table: "Clients",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Clients",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Users",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReferredBy",
                table: "Clients",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Clients",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FixedCostPerHour",
                table: "Clients",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContactPhone",
                table: "Clients",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContactJobName",
                table: "Clients",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContactEmail",
                table: "Clients",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Clients",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
