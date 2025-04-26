using Database.Entities.Units;

namespace Database.Entities.Sets;

public class SetEntity : EntityBase
{
    /// <summary>
    /// Название набора
    /// </summary>
    public required string SetName { get; set; }

    /// <summary>
    /// GTIN набора
    /// </summary>
    public string Gtin { get; set; }

    /// <summary>
    /// GTIN`ы юнитов в наборе
    /// </summary>
    public ICollection<UnitEntity> Units { get; set; }
}