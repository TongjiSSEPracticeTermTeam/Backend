using System.Text.Json.Serialization;

namespace Cinema.DTO;

/// <summary>
/// API返回数据的公共接口
/// </summary>
public interface IAPIResponse
{
    /// <summary>
    /// 为10000时表示成功，否则为失败
    /// </summary>
    [JsonPropertyName("status")] public string Status { get; set; }

    /// <summary>
    /// 返回给前端的信息
    /// </summary>
    [JsonPropertyName("message")] public string Message { get; set; }
}

/// <summary>
/// API返回数据的接口实现
/// </summary>
public class APIResponse : IAPIResponse
{
    /// <summary>
    /// 为10000时表示成功，否则为失败
    /// </summary>
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 返回给前端的信息
    /// </summary>
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 制作失败结果
    /// </summary>
    /// <param name="Status"></param>
    /// <param name="Message"></param>
    /// <returns></returns>
    public static APIResponse Failaure(string Status, string Message)
    {
        return new APIResponse
        {
            Status = Status,
            Message = Message
        };
    }

    /// <summary>
    /// 制作成功结果
    /// </summary>
    /// <returns></returns>
    public static APIResponse Success()
    {
        return new APIResponse
        {
            Status = "10000",
            Message = "成功"
        };
    }
}