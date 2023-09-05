using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.Entities;

namespace Cinema.DTO.ManagerService
{
    /// <summary>
    /// 影厅管理DTO
    /// </summary>
    public class HallDTO
    {
        /// <summary>
        /// hall_id 主键
        /// </summary>
        [Required]
        [JsonPropertyName("hallID")]
        public string Id { get; set; } = String.Empty;

        /// <summary>
        /// cinema_id 所属影院
        /// </summary>
        [Required]
        [JsonPropertyName("cinemaId")]
        public string CinemaId { get; set; } = String.Empty;

        /// <summary>
        /// hall_type 影厅类型
        /// </summary>
        [JsonPropertyName("hallType")]
        public string? HallType { get; set; } = String.Empty;

        /// <summary>
        /// seat 座位信息
        /// </summary>
        [JsonPropertyName("seat")]
        public Seat? Seat { get; set; } = new Seat();

        /// <summary>
        /// 默认构造
        /// </summary>
        public HallDTO()
        {
        }

        /// <summary>
        /// 由Hall实体构造
        /// </summary>
        /// <param name="entity"></param>
        public HallDTO(Hall entity)
        {
            Id = entity.Id;
            CinemaId=entity.CinemaId;
            HallType = entity.HallType;
            Seat = entity.Seat;
        }
    }
    /// <summary>
    /// 影厅添加用接口
    /// </summary>
    public class HallCreator: HallDTO
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
