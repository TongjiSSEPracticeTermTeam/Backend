using Cinema.DTO;
using Cinema.DTO.CustomerService;
using Cinema.DTO.MoviesService;
using Cinema.Entities;
using Cinema.Helpers;
using Cinema.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Validations.Rules;
using Swashbuckle.AspNetCore.SwaggerGen;
using TencentCloud.Tcss.V20201101.Models;
using TencentCloud.Tione.V20191022.Models;

namespace Cinema.Controllers;

/// <summary>
/// 顾客相关接口
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly CinemaDb _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtHelper _jwtHelper;


    private static int _customerId;

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
        //_logger = logger;
        //_qCosSrvice = qCosSrvice;


        if (_customerId==0)
        {
            _customerId = int.Parse(_db.Customers.Max(m => m.CustomerId) ?? "0");
        }
    }

    /// <summary>
    /// 获取已经登录的用户的信息（要求必须已经登录）。
    /// </summary>
    /// <returns>
    /// 用户名称
    /// </returns>
    /// <remarks>
    /// 前端调用方法：加上HTTP请求头：Authorization: Bearer [令牌]（所有需要身份验证的地方都这样调用）
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(Customer),StatusCodes.Status200OK)]
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
    public async Task<CustomerLoginResponse> CustomerLogin([FromBody] CustomerLoginRequest request)
    {
        if (request.Password == "" || request.Email == "")
            return new CustomerLoginResponse
            {
                Status = "4001",
                Message = "邮箱或密码为空"
            };
        var customer = await _db.Customers.FirstOrDefaultAsync(c=>c.Email==request.Email);
        if (customer == null)
            return new CustomerLoginResponse
            {
                Status = "4002",
                Message = "邮箱或密码错误"
            };

        if (customer.Password == Md5Helper.CalculateMd5Hash(request.Password))
            return new CustomerLoginResponse
            {
                Status = "10000",
                Message = "登录成功",
                Token = _jwtHelper.GenerateToken(customer.CustomerId, UserRole.User),
                UserData = customer
            };
        return new CustomerLoginResponse
        {
            Status = "4002",
            Message = "用户名或密码错误"
        };
    }

    /// <summary>
    /// 注册
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
    public async Task<IAPIResponse> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.Username))
            return new APIResponse
            {
                Status = "4001",
                Message = "用户名或密码为空"
            };

        if (string.IsNullOrWhiteSpace(request.Email))
            return new APIResponse
            {
                Status = "4003",
                Message = "邮箱不能为空"
            };


        try
        {
            if(await _db.Customers.FirstOrDefaultAsync(c => c.Email == request.Email) != null)
            {
                return new APIResponse
                {
                    Status = "4002",
                    Message = "邮箱已存在"
                };
            }
        } catch (Exception ex)
        {
            return new APIResponse
            {
                Status = "error",
                Message = ex.ToString()
            };
        };

            
        var customerId = Interlocked.Increment(ref _customerId);
        var customer = new Customer
        {
            CustomerId = String.Format("{0:000000}",customerId),
            Password = Md5Helper.CalculateMd5Hash(request.Password),
            Name = request.Username,
            Email = request.Email,
            AvatarUrl="/img/default_avatar.jpg"
        };

        await _db.Customers.AddAsync(customer);
        await _db.SaveChangesAsync();
        return new RegisterResponse
        {
            Status = "10000",
            Message = "注册成功",
            Token = _jwtHelper.GenerateToken(customer.CustomerId, UserRole.User),
            UserData = customer
        };
    }


    /// <summary>
    /// 用户端接口，修改个人信息
    /// </summary>
    /// <param name="customer"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesDefaultResponseType(typeof(APIResponse))]
    public async Task<IAPIResponse> EditCustomer([FromBody] CustomerDTO customer)
    {
        var customerEntity = await _db.Customers
            .Where(m => m.CustomerId == customer.CustomerId)
            .FirstOrDefaultAsync();


        if (customerEntity == null)
        {
            return APIResponse.Failaure("4000", "用户不存在");
        }

        
        if (customerEntity.Email != customer.Email && await _db.Customers.FirstOrDefaultAsync(c => c.Email == customer.Email) != null)
        {
            return new APIResponse
            {
                Status = "4002",
                Message = "邮箱已存在"
            };
        }

        customerEntity.Name = customer.Name;
        customerEntity.Email = customer.Email;
        customerEntity.AvatarUrl= customer.AvatarUrl;

        _db.SaveChanges();

        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            return APIResponse.Failaure("5000", "服务器内部错误");
        }

        //_db.Movies.Update(movieEntity);
        //await _db.SaveChangesAsync();
        return APIResponse.Success();
    }


    /// <summary>
    /// 用户端接口，修改密码
    /// </summary>
    /// <param name="customer"></param>
    /// <returns></returns>
    [HttpPost("changePwd")]
    [ProducesDefaultResponseType(typeof(APIResponse))]
    public async Task<IAPIResponse> EditPassword([FromBody] ChangePwdDTO data)
    {
        var customerEntity = await _db.Customers
            .Where(m => m.CustomerId == data.CustomerId)
            .FirstOrDefaultAsync();

        if (customerEntity == null) { 
            return APIResponse.Failaure("4000", "用户不存在"); 
        }

        if (customerEntity.Password != Md5Helper.CalculateMd5Hash(data.OldPwd))
        {
            return APIResponse.Failaure("4001", "原密码错误");
        }
        
        if(customerEntity.Password == Md5Helper.CalculateMd5Hash(data.NewPwd))
        {
            return APIResponse.Failaure("4002", "新密码不能与原密码相同");
        }

        customerEntity.Password = Md5Helper.CalculateMd5Hash(data.NewPwd);

        _db.SaveChanges();

        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            return APIResponse.Failaure("5000", "服务器内部错误");
        }
        return APIResponse.Success();
    }
}