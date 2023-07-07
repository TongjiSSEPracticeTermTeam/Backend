using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;


namespace Cinema.Entities;

[Table("Cinemas")]
public class Cinemas
{
    [Required] [Key] [Column("CINEMA_ID")] public string CinemaId { get; set; } = null!;
    [Required] [Column("LOCATION")] public string Location { get; set; } = null!;
    [Required] [Column("NAME")] public string Name { get; set; } = null!;
    [Required] [Column("MANAGER_ID")] public string ManagerId { get; set; } = null!;
    [Column("CINEMA_IMAGE_URL")] public string? CinemaImageUrl { get; set; }
    [Column("FEATURE")] public string? Feature { get; set; }

    public static void ConfigureDbContext(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cinemas>()
            .HasKey(c => c.CinemaId);
    }
}