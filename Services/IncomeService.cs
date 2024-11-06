using System;
using Entities;
using ServiceContracts;
using ServiceContracts.Enums;
using ServiceContracts.DTO;
using System.ComponentModel.DataAnnotations;
using Services.Helpers;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;
using System.Linq;


namespace Services
{
    public class IncomeService : IIncomeService
    {
        private readonly ApplicationDbContext _db;

		public IncomeService(ApplicationDbContext incomeDbContext)
        {
            _db = incomeDbContext;
		}

        public async Task<IncomeResponse> AddIncome(IncomeAddRequest? incomeAddRequest)
        {
            if (incomeAddRequest == null)
            {
                throw new ArgumentNullException(nameof(incomeAddRequest));
            }

            //Model validation
            ValidationHelper.ModelValidation(incomeAddRequest);

            //incomeAddRequest -> Income 
            Income income = incomeAddRequest.ToIncome();

            // ID 생성
            income.IncomeID = Guid.NewGuid();

            // 추가
            _db.Incomes.Add(income);
			await _db.SaveChangesAsync();

            // income -> IncomeResponse 
            IncomeResponse incomeResponse = income.ToIncomeResponse();

            return incomeResponse;
        }

		//수입내역 삭제하기
		public async Task<bool> DeleteIncome(Guid? incomeID)
		{
			if (incomeID == null)
			{
				throw new ArgumentNullException(nameof(incomeID));
			}

			Income? income = await _db.Incomes.FirstOrDefaultAsync(temp => temp.IncomeID == incomeID);
			if (income == null)
				return false;

			_db.Incomes.Remove(_db.Incomes.First(temp => temp.IncomeID == incomeID));
			await _db.SaveChangesAsync();

			return true;
		}


		//첫 화면에 이번 달의 수입 내역을 보여주기 
		public async Task<List<IncomeResponse>> GetDefaultIncomes(string? searchBy, string? searchString, DateTime? fromDate, DateTime? toDate)
		{
			List<IncomeResponse> allIncomes = await _db.Incomes.Select(temp => temp.ToIncomeResponse()).ToListAsync();
            List<IncomeResponse> results = new List<IncomeResponse>();

            toDate = DateTime.Today;
            
            fromDate = new DateTime(toDate.Value.Year, toDate.Value.Month, 1);
            results = allIncomes.Where(temp => temp.DateOfIncome >= fromDate && temp.DateOfIncome <= toDate).OrderBy(temp => temp.DateOfIncome).ToList();

			return results;
		}

		// 보이는 화면을 엑셀로 출력하기
		public async Task<MemoryStream> GetExcelDataFromIncome(string name1, string name2, string name3, string name4, string name5)
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
				List<IncomeResponse> incomes = new List<IncomeResponse>();

				if (name1 == "00000")
				{
					List<IncomeResponse> allIncomes = await _db.Incomes.Select(temp => temp.ToIncomeResponse()).ToListAsync();
					
					DateTime toDate = DateTime.Today;
					DateTime fromDate = new DateTime(toDate.Year, toDate.Month, 1);

					incomes = allIncomes.Where(temp => temp.DateOfIncome >= fromDate && temp.DateOfIncome <= toDate).OrderBy(temp => temp.DateOfIncome).ToList();
				}
				else
				{
					incomes = await GetSelectedIncomes(name1, name2, name3 != null? Convert.ToDateTime(name3) : null, name4 != null ? Convert.ToDateTime(name4) : null);
				}

				

				foreach (var income in incomes)
				{
					excelWorkSheet.Cells[row, 1].Value = income.DateOfIncome.Value.ToString("yyyy-MM-dd");
					excelWorkSheet.Cells[row, 2].Value = income.IncomeName;
					excelWorkSheet.Cells[row, 3].Value = income.IncomeType?.GetDisplayName();
					excelWorkSheet.Cells[row, 4].Value = income.IncomeAmount;
					if (!string.IsNullOrEmpty(income.IncomeRemark))
					{
						excelWorkSheet.Cells[row, 5].Value = income.IncomeRemark;
					}
					if (incomes.Count != 0 && row == incomes.Count+1)
					{
						excelWorkSheet.Cells[row + 1, 6].Value = Double.Parse(name5);
					}

					row++;
				}
				

				excelWorkSheet.Cells[$"A1:F{row}"].AutoFitColumns();

				await excelPackage.SaveAsAsync(memoryStream);
			}
			
			

			memoryStream.Position = 0;
			return memoryStream;
		}



		// IncomeID에 해당하는 수입 내역을 보여주기
		public async Task<IncomeResponse?> GetIncomeByIncomeID(Guid? incomeID)
		{
			if (incomeID == null) return null;

			Income? income = await _db.Incomes
				.FirstOrDefaultAsync(temp => temp.IncomeID == incomeID);
			
			if(income == null) return null;
			
			IncomeResponse incomeResponse = income.ToIncomeResponse();

			return incomeResponse;
		}



		// 수입 조건에 따라 보여주기
		public async Task<List<IncomeResponse>> GetSelectedIncomes(string? searchBy, string? searchString, DateTime? fromDate, DateTime? toDate)
		{
			List<IncomeResponse> allIncomes = await _db.Incomes.Select(temp => temp.ToIncomeResponse()).ToListAsync();
			List<IncomeResponse> results = new List<IncomeResponse>();

			// 항목만 선택했을 때
			if (searchBy != "NotSelected" && string.IsNullOrEmpty(searchString) && fromDate == null && toDate == null)
            {
                results = allIncomes.Where(temp => temp.IncomeType.Equals(searchBy)).OrderBy(temp => temp.DateOfIncome).ToList();
				return results;
			}

			// 검색어만 조회했을 때
			if (!string.IsNullOrEmpty(searchString) && (searchBy == "NotSelected") && fromDate == null && toDate == null) 
            {
                results = allIncomes.Where(temp => temp.IncomeName.Contains(searchString)).OrderBy(temp => temp.DateOfIncome).ToList();
				return results;
			}

			// 날짜로만 조회했을 때
			if (fromDate != null && toDate != null && (searchBy == "NotSelected") && string.IsNullOrEmpty(searchString)) 
            {
                results = allIncomes.Where(temp => temp.DateOfIncome >= fromDate && temp.DateOfIncome <= toDate).OrderBy(temp => temp.DateOfIncome).ToList();
				return results;
			}

			// 항목과 날짜로 조회했을 때
			if (searchBy != "NotSelected" && fromDate != null && toDate != null && string.IsNullOrEmpty(searchString))
            {
                results = allIncomes.Where(temp => (temp.DateOfIncome >= fromDate && temp.DateOfIncome <= toDate) && (temp.IncomeType.Equals(searchBy)))
					        .OrderBy(temp => temp.DateOfIncome).ToList();

				return results;
			}

			// 검색어와 날짜로 조회했을 때
			if (!string.IsNullOrEmpty(searchString) && fromDate != null && toDate != null && searchBy == "NotSelected")
            {
                results = allIncomes.Where(temp => (temp.DateOfIncome >= fromDate && temp.DateOfIncome <= toDate) && (temp.IncomeName.Contains(searchString)))
							.OrderBy(temp => temp.DateOfIncome).ToList();

				return results;
			}

			// 항목과 검색어로 조회했을 때
			if (searchBy != "NotSelected" && !string.IsNullOrEmpty(searchString) && fromDate == null && toDate == null)
			{
				results = allIncomes.Where(temp => (temp.IncomeType.Equals(searchBy)) && (temp.IncomeName.Contains(searchString)))
							.OrderBy(temp => temp.DateOfIncome).ToList();

				return results;
			}

			// 항목, 검색어, 날짜로 조회했을 때 
			if (searchBy != "NotSelected" && !string.IsNullOrEmpty(searchString) && fromDate != null && toDate != null)
			{
				results = allIncomes.Where(temp => (temp.DateOfIncome >= fromDate && temp.DateOfIncome <= toDate) && (temp.IncomeName.Contains(searchString))
							&& (temp.IncomeType.Equals(searchBy))).OrderBy(temp => temp.DateOfIncome).ToList();

				return results;
			}




			return results;
		}

		// 수입내역 수정하기
		public async Task<IncomeResponse> UpdateIncome(IncomeUpdateRequest? incomeUpdateRequest)
		{
			if (incomeUpdateRequest == null)
			{
				throw new ArgumentNullException(nameof(Income));
			}

			//Model validation
			ValidationHelper.ModelValidation(incomeUpdateRequest);

			//맞는 데이터 
			Income? matchingIncome = await _db.Incomes.FirstOrDefaultAsync(temp => temp.IncomeID == incomeUpdateRequest.IncomeID);
			
			if (matchingIncome == null)
			{
				throw new ArgumentException("ID가 존재하지 않습니다");
			}

			matchingIncome.DateOfIncome = incomeUpdateRequest.DateOfIncome;
			matchingIncome.IncomeName = incomeUpdateRequest.IncomeName;
			matchingIncome.IncomeType = incomeUpdateRequest.IncomeType.ToString();
			matchingIncome.IncomeAmount = incomeUpdateRequest.IncomeAmount;
			matchingIncome.IncomeRemark = incomeUpdateRequest.IncomeRemark;

			await _db.SaveChangesAsync();

			return matchingIncome.ToIncomeResponse();
		}


		// 엑셀 업로드 
		public async Task<int> UploadIncomeFromExcelFile(IFormFile formFile)
		{
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
						double p = 0;


						if (Regex.IsMatch(cellValue1, @"^(19|20)\d{2}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[0-1])$")
							&& Double.TryParse(cellValue4, out p) &&
							(cellValue3.Equals("주수입") || cellValue3.Equals("부수입")))
						{
							if (p >= 0)
							{
								Income income = new Income()
								{
									DateOfIncome = DateTime.Parse(cellValue1),
									IncomeName = cellValue2,
									IncomeType = cellValue3.Equals("주수입") ? "MainIncome" : "ExtraIncome",
									IncomeAmount = p,
									IncomeRemark = cellValue5
								};
								_db.Incomes.Add(income);
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
	}
}
