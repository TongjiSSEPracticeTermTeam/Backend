using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Entities
{
    /// <summary>
    /// 影票表
    /// </summary>
    public class Ticket
    {
        /// <summary>
        /// 影票ID
        /// </summary>
        [Required]
        [Key]
        [Column("TICKET_ID")]
        public string Id { get; set; } = String.Empty;

        /// <summary>
        /// 座位（行）
        /// </summary>
        [Required]
        [Column("SEAT_ROW")]
        public int Row { get; set; }

        /// <summary>
        /// 座位（列）
        /// </summary>
        [Required]
        [Column("SEAT_COL")]
        public int Col { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        [Required]
        [Column("ORDER_TIME")]
        public DateTime OrderTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 影票状态
        /// </summary>
        [Column("STATE")]
        public TicketState State { get; set; } = TicketState.normal;

        /// <summary>
        /// 是否已经出票
        /// </summary>
        [Required]
        [Column("DRAW")]
        public int Draw { get; set; }

        /// <summary>
        /// 票价
        /// </summary>
        [Required]
        [Column("PRICE")]
        public float Price { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        [Required]
        [Column("CUSTOMER_ID")]
        public string CustomerId { get; set; } = String.Empty;

        /// <summary>
        /// 电影编号
        /// </summary>
        [Required]
        [Column("MOVIE_ID")]
        public string MovieId { get; set; } = String.Empty;

        /// <summary>
        /// 影厅编号
        /// </summary>
        [Required]
        [Column("HALL_ID")]
        public string HallId { get; set; } = String.Empty;

        /// <summary>
        /// 开始时间
        /// </summary>
        [Required]
        [Column("START_TIME")]
        public DateTime StartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 影院编号
        /// </summary>
        [Required]
        [Column("CINEMA_ID")]
        public string CinemaId { get; set; } = String.Empty;

        /// <summary>
        /// 导航属性 - 所属排片
        /// </summary>
        public Session SessionAt { get; set; } = null!;

        /// <summary>
        /// 导航属性 - 购买客户
        /// </summary>
        public Customer Buyer { get; set; } = null!;

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ConfigureDbContext(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.SessionAt)
                .WithMany(s => s.Tickets)
                .HasForeignKey(t => new { t.HallId, t.CinemaId, t.MovieId, t.StartTime });

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Buyer)
                .WithMany(b => b.Tickets)
                .HasForeignKey(t => t.CustomerId);
        }
    }
}
