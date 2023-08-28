using Cinema.DTO;
using Cinema.DTO.UserGeneralService;
using Cinema.Entities;
using Cinema.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Controllers
{
    /// <summary>
    /// 通用接口
    /// </summary>
    [Route("api/user")]
    [ApiController]
    public class UserGeneralController : ControllerBase
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
        public UserGeneralController(CinemaDb db, IHttpContextAccessor httpContextAccessor, JwtHelper jwtHelper)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// 获取当前登录的用户的信息
        /// </summary>
        /// <returns>用户信息</returns>
        /// <remarks>
        /// 如果用户没有登录，或者登录过期，会返回401。
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
        [Authorize(Policy = "RegUser")]
        public async Task<IAPIResponse> getUserCurrentLoggedin()
        {
            var name = JwtHelper.SolveName(_httpContextAccessor);
            var roleByString = JwtHelper.SolveRole(_httpContextAccessor);
            if (name == null || roleByString == null)
            {
                return new APIResponse
                {
                    Status = "4000",
                    Message = "系统错误"
                };
            }

            var role = (UserRole)Enum.Parse(typeof(UserRole), roleByString);
            switch (role)
            {
                case UserRole.User:
                    var user = await _db.Customers.FindAsync(name);
                    if (user == null)
                    {
                        return new APIResponse
                        {
                            Status = "4000",
                            Message = "系统错误"
                        };
                    }
                    return new CurrentUserResponse
                    {
                        Status = "10000",
                        Message = "成功",
                        Type = "Customer",
                        Username = user.CustomerId,
                        DisplayName = user.Name,
                        Avatar = user.AvatarUrl
                    };
                case UserRole.CinemaAdmin:
                    var cinemaAdmin = await _db.Managers.FindAsync(name);
                    if (cinemaAdmin == null)
                    {
                        return new APIResponse
                        {
                            Status = "4000",
                            Message = "系统错误"
                        };
                    }
                    return new CurrentUserResponse
                    {
                        Status = "10000",
                        Message = "成功",
                        Type = "CinemaAdmin",
                        Username = cinemaAdmin.Id,
                        DisplayName = cinemaAdmin.Name,
                        Avatar = cinemaAdmin.AvatarUrl
                    };
                case UserRole.SysAdmin:
                    var admin = await _db.Administrators.FindAsync(name);
                    if (admin == null)
                    {
                        return new APIResponse
                        {
                            Status = "4000",
                            Message = "系统错误"
                        };
                    }
                    return new CurrentUserResponse
                    {
                        Status = "10000",
                        Message = "成功",
                        Type = "SysAdmin",
                        Username = admin.Id,
                        Avatar = admin.AvatarUrl,
                        DisplayName = $"系统管理员{admin.Id}"
                    };
                default:
                    return new APIResponse
                    {
                        Status = "4000",
                        Message = "系统错误"
                    };
            }
        }
    }
}
