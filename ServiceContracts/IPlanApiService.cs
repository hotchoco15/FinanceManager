using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
	/// <summary>
	/// 목표정하기에 대한 비지니스 로직입니다
	/// </summary>
	public interface IPlanApiService
	{
		/// <summary>
		/// 목표를 보여줍니다
		/// </summary>
		/// <returns></returns>
		Task<PagingResponse<PlanDTO>> GetPlans(int page, int pageSize);


		/// <summary>
		/// 목표를 추가합니다
		/// </summary>
		/// <returns></returns>
		Task AddPlan(PlanDTO planDto);

	}
}
