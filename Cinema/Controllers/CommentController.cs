using Cinema.DTO;
using Cinema.DTO.CommentService;
using Cinema.DTO.StaffService;
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
    /// 评论控制器类
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CinemaDb _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        private readonly JwtHelper _jwtHelper;
        private readonly QCosSrvice _qCosSrvice;

        private static int _commentId;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="db"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="logger"></param>
        /// <param name="jwtHelper"></param>
        /// <param name="qCosSrvice"></param>
        public CommentController(CinemaDb db, IHttpContextAccessor httpContextAccessor, ILogger<CommentController> logger, JwtHelper jwtHelper, QCosSrvice qCosSrvice)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _jwtHelper = jwtHelper;
            _qCosSrvice = qCosSrvice;

            if (_commentId == 0)
            {
                _commentId = int.Parse(_db.Comments.Max(m => m.CommentId) ?? "0");
            }
        }

        /// <summary>
        /// 根据电影ID获取对应评论
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        [HttpGet("byMovieId/{movieId}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<CommentDTO>>))]
        public async Task<IAPIResponse> GetCommentsByMovieId([FromRoute] string movieId)
        {
            var comments = await _db.Comments
                .Where(c => (c.MovieId == movieId && c.Display == "1"))
                .Include(c => c.Sender)
                .ToListAsync();
            var commentDTOs = comments.Select(c => new CommentDTO(c)).ToList();
            return APIDataResponse<List<CommentDTO>>.Success(commentDTOs);
        }

        /// <summary>
        /// 在电影详情页中获取评论
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        [HttpGet("inMovieDetail/{movieId}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<MovieCommentResponse>))]
        public async Task<IAPIResponse> GetCommentsInMovieDetail([FromRoute] string movieId)
        {
            var newComments = await _db.Comments
                                .Where(c => (c.MovieId == movieId && c.Display == "1"))
                                .Include(c => c.Sender)
                                .OrderByDescending(c => c.PublishDate)
                                .Take(5)
                                .ToListAsync();
            var hotComments = await _db.Comments
                                .Where(c => (c.MovieId == movieId && c.Display == "1"))
                                .Include(c => c.Sender)
                                .OrderByDescending(c => c.LikeCount)
                                .Take(5)
                                .ToListAsync();
            return APIDataResponse<MovieCommentResponse>.Success(new MovieCommentResponse
            {
                NewComments = newComments,
                HotComments = hotComments
            });
        }

        /// <summary>
        /// 根据用户ID获取对应评论
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet("byCustomerId/{customerId}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<CommentDTO>>))]
        public async Task<IAPIResponse> GetCommentsByCustomerId([FromRoute] string customerId)
        {
            var comments = await _db.Comments
                .Where(c => c.CustomerId == customerId)
                .Include(c => c.Sender)
                .ToListAsync();
            var commentDTOs = comments.Select(c => new CommentDTO(c)).ToList();
            return APIDataResponse<List<CommentDTO>>.Success(commentDTOs);
        }

        /// <summary>
        /// 管理员根据用户ID获取对应评论,包括被屏蔽的
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet("byCustomerId(Admin)/{customerId}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<CommentDTO>>))]
        public async Task<IAPIResponse> AdminGetCommentsByCustomerId([FromRoute] string customerId)
        {
            var comments = await _db.Comments
                .Where(c => c.CustomerId == customerId)
                .Include(c => c.Sender)
                .ToListAsync();
            var commentDTOs = comments.Select(c => new CommentDTO(c)).ToList();
            return APIDataResponse<List<CommentDTO>>.Success(commentDTOs);
        }


        /// <summary>
        /// 管理员根据电影ID获取所有评论(包括被屏蔽的)
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        [HttpGet("byMovieId(Admin)/{movieId}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<CommentDTO>>))]
        public async Task<IAPIResponse> AdminGetCommentsByMovieId([FromRoute] string movieId)
        {
            var comments = await _db.Comments
                .Where(c => (c.MovieId == movieId))
                .Include(c => c.Sender)
                .ToListAsync();
            var commentDTOs = comments.Select(c => new CommentDTO(c)).ToList();
            return APIDataResponse<List<CommentDTO>>.Success(commentDTOs);
        }


        /// <summary>
        /// 管理员根据评论ID获取对应评论
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpGet("byCommentId(Admin)/{commentId}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<CommentDTO>))]
        public async Task<IAPIResponse> AdminGetCommentByCommentId([FromRoute] string commentId)
        {
            var comments = await _db.Comments
                .Where(c => (c.CommentId == commentId))
                .Include(c => c.Sender)
                .ToListAsync();
            var commentDTOs = comments.Select(c => new CommentDTO(c)).ToList();
            return APIDataResponse<List<CommentDTO>>.Success(commentDTOs);
        }

        /// <summary>
        /// 根据评论ID屏蔽评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("ban/{id}")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> BanCommentById([FromRoute] string id)
        {
            var comment = await _db.Comments.FirstOrDefaultAsync(c => c.CommentId == id);

            if (comment == null)
            {
                return APIResponse.Failaure("4001", "评论不存在");
            }

            comment.Display = "0";
            await _db.SaveChangesAsync();

            return APIResponse.Success();
        }

        /// <summary>
        /// 根据评论ID解除屏蔽
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("unban/{id}")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> UnbanCommentById([FromRoute] string id)
        {
            var comment = await _db.Comments.FirstOrDefaultAsync(c => c.CommentId == id);

            if (comment == null)
            {
                return APIResponse.Failaure("4001", "评论不存在");
            }

            comment.Display = "1";
            await _db.SaveChangesAsync();

            return APIResponse.Success();
        }

        /// <summary>
        /// 根据评论内容获取对应评论
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("byContent/{keyword}")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<Comment>>))]
        public async Task<IAPIResponse> GetCommentByContent([FromRoute] string keyword)
        {
            if (keyword == null)
            {
                return APIResponse.Failaure("10001", "关键词不能为空");
            }

            var comments = await _db.Comments
                .Where(c => c.Content.Contains(keyword))
                .ToListAsync();

            return APIDataResponse<List<Comment>>.Success(comments);
        }

        /// <summary>
        /// 返回对应用户和电影间是否存在评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("commented")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<string>>))]
        public async Task<IAPIResponse> Commented([FromQuery] List<string> id)
        {
            var role = JwtHelper.SolveRole(_httpContextAccessor);
            Enum.TryParse(typeof(UserRole), role, out object? userRole);
            if (userRole == null || (UserRole)userRole != UserRole.User)
                return APIDataResponse<List<Boolean>>.Success(new List<bool>());

            var customerId = JwtHelper.SolveName(_httpContextAccessor);

            var movies = await _db.Comments.Where(c => c.CustomerId == customerId && id.Contains(c.MovieId)).Select(c => c.MovieId).ToListAsync();
            return APIDataResponse<List<string>>.Success(movies);
        }

        /// <summary>
        /// 用户编辑前获取评论内容
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        [HttpGet("edit")]
        [Authorize(Policy = "Customer")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<CommentCreator>))]
        public async Task<IAPIResponse> GetComment([FromQuery] string movieId)
        {
            var customerId = JwtHelper.SolveName(_httpContextAccessor);
            var commentEntity = await _db.Comments
                .Where(c => c.CustomerId == customerId && c.MovieId == movieId)
                .FirstOrDefaultAsync();

            if (commentEntity == null)
                return APIDataResponse<CommentCreator>.Success(new CommentCreator
                {
                    MovieId = movieId,
                });
            else
                return APIDataResponse<CommentCreator>.Success(new CommentCreator
                {
                    MovieId = movieId,
                    PublishDate = commentEntity.PublishDate,
                    Content = commentEntity.Content,
                    Score = commentEntity.Score,
                });
        }

        /// <summary>
        /// 添加或修改评论（限制一个用户对一个电影只能有一条评论）
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Policy = "Customer")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> AddComment([FromBody] CommentCreator comment)
        {
            var customerId = JwtHelper.SolveName(_httpContextAccessor);
            if (customerId == null)
            {
                return APIResponse.Failaure("4001", "系统错误");
            }

            var movie = await _db.Movies.FindAsync(comment.MovieId);

            if (movie == null)
            {
                return APIResponse.Failaure("4001", "电影不存在");
            }

            var oComment = _db.Comments
                .Where(c => c.CustomerId == customerId && c.MovieId == comment.MovieId)
                .FirstOrDefault();

            if (oComment != null)
            {
                oComment.Content = comment.Content;
                oComment.Score = comment.Score;
                oComment.PublishDate = DateTime.Now;
            }
            else
            {
                var nextId = Interlocked.Increment(ref _commentId);
                var commentId = String.Format("{0:000000000}", nextId);

                var nComment = new Comment
                {
                    CommentId = commentId,
                    Content = comment.Content,
                    Score = comment.Score,
                    LikeCount = 0,
                    DislikeCount = 0,
                    PublishDate = DateTime.Now,
                    Display = "1",
                    MovieId = comment.MovieId,
                    CustomerId = customerId
                };
                await _db.Comments.AddAsync(nComment);
            }

            await _db.SaveChangesAsync();
            

            //更新对应电影评分
            var totalScore = _db.Comments
                .Where(c => c.MovieId == comment.MovieId)
                .Select(c => c.Score).ToList();
            movie.Score = totalScore.Average();

            await _db.SaveChangesAsync();

            return APIResponse.Success();
        }
    }
}
