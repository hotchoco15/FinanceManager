using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        
        public ExpenseController(IExpenseService expenseService)
        {
		_expenseService = expenseService;
        }

        [Route("[action]")]
        public async Task<IActionResult> Index(string? searchBy = null, string? searchString = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
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
                List<ExpenseResponse> expenses = await _expenseService.GetDefaultExpenses(searchBy, searchString, fromDate, toDate);

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

		List<ExpenseResponse> filteredExpenses = await _expenseService.GetSelectedExpenses(searchBy, searchString, fromDate, toDate);


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
	    ExpenseResponse? expenseResponse = await _expenseService.GetExpenseByExpenseID(expenseID);
            if (expenseResponse == null)
            {
                return RedirectToAction("Index");
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
	    ExpenseResponse? expenseResponse = await _expenseService.GetExpenseByExpenseID(expenseID);

            if(expenseResponse == null) 
            {
                return RedirectToAction("Index");
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
	public async Task<IActionResult> ExpensesExcel(string name1, string name2, string name3, string name4, string name5)
	{
		
		MemoryStream memoryStream = await _expenseService.GetExcelDataFromExpense(name1, name2, name3, name4, name5);
		return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "expenses.xlsx");

	}
     }
}
