using System.Text.Json.Serialization;
using Cinema.Entities;

namespace Cinema.DTO.AdministratorService;

/// <summary>
/// 登录结果
/// </summary>
public class AdminLoginResponse : IAPIResponse
{
    /// <summary>
    /// 用户数据
    /// </summary>
    [JsonPropertyName("userdata")] public Administrator? UserData { get; set; }

    /// <summary>
    /// 令牌，用法：加上HTTP请求头：Authorization: Bearer [令牌]（所有需要身份验证的地方都这样调用）
    /// </summary>
    [JsonPropertyName("token")] public string? Token { get; set; }

    /// <summary>
    /// 状态,10000为成功
    /// </summary>
    public string Status { get; set; } = String.Empty;

    /// <summary>
    /// 信息
    /// </summary>
    public string Message { get; set; } = String.Empty;
}