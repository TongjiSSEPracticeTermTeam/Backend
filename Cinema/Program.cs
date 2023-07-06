using System.Security.Claims;
using System.Text;
using Cinema.Entities;
using Cinema.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

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
    policy.AddPolicy("CorsPolicy", policyBuilder => policyBuilder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

// 配置数据库
if (Environment.GetEnvironmentVariable("CINEMA_DATABASE") == null)
    throw new InvalidOperationException("请配置Oracle连接信息");
var oracleConnectionString = Environment.GetEnvironmentVariable("CINEMA_DATABASE")!.TrimEnd('\r', '\n');
Console.WriteLine(oracleConnectionString);
builder.Services.AddDbContext<CinemaDb>(options =>
    options.UseOracle(oracleConnectionString));

// 配置JWT
if (configuration["Jwt:Issuer"] == null)
    Console.WriteLine("注意：您没有配置Jwt相关信息。程序会使用默认的配置，但这是极不安全的。");
builder.Services.AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = configuration["Jwt:Issuer"] ?? "SampleIssuer",
            ValidateAudience = true,
            ValidAudience = configuration["Jwt:Audience"] ?? "SampleAudience",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration["Jwt:Key"] ?? "SampleKey")),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            RequireExpirationTime = true
        };
    });
builder.Services.AddSingleton(new JwtHelper(configuration));

// 配置角色和授权
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RegUser", policy => policy.RequireClaim(ClaimTypes.Name));
    options.AddPolicy("Customer",
        policy => policy.RequireClaim(ClaimTypes.Role, UserRole.User.ToString(), UserRole.CinemaAdmin.ToString(),
            UserRole.SysAdmin.ToString()));
    options.AddPolicy("CinemaAdmin",
        policy => policy.RequireClaim(ClaimTypes.Role, UserRole.CinemaAdmin.ToString(), UserRole.SysAdmin.ToString()));
    options.AddPolicy("SysAdmin", policy => policy.RequireClaim(ClaimTypes.Role, UserRole.SysAdmin.ToString()));
});

// 注入HTTP上下文
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("CorsPolicy");
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/dist"))
});
app.UseStatusCodePages(new StatusCodePagesOptions
{
    HandleAsync = context =>
    {
        var response = context.HttpContext.Response;
        if (response.StatusCode == 404) response.Redirect("/index.html");
        return Task.CompletedTask;
    }
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();