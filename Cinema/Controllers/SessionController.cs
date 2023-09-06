using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinema.DTO;
using Cinema.DTO.SessionService;
using Cinema.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TencentCloud.Mna.V20210119.Models;

namespace Namespace
{
    /// <summary>
    /// 排片控制器类
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private readonly CinemaDb _db;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="db"></param>
        public SessionController(CinemaDb db)
        {
            _db = db;
        }

        /// <summary>
        /// 根据电影ID获取排片，客户端接口
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        [HttpGet("movie/{movieId}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<Dictionary<DateTime, List<Session>>>))]
        public async Task<IAPIResponse> GetByMovieId([FromRoute] string movieId)
        {
            var sessions = await _db.Sessions
                                .Include(s => s.HallLocatedAt)
                                .ThenInclude(s => s.CinemaBelongTo)
                                .Where(s => s.StartTime > DateTime.Now)
                                .ToListAsync();

            // 根据session.StartTime对sessions分类
            var groupedSessions =
                from session in sessions
                group session by session.StartTime.Date into g
                orderby g.Key ascending
                select g;

            return APIDataResponse<Dictionary<DateTime, List<Session>>>.Success(groupedSessions.ToDictionary(g=>g.Key,g=>g.ToList()));
        }

        /// <summary>
        /// 尝试在某个影厅的某个时间点排片
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> PutSession([FromBody] SessionDTO data)
        {
            // 先判断影厅是否存在
            var hall = await _db.Halls.FirstOrDefaultAsync(h => h.Id == data.HallId && h.CinemaId == data.CinemaId);
            if (hall == null)
                return APIResponse.Failaure("40001","影厅不存在");
            // 之后判断电影是否存在
            var movie = await _db.Movies.FirstOrDefaultAsync(m => m.MovieId == data.MovieId);
            if (movie == null)
                return APIResponse.Failaure("40002", "电影不存在");
            // 再判断拍片时间是否在电影上下映之间
            if (data.StartTime < movie.ReleaseDate || data.StartTime > movie.RemovalDate)
                return APIResponse.Failaure("40003", "排片时间不在电影上映时间内");
            //最后判断拍片时间是否已经被占用
            var existed = await _db.Sessions
                .AnyAsync(s => s.HallId == data.HallId
               && s.CinemaId == data.CinemaId
               && s.StartTime == data.StartTime);
            if (existed)
                return APIResponse.Failaure("40004", "该时间段已被排片");
            // 如果都没有问题，就创建排片

        }
    }
}