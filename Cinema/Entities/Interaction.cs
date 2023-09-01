using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cinema.Entities
{
    /// <summary>
    /// 评论互动
    /// </summary>
    public class Interaction
    {
        /// <summary>
        /// 评论ID
        /// </summary>
        [Required]
        [Column("COMMENT_ID")]
        public string CommentId { get; set; } = String.Empty;

        /// <summary>
        /// 客户ID
        /// </summary>
        [Required]
        [Column("CUSTOMER_ID")]
        public string CustomerId { get; set; } = String.Empty;

        /// <summary>
        /// 互动类型，可以为0和1,1为点赞，0为点踩
        /// </summary>
        [Required]
        [Column("TYPE")]
        public int Type { get; set; }

        /// <summary>
        /// 导航属性 - 评论
        /// </summary>
        [JsonIgnore] public Comment Comment { get; set; } = null!;

        /// <summary>
        /// 导航属性 - 顾客
        /// </summary>
        [JsonIgnore] public Customer Customer { get; set; } = null!;

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ConfigureDbContext(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Interaction>()
                .HasKey(e => new { e.CommentId, e.CustomerId });

            modelBuilder.Entity<Interaction>()
                .HasOne(i => i.Comment)
                .WithMany(Comment => Comment.Interactions)
                .HasForeignKey(i => i.CommentId);

            modelBuilder.Entity<Interaction>()
                .HasOne(i => i.Customer)
                .WithMany(Customer => Customer.Interactions)
                .HasForeignKey(i => i.CustomerId);
        }
    }
}
