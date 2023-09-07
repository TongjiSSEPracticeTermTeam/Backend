using Cinema.DTO;
using Cinema.DTO.CustomerService;
using Cinema.Entities;
using Cinema.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace Cinema.Controllers;

/// <summary>
/// 用户会员VIP相关接口
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class VipController : ControllerBase
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
    public VipController(CinemaDb db, IHttpContextAccessor httpContextAccessor, JwtHelper jwtHelper)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
        _jwtHelper = jwtHelper;
    }

    /// <summary>
    /// 返回对应用户的VIP信息
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [HttpGet("{customerId}")]
    //[Authorize(Policy = "Customer")]
    [ProducesDefaultResponseType(typeof(APIDataResponse<VipInfo?>))]
    public async Task<IAPIResponse> GetVipinfoByCustomerId(string customerId)
    {
        var customer = await _db.Customers
            .Include(c => c.Vip)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (customer == null)
        {
            return APIResponse.Failaure("4001", "用户不存在");
        }

        return APIDataResponse<VipInfo?>.Success(customer.Vip);
    }

    /// <summary>
    /// 修改对应ID用户的VIP结束时间
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="endTime"></param>
    /// <returns></returns>
    [HttpPut]
    //[Authorize(Policy = "Customer")]
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

    /// <summary>
    /// 根据模式修改对应用户的会员到期时间
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    [HttpPut("byMode")]
    //[Authorize(Policy = "Customer")]
    [ProducesDefaultResponseType(typeof(APIResponse))]
    public async Task<IAPIResponse> UpdateVipBymode([FromQuery] string customerId, [FromQuery] int mode)
    {
        var customer = await _db.Customers.FindAsync(customerId);

        if (customer == null)
        {
            return APIResponse.Failaure("10001", "用户不存在");
        }

        DateTime endTime = DateTime.Now;
        var vipInfo = await _db.VipInfos.FindAsync(customerId);
        
        if (vipInfo != null)
        {
            //已经过期则是续费
            if (vipInfo.EndDate.CompareTo(DateTime.Now) < 0)
                endTime = DateTime.Now;
            //否则为开通
            else
                endTime = vipInfo.EndDate;
        }

        switch (mode)
        { 
            case 1 :
                endTime = endTime.AddMonths(1);
                break;
            case 2 :
                endTime = endTime.AddMonths(3);
                break;
            case 3 :
                endTime = endTime.AddYears(1);
                break;
            default :
                return APIResponse.Failaure("4001", "模式错误");
        }


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