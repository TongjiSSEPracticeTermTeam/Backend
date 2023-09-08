using Cinema.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.DTO.MoviesService;

namespace Cinema.DTO.SessionService
{
    /// <summary>
    /// 客户端展示用排片
    /// </summary>
    public class SessionDisplay
    {
        /// <summary>
        /// 电影ID
        /// </summary>
        [Required]
        public string MovieId { get; set; } = String.Empty;

        /// <summary>
        /// 电影名
        /// </summary>
        [Required]
        public string MovieName{ get; set; } = String.Empty;

        /// <summary>
        /// 影院ID
        /// </summary>
        [Required]
        public string CinemaId { get; set; } = String.Empty;

        /// <summary>
        /// 影厅ID
        /// </summary>
        [Required]
        public string HallId { get; set; } = String.Empty;

        /// <summary>
        /// 起始时间
        /// </summary>
        [Required]
        public DateTime StartTime { get; set; } = new DateTime();

        /// <summary>
        /// 结束时间
        /// </summary>
        [Required]
        public DateTime EndTime { get; set; } = new DateTime();

        /// <summary>
        /// 上座率
        /// </summary>
        public float? Attendence { get; set; }

        /// <summary>
        /// 票价
        /// </summary>
        [Required]
        public float Price { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        [Required]
        public string Language { get; set; } = String.Empty;

        /// <summary>
        /// 播放维数（3d、2d）
        /// </summary>
        [Required]
        public string Dimesion { get; set; } = String.Empty;

        /// <summary>
        /// 导航属性 - 所位于影厅
        /// </summary>
        public Hall HallLocatedAt { get; set; } = null!;

        /// <summary>
        /// 默认构造
        /// </summary>
        public SessionDisplay() { }

        /// <summary>
        /// 尸体构造
        /// </summary>
        /// <param name="entity"></param>
        public SessionDisplay(Session entity)
        {
            MovieId = entity.MovieId;
            MovieName = entity.MovieBelongsTo.Name;
            CinemaId = entity.CinemaId;
            HallId = entity.HallId;
            StartTime = entity.StartTime;
            EndTime = entity.StartTime.AddMinutes(double.Parse(entity.MovieBelongsTo.Duration));
            Attendence = entity.Attendence;
            Price = entity.Price;
            Language = entity.Language;
            Dimesion = entity.Dimesion;
            HallLocatedAt = entity.HallLocatedAt;
        }
    }
}
