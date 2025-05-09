namespace Domain.Models.NationalCatalog.Dto;

public class Product
{
    public int? GoodId { get; set; }
    /// <summary>
    /// Код товара
    /// </summary>
    public string? Gtin { get; set; }
    /// <summary>
    /// Наименование товара
    /// </summary>
    public string? GoodName { get; set; }
    /// <summary>
    /// Наименование товарного знака
    /// </summary>
    public string? BrandName { get; set; }
}