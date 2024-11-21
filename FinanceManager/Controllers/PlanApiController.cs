using Entities;
using Entities.IdentityEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace FinanceManager.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
	public class PlanApiController : Controller
	{
		private readonly IPlanApiService _planApiService;


		public PlanApiController(IPlanApiService planApiService)
		{
			_planApiService = planApiService;
		}

		[Route("[action]")]
		[HttpGet]
		public async Task<IActionResult> Index(int page = 1, int pageSize = 2)
		{
			var plans = await _planApiService.GetPlans(page, pageSize);

			ViewBag.CurrentPage = page;
			ViewBag.TotalPage = plans.TotalPage;

			var result = plans.Data.ToList();

			return View(result);
		}


		[Route("[action]")]
		[HttpGet]
		public IActionResult AddPlan()
		{
			var model = new PlanDTO
			{
				CurrentDate = DateTime.Now
			};

			return View(model);
		}


		[Route("[action]")]
		[HttpPost]
		public async Task<IActionResult> AddPlan(PlanDTO planDto)
		{
			await _planApiService.AddPlan(planDto);	
			return RedirectToAction("Index");
		}
	}
}
