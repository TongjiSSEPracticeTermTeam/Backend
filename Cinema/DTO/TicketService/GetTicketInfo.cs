namespace Cinema.DTO.TicketService
{
    /// <summary>
    /// 用于取票的信息
    /// </summary>
    public class GetTicketInfo
    {
        /// <summary>
        /// 需要取的票
        /// </summary>
        public List<string> Tickets { get; set; } = new List<string>();

        /// <summary>
        /// 在此时间前使用无效
        /// </summary>
        public DateTime NotBefore { get; set; } = DateTime.Now;

        /// <summary>
        /// 在此时间后使用无效
        /// </summary>
        public DateTime NotAfter { get; set; } = DateTime.Now;
    }
}
