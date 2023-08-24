using Cinema.Entities;
using System.Text.Json.Serialization;

namespace Cinema.DTO.CustomerService
{
    /// <summary>
    /// 注册结果
    /// </summary>
    public class RegisterResponse: APIResponse
    {
        /// <summary>
        /// 令牌
        /// </summary>
        [JsonPropertyName("token")]
        public string? Token { get; set; }

        /// <summary>
        /// 新用户的实体类数据
        /// </summary>
        [JsonPropertyName("userdata")]
        public Customer? UserData { get; set; }
    }
}
