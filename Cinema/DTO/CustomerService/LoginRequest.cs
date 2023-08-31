using System.Text.Json.Serialization;

namespace Cinema.DTO.CustomerService;

/// <summary>
/// 登录请求
/// </summary>
public class CustomerLoginRequest
{
    /// <summary>
    /// 邮箱地址
    /// </summary>
    [JsonPropertyName("email")] public string Email { get; set; } = "";

    /// <summary>
    /// 密码
    /// </summary>
    [JsonPropertyName("password")] public string Password { get; set; } = "";
}