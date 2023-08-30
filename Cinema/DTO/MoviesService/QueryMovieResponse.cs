using Cinema.Entities;
using System.Text.Json.Serialization;

namespace Cinema.DTO.MoviesService
{
    /// <summary>
    /// 获取电影数组的返回值
    /// </summary>
    public class QueryMovieResponse:APIResponse
    {
        /// <summary>
        /// 电影数据
        /// </summary>
        [JsonPropertyName("data")]
        public Movie[]? Data { get; set; }
    }
}
