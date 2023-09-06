using Cinema.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.DTO.MoviesService;
using TencentCloud.Bda.V20200324.Models;
using TencentCloud.Ecm.V20190719.Models;

namespace Cinema.DTO.StaffService
{
    /// <summary>
    /// 演职员DTO
    /// </summary>
    public class StaffDTO
    {
        /// <summary>
        /// 演员Id
        /// </summary>
        [Required]
        [JsonPropertyName("staffId")]
        public string StaffId { get; set; } = "";

        /// <summary>
        /// 姓名
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// 性别
        /// </summary>
        [JsonPropertyName("gender")]
        public Gender_ Gender { get; set; } = 0;

        /// <summary>
        /// 介绍
        /// </summary>
        [JsonPropertyName("introduction")]
        public string? Introduction { get; set; }

        /// <summary>
        /// 图片链接
        /// </summary>
        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// 默认构造
        /// </summary>
        public StaffDTO()
        {
        }

        /// <summary>
        /// 实体构造
        /// </summary>
        /// <param name="entity"></param>
        public StaffDTO(Staff entity)
        {
            StaffId = entity.StaffId;
            Name = entity.Name;
            Gender = entity.Gender;
            Introduction = entity.Introduction;
            ImageUrl = entity.ImageUrl;
        }
    }

    /// <summary>
    /// 影人名字和ID
    /// </summary>
    public class EStaff
    {
        /// <summary>
        /// 影人ID
        /// </summary>
        [Required]
        [JsonPropertyName("staffId")]
        public string StaffId { get; set; } = String.Empty;

        /// <summary>
        /// 影人名字
        /// </summary>
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; } = String.Empty;

        /// <summary>
        /// 默认构造
        /// </summary>
        public EStaff()
        {
        }

        /// <summary>
        /// 实体构造
        /// </summary>
        /// <param name="entity"></param>
        public EStaff(Staff entity)
        {
            StaffId = entity.StaffId;
            Name = entity.Name;
        }
    }

    /// <summary>
    /// 影人详细信息（含出演电影）
    /// </summary>
    public class StaffDetail : StaffDTO
    {
        /// <summary>
        /// 导演电影
        /// </summary>
        public List<EMovie>? directMovies { get; set; }

        /// <summary>
        /// 参演电影
        /// </summary>
        public List<EMovie>? starMovies { get; set; }

        /// <summary>
        /// 默认构造
        /// </summary>
        public StaffDetail() { }

        /// <summary>
        /// 实体构造
        /// </summary>
        /// <param name="entity"></param>
        public StaffDetail(Staff entity) : base(entity)
        {
            if (directMovies == null)
            {
                directMovies = new List<EMovie>();
            }

            if (starMovies == null)
            {
                starMovies = new List<EMovie>();
            }

            directMovies = entity.Acts
                .Where(a => a.Role == "1")
                .Select(a => new EMovie(a.Movie))
                .ToList();

            starMovies = entity.Acts
                .Where(a => a.Role == "0")
                .Select(a => new EMovie(a.Movie))
                .ToList();
        }
    }
}
