using ServiceContracts;
using Services;
using Entities;
using Microsoft.EntityFrameworkCore;
using Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews(options =>
{
	options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});


//add services into IOC container
builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();

builder.Services.AddDbContext<ApplicationDbContext>
(opitons => {
	opitons.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection"),
		sqlOptions => sqlOptions.MigrationsAssembly("FinanceManager")
	);
});



builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
	options.Password.RequiredLength = 8;
	options.Password.RequireUppercase = false;
	options.Password.RequireLowercase = true;
	options.Password.RequiredUniqueChars = 3;	// 비밀번호에서 같은 문자(숫자)가 최소 몇가지인지 
})
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders()
	.AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
	.AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();


builder.Services.AddAuthorization(options =>
{
	options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

	options.AddPolicy("NotAuthorized", policy 
	=> {
		policy.RequireAssertion(context =>
		{
			return !context.User.Identity.IsAuthenticated;
		});	
	});
});			

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Account/Login";
});


var app = builder.Build();


// https 활성화 
app.UseHsts();
app.UseHttpsRedirection();


app.UseStaticFiles();

app.UseRouting();			// Route에 의해 action method를 확인하는 것
app.UseAuthentication();
app.UseAuthorization();		// 로그인 안 되어있으면 redirection하기 위해 설정
app.MapControllers();		// filter pipeline 수행하는 것 

app.UseEndpoints(endpoints => { 
	endpoints.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}"); 
});

app.Run();
