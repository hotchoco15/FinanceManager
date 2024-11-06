using Microsoft.AspNetCore.Mvc;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ServiceContracts.DTO
{
	public class RegisterDTO
	{
		[Required(ErrorMessage = "이름은 필수입력입니다")]
		public string PersonName { get; set; }

		[Required(ErrorMessage = "이메일은 필수입력입니다")]
		[EmailAddress(ErrorMessage = "입력하신 이메일 형식이 맞지 않습니다")]
		[Remote(action: "IsEmailRegistered", controller: "Account", ErrorMessage = "이미 사용중인 이메일입니다")]
		public string Email { get; set; }

		[Required(ErrorMessage = "비밀번호는 필수입력입니다")]
		[DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$",
		ErrorMessage = "비밀번호는 8자리 이상으로 소문자, 숫자, 특수문자를 포함해야 합니다")]
        public string Password { get; set; }

		[Required(ErrorMessage = "비밀번호 재입력은 필수입력입니다")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage ="비밀번호가 일치하지 않습니다")]
		public string ConfirmPassword { get; set; }



		public UserTypeOptions UserType { get; set; } = UserTypeOptions.User;

	}
}
