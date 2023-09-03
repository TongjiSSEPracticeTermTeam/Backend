using Cinema.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;

namespace Cinema.DTO.CommentService
{
    /// <summary>
    /// 评论DTO（管理员视图）
    /// </summary>
    public class CommentDTO
    {
        public string CommentId { get; set; } = String.Empty;

        public string Content { get; set; } = String.Empty;

        public float Score { get; set; }

        public int LikeCount { get; set; }

        public int DislikeCount { get; set; }

        public DateTime PublishDate { get; set; }

        public bool Display { get; set; }

        public string MovieId { get; set; } = String.Empty;

        public string CustomerId { get; set; } = String.Empty;

        /// <summary>
        /// 用户名称
        /// </summary>
        public string? CustomerName { get; set; } = String.Empty;

        /// <summary>
        /// 用户头像
        /// </summary>
        public string? AvatarUrl { get; set; } = String.Empty;

        /// <summary>
        /// 默认构造
        /// </summary>
        public CommentDTO()
        {
        }

        /// <summary>
        /// 由Comment实体构造
        /// </summary>
        /// <param name="comment"></param>
        public CommentDTO(Comment comment)
        {
            CommentId = comment.CommentId;
            Content = comment.Content;
            Score = comment.Score;
            LikeCount = comment.LikeCount;
            DislikeCount = comment.DislikeCount;
            Display = comment.Display;
            MovieId = comment.MovieId;
            CustomerId = comment.CustomerId;
            CustomerName = comment.Sender.Name;
            AvatarUrl = comment.Sender.AvatarUrl;
        }
    }

    public class CommentCreator
    {
        /// <summary>
        /// 评论内容
        /// </summary> 
        [Required]
        public string Content { get; set; } = String.Empty;

        /// <summary>
        /// 评分
        /// </summary>
        [Required]
        public float Score { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishDate { get; set; } = new DateTime();

        /// <summary>
        /// 电影ID
        /// </summary>
        [Required]
        public string MovieId { get; set; } = String.Empty;

        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        public string CustomerId { get; set; } = String.Empty;

        /// <summary>
        /// 默认构造
        /// </summary>
        public CommentCreator() { }
    }
}