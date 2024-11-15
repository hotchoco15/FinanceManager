using Entities;
using Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System.Collections.Generic;
using System.IO;

namespace FinanceManager.Controllers
{
    [Route("[controller]")]
    public class IncomeController : Controller
    {
        private readonly IIncomeService _incomeService;

		private readonly UserManager<ApplicationUser> _userManager;


		public IncomeController(IIncomeService incomeService, UserManager<ApplicationUser> userManager)
        {
            _incomeService = incomeService;

            _userManager = userManager;
        }

        [Route("[action]")]
        public async Task<IActionResult> Index(string? searchBy = null, string? searchString = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
			var userId = _userManager.GetUserId(User);

			//검색
			ViewBag.SearchType = new Dictionary<string, string>()
            {
                {"NotSelected", "선택안함"},
                {"MainIncome", "주수입"},
                {"ExtraIncome", "부수입"}
            };

            if (string.IsNullOrEmpty(searchBy))
            {
                List<IncomeResponse> incomes = await _incomeService.GetDefaultIncomes(searchBy, searchString, fromDate, toDate, userId);

                ViewBag.SearchBy = "00000";
                ViewBag.SearchString = searchString;


		        fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                toDate = DateTime.Today;

		        ViewBag.ToDate = toDate;
                ViewBag.FromDate = fromDate;


                if (incomes.Count > 0)
                {
                    ViewBag.Last = incomes.Last().IncomeID;
                    ViewBag.Sum = incomes.Sum(i => i.IncomeAmount);
                }

                ViewBag.InitialMessage = "*** 이번 달의 수입 내역입니다 ***";

                fromDate = null;
                toDate = null;

                return View(incomes);
            }

            else 
            {
		        ViewBag.SearchBy = searchBy;
		        ViewBag.SearchString = searchString;
                ViewBag.ToDate = toDate;
                ViewBag.FromDate = fromDate;


		        List<IncomeResponse> filteredIncomes = await _incomeService.GetSelectedIncomes(searchBy, searchString, fromDate, toDate, userId);


                if (filteredIncomes.Count > 0)
                { 
                    ViewBag.Last = filteredIncomes.Last().IncomeID;
                    ViewBag.Sum = filteredIncomes.Sum(i => i.IncomeAmount);	
		        }
                else
                {
                    ViewBag.Message = "empty";
		        }

		        return View(filteredIncomes);
	        }
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.IncomeOptions = new Dictionary<string, string>()
            {
                {"MainIncome", "주수입"},
                {"ExtraIncome", "부수입"}
            };


			var userId = _userManager.GetUserId(User);
			ViewBag.UserID = userId;

			return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Create(IncomeAddRequest incomeAddRequest)
        {
            if (!ModelState.IsValid) 
            {
		        ViewBag.IncomeOptions = new Dictionary<string, string>()
	            {
		            {"MainIncome", "주수입"},
		            {"ExtraIncome", "부수입"}
	            };

		        ViewBag.Errors = ModelState.Values.SelectMany(error => error.Errors).Select(e => e.ErrorMessage).ToList();
		        return View();
	        }


		    IncomeResponse incomeResponse = await _incomeService.AddIncome(incomeAddRequest);
		    return RedirectToAction("Index");
        }

	    [Route("[action]/{incomeID}")]
	    [HttpGet]
        public async Task<IActionResult> Update(Guid incomeID)
        {
			var userId = _userManager.GetUserId(User);
			Guid parsedUserId = new Guid(userId);

			IncomeResponse? incomeResponse = await _incomeService.GetIncomeByIncomeID(incomeID);
            if (incomeResponse == null)
            {
                return RedirectToAction("Index");
            }

			if (incomeResponse.UserID != parsedUserId)
			{
				return Unauthorized();
			}

			IncomeUpdateRequest incomeUpdateRequest = incomeResponse.ToIncomeUpdateRequest();

			ViewBag.IncomeOptions = new Dictionary<string, string>()
			{
				{"MainIncome", "주수입"},
				{"ExtraIncome", "부수입"}
			};


			return View(incomeUpdateRequest);
        }

	    [Route("[action]/{incomeID}")]
	    [HttpPost]
	    public async Task<IActionResult> Update(IncomeUpdateRequest incomeUpdateRequest)
        {
            IncomeResponse? incomeResponse = await _incomeService.GetIncomeByIncomeID(incomeUpdateRequest.IncomeID);

            if(incomeResponse == null) 
            { 
                return RedirectToAction("Index"); 
            }

            if(ModelState.IsValid) 
            {
               IncomeResponse updatedPerson = await _incomeService.UpdateIncome(incomeUpdateRequest);
               return RedirectToAction("Index");
            }
            else
            {
		        ViewBag.IncomeOptions = new Dictionary<string, string>()
		        {
			        {"MainIncome", "주수입"},
			        {"ExtraIncome", "부수입"}
		        };

		        ViewBag.Errors = ModelState.Values.SelectMany(error => error.Errors).Select(e => e.ErrorMessage).ToList();
		        return View(incomeResponse.ToIncomeUpdateRequest());
	        }
        }


        [HttpGet]
        [Route("[action]/{incomeID}")]
        public async Task<IActionResult> Delete(Guid incomeID) 
        {
			var userId = _userManager.GetUserId(User);
			Guid parsedUserId = new Guid(userId);

			IncomeResponse? incomeResponse = await _incomeService.GetIncomeByIncomeID(incomeID);

            if(incomeResponse == null) 
            {
                return RedirectToAction("Index");
            }

			if (incomeResponse.UserID != parsedUserId)
			{
				return Unauthorized();
			}

			return View(incomeResponse);
        }

        [HttpPost]
	    [Route("[action]/{incomeID}")]
        public async Task<IActionResult> Delete(IncomeUpdateRequest result)
        {
            IncomeResponse? incomeResponse = await _incomeService.GetIncomeByIncomeID(result.IncomeID);

            if(incomeResponse == null)
            {
                return RedirectToAction("Index");
            }

            await _incomeService.DeleteIncome(result.IncomeID);

            return RedirectToAction("Index");
	    }


        [Route("IncomesExcel")]
        public async Task<IActionResult> IncomesExcel(string searchBy, string searchString, string fromDate, string toDate, string sum)
        {
			var userId = _userManager.GetUserId(User);

			MemoryStream memoryStream = await _incomeService.GetExcelDataFromIncome(searchBy, searchString, fromDate, toDate, sum, userId);
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "incomes.xlsx");

        }
    }
}
