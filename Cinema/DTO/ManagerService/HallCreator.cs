using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.Entities;
using Cinema.DTO.CinemaService;

namespace Cinema.DTO.ManagerService
{
    /// <summary>
    /// 影厅添加用接口
    /// </summary>
    public class HallCreator : HallDTO
    {
        /// <summary>
        /// 管理员名称
        /// </summary>
        [Required]
        [JsonPropertyName("managerName")]
        public string ManagerName { get; set; } = String.Empty;

        /// <summary>
        /// 管理员密码
        /// </summary>
        [Required]
        [JsonPropertyName("managerPassword")]
        public string ManagerPassword { get; set; } = String.Empty;

        /// <summary>
        /// 管理员邮箱
        /// </summary>
        [Required]
        [JsonPropertyName("managerEmail")]
        public string ManagerEmail { get; set; } = String.Empty;
    }

}
