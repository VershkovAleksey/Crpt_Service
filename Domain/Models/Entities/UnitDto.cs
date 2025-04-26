namespace Domain.Models.Entities;

public class UnitDto
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
    
    public SetDto Set { get; set; }
}