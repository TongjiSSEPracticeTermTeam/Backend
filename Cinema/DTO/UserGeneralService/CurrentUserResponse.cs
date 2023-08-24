using System.Text.Json.Serialization;

namespace Cinema.DTO.UserGeneralService
{
    /// <summary>
    /// 当前登录的用户信息
    /// </summary>
    public class CurrentUserResponse: APIResponse
    {
        /// <summary>
        /// 用户类型，以下三选一：'Customer' | 'Manager' | 'Administrator'
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 用户名
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 显示昵称
        /// </summary>
        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// 头像Url
        /// </summary>
        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; }

    }
}
