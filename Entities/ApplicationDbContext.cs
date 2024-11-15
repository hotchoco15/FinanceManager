using System;
using System.Collections.Generic;
using Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
	{
		public ApplicationDbContext(DbContextOptions options) : base(options){}
		public DbSet<Income> Incomes { get; set; }
		public DbSet<Expense> Expenses { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Income>().ToTable("Incomes");
			modelBuilder.Entity<Expense>().ToTable("Expenses");

			//수입 데이터 추가
			//string incomesJson = System.IO.File.ReadAllText("incomes.json");
			//List<Income> incomes =
			//System.Text.Json.JsonSerializer.Deserialize<List<Income>>(incomesJson);

			//foreach(Income income in incomes)
			//{
			//	modelBuilder.Entity<Income>().HasData(income);
			//}

			//지출 데이터 추가
			//string expensesJson = System.IO.File.ReadAllText("expenses.json");
			//List<Expense> expenses =
			//System.Text.Json.JsonSerializer.Deserialize<List<Expense>>(expensesJson);

			//foreach (Expense expense in expenses)
			//{
			//	modelBuilder.Entity<Expense>().HasData(expense);
			//}

		}
	}
}
