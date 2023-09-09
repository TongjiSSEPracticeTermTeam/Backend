using Cinema.DTO;
using Cinema.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Controllers
{
    /// <summary>
    /// 头图控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HeaderImageController : ControllerBase
    {
        private readonly CinemaDb _db;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="db"></param>
        public HeaderImageController(CinemaDb db)
        {
            _db = db;
        }

        /// <summary>
        /// 获取所有头图信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<HeaderImage>>))]
        public async Task<IAPIResponse> GetAll()
        {
            return APIDataResponse<List<HeaderImage>>.Success(await _db.HeaderImages.ToListAsync());
        }

        /// <summary>
        /// 添加头图
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ProducesDefaultResponseType(typeof(APIDataResponse<HeaderImage>))]
        public async Task<IAPIResponse> Add(HeaderImageDTO headerImage)
        {
            try
            {
                var headerImageEntity = new HeaderImage
                {
                    Url = headerImage.Url,
                    MovieId = headerImage.MovieId,
                };
                await _db.AddAsync(headerImageEntity);
                await _db.SaveChangesAsync();
                return APIDataResponse<HeaderImage>.Success(headerImageEntity);
            }
            catch
            {
                return APIResponse.Failaure("4000", "电影不存在或出现系统错误");
            }
        }

        /// <summary>
        /// 删除头图
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> Delete([FromQuery] int headerImageId)
        {
            try
            {
                var headerImageEntity = await _db.HeaderImages.FindAsync(headerImageId) ?? throw new NullReferenceException();
                _db.HeaderImages.Remove(headerImageEntity);
                await _db.SaveChangesAsync();
                return APIResponse.Success();
            }
            catch
            {
                return APIResponse.Failaure("4000", "电影头图不存在或出现系统错误");
            }
        }

        /// <summary>
        /// 获取包含电影信息的返回数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("detail")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<HeaderImage>>))]
        public async Task<IAPIResponse> GetWithDetail()
        {
            return APIDataResponse<List<HeaderImage>>.Success(await _db.HeaderImages.Include(h=>h.Movie).ToListAsync());
        }
    }
}
