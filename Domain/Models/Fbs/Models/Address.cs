namespace Domain.Models.Fbs.Models;

public class Address
{
    /// <summary>
    /// Адрес доставки
    /// </summary>
    public string? FullAddress { get; set; }
    /// <summary>
    /// Область
    /// </summary>
    public string? Province { get; set; }
    /// <summary>
    /// Район
    /// </summary>
    public string? Area { get; set; }
    /// <summary>
    /// Город
    /// </summary>
    public string? City { get; set; }
    /// <summary>
    /// Улица
    /// </summary>
    public string? Street { get; set; }
    /// <summary>
    /// Номер дома
    /// </summary>
    public string? Home { get; set; }
    /// <summary>
    /// Номер квартиры
    /// </summary>
    public string? Flat { get; set; }
    /// <summary>
    /// Подъезд
    /// </summary>
    public string? Entrance { get; set; }
    /// <summary>
    /// Координата долготы
    /// </summary>
    public double Longitude { get; set; }
    /// <summary>
    /// Координаты широты
    /// </summary>
    public double Latitude { get; set; }
}
