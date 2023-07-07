using System.Text.Json.Serialization;

namespace Cinema.DTO;

/// <summary>
/// API接口返回的基类
/// </summary>
public interface IAPIResponse
{
    /// <summary>
    /// 为10000时表示成功，其余为失败
    /// </summary>
    [JsonPropertyName("status")] public string Status { get; set; }

    /// <summary>
    /// 返回给前端的消息
    /// </summary>
    [JsonPropertyName("message")] public string Message { get; set; }
}