using Cinema.Entities;
using System.Text.Json.Serialization;

namespace Cinema.DTO.MoviesService
{
    /// <summary>
    /// 获取电影数量的返回值
    /// </summary>
    public class QueryMovieNumResponse:APIResponse
    {
        /// <summary>
        /// 电影数据
        /// </summary>
        [JsonPropertyName("data")]
        public int Length { get; set; }
    }
}
