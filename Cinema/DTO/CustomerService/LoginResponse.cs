using System.Text.Json.Serialization;
using Cinema.Entities;

namespace Cinema.DTO.CustomerService;

public class LoginResponse : IAPIResponse
{
    [JsonPropertyName("userdata")] public Customer? UserData { get; set; }
    
    [JsonPropertyName("token")] public string? Token { get; set; }

    public string Status { get; set; } = null!;
    public string Message { get; set; } = null!;
}