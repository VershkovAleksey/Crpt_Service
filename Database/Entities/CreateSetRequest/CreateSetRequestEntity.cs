using Database.Entities.Users;

namespace Database.Entities.CreateSetRequest;

public class CreateSetRequestEntity : EntityBase
{
    public required string SetName { get; set; }
    public required string Gtin { get; set; }
    public required int Count { get; set; }
    public required int UserId { get; set; }
    
    public string? Response { get; set; }
    
    public int Status { get; set; }
    
    public UserEntity? User { get; set; }
}