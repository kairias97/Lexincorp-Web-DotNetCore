using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class VacationsMovementCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "VacationCount",
                table: "Attorneys",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "VacationsMovement",
                columns: table => new
                {
                    VacationsMovementId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Reason = table.Column<string>(nullable: false),
                    Quantity = table.Column<decimal>(nullable: false),
                    MovementType = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacationsMovement", x => x.VacationsMovementId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VacationsMovement");

            migrationBuilder.DropColumn(
                name: "VacationCount",
                table: "Attorneys");
        }
    }
}
