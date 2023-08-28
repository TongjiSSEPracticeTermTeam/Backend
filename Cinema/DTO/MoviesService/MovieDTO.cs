using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Cinema.DTO.MoviesService
{
    /// <summary>
    /// 电影的DTO
    /// </summary>
    public class MovieDTO
    {
        /// <summary>
        /// 电影ID
        /// </summary>
        [Required]
        [JsonPropertyName("movieId")]
        public string MovieId { get; set; } = "";

        /// <summary>
        /// 名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// 时长
        /// </summary>
        [JsonPropertyName("duration")]
        public string Duration { get; set; } = "";

        /// <summary>
        /// 介绍
        /// </summary>
        [JsonPropertyName("instruction")]
        public string? Instruction { get; set; }

        /// <summary>
        /// 分数
        /// </summary>
        [JsonPropertyName("score")]
        public float? Score { get; set; }

        /// <summary>
        /// 海报图片
        /// </summary>
        [JsonPropertyName("postUrl")]
        public string? PostUrl { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        [JsonPropertyName("tags")]
        public string? Tags { get; set; }

        /// <summary>
        /// 上映日期
        /// </summary>
        [JsonPropertyName("releaseDate")]
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// 到期日期
        /// </summary>
        [JsonPropertyName("removalDate")]
        public DateTime RemovalDate { get; set; }
    }
}
