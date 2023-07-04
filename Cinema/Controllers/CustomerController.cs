using Cinema.DTO.CustomerService;
using Cinema.Entities;
using Cinema.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController
{
    private readonly CinemaDb _db;

    public CustomerController(CinemaDb db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        return new JsonResult(await _db.Customers.ToListAsync());
    }

    [HttpPost("login")]
    public async Task<IActionResult> CustomerLogin(LoginRequest request)
    {
        if(request.Password=="" || request.UserName=="")
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
                UserData = customer
            });
        return new JsonResult(new LoginResponse
        {
            Status = "4002",
            Message = "用户名或密码错误"
        });
    }
}