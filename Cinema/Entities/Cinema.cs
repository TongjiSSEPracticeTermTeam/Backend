using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace Cinema.Entities;

/// <summary>
/// 电影院实体类
/// </summary>
[Table("Cinemas")]
public class Cinemas
{
    /// <summary>
    /// cinema_id 主键
    /// </summary>
    [Required][Key][Column("CINEMA_ID")] public string CinemaId { get; set; } = String.Empty;

    /// <summary>
    /// 位置
    /// </summary>
    [Required][Column("LOCATION")] public string Location { get; set; } = String.Empty;

    /// <summary>
    /// 名字
    /// </summary>
    [Required][Column("NAME")] public string Name { get; set; } = String.Empty;

    /// <summary>
    /// 管理员id
    /// </summary>
    [Required][Column("MANAGER_ID")] public string ManagerId { get; set; } = String.Empty;

    /// <summary>
    /// 电影封面Url
    /// </summary>
    [Column("CINEMA_IMAGE_URL")] public string? CinemaImageUrl { get; set; }

    /// <summary>
    /// 电影特点字符串，不同特点用逗号分割
    /// </summary>
    [Column("FEATURE")] public string? Feature { get; set; }

    /// <summary>
    /// 导航属性 - 所拥有的影厅
    /// </summary>
    [JsonIgnore] public ICollection<Hall> Halls { get; set; } = new List<Hall>();

    /// <summary>
    /// 导航属性 - 影厅经理
    /// </summary>
    public Manager Manager { get; set; } = null!;

    /// <summary>
    /// 注册主键
    /// </summary>
    /// <param name="modelBuilder"></param>
    public static void ConfigureDbContext(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cinemas>()
            .HasKey(c => c.CinemaId);
    }
}