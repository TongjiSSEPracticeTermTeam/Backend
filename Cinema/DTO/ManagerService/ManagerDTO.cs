using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.Entities;
using Cinema.DTO.CinemaService;

namespace Cinema.DTO.ManagerService
{
    /// <summary>
    /// 影院经理DTO
    /// </summary>
    public class ManagerDTO
    {
        /// <summary>
        /// manager_id 主键
        /// </summary>
        [Required]
        [JsonPropertyName("managerID")]
        public string Id { get; set; } = String.Empty;

        /// <summary>
        /// name 姓名
        /// </summary>
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; } = String.Empty;

        /// <summary>
        /// password 密码
        /// </summary>
        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; } = String.Empty;

        /// <summary>
        /// email 邮箱
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; } = String.Empty;

        /// <summary>
        /// avatarUrl 头像地址
        /// </summary>
        [JsonPropertyName("avatarUrl")]
        public string? AvatarUrl { get; set; } = String.Empty;

        /// <summary>
        /// 管理的影院 
        /// </summary>
        [JsonPropertyName("managedCinema")]
        public CinemaDTO? ManagedCinema { get; set; }

        /// <summary>
        /// 默认构造
        /// </summary>
        public ManagerDTO()
        {
        }

        /// <summary>
        /// 由Manager实体构造
        /// </summary>
        /// <param name="entity"></param>
        public ManagerDTO(Manager entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Password = entity.Password;
            Email = entity.Email;
            AvatarUrl = entity.AvatarUrl;

            if (entity.ManagedCinema != null)
            {
                ManagedCinema = new CinemaDTO(entity.ManagedCinema);
            }
        }
    }
}
