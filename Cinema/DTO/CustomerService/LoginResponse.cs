using System.Text.Json.Serialization;
using Cinema.Entities;

namespace Cinema.DTO.CustomerService;

public class LoginResponse : IAPIResponse
{
    public string Status { get; set; } = null!;
    public string Message { get; set; } = null!;

    [JsonPropertyName("userdata")]
    public Customer? UserData { get; set; }
}