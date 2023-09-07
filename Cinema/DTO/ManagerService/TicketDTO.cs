using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.Entities;
using Cinema.DTO.CinemaService;

namespace Cinema.DTO.ManagerService
{
    /// <summary>
    /// 电影票DTO
    /// </summary>
    public class TicketDTO
    {
        /// <summary>
        /// 影票ID
        /// </summary>
        [Required]
        [JsonPropertyName("ticketId")]
        public string Id { get; set; } = String.Empty;

        /// <summary>
        /// 影票状态
        /// </summary>
        [JsonPropertyName("state")]
        public TicketState State { get; set; } = TicketState.normal;

        /// <summary>
        /// 票价
        /// </summary>
        [JsonPropertyName("price")]
        public float Price { get; set; }

        /// <summary>
        /// 电影编号
        /// </summary>
        [JsonPropertyName("movieId")]
        public string MovieId { get; set; } = String.Empty;

        /// <summary>
        /// 开始时间
        /// </summary>
        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 影院编号
        /// </summary>
        [JsonPropertyName("cinemaId")]
        public string CinemaId { get; set; } = String.Empty;

        /// <summary>
        /// 影厅
        /// </summary>
        [JsonPropertyName("hallId")]
        public string HallId { get; set; } = String.Empty;

        /// <summary>
        /// 导航属性 - 所属排片
        /// </summary>
        [JsonPropertyName("sessionId")]
        public Session SessionAt { get; set; } = null!;

        /// <summary>
        /// 默认构造
        /// </summary>
        public TicketDTO() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entity"></param>
        public TicketDTO(Ticket entity)
        {
            Id = entity.Id;
            State = entity.State;
            Price = entity.Price;
            MovieId = entity.MovieId;
            StartTime = entity.StartTime;
            CinemaId = entity.CinemaId;
            HallId = entity.HallId;
            SessionAt = entity.SessionAt;
        }
    }
}
