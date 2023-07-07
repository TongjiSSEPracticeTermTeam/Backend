using Cinema.DTO.CustomerService;
using Cinema.Entities;
using Cinema.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations.Rules;

namespace Cinema.Controllers;

/// <summary>
/// 顾客相关接口
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CustomerController
{
    private readonly CinemaDb _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtHelper _jwtHelper;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="db"></param>
    /// <param name="httpContextAccessor"></param>
    /// <param name="jwtHelper"></param>
    public CustomerController(CinemaDb db, IHttpContextAccessor httpContextAccessor, JwtHelper jwtHelper)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
        _jwtHelper = jwtHelper;
    }

    /// <summary>
    /// 获取已经登录的用户的信息（要求必须已经登录）。
    /// 
    /// 前端调用方法：加上HTTP请求头：Authorization: Bearer [令牌]（所有需要身份验证的地方都这样调用）
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "RegUser")]
    public async Task<IActionResult> GetSelfInfo()
    {
        var name = JwtHelper.SolveName(_httpContextAccessor);
        if (name == null)
        {
            return new UnauthorizedResult();
        }

        return new JsonResult(await _db.Customers.FindAsync(name));
    }

    /// <summary>
    /// 登录
    /// </summary>
    [HttpPost("login")]
    public async Task<LoginResponse> CustomerLogin(LoginRequest request)
    {
        if (request.Password == "" || request.UserName == "")
            return new LoginResponse
            {
                Status = "4001",
                Message = "用户名或密码为空"
            };

        var customer = await _db.Customers.FindAsync(request.UserName);
        if (customer == null)
            return new LoginResponse
            {
                Status = "4002",
                Message = "用户名或密码错误"
            };

        if (customer.Password == Md5Helper.CalculateMd5Hash(request.Password))
            return new LoginResponse
            {
                Status = "10000",
                Message = "登录成功",
                Token = _jwtHelper.GenerateToken(customer.CustomerId, UserRole.User),
                UserData = customer
            };
        return new LoginResponse
        {
            Status = "4002",
            Message = "用户名或密码错误"
        };
    }

    /// <summary>
    /// 注册
    /// </summary>
    [HttpPut]
    public IActionResult register()
    {
        return new OkObjectResult("您正在尝试注册");
    }
}