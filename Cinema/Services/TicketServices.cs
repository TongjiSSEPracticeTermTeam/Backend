using Cinema.DTO.BoxOfficeService;
using Cinema.DTO.ManagerService;
using Cinema.Entities;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;

namespace Cinema.Services
{
    /// <summary>
    /// 售票服务类，获取影院的售票
    /// </summary>
    public class TicketServices
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly CinemaDb _db;

        /// <summary>
        /// 初始化_redis
        /// </summary>
        /// <param name="redis"></param>
        /// <param name="db"></param>
        public TicketServices(IConnectionMultiplexer redis, CinemaDb db)
        {
            _redis = redis;
            _db = db;
        }

        /// <summary>
        /// 获取所有的售票信息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<TicketDTO>> GetTickets()
        {
            var db = _redis.GetDatabase();
            string? res = db.StringGet("tickets");
            if (res != null)
            {
                //如果redis中已经有相关数据，则直接返回
                return JsonSerializer.Deserialize<List<TicketDTO>>(res)!;
            }
            else
            {
                var tickets = await _db.Tickets
                              .OrderBy(t => t.Id) .ToArrayAsync();
                if(tickets.Length == 0)
                {
                    throw new Exception("没有售票");
                }
                var ticketDTOs = tickets.Select(t => new TicketDTO(t)).ToList();

                var ticketDTOsJSON = JsonSerializer.Serialize(ticketDTOs);
                db.StringSet("tickets", ticketDTOsJSON, TimeSpan.FromHours(1));
                return ticketDTOs;
            }
        }
    }
}
