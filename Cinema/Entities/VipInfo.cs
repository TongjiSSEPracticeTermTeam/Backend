using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cinema.Entities
{
    /// <summary>
    /// VIP身份表
    /// </summary>
    public class VipInfo
    {
        /// <summary>
        /// 客户ID
        /// </summary>
        [Required]
        [Key]
        [Column("CUSTOMER_ID")]
        public string CustomerId { get; set; } = string.Empty;

        /// <summary>
        /// 到期时间
        /// </summary>
        [Required]
        [Column("END_TIME")]
        public DateTime EndDate { get; set; } = DateTime.MinValue;

        /// <summary>
        /// 导航属性 - 所有者
        /// </summary>
        [JsonIgnore] public Customer Owner { get; set; } = null!;

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ConfigureDbContext(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VipInfo>()
                .HasOne(e => e.Owner)
                .WithOne(o => o.Vip)
                .HasForeignKey<VipInfo>(o => o.CustomerId);
        }
    }
}
