using Database.Entities.Sets;
using Database.Entities.Units;

namespace Database.Entities.Users;

public class UserEntity : EntityBase
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? Login { get; set; }
    public string? Role { get; set; }
    public string? PhoneNumber { get; set; }
    
    public string? Inn { get; set; }

    public string? NkApiKey { get; set; }

    public ICollection<UnitEntity> Units { get; set; }

    public ICollection<SetEntity> Sets { get; set; }
}