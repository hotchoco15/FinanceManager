using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    /// <summary>
    /// 지출 도메인 모델 클래스
    /// </summary>
    public class Expense
    {
	[Key]
	public Guid ExpenseID { get; set; }

        public DateTime? DateOfExpense { get; set; }

	[StringLength(40)]
	public string? ExpenseName { get; set; }

	[StringLength(40)]
	public string? ExpenseType { get; set; }


        public double? ExpenseAmount { get; set; }

	[StringLength(40)]
	public string? ExpenseRemark { get; set; }
    }
}
