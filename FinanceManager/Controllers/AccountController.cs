using Entities.IdentityEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

namespace FinanceManager.Controllers
{
	[Route("[controller]")]
	[AllowAnonymous]
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;

		private readonly SignInManager<ApplicationUser> _signInManager;

		private readonly RoleManager<ApplicationRole> _roleManager;

		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
			RoleManager<ApplicationRole> roleManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}

		[Route("[action]")]
		[HttpGet]
		[Authorize("NotAuthorized")]
		public IActionResult Register()
		{
			return View();
		}

		[Route("[action]")]
		[HttpPost]
        [Authorize("NotAuthorized")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
		{
			if (ModelState.IsValid == false)
			{
				ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(temp => temp.ErrorMessage);
				return View(registerDTO);
			}

			ApplicationUser user = new ApplicationUser()
			{
				Email = registerDTO.Email,
				UserName = registerDTO.Email,
				PersonName = registerDTO.PersonName
			};

			IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);
			if (result.Succeeded)
			{
				if (registerDTO.UserType == ServiceContracts.Enums.UserTypeOptions.Admin)
				{
					if (await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
					{
						ApplicationRole applicationRole = new ApplicationRole()
						{ Name = UserTypeOptions.Admin.ToString() };
						await _roleManager.CreateAsync(applicationRole);
					}

					await _userManager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());
				}
				else
				{
					await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());
				}

				await _signInManager.SignInAsync(user, isPersistent: false);


				return RedirectToAction("Index", "Income");
			}
			else
			{
				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError("Register", error.Description);
				}

				return View(registerDTO);
			}
		}

		[Route("[action]")]
		[Route("/")]
		[HttpGet]
        [Authorize("NotAuthorized")]
        public IActionResult Login()
		{
			return View();
		}

		
		[Route("[action]")]
		[HttpPost]
        [Authorize("NotAuthorized")]
        public async Task<IActionResult> Login(LoginDTO loginDTO, string? ReturnUrl)
		{
			if (ModelState.IsValid == false)
			{
				ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
				return View(loginDTO);
			}

			var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);

			if (result.Succeeded)
			{
				ApplicationUser user = await _userManager.FindByEmailAsync(loginDTO.Email);

				if (user != null) 
				{
					if (await _userManager.IsInRoleAsync(user, UserTypeOptions.Admin.ToString()))
					{
						return RedirectToAction("Index", "Home", new { area = "Admin"});	
					}
				}

				if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl)) 
				{
					return LocalRedirect(ReturnUrl);
				}
				return RedirectToAction("Index", "Income");
			}

			ModelState.AddModelError("Login", "이메일이나 비밀번호가 맞지 않습니다");

			return View(loginDTO);
		}

		[Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Login", "Account");
		}

		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> IsEmailRegistered(string email)
		{
			ApplicationUser user = await _userManager.FindByEmailAsync(email);
			if (user == null) 
			{
				return Json(true);
			}
			else
			{
				return Json(false);
			}
		}
	}
}
