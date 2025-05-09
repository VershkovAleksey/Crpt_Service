using System;

namespace Database.Entities;

public class EntityBase
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
}