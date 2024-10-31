using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using System.ComponentModel.Design;

namespace FinanceManager.Controllers
{
	[Route("[controller]")]
	public class ExpenseUploadController : Controller
	{
		private readonly IExpenseService _expenseService;

		public ExpenseUploadController(IExpenseService expenseService)
		{
			_expenseService = expenseService;	
		}

		[Route("ExpenseUpload")]
		public IActionResult ExpenseUpload()
		{
			return View();
		}


		[HttpPost]
		[Route("ExpenseUpload")]
		public async Task<IActionResult> ExpenseUpload(IFormFile excelFile)
		{
			if (excelFile == null || excelFile.Length == 0) 
			{
				ViewBag.ErrorMessage = "작성 완료된 엑셀파일을 선택해주세요";
				return View();
			}

			

			if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx",
				StringComparison.OrdinalIgnoreCase))
			{
				ViewBag.ErrorMessage = "엑셀파일을 선택해주세요";
				return View();
			}


			int insertedCount = await _expenseService.UploadExpenseFromExcelFile(excelFile);

			if (insertedCount == 99999)
			{
				ViewBag.ErrorMessage = "날짜(yyyy-MM-dd), 금액(0 이상인 숫자), 항목(주거비/교통비/장보기/외식비/쇼핑/보험료/기타)이 맞게 입력되었는지 확인해주세요";
			}
			else if (insertedCount == 99998)
			{
				ViewBag.ErrorMessage = "날짜, 이름, 항목, 금액은 필수 입력입니다";
			}
			else
			{
				ViewBag.Message = $"{insertedCount}개가 업로드 되었습니다";
			}
			
			return View();
		}
	}
}
