using System.Text.Json.Serialization;
using Domain.Models.Fbs.Models;
using WbManageBot.Models;

namespace Domain.Models.Fbs;

public class Order
{
    /// <summary>
    /// Идентификатор сборочного задания в Маркетплейсе
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }
    /// <summary>
    /// Идентификатор сборочного задания в системе Wildberries
    /// </summary>
    [JsonPropertyName("rid")]
    public string? Rid { get; set; }
    /// <summary>
    /// Дата создания сборочного задания
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    /// <summary>
    /// Идентификатор склада продавца, на который поступило сборочное задание
    /// </summary>
    [JsonPropertyName("warehouseId")]
    public int WarehouseId { get; set; }
    /// <summary>
    /// Идентификатор поставки. Возвращается, если заказ закреплён за поставкой
    /// </summary>
    [JsonPropertyName("supplyId")]
    public string? SupplyId { get; set; }
    /// <summary>
    /// Список офисов, куда следует привезти товар
    /// </summary>
    [JsonPropertyName("offices")]
    public string[]? Offices { get; set; }
    /// <summary>
    /// Детализованный адрес покупателя для доставки (если применимо). Некоторые из полей могут прийти пустыми из-за специфики адреса
    /// </summary>
    [JsonPropertyName("address")]
    public Address? Address { get; set; }
    /// <summary>
    /// Информация о покупателе (только для доставки силами продавца)
    /// </summary>
    [JsonPropertyName("user")]
    public User User { get; set; }

    /// <summary>
    /// Массив баркодов товара
    /// </summary>
    [JsonPropertyName("skus")]
    public string[]? Skus { get; set; }
    /// <summary>
    /// Цена в валюте продажи с учетом всех скидок, умноженная на 100. Код валюты продажи в поле currencyCode.
    /// </summary>
    [JsonPropertyName("price")]
    public int Price { get; set; }
    /// <summary>
    /// Цена в валюте продажи с учетом всех скидок, сконвертированная по курсу на момент продажи в российские копейки. Предоставляется в информационных целях
    /// </summary>
    [JsonPropertyName("convertedPrice")]
    public int ConvertedPrice { get; set; }
    /// <summary>
    /// Код валюты продажи (ISO 4217)
    /// </summary>
    [JsonPropertyName("currencyCode")]
    public int CurrencyCode { get; set; }
    /// <summary>
    /// Код валюты страны продавца
    /// </summary>
    [JsonPropertyName("convertedCurrencyCode")]
    public int ConvertedCurrencyCode { get; set; }
    /// <summary>
    /// Идентификатор транзакции для группировки сборочных заданий. Сборочные задания в одной корзине покупателя будут иметь одинаковый orderUID
    /// </summary>
    [JsonPropertyName("orderUid")]
    public string? OrderUid { get; set; }
    [JsonPropertyName("deliveryType")]
    public DeliveryType DeliveryType { get; set; }
    /// <summary>
    /// Артикул WB
    /// </summary>
    [JsonPropertyName("nmid")]
    public int NmId { get; set; }
    /// <summary>
    /// Идентификатор размера товара в системе Wildberries
    /// </summary>
    [JsonPropertyName("chrtId")]
    public SizeType ChrtId { get; set; }
    /// <summary>
    /// Артикул продавца
    /// </summary>
    [JsonPropertyName("article")]
    public string? Article { get; set; }
    [JsonPropertyName("cargoType")]
    public CargoType CargoType { get; set; }

}
