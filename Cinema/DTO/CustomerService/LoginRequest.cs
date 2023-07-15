using System.Text.Json.Serialization;

namespace Cinema.DTO.CustomerService;

/// <summary>
/// 登录请求
/// </summary>
public class CustomerLoginRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    [JsonPropertyName("username")] public string UserName { get; set; } = "";

    /// <summary>
    /// 密码
    /// </summary>
    [JsonPropertyName("password")] public string Password { get; set; } = "";
}