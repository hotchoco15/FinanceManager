using System;
using Microsoft.AspNetCore.Http;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace ServiceContracts
{
    /// <summary>
    /// 지출에 대한 비지니스 로직입니다
    /// </summary>
    public interface IExpenseService
    {
		/// <summary>
		/// 이번달의 모든 지출을 보여준다 
		/// </summary>
		/// <returns></returns>
		Task<List<ExpenseResponse>> GetDefaultExpenses(string? searchBy, string? searchString, DateTime? fromDate, DateTime? toDate);


		/// <summary>
		/// 선택 항목, 검색어, 날짜에 해당하는 데이터를 보여준다 
		/// </summary>
		/// <param name="searchBy"></param>
		/// <param name="searchString"></param>
		/// <param name="fromDate"></param>
		/// <param name="toDate"></param>
		/// <returns></returns>
		Task<List<ExpenseResponse>> GetSelectedExpenses(string? searchBy, string? searchString, DateTime? fromDate, DateTime? toDate);


		/// <summary>
		/// 지출 건을 추가한다
		/// </summary>
		/// <param name="expenseAddRequest">Expense to add</param>
		/// <returns></returns>
		Task<ExpenseResponse> AddExpense(ExpenseAddRequest? expenseAddRequest);



		/// <summary>
		/// ExpenseID에 해당하는 지출내역을 보여준다
		/// </summary>
		/// <param name="incomeID"></param>
		/// <returns></returns>
		Task<ExpenseResponse?> GetExpenseByExpenseID(Guid? expenseID);




		/// <summary>
		/// 특정 지출 건을 수정한다
		/// </summary>
		/// <param name="expenseUpdateRequest"></param>
		/// <returns></returns>
		Task<ExpenseResponse> UpdateExpense(ExpenseUpdateRequest? expenseUpdateRequest);




		/// <summary>
		/// 특정 지출 건을 삭제한다
		/// </summary>
		/// <param name="expenseID"></param>
		/// <returns></returns>
		Task<bool> DeleteExpense(Guid? expenseID);


		/// <summary>
		/// 내역을 엑셀로 출력한다 
		/// </summary>
		/// <returns></returns>
		Task<MemoryStream> GetExcelDataFromExpense(string name1, string name2, string name3, string name4, string name5);



		/// <summary>
		/// 엑셀에 있는 내역을 DB에 저장하기
		/// </summary>
		/// <param name="formFile"></param>
		/// <returns></returns>
		Task<int> UploadExpenseFromExcelFile(IFormFile formFile);
	}
}
