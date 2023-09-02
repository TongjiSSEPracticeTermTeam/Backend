using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinema.DTO;
using Cinema.DTO.SessionService;
using Cinema.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        /// 获取排片详情，客户端接口
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="cinemaId"></param>
        /// <param name="hallId"></param>
        /// <param name="rawStartTime"></param>
        /// <returns></returns>
        [HttpGet("detail")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<Session>))]
        public async Task<IAPIResponse> GetSessionDetail([FromQuery] string movieId, [FromQuery] string cinemaId, [FromQuery] string hallId, [FromQuery] long rawStartTime)
        {
            var startTime = DateTime.UnixEpoch.AddSeconds(rawStartTime/1000).ToLocalTime();
            var session = await _db.Sessions
                .Include(s=>s.MovieBelongsTo)
                .Include(s => s.HallLocatedAt)
                .ThenInclude(s => s.CinemaBelongTo)
                .FirstOrDefaultAsync(s => s.MovieId == movieId && s.CinemaId == cinemaId && s.HallId == hallId && s.StartTime == startTime);
            if (session == null) 
                return APIResponse.Failaure("4001","找不到对应排片");
            return APIDataResponse<SessionDetailResponse>.Success(new SessionDetailResponse
            {
                Session = session,
                Movie = session.MovieBelongsTo
            });
        }
    }
}