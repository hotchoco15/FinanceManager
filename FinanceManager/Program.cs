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
	// Post method���� AntiforgeryToken Ȱ��ȭ 
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
	options.Password.RequiredUniqueChars = 3;	// ��й�ȣ���� ���� ����(����)�� �ּ� ������� 
})
	// ���ø����̼Ƿ��̾� ����
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders()

	// �������丮���̾� ����
	.AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
	.AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();


builder.Services.AddAuthorization(options =>
{
	// ����ڴ� �α��εǾ�� �Ѵ�(���� ����)
	options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

	// �α����� ���� ���� ����ڴ� [NotAuthorized]���� ���� ���� 
	options.AddPolicy("NotAuthorized", policy 
	=> {
		policy.RequireAssertion(context =>
		{
			return !context.User.Identity.IsAuthenticated;
		});	
	});
});			

// ��Ű�� ������� �ʾ��� �� �����̷���
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
app.UseAuthentication();	// ��Ű Ȯ���� ���� ����
app.UseAuthorization();		// ���� ���� Ȯ�� / �α��� �� �Ǿ������� redirection�ϱ� ���� ����
app.MapControllers();		// filter pipeline �����ϴ� �� 

app.UseEndpoints(endpoints => { 
	endpoints.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}"); 
});

app.Run();
