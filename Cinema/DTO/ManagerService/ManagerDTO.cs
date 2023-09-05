using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.Entities;

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


}
