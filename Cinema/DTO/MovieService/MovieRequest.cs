using System.Text.Json.Serialization;

namespace Cinema.DTO.MovieService;

/// <summary>
/// 添加电影的请求体定义
/// </summary>
public class AddMovieRequest
{
    /// <summary>
    /// 电影ID
    /// </summary>
    [JsonPropertyName("movie_id")] public string MovieId { get; set; } = String.Empty;

    /// <summary>
    /// 电影名称
    /// </summary>
    [JsonPropertyName("name")] public string Name { get; set; } = String.Empty;

    /// <summary>
    /// 电影时长
    /// </summary>
    [JsonPropertyName("duration")] public string Duration { get; set; } = String.Empty;

    /// <summary>
    /// 电影介绍
    /// </summary>
    [JsonPropertyName("instruction")] public string Instruction { get; set; } = String.Empty;

    /// <summary>
    /// 电影海报URL
    /// </summary>
    [JsonPropertyName("poster_url")] public string PostUrl { get; set; } = String.Empty;

    /// <summary>
    /// 电影标签
    /// </summary>
    [JsonPropertyName("tags")] public string Tags { get; set; } = String.Empty;

    /// <summary>
    /// 上映日期
    /// </summary>
    [JsonPropertyName("release_date")] public string ReleaseDate { get; set; } = String.Empty;

    /// <summary>
    /// 到期日期
    /// </summary>
    [JsonPropertyName("removal_date")] public string RemovalDate { get; set; } = String.Empty;
}