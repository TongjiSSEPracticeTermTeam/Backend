using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json.Serialization;
using Cinema.DTO.StaffService;
using Cinema.Entities;
using TencentCloud.Pds.V20210701.Models;
using TencentCloud.Ssl.V20191205.Models;

namespace Cinema.DTO.CustomerService
{
    /// <summary>
    /// customer的DTO
    /// </summary>
    public class CustomerDTO
    {

        /// <summary>
        /// ID
        /// </summary>
        [Required]
        [JsonPropertyName("customerId")]
        public string CustomerId { get; set; } = "";

        /// <summary>
        /// 用户名
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// 邮箱
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = "";

        /// <summary>
        /// 头像url
        /// </summary>
        [JsonPropertyName("avatarUrl")]
        public string AvatarUrl { get; set; } = "";

        /// <summary>
        /// 默认构造
        /// </summary>
        public CustomerDTO()
        {
        }

        /// <summary>
        /// 实体构造
        /// </summary>
        /// <param name="entity"></param>
        public CustomerDTO(Customer entity)
        {
            CustomerId = entity.CustomerId;
            Name = entity.Name ?? "";
            Email = entity.Email ?? "";
            AvatarUrl = entity.AvatarUrl ?? "";
        }

    }
}
