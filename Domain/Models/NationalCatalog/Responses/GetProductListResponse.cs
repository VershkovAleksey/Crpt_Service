using Newtonsoft.Json;

namespace Domain.Models.NationalCatalog.Responses;

public class GetProductListResponse
{
    public int Apiversion { get; set; }
    public GetProductListResult? Result { get; set; }
}

public class GetProductListResult
{
    /// <summary>
    /// Максимальное количество записей в ответе
    /// </summary>
    public int? Limit { get; set; }
    
    /// <summary>
    /// Смещение относительно начала списка выдачи
    /// </summary>
    public int? Offset { get; set; }
    
    /// <summary>
    /// Общее количество записей соответствующих заданным параметрам
    /// <remarks>Не более 10000</remarks>
    /// </summary>
    public int? Total { get; set; }
    
    /// <summary>
    /// Массив карточек товаров
    /// </summary>
    public IEnumerable<Product>? Goods { get; set; }
}

public class Product
{
    /// <summary>
    /// Идентификатор карточки товара
    /// </summary>
    [JsonProperty("good_id")]
    public int? GoodId { get; set; }
    
    /// <summary>
    /// Код товара
    /// </summary>
    public string? Gtin { get; set; }
    
    /// <summary>
    /// Наименование товара
    /// </summary>
    [JsonProperty("good_name")]
    public string? GoodName { get; set; }
    
    /// <summary>
    /// Код группы ТН ВЭД
    /// </summary>
    public string? Tnved { get; set; }
    
    /// <summary>
    /// Наименование товарного знака
    /// </summary>
    [JsonProperty("brand_name")]
    public string? BrandName { get; set; }
    
    /// <summary>
    /// Технологический статус карточки товара
    /// </summary>
    [JsonProperty("good_status")]
    public string? GoodStatus { get; set; }
    
    /// <summary>
    /// Массив текущих статусов карточки товара
    /// <remarks>Возможные значения:
    /// «draft» — черновик;
    /// «moderation» — на модерации;
    /// «errors» — требует изменений;
    /// «notsigned» — ожидает подписания;
    /// «published» — опубликована</remarks>
    /// </summary>
    [JsonProperty("good_detailed_status")]
    public string[]? GoodDetailedStatus { get; set; }
    
    /// <summary>
    /// Дата последнего обновления технологического статуса карточки товара
    /// </summary>
    [JsonProperty("to_date")]
    public string? ToDate { get; set; }
}