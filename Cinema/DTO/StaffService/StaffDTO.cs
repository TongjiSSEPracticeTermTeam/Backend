using Cinema.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        public string Gender { get; set; } = "0";

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
            Gender = entity.Gender.ToString();
            Introduction = entity.Introduction;
            ImageUrl = entity.ImageUrl;
        }
    }
}
