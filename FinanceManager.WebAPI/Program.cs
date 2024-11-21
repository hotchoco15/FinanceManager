using Asp.Versioning;
using Entities;
using FinanceManager.WebAPI.Mappings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(options => {
	options.Filters.Add(new ProducesAttribute("application/json"));
	options.Filters.Add(new ConsumesAttribute("application/json"));
});

builder.Services.AddApiVersioning(config =>
{
	//config.ApiVersionReader = new UrlSegmentApiVersionReader();
	config.DefaultApiVersion = new ApiVersion(1, 0);
	config.AssumeDefaultVersionWhenUnspecified = true;
	config.ReportApiVersions = true;

}).AddApiExplorer(options =>
{
	options.GroupNameFormat = "'v'VV";          // v10
	options.SubstituteApiVersionInUrl = true;
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});


builder.Services.AddEndpointsApiExplorer(); // Swagger�� ��Ÿ����Ÿ(Http method, URL)�� ���� �� �ְ� �ϴ� ��
builder.Services.AddSwaggerGen();



builder.Services.AddCors(options => {
	options.AddDefaultPolicy(policyBuilder =>
	{
		policyBuilder.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>());
	});
});


builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));


var app = builder.Build();


// https Ȱ��ȭ 
app.UseHsts();
app.UseHttpsRedirection();


if (app.Environment.IsDevelopment())
{
	app.UseSwagger();           // swagger.json�� endpoint�� ����
	app.UseSwaggerUI();         // swagger UI ���� 
}

app.UseRouting();           // Route�� ���� action method�� Ȯ���ϴ� ��
app.UseCors();


app.MapControllers();

app.Run();

