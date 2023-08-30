using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.Entities;

namespace Cinema.DTO.CinemaService
{
    /// <summary>
    /// 电影院DTO
    /// </summary>
    public class CinemaDTO
    {
        /// <summary>
        /// cinema_id 主键
        /// </summary>
        [Required]
        [JsonPropertyName("cinemaId")]
        public string CinemaId { get; set; } = String.Empty;

        /// <summary>
        /// 位置
        /// </summary>
        [JsonPropertyName("location")] 
        public string Location { get; set; } = String.Empty;

        /// <summary>
        /// 名字
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = String.Empty;

        /// <summary>
        /// 管理员id
        /// </summary>
        [JsonPropertyName("managerId")]
        public string ManagerId { get; set; } = String.Empty;

        /// <summary>
        /// 电影封面Url
        /// </summary>
        [JsonPropertyName("cinemaImageUrl")]
        public string? CinemaImageUrl { get; set; }

        /// <summary>
        /// 电影特点字符串，不同特点用逗号分割
        /// </summary>
        [JsonPropertyName("_feature")]
        public string? Feature { get; set; }

        /// <summary>
        /// 默认构造
        /// </summary>
        public CinemaDTO()
        {
        }

        /// <summary>
        /// 由实体类构造
        /// </summary>
        public CinemaDTO(Cinemas entity)
        {
            CinemaId = entity.CinemaId;
            Location = entity.Location;
            Name = entity.Name;
            ManagerId = entity.ManagerId;
            CinemaImageUrl = entity.CinemaImageUrl;
            Feature = entity.Feature;
        }
    }
}
