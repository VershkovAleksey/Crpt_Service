using Newtonsoft.Json;

namespace Domain.Models.Fbs.Responses;

public class GetSuppliesListResponse
{
    /// <summary>
    /// Список поставок
    /// </summary>
    [JsonProperty("supplies")]
    public List<Supplies> Supplies { get; set; } = new();
    
    /// <summary>
    /// Параметр пагинации. Содержит значение, которое необходимо указать в запросе для получения следующего пакета данных
    /// </summary>
    [JsonProperty("next")]
    public long Next { get; set; }
}

public class Supplies
{
    /// <summary>
    /// Дата закрытия поставки (RFC3339)
    /// </summary>
    [JsonProperty("closedAt")]
    public string? ClosedAt { get; set; }
    
    /// <summary>
    /// Дата сканирования поставки или первого заказа (RFC3339)
    /// </summary>
    [JsonProperty("scanDt")]
    public string? ScanDt { get; set; }
    
    [JsonProperty("rejectDt")]
    public string? RejectDt { get; set; }
    
    /// <summary>
    /// ID склада назначения поставки. Если null, склад назначения не указан
    /// </summary>
    [JsonProperty("destinationOfficeId")]
    public int DestinationOfficeId { get; set; }
    
    /// <summary>
    /// Enum: 0 1
    /// Тип поставки:
    /// 0 — внутренняя поставка
    /// 1 — трансграничная поставка
    /// null — значение отсутствует
    /// </summary>
    [JsonProperty("crossBorderType")]
    public int CrossBorderType { get; set; }
    
    /// <summary>
    /// Признак B2B-продажи:
    /// true — B2B-продажа
    /// false — не B2B-продажа
    /// null — признак отсутствует, сборочные задания не добавлены к поставке
    /// </summary>
    [JsonProperty("isB2b")]
    public bool IsB2B { get; set; }
    
    /// <summary>
    /// ID поставки
    /// </summary>
    [JsonProperty("id")]
    public required string Id { get; set; }
    
    /// <summary>
    /// Наименование поставки
    /// </summary>
    [JsonProperty("name")]
    public required string Name { get; set; }
    
    /// <summary>
    /// Дата создания поставки (RFC3339)
    /// </summary>
    [JsonProperty("createdAt")]
    public string? CreatedAt { get; set; }
    
    /// <summary>
    /// ID рекомендуемого склада для приёмки поставки для Москвы и МО.
    /// Рекомендуется ближайший к покупателям склад, который определяется автоматически при передаче поставки в доставку с учётом параметров всех сборочных заданий в поставке.
    /// Если 0, рекомендуемый склад не определён
    /// </summary>
    [JsonProperty("recommendedWhId")]
    public int RecommendedWhId { get; set; }
    
    /// <summary>
    /// Enum: 0 1 2 3
    /// Тип товара:
    /// 1 — малогабаритный товар (МГТ)
    /// 2 — сверхгабаритный товар (СГТ)
    /// 3 — крупногабаритный товар (КГТ+)
    /// </summary>
    [JsonProperty("cargoType")]
    public int CargoType { get; set; }
    
    /// <summary>
    /// Флаг закрытия поставки:
    /// true — закрыта
    /// false — открыта
    /// </summary>
    [JsonProperty("done")]
    public bool Done { get; set; }
    
    /// <summary>
    /// Можно ли отгрузить заказ на ПВЗ:
    /// false — нет
    /// true — да
    /// </summary>
    [JsonProperty("isPickupPointShipmentAllowed")]
    public bool IsPickupPointShipmentAllowed { get; set; }
}