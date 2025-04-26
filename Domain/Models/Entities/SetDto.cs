namespace Domain.Models.Entities;

public class SetDto
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
    /// Юниты в наборе
    /// </summary>
    public ICollection<UnitDto> Units { get; set; }
}