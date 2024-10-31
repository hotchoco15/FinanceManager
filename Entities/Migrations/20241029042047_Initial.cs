using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    ExpenseID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateOfExpense = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpenseName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    ExpenseType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    ExpenseAmount = table.Column<double>(type: "float", nullable: true),
                    ExpenseRemark = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.ExpenseID);
                });

            migrationBuilder.CreateTable(
                name: "Incomes",
                columns: table => new
                {
                    IncomeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateOfIncome = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IncomeName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    IncomeType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    IncomeAmount = table.Column<double>(type: "float", nullable: true),
                    IncomeRemark = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incomes", x => x.IncomeID);
                });

            migrationBuilder.InsertData(
                table: "Expenses",
                columns: new[] { "ExpenseID", "DateOfExpense", "ExpenseAmount", "ExpenseName", "ExpenseRemark", "ExpenseType" },
                values: new object[,]
                {
                    { new Guid("10251e15-5615-4c4f-87ce-69c0ea504afe"), new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 3500.0, "카페", "", "Food" },
                    { new Guid("2a5ef37f-9255-4f60-9c89-a0a8b6aea114"), new DateTime(2024, 10, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 32000.0, "보험료", "", "InsuranceFee" },
                    { new Guid("75c4cd52-53f7-4416-aa60-c17ab75a8689"), new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 9000.0, "점심", "", "Food" },
                    { new Guid("76a87f9b-49e2-4f85-a2a1-6988615852c8"), new DateTime(2024, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 65000.0, "교통비", "", "Transportation" },
                    { new Guid("ad739691-ff88-4e3e-b698-83b9072a24ef"), new DateTime(2024, 10, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 75000.0, "옷", "", "Shopping" }
                });

            migrationBuilder.InsertData(
                table: "Incomes",
                columns: new[] { "IncomeID", "DateOfIncome", "IncomeAmount", "IncomeName", "IncomeRemark", "IncomeType" },
                values: new object[,]
                {
                    { new Guid("16800ece-bae7-410b-9d20-fe4265951695"), new DateTime(2024, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1000000.0, "월급", "", "MainIncome" },
                    { new Guid("36feeb02-a3be-464f-a0b3-f2ece81f7137"), new DateTime(2024, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 12500.0, "앱테크", "", "ExtraIncome" },
                    { new Guid("3fad9624-8bb7-43b4-8481-4b189cca54a4"), new DateTime(2024, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 50000.0, "로또", "", "ExtraIncome" },
                    { new Guid("a5501da1-bb27-4bcb-9e85-dda63ab9bbcf"), new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 15000.0, "포인트환급", "", "ExtraIncome" },
                    { new Guid("b80e20c8-dc39-4330-8242-671cb946f463"), new DateTime(2024, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1000000.0, "월급", "", "MainIncome" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "Incomes");
        }
    }
}
