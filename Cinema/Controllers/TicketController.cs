using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinema.DTO;
using Cinema.Entities;
using Cinema.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema
{
    /// <summary>
    /// 电影票控制器类
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly CinemaDb _db;
        private readonly IHttpContextAccessor _contextAccessor;

        private static int _ticketId;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="db"></param>
        /// <param name="contextAccessor"></param>
        public TicketController(CinemaDb db, IHttpContextAccessor contextAccessor)
        {
            _db = db;
            _contextAccessor = contextAccessor;

            if (_ticketId == 0)
            {
                _ticketId = int.Parse(_db.Tickets.Max(t => t.Id) ?? "0");
            }
        }

        /// <summary>
        /// 获取所有电影票
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Policy = "CinemaAdmin")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// 获取特定电影票
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 本接口包含鉴权：普通客户只能查看自己的电影票，管理和经理随意
        /// </remarks>
        [HttpGet("{id}")]
        [Authorize(Policy = "RegUser")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// 获取某场排片中已售出的电影票
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="cinemaId"></param>
        /// <param name="hallId"></param>
        /// <param name="rawStartTime"></param>
        /// <returns></returns>
        [HttpGet("session")]
        [Authorize(Policy = "RegUser")]
        [ProducesDefaultResponseType(typeof(List<Ticket>))]
        public async Task<IAPIResponse> Get([FromQuery] string movieId, [FromQuery] string cinemaId, [FromQuery] string hallId, [FromQuery] long rawStartTime)
        {
            var startTime = DateTime.UnixEpoch.AddSeconds(rawStartTime / 1000).ToLocalTime();
            var session = await _db.Sessions
                .Include(s => s.MovieBelongsTo)
                .Include(s => s.HallLocatedAt)
                .ThenInclude(s => s.CinemaBelongTo)
                .FirstOrDefaultAsync(s => s.MovieId == movieId && s.CinemaId == cinemaId && s.HallId == hallId && s.StartTime == startTime);
            if (session == null)
                return APIResponse.Failaure("4001", "找不到对应排片");

            var tickets = await _db.Tickets
                .Where(t=>t.SessionAt == session)
                .ToListAsync();
            return APIDataResponse<List<Ticket>>.Success(tickets);
        }

        /// <summary>
        /// 购票
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="cinemaId"></param>
        /// <param name="hallId"></param>
        /// <param name="rawStartTime"></param>
        /// <param name="seats"></param>
        /// <returns></returns>
        [HttpPost("buy")]
        [Authorize(Policy ="Customer")]
        public async Task<IAPIResponse> Buy([FromForm] string movieId, [FromForm] string cinemaId, [FromForm] string hallId, [FromForm] long rawStartTime, [FromForm] List<int> seats)
        {
            var startTime = DateTime.UnixEpoch.AddSeconds(rawStartTime / 1000).ToLocalTime();
            if(startTime< DateTime.Now)
                return APIResponse.Failaure("4000", "该场次已不可售");

            var session = await _db.Sessions
                .Include(s => s.MovieBelongsTo)
                .Include(s => s.HallLocatedAt)
                .ThenInclude(s => s.CinemaBelongTo)
                .FirstOrDefaultAsync(s => s.MovieId == movieId && s.CinemaId == cinemaId && s.HallId == hallId && s.StartTime == startTime);
            if (session == null)
                return APIResponse.Failaure("4001", "找不到对应排片");

            var customerId = JwtHelper.SolveName(_contextAccessor);
            if (customerId == null)
                return APIResponse.Failaure("4002", "系统错误");

            using var transcation = _db.Database.BeginTransaction();

            try
            {
                foreach (var seat in seats)
                {
                    int row = seat >> 7;
                    int col = seat & 127;

                    // 用Any会出现bug，Oracle有bug。退求其次，用Count
                    if (await _db.Tickets.CountAsync(t => t.SessionAt == session && t.Row == row && t.Col == col) > 0) 
                    {
                        await transcation.RollbackAsync();
                        return APIResponse.Failaure("4000", $"座位{row}行{col}列已不可售");
                    }

                    var ticketId = Interlocked.Increment(ref _ticketId);
                    var ticket = new Ticket
                    {
                        Id = $"{ticketId:00000000}",
                        Row = row,
                        Col = col,
                        OrderTime = DateTime.Now,
                        State = 0,
                        Draw = 0,
                        Price = session.Price,
                        CustomerId = customerId,
                        MovieId = movieId,
                        HallId = hallId,
                        StartTime = startTime,
                        CinemaId = cinemaId,
                    };

                    await _db.Tickets.AddAsync(ticket);
                }
                await transcation.CommitAsync();
                await _db.SaveChangesAsync();
            }
            catch
            {
                await transcation.RollbackAsync();
                return APIResponse.Failaure("4002", "系统错误");
            }

            return APIResponse.Success();
        }
    }
}