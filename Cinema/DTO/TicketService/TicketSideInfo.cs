using Cinema.Entities;

namespace Cinema.DTO.TicketService
{
    /// <summary>
    /// 影票的附加信息
    /// </summary>
    public class TicketSideInfo
    {
        /// <summary>
        /// 电影
        /// </summary>
        public Movie? Movie { get; set; }

        /// <summary>
        /// 排片
        /// </summary>
        public Session? Session { get; set; }
    }
}
