using Cinema.Entities;

namespace Cinema.DTO.SessionService
{
    /// <summary>
    /// 排片信息结构体
    /// </summary>
    public class SessionDetailResponse
    {
        /// <summary>
        /// 排片
        /// </summary>
        public Session Session { get; set; } = null!;

        /// <summary>
        /// 所属影片
        /// </summary>
        public Movie Movie { get; set; } = null!;
    }
}
