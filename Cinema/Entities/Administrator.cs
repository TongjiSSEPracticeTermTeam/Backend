using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Entities
{
    /// <summary>
    /// 管理员表
    /// </summary>
    public class Administrator
    {
        /// <summary>
        /// 管理员Id
        /// </summary>
        [Required][Key][Column("ADMINISTRATOR_ID")] public string Id { get; set; } = String.Empty;

        /// <summary>
        /// 管理员密码
        /// </summary>
        [Required][Column("PASSWORD")] public string Password { get; set; } = String.Empty;

        /// <summary>
        /// 头像链接
        /// </summary>
        [Column("AVATAR_URL")] public string? AvatarUrl { get; set; }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ConfigureDbContext(ModelBuilder modelBuilder)
        {
        }
    }
}
