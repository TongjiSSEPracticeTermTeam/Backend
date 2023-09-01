using Cinema.Entities;

namespace Cinema.DTO.CommentService
{
    /// <summary>
    /// 根据电影获取评论响应
    /// </summary>
    public class MovieCommentResponse
    {
        /// <summary>
        /// 热门评论
        /// </summary>
        public ICollection<Comment> HotComments { get; set; } = new List<Comment>();

        /// <summary>
        /// 最新评论
        /// </summary>
        public ICollection<Comment> NewComments { get; set; } = new List<Comment>();
    }
}
