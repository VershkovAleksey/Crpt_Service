using Database.Entities.Sets;
using Database.Entities.Users;

namespace Database.Entities.Units;

public class UnitEntity : EntityBase
{
    /// <summary>
    /// Название юнита
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// GTIN
    /// </summary>
    public required string Gtin { get; set; }

    public List<int>? SetIds { get; set; }

    public int UserId { get; set; }
    
    public UserEntity? User { get; set; }
}