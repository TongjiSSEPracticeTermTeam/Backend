using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cinema.Entities
{
    public class Hall
    {
        [Required][Column("HALL_ID")] public string Id { get; set; } = null!;

        [Required][Column("CINEMA_ID")] public string CinemaId { get; set; } = null!;

        [Required][Column("ROW")] public int RowCount { get; set; }

        [Required][Column("COL")] public int ColumnCount { get; set; }

        [Column("HALL_TYPE")] public string? HallType { get; set; }

        [JsonIgnore] public Cinemas CinemaBelongTo { get; set; }

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ConfigureDbContext(ModelBuilder modelBuilder)
        {

        }
    }
}
