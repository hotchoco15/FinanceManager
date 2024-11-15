using Entities;
using Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration.UserSecrets;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;
using System.Collections.Generic;

namespace FinanceManager.Controllers
{
    [Route("[controller]")]
    public class ExpenseController : Controller
    {
        private readonly IExpenseService _expenseService;

		private readonly UserManager<ApplicationUser> _userManager;


		public ExpenseController(IExpenseService expenseService, UserManager<ApplicationUser> userManager)
        {
		    _expenseService = expenseService;

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
                {"Housing", "주거비"},
		        {"Transportation", "교통비"},
		        {"Grocery", "장보기"},
		        {"Food", "외식비"},
		        {"Shopping", "쇼핑"},
		        {"InsuranceFee", "보험료"},
		        {"Other", "기타"}

	            };

            if (string.IsNullOrEmpty(searchBy))
            {
				List<ExpenseResponse> expenses = await _expenseService.GetDefaultExpenses(searchBy, searchString, fromDate, toDate, userId);

                ViewBag.SearchBy = "00000";
                ViewBag.SearchString = searchString;

                fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                toDate = DateTime.Today;

		        ViewBag.ToDate = toDate;
                ViewBag.FromDate = fromDate;

                if (expenses.Count > 0)
                {
                    ViewBag.Last = expenses.Last().ExpenseID;
                    ViewBag.Sum = expenses.Sum(i => i.ExpenseAmount);
                }

		        ViewBag.InitialMessage = "*** 이번 달의 지출 내역입니다 ***";

		        fromDate = null;
                toDate = null;

				return View(expenses);
            }

            else 
            {
		        ViewBag.SearchBy = searchBy;
		        ViewBag.SearchString = searchString;
                ViewBag.ToDate = toDate;
                ViewBag.FromDate = fromDate;

		        List<ExpenseResponse> filteredExpenses = await _expenseService.GetSelectedExpenses(searchBy, searchString, fromDate, toDate, userId);


                if (filteredExpenses.Count > 0)
                { 
                    ViewBag.Last = filteredExpenses.Last().ExpenseID;
                    ViewBag.Sum = filteredExpenses.Sum(i => i.ExpenseAmount);
                }
                else
                {
                    ViewBag.Message = "empty";
                }

				return View(filteredExpenses);
	        }
        }

        [Route("[action]")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.ExpenseOptions = new Dictionary<string, string>()
            {
		        {"Housing", "주거비"},
		        {"Transportation", "교통비"},
		        {"Grocery", "장보기"},
		        {"Food", "외식비"},
		        {"Shopping", "쇼핑"},
		        {"InsuranceFee", "보험료"},
		        {"Other", "기타"}
	        };

            var userId = _userManager.GetUserId(User);
            ViewBag.UserID = userId;

			return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Create(ExpenseAddRequest expenseAddRequest)
        {
            if (!ModelState.IsValid) 
            {
		        ViewBag.ExpenseOptions = new Dictionary<string, string>()
		        {
		            {"Housing", "주거비"},
		            {"Transportation", "교통비"},
		            {"Grocery", "장보기"},
		            {"Food", "외식비"},
		            {"Shopping", "쇼핑"},
		            {"InsuranceFee", "보험료"},
		            {"Other", "기타"}
		        };

		        ViewBag.Errors = ModelState.Values.SelectMany(error => error.Errors).Select(e => e.ErrorMessage).ToList();
		        return View();
	        }

            
		    ExpenseResponse expenseResponse = await _expenseService.AddExpense(expenseAddRequest);

		    return RedirectToAction("Index");
        }

	    [Route("[action]/{expenseID}")]
	    [HttpGet]
        public async Task<IActionResult> Update(Guid expenseID)
        {
			var userId = _userManager.GetUserId(User);
			Guid parsedUserId = new Guid(userId);

			ExpenseResponse? expenseResponse = await _expenseService.GetExpenseByExpenseID(expenseID);
            if (expenseResponse == null)
            {
                return RedirectToAction("Index");
            }

            if (expenseResponse.UserID != parsedUserId)
            {
                return Unauthorized();
            }


            ExpenseUpdateRequest expenseUpdateRequest = expenseResponse.ToExpenseUpdateRequest();

	        ViewBag.ExpenseOptions = new Dictionary<string, string>()
	        {
		        {"Housing", "주거비"},
		        {"Transportation", "교통비"},
		        {"Grocery", "장보기"},
		        {"Food", "외식비"},
		        {"Shopping", "쇼핑"},
		        {"InsuranceFee", "보험료"},
		        {"Other", "기타"}
	        };
	    
     	    return View(expenseUpdateRequest);
        }

	    [Route("[action]/{expenseID}")]
	    [HttpPost]
	    public async Task<IActionResult> Update(ExpenseUpdateRequest expenseUpdateRequest)
        {
	        ExpenseResponse? expenseResponse = await _expenseService.GetExpenseByExpenseID(expenseUpdateRequest.ExpenseID);

            if(expenseResponse == null) 
            { 
                return RedirectToAction("Index"); 
            }

            if(ModelState.IsValid) 
            {
			    ExpenseResponse updatedPerson = await _expenseService.UpdateExpense(expenseUpdateRequest);
                return RedirectToAction("Index");
            }
            else
            {
		        ViewBag.ExpenseOptions = new Dictionary<string, string>()
		        {
		            {"Housing", "주거비"},
		            {"Transportation", "교통비"},
		            {"Grocery", "장보기"},
		            {"Food", "외식비"},
		            {"Shopping", "쇼핑"},
		            {"InsuranceFee", "보험료"},
		            {"Other", "기타"}
		        };

		        ViewBag.Errors = ModelState.Values.SelectMany(error => error.Errors).Select(e => e.ErrorMessage).ToList();
		        return View(expenseResponse.ToExpenseUpdateRequest());
		    }
        }


        [HttpGet]
        [Route("[action]/{expenseID}")]
        public async Task<IActionResult> Delete(Guid expenseID) 
        {
			var userId = _userManager.GetUserId(User);
			Guid parsedUserId = new Guid(userId);

			ExpenseResponse? expenseResponse = await _expenseService.GetExpenseByExpenseID(expenseID);

            if(expenseResponse == null) 
            {
                return RedirectToAction("Index");
            }

			if (expenseResponse.UserID != parsedUserId)
			{
				return Unauthorized();
			}

			return View(expenseResponse);
        }

        [HttpPost]
	    [Route("[action]/{expenseID}")]
        public async Task<IActionResult> Delete(ExpenseUpdateRequest result)
        {
	        ExpenseResponse? expenseResponse = await _expenseService.GetExpenseByExpenseID(result.ExpenseID);

            if(expenseResponse == null)
            {
                return RedirectToAction("Index");
            }

	        await _expenseService.DeleteExpense(result.ExpenseID);

            return RedirectToAction("Index");
	    }

	    [Route("ExpensesExcel")]
	    public async Task<IActionResult> ExpensesExcel(string searchBy, string searchString, string fromDate, string toDate, string sum)
	    {
			var userId = _userManager.GetUserId(User);

			MemoryStream memoryStream = await _expenseService.GetExcelDataFromExpense(searchBy, searchString, fromDate, toDate, sum, userId);
		    return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "expenses.xlsx");

	    }
    }
}
