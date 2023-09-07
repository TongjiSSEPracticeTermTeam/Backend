using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.Entities;
using Cinema.DTO.CinemaService;

namespace Cinema.DTO.ManagerService
{
    /// <summary>
    /// 电影数据
    /// </summary>
    public class CinemaMovieData
    {
        /// <summary>
        /// 电影名
        /// </summary>
        public string? MovieName { get; set; }

        /// <summary>
        /// 总票房
        /// </summary>
        public double TotalBoxOffice { get; set; }

        /// <summary>
        /// 上座率
        /// </summary>
        public float Attendance { get; set; }

        /// <summary>
        /// 观影人次
        /// </summary>
        public int AudienceNumber { get; set; }

        /// <summary>
        /// 首映日期
        /// </summary>
        public DateTime PremiereDate { get; set; }

        /// <summary>
        /// 默认构造
        /// </summary>
        public CinemaMovieData() { }
    }
}
