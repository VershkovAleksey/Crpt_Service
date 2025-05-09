using Database.Entities.Units;
using Database.Entities.Users;
using Domain.Models.NationalCatalog;

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
    
    public SetTypeEnum SetType { get; set; }

    public int UserId { get; set; }
    
    public UserEntity? User { get; set; }
}