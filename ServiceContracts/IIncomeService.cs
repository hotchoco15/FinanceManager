﻿using System;
using System.IO;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Microsoft.AspNetCore.Http;


namespace ServiceContracts
{
	/// <summary>
	/// 수입에 대한 비지니스 로직입니다
	/// </summary>
	public interface IIncomeService
        {
		/// <summary>
		/// 이번달의 모든 수입을 보여줍니다 
		/// </summary>
		/// <returns></returns>
		Task<List<IncomeResponse>> GetDefaultIncomes(string? searchBy, string? searchString, DateTime? fromDate, DateTime? toDate, string userId);


		/// <summary>
		/// 선택 항목, 검색어, 날짜에 해당하는 데이터를 보여줍니다 
		/// </summary>
		/// <param name="searchBy"></param>
		/// <param name="searchString"></param>
		/// <param name="fromDate"></param>
		/// <param name="toDate"></param>
		/// <returns></returns>
		Task<List<IncomeResponse>> GetSelectedIncomes(string? searchBy, string? searchString, DateTime? fromDate, DateTime? toDate, string userId);


		/// <summary>
		/// 수입 건을 추가합니다
		/// </summary>
		/// <param name="incomeAddRequest"></param>
		/// <returns></returns>
		Task<IncomeResponse> AddIncome(IncomeAddRequest? incomeAddRequest);



		/// <summary>
		/// IncomeID에 해당하는 수입내역을 보여줍니다다
		/// </summary>
		/// <param name="incomeID"></param>
		/// <returns></returns>
		Task<IncomeResponse?> GetIncomeByIncomeID(Guid? incomeID);




		/// <summary>
		/// 특정 수입 건을 수정합니다
		/// </summary>
		/// <param name="incomeUpdateRequest"></param>
		/// <returns></returns>
		Task<IncomeResponse> UpdateIncome(IncomeUpdateRequest? incomeUpdateRequest);




		/// <summary>
		/// 특정 수입 건을 삭제합니다
		/// </summary>
		/// <param name="incomeID"></param>
		/// <returns></returns>
		Task<bool> DeleteIncome(Guid? incomeID);



		/// <summary>
		/// 내역을 엑셀로 출력합니다 
		/// </summary>
		/// <returns></returns>
		Task<MemoryStream> GetExcelDataFromIncome(string searchBy, string searchString, string fromDate, string toDate, string sum, string userId);



		/// <summary>
		/// 엑셀에 있는 내역을 DB에 저장합니다
		/// </summary>
		/// <param name="formFile"></param>
		/// <returns></returns>
		Task<int> UploadIncomeFromExcelFile(IFormFile formFile, string userId);


		/// <summary>
		/// 페이징하여 보여줍니다
		/// </summary>
		/// <param name="list"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		Task<PagingResponse<IncomeResponse>> GetPages(List<IncomeResponse> list, int page, int pageSize);

	}
}
