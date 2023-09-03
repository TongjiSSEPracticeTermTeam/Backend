using Cinema.Entities;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;

namespace Cinema.DTO.CommentService
{
    /// <summary>
    /// ����DTO������Ա��ͼ��
    /// </summary>
    public class CommentDTO
    {
        public string CommentId { get; set; } = String.Empty;

        public string Content { get; set; } = String.Empty;

        public float Score { get; set; }

        public int LikeCount { get; set; }

        public int DislikeCount { get; set; }

        public DateTime PublishDate { get; set; }

        public bool Display { get; set; }

        public string MovieId { get; set; } = String.Empty;

        public string CustomerId { get; set; } = String.Empty;

        /// <summary>
        /// �û�����
        /// </summary>
        public string? CustomerName { get; set; } = String.Empty;

        /// <summary>
        /// �û�ͷ��
        /// </summary>
        public string? AvatarUrl { get; set; } = String.Empty;

        /// <summary>
        /// Ĭ�Ϲ���
        /// </summary>
        public CommentDTO()
        {
        }

        /// <summary>
        /// ��Commentʵ�幹��
        /// </summary>
        /// <param name="comment"></param>
        public CommentDTO(Comment comment)
        {
            CommentId = comment.CommentId;
            Content = comment.Content;
            Score = comment.Score;
            LikeCount = comment.LikeCount;
            DislikeCount = comment.DislikeCount;
            Display = comment.Display;
            MovieId = comment.MovieId;
            CustomerId = comment.CustomerId;
            CustomerName = comment.Sender.Name;
            AvatarUrl = comment.Sender.AvatarUrl;
        }
    }

    public class CommentCreator
    {
        /// <summary>
        /// ��������
        /// </summary> 
        [Required]
        public string Content { get; set; } = String.Empty;

        /// <summary>
        /// ����
        /// </summary>
        [Required]
        public float Score { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime PublishDate { get; set; } = new DateTime();

        /// <summary>
        /// ��ӰID
        /// </summary>
        [Required]
        public string MovieId { get; set; } = String.Empty;

        /// <summary>
        /// �û�ID
        /// </summary>
        [Required]
        public string CustomerId { get; set; } = String.Empty;

        /// <summary>
        /// Ĭ�Ϲ���
        /// </summary>
        public CommentCreator() { }
    }
}