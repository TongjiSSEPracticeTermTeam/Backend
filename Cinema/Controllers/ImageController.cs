using Cinema.DTO;
using Cinema.Entities;
using Cinema.Helpers;
using Cinema.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using TencentCloud.Ecm.V20190719.Models;

namespace Cinema.Controllers
{
    /// <summary>
    /// 图片相关接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
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
        public ImageController(CinemaDb db, IHttpContextAccessor httpContextAccessor, JwtHelper jwtHelper, QCosSrvice qCosSrvice)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _jwtHelper = jwtHelper;
            _qCosSrvice = qCosSrvice;
        }

        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="imageName">【可选】意向图片名，留空使用GUID</param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Policy = "CinemaAdmin")]
        public async Task<IAPIResponse> UploadImage([FromForm] string prefix, [FromForm] string? imageName, [FromForm] IFormFile file)
        {
            return await ImageService.UploadImage(prefix, imageName, file, _qCosSrvice);
        }
    }
}
