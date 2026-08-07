using Domain.Models.Fbs;

namespace WbManageBot.Models;

public class Warehouses
{
    /// <summary>
    /// Название склада продавца
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// ID склада WB
    /// </summary>
    public int OfficeId { get; set; }
    /// <summary>
    /// ID склада продавца
    /// </summary>
    public int Id { get; set; }

    public CargoType CargoType { get; set; }
    public DeliveryType DeliveryType { get; set; }
}
