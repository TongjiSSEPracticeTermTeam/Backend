using System.Text.Json.Serialization;

namespace Cinema.DTO.CustomerService;

public class LoginRequest
{
    [JsonPropertyName("username")]
    public string UserName { get; set; } = "";
    
    [JsonPropertyName("password")]
    public string Password { get; set; } = "";
}