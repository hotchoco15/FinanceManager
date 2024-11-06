using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTO
{
	public class LoginDTO
	{
		[Required(ErrorMessage = "이메일은 필수입력입니다")]
		[EmailAddress(ErrorMessage = "맞는 이메일 형식으로 입력해주세요")]
		[DataType(DataType.EmailAddress)]
		public string? Email { get; set; }

		[Required(ErrorMessage = "비밀번호는 필수입력입니다")]
		[DataType(DataType.Password)]
		public string? Password { get; set; }
	}
}
