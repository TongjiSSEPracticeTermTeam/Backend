using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.Entities;
using Cinema.DTO.CinemaService;

namespace Cinema.DTO.ManagerService
{
    /// <summary>
    /// 总体数据
    /// </summary>
    public class GlobalData
    {
        /// <summary>
        /// 今日影院总票房
        /// </summary>
        public double TotalBoxOfficeToday { get; set; }

        /// <summary>
        /// 今日影院总观影人次
        /// </summary>
        public int AudienceNumberToday { get; set; }

        /// <summary>
        /// 默认构造
        /// </summary>
        public GlobalData() { }
    }
}
