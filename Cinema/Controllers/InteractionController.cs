using Cinema.DTO;
using Cinema.DTO.InteractionService;
using Cinema.Entities;
using Cinema.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Controllers
{
    /// <summary>
    /// 评论交互控制器类
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InteractionController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CinemaDb _db;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="db"></param>
        public InteractionController(IHttpContextAccessor httpContextAccessor, CinemaDb db)
        {
            _httpContextAccessor = httpContextAccessor;
            _db = db;
        }

        /// <summary>
        /// 获取用户已经进行的交互
        /// </summary>
        /// <param name="commentIds">需要查询的评论ID</param>
        /// <returns></returns>
        [HttpPost("get")]
        [ProducesDefaultResponseType(typeof(APIDataResponse<List<Interaction>>))]
        public async Task<IAPIResponse> GetInteractions([FromBody] List<string> commentIds)
        {
            var userId = JwtHelper.SolveName(_httpContextAccessor);
            if(userId == null)
            {
                return APIDataResponse<List<Interaction>>.Success(new List<Interaction>());
            }

            var interactions = await _db.Interactions
                    .Where(w => w.CustomerId == userId && commentIds.Contains(w.CommentId))
                    .ToListAsync();
            return APIDataResponse<List<Interaction>>.Success(interactions);
        }

        /// <summary>
        /// 进行评论交互
        /// </summary>
        /// <param name="interaction"></param>
        /// <returns></returns>
        /// <remarks>
        /// 前端不需要传入用户id，后端会自动获取
        /// </remarks>
        [HttpPost]
        [Authorize(Policy = "Customer")]
        [ProducesDefaultResponseType(typeof(APIResponse))]
        public async Task<IAPIResponse> DoInteraction([FromBody] InteractionDTO interaction)
        {
            if (interaction.Type != 0 && interaction.Type != 1 && interaction.Type != -1) 
            {
                return APIResponse.Failaure("4000", "非法的Type");
            }
            var comment = await _db.Comments.FindAsync(interaction.CommentId);
            if (comment == null)
            {
                return APIResponse.Failaure("4001", "评论不存在");
            }
            var customerId = JwtHelper.SolveName(_httpContextAccessor);
            if (customerId == null)
            {
                return APIResponse.Failaure("4001", "内部错误");
            }

            bool isNew = true;

            var interactionEntity = await _db.Interactions.FindAsync(interaction.CommentId, customerId);
            if (interactionEntity == null)
            {
                interactionEntity = new Interaction
                {
                    CommentId = interaction.CommentId,
                    CustomerId = customerId,
                    Type = interaction.Type
                };
            }
            else
            {
                isNew = false;
                if (interaction.Type == 1)
                {
                    comment.LikeCount--;
                }
                else
                {
                    comment.DislikeCount--;
                }
                interactionEntity.Type = interaction.Type;
            }

            try
            {
                if(interaction.Type>=0 && isNew)
                {
                    await _db.Interactions.AddAsync(interactionEntity);
                }
                else
                {
                    _db.Interactions.Remove(interactionEntity);
                }
            }
            catch
            {
                return APIResponse.Failaure("4000", "操作失败");
            }

            if(interaction.Type == 1)
            {
                comment.LikeCount++;
            }
            else
            {
                comment.DislikeCount++;
            }

            await _db.SaveChangesAsync();
            return APIResponse.Success();
        }
    }
}
