namespace Domain.Models.Registration;

public class RegisterDto
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ApiKey { get; set; }
    
    public string? Inn { get; set; }
}