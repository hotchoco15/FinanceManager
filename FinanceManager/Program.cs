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
	options.Password.RequiredUniqueChars = 3;	// ��й�ȣ���� ���� ����(����)�� �ּ� ������� 
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


// https Ȱ��ȭ 
app.UseHsts();
app.UseHttpsRedirection();


app.UseStaticFiles();

app.UseRouting();			// Route�� ���� action method�� Ȯ���ϴ� ��
app.UseAuthentication();
app.UseAuthorization();		// �α��� �� �Ǿ������� redirection�ϱ� ���� ����
app.MapControllers();		// filter pipeline �����ϴ� �� 

app.UseEndpoints(endpoints => { 
	endpoints.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}"); 
});

app.Run();
