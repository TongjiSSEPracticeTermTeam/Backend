using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Cinema.Entities;

/// <summary>
///     主数据库
/// </summary>
public class CinemaDb : DbContext
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="options"></param>
    public CinemaDb(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// 客户数据表
    /// </summary>
    public DbSet<Customer> Customers { get; set; } = null!;

    /// <summary>
    /// 电影院表
    /// </summary>
    public DbSet<Cinemas> Cinemas { get; set; } = null!;

    /// <summary>
    /// 电影表
    /// </summary>
    public DbSet<Movie> Movies { get; set; } = null!;

    /// <summary>
    /// 演员表
    /// </summary>
    public DbSet<Staff> Staffs { get; set; } = null!;

    /// <summary>
    /// 电影-演员关系表
    /// </summary>
    public DbSet<Act> Acts { get; set; } = null!;

    /// <summary>
    /// 评论表
    /// </summary>
    public DbSet<Comment> Comments { get; set; } = null!;

    /// <summary>
    /// 影厅表
    /// </summary>
    public DbSet<Hall> Halls { get; set; } = null!;

    /// <summary>
    /// 头图表
    /// </summary>
    public DbSet<HeaderImage> HeaderImages { get; set; } = null!;

    /// <summary>
    /// 评论交互表
    /// </summary>
    public DbSet<Interaction> Interactions { get; set; } = null!;

    /// <summary>
    /// 影院经理表
    /// </summary>
    public DbSet<Manager> Managers { get; set; } = null!;

    /// <summary>
    /// 管理员表
    /// </summary>
    public DbSet<Administrator> Administrators { get; set; } = null!;

    /// <summary>
    /// 电影排片表
    /// </summary>
    public DbSet<Session> Sessions { get; set; } = null!;

    /// <summary>
    /// 电影排片表
    /// </summary>
    public DbSet<Ticket> Tickets { get; set; } = null!;

    /// <summary>
    /// VIP表
    /// </summary>
    public DbSet<VipInfo> VipInfos { get; set; } = null!;


    /// <summary>
    /// 初始化数据库
    /// </summary>
    /// <param name="modelBuilder">不用管</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Customer.ConfigureDbContext(modelBuilder);
        Entities.Cinemas.ConfigureDbContext(modelBuilder);
        Movie.ConfigureDbContext(modelBuilder);
        Staff.ConfigureDbContext(modelBuilder);
        Act.ConfigureDbContext(modelBuilder);
        Comment.ConfigureDbContext(modelBuilder);
        Hall.ConfigureDbContext(modelBuilder);
        HeaderImage.ConfigureDbContext(modelBuilder);
        Interaction.ConfigureDbContext(modelBuilder);
        Manager.ConfigureDbContext(modelBuilder);
        Administrator.ConfigureDbContext(modelBuilder);
        Session.ConfigureDbContext(modelBuilder);
        Ticket.ConfigureDbContext(modelBuilder);
        VipInfo.ConfigureDbContext(modelBuilder);
    }
}