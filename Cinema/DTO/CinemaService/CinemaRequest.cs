using System.Text.Json.Serialization;

namespace Cinema.DTO.CinemaService;

public class AddCinemaRequest
{
    [JsonPropertyName("cinema_id")] public string CinemaId { get; set; } = null!;

    [JsonPropertyName("location")] public string Location { get; set; } = null!;

    [JsonPropertyName("name")] public string Name { get; set; } = null!;

    [JsonPropertyName("manager_id")] public string ManagerId { get; set; } = null!;

    [JsonPropertyName("cinema_image_url")] public string? CinemaImageUrl { get; set; }

    [JsonPropertyName("feature")] public string? Feature { get; set; }
}