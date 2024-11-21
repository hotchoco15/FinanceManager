using Azure;
using Newtonsoft.Json;
using ServiceContracts;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
	public class PlanApiService : IPlanApiService
	{
		private readonly HttpClient _httpClient; 

		public PlanApiService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task AddPlan(PlanDTO planDto)
		{
			try
			{
				var stringContent = new StringContent(JsonConvert.SerializeObject(planDto), Encoding.UTF8, "application/json");
				var httpResponseMessage = await _httpClient.PostAsync("https://localhost:5010/api/v1/plan", stringContent);
				httpResponseMessage.EnsureSuccessStatusCode();
				
			}

			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw;
			}
		}
		


		public async Task<PagingResponse<PlanDTO>> GetPlans(int page, int pageSize)
		{
			try
			{
				var httpResponseMessage = await _httpClient.GetAsync($"https://localhost:5010/api/v1/plan?page={page}&pageSize={pageSize}");
				httpResponseMessage.EnsureSuccessStatusCode();
				return await httpResponseMessage.Content.ReadFromJsonAsync<PagingResponse<PlanDTO>>();

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw;
			}
		}
	}
}
