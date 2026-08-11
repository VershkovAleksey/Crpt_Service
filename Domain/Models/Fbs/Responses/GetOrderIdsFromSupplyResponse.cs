using Newtonsoft.Json;

namespace Domain.Models.Fbs.Responses;

/// <summary>
/// Ответ на запрос о получении списка идентификаторов сборочных заданий из поставки
/// </summary>
public class GetOrderIdsFromSupplyResponse
{
    /// <summary>
    /// ID сборочных заданий
    /// </summary>
    [JsonProperty("orderIds")]
    public List<long>? OrderIds { get; set; }
}

