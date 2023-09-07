using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.Entities;
using Cinema.DTO.CinemaService;

namespace Cinema.DTO.ManagerService
{
    /// <summary>
    /// 影厅管理DTO
    /// </summary>
    public class HallDTO
    {
        /// <summary>
        /// hall_id 主键
        /// </summary>
        [Required]
        [JsonPropertyName("hallID")]
        public string Id { get; set; } = String.Empty;

        /// <summary>
        /// cinema_id 所属影院
        /// </summary>
        [Required]
        [JsonPropertyName("cinemaId")]
        public string CinemaId { get; set; } = String.Empty;

        /// <summary>
        /// hall_type 影厅类型
        /// </summary>
        [JsonPropertyName("hallType")]
        public string? HallType { get; set; } = String.Empty;

        /// <summary>
        /// seat 座位信息
        /// </summary>
        [JsonPropertyName("seat")]
        public Seat? Seat { get; set; } = new Seat();

        /// <summary>
        /// 默认构造
        /// </summary>
        public HallDTO()
        {
        }

        /// <summary>
        /// 由Hall实体构造
        /// </summary>
        /// <param name="entity"></param>
        public HallDTO(Hall entity)
        {
            Id = entity.Id;
            CinemaId=entity.CinemaId;
            HallType = entity.HallType;
            Seat = entity.Seat;
        }
    }
    /// <summary>
    /// 影厅添加用接口
    /// </summary>
    public class HallCreator: HallDTO
    {
        /// <summary>
        /// 管理员名称
        /// </summary>
        [Required]
        [JsonPropertyName("managerName")]
        public string ManagerName { get; set; } = String.Empty;

        /// <summary>
        /// 管理员密码
        /// </summary>
        [Required]
        [JsonPropertyName("managerPassword")]
        public string ManagerPassword { get; set; } = String.Empty;

        /// <summary>
        /// 管理员邮箱
        /// </summary>
        [Required]
        [JsonPropertyName("managerEmail")]
        public string ManagerEmail { get; set; } = String.Empty;
    }

    /// <summary>
    /// 影院经理DTO
    /// </summary>
    public class ManagerDTO
    {
        /// <summary>
        /// manager_id 主键
        /// </summary>
        [Required]
        [JsonPropertyName("managerID")]
        public string Id { get; set; } = String.Empty;

        /// <summary>
        /// name 姓名
        /// </summary>
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; } = String.Empty;

        /// <summary>
        /// password 密码
        /// </summary>
        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; } = String.Empty;

        /// <summary>
        /// email 邮箱
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; } = String.Empty;

        /// <summary>
        /// avatarUrl 头像地址
        /// </summary>
        [JsonPropertyName("avatarUrl")]
        public string? AvatarUrl { get; set; } = String.Empty;

        /// <summary>
        /// 管理的影院 
        /// </summary>
        [JsonPropertyName("managedCinema")]
        public CinemaDTO? ManagedCinema { get; set; }

        /// <summary>
        /// 默认构造
        /// </summary>
        public ManagerDTO()
        {
        }

        /// <summary>
        /// 由Manager实体构造
        /// </summary>
        /// <param name="entity"></param>
        public ManagerDTO(Manager entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Password = entity.Password;
            Email = entity.Email;
            AvatarUrl = entity.AvatarUrl;

            if(entity.ManagedCinema != null) {
                ManagedCinema = new CinemaDTO(entity.ManagedCinema);
            }
            
        }
    }

    /// <summary>
    /// 电影票DTO
    /// </summary>
    public class TicketDTO
    {
        /// <summary>
        /// 影票ID
        /// </summary>
        [Required]
        [JsonPropertyName("ticketId")]
        public string Id { get; set; } = String.Empty;

        /// <summary>
        /// 影票状态
        /// </summary>
        [JsonPropertyName("state")]
        public TicketState State { get; set; } = TicketState.normal;

        /// <summary>
        /// 票价
        /// </summary>
        [JsonPropertyName("price")]
        public float Price { get; set; }

        /// <summary>
        /// 电影编号
        /// </summary>
        [JsonPropertyName("movieId")]
        public string MovieId { get; set; } = String.Empty;

        /// <summary>
        /// 开始时间
        /// </summary>
        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 影院编号
        /// </summary>
        [JsonPropertyName("cinemaId")]
        public string CinemaId { get; set; } = String.Empty;

        /// <summary>
        /// 影厅
        /// </summary>
        [JsonPropertyName("hallId")]
        public string HallId { get; set; } = String.Empty;

        /// <summary>
        /// 导航属性 - 所属排片
        /// </summary>
        [JsonPropertyName("sessionId")]
        public Session SessionAt { get; set; } = null!;

        /// <summary>
        /// 默认构造
        /// </summary>
        public TicketDTO() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entity"></param>
        public TicketDTO(Ticket entity)
        {
            Id = entity.Id;
            State = entity.State;
            Price = entity.Price;
            MovieId = entity.MovieId;
            StartTime = entity.StartTime;
            CinemaId = entity.CinemaId;
            HallId = entity.HallId;
            SessionAt = entity.SessionAt;
        }
    }

    /// <summary>
    /// 影厅数据总览DTO
    /// </summary>
    public class OverviewDTO
    {
        /// <summary>
        /// 总体数据
        /// </summary>
        public GlobalData GlobalData { get; set; } = new GlobalData();

        /// <summary>
        /// 电影数据
        /// </summary>
        public List<CinemaMovieData> MovieDatas { get; set; } = new List<CinemaMovieData>();

        /// <summary>
        /// 默认构造
        /// </summary>
        public OverviewDTO() { }
    }

    /// <summary>
    /// 总体数据
    /// </summary>
    public class GlobalData
    {
        /// <summary>
        /// 今日影院总票房
        /// </summary>
        public double TotalBoxOfficeToday { get; set; }

        /// <summary>
        /// 今日影院总观影人次
        /// </summary>
        public int AudienceNumberToday { get; set; }

        /// <summary>
        /// 默认构造
        /// </summary>
        public GlobalData() { }
    }

    /// <summary>
    /// 电影数据
    /// </summary>
    public class CinemaMovieData
    {
        /// <summary>
        /// 电影名
        /// </summary>
        public string? MovieName { get; set; }

        /// <summary>
        /// 总票房
        /// </summary>
        public double TotalBoxOffice { get; set; }

        /// <summary>
        /// 上座率
        /// </summary>
        public float Attendance { get; set; }

        /// <summary>
        /// 观影人次
        /// </summary>
        public int AudienceNumber { get; set; }

        /// <summary>
        /// 首映日期
        /// </summary>
        public DateTime PremiereDate { get; set; }

        /// <summary>
        /// 默认构造
        /// </summary>
        public CinemaMovieData() { }
    }
}
