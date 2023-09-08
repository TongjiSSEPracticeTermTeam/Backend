using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Cinema.DTO;
using Cinema.DTO.TicketService;
using Cinema.Entities;
using Cinema.Helpers;
using Cinema.Services;
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
        private readonly AesEncryptionervice _aes;

        private static int _ticketId;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="db"></param>
        /// <param name="contextAccessor"></param>
        /// <param name="aes"></param>
        public TicketController(CinemaDb db, IHttpContextAccessor contextAccessor, AesEncryptionervice aes)
        {
            _db = db;
            _contextAccessor = contextAccessor;
            _aes = aes;

            if (_ticketId == 0)
            {
                _ticketId = int.Parse(_db.Tickets.Max(t => t.Id) ?? "0");
            }
        }

        /// <summary>
        /// 查询电影票
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 顾客调用时，查询到的是自己的票；否则，是全部的票
        /// </remarks>
        [HttpGet]
        [Authorize(Policy = "RegUser")]
        [ProducesDefaultResponseType(typeof(List<Ticket>))]
        public async Task<IAPIResponse> Get()
        {
            var role = (UserRole)Enum.Parse(typeof(UserRole), JwtHelper.SolveRole(_contextAccessor) ?? "");

            if (role != UserRole.User)
            {
                return APIDataResponse<List<Ticket>>.Success(await _db.Tickets.ToListAsync());
            }
            else
            {
                var customerId = JwtHelper.SolveName(_contextAccessor) ?? "";
                var tickets = await _db.Tickets
                    .Where(t => t.CustomerId == customerId)
                    .ToListAsync();
                return APIDataResponse<List<Ticket>>.Success(tickets);
            }
        }

        /// <summary>
        /// 查询电影票的相关信息（影厅、电影等）
        /// </summary>
        /// <param name="sessionStr"></param>
        /// <returns></returns>
        [HttpGet("info")]
        [Authorize(Policy = "RegUser")]
        [ProducesDefaultResponseType(typeof(Dictionary<string, TicketSideInfo>))]
        public async Task<IAPIResponse> Get([FromQuery] List<string> sessionStr)
        {
            var result = new Dictionary<string, TicketSideInfo>();
            foreach (var str in sessionStr)
            {
                var parts = str.Split(' ');
                if (parts.Length != 4)
                    return APIResponse.Failaure("4000", "前端提供的数据格式不正确");

                string cinemaId = parts[1];
                string hallId = parts[2];
                string movieId = parts[3];

                if (long.TryParse(parts[0], out long startTimeRaw))
                {
                    var startTime = DateTime.UnixEpoch.AddSeconds(startTimeRaw / 1000).ToLocalTime();
                    var session = await _db.Sessions
                        .Include(s => s.MovieBelongsTo)
                        .Include(s => s.HallLocatedAt)
                        .ThenInclude(s => s.CinemaBelongTo)
                        .FirstOrDefaultAsync(s => s.MovieId == movieId && s.CinemaId == cinemaId && s.HallId == hallId && s.StartTime == startTime);
                    if (session == null)
                        return APIResponse.Failaure("4001", "找不到对应排片");

                    result.Add(str, new TicketSideInfo
                    {
                        Movie = session.MovieBelongsTo,
                        Session = session
                    });
                }
                else
                    return APIResponse.Failaure("4000", "前端提供的数据格式不正确");
            }
            return APIDataResponse<Dictionary<string, TicketSideInfo>>.Success(result);
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
                .Where(t => t.SessionAt == session && t.State == 0)
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
        [Authorize(Policy = "Customer")]
        public async Task<IAPIResponse> Buy([FromForm] string movieId, [FromForm] string cinemaId, [FromForm] string hallId, [FromForm] long rawStartTime, [FromForm] List<int> seats)
        {
            var startTime = DateTime.UnixEpoch.AddSeconds(rawStartTime / 1000).ToLocalTime();
            if (startTime < DateTime.Now)
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

        /// <summary>
        /// 获得取票码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("getTicketCode")]
        [Authorize(Policy = "Customer")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<string>))]
        public async Task<IAPIResponse> GetTicketCode([FromForm] List<string> id)
        {
            var customerId = JwtHelper.SolveName(_contextAccessor) ?? "";

            foreach (var ticketId in id)
            {
                var ticket = await _db.Tickets.FindAsync(ticketId);
                if (ticket == null)
                    return APIResponse.Failaure("4000", "包含不存在的票");
                if (ticket.CustomerId != customerId)
                    return APIResponse.Failaure("4001", "不允许取此票");
                if (ticket.StartTime.AddMinutes(30) < DateTime.Now)
                    return APIResponse.Failaure("4001", "影片已开场30分钟，不允许取票");
                if (ticket.Draw > 0)
                    return APIResponse.Failaure("4001", "不允许重复取票");
                if (ticket.State != TicketState.normal)
                    return APIResponse.Failaure("4001", "不允许取此票");
            }

            var responseRaw = new GetTicketInfo
            {
                Tickets = id,
                NotAfter = DateTime.Now.AddMinutes(10), // 有效期十分钟
            };

            var responseJson = JsonSerializer.Serialize(responseRaw);
            var response = _aes.Encrypt(responseJson);
            return APIDataResponse<string>.Success(response);
        }



        /// <summary>
        /// 使用取票码取票
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost("getTicket")]
        [Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> GetTicket([FromForm] string code)
        {
            string? getTicketInfoJson;
            GetTicketInfo? getTicketInfo;

            try
            {
                getTicketInfoJson = _aes.Decrypt(code);
            }
            catch
            {
                return APIResponse.Failaure("4001", "取票码已损坏");
            }

            try
            {
                getTicketInfo = JsonSerializer.Deserialize<GetTicketInfo>(getTicketInfoJson)!;
            }
            catch
            {
                return APIResponse.Failaure("4001", "取票码已损坏");
            }

            if (getTicketInfo.NotBefore > DateTime.Now || getTicketInfo.NotAfter < DateTime.Now)
                return APIResponse.Failaure("4001", "取票码已过期");

            using var transcation = _db.Database.BeginTransaction();

            try
            {
                foreach (var ticketId in getTicketInfo.Tickets)
                {
                    var ticket = await _db.Tickets.FindAsync(ticketId) ?? throw new SystemException();
                    if (ticket.Draw > 0)
                    {
                        await transcation.RollbackAsync();
                        return APIResponse.Failaure("4001", "不允许重复取票");
                    }
                    if (ticket.State != TicketState.normal)
                    {
                        await transcation.RollbackAsync();
                        return APIResponse.Failaure("4001", "不允许取此票");
                    }
                    ticket.Draw = 1;
                }
            }
            catch
            {
                await transcation.RollbackAsync();
                return APIResponse.Failaure("4002", "系统错误");
            }

            await _db.SaveChangesAsync();
            await transcation.CommitAsync();

            return APIResponse.Success();
        }

        /// <summary>
        /// 退票
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost("cancel")]
        [Authorize(Policy = "Customer")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> CancelTicket([FromForm] List<string> ids)
        {
            string userId = JwtHelper.SolveName(_contextAccessor) ?? "";

            var tickets = await _db.Tickets.Where(t=>ids.Contains(t.Id)).ToListAsync();
            if (tickets == null || tickets.Count == 0)
                return APIResponse.Failaure("4000", "票不存在");

            using var transcation = _db.Database.BeginTransaction();
            foreach (var ticket in tickets)
            {
                if(ticket.CustomerId != userId)
                {
                    await transcation.RollbackAsync();
                    return APIResponse.Failaure("4001", "部分票无权限操作");
                }

                ticket.State = TicketState.refunded;
            }

            await _db.SaveChangesAsync();
            await transcation.CommitAsync();
            return APIResponse.Success();
        }
    }
}