using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Entities
{
    /// <summary>
    /// 演职员
    /// </summary>
    public class Staff
    {
        /// <summary>
        /// 演员Id
        /// </summary>
        [Required][Key][Column("STAFF_ID")] public string StaffId { get; set; } = "";
        
        /// <summary>
        /// 姓名
        /// </summary>
        [Required][Column("NAME")] public string Name { get; set; } = "";
        
        /// <summary>
        /// 性别
        /// </summary>
        [Required][Column("GENDER")] public Gender_ Gender { get; set; }
        
        /// <summary>
        /// 介绍
        /// </summary>
        [Column("INTRODUCTION")] public string? Introduction { get; set; }
        
        /// <summary>
        /// 图片链接
        /// </summary>
        [Column("IMAGE_URL")] public string? ImageUrl { get; set; }

        /// <summary>
        /// 导航属性 - 演电影（多对多）
        /// </summary>
        public ICollection<Act> Acts { get; set; } = new HashSet<Act>();

        /// <summary>
        /// 配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void ConfigureDbContext(ModelBuilder modelBuilder)
        {

        }
    }
}
