using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceManager.Migrations
{
    /// <inheritdoc />
    public partial class AddUserID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.AlterColumn<Guid>(
			name: "UserID",
			table: "Incomes",
			oldClrType: typeof(string),
			oldType: "nvarchar(max)");

			migrationBuilder.AlterColumn<Guid>(
			name: "UserID",
			table: "Expenses",
			oldClrType: typeof(string),
			oldType: "nvarchar(max)");
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.DropColumn(
			name: "UserID",
			table: "Incomes");

			migrationBuilder.DropColumn(
				name: "UserID",
				table: "Expenses");
		}
    }
}
