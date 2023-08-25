namespace Cinema.DTO.AvatarService
{
    /// <summary>
    /// 获取头像
    /// </summary>
    public class AvatarResponse: APIResponse
    {
        /// <summary>
        /// 是否有头像
        /// </summary>
        public bool HasAvatar { get; set; } = false;

        /// <summary>
        /// 头像链接
        /// </summary>
        public string? Url { get; set; }
    }
}
