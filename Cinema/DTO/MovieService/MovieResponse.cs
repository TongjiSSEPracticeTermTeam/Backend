using System.Text.Json.Serialization;
using Cinema.Entities;

namespace Cinema.DTO.MovieService;

/// <summary>
/// 通过电影id查询演员列表
/// </summary>
public class GetActorsByMovieIdResponse : IAPIResponse
{
    /// <sumamary>
    /// 演员实体
    /// </sumamary>
    [JsonPropertyName("act")] public ICollection<Act>? Act { get; set; }

    /// <summary>
    /// 响应状态
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// 响应信息
    /// </summary>
    public string Message { get; set; } = String.Empty;
}
