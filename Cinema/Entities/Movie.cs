using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Entities
{
    /// <summary>
    /// 电影
    /// </summary>
    public class Movie
    {
        /// <summary>
        /// 电影ID
        /// </summary>
        [Required][Key][Column("MOVIE_ID")] public string MovieId { get; set; } = "";

        /// <summary>
        /// 名称
        /// </summary>
        [Required][Column("NAME")] public string Name { get; set; } = "";

        /// <summary>
        /// 时长
        /// </summary>
        [Required][Column("DURATION")] public string Duration { get; set; } = "";

        /// <summary>
        /// 介绍
        /// </summary>
        [Column("INSTRUCTION")] public string? Instruction { get; set; }

        /// <summary>
        /// 分数
        /// </summary>
        [Column("SCORE")] public float? Score { get; set; }

        /// <summary>
        /// 海报图片
        /// </summary>
        [Column("POST_URL")] public string? PostUrl { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        [Column("TAGS")] public string? Tags { get; set; }

        /// <summary>
        /// 上映日期
        /// </summary>
        [Required][Column("RELEASE_DATE")] public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// 到期日期
        /// </summary>
        [Required][Column("REMOVAL_DATE")] public DateTime RemovalDate { get; set; }

        /// <summary>
        /// 导航属性 - 演电影（多对多）
        /// </summary>
        public ICollection<Act> Acts { get; set; } = new HashSet<Act>();

        /// <summary>
        /// 导航属性 - 评论）
        /// </summary>
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

        /// <summary>
        /// 导航属性 - 拍片
        /// </summary>
        public ICollection<Session> Sessions { get; set; } = new List<Session>();

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ConfigureDbContext(ModelBuilder modelBuilder)
        {
            
        }
    }
}
