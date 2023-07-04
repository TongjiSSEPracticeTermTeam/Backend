using System.Text.Json.Serialization;

namespace Cinema.DTO;

public interface IAPIResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; }
}