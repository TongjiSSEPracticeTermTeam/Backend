using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.Entities;
using Cinema.DTO.StaffService;

namespace Cinema.DTO.MoviesInHallService
{
    /// <summary>
    /// 电影的DTO
    /// </summary>
    public class MovieInHallDTO
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
        /// 默认构造
        /// </summary>
        public MovieInHallDTO()
        {
        }

        /// <summary>
        /// 实体构造
        /// </summary>
        /// <param name="entity"></param>
        public MovieInHallDTO(Movie entity)
        {
            MovieId = entity.MovieId;
            Name = entity.Name;
            Duration = entity.Duration;
            PostUrl = entity.PostUrl;
            Tags = entity.Tags;
        }

    }
}
