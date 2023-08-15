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
/// 通过id查询电影和对应影人响应
/// </summary>
public class GetMovieByIdwithStaffResponse : IAPIResponse
{
    /// <summary>
    /// 电影院实体
    /// </summary>
    [JsonPropertyName("movie")] public Movie? Movie { get; set; }

    /// <summary>
    /// 导演
    /// </summary>
    [JsonPropertyName("director")] public Staff? Director { get; set; }

    /// <summary>
    /// 主演
    /// </summary>
    [JsonPropertyName("actors")] public List<Staff?>? Actors { get; set; }

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
    /// 查询得到的电影实体列表
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
/// 通过特点查询电影响应，模糊查找
/// </summary>
public class GetMovieByTagsResponse : IAPIResponse
{
    /// <summary>
    /// 电影实体列表
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
/// 修改电影响应体
/// </summary>
public class UpdateMovieResponse : IAPIResponse
{
    /// <summary>
    /// 修改后电影实体
    /// </summary>
    [JsonPropertyName("movies")] public Movie? Movie { get; set; }

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
    /// 响应状态
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = String.Empty;
}