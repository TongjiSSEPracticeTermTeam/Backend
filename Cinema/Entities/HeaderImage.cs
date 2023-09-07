using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cinema.Entities
{
    /// <summary>
    /// 头图表
    /// </summary>
    public class HeaderImage
    {
        /// <summary>
        /// 头图ID
        /// </summary>
        [Required][Column("ID")] public int Id { get; set; }

        /// <summary>
        /// 图片URL
        /// </summary>
        [Required][Column("URL")] public string Url { get; set; } = String.Empty;

        /// <summary>
        /// 所属电影
        /// </summary>
        [Column("MOVIE_ID")] public string MovieId { get; set; } = String.Empty;

        /// <summary>
        /// 导航属性：所属电影
        /// </summary>
        public Movie Movie { get; set; } = null!;

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ConfigureDbContext(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HeaderImage>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<HeaderImage>()
                .HasOne(e => e.Movie)
                .WithMany(m => m.HeaderImages)
                .HasForeignKey(e => e.MovieId);
        }
    }
}
