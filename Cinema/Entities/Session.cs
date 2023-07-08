using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cinema.Entities
{
    /// <summary>
    /// 排片表
    /// </summary>
    public class Session
    {
        /// <summary>
        /// 电影ID
        /// </summary>
        [Required]
        [Column("MOVIE_ID")]
        public string MovieId { get; set; } = String.Empty;

        /// <summary>
        /// 影院ID
        /// </summary>
        [Required]
        [Column("CINEMA_ID")]
        public string CinemaId { get; set; } = String.Empty;

        /// <summary>
        /// 影厅ID
        /// </summary>
        [Required]
        [Column("HALL_ID")]
        public string HallId { get; set; } = String.Empty;

        /// <summary>
        /// 起始时间
        /// </summary>
        [Required]
        [Column("START_TIME")]
        public DateTime StartTime { get; set; } = new DateTime();

        /// <summary>
        /// 上座率
        /// </summary>
        [Column("ATTENDANCE")]
        public float? Attendence { get; set; }

        /// <summary>
        /// 票价
        /// </summary>
        [Required]
        [Column("PRICE")]
        public float Price { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        [Required]
        [Column("LANGUAGE")]
        public string Language { get; set; } = String.Empty;

        /// <summary>
        /// 播放维数（3d、2d）
        /// </summary>
        [Required]
        [Column("DIMESION")]
        public string Dimesion { get; set; } = String.Empty;

        /// <summary>
        /// 导航属性 - 所属电影
        /// </summary>
        [JsonIgnore] public Movie MovieBelongsTo { get; set; } = null!;

        /// <summary>
        /// 导航属性 - 所位于影厅
        /// </summary>
        [JsonIgnore] public Hall HallLocatedAt { get; set; } = null!;

        /// <summary>
        /// 导航属性 - 所关联影票
        /// </summary>
        [JsonIgnore] public ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ConfigureDbContext(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Session>()
                .HasKey(e => new { e.HallId, e.CinemaId, e.MovieId, e.StartTime });

            modelBuilder.Entity<Session>()
                .HasOne(e => e.MovieBelongsTo)
                .WithMany(m => m.Sessions)
                .HasForeignKey(e => e.MovieId);

            modelBuilder.Entity<Session>()
                .HasOne(e => e.HallLocatedAt)
                .WithMany(h => h.Sessions)
                .HasForeignKey(e => new { e.HallId, e.CinemaId});
        }
    }
}
