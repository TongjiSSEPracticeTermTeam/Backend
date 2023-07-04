using Microsoft.EntityFrameworkCore;

namespace Cinema.Entities;

/// <summary>
///     主数据库
/// </summary>
public class CinemaDb : DbContext
{
    public CinemaDb(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Customer.ConfigureDbContext(modelBuilder);
    }
}