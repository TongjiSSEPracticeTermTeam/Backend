using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cinema.Entities
{
    /// <summary>
    /// 头图
    /// </summary>
    public class HeaderImageDTO
    {
        /// <summary>
        /// 头图ID
        /// </summary>
        [Column("ID")] public int Id { get; set; }

        /// <summary>
        /// 图片URL
        /// </summary>
        [Required][Column("URL")] public string Url { get; set; } = String.Empty;

        /// <summary>
        /// 所属电影
        /// </summary>
        [Required][Column("MOVIE_ID")] public string MovieId { get; set; } = String.Empty;
    }
}
