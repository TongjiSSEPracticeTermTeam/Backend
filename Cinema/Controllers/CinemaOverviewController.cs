using Cinema.DTO;
using Cinema.DTO.ManagerService;
using Cinema.Entities;
using Cinema.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Cinema.Controllers
{
    /// <summary>
    /// 针对单独影院的数据控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CinemaOverviewController : ControllerBase
    {
        private readonly CinemaDb _db;
        private readonly TicketServices _ticketServices;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ticketServices"></param>
        public CinemaOverviewController(CinemaDb db, TicketServices ticketServices)
        {
            _db = db;
            _ticketServices = ticketServices;
        }

        /// <summary>
        /// 管理端接口，获取总览页面的数据
        /// </summary>
        /// <param name="cinemaId"></param>
        /// <returns></returns>
        [HttpGet("{cinemaId}")]
        [Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<OverviewDTO>))]
        public async Task<IAPIResponse> GetOverviewDataByCinemaId([FromRoute] string cinemaId)
        {
            var cinema = await _db.Cinemas.FindAsync(cinemaId);  //获取电影院
            if (cinema == null)
            {
                return APIResponse.Failaure("4004", "影院不存在");
            }
            var allTickets = await _ticketServices.GetTickets();  //获取所有售票
            var tickets = allTickets.Where(t => t.CinemaId == cinemaId).ToList(); //获取本影院的售票

            var OverviewData = new OverviewDTO();

            var ticketsToday = tickets.Where(t => t.StartTime.Date == DateTime.Today && 
                                             t.StartTime.AddMinutes(-30) < DateTime.Now && 
                                             t.State != TicketState.refunded).ToList();  //获取今日的售票

            OverviewData.GlobalData.AudienceNumberToday = ticketsToday.Count; //今日的总观影人数（已观影）
            double TotalBoxOfficeToday = 0.0;
            foreach (var ticket in ticketsToday)
            {
                TotalBoxOfficeToday += ticket.Price;
            }
            OverviewData.GlobalData.TotalBoxOfficeToday = TotalBoxOfficeToday;  //今日总票房（已观影）

            var sessions = await _db.Sessions.Where(s => s.CinemaId == cinemaId).ToArrayAsync(); //获取本影院的所有排片
            if (sessions.Length == 0)
            {
                return APIResponse.Failaure("4003", "影院没有排片");
            }
            var allSessionsGyMovie = sessions.GroupBy(s => s.MovieId); //按照不同的电影将排片分类

            foreach (var sessionsGyMovie in allSessionsGyMovie)
            {
                CinemaMovieData movieData = new();
                var movieEntity = await _db.Movies.FindAsync(sessionsGyMovie.Key); //查询电影数据
                if (movieEntity == null)
                {
                    return APIResponse.Failaure("4002", "电影不存在");
                }
                movieData.MovieName = movieEntity.Name;
                movieData.PremiereDate = movieEntity.ReleaseDate;

                var movieTickets = tickets.Where(t => t.MovieId == sessionsGyMovie.Key &&
                                                 t.StartTime.AddMinutes(-30) < DateTime.Now &&
                                                 t.State != TicketState.refunded).ToList(); //获取本影院该电影的所有售票（已观影）
                movieData.AudienceNumber = movieTickets.Count;

                double TotalBoxOffice = 0.0;
                foreach (var ticket in movieTickets)
                {
                    TotalBoxOffice += ticket.Price;
                }
                movieData.TotalBoxOffice = TotalBoxOffice;  //获取总票房

                int totalSeat = 0;
                var sessionsDone = sessionsGyMovie.Where(s => s.StartTime.AddMinutes(-30) < DateTime.Now); //获取本影院的所有排片（已观影）
                foreach (var session in sessionsDone)
                {
                    var hall = await _db.Halls.FirstAsync(h => h.Id == session.HallId && h.CinemaId == cinemaId);
                    if (hall == null || hall.Seat == null)
                    {
                        return APIResponse.Failaure("4001", "影厅或座位不存在");
                    }
                    foreach (var col in hall.Seat.Cols)
                    {
                        totalSeat += col;
                    }
                }
                movieData.Attendance = (totalSeat == 0 ? 0 : movieData.AudienceNumber / (float)totalSeat);

                OverviewData.MovieDatas.Add(movieData);
            }

            return APIDataResponse<OverviewDTO>.Success(OverviewData);
        }
    }
}
