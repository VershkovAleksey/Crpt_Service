using Database.Entities.Sets;

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
    
    public int SetId { get; set; }
    public SetEntity Set { get; set; }
}