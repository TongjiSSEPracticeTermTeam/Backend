using System.Text.Json.Serialization;

namespace Cinema.DTO.BoxOfficeService
{
    /// <summary>
    /// 票房数据
    /// </summary>
    public class BoxOfficeData
    {
        /// <summary>
        /// 通用数据
        /// </summary>
        public GeneralData GereralData { get; set; } = new GeneralData();

        /// <summary>
        /// 影片数据
        /// </summary>
        public List<MovieData> MovieData { get; set; } = new List<MovieData>();
    }

    /// <summary>
    /// 通用数据
    /// </summary>
    public class GeneralData
    {
        /// <summary>
        /// 数据更新时间
        /// </summary>
        public string? UpdateTime { get; set; }

        /// <summary>
        /// 总票房
        /// </summary>
        public double SumBoxOffice { get; set; }

        /// <summary>
        /// 总场次
        /// </summary>
        public double SumShowCount { get; set; }

        /// <summary>
        /// 总人次
        /// </summary>
        public double SumAudienceCount { get; set; }
    }

    /// <summary>
    /// 影片数据
    /// </summary>
    public class MovieData
    {
        /// <summary>
        /// 影片名称
        /// </summary>
        public string? MovieName { get; set; }

        /// <summary>
        /// 排名
        /// </summary>
        public long Irank { get; set; }

        /// <summary>
        /// 英文电影名称
        /// </summary>
        public string? EnMovieName { get; set; }

        /// <summary>
        /// 当日票房
        /// </summary>
        public double BoxOffice { get; set; }

        /// <summary>
        /// 当日场次
        /// </summary>
        public long ShowCount { get; set; }

        /// <summary>
        /// 当日观众
        /// </summary>
        public long AudienceCount { get; set; }
    }
}
