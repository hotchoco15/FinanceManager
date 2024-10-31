using System.ComponentModel.DataAnnotations;

namespace Entities
{
	/// <summary>
	/// 수입 도메인 모델 클래스
	/// </summary>
	public class Income
    {
        [Key]
        public Guid IncomeID { get; set; }

        public DateTime? DateOfIncome {  get; set; }

        [StringLength(40)]
        public string? IncomeName { get; set;}

		[StringLength(40)]
		public string? IncomeType { get; set; }


        public double? IncomeAmount { get; set; }

		[StringLength(40)]
		public string? IncomeRemark { get; set; }
    }
}
