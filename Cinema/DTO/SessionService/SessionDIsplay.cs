using Cinema.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Cinema.DTO.MoviesService;

namespace Cinema.DTO.SessionService
{
    /// <summary>
    /// �ͻ���չʾ����Ƭ
    /// </summary>
    public class SessionDisplay
    {
        /// <summary>
        /// ��ӰID
        /// </summary>
        [Required]
        public string MovieId { get; set; } = String.Empty;

        /// <summary>
        /// ��Ӱ��
        /// </summary>
        [Required]
        public string MovieName{ get; set; } = String.Empty;

        /// <summary>
        /// ӰԺID
        /// </summary>
        [Required]
        public string CinemaId { get; set; } = String.Empty;

        /// <summary>
        /// Ӱ��ID
        /// </summary>
        [Required]
        public string HallId { get; set; } = String.Empty;

        /// <summary>
        /// ��ʼʱ��
        /// </summary>
        [Required]
        public DateTime StartTime { get; set; } = new DateTime();

        /// <summary>
        /// ����ʱ��
        /// </summary>
        [Required]
        public DateTime EndTime { get; set; } = new DateTime();

        /// <summary>
        /// ������
        /// </summary>
        public float? Attendence { get; set; }

        /// <summary>
        /// Ʊ��
        /// </summary>
        [Required]
        public float Price { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [Required]
        public string Language { get; set; } = String.Empty;

        /// <summary>
        /// ����ά����3d��2d��
        /// </summary>
        [Required]
        public string Dimesion { get; set; } = String.Empty;

        /// <summary>
        /// �������� - ��λ��Ӱ��
        /// </summary>
        public Hall HallLocatedAt { get; set; } = null!;

        /// <summary>
        /// Ĭ�Ϲ���
        /// </summary>
        public SessionDisplay() { }

        /// <summary>
        /// ʬ�幹��
        /// </summary>
        /// <param name="entity"></param>
        public SessionDisplay(Session entity)
        {
            MovieId = entity.MovieId;
            MovieName = entity.MovieBelongsTo.Name;
            CinemaId = entity.CinemaId;
            HallId = entity.HallId;
            StartTime = entity.StartTime;
            EndTime = entity.StartTime.AddMinutes(double.Parse(entity.MovieBelongsTo.Duration));
            Attendence = entity.Attendence;
            Price = entity.Price;
            Language = entity.Language;
            Dimesion = entity.Dimesion;
            HallLocatedAt = entity.HallLocatedAt;
        }
    }
}
