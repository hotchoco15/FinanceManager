using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    public class IncomeResponse
    {
        public Guid IncomeID { get; set; }

        public DateTime? DateOfIncome { get; set; }

        public string? IncomeName { get; set; }

        public string? IncomeType { get; set; }

        public double? IncomeAmount { get; set; }

        public string? IncomeRemark { get; set; }


        public IncomeUpdateRequest ToIncomeUpdateRequest()
        {
            return new IncomeUpdateRequest()
            { 
                IncomeID = IncomeID,
                DateOfIncome = DateOfIncome,
                IncomeName = IncomeName,
                IncomeType = (IncomeOptions)Enum.Parse(typeof(IncomeOptions), IncomeType, true),
                IncomeAmount = IncomeAmount,
                IncomeRemark = IncomeRemark
            };
        }

    }

    public static class IncomeExtensions
    {
        public static IncomeResponse ToIncomeResponse(this Income income) 
        {
            return new IncomeResponse()
            {
                IncomeID = income.IncomeID,
                DateOfIncome = income.DateOfIncome,
                IncomeName = income.IncomeName,
                IncomeType = income.IncomeType,
                IncomeAmount = income.IncomeAmount,
                IncomeRemark = income.IncomeRemark 
            };
        }

		public static string GetDisplayName(this string value)
		{
			if (value == "MainIncome")
            {
                return "주수입";
            }
            else if (value == "ExtraIncome")
            {
                return "부수입";
			}
            else 
            { 
                return "없음"; 
            }
		}
	}
}
