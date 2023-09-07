using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.Entities;
using Cinema.DTO.CinemaService;

namespace Cinema.DTO.ManagerService
{
    /// <summary>
    /// 影厅数据总览DTO
    /// </summary>
    public class OverviewDTO
    {
        /// <summary>
        /// 总体数据
        /// </summary>
        public GlobalData GlobalData { get; set; } = new GlobalData();

        /// <summary>
        /// 电影数据
        /// </summary>
        public List<CinemaMovieData> MovieDatas { get; set; } = new List<CinemaMovieData>();

        /// <summary>
        /// 默认构造
        /// </summary>
        public OverviewDTO() { }
    }
}
