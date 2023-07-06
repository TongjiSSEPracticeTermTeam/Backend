using Cinema.DTO.CustomerService;
using Cinema.Entities;
using Cinema.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController
{
    private readonly CinemaDb _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtHelper _jwtHelper;

    public CustomerController(CinemaDb db, IHttpContextAccessor httpContextAccessor, JwtHelper jwtHelper)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
        _jwtHelper = jwtHelper;
    }

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

    [HttpPost("login")]
    public async Task<IActionResult> CustomerLogin(LoginRequest request)
    {
        if (request.Password == "" || request.UserName == "")
            return new JsonResult(new LoginResponse
            {
                Status = "4001",
                Message = "用户名或密码为空"
            });

        var customer = await _db.Customers.FindAsync(request.UserName);
        if (customer == null)
            return new JsonResult(new LoginResponse
            {
                Status = "4002",
                Message = "用户名或密码错误"
            });

        if (customer.Password == Md5Helper.CalculateMd5Hash(request.Password))
            return new JsonResult(new LoginResponse
            {
                Status = "10000",
                Message = "登录成功",
                Token = _jwtHelper.GenerateToken(customer.CustomerId, UserRole.User),
                UserData = customer
            });
        return new JsonResult(new LoginResponse
        {
            Status = "4002",
            Message = "用户名或密码错误"
        });
    }

    [HttpPut]
    public IActionResult register()
    {
        return new OkObjectResult("您正在尝试注册");
    }
}