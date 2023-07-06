using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Entities;

public class Cinemas
{
    [Required] public string CinemaId { get; set; } = null!;
    [Required] public string Location { get; set; } = null!;
    [Required] public string Name { get; set; } = null!;
    [Required] public string ManagerId { get; set; } = null!;
    public string? CinemaImageUrl { get; set; }
    public string? Feature { get; set; }

    public static void ConfigureDbContext(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cinemas>()
            .HasKey(c => c.CinemaId);
    }
}