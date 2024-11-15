using Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using System.ComponentModel.Design;

namespace FinanceManager.Controllers
{
	[Route("[controller]")]
	public class IncomeUploadController : Controller
	{
		private readonly IIncomeService _incomeService;

		private readonly UserManager<ApplicationUser> _userManager;

		public IncomeUploadController(IIncomeService incomeService, UserManager<ApplicationUser> userManager)
		{
			_incomeService = incomeService;	

			_userManager = userManager;
		}

		[Route("IncomeUpload")]
		public IActionResult IncomeUpload()
		{
			return View();
		}


		[HttpPost]
		[Route("IncomeUpload")]
		public async Task<IActionResult> IncomeUpload(IFormFile excelFile)
		{
			var userId = _userManager.GetUserId(User);

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


			int insertedCount = await _incomeService.UploadIncomeFromExcelFile(excelFile, userId);

			if (insertedCount == 99999)
			{
				ViewBag.ErrorMessage = "날짜(yyyy-MM-dd), 금액(0 이상인 숫자), 항목(주수입/부수입)이 맞게 입력되었는지 확인해주세요";
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
