using Cinema.DTO;
using Cinema.DTO.MoviesInHallService;
using Cinema.Entities;
using Cinema.Helpers;
using Cinema.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Controllers
{
    /// <summary>
    /// 电影相关控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesInHallController : ControllerBase
    {
        private readonly CinemaDb _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly JwtHelper _jwtHelper;
        private readonly QCosSrvice _qCosSrvice;

        private static int _movieId;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="db"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="logger"></param>
        /// <param name="jwtHelper"></param>
        /// <param name="qCosSrvice"></param>
        public MoviesInHallController(CinemaDb db, IHttpContextAccessor httpContextAccessor, ILogger<MoviesInHallController> logger, JwtHelper jwtHelper, QCosSrvice qCosSrvice)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _jwtHelper = jwtHelper;
            _qCosSrvice = qCosSrvice;

            if(_movieId==0)
            {
                _movieId = int.Parse(_db.Movies.Max(m => m.MovieId) ?? "0");
            }
        }

        /// <summary>
        /// 获取当前影厅所有电影的信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<MovieInHallDTO>>))]
        public async Task<IAPIResponse> GetMovies([FromRoute] string id)
        {
            var sessions = await _db.Sessions
                                .Where(s => s.CinemaId == id)
                                .ToListAsync();

            var movieIds = sessions.Select(s => s.MovieId).ToList();

            var movies = await _db.Movies
                                .Where(m => movieIds.Contains(m.MovieId))
                                .OrderBy(m => m.MovieId)
                                .ToListAsync();

            var moviesDTO = movies.Select(c => new MovieInHallDTO(c)).ToList();
            return APIDataResponse<List<MovieInHallDTO>>.Success(moviesDTO);
        }

        /// <summary>
        /// 获取特定电影的详细信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("detail/{id}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<Movie>))]
        public async Task<IAPIResponse> GetMovie([FromRoute] string id)
        {
            var movies = await _db.Movies
                .Where(m=>m.MovieId==id)
                .FirstAsync();
            if (movies != null)
            {
                return APIDataResponse<Movie>.Success(movies);
            }
            else
            {
                return APIResponse.Failaure("4001", "未找到此电影");
            }
        }
    }
}
