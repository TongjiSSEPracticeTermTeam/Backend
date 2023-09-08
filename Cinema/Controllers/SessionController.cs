using Cinema.DTO;
using Cinema.DTO.CinemaService;
using Cinema.DTO.SessionService;
using Cinema.Entities;
using Microsoft.AspNetCore.Authorization;
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
                                .Where(s => s.StartTime > DateTime.Now && s.MovieId == movieId)
                                .ToListAsync();

            // 根据session.StartTime对sessions分类
            var groupedSessions =
                from session in sessions
                group session by session.StartTime.Date into g
                orderby g.Key ascending
                select g;

            return APIDataResponse<Dictionary<DateTime, List<Session>>>.Success(groupedSessions.ToDictionary(g => g.Key, g => g.ToList()));
        }

        /// <summary>
        /// 根据影厅ID获取排片
        /// </summary>
        /// <param name="cinema_id"></param>
        /// <param name="hall_id"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<SessionDTO>>))]
        public async Task<IAPIResponse> GetByHallId([FromQuery] string cinema_id, [FromQuery] string hall_id)
        {
            var sessions = await _db.Sessions
                                .Where(s => s.CinemaId == cinema_id && s.HallId == hall_id)
                                .OrderBy(s => s.StartTime)
                                .ToListAsync();
            if (sessions == null || sessions.Count == 0)
            {
                return APIResponse.Failaure("40004", "当前影厅下无排片");
            }

            var sessionsDTO = sessions.Select(c => new SessionDTO(c)).ToList();
            return APIDataResponse<List<SessionDTO>>.Success(sessionsDTO);
        }

        /// <summary>
        /// 在某个影厅的某个时间点排片
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> PutSession([FromBody] SessionDTO data)
        {
            // 先判断影厅是否存在
            var hall = await _db.Halls.FirstOrDefaultAsync(h => h.Id == data.HallId && h.CinemaId == data.CinemaId);
            if (hall == null)
                return APIResponse.Failaure("40001", "影厅不存在");
            // 之后判断电影是否存在
            var movie = await _db.Movies.FirstOrDefaultAsync(m => m.MovieId == data.MovieId);
            if (movie == null)
                return APIResponse.Failaure("40002", "电影不存在");
            // 再判断拍片时间是否在电影上下映之间
            if (data.StartTime < DateTime.Now || data.StartTime > movie.RemovalDate)
                return APIResponse.Failaure("40003", "排片时间不在电影上映时间内或在过去时间段内");
            //最后判断拍片时间是否已经被占用


            var duration = int.Parse(movie.Duration);

            var endTime = data.StartTime.AddMinutes(duration);
            Console.Write(endTime);
            var sessions = await _db.Sessions.Where(s=>s.HallId==data.HallId).Join(_db.Movies, s => s.MovieId, m => m.MovieId, (s, m) => new { Session = s, Movie = m })
     .ToListAsync();

            bool existed = false;

            foreach (var sm in sessions)
            {
                var sessionStartTime = sm.Session.StartTime;
                var movieDuration = int.Parse(sm.Movie.Duration);
                var sessionEndTime = sessionStartTime.AddMinutes(movieDuration);

                if ((sessionStartTime >= data.StartTime && sessionStartTime <= endTime)
                    || (sessionEndTime >= data.StartTime && sessionEndTime <= endTime)
                    || (sessionStartTime <= data.StartTime && sessionEndTime >= endTime))
                {
                    existed = true;
                    break;
                }
            }


            if (existed)
                return APIResponse.Failaure("40004", "该时间段已被排片");
            // 如果都没有问题，就创建排片
            var session = new Session
            {
                MovieId = data.MovieId,
                CinemaId = data.CinemaId,
                HallId = data.HallId,
                StartTime = data.StartTime,
                Price = data.Price,
                Attendence = 0,
                Language = data.Language,
                Dimesion = data.Dimesion,
            };
            await _db.Sessions.AddAsync(session);
            await _db.SaveChangesAsync();
            return APIResponse.Success();
        }

        /// <summary>
        /// 获取拥有对应电影排场的影院，客户端接口
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        [HttpGet("isMovieInCinema/{movieId}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<CinemaDTO>>))]
        public async Task<IAPIResponse> GetCinemaByMovieSession([FromRoute] string movieId)
        {
            var sessions = await _db.Sessions
                    .Include(s => s.HallLocatedAt)
                    .ThenInclude(s => s.CinemaBelongTo)
                    .Where(s => s.StartTime > DateTime.Now && s.MovieId == movieId)
                    .ToListAsync();

            var cinemaDTOs = sessions.Select(s => new CinemaDTO(s.HallLocatedAt.CinemaBelongTo)).ToList();

            return APIDataResponse<List<CinemaDTO>>.Success(cinemaDTOs);
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
            var startTime = DateTime.UnixEpoch.AddSeconds(rawStartTime / 1000).ToLocalTime();
            var session = await _db.Sessions
                .Include(s => s.MovieBelongsTo)
                .Include(s => s.HallLocatedAt)
                .ThenInclude(s => s.CinemaBelongTo)
                .FirstOrDefaultAsync(s => s.MovieId == movieId && s.CinemaId == cinemaId && s.HallId == hallId && s.StartTime == startTime);
            if (session == null)
                return APIResponse.Failaure("4001", "找不到对应排片");
            return APIDataResponse<SessionDetailResponse>.Success(new SessionDetailResponse
            {
                Session = session,
                Movie = session.MovieBelongsTo
            });
        }

        /// <summary>
        /// 根据影院ID获取排片，客户端接口
        /// </summary>
        /// <param name="cinemaId"></param>
        /// <returns></returns>
        [HttpGet("movieInCinema")]
        [HttpGet("cinema/{cinemaId}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<Dictionary<DateTime, List<Session>>>))]
        public async Task<IAPIResponse> GetByCinemaId([FromRoute] string cinemaId)
        {
            var sessions = await _db.Sessions
                    .Include(s => s.HallLocatedAt)
                    .ThenInclude(s => s.CinemaBelongTo)
                    .Where(s => s.StartTime > DateTime.Now && s.CinemaId == cinemaId)
                    .ToListAsync();

            // 根据session.StartTime对sessions分类
            var groupedSessions =
                from session in sessions
                group session by session.StartTime.Date into g
                orderby g.Key ascending
                select g;

            return APIDataResponse<Dictionary<DateTime, List<Session>>>.Success(groupedSessions.ToDictionary(g => g.Key, g => g.ToList()));
        }
    }
}