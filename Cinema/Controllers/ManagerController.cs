using Cinema.DTO.ManagerService;
using Cinema.Entities;
using Cinema.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Controllers;

/// <summary>
/// 影院经理相关接口
/// </summary>
[Route("api/[controller]")]
[ApiController]
[ProducesResponseType(typeof(Manager),StatusCodes.Status200OK)]
public class ManagerController : ControllerBase
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
    public ManagerController(CinemaDb db, IHttpContextAccessor httpContextAccessor, JwtHelper jwtHelper)
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
        return new JsonResult(await _db.Managers.Include(m => m.ManagedCinema).FirstOrDefaultAsync(m => m.Id == name));
    }

    /// <summary>
    /// 登录
    /// </summary>
    [HttpPost("login")]
    [ProducesDefaultResponseType(typeof(ManagerLoginResponse))]
    public async Task<ManagerLoginResponse> ManagerLogin([FromBody] ManagerLoginRequest request)
    {
        if (request.Password == "" || request.UserName == "")
            return new ManagerLoginResponse
            {
                Status = "4001",
                Message = "用户名或密码为空"
            };

        var manager = await _db.Managers.FindAsync(request.UserName);
        if (manager == null)
            return new ManagerLoginResponse
            {
                Status = "4002",
                Message = "用户名或密码错误"
            };

        if (manager.Password == Md5Helper.CalculateMd5Hash(request.Password))
            return new ManagerLoginResponse
            {
                Status = "10000",
                Message = "登录成功",
                Token = _jwtHelper.GenerateToken(manager.Id, UserRole.CinemaAdmin),
                UserData = manager
            };
        return new ManagerLoginResponse
        {
            Status = "4002",
            Message = "用户名或密码错误"
        };
    }
}