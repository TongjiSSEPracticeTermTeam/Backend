using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cinema.Entities;

namespace Cinema.DTO.SessionService
{
    /// <summary>
    /// 排片表
    /// </summary>
    public class SessionDTO
    {

        /// <summary>
        /// 电影ID
        /// </summary>
        [Required][Column("MOVIE_ID")] public string MovieId { get; set; } = String.Empty;

        /// <summary>
        /// 影院ID
        /// </summary>
        [Required][Column("CINEMA_ID")] public string CinemaId { get; set; } = String.Empty;

        /// <summary>
        /// 影厅ID
        /// </summary>
        [Required][Column("HALL_ID")] public string HallId { get; set; } = String.Empty;

        /// <summary>
        /// 起始时间
        /// </summary>
        [Required]
        [Column("START_TIME")]public DateTime StartTime { get; set; } = new DateTime();

        /// <summary>
        /// 上座率
        /// </summary>
        [Column("ATTENDANCE")]public float? Attendence { get; set; }

        /// <summary>
        /// 票价
        /// </summary>
        [Required][Column("PRICE")]public float Price { get; set; }

        /// <summary>
        /// 默认构造
        /// </summary>
        public SessionDTO()
        {
        }

        /// <summary>
        /// 由Session实体构造
        /// </summary>
        /// <param name="entity"></param>
        public SessionDTO(Session entity)
        {
            MovieId=entity.MovieId;
            CinemaId=entity.CinemaId;
            HallId=entity.HallId;
            StartTime=entity.StartTime;
            Attendence=entity.Attendence;
            Price=entity.Price;
        }
    }
}
