using Entities;
using Entities.IdentityEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ServiceContracts.Enums;
using System.Runtime.Intrinsics.X86;

namespace FinanceManager.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class HomeController : Controller
	{
        private readonly RoleManager<ApplicationRole> _roleManager;

		private readonly UserManager<ApplicationUser> _userManager;

		private readonly ApplicationDbContext _db;

		public HomeController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager,
			ApplicationDbContext applicationDbContext)
		{
			_roleManager = roleManager;
			_userManager = userManager;
			_db = applicationDbContext;
		}

		[HttpGet]
        [Route("~/Home/Index")]
		public async Task<IActionResult> Index()
		{
			var allUsers = _db.Users.ToList();
			var users = new List<ApplicationUser>();

			foreach (var user in allUsers)
			{ 
				if (!(await _userManager.IsInRoleAsync(user, "admin")))
				{
					users.Add(user);	
				}
			}

			
			return View(users);
		}

		[HttpPost]
		[Route("~/Home/Index")]
		public async Task<IActionResult> Index(IFormCollection form)
		{
			List<string> emails = new List<string>();	

			if (form.Count > 0) 
			{
				for (int i = 0; i < form.Count - 1 ; i++)
				{
					emails = form.Keys.Where(e => e.Contains("@")).ToList();
				}

				foreach (var email in emails)
				{
					var user = await _userManager.FindByEmailAsync(email);


					if (user != null)
					{
						await _userManager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());
						
					}
				}
			}

			return RedirectToAction("Index"); 
		}
	}
}
