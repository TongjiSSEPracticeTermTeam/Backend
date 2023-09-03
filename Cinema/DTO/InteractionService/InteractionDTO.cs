using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cinema.DTO.InteractionService
{
    /// <summary>
    /// 评论交互DTO
    /// </summary>
    public class InteractionDTO
    {
        /// <summary>
        /// 评论ID
        /// </summary>
        [Required]
        public string CommentId { get; set; } = String.Empty;

        /// <summary>
        /// 客户ID
        /// </summary>
        public string CustomerId { get; set; } = String.Empty;

        /// <summary>
        /// 互动类型，可以为0和1,1为点赞，0为点踩
        /// </summary>
        [Required]
        public int Type { get; set; }
    }
}
