using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ServiceContracts.DTO
{
	public class ExpenseUpdateRequest
	{
		public Guid ExpenseID { get; set; }


		[Required(ErrorMessage = "이름은 필수항목입니다.")]
		public string? ExpenseName { get; set; }

		[Required(ErrorMessage = "날짜는 필수항목입니다.")]
		public DateTime? DateOfExpense { get; set; }

		[Required(ErrorMessage = "항목은 필수항목입니다.")]
		public ExpenseOptions? ExpenseType { get; set; }

		[Required(ErrorMessage = "금액은 필수항목입니다. 숫자로 입력해주세요.")]
		public double? ExpenseAmount { get; set; }

		public string? ExpenseRemark { get; set; }

		public Expense ToExpense()
		{
			return new Expense
			{
				ExpenseName = ExpenseName,
				DateOfExpense = DateOfExpense,
				ExpenseType = ExpenseType.ToString(),
				ExpenseAmount = ExpenseAmount,
				ExpenseRemark = ExpenseRemark
			};
		}
	}
}

