using System;
using Entities;
using ServiceContracts;
using ServiceContracts.Enums;
using ServiceContracts.DTO;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Entities.IdentityEntities;
using System.Security.Claims;

namespace Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ApplicationDbContext _db;

        public ExpenseService(ApplicationDbContext incomeDbContext)
        {
			_db = incomeDbContext;	
		}

        public async Task<ExpenseResponse> AddExpense(ExpenseAddRequest? expenseAddRequest)
        {
            if (expenseAddRequest == null)
            {
                throw new ArgumentNullException(nameof(expenseAddRequest));
            }

            //Model validation
            ValidationHelper.ModelValidation(expenseAddRequest);

			//expenseAddRequest -> Expense 
			Expense expense = expenseAddRequest.ToExpense();
	
			// ID 생성
			expense.ExpenseID = Guid.NewGuid();
	
			// 추가
			_db.Expenses.Add(expense);
			await _db.SaveChangesAsync();
	
			// expense -> ExpenseResponse 
			ExpenseResponse expenseResponse = expense.ToExpenseResponse();

			return expenseResponse;
        }

		//지출내역 삭제하기
		public async Task<bool> DeleteExpense(Guid? expenseID)
		{
			if (expenseID == null)
			{
				throw new ArgumentNullException(nameof(expenseID));
			}
	
			Expense? expense = await _db.Expenses.FirstOrDefaultAsync(temp => temp.ExpenseID == expenseID);
			if (expense == null)
				return false;
	
			_db.Expenses.Remove(_db.Expenses.First(temp => temp.ExpenseID == expenseID));
			await _db.SaveChangesAsync();
	
			return true;
		}

		//첫 화면에 이번 달의 지출 내역을 보여준다 
		public async Task<List<ExpenseResponse>> GetDefaultExpenses(string? searchBy, string? searchString, DateTime? fromDate, DateTime? toDate, string userId)
		{
			Guid parsedUserId =  new Guid(userId);

			var expenseByUser = await _db.Expenses.Where(e => e.UserID == parsedUserId).ToListAsync();

			List<ExpenseResponse> allExpenses = expenseByUser
					.Select(temp => temp.ToExpenseResponse()).ToList();
			List<ExpenseResponse> results = new List<ExpenseResponse>();

			toDate = DateTime.Today;
            
			fromDate = new DateTime(toDate.Value.Year, toDate.Value.Month, 1);
			results = allExpenses.Where(temp => temp.DateOfExpense >= fromDate && temp.DateOfExpense <= toDate).OrderBy(temp => temp.DateOfExpense).ToList();

			return results;
		}

		// 보이는 화면을 엑셀로 출력하기
		public async Task<MemoryStream> GetExcelDataFromExpense(string searchBy, string searchString, string fromDate, string toDate, string sum, string userId)
		{
			MemoryStream memoryStream = new MemoryStream();

			using (ExcelPackage excelPackage = new ExcelPackage())
			{
				ExcelWorksheet excelWorkSheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
				excelWorkSheet.Cells["A1"].Value = "날짜";
				excelWorkSheet.Cells["B1"].Value = "이름";
				excelWorkSheet.Cells["C1"].Value = "항목";
				excelWorkSheet.Cells["D1"].Value = "금액";
				excelWorkSheet.Cells["E1"].Value = "비고";
				excelWorkSheet.Cells["F1"].Value = "합계";


				int row = 2;
				List<ExpenseResponse> expenses = new List<ExpenseResponse>();

				if (searchBy == "00000")
				{
					Guid parsedUserId = new Guid(userId);

					var expenseByUser = await _db.Expenses.Where(e => e.UserID == parsedUserId).ToListAsync();

					List<ExpenseResponse> allExpenses = expenseByUser
							.Select(temp => temp.ToExpenseResponse()).ToList();

					DateTime today = DateTime.Today;
					DateTime fromFirstDate = new DateTime(today.Year, today.Month, 1);

					expenses = allExpenses.Where(temp => temp.DateOfExpense >= fromFirstDate && temp.DateOfExpense <= today).OrderBy(temp => temp.DateOfExpense).ToList();
				}
				else
				{
					expenses = await GetSelectedExpenses(searchBy, searchString, fromDate != null ? Convert.ToDateTime(fromDate) : null, toDate != null ? Convert.ToDateTime(toDate) : null, userId);
				}



				foreach (var expense in expenses)
				{
					excelWorkSheet.Cells[row, 1].Value = expense.DateOfExpense.Value.ToString("yyyy-MM-dd");
					excelWorkSheet.Cells[row, 2].Value = expense.ExpenseName;
					excelWorkSheet.Cells[row, 3].Value = expense.ExpenseType?.GetDisplayNameofExpense();
					excelWorkSheet.Cells[row, 4].Value = expense.ExpenseAmount;
					if (!string.IsNullOrEmpty(expense.ExpenseRemark))
					{
						excelWorkSheet.Cells[row, 5].Value = expense.ExpenseRemark;
					}
					if (expenses.Count != 0 && row == expenses.Count + 1)
					{
						excelWorkSheet.Cells[row + 1, 6].Value = Double.Parse(sum);
					}

					row++;
				}


				excelWorkSheet.Cells[$"A1:F{row}"].AutoFitColumns();

				await excelPackage.SaveAsAsync(memoryStream);
			}



			memoryStream.Position = 0;
			return memoryStream;
		}


		//ExpenseID에 해당하는 지출 내역을 보여준다
		public async Task<ExpenseResponse?> GetExpenseByExpenseID(Guid? expenseID)
		{
			if (expenseID == null) return null;

			Expense? expense = await _db.Expenses
				.FirstOrDefaultAsync(temp => temp.ExpenseID == expenseID);
		
			if(expense == null) return null;

			ExpenseResponse expenseResponse = expense.ToExpenseResponse();

			return expenseResponse;
		}



		// 지출 조건에 따라 보여준다 
		public async Task<List<ExpenseResponse>> GetSelectedExpenses(string? searchBy, string? searchString, DateTime? fromDate, DateTime? toDate, string userId)
		{
			Guid parsedUserId = new Guid(userId);

			var expenseByUser = await _db.Expenses.Where(e => e.UserID == parsedUserId).ToListAsync();

			List<ExpenseResponse> allExpenses = expenseByUser
					.Select(temp => temp.ToExpenseResponse()).ToList();
			List<ExpenseResponse> results = new List<ExpenseResponse>();

			// 항목만 선택했을 때
			if (searchBy != "NotSelected" && string.IsNullOrEmpty(searchString) && fromDate == null && toDate == null)
			{
				results = allExpenses.Where(temp => temp.ExpenseType.Equals(searchBy)).OrderBy(temp => temp.DateOfExpense).ToList();
				return results;
			}
	
			// 검색어만 조회했을 때
			if (!string.IsNullOrEmpty(searchString) && (searchBy == "NotSelected") && fromDate == null && toDate == null) 
			{
				results = allExpenses.Where(temp => temp.ExpenseName.Contains(searchString)).OrderBy(temp => temp.DateOfExpense).ToList();
				return results;
			}
	
			// 날짜로만 조회했을 때
			if (fromDate != null && toDate != null && (searchBy == "NotSelected") && string.IsNullOrEmpty(searchString)) 
			{
				results = allExpenses.Where(temp => temp.DateOfExpense >= fromDate && temp.DateOfExpense <= toDate).OrderBy(temp => temp.DateOfExpense).ToList();
				return results;
			}
	
			// 항목과 날짜로 조회했을 때
			if (searchBy != "NotSelected" && fromDate != null && toDate != null && string.IsNullOrEmpty(searchString))
			{
				results = allExpenses.Where(temp => (temp.DateOfExpense >= fromDate && temp.DateOfExpense <= toDate) && (temp.ExpenseType.Equals(searchBy)))
								.OrderBy(temp => temp.DateOfExpense).ToList();
	
				return results;
			}
	
			// 검색어와 날짜로 조회했을 때
			if (!string.IsNullOrEmpty(searchString) && fromDate != null && toDate != null && searchBy == "NotSelected")
			{
				results = allExpenses.Where(temp => (temp.DateOfExpense >= fromDate && temp.DateOfExpense <= toDate) && (temp.ExpenseName.Contains(searchString)))
								.OrderBy(temp => temp.DateOfExpense).ToList();
	
				return results;
			}
	
			// 항목과 검색어로 조회했을 때
			if (searchBy != "NotSelected" && !string.IsNullOrEmpty(searchString) && fromDate == null && toDate == null)
			{
				results = allExpenses.Where(temp => (temp.ExpenseType.Equals(searchBy)) && (temp.ExpenseName.Contains(searchString)))
							.OrderBy(temp => temp.DateOfExpense).ToList();
	
				return results;
			}
	
			// 항목, 검색어, 날짜로 조회했을 때 
			if (searchBy != "NotSelected" && !string.IsNullOrEmpty(searchString) && fromDate != null && toDate != null)
			{
				results = allExpenses.Where(temp => (temp.DateOfExpense >= fromDate && temp.DateOfExpense <= toDate) && (temp.ExpenseName.Contains(searchString))
							&& (temp.ExpenseType.Equals(searchBy))).OrderBy(temp => temp.DateOfExpense).ToList();
	
				return results;
			}
	
	
			return results;
		}

		
		// 지출내역 수정하기
		public async Task<ExpenseResponse> UpdateExpense(ExpenseUpdateRequest? expenseUpdateRequest)
		{
			if (expenseUpdateRequest == null)
			{
				throw new ArgumentNullException(nameof(Expense));
			}

			//Model validation
			ValidationHelper.ModelValidation(expenseUpdateRequest);

			//맞는 데이터 
			Expense? matchingExpense = await _db.Expenses.FirstOrDefaultAsync(temp => temp.ExpenseID == expenseUpdateRequest.ExpenseID);
		
			if (matchingExpense == null)
			{
				throw new ArgumentException("ID가 존재하지 않습니다");
			}

			matchingExpense.DateOfExpense = expenseUpdateRequest.DateOfExpense;
			matchingExpense.ExpenseName = expenseUpdateRequest.ExpenseName;
			matchingExpense.ExpenseType = expenseUpdateRequest.ExpenseType.ToString();
			matchingExpense.ExpenseAmount = expenseUpdateRequest.ExpenseAmount;
			matchingExpense.ExpenseRemark = expenseUpdateRequest.ExpenseRemark;

			await _db.SaveChangesAsync();

			return matchingExpense.ToExpenseResponse();
		}

		// 엑셀 업로드 
		public async Task<int> UploadExpenseFromExcelFile(IFormFile formFile, string userId)
		{
			Guid parsedUserId = new Guid(userId);

			MemoryStream memoryStream = new MemoryStream();
			await formFile.CopyToAsync(memoryStream);
			int insertedCount = 0;

			using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
			{
				ExcelWorksheet excelWorkSheet = excelPackage.Workbook.Worksheets["Sheet1"];

				int rowCount = excelWorkSheet.Dimension.Rows;

				for (int row = 2; row <= rowCount; row++)
				{
					string? cellValue1 = Convert.ToString(excelWorkSheet.Cells[row, 1].Value).Substring(0, 10);
					string? cellValue2 = Convert.ToString(excelWorkSheet.Cells[row, 2].Value);
					string? cellValue3 = Convert.ToString(excelWorkSheet.Cells[row, 3].Value);
					string? cellValue4 = Convert.ToString(excelWorkSheet.Cells[row, 4].Value);
					string? cellValue5 = Convert.ToString(excelWorkSheet.Cells[row, 5].Value);


					if (!string.IsNullOrEmpty(cellValue1) && !string.IsNullOrEmpty(cellValue2) && !string.IsNullOrEmpty(cellValue3) && !string.IsNullOrEmpty(cellValue4))
					{
						double price = 0;

						if (Regex.IsMatch(cellValue1, @"^(19|20)\d{2}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[0-1])$")
							&& Double.TryParse(cellValue4, out price) &&
							(cellValue3.Equals("주거비") || cellValue3.Equals("교통비") || cellValue3.Equals("장보기")
							|| cellValue3.Equals("외식비") || cellValue3.Equals("쇼핑") || cellValue3.Equals("보험료")
							|| cellValue3.Equals("기타")))
						{
							if (price >= 0)
							{
								Expense expense = new Expense()
								{
									DateOfExpense = DateTime.Parse(cellValue1),
									ExpenseName = cellValue2,
									ExpenseType = cellValue3.Equals("주거비") ? "Housing" 
									: cellValue3.Equals("교통비") ? "Transportation"
									: cellValue3.Equals("장보기") ? "Grocery"
									: cellValue3.Equals("외식비") ? "Food"
									: cellValue3.Equals("쇼핑") ? "Shopping"
									: cellValue3.Equals("보험료") ? "InsuranceFee" : "Other",
									ExpenseAmount = price,
									ExpenseRemark = cellValue5,
									UserID = parsedUserId
								};

								_db.Expenses.Add(expense);
								await _db.SaveChangesAsync();

								insertedCount++;
							}
							else
							{
								// 날짜 타입, 항목, 금액 타입이 맞지 않거나 0보다 작을 때   
								insertedCount = 99999;
							}
						}
						else
						{
							// 날짜 타입, 항목, 금액 타입이 맞지 않거나 0보다 작을 때 
							insertedCount = 99999;
						}
					}

					else
					{
						// 필수항목 널 체크 
						insertedCount = 99998;
					}
				}
			}

			return insertedCount == 99999 ? 99999 : insertedCount == 99998 ? 99998 : insertedCount;
		}


		// 페이징하여 보여주기
		public async Task<PagingResponse<ExpenseResponse>> GetPages(List<ExpenseResponse> list, int page, int pageSize)
		{
			var skipResult = (page - 1) * pageSize;

			var pagenatedExpenses = list.Skip(skipResult).Take(pageSize).ToList();

			int totalPage = 0;
			int totalCount = list.Count();

			if (totalCount % pageSize == 0)
			{
				totalPage = totalCount / pageSize;
			}
			else
			{
				totalPage = totalCount / pageSize + 1;
			}

			var response = new PagingResponse<ExpenseResponse>();

			response.Data = pagenatedExpenses;
			response.TotalPage = totalPage;

			return response;
		}
	}
}
