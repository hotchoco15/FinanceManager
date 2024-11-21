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


builder.Services.AddEndpointsApiExplorer(); // Swagger가 메타데이타(Http method, URL)를 읽을 수 있게 하는 것
builder.Services.AddSwaggerGen();



builder.Services.AddCors(options => {
	options.AddDefaultPolicy(policyBuilder =>
	{
		policyBuilder.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>());
	});
});


builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));


var app = builder.Build();


// https 활성화 
app.UseHsts();
app.UseHttpsRedirection();


if (app.Environment.IsDevelopment())
{
	app.UseSwagger();           // swagger.json의 endpoint를 생성
	app.UseSwaggerUI();         // swagger UI 생성 
}

app.UseRouting();           // Route에 의해 action method를 확인하는 것
app.UseCors();


app.MapControllers();

app.Run();

