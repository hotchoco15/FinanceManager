using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("UPDATE Expenses SET UserID = '00000000-0000-0000-0000-000000000000' WHERE UserID IS NULL");
			migrationBuilder.Sql("UPDATE Incomes SET UserID = '00000000-0000-0000-0000-000000000000' WHERE UserID IS NULL");

			migrationBuilder.AlterColumn<Guid>(
                name: "UserID",
                table: "Incomes",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");


			migrationBuilder.AlterColumn<Guid>(
                name: "UserID",
                table: "Expenses",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");			
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Incomes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
        }
    }
}
