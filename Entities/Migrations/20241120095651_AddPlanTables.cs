using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    PlanID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlanName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TargetAmount = table.Column<double>(type: "float", nullable: false),
                    CurrentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.PlanID);
                });

            migrationBuilder.InsertData(
                table: "Plans",
                columns: new[] { "PlanID", "Amount", "CurrentDate", "PlanName", "TargetAmount", "TargetDate", "UserName" },
                values: new object[] { new Guid("bd318ece-932a-4b52-bccc-a954c3e4319a"), 50000.0, new DateTime(2024, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "이사", 10000000.0, new DateTime(2028, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "qwer" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    GoalID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    CurrentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GoalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetAmount = table.Column<double>(type: "float", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.GoalID);
                });

            migrationBuilder.InsertData(
                table: "Goals",
                columns: new[] { "GoalID", "Amount", "CurrentDate", "GoalName", "TargetAmount", "TargetDate", "UserID" },
                values: new object[,]
                {
                    { new Guid("36af092b-c3b9-48a0-884a-0006d41da328"), 10000.0, new DateTime(2024, 11, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "차 사기", 30000000.0, new DateTime(2029, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("a7df3cd1-7850-485a-90fc-08dcfce7c1d9") },
                    { new Guid("f220c2df-b7f7-4ec3-a8bf-30accc9f542e"), 10000.0, new DateTime(2024, 11, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "유럽여행", 3500000.0, new DateTime(2026, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("a7df3cd1-7850-485a-90fc-08dcfce7c1d9") }
                });
        }
    }
}
