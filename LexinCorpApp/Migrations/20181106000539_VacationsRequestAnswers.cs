using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LexincorpApp.Migrations
{
    public partial class VacationsRequestAnswers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VacationsRequestId",
                table: "VacationsRequests",
                newName: "Id");

            migrationBuilder.AddColumn<bool>(
                name: "IsApplied",
                table: "VacationsRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "VacationsRequests",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VacationsMonthlyCredits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Month = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacationsMonthlyCredits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VacationsRequestAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsApproved = table.Column<bool>(nullable: true),
                    RequestId = table.Column<int>(nullable: false),
                    ApproverId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacationsRequestAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VacationsRequestAnswers_Users_ApproverId",
                        column: x => x.ApproverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacationsRequestAnswers_VacationsRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "VacationsRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VacationsRequestAnswers_ApproverId",
                table: "VacationsRequestAnswers",
                column: "ApproverId");

            migrationBuilder.CreateIndex(
                name: "IX_VacationsRequestAnswers_RequestId",
                table: "VacationsRequestAnswers",
                column: "RequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VacationsMonthlyCredits");

            migrationBuilder.DropTable(
                name: "VacationsRequestAnswers");

            migrationBuilder.DropColumn(
                name: "IsApplied",
                table: "VacationsRequests");

            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "VacationsRequests");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "VacationsRequests",
                newName: "VacationsRequestId");
        }
    }
}
