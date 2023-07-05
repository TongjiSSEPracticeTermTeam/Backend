using Cinema.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 配置跨域
builder.Services.AddCors(policy =>
{
    policy.AddPolicy("CorsPolicy", policyBuilder =>policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// 配置数据库
if (configuration.GetConnectionString("Oracle") == null)
    throw new InvalidOperationException("请配置Oracle连接信息");
var oracleConnectionString = configuration.GetConnectionString("Oracle")!;
builder.Services.AddDbContext<CinemaDb>(options =>
    options.UseOracle(oracleConnectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("CorsPolicy");
}
app.UseStaticFiles();
app.UseStatusCodePages(options: new StatusCodePagesOptions
{
    HandleAsync = context =>
    {
        var response = context.HttpContext.Response;
        if (response.StatusCode == 404)
        {
            response.Redirect("/index.html");
        }
        return Task.CompletedTask;
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();