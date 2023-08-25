using Cinema.DTO;
using Cinema.DTO.AvatarService;
using Cinema.Entities;
using Cinema.Helpers;
using Cinema.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Buffers;

namespace Cinema.Controllers
{
    /// <summary>
    /// 头像相关控制器类
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AvatarController : ControllerBase
    {
        private readonly CinemaDb _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtHelper _jwtHelper;
        private readonly QCosSrvice _qCosSrvice;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="db"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="jwtHelper"></param>
        /// <param name="qCosSrvice"></param>
        public AvatarController(CinemaDb db, IHttpContextAccessor httpContextAccessor, JwtHelper jwtHelper, QCosSrvice qCosSrvice)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _jwtHelper = jwtHelper;
            _qCosSrvice = qCosSrvice;
        }

        /// <summary>
        /// 获取当前已登录用户的头像
        /// </summary>
        /// <returns>头像URL</returns>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(AvatarResponse))]
        [Authorize(Policy = "RegUser")]
        public async Task<IAPIResponse> GetMyAvatar()
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
            string? avatar;
            switch (role)
            {
                case UserRole.User:
                    avatar = (await _db.Customers.FindAsync(name))?.AvatarUrl;
                    break;
                case UserRole.SysAdmin:
                    avatar = (await _db.Administrators.FindAsync(name))?.AvatarUrl;
                    break;
                case UserRole.CinemaAdmin:
                    avatar = (await _db.Managers.FindAsync(name))?.AvatarUrl;
                    break;
                default:
                    return new APIResponse
                    {
                        Status = "4000",
                        Message = "系统错误"
                    };
            }
            if(avatar== null)
            {
                return new AvatarResponse
                {
                    Status = "10000",
                    Message = "成功",
                    HasAvatar = false,
                };
            }
            else
            {
                return new AvatarResponse
                {
                    Status = "10000",
                    Message = "成功",
                    HasAvatar = true,
                    Url = avatar
                };
            }
        }

        /// <summary>
        /// 获取任意用户的头像
        /// </summary>
        /// <param name="Username">用户名</param>
        /// <returns>头像URL</returns>
        [HttpGet("{Username}")]
        [ProducesDefaultResponseType(typeof(AvatarResponse))]
        public async Task<IAPIResponse> GetUserAvatar([FromRoute] string Username)
        {
            string? avatarUrl;
            avatarUrl = (await _db.Customers.FindAsync(Username))?.AvatarUrl;
            if (avatarUrl != null) 
            {
                return new AvatarResponse
                {
                    Status = "10000",
                    Message = "成功",
                    HasAvatar = true,
                    Url = avatarUrl
                };
            }
            avatarUrl = (await _db.Managers.FindAsync(Username))?.AvatarUrl;
            if (avatarUrl != null)
            {
                return new AvatarResponse
                {
                    Status = "10000",
                    Message = "成功",
                    HasAvatar = true,
                    Url = avatarUrl
                };
            }
            avatarUrl = (await _db.Administrators.FindAsync(Username))?.AvatarUrl;
            if (avatarUrl != null)
            {
                return new AvatarResponse
                {
                    Status = "10000",
                    Message = "成功",
                    HasAvatar = true,
                    Url = avatarUrl
                };
            }
            else
            {
                return new AvatarResponse
                {
                    Status = "10000",
                    Message = "成功",
                    HasAvatar = false,
                };
            }
        }

        /// <summary>
        /// 上传新头像
        /// </summary>
        /// <returns>新头像的URL</returns>
        /// <remarks>文件从请求体中上传</remarks>
        [HttpPut]
        [Authorize(Policy = "RegUser")]
        [ProducesDefaultResponseType(typeof(AvatarResponse))]
        public async Task<IAPIResponse> ChangeAvatar(IFormFile file)
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

            if(file==null || file.Length==0)
            {
                return new APIResponse
                {
                    Status = "4002",
                    Message = "未上传文件"
                };
            }

            // Save to temp file
            var tempFileName = Path.GetTempFileName();
            await using (var tempFile = new FileStream(tempFileName, FileMode.Create))
            {
                await file.CopyToAsync(tempFile);
            }

            var isValidImage = PreprocessImage(tempFileName);
            if (!isValidImage)
            {
                return new APIResponse
                {
                    Status = "4000",
                    Message = "不是有效的图片"
                };
            }

            var avatarPath = String.Format("userdata/avatar/{0}.jpg", name);
            string? avatarUrl;
            try
            {
                avatarUrl = await _qCosSrvice.UploadFile(avatarPath, tempFileName);
            }
            catch
            {
                System.IO.File.Delete(tempFileName);
                return new APIResponse
                {
                    Status = "4000",
                    Message = "上传头像失败（内部错误）"
                };
            }
            System.IO.File.Delete(tempFileName);

            var role = (UserRole)Enum.Parse(typeof(UserRole), roleByString);
            switch (role)
            {
                case UserRole.User:
                    var customer = await _db.Customers.FindAsync(name);
                    if (customer == null)
                    {
                        return new APIResponse
                        {
                            Status = "4001",
                            Message = "系统错误"
                        };
                    }
                    customer.AvatarUrl = avatarUrl;
                    await _db.SaveChangesAsync();
                    break;
                case UserRole.CinemaAdmin:
                    var manager = await _db.Managers.FindAsync(name);
                    if (manager == null)
                    {
                        return new APIResponse
                        {
                            Status = "4001",
                            Message = "系统错误"
                        };
                    }
                    manager.AvatarUrl = avatarUrl;
                    await _db.SaveChangesAsync();
                    break;  
                case UserRole.SysAdmin:
                    var admin = await _db.Administrators.FindAsync(name);
                    if (admin == null)
                    {
                        return new APIResponse
                        {
                            Status = "4001",
                            Message = "系统错误"
                        };
                    }
                    admin.AvatarUrl = avatarUrl;
                    await _db.SaveChangesAsync();
                    break;
            }

            return new AvatarResponse
            {
                Status = "10000",
                Message = "成功",
                HasAvatar = true,
                Url = avatarUrl
            };
        }

        private static bool PreprocessImage(string fileName)
        {
            try
            {
                using var image = Image.Load(fileName);
                image.Save(fileName, new JpegEncoder() { 
                    Quality = 80
                });
                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}
