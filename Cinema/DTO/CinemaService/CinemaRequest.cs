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
    /// 电影院位置
    /// </summary>
    [JsonPropertyName("location")] public string Location { get; set; } = String.Empty;

    /// <summary>
    /// 电影院名字
    /// </summary>
    [JsonPropertyName("name")] public string Name { get; set; } = String.Empty;

    /// <summary>
    /// 管理员id
    /// </summary>
    [JsonPropertyName("manager_id")] public string ManagerId { get; set; } = String.Empty;

    /// <summary>
    /// 电影院封面外链地址
    /// </summary>
    [JsonPropertyName("cinema_image_url")] public string? CinemaImageUrl { get; set; }
    
    
    /// <summary>
    /// 电影院特点
    /// </summary>
    [JsonPropertyName("feature")] public string? Feature { get; set; }
}

/// <summary>
/// 修改电影院的请求体定义
/// </summary>
public class UpdateCinemaRequest
{
    /// <summary>
    /// 电影院id
    /// </summary>
    [JsonPropertyName("cinema_id")] public string CinemaId { get; set; } = String.Empty;

    /// <summary>
    /// 电影院位置
    /// </summary>
    [JsonPropertyName("location")] public string Location { get; set; } = String.Empty;

    /// <summary>
    /// 电影院名字
    /// </summary>
    [JsonPropertyName("name")] public string Name { get; set; } = String.Empty;

    /// <summary>
    /// 管理员id
    /// </summary>
    [JsonPropertyName("manager_id")] public string ManagerId { get; set; } = String.Empty;

    /// <summary>
    /// 电影院封面外链地址
    /// </summary>
    [JsonPropertyName("cinema_image_url")] public string? CinemaImageUrl { get; set; }


    /// <summary>
    /// 电影院特点
    /// </summary>
    [JsonPropertyName("feature")] public string? Feature { get; set; }
}
