using System.Text.Json.Serialization;

namespace Cinema.DTO.CustomerService;

/// <summary>
/// ��¼����
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// �û���
    /// </summary>
    [JsonPropertyName("username")] public string UserName { get; set; } = "";

    /// <summary>
    /// ����
    /// </summary>
    [JsonPropertyName("password")] public string Password { get; set; } = "";
}