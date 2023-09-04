using Cinema.DTO;
using Cinema.DTO.CustomerService;
using Cinema.Entities;
using Cinema.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations.Rules;
using Swashbuckle.AspNetCore.SwaggerGen;
using TencentCloud.Tcss.V20201101.Models;

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

        if(_customerId==0)
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
            Email = request.Email
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
    /// 根据用户ID获取所有订单
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("GetAllTickets/{id}")]
    //[Authorize(Policy = "RegUser")]
    [ProducesDefaultResponseType(typeof(APIDataResponse<List<Ticket>>))]
    public async Task<IAPIResponse> GetAllTicket([FromRoute] string id)
    {
        var custormer = await _db.Customers
            .Where(c => c.CustomerId == id)
            .Include(c => c.Tickets)
            .FirstOrDefaultAsync();

        if (custormer == null)
        {
            return APIResponse.Failaure("4001", "用户不存在");
        }

        return APIDataResponse<List<Ticket>>.Success(custormer.Tickets.ToList());
    }

    /// <summary>
    /// 修改对应ID用户的VIP结束时间
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    [HttpPost("updateVIP")]
    //[Authorize(Policy = "RegUser")]
    [ProducesDefaultResponseType(typeof(APIResponse))]
    public async Task<IAPIResponse> UpdateVIP([FromQuery] string customerId, [FromQuery] DateTime endTime)
    {
        var customer = await _db.Customers.FindAsync(customerId);

        if (customer == null)
        {
            return APIResponse.Failaure("10001", "用户不存在");
        }

        var vipInfo = await _db.VipInfos.FindAsync(customerId);

        //如果未有过vip记录则增加，否则进行修改
        if (vipInfo == null)
        {
            var nVipInfo = new VipInfo
            {
                CustomerId = customerId,
                EndDate = endTime
            };
            await _db.VipInfos.AddAsync(nVipInfo);
        }
        else
        {
            vipInfo.EndDate = endTime;
            _db.VipInfos.Update(vipInfo);
        }

        await _db.SaveChangesAsync();
        return APIResponse.Success();
    }
}