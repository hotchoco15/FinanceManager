using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
	/// <summary>
	/// 목표정하기 도메인 모델 클래스
	/// </summary>
	public class Plan
	{
		[Key]
		public Guid PlanID { get; set; }

		public string UserName { get; set; }

		public string PlanName { get; set; }

		public DateTime TargetDate { get; set; }

		public double TargetAmount { get; set; }

		public DateTime CurrentDate { get; set; } = DateTime.Now;

		public double Amount { get; set; }

	}
}

