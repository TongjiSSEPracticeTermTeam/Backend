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
                .Where(c => c.MovieId == movieId)
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
                                .Where(c => c.MovieId == movieId)
                                .Include(c => c.Sender)
                                .OrderByDescending(c => c.PublishDate)
                                .Take(5)
                                .ToListAsync();
            var hotComments = await _db.Comments
                                .Where(c => c.MovieId == movieId)
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
        /// 根据评论ID屏蔽评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("ban/{id}")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> BanCommentById([FromRoute] string id)
        {
            var comment = await _db.Comments.FirstOrDefaultAsync(c => c.CommentId == id);
            
            if(comment == null)
            {
                return APIResponse.Failaure("4001", "评论不存在");
            }

            comment.Display = false;
            await _db.SaveChangesAsync();

            return APIResponse.Success();
        }

        /// <summary>
        /// 根据评论ID解除屏蔽
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("unban/{id}")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> UnbanCommentById([FromRoute] string id)
        {
            var comment = await _db.Comments.FirstOrDefaultAsync(c => c.CommentId == id);

            if (comment == null)
            {
                return APIResponse.Failaure("4001", "评论不存在");
            }

            comment.Display = true;
            await _db.SaveChangesAsync();

            return APIResponse.Success();
        }
    }
}
