using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
	public class PagingResponse<T>
	{
		public IEnumerable<T> Data { get; set; }

		public int TotalPage { get; set; }

	}
}
