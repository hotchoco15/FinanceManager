using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    public class ExpenseResponse
    {
        public Guid ExpenseID { get; set; }

        public DateTime? DateOfExpense { get; set; }

        public string? ExpenseName { get; set; }

        public string? ExpenseType { get; set; }

        public double? ExpenseAmount { get; set; }

        public string? ExpenseRemark { get; set; }


        public ExpenseUpdateRequest ToExpenseUpdateRequest()
        {
            return new ExpenseUpdateRequest()
            {
				ExpenseID = ExpenseID,
				DateOfExpense = DateOfExpense,
				ExpenseName = ExpenseName,
				ExpenseType = (ExpenseOptions)Enum.Parse(typeof(ExpenseOptions), ExpenseType, true),
				ExpenseAmount = ExpenseAmount,
				ExpenseRemark = ExpenseRemark
			};
        }

    }

    public static class ExpenseExtensions
	{
        public static ExpenseResponse ToExpenseResponse(this Expense expense) 
        {
            return new ExpenseResponse()
            {
				ExpenseID = expense.ExpenseID,
				DateOfExpense = expense.DateOfExpense,
				ExpenseName = expense.ExpenseName,
				ExpenseType = expense.ExpenseType,
				ExpenseAmount = expense.ExpenseAmount,
				ExpenseRemark = expense.ExpenseRemark
			};
        }

		public static string GetDisplayNameofExpense(this string value)
		{
			if (value == "Housing")
            {
                return "주거비";
            }
            else if (value == "Transportation")
            {
                return "교통비";
			}
			else if (value == "Grocery")
			{
				return "장보기";
			}
			else if (value == "Food")
			{
				return "외식비";
			}
			else if (value == "Shopping")
			{
				return "쇼핑";
			}
			else if (value == "InsuranceFee")
			{
				return "보험료";
			}
			else if (value == "Other")
			{
				return "기타";
			}
			else 
            { 
                return "없음"; 
            }
		}
	}
}
