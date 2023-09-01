using System.Text.Json.Serialization;

namespace Cinema.DTO.CustomerService
{
    /// <summary>
    /// 注册请求
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
        
        /// <summary>
        /// 电子邮箱
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
    }
}
