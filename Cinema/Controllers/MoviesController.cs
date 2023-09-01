using Cinema.DTO;
using Cinema.DTO.AvatarService;
using Cinema.DTO.CinemaService;
using Cinema.DTO.MoviesService;
using Cinema.DTO.StaffService;
using Cinema.Entities;
using Cinema.Helpers;
using Cinema.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.IO;
using TencentCloud.Cme.V20191029.Models;

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
        /// 获取所有电影的信息（分页）
        /// </summary>
        /// <returns></returns>
        /// <remarks>提醒，要分页！分页从1开始，小于1出现未定义行为</remarks>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<MovieDTO>>))]
        public async Task<IAPIResponse> GetMovies([FromQuery] ulong page_size, [FromQuery] ulong page_number)
        {
            var movies = await _db.Movies
                    .Skip((int)((page_number - 1ul) * page_size))
                    .Take((int)page_size)
                    .Include(m => m.Acts)
                    .ThenInclude(a => a.Staff)
                    .OrderBy(m => m.MovieId)
                    .ToArrayAsync();

            var moviesDTO = movies.Select(c => new MovieDTO(c)).ToList();
            return APIDataResponse<List<MovieDTO>>.Success(moviesDTO);
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
                .Include(m=>m.Acts)
                .ThenInclude(a=>a.Staff)
                .Include(m=>m.Sessions)
                .Include(m=>m.Comments.OrderByDescending(c=>c.PublishDate).Take(10))
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

        /// <summary>
        /// 获取所有电影的数量
        /// </summary>
        /// <returns></returns>
        /// <remarks>用于分页</remarks>
        [HttpGet("length")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<int>))]
        public async Task<IAPIResponse> GetMoviesLength()
        {
            var length = await _db.Movies.CountAsync();
            return APIDataResponse<int>.Success(length);
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

            var acts = new List<Act>();
            if (movie.Director != null) 
            {
                var act = new Act
                {
                    StaffId = movie.Director.StaffId,
                    MovieId = movie.MovieId,
                    Role = "1"
                };
                acts.Add(act);
            }
            if (movie.Actors != null && movie.Actors.Count > 0)
            {
                foreach (var actor in movie.Actors)
                {
                    var act = new Act
                    {
                        StaffId = actor.StaffId,
                        MovieId = movie.MovieId,
                        Role = "0"
                    };
                    acts.Add(act);
                }
            }

            await _db.Acts.AddRangeAsync(acts);
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
        //[Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> EditMovie([FromBody] MovieDTO movie)
        {
            var movieEntity = await _db.Movies
                .Where(m => m.MovieId == movie.MovieId)
                .Include(m => m.Acts)
                .FirstOrDefaultAsync();


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

            //删除所有旧的Acts
            _db.Acts.RemoveRange(movieEntity.Acts);
            //先保存数据库，否则此时追踪的Acts和新添加的Acts可能发生冲突
            _db.SaveChanges();
            var acts = new List<Act>();
            if (movie.Director != null)
            {
                var act = new Act
                {
                    StaffId = movie.Director.StaffId,
                    MovieId = movie.MovieId,
                    Role = "1"
                };
                acts.Add(act);
            }
            if (movie.Actors != null && movie.Actors.Count > 0)
            {
                foreach (var actor in movie.Actors)
                {
                    var act = new Act
                    {
                        StaffId = actor.StaffId,
                        MovieId = movie.MovieId,
                        Role = "0"
                    };
                    acts.Add(act);
                }
            }
            await _db.Acts.AddRangeAsync(acts);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch 
            {
                return APIResponse.Failaure("5000", "服务器内部错误");
            }

            //_db.Movies.Update(movieEntity);
            //await _db.SaveChangesAsync();
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
        /// 根据电影名获得对应电影信息，模糊查询
        /// </summary>
        /// <param name="name">电影名称</param>
        /// <returns>
        /// 返回电影列表json
        /// </returns>
        [HttpGet("ByName/{name}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<MovieDTO>>))]
        public async Task<IAPIResponse> GetMoviesByName([FromRoute]string name)
        {
            var movies = await _db.Movies
                .Where(m => m.Name.Contains(name))
                .Include(m => m.Acts)
                .ThenInclude(a => a.Staff)
                .ToListAsync();

            if (movies!.Count() == 0)
            {
                return APIDataResponse<Movie>.Failaure("4001", "电影不存在");
            }

            var movieDTOs = movies.Select(m => new MovieDTO(m)).ToList();
            return APIDataResponse<List<MovieDTO>>.Success(movieDTOs);
        }

        /// <summary>
        /// 根据电影id获得对应电影
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<MovieDTO>))]
        public async Task<IAPIResponse> GetMovieById([FromRoute]string id)
        {
            var movie = await _db.Movies
                .Where(m => m.MovieId == id)
                .Include(m => m.Acts)
                .ThenInclude(a => a.Staff)
                .FirstOrDefaultAsync();
            if(movie == null)
            {
                return APIDataResponse<Movie>.Failaure("4001", "电影不存在");
            }

            var movieDTO = new MovieDTO(movie);

            return APIDataResponse<MovieDTO>.Success(movieDTO);
        }

        /// <summary>
        /// 通过ID删除电影
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "CinemaAdmin")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> DeleteMovie([FromRoute] string id)
        {
            var movie = await _db.Movies.FindAsync(id);
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
