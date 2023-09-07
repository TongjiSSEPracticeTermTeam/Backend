using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cinema.Entities
{
    /// <summary>
    /// 影厅表
    /// </summary>
    public class Hall
    {
        /// <summary>
        /// 影厅ID
        /// </summary>
        [Required][Column("HALL_ID")] public string Id { get; set; } = String.Empty;

        /// <summary>
        /// 所属影院
        /// </summary>
        [Required][Column("CINEMA_ID")] public string CinemaId { get; set; } = String.Empty;

        /// <summary>
        /// 影厅类型
        /// </summary>
        [Column("HALL_TYPE")] public string? HallType { get; set; }

        /// <summary>
        /// 座位信息
        /// </summary>
        [Column("SEAT")] public Seat? Seat { get; set; }

        /// <summary>
        /// 导航属性 - 所属影院
        /// </summary>
        public Cinemas CinemaBelongTo { get; set; } = null!;

        /// <summary>
        /// 导航属性 - 排片
        /// </summary>
        [JsonIgnore] public ICollection<Session> Sessions { get; set; } = new List<Session>();

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ConfigureDbContext(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hall>()
                .HasKey(e => new { e.Id, e.CinemaId });

            modelBuilder.Entity<Hall>()
                .HasOne(h => h.CinemaBelongTo)
                .WithMany(c => c.Halls)
                .HasForeignKey(h => h.CinemaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Hall>().Property(h => h.Seat).HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
                v => JsonSerializer.Deserialize<Seat>(v, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull })
            );
        }
    }
}
