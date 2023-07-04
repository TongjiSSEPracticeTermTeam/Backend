using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Entities;

/// <summary>
///     用户
/// </summary>
public class Customer
{
    [Required] public string CustomerId { get; set; } = null!;

    public string? Name { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public string? AvatarUrl { get; set; }

    public static void ConfigureDbContext(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasKey(c => c.CustomerId);
    }
}