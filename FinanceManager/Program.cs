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
	// Post method에만 AntiforgeryToken 활성화 
	options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});


builder.Services.AddHttpClient<IPlanApiService, PlanApiService>();


builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IPlanApiService, PlanApiService>();

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
	// 애플리케이션레이어 설정
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders()

	// 레퍼지토리레이어 설정
	.AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
	.AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();


builder.Services.AddAuthorization(options =>
{
	// 사용자는 로그인되어야 한다(접근 권한)
	options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

	// 로그인이 되지 않은 사용자는 [NotAuthorized]에는 접근 가능 
	options.AddPolicy("NotAuthorized", policy 
	=> {
		policy.RequireAssertion(context =>
		{
			return !context.User.Identity.IsAuthenticated;
		});	
	});
});			

// 쿠키가 제출되지 않았을 때 리다이렉션
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
app.UseAuthentication();	// 쿠키 확인을 위한 설정
app.UseAuthorization();		// 접근 권한 확인 / 로그인 안 되어있으면 redirection하기 위해 설정
app.MapControllers();		// filter pipeline 수행하는 것 

app.UseEndpoints(endpoints => { 
	endpoints.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}"); 
});

app.Run();
