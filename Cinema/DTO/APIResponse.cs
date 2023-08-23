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