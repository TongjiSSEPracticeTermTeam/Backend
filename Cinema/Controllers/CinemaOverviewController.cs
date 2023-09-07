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
            var cinema = await _db.Cinemas.FindAsync(cinemaId);
            if (cinema == null)
            {
                return APIResponse.Failaure("4004", "影院不存在");
            }
            var allTickets = await _ticketServices.GetTickets();
            var tickets = allTickets.Where(t => t.CinemaId == cinemaId).ToList();

            var OverviewData = new OverviewDTO();

            var ticketsToday = tickets.Where(t => t.StartTime == DateTime.Today && t.State != TicketState.refunded).ToList();

            OverviewData.GlobalData.AudienceNumberToday = ticketsToday.Count;
            double TotalBoxOfficeToday = 0.0;
            foreach (var ticket in ticketsToday)
            {
                TotalBoxOfficeToday += ticket.Price;
            }
            OverviewData.GlobalData.TotalBoxOfficeToday = TotalBoxOfficeToday;

            var sessions = await _db.Sessions.Where(s => s.CinemaId == cinemaId)
                           .OrderBy(s => s.MovieId).ToArrayAsync();
            if (sessions.Length == 0)
            {
                return APIResponse.Failaure("4003", "影院没有排片");
            }
            var SessionGyMovie = sessions.GroupBy(s => s.MovieId);

            foreach (var movie in SessionGyMovie)
            {
                CinemaMovieData movieData = new();
                var movieEntity = await _db.Movies.FindAsync(movie.Key);
                if (movieEntity == null)
                {
                    return APIResponse.Failaure("4002", "电影不存在");
                }
                movieData.MovieName = movieEntity.Name;
                movieData.PremiereDate = movieEntity.ReleaseDate;

                var movieTickets = tickets.Where(t => t.MovieId == movie.Key && t.State != TicketState.refunded).ToList();
                movieData.AudienceNumber = movieTickets.Count;

                double TotalBoxOffice = 0.0;
                int totalSeat = 0;
                foreach (var ticket in movieTickets)
                {
                    TotalBoxOffice += ticket.Price;
                    var hall = await _db.Halls.FindAsync(ticket.HallId);
                    if (hall == null || hall.Seat == null)
                    {
                        return APIResponse.Failaure("4001", "影厅或座位不存在");
                    }
                    foreach (var col in hall.Seat.Cols)
                    {
                        totalSeat += col;
                    }
                }
                movieData.TotalBoxOffice = TotalBoxOffice;
                movieData.Attendance = (totalSeat == 0 ? 0 : movieData.AudienceNumber / totalSeat);

                OverviewData.MovieDatas.Add(movieData);
            }

            return APIDataResponse<OverviewDTO>.Success(OverviewData);
        }
    }
}
