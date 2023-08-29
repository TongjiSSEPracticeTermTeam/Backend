using Cinema.DTO;
using Cinema.DTO.AvatarService;
using Cinema.DTO.MoviesService;
using Cinema.Entities;
using Cinema.Helpers;
using Cinema.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Cinema.Controllers
{
    /// <summary>
    /// 电影相关控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
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
        public MoviesController(CinemaDb db, IHttpContextAccessor httpContextAccessor, ILogger<MoviesController> logger, JwtHelper jwtHelper, QCosSrvice qCosSrvice)
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
        /// 管理端接口，获取所有电影的信息（分页）
        /// </summary>
        /// <returns></returns>
        /// <remarks>提醒，要分页！分页从1开始，小于1出现未定义行为</remarks>
        [HttpGet]
        [Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(QueryMovieResponse))]
        public async Task<IAPIResponse> GetMovies([FromQuery] ulong page_size, [FromQuery] ulong page_number)
        {
            var movies = await _db.Movies
                    .Skip((int)((page_number - 1ul) * page_size))
                    .Take((int)page_size)
                    .OrderBy(m => m.MovieId)
                    .ToArrayAsync();
            return new QueryMovieResponse
            {
                Status = "10000",
                Message = "成功",
                Data = movies
            };
        }

        /// <summary>
        /// 管理端接口，获取所有电影的数量
        /// </summary>
        /// <returns></returns>
        /// <remarks>用于分页</remarks>
        [HttpGet("length")]
        [Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(QueryMovieNumResponse))]
        public async Task<IAPIResponse> GetMoviesLength()
        {
            var length = await _db.Movies.CountAsync();
            return new QueryMovieNumResponse
            {
                Status = "10000",
                Message = "成功",
                Length = length
            };
        }

        /// <summary>
        /// 管理端接口，添加电影
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> AddMovie([FromBody] MovieDTO movie)
        {
            var movieId = Interlocked.Increment(ref _movieId);
            movie.MovieId = String.Format("{0:000000}", movieId);

            var movieEntity = new Movie
            {
                Duration = movie.Duration,
                Instruction = movie.Instruction,
                MovieId = movie.MovieId,
                Name = movie.Name,
                PostUrl = movie.PostUrl,
                ReleaseDate = movie.ReleaseDate,
                RemovalDate = movie.RemovalDate,
                Tags = movie.Tags
            };

            await _db.Movies.AddAsync(movieEntity);
            await _db.SaveChangesAsync();
            return APIResponse.Success();
        }

        /// <summary>
        /// 管理端接口，修改电影信息
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> EditMovie([FromBody] MovieDTO movie)
        {
            var movieEntity = await _db.Movies.FindAsync(movie.MovieId);
            if(movieEntity==null)
            {
                return APIResponse.Failaure("4000", "电影不存在");
            }

            movieEntity.Name = movie.Name;
            movieEntity.Instruction = movie.Instruction;
            movieEntity.Duration = movie.Duration;
            movieEntity.Instruction = movie.Instruction;
            movieEntity.PostUrl = movie.PostUrl;
            movieEntity.Tags = movie.Tags;
            movieEntity.ReleaseDate = movie.ReleaseDate;
            movieEntity.RemovalDate = movie.RemovalDate;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch 
            {
                return APIResponse.Failaure("5000", "服务器内部错误");
            }

            _db.Movies.Update(movieEntity);
            await _db.SaveChangesAsync();
            return APIResponse.Success();
        }

        /// <summary>
        /// 上传电影海报
        /// </summary>
        /// <returns>电影海报的URL</returns>
        /// <remarks>文件从请求体中上传</remarks>
        [HttpPut("poster")]
        [Authorize(Policy = "RegUser")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<string>))]
        public async Task<IAPIResponse> ChangePoster(IFormFile file)
        {
            _logger.LogInformation("Poster upload request received.");

            if (file == null || file.Length == 0)
            {
                return APIResponse.Failaure("4002", "未上传文件");
            }

            // Save to temp file
            var tempFileName = Path.GetTempFileName();
            await using (var tempFile = new FileStream(tempFileName, FileMode.Create))
            {
                await file.CopyToAsync(tempFile);
            }

            var isValidImage = ImageService.PreprocessUploadedImage(tempFileName);
            if (!isValidImage)
            {
                return APIResponse.Failaure("4000", "不是有效的图片");
            }

            var posterPath = String.Format("userdata/poster/{0}.jpg", Guid.NewGuid().ToString("N"));
            string? posterUrl;
            try
            {
                posterUrl = await _qCosSrvice.UploadFile(posterPath, tempFileName);
            }
            catch
            {
                System.IO.File.Delete(tempFileName);
                return APIResponse.Failaure("4000", "上传图像失败（内部错误）");
            }
            System.IO.File.Delete(tempFileName);

            _logger.LogInformation("Poster upload request succesfully completed.");
            return APIDataResponse<string>.Success(posterUrl);
        }

        /// <summary>
        /// 删除电影
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> DeleteMovie([FromQuery] string movieId)
        {
            var movie = await _db.Movies.FindAsync(movieId);
            if(movie==null)
            {
                return APIResponse.Failaure("4000", "电影不存在");
            }

            _db.Movies.Remove(movie);
            await _db.SaveChangesAsync();
            return APIResponse.Success();
        }
    }
}
