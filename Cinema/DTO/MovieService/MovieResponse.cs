using System.Text.Json.Serialization;
using Cinema.Entities;

namespace Cinema.DTO.MovieService;

/// <summary>
/// 添加电影响应体
/// </summary>
public class AddMovieResponse : IAPIResponse
{
    /// <summary>
    /// 电影实体
    /// </summary>
    [JsonPropertyName("movie")] public Movie? Movie { get; set; }

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
/// 通过id查询电影响应
/// </summary>
public class GetMovieByIdResponse : IAPIResponse
{
    /// <summary>
    /// 电影院实体
    /// </summary>
    [JsonPropertyName("movie")] public Movie? Movie { get; set; }

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
/// 通过电影名查询电影响应，模糊查找
/// </summary>
public class GetMovieByNameResponse : IAPIResponse
{
    /// <summary>
    /// 查询得到的电影院实体列表
    /// </summary>
    [JsonPropertyName("movie")] public List<Movie?>? Movies { get; set; }

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
/// 通过特点查询电影响应，模糊查找
/// </summary>
public class GetMovieByTagsResponse : IAPIResponse
{
    /// <summary>
    /// 查询得到的电影院实体列表
    /// </summary>
    [JsonPropertyName("movies")] public List<Movie?>? Movies { get; set; }

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
/// 通过id删除对应电影
/// </summary>
public class DeleteMovieByIdResponse : IAPIResponse
{
    /// <summary>
    /// 电影院实体
    /// </summary>
    [JsonPropertyName("movie")] public Movie? Movie { get; set; }

    /// <summary>
    /// 响应状态
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = String.Empty;
}