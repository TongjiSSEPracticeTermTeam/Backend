using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Entities;

/// <summary>
///     用户
/// </summary>
public class Customer
{
    [Required] [Column("CUSTOMER_ID")] public string CustomerId { get; set; } = null!;

    [Column("NAME")] public string? Name { get; set; }

    [Column("PASSWORD")] public string? Password { get; set; }

    [Column("EMAIL")] public string? Email { get; set; }

    [Column("AVATAR_URL")] public string? AvatarUrl { get; set; }

    public static void ConfigureDbContext(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasKey(c => c.CustomerId);
    }
}