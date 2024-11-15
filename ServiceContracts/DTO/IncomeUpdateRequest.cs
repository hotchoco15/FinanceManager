using Entities;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ServiceContracts.DTO
{
	public class IncomeUpdateRequest
	{
		public Guid UserID { get; set; }

		public Guid IncomeID { get; set; }
	
		[Required(ErrorMessage = "이름은 필수항목입니다.")]
		public string? IncomeName { get; set; }
	
		[Required(ErrorMessage = "날짜는 필수항목입니다.")]
		public DateTime? DateOfIncome { get; set; }
	
		[Required(ErrorMessage = "항목은 필수항목입니다.")]
		public IncomeOptions? IncomeType { get; set; }
	
		[Required(ErrorMessage = "금액은 필수항목입니다. 숫자로 입력해주세요.")]
		public double? IncomeAmount { get; set; }
	
		public string? IncomeRemark { get; set; }
	
		public Income ToIncome()
		{
			return new Income
			{
				UserID = UserID,
				IncomeName = IncomeName,
				DateOfIncome = DateOfIncome,
				IncomeType = IncomeType.ToString(),
				IncomeAmount = IncomeAmount,
				IncomeRemark = IncomeRemark
			};
		}
	}
}

