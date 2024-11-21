using AutoMapper;
using Entities;
using ServiceContracts.DTO;

namespace FinanceManager.WebAPI.Mappings
{
	public class AutoMapperProfiles : Profile
	{
		public AutoMapperProfiles() 
		{
			CreateMap<Plan, PlanDTO>();
		}
	}
}
