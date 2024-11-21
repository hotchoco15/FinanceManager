using Asp.Versioning;
using AutoMapper;
using Entities;
using Entities.IdentityEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace FinanceManager.WebAPI.Controllers.v1
{
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiController]
	[ApiVersion("1.0")]

	public class PlanController : Controller
	{
		private readonly ApplicationDbContext _db;

		private readonly IMapper _mapper;

		public PlanController(ApplicationDbContext db, IMapper mapper)
		{
			_db = db;
			_mapper = mapper;
		}

		[MapToApiVersion("1.0")]
		[AllowAnonymous]
		[HttpPost]
		public async Task<ActionResult> AddPlan(PlanDTO planDto)
		{
			if (ModelState.IsValid)
			{
				var plan = new Plan
				{
					UserName = planDto.UserName,
					PlanName = planDto.PlanName,
					TargetDate = planDto.TargetDate,
					TargetAmount = planDto.TargetAmount,
					CurrentDate = planDto.CurrentDate,
					Amount = planDto.Amount
				};


				_db.Plans.Add(plan);
				await _db.SaveChangesAsync();

				return CreatedAtAction("GetPlan", new { id = plan.PlanID }, planDto);
			}

			return BadRequest(ModelState);
		}

		[MapToApiVersion("1.0")]
		[AllowAnonymous]
		[HttpGet]
		public async Task<ActionResult<PagingResponse<PlanDTO>>> GetPlan(int page, int pageSize)
		{
			var skipResult = (page - 1) * pageSize;
			
			var pagenatedPlans = await _db.Plans.OrderByDescending(temp => temp.CurrentDate).Skip(skipResult).Take(pageSize).ToListAsync();

			int totalPage = 0;
			int totalCount = _db.Plans.Count();

			if (totalCount % pageSize == 0)
			{
				totalPage = totalCount / pageSize;
			}
			else
			{
				totalPage = totalCount / pageSize + 1;
			}

			var planDto = _mapper.Map<IEnumerable<PlanDTO>>(pagenatedPlans);

			var response = new PagingResponse<PlanDTO>();

			response.Data = planDto;
			response.TotalPage = totalPage;


			return Ok(response);

		}
	}
}

