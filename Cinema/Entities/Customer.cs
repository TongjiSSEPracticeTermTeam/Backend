using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Entities;

/// <summary>
///     用户
/// </summary>
public class Customer
{
    /// <summary>
    /// 用户id
    /// </summary>
    [Required] [Column("CUSTOMER_ID")] public string CustomerId { get; set; } = null!;

    /// <summary>
    /// 用户名
    /// </summary>
    [Column("NAME")] public string? Name { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Column("PASSWORD")] public string? Password { get; set; }

    /// <summary>
    /// 邮件
    /// </summary>
    [Column("EMAIL")] public string? Email { get; set; }

    /// <summary>
    /// 头像链接
    /// </summary>
    [Column("AVATAR_URL")] public string? AvatarUrl { get; set; }

    /// <summary>
    /// 导航属性 - 评论）
    /// </summary>
    [JsonIgnore] public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    /// <summary>
    /// 配置
    /// </summary>
    /// <param name="modelBuilder"></param>
    public static void ConfigureDbContext(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasKey(c => c.CustomerId);
    }
}