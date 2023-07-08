using System.Text.Json.Serialization;

namespace Cinema.DTO.CinemaService;

/// <summary>
/// 添加电影院的请求体定义
/// </summary>
public class AddCinemaRequest
{
    /// <summary>
    /// 电影院id
    /// </summary>
    [JsonPropertyName("cinema_id")] public string CinemaId { get; set; } = String.Empty;

    /// <summary>
    /// 电影位置
    /// </summary>
    [JsonPropertyName("location")] public string Location { get; set; } = String.Empty;

    /// <summary>
    /// 电影名字
    /// </summary>
    [JsonPropertyName("name")] public string Name { get; set; } = String.Empty;

    /// <summary>
    /// 管理员id
    /// </summary>
    [JsonPropertyName("manager_id")] public string ManagerId { get; set; } = String.Empty;

    /// <summary>
    /// 电影封面外链地址
    /// </summary>
    [JsonPropertyName("cinema_image_url")] public string? CinemaImageUrl { get; set; }
    
    
    /// <summary>
    /// 电影特点
    /// </summary>
    [JsonPropertyName("feature")] public string? Feature { get; set; }
}