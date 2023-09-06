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
        [Required][Key][Column("COMMENT_ID")] public string CommentId { get; set; } = String.Empty;

        /// <summary>
        /// 内容
        /// </summary>
        [Required][Column("CONTENT")] public string Content { get; set; } = String.Empty;

        /// <summary>
        /// 分数
        /// </summary>
        [Required][Column("SCORE")] public float Score { get; set; } 

        /// <summary>
        /// 点赞数
        /// </summary>
        [Required][Column("LIKE_COUNT")] public int LikeCount { get; set; } 

        /// <summary>
        /// 点踩数
        /// </summary>
        [Required][Column("DISLIKE_COUNT")] public int DislikeCount { get; set; } 

        /// <summary>
        /// 发送日期
        /// </summary>
        [Required][Column("PUBLISH_TIME")] public DateTime PublishDate { get; set; } = new DateTime();

        /// <summary>
        /// 是否展示
        /// </summary>
        [Required][Column("DISPLAY")] public string Display { get; set; } = "1";

        /// <summary>
        /// 所属电影id
        /// </summary>
        [Required][Column("MOVIE_ID")] public string MovieId { get; set; } = String.Empty;

        /// <summary>
        /// 发送用户id
        /// </summary>
        [Required][Column("CUSTOMER_ID")] public string CustomerId { get; set; } = String.Empty;

        /// <summary>
        /// 导航属性 - 所属电影
        /// </summary>
        [JsonIgnore] public Movie MovieBelongTo { get; set; } = null!;

        /// <summary>
        /// 导航属性 - 发送用户
        /// </summary>
        public Customer Sender { get; set; } = null!;

        /// <summary>
        /// 导航属性 - 互动
        /// </summary>
        [JsonIgnore] public ICollection<Interaction> Interactions { get; set; } = null!;

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
