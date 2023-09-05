using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.Entities;
using Cinema.DTO.CinemaService;

namespace Cinema.DTO.ManagerService
{
    /// <summary>
    /// Ӱ������DTO
    /// </summary>
    public class HallDTO
    {
        /// <summary>
        /// hall_id ����
        /// </summary>
        [Required]
        [JsonPropertyName("hallID")]
        public string Id { get; set; } = String.Empty;

        /// <summary>
        /// cinema_id ����ӰԺ
        /// </summary>
        [Required]
        [JsonPropertyName("cinemaId")]
        public string CinemaId { get; set; } = String.Empty;

        /// <summary>
        /// hall_type Ӱ������
        /// </summary>
        [JsonPropertyName("hallType")]
        public string? HallType { get; set; } = String.Empty;

        /// <summary>
        /// seat ��λ��Ϣ
        /// </summary>
        [JsonPropertyName("seat")]
        public Seat? Seat { get; set; } = new Seat();

        /// <summary>
        /// Ĭ�Ϲ���
        /// </summary>
        public HallDTO()
        {
        }

        /// <summary>
        /// ��Hallʵ�幹��
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
    /// Ӱ������ýӿ�
    /// </summary>
    public class HallCreator: HallDTO
    {
        /// <summary>
        /// ����Ա����
        /// </summary>
        [Required]
        [JsonPropertyName("managerName")]
        public string ManagerName { get; set; } = String.Empty;

        /// <summary>
        /// ����Ա����
        /// </summary>
        [Required]
        [JsonPropertyName("managerPassword")]
        public string ManagerPassword { get; set; } = String.Empty;

        /// <summary>
        /// ����Ա����
        /// </summary>
        [Required]
        [JsonPropertyName("managerEmail")]
        public string ManagerEmail { get; set; } = String.Empty;
    }

    /// <summary>
    /// ӰԺ����DTO
    /// </summary>
    public class ManagerDTO
    {
        /// <summary>
        /// manager_id ����
        /// </summary>
        [Required]
        [JsonPropertyName("managerID")]
        public string Id { get; set; } = String.Empty;

        /// <summary>
        /// name ����
        /// </summary>
        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; } = String.Empty;

        /// <summary>
        /// password ����
        /// </summary>
        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; } = String.Empty;

        /// <summary>
        /// email ����
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; } = String.Empty;

        /// <summary>
        /// avatarUrl ͷ���ַ
        /// </summary>
        [JsonPropertyName("avatarUrl")]
        public string? AvatarUrl { get; set; } = String.Empty;

        /// <summary>
        /// �����ӰԺ 
        /// </summary>
        [JsonPropertyName("managedCinema")]
        public CinemaDTO? ManagedCinema { get; set; }

        /// <summary>
        /// Ĭ�Ϲ���
        /// </summary>
        public ManagerDTO()
        {
        }

        /// <summary>
        /// ��Managerʵ�幹��
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
    /// ��ӰƱDTO
    /// </summary>
    public class TicketDTO
    {
        /// <summary>
        /// ӰƱID
        /// </summary>
        [Required]
        [JsonPropertyName("ticketId")]
        public string Id { get; set; } = String.Empty;

        /// <summary>
        /// ӰƱ״̬
        /// </summary>
        [JsonPropertyName("state")]
        public TicketState State { get; set; } = TicketState.normal;

        /// <summary>
        /// Ʊ��
        /// </summary>
        [JsonPropertyName("price")]
        public float Price { get; set; }

        /// <summary>
        /// ��Ӱ���
        /// </summary>
        [JsonPropertyName("movieId")]
        public string MovieId { get; set; } = String.Empty;

        /// <summary>
        /// ��ʼʱ��
        /// </summary>
        [JsonPropertyName("startTime")]
        public DateTime StartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// ӰԺ���
        /// </summary>
        [JsonPropertyName("cinemaId")]
        public string CinemaId { get; set; } = String.Empty;

        /// <summary>
        /// Ӱ��
        /// </summary>
        [JsonPropertyName("hallId")]
        public string HallId { get; set; } = String.Empty;

        /// <summary>
        /// �������� - ������Ƭ
        /// </summary>
        [JsonPropertyName("sessionId")]
        public Session SessionAt { get; set; } = null!;

        /// <summary>
        /// Ĭ�Ϲ���
        /// </summary>
        public TicketDTO() { }

        /// <summary>
        /// ���캯��
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
    /// Ӱ����������DTO
    /// </summary>
    public class OverviewDTO
    {
        /// <summary>
        /// ��������
        /// </summary>
        public GlobalData GlobalData { get; set; } = new GlobalData();

        /// <summary>
        /// ��Ӱ����
        /// </summary>
        public List<CinemaMovieData> MovieDatas { get; set; } = new List<CinemaMovieData>();

        /// <summary>
        /// Ĭ�Ϲ���
        /// </summary>
        public OverviewDTO() { }
    }

    /// <summary>
    /// ��������
    /// </summary>
    public class GlobalData
    {
        /// <summary>
        /// ����ӰԺ��Ʊ��
        /// </summary>
        public double TotalBoxOfficeToday { get; set; }

        /// <summary>
        /// ����ӰԺ�ܹ�Ӱ�˴�
        /// </summary>
        public int AudienceNumberToday { get; set; }

        /// <summary>
        /// Ĭ�Ϲ���
        /// </summary>
        public GlobalData() { }
    }

    /// <summary>
    /// ��Ӱ����
    /// </summary>
    public class CinemaMovieData
    {
        /// <summary>
        /// ��Ӱ��
        /// </summary>
        public string? MovieName { get; set; }

        /// <summary>
        /// ��Ʊ��
        /// </summary>
        public double TotalBoxOffice { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public float Attendance { get; set; }

        /// <summary>
        /// ��Ӱ�˴�
        /// </summary>
        public int AudienceNumber { get; set; }

        /// <summary>
        /// ��ӳ����
        /// </summary>
        public DateTime PremiereDate { get; set; }

        /// <summary>
        /// Ĭ�Ϲ���
        /// </summary>
        public CinemaMovieData() { }
    }
}
