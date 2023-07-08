using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cinema.Entities
{
    /// <summary>
    /// 电影演员表
    /// </summary>
    public class Act
    {
        /// <summary>
        /// 演员id
        /// </summary>
        [Required][Column("STAFF_ID")] public string StaffId { get; set; } = String.Empty;

        /// <summary>
        /// 电影id
        /// </summary>
        [Required][Column("MOVIE_ID")] public string MovieId { get; set; } = String.Empty;

        /// <summary>
        /// 职务
        /// </summary>
        [Required][Column("ROLE")] public string Role { get; set; } = String.Empty;

        /// <summary>
        /// 导航属性
        /// </summary>
        [JsonIgnore] public Staff Staff { get; set; } = null!;

        /// <summary>
        /// 导航属性
        /// </summary>
        [JsonIgnore] public Movie Movie { get; set; } = null!;

        /// <summary>
        /// 配置多对多
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ConfigureDbContext(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Act>()
                .HasKey(e => new { e.StaffId, e.MovieId });

            modelBuilder.Entity<Act>()
                .HasOne(e => e.Staff)
                .WithMany(s => s.Acts)
                .HasForeignKey(e => e.StaffId);

            modelBuilder.Entity<Act>()
                .HasOne(e => e.Movie)
                .WithMany(m => m.Acts)
                .HasForeignKey(e => e.MovieId);
        }
    }
}
