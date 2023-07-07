using Microsoft.EntityFrameworkCore;

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
    public DbSet<Cinemas> Cinemas { get; set; } = null!;

    /// <summary>
    /// 初始化数据库
    /// </summary>
    /// <param name="modelBuilder">不用管</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Customer.ConfigureDbContext(modelBuilder);
    }
}