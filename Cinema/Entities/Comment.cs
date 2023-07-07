using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cinema.Entities
{
    /// <summary>
    /// 评论
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// 评论id
        /// </summary>
        [Required][Key][Column("COMMENT_ID")] public string CommentId { get; set; } = null!;

        /// <summary>
        /// 内容
        /// </summary>
        [Required][Column("CONTENT")] public string Content { get; set; } = null!;

        /// <summary>
        /// 分数
        /// </summary>
        [Required][Column("SCORE")] public float Score { get; set; } = null!;

        /// <summary>
        /// 点赞数
        /// </summary>
        [Required][Column("LIKE_COUNT")] public int LikeCount { get; set; } = null!;

        /// <summary>
        /// 点踩数
        /// </summary>
        [Required][Column("DISLIKE_COUNT")] public int DislikeCount { get; set; } = null!;

        /// <summary>
        /// 发送日期
        /// </summary>
        [Required][Column("PUBLISH_TIME")] public DateTime PublishDate { get; set; } = new DateTime();

        /// <summary>
        /// 阅读数
        /// </summary>
        [Required][Column("DISPLAY")] public int Display { get; set; }

        /// <summary>
        /// 所属电影id
        /// </summary>
        [Required][Column("MOVIE_ID")] public string MovieId { get; set; } = null!;

        /// <summary>
        /// 发送用户id
        /// </summary>
        [Required][Column("CUSTOMER_ID")] public string CustomerId { get; set; } = null!;

        /// <summary>
        /// 导航属性 - 所属电影
        /// </summary>
        [JsonIgnore] public Movie MovieBelongTo { get; set; } = null!;

        /// <summary>
        /// 导航属性 - 发送用户
        /// </summary>
        public Customer Sender { get; set; } = null!;

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ConfigureDbContext(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>()
                .HasOne(a => a.MovieBelongTo)
                .WithMany(m => m.Comments)
                .HasForeignKey(a => a.MovieId);

            modelBuilder.Entity<Comment>()
                .HasOne(a => a.Sender)
                .WithMany(s => s.Comments)
                .HasForeignKey(e => e.CustomerId);
        }

    }
}
