using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
	public class PlanDTO
	{
		[Required]
		public Guid PlanID { get; set; }

		[Required(ErrorMessage = "이름은 필수입력입니다")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "목표명은 필수입력입니다")]
		public string PlanName { get; set; }

		[Required(ErrorMessage = "목표일은 필수입력입니다")]
		public DateTime TargetDate { get; set; }

		[Required(ErrorMessage = "목표 금액은 필수입력입니다")]
		public double TargetAmount { get; set; }

		[Required]
		public DateTime CurrentDate { get; set; } = DateTime.Now;

		[Required(ErrorMessage = "추가할 금액은 필수입력입니다")]
		public double Amount { get; set; }

	}
}
