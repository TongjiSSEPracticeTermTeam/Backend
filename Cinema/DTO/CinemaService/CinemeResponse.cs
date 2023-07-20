using System.Text.Json.Serialization;
using Cinema.Entities;

namespace Cinema.DTO.CinemaService;

/// <summary>
/// 添加电影院响应体
/// </summary>
public class AddCinemaResponse : IAPIResponse
{
    /// <summary>
    /// 电影院实体
    /// </summary>
    [JsonPropertyName("cinema")] public Cinemas? Cinema { get; set; }

    /// <summary>
    /// 响应状态
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = String.Empty;
}

/// <summary>
/// 通过id查询电影院响应
/// </summary>
public class GetCinemaByIdResponse : IAPIResponse
{
    /// <summary>
    /// 电影院实体
    /// </summary>
    [JsonPropertyName("cinema")] public Cinemas? Cinema { get; set; }

    /// <summary>
    /// 响应状态
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = String.Empty;
}

/// <summary>
/// 通过管理员Id查询电影院响应
/// </summary>
public class GetCinemaByManagerIdResponse : IAPIResponse
{
    /// <summary>
    /// 电影院实体
    /// </summary>
    [JsonPropertyName("cinema")] public Cinemas? Cinema { get; set; }

    /// <summary>
    /// 响应状态
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = String.Empty;
}

/// <summary>
/// 通过电影院名查询电影院响应,支持模糊查找
/// </summary>
public class GetCinemaByNameResponse : IAPIResponse
{
    /// <summary>
    /// 查询得到的电影院实体列表
    /// </summary>
    [JsonPropertyName("cinemas")] public List<Cinemas?>? Cinemas { get; set; }

    /// <summary>
    /// 响应状态
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = String.Empty;
}

/// <summary>
/// 通过特点查询电影院响应,支持模糊查找
/// </summary>
public class GetCinemaByFeatureResponse : IAPIResponse
{
    /// <summary>
    /// 查询得到的电影院实体列表
    /// </summary>
    [JsonPropertyName("cinemas")] public List<Cinemas?>? Cinemas { get; set; }

    /// <summary>
    /// 响应状态
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = String.Empty;
}

/// <summary>
/// 通过id删除对应电影院
/// </summary>
public class DeleteCinemaByIdResponse : IAPIResponse
{
    /// <summary>
    /// 响应状态
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = String.Empty;
}

