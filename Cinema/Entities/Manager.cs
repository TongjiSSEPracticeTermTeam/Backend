using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cinema.Entities
{
    /// <summary>
    /// 经理表
    /// </summary>
    public class Manager
    {
        /// <summary>
        /// 经理Id
        /// </summary>
        [Required][Key][Column("MANAGER_ID")] public string Id { get; set; } = String.Empty;

        /// <summary>
        /// 姓名
        /// </summary>
        [Required][Column("NAME")] public string Name { get; set; } = String.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        [Required][Column("PASSWORD")] public string Password { get; set; } = String.Empty;

        /// <summary>
        /// 邮箱
        /// </summary>
        [Column("EMAIL")] public string? Email { get; set; } 

        /// <summary>
        /// 头像地址
        /// </summary>
        [Column("AVATAR_URL")] public string? AvatarUrl { get; set; }

        /// <summary>
        /// 导航属性 - 管理的影厅
        /// </summary>
        [JsonIgnore] public Cinemas ManagedCinema { get; set; } = null!;

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ConfigureDbContext(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Manager>()
                .HasOne(m => m.ManagedCinema)
                .WithOne(c => c.Manager)
                .HasForeignKey<Cinemas>(c => c.ManagerId);
        }
    }
}
