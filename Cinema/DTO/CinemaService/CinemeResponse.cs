using System.Text.Json.Serialization;
using Cinema.Entities;

namespace Cinema.DTO.CinemaService;

public class AddCinemaResponse : IAPIResponse
{
    [JsonPropertyName("cinema")] public Cinemas? Cinema { get; set; }

    public string Status { get; set; } = null!;
    public string Message { get; set; } = null!;
}

public class GetCinemaByIdResponse : IAPIResponse
{
    [JsonPropertyName("cinema")] public Cinemas? Cinema { get; set; }

    public string Status { get; set; } = null!;
    public string Message { get; set; } = null!;
}

public class GetCinemaByManagerIdResponse : IAPIResponse
{
    [JsonPropertyName("cinema")] public Cinemas? Cinema { get; set; }

    public string Status { get; set; } = null!;
    public string Message { get; set; } = null!;
}

public class GetCinemaByNameResponse : IAPIResponse
{
    [JsonPropertyName("cinemas")] public List<Cinemas?>? Cinemas { get; set; }

    public string Status { get; set; } = null!;
    public string Message { get; set; } = null!;
}

public class GetCinemaByFeatureResponse : IAPIResponse
{
    [JsonPropertyName("cinemas")] public List<Cinemas?>? Cinemas { get; set; }

    public string Status { get; set; } = null!;
    public string Message { get; set; } = null!;
}



